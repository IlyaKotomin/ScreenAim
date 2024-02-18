using GameOverlay.Drawing;
using GameOverlay.Windows;
using ScreenAim.ScreenSystem;

namespace ScreenAim.ScreenSystem;

internal sealed class ScreenMarker : IDisposable
{
    private const int Height = 300;
    private const int Width = (int)(Height * 2.5);
    
    private const int MaxFps = 60;
    private const int LifeTimeFrames = (int)(MaxFps * LifeTimeSeconds);
    private const double LifeTimeSeconds = 0.25;
    private int _currentLifeFrame;
    
    private readonly GraphicsWindow _window;
    
    private readonly Dictionary<string, SolidBrush> _brushes = new();
    private readonly Dictionary<string, Font> _fonts = new();
    private readonly MonitorInfo _monitorInfo;

    public ScreenMarker(MonitorInfo monitorInfo)
    {
        var (centerX, centerY) = monitorInfo.GetMonitorCenter();
        
        var gfx = new Graphics()
        {
            PerPrimitiveAntiAliasing = true,
            TextAntiAliasing = true
        };

        _window = new GraphicsWindow(centerX - Width / 2, centerY - Height / 2, Width, Height, gfx)
        {
            FPS = MaxFps,
            IsTopmost = true,
            IsVisible = true,
        };

        _monitorInfo = monitorInfo;
        _window.DestroyGraphics += _window_DestroyGraphics;
        _window.DrawGraphics += _window_DrawGraphics;
        _window.SetupGraphics += _window_SetupGraphics;
    }

    public void Run()
    {
        _window.Create();
        _window.Join();
    }
    
    private void _window_SetupGraphics(object? sender, SetupGraphicsEventArgs e)
    {
        var gfx = e.Graphics;
        
        if (e.RecreateResources)
            foreach (var pair in _brushes) pair.Value.Dispose();

        _brushes["background"] = gfx.CreateSolidBrush(255, 255, 255);
        _brushes["content"] = gfx.CreateSolidBrush(51, 51, 51);
        
        if (e.RecreateResources)
            return;
        
        _fonts["arial"] = gfx.CreateFont("Impact", 120);
    }

    private void _window_DrawGraphics(object? sender, DrawGraphicsEventArgs e)
    {
        var gfx = e.Graphics;
        var text = _monitorInfo.DeviceName;
        
        gfx.ClearScene();
        
        gfx.FillRoundedRectangle(_brushes["background"], 0, 0, Width, Height, 20);

        gfx.DrawText(_fonts["arial"], _brushes["content"], 20, 20, _monitorInfo.DeviceName);
        gfx.FillRoundedRectangle(_brushes["content"], 20, Height - 100, GetProgressBar, Height - 20, 10);
        
        if(ScreenManager.GetCurrentMonitor().DeviceName != _monitorInfo.DeviceName) Dispose();
        
        if (_currentLifeFrame++ >= LifeTimeFrames) Dispose();
    }

    private int GetProgressBar => (Width - 20) - (int)((Width - 40) * ((double)_currentLifeFrame / LifeTimeFrames));

    private void _window_DestroyGraphics(object? sender, DestroyGraphicsEventArgs e)
    {
        foreach (var pair in _brushes) pair.Value.Dispose();
        foreach (var pair in _fonts) pair.Value.Dispose();
    }

    
    
    ~ScreenMarker() => Dispose(_disposedValue);

    #region IDisposable Support
    private bool _disposedValue;

    private void Dispose(bool disposing)
    {
        if (disposing) return;
        _window.Hide();
        _window.Dispose();
        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(_disposedValue);
        GC.SuppressFinalize(this);
    }
    #endregion
}