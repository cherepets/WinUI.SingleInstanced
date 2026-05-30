using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WinUI.SingleInstanced
{
    /// <summary>
    /// Manages instances of WinUI application.
    /// </summary>
    /// <param name="instanceKey">A non-empty string as a key for the instance.</param>
    /// <param name="createApp">Function that creates an App when it's requested.</param>
    /// <param name="onAppActivated">Callback that is triggered when the app is activated.</param>
    public class Manager(string instanceKey, Func<Application> createApp, EventHandler<AppActivationArguments> onAppActivated)
    {
        private IntPtr _redirectEventHandle = IntPtr.Zero;

        /// <summary>
        /// Called on every app launch to handle instance creation.
        /// </summary>
        public void Activate()
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();

            var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var instance = AppInstance.FindOrRegisterForKey(instanceKey);
            if (!DecideRedirection(instance, activationArgs))
            {
                Application.Start(p =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        syncContext: new DispatcherQueueSynchronizationContext(
                            dispatcherQueue: DispatcherQueue.GetForCurrentThread()));
                    createApp();
                    onAppActivated(instance, activationArgs);
                });
            }
        }

        private bool DecideRedirection(AppInstance instance, AppActivationArguments args)
        {
            if (instance.IsCurrent)
            {
                instance.Activated += (s, e) => onAppActivated(s, e);
                return false;
            }
            else
            {
                RedirectActivationTo(instance, args);
                return true;
            }
        }

        private void RedirectActivationTo(AppInstance keyInstance, AppActivationArguments args)
        {
            _redirectEventHandle = Win32.Kernel32.CreateEvent(IntPtr.Zero, true, false, null);
            Task.Run(() =>
            {
                keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
                Win32.Kernel32.SetEvent(_redirectEventHandle);
            });

            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            _ = Win32.Ole32.CoWaitForMultipleObjects(
               CWMO_DEFAULT, INFINITE, 1,
               [_redirectEventHandle], out uint handleIndex);

            Win32.User32.SetForegroundWindow(
                Process.GetProcessById((int)keyInstance.ProcessId)
                    .MainWindowHandle);
        }
    }
}
