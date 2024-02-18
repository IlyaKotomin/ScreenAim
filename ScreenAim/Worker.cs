using ScreenAim.ScreenSystem;
using SharpHook;
using SharpHook.Native;

namespace ScreenAim;

public class Worker(KeyCode key1, KeyCode key2)
{
    private readonly TaskPoolGlobalHook _inputHook = new();

    private bool BindPressed => _key1Pressed && _key2Pressed;
    
    private bool _key1Pressed;
    private bool _key2Pressed;
    
    internal async Task RunAsync()
    {
        Console.WriteLine("Hooking input");
        
        _inputHook.KeyPressed += InputHookOnKeyPressed;
        _inputHook.KeyReleased += InputHookOnKeyReleased;

        await _inputHook.RunAsync();
        await Task.CompletedTask;
    }

    //I use Thread instead of Tasks because that method faster to create and update forms
    private static void OnBindPressed()
    {
        ScreenManager.GeToNext(out var nextMonitor);
        new Thread(() => new ScreenMarker(nextMonitor).Run()).Start();
    }

    private void InputHookOnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        var key = e.Data.KeyCode;

        if (key == key1)
            _key1Pressed = true;
        else if (key == key2)
            _key2Pressed = true;
        
        if (BindPressed) OnBindPressed();
    }

    private void InputHookOnKeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        var key = e.Data.KeyCode;
        
        if (key == key1)
            _key1Pressed = false;
        else if (key == key2)
            _key2Pressed = false;
    }
}