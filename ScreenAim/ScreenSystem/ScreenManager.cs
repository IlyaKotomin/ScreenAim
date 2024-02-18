using System.Runtime.InteropServices;
using ScreenAim.WinApi;
using static ScreenAim.WinApi.Win32;

namespace ScreenAim.ScreenSystem;

public static class ScreenManager
{
    private static List<MonitorInfo> Monitors => GetMonitors();

    public static void GeToNext(out MonitorInfo nextMonitor)
    {
        var currentMonitor = GetCurrentMonitor();

        var currentIndex = Monitors.FindIndex(i => i.DeviceName == currentMonitor.DeviceName);
        var nextIndex = currentIndex != (Monitors.Count - 1) ? currentIndex + 1 : 0;
        nextMonitor = Monitors[nextIndex];

        var (centerX, centerY) = nextMonitor.GetMonitorCenter();
        SetCursorPos(centerX, centerY);
    }

    public static MonitorInfo GetCurrentMonitor()
    {
        if (!GetCursorPos(out var cursorPos))
            throw new Exception("Failed to get cursor position");

        var hMonitor = MonitorFromPoint(cursorPos, 0);
        if (hMonitor == IntPtr.Zero)
            throw new Exception("Failed to get monitor handle");

        var monitorInfo = new Win32Structs.MonitorInfoEx
        {
            cbSize = Marshal.SizeOf(typeof(Win32Structs.MonitorInfoEx))
        };
        
        if (!GetMonitorInfo(hMonitor, ref monitorInfo))
            throw new Exception("Failed to get monitor info");

        return new MonitorInfo(deviceName: monitorInfo.szDevice, monitorRect: monitorInfo.rcMonitor,
            workRect: monitorInfo.rcWork);
    }

    private static List<MonitorInfo> GetMonitors()
    {
        var monitors = new List<MonitorInfo>();

        if (!EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, EnumMonitorsDelegateCallback, IntPtr.Zero))
            throw new Exception("Failed to enumerate monitors.");

        return monitors;

        bool EnumMonitorsDelegateCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref Win32Structs.Rect lprcMonitor, IntPtr dwData)
        {
            var monitorInfo = new Win32Structs.MonitorInfoEx(cbSize: Marshal.SizeOf(typeof(Win32Structs.MonitorInfoEx)));

            if (!GetMonitorInfo(hMonitor, ref monitorInfo)) return true;
            
            var info = new MonitorInfo(monitorInfo.szDevice, monitorInfo.rcMonitor, monitorInfo.rcWork);

            monitors.Add(info);
            return true;
        }
    }
}