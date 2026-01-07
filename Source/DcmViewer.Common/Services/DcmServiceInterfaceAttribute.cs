namespace DcmViewer.Common.Services;

[System.AttributeUsage(System.AttributeTargets.Interface)]
public class DcmServiceInterfaceAttribute : System.Attribute
{
    public string ServiceName { get; }
    public string ExecutableName { get; }
    public int Port { get; }

    public DcmServiceInterfaceAttribute(string serviceName, string executableName, int port)
    {
        ServiceName = serviceName;
        ExecutableName = executableName;
        Port = port;
    }

    public static DcmServiceInterfaceAttribute? Get<TTarget>()
    {
        return (DcmServiceInterfaceAttribute?)System.Attribute.GetCustomAttribute(typeof(TTarget), typeof(DcmServiceInterfaceAttribute));
    }
}