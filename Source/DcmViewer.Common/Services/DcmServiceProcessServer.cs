using System.Diagnostics;

namespace DcmViewer.Common.Services;

public class DcmServiceProcessServer<TService> : DcmServiceServer<TService>
    where TService : IDcmService
{
    private Process? _process;
    private readonly bool _enableConsole;
    private readonly bool _closeProcessOnDispose;

    public DcmServiceProcessServer(bool enableConsole = false, bool closeProcessOnDispose = false)
    {
        _closeProcessOnDispose = closeProcessOnDispose;
        _enableConsole = true;
    }

    public override async Task Start()
    {
        await Task.Run(() =>
        {
            _process = Process.GetProcessesByName(ServiceAttribute.ExecutableName).FirstOrDefault();

            if (_process is null)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(ServiceAttribute.ExecutableName)
                {
                    Arguments = _enableConsole ? "--console" : string.Empty,
                    UseShellExecute = false
                };

                _process = Process.Start(startInfo);
            }

            if (_process?.HasExited ?? false)
                throw new InvalidOperationException($"Process {ServiceAttribute.ExecutableName} is exited");
        });
    }

    public override async Task Stop()
        => await Task.CompletedTask;

    public override void Dispose()
    {
        if (_closeProcessOnDispose && _process is { HasExited: false })
            _process.Kill();
    }
}