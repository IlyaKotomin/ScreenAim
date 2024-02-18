using ScreenAim;
using SharpHook.Native;

Task.Run(async () => await new Worker(KeyCode.VcLeftShift, KeyCode.VcLeftAlt).RunAsync());

Thread.Sleep(-1);