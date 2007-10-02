using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Markup;

[assembly : XmlnsDefinition("http://schemas.kingsmill.com/kingsmill/windows", "Kingsmill.Windows")]

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Persists a Window's Size, Location and WindowState to UserScopeSettings 
    /// </summary>
    public class WindowSettings
    {
        #region WindowApplicationSettings Helper Class

        public class WindowApplicationSettings : ApplicationSettingsBase
        {
            private WindowSettings windowSettings;

            public WindowApplicationSettings(WindowSettings windowSettings)
                : base(windowSettings.window.PersistId.ToString())
            {
                this.windowSettings = windowSettings;
            }

            [UserScopedSetting]
            public Rect Location
            {
                get
                {
                    if (this["Location"] != null)
                    {
                        return ((Rect) this["Location"]);
                    }
                    return Rect.Empty;
                }
                set
                {
                    this["Location"] = value;
                }
            }

            [UserScopedSetting]
            public WindowState WindowState
            {
                get
                {
                    if (this["WindowState"] != null)
                    {
                        return (WindowState) this["WindowState"];
                    }
                    return WindowState.Normal;
                }
                set
                {
                    this["WindowState"] = value;
                }
            }
        }

        #endregion

        #region Constructor

        private readonly Window window = null;

        public WindowSettings(Window window)
        {
            this.window = window;
        }

        #endregion

        #region Attached "Save" Property Implementation 

        /// <summary>
        /// Register the "Save" attached property and the "OnSaveInvalidated" callback 
        /// </summary>
        public static readonly DependencyProperty SaveProperty
            = DependencyProperty.RegisterAttached("Save", typeof (bool), typeof (WindowSettings),
                                                  new FrameworkPropertyMetadata(
                                                      new PropertyChangedCallback(OnSaveInvalidated)));

        public static void SetSave(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(SaveProperty, enabled);
        }

        /// <summary>
        /// Called when Save is changed on an object.
        /// </summary>
        private static void OnSaveInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Window window = dependencyObject as Window;
            if (window != null)
            {
                if ((bool) e.NewValue)
                {
                    WindowSettings settings = new WindowSettings(window);
                    settings.Attach();
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load the Window Size Location and State from the settings object
        /// </summary>
        protected virtual void LoadWindowState()
        {
            Settings.Reload();
            window.WindowState = Settings.WindowState;
            if (Settings.Location != Rect.Empty)
            {
                window.Left = Settings.Location.Left;
                window.Top = Settings.Location.Top;
                window.Width = Settings.Location.Width;
                window.Height = Settings.Location.Height;
            }
        }

        /// <summary>
        /// Save the Window Size, Location and State to the settings object
        /// </summary>
        protected virtual void SaveWindowState()
        {
            Settings.WindowState = window.WindowState;
            Settings.Location = window.RestoreBounds;
            Settings.Save();
        }

        #endregion

        #region Private Methods

        private void Attach()
        {
            if (window != null)
            {
                window.Closing += window_Closing;
                window.Initialized += window_Initialized;
            }
        }

        private void window_Initialized(object sender, EventArgs e)
        {
            LoadWindowState();
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            SaveWindowState();
        }

        #endregion

        #region Settings Property Implementation

        private WindowApplicationSettings windowApplicationSettings = null;

        [Browsable(false)]
        public WindowApplicationSettings Settings
        {
            get
            {
                if (windowApplicationSettings == null)
                {
                    windowApplicationSettings = CreateWindowApplicationSettingsInstance();
                }
                return windowApplicationSettings;
            }
        }

        protected virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
        {
            return new WindowApplicationSettings(this);
        }

        #endregion
    }
}