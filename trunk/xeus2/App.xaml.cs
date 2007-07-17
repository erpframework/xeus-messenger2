using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using xeus.Data;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;

namespace xeus2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const DispatcherPriority _dispatcherPriority = DispatcherPriority.Background;

        public App()
        {
            xeus2.Properties.Resources.Culture = new CultureInfo( "en-US" ) ;
            // string h = xeus2.Properties.Resources.Event_PresenceChange ;

            Console.WriteLine("RCL: {0}", (RenderCapability.Tier >> 16));

            ErrorPopup.Instance.Init();

            Database.OpenDatabase();
            Database.LoadMucMarks();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Database.CloseDatabase();
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