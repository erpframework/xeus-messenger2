using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Brushes=System.Windows.Media.Brushes;

namespace xeus2.xeus.UI
{
    internal class Vista
    {
        [DllImport("DwmApi.dll", PreserveSig = false)]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        public static bool IsComposition
        {
            get
            {
                return (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled());
            }
        }

        public static void MakeVistaFrame(Window window, int actualHeight)
        {
            try
            {
                if (!IsComposition)
                {
                    // No glass 
                    return;
                }

                // Obtain the window handle for WPF application
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);

                window.Background = Brushes.Transparent;

                mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                // Get System Dpi
                Graphics desktop = Graphics.FromHwnd(mainWindowPtr);
                float DesktopDpiX = desktop.DpiX;
                //float DesktopDpiY = desktop.DpiY;

                // Set Margins
                MARGINS margins = new MARGINS();

                // Extend glass frame into client area
                // Note that the default desktop Dpi is 96dpi. The  margins are
                // adjusted for the system Dpi.
                margins.cxLeftWidth = Convert.ToInt32(5 * (DesktopDpiX / 96));
                margins.cxRightWidth = Convert.ToInt32(5 * (DesktopDpiX / 96));
                margins.cyTopHeight = Convert.ToInt32((actualHeight + 5) * (DesktopDpiX / 96));
                margins.cyBottomHeight = Convert.ToInt32(5 * (DesktopDpiX / 96));

                int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                //
                if (hr < 0)
                {
                    //DwmExtendFrameIntoClientArea Failed
                }
            }
                // If not Vista, paint background white.
            catch (DllNotFoundException)
            {
                // Application.Current.MainWindow.Background = Brushes.White;
            }
        }

        #region Nested type: MARGINS

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth; // width of left border that retains its size
            public int cxRightWidth; // width of right border that retains its size
            public int cyTopHeight; // height of top border that retains its size
            public int cyBottomHeight; // height of bottom border that retains its size
        } ;

        #endregion
    }
}