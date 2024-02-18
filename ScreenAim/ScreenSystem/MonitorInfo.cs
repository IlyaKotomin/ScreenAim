using ScreenAim.WinApi;

namespace ScreenAim.ScreenSystem;

public class MonitorInfo(string? deviceName, Win32Structs.Rect monitorRect, Win32Structs.Rect workRect)
{
    public string? DeviceName { get; set; } = deviceName;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public Win32Structs.Rect MonitorRect { get; set; } = monitorRect;
    public Win32Structs.Rect WorkRect { get; set; } = workRect;

    public (int centerX, int centerY) GetMonitorCenter()
    {
        var centerX = MonitorRect.Left + (MonitorRect.Right - MonitorRect.Left) / 2;
        var centerY = MonitorRect.Top + (MonitorRect.Bottom - MonitorRect.Top) / 2;
        return (centerX, centerY);
    }
}