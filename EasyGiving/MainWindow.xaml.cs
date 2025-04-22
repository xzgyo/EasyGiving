using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using System.Runtime.InteropServices;
using Windows.Graphics.Display;
using WinRT.Interop;
using System.Diagnostics;

namespace EasyGiving
{
    public sealed partial class MainWindow : Window
    {
        public static MainWindow m_mainWindow;
        public static AppWindow m_appWindow;
        public string appName = "Give指令生成器";//Windows.ApplicationModel.Package.Current.DisplayName;
        [DllImport("User32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);
        public MainWindow()
        {
            this.InitializeComponent();

            this.Activated += MainWindow_Activated;

            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().First();
            ContentFrame.Navigate(typeof(Views.HomePage), null, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

            SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.BaseAlt };
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            NativeWindowHelper.ForceDisableMaximize(this);
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;

            m_mainWindow = this;
            m_appWindow = this.AppWindow;
            NavigationViewControl.Header = null;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(@"Assets\Images\CmdBlock.png");

            // 获取窗口句柄
            IntPtr hwnd = WindowNative.GetWindowHandle(this);

            // 获取DPI值
            uint dpi = GetDpiForWindow(hwnd);
            double scaleFactor = dpi / 96.0;
            System.Diagnostics.Debug.Write($"缩放比例: {scaleFactor}\n");
            int WinWidth = Convert.ToInt32(Math.Floor(685 * scaleFactor));
            int WinHeight = Convert.ToInt32(Math.Floor(570 * scaleFactor));
            try
            {
                m_appWindow.Resize(new Windows.Graphics.SizeInt32(WinWidth, WinHeight));
            }
            catch (Exception ex)
            {
                Debug.Write(ex + "\n");
            }
        }

        private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true) ContentFrame.Navigate(typeof(Views.SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            else if (args.InvokedItemContainer != null && (args.InvokedItemContainer.Tag != null)) ContentFrame.Navigate(Type.GetType(args.InvokedItemContainer.Tag.ToString()), null, args.RecommendedNavigationTransitionInfo);
        }

        private void NavigationViewControl_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack) ContentFrame.GoBack();
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavigationViewControl.IsBackEnabled = ContentFrame.CanGoBack;
            if (ContentFrame.SourcePageType == typeof(Views.SettingsPage)) NavigationViewControl.SelectedItem = (NavigationViewItem)NavigationViewControl.SettingsItem;
            else if (ContentFrame.SourcePageType != null) NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().First(n => n.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
            //NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();
        }
    }
}
