using System.Net.Sockets;
using DcmViewer.Common.Config;

namespace DcmViewer.ListenerService.Implementation;

public class TcpFileReceiver : IDisposable
{
    private readonly DcmViewerConfig _config;
    private readonly CancellationTokenSource _cts = new();

    private Task? listenerTask;

    public bool IsRunning => listenerTask is not null && !listenerTask.IsCompleted;

    public event Action<ListenerEvent>? EventOccurred;
    public event Action<TransmissionCompletedEvent>? TransmissionCompleted;


    public TcpFileReceiver(DcmViewerConfig config)
    {
        _config = config;
    }

    public async Task Start()
    {
        if (IsRunning)
            return;

        listenerTask = Task.Run(async () =>
        {
            TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any, _config.TcpFileReceiverPort);

            try
            {
                EnsureDirectoryExists(_config.FullDcmDirectory);

                tcpListener.Start();
                OnEventOccurred(ListenerEventType.Started);

                CancellationToken ct = _cts.Token;

                while (!ct.IsCancellationRequested)
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync(ct);
                    ReadFileToEndAndSave(client, DateTime.Now, ct);
                }

                tcpListener.Stop();
                OnEventOccurred(ListenerEventType.Stopped);
            }
            catch (OperationCanceledException cancelledException)
            {
                OnEventOccurred(ListenerEventType.Error, "DICOM Listener cancellation request received");
            }
            catch (Exception ex)
            {
                OnEventOccurred(ListenerEventType.Error, $"Error in DICOM Listener: {ex.Message}");
            }
            finally
            {
                tcpListener.Dispose();
            }
        });
    }

    private void EnsureDirectoryExists(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public async Task Stop()
    {
        if (IsRunning)
        {
            await _cts.CancelAsync();
            await listenerTask!;
        }
    }

    private async Task ReadFileToEndAndSave(TcpClient client, DateTime transmissionStartedAt, CancellationToken ct)
    {
        try
        {
            string fileName = $"{transmissionStartedAt:yy-MM-dd-HH-mm-ss-ffff}.dcm";
            string filePath = System.IO.Path.Combine(_config.FullDcmDirectory, fileName);
            await using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
            await using NetworkStream networkStream = client.GetStream();

            await networkStream.CopyToAsync(fileStream, ct);
            long fileSize = fileStream.Length;

            await fileStream.FlushAsync(ct);
            fileStream.Close();

            OnTransmissionCompleted(transmissionStartedAt, fileName, fileSize);
        }
        catch (Exception ex)
        {
            OnEventOccurred(ListenerEventType.Error, $"Error receiving DICOM file: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    protected virtual void OnEventOccurred(ListenerEventType et, string message = "")
    {
        EventOccurred?.Invoke(new ListenerEvent()
        {
            EventType = et,
            Message = message
        });
    }

    protected virtual void OnTransmissionCompleted(DateTime startTime, string filePath, long fileSize)
    {
        TransmissionCompleted?.Invoke(new TransmissionCompletedEvent
        {
            TransmissionId = DateTime.Now.Ticks,
            StartTime = startTime,
            FileName = filePath,
            FileSize = fileSize
        });
    }

    public void Dispose()
    {
        Stop().Wait();
    }
}
