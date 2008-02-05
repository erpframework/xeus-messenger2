using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using xeus2.Properties;
using xeus2.xeus.Core;
using xeus2.xeus.Data;
using xeus2.xeus.Middle;
using xeus2.xeus.UI;

namespace xeus2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const DispatcherPriority _dispatcherPriority = DispatcherPriority.ContextIdle;
       

        public App()
        {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

            xeus2.Properties.Resources.Culture = new CultureInfo("en-US");

            //Console.WriteLine("RCL: {0}", (RenderCapability.Tier >> 16));

            NotificationPopup.Instance.Initialize(new InfoPopup());

            Database.OpenDatabase();

        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
           throw e.Exception;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            RecentItems.Instance.SaveItems();

            Database.CloseDatabase();
            Settings.Default.Save();

            base.OnExit(e);
        }

        public static bool CheckAccessSafe()
        {
            if (Current == null)
            {
                return false;
            }

            return Current.Dispatcher.CheckAccess();
        }

        public static void InvokeSafe(DispatcherPriority priority, Delegate method)
        {
            if (Current == null)
            {
                return;
            }

            Current.Dispatcher.Invoke(priority, method);
        }

        public static void InvokeSafe(DispatcherPriority priority, Delegate method, object arg)
        {
            if (Current == null)
            {
                return;
            }

            Current.Dispatcher.Invoke(priority, method, arg);
        }

        public static void InvokeSafe(DispatcherPriority priority, Delegate method, object arg, params object[] args)
        {
            if (Current == null)
            {
                return;
            }

            Current.Dispatcher.Invoke(priority, method, arg, args);
        }

        public static void InvokeSafe(DispatcherPriority priority, Delegate method, params object[] args)
        {
            if (Current == null)
            {
                return;
            }

            Current.Dispatcher.Invoke(priority, method, args);
        }      
    }
}