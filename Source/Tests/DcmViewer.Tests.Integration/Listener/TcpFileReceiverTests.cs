using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DcmViewer.ListenerService;
using DcmViewer.ListenerService.Implementation;

namespace DcmViewer.Tests.Integration.Listener;

[Trait("Category", "Integration")]
public class TcpFileReceiverTests : TestBase
{
    private TcpFileReceiver Receiver { get; }

    public TcpFileReceiverTests()
    {
        Receiver = new TcpFileReceiver(Config);
    }

    [Fact]
    public async Task TcpFileReceiver_StartsAndStopsSuccessfully_InSequence()
    {
        for (int attempt = 1; attempt <= 5; attempt++)
        {
            await Receiver.Start();
            Assert.True(Receiver.IsRunning);

            await Receiver.Stop();
            Assert.False(Receiver.IsRunning);
        }
    }

    [Fact]
    public async Task TcpFileReceiver_SavesReceivedContent_ToFileWithTimestamp()
    {
        Receiver.Start();

        int fileLength = 1024 * 1024;
        byte[] testData = new byte[fileLength];
        new Random().NextBytes(testData);

        ManualResetEvent transmissionReceivedEvent = new(false);
        TransmissionCompletedEvent? receivedEvent = null;
        Receiver.TransmissionCompleted += (evt) =>
        {
            receivedEvent = evt;
            transmissionReceivedEvent.Set();
        };

        await Receiver.Start();
        TcpBytesSender sender = new(IPAddress.Loopback, Config.TcpFileReceiverPort);
        await sender.SendBytesAsync(testData);

        bool signaled = transmissionReceivedEvent.WaitOne(TimeSpan.FromSeconds(10));
        Assert.True(signaled);
        Assert.NotNull(receivedEvent);
        Assert.Equal(fileLength, receivedEvent!.FileSize);
        string receivedFilePath = Path.Combine(Config.FullDcmDirectory, receivedEvent.FileName);
        Assert.True(File.Exists(receivedFilePath));

        byte[] savedData = await File.ReadAllBytesAsync(receivedFilePath);
        Assert.Equal(testData, savedData);

        await Receiver.Stop();
    }

    //ToDo: Add test for parallel file uploads

    public override void Dispose()
    {
        Receiver.Dispose();
        base.Dispose();
    }

    private class TcpBytesSender
    {
        private readonly IPAddress _host;
        private readonly int _port;

        public TcpBytesSender(IPAddress host, int port)
        {
            _host = host;
            _port = port;
        }
        public async Task SendBytesAsync(byte[] data)
        {
            using TcpClient client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            using NetworkStream stream = client.GetStream();
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }
    }
}
