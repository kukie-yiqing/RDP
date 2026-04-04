using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace RdpLauncher
{
    [DataContract]
    public class ConnectionEntry
    {
        [DataMember] public string Host { get; set; }
        [DataMember] public string Username { get; set; }
        [DataMember] public string EncryptedPass { get; set; }
        [DataMember] public int DisplayModeIndex { get; set; }
        [DataMember] public int ResolutionIndex { get; set; }
        [DataMember] public int ColorDepthIndex { get; set; }
        public string DisplayName { get { return string.IsNullOrEmpty(this.Username) ? this.Host : this.Host + " (" + this.Username + ")"; } }
        public override string ToString() { return this.Host; }
    }

    public class App : System.Windows.Application
    {
        private const string ICON_B64 = "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";

        private Window mainWindow;
        private ComboBox ipBox;
        private System.Windows.Controls.TextBox userBox;
        private PasswordBox passBox;
        private ComboBox displayModeBox;
        private ComboBox resolutionBox;
        private ComboBox colorDepthBox;
        private Grid rootGrid;
        private StackPanel mainStack;
        private System.Windows.Controls.Button connectBtn;
        private Ellipse avatarCircle;
        private Border guideBox;
        private Border shareIcon;
        private TextBlock subTitleTb;
        private Process activeMstsc;
        private DispatcherTimer statusTimer;
        private ObservableCollection<ConnectionEntry> connections = new ObservableCollection<ConnectionEntry>();
        private List<Border> fieldsBorders = new List<Border>();
        private List<TextBlock> labels = new List<TextBlock>();
        private string configDir;
        private string configPath;
        private string avatarPath;
        private DispatcherTimer themeTimer;
        private bool isConnecting = false;
        private bool isAutoFilling = false;
        private Forms.NotifyIcon trayIcon;
        private Window stickyWindow;

        [STAThread]
        public static void Main()
        {
            bool res;
            using (System.Threading.Mutex m = new System.Threading.Mutex(true, "RemoteDesk_Mutex_Final", out res))
            {
                if (!res) return;
                try { App a = new App(); a.Run(); } catch { }
            }
        }

        public App()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.configDir = System.IO.Path.Combine(appData, "RemoteDesk");
            if (!System.IO.Directory.Exists(this.configDir)) {
                string oldDir = System.IO.Path.Combine(appData, "OpenClow");
                if (System.IO.Directory.Exists(oldDir)) { try { System.IO.Directory.Move(oldDir, this.configDir); } catch { } }
                else { System.IO.Directory.CreateDirectory(this.configDir); }
            }
            this.configPath = System.IO.Path.Combine(this.configDir, "connections.json");
            this.avatarPath = System.IO.Path.Combine(this.configDir, "avatar.png");
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(OnException);
        }

        private void OnException(object s, DispatcherUnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show("Error: " + e.Exception.Message);
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.LoadConnections();
            this.CreateUI();
            this.CreateTrayIcon();
            this.CreateStickyWindow();
            this.themeTimer = new DispatcherTimer();
            this.themeTimer.Interval = TimeSpan.FromSeconds(1.5);
            this.themeTimer.Tick += new EventHandler(OnThemeTick);
            this.themeTimer.Start();
            if (this.connections.Count > 0) this.ipBox.SelectedIndex = 0;
        }

        private void OnThemeTick(object s, EventArgs ev) { this.UpdateThemeColor(); }

        private void CreateTrayIcon() {
            this.trayIcon = new Forms.NotifyIcon();
            string exePath = Assembly.GetExecutingAssembly().Location;
            string dir = System.IO.Path.GetDirectoryName(exePath);
            string icoPath = System.IO.Path.Combine(dir, "app.ico");
            if (System.IO.File.Exists(icoPath)) {
                try { 
                    Drawing.Icon loadedIcon = new Drawing.Icon(icoPath);
                    IntPtr h = loadedIcon.Handle; // Will throw here if the icon format is invalid
                    this.trayIcon.Icon = loadedIcon; 
                } catch { 
                    this.trayIcon.Icon = Drawing.SystemIcons.Application; 
                }
            } else {
                this.trayIcon.Icon = Drawing.SystemIcons.Application;
            }
            this.trayIcon.Text = "Remote Desk";
            try { this.trayIcon.Visible = true; } catch { }
            this.trayIcon.DoubleClick += (s, e) => { this.RestoreMainWindow(); };
            
            Forms.ContextMenuStrip menu = new Forms.ContextMenuStrip();
            menu.Items.Add("打开主界面", null, (s, e) => { this.RestoreMainWindow(); });
            menu.Items.Add("退出", null, (s, e) => { Forms.Application.Exit(); Environment.Exit(0); });
            this.trayIcon.ContextMenuStrip = menu;
        }

        private void CreateStickyWindow() {
            this.stickyWindow = new Window();
            this.stickyWindow.Title = "RemoteDesk Mini";
            this.stickyWindow.Width = 60; this.stickyWindow.Height = 60;
            this.stickyWindow.WindowStyle = WindowStyle.None;
            this.stickyWindow.AllowsTransparency = true;
            this.stickyWindow.Background = System.Windows.Media.Brushes.Transparent;
            this.stickyWindow.Topmost = true;
            this.stickyWindow.ShowInTaskbar = false;

            Border ball = new Border();
            ball.Width = 48; ball.Height = 48;
            ball.CornerRadius = new CornerRadius(24);
            ball.Background = System.Windows.Media.Brushes.Transparent;
            ball.BorderThickness = new Thickness(0);
            ball.Cursor = System.Windows.Input.Cursors.Hand;
            ball.Effect = new DropShadowEffect() { BlurRadius = 10, ShadowDepth = 0, Opacity = 0.5 };
            
            System.Windows.Controls.Image icon = new System.Windows.Controls.Image(); icon.Source = this.CreateSvgDrawing(System.Windows.Media.Color.FromRgb(2, 248, 233)); icon.Width = 40; icon.Height = 40;
            ball.Child = icon;
            this.stickyWindow.Content = ball;

            ball.MouseLeftButtonDown += (s, e) => { 
                if (e.ClickCount == 2) this.RestoreMainWindow(); 
                else {
                    this.stickyWindow.DragMove();
                    double curLeft = this.stickyWindow.Left + this.stickyWindow.Width / 2;
                    double screenMid = SystemParameters.PrimaryScreenWidth / 2;
                    this.stickyWindow.Left = curLeft < screenMid ? 0 : SystemParameters.PrimaryScreenWidth - this.stickyWindow.Width;
                }
            };
            
            System.Windows.Controls.Primitives.Popup popup = new System.Windows.Controls.Primitives.Popup();
            popup.StaysOpen = true;
            popup.AllowsTransparency = true;
            popup.PlacementTarget = ball;
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Left;
            popup.HorizontalOffset = -5;

            Border popupBorder = new Border();
            popupBorder.SetResourceReference(Border.BackgroundProperty, "PopupBg");
            popupBorder.SetResourceReference(Border.BorderBrushProperty, "PopupBorder");
            popupBorder.BorderThickness = new Thickness(1);
            popupBorder.CornerRadius = new CornerRadius(5);
            popupBorder.Padding = new Thickness(10);
            popupBorder.Effect = new DropShadowEffect() { BlurRadius = 10, ShadowDepth = 0, Opacity = 0.2 };

            StackPanel ttp = new StackPanel() { Margin = new Thickness(0) };
            
            this.CreateLabelInPanel(ttp, "快速模式");
            ComboBox miniMode = new ComboBox() { Width = 150, Margin = new Thickness(0, 0, 0, 5) };
            miniMode.Items.Add("📺 全部显示器"); miniMode.Items.Add("🖥️ 仅主显示器"); miniMode.Items.Add("💻 仅副显示器");
            miniMode.SelectedIndex = 0;
            miniMode.SelectionChanged += (s, e) => { if (this.displayModeBox != null) this.displayModeBox.SelectedIndex = miniMode.SelectedIndex; };
            ttp.Children.Add(miniMode);

            System.Windows.Controls.Button reconnect = new System.Windows.Controls.Button() { Content = "重新连接", Height = 28 };
            reconnect.Click += (s, e) => { this.StartConnection(); popup.IsOpen = false; };
            ttp.Children.Add(reconnect);

            System.Windows.Controls.Button restore = new System.Windows.Controls.Button() { Content = "返回界面", Height = 28, Margin = new Thickness(0, 5, 0, 0) };
            restore.Click += (s, e) => { this.RestoreMainWindow(); popup.IsOpen = false; };
            ttp.Children.Add(restore);

            popupBorder.Child = ttp;
            popup.Child = popupBorder;

            DispatcherTimer closeTimer = new DispatcherTimer();
            closeTimer.Interval = TimeSpan.FromMilliseconds(400);
            closeTimer.Tick += (s, e) => {
                closeTimer.Stop();
                if (!ball.IsMouseOver && !popupBorder.IsMouseOver) popup.IsOpen = false;
            };

            ball.MouseEnter += (s, e) => { closeTimer.Stop(); popup.IsOpen = true; };
            ball.MouseLeave += (s, e) => { closeTimer.Start(); };
            popupBorder.MouseEnter += (s, e) => { closeTimer.Stop(); };
            popupBorder.MouseLeave += (s, e) => { closeTimer.Start(); };

            double screenW = SystemParameters.PrimaryScreenWidth;
            double screenH = SystemParameters.PrimaryScreenHeight;
            this.stickyWindow.Left = screenW - 80;
            this.stickyWindow.Top = screenH - 150;
            this.stickyWindow.Hide();
        }

        private void CreateLabelInPanel(StackPanel p, string t) {
            TextBlock tb = new TextBlock() { Text = t, Margin = new Thickness(0, 0, 0, 5), FontWeight = FontWeights.Bold };
            p.Children.Add(tb);
        }

        private void RestoreMainWindow() {
            if (this.mainWindow != null) {
                this.mainWindow.Visibility = Visibility.Visible;
                this.mainWindow.WindowState = WindowState.Normal;
                this.mainWindow.Activate();
                if (this.stickyWindow != null) this.stickyWindow.Hide();
            }
        }

        private void CreateUI()
        {
            this.mainWindow = new Window();
            this.mainWindow.Title = "Remote Desk RDP Launcher";
            try { this.mainWindow.Icon = this.GetImg(ICON_B64); } catch { }
            this.mainWindow.Width = 400;
            this.mainWindow.Height = 760;
            this.mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.mainWindow.ResizeMode = ResizeMode.CanMinimize;
            this.mainWindow.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            
            this.mainWindow.Closing += (s, e) => {
                e.Cancel = true;
                this.mainWindow.Visibility = Visibility.Collapsed;
                if (this.stickyWindow != null) this.stickyWindow.Show();
            };

            this.rootGrid = new Grid();
            this.mainWindow.Content = this.rootGrid;

            this.mainStack = new StackPanel();
            this.mainStack.Margin = new Thickness(25, 15, 25, 15);
            this.rootGrid.Children.Add(this.mainStack);

            this.avatarCircle = new Ellipse();
            this.avatarCircle.Width = 70;
            this.avatarCircle.Height = 70;
            this.avatarCircle.Margin = new Thickness(0, 15, 0, 10);
            this.avatarCircle.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.avatarCircle.Cursor = System.Windows.Input.Cursors.Hand;
            this.avatarCircle.StrokeThickness = 2;
            
            DropShadowEffect shadow = new DropShadowEffect();
            shadow.BlurRadius = 30; shadow.ShadowDepth = 0; shadow.Opacity = 0.4; shadow.Color = Colors.Black;
            this.avatarCircle.Effect = shadow;
            RenderOptions.SetBitmapScalingMode(this.avatarCircle, BitmapScalingMode.HighQuality);
            this.avatarCircle.MouseLeftButtonDown += new MouseButtonEventHandler(OnAvatarClick);
            this.UpdateAvatarDisplay(this.avatarCircle);
            this.mainStack.Children.Add(this.avatarCircle);

            StackPanel hv = new StackPanel();
            hv.Orientation = System.Windows.Controls.Orientation.Horizontal;
            hv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            hv.Margin = new Thickness(0, 5, 0, 3);
            this.mainStack.Children.Add(hv);

            this.guideBox = new Border();
            this.guideBox.Width = 26; this.guideBox.Height = 26;
            this.guideBox.CornerRadius = new CornerRadius(6);
            this.guideBox.BorderThickness = new Thickness(1);
            this.guideBox.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 128, 128, 128));
            this.guideBox.Background = System.Windows.Media.Brushes.Transparent;
            this.guideBox.Cursor = System.Windows.Input.Cursors.Hand;
            this.guideBox.ToolTip = "打开配置目录";
            this.guideBox.MouseLeftButtonDown += (s, e) => { try { Process.Start("explorer.exe", this.configDir); } catch { } };
            this.guideBox.MouseEnter += (s, e) => { this.guideBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 0, 229, 255)); this.guideBox.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 0, 229, 255)); };
            this.guideBox.MouseLeave += (s, e) => { this.guideBox.Background = System.Windows.Media.Brushes.Transparent; this.guideBox.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 128, 128, 128)); };
            
            System.Windows.Controls.Image gImg = new System.Windows.Controls.Image(); gImg.Source = this.CreateSvgDrawing(System.Windows.Media.Color.FromRgb(2, 248, 233)); gImg.Width = 16; gImg.Height = 16;
            this.guideBox.Child = gImg;
            hv.Children.Add(this.guideBox);

            TextBlock hostT = new TextBlock();
            hostT.Text = "Remote Desk";
            hostT.FontSize = 20;
            hostT.FontWeight = FontWeights.ExtraBold;
            hostT.VerticalAlignment = VerticalAlignment.Center;
            hostT.Margin = new Thickness(12, 0, 12, 0);
            this.labels.Add(hostT);
            hv.Children.Add(hostT);

            this.shareIcon = new Border();
            this.shareIcon.Width = 26; this.shareIcon.Height = 26;
            this.shareIcon.CornerRadius = new CornerRadius(6);
            this.shareIcon.BorderThickness = new Thickness(1);
            this.shareIcon.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 128, 128, 128));
            this.shareIcon.Background = System.Windows.Media.Brushes.Transparent; 
            this.shareIcon.Cursor = System.Windows.Input.Cursors.Hand;
            this.shareIcon.ToolTip = "打开共享文件夹 (\\ip)";
            this.shareIcon.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnShareClick);
            this.shareIcon.MouseEnter += (s, ev) => { this.shareIcon.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 0, 229, 255)); this.shareIcon.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 0, 229, 255)); };
            this.shareIcon.MouseLeave += (s, ev) => { this.shareIcon.Background = System.Windows.Media.Brushes.Transparent; this.shareIcon.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 128, 128, 128)); };

            System.Windows.Shapes.Path sPath = new System.Windows.Shapes.Path();
            sPath.Data = Geometry.Parse("M19,10 L5,10 L9,6 M5,10 L9,14 M5,18 L19,18 L15,14 M19,18 L15,22");
            sPath.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 229, 255)); 
            sPath.StrokeThickness = 2.2;
            sPath.StrokeStartLineCap = PenLineCap.Round; sPath.StrokeEndLineCap = PenLineCap.Round;
            
            Viewbox sv = new Viewbox(); sv.Width = 16; sv.Height = 16; sv.Child = sPath;
            this.shareIcon.Child = sv;
            hv.Children.Add(this.shareIcon);

            this.subTitleTb = new TextBlock();
            this.subTitleTb.Text = "极速、一键直连远程桌面";
            this.subTitleTb.FontSize = 11;
            this.subTitleTb.Margin = new Thickness(0, -2, 0, 8);
            this.subTitleTb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.labels.Add(this.subTitleTb);
            this.mainStack.Children.Add(this.subTitleTb);

            this.ipBox = this.CreateModernDropdown(this.mainStack, "远程主机 IP");
            this.userBox = this.CreateModernField(this.mainStack, "用户名");
            this.userBox.TextChanged += new TextChangedEventHandler(OnUserTextChanged);
            
            this.CreateLabel(this.mainStack, "登录密码");
            Border pb = new Border();
            pb.CornerRadius = new CornerRadius(6); pb.BorderThickness = new Thickness(1.2); pb.Height = 36; pb.Padding = new Thickness(8, 0, 8, 0); pb.Margin = new Thickness(0, 0, 0, 10);
            this.fieldsBorders.Add(pb);
            this.passBox = new PasswordBox();
            this.passBox.Background = System.Windows.Media.Brushes.Transparent; this.passBox.BorderThickness = new Thickness(0); this.passBox.VerticalContentAlignment = VerticalAlignment.Center;
            pb.Child = this.passBox; this.mainStack.Children.Add(pb);

            this.CreateLabel(this.mainStack, "显示模式");
            Border mb = new Border();
            mb.CornerRadius = new CornerRadius(6); mb.BorderThickness = new Thickness(1.2); mb.Height = 36; mb.Margin = new Thickness(0, 0, 0, 10); mb.Padding = new Thickness(0);
            this.fieldsBorders.Add(mb);
            this.displayModeBox = new ComboBox();
            this.displayModeBox.Style = this.GetModernComboStyle();
            this.displayModeBox.IsEditable = true; this.displayModeBox.IsReadOnly = true; this.displayModeBox.Background = System.Windows.Media.Brushes.Transparent; this.displayModeBox.BorderThickness = new Thickness(0); this.displayModeBox.VerticalContentAlignment = VerticalAlignment.Center; this.displayModeBox.FontSize = 14;
            this.displayModeBox.ItemContainerStyle = this.GetComboItemStyle();
            this.displayModeBox.Items.Add("📺 全部显示器"); this.displayModeBox.Items.Add("🖥️ 仅主显示器"); this.displayModeBox.Items.Add("💻 仅副显示器"); this.displayModeBox.SelectedIndex = 0;
            mb.Child = this.displayModeBox; this.mainStack.Children.Add(mb);

            this.CreateLabel(this.mainStack, "分辨率");
            Border rb = new Border();
            rb.CornerRadius = new CornerRadius(6); rb.BorderThickness = new Thickness(1.2); rb.Height = 36; rb.Margin = new Thickness(0, 0, 0, 10); rb.Padding = new Thickness(0);
            this.fieldsBorders.Add(rb);
            this.resolutionBox = new ComboBox();
            this.resolutionBox.Style = this.GetModernComboStyle();
            this.resolutionBox.IsEditable = true; this.resolutionBox.IsReadOnly = true; this.resolutionBox.Background = System.Windows.Media.Brushes.Transparent; this.resolutionBox.BorderThickness = new Thickness(0); this.resolutionBox.VerticalContentAlignment = VerticalAlignment.Center; this.resolutionBox.FontSize = 14;
            this.resolutionBox.ItemContainerStyle = this.GetComboItemStyle();
            this.resolutionBox.Items.Add("📐 自适应全屏"); this.resolutionBox.Items.Add("📐 3840 x 2160 (4K)"); this.resolutionBox.Items.Add("📐 2560 x 1440 (2K)"); this.resolutionBox.Items.Add("📐 1920 x 1080"); this.resolutionBox.Items.Add("📐 1366 x 768"); this.resolutionBox.Items.Add("📐 1280 x 720"); this.resolutionBox.Items.Add("📐 1024 x 768"); this.resolutionBox.SelectedIndex = 0;
            rb.Child = this.resolutionBox; this.mainStack.Children.Add(rb);

            this.CreateLabel(this.mainStack, "色彩深度");
            Border cb = new Border();
            cb.CornerRadius = new CornerRadius(6); cb.BorderThickness = new Thickness(1.2); cb.Height = 36; cb.Margin = new Thickness(0, 0, 0, 10); cb.Padding = new Thickness(0);
            this.fieldsBorders.Add(cb);
            this.colorDepthBox = new ComboBox();
            this.colorDepthBox.Style = this.GetModernComboStyle();
            this.colorDepthBox.IsEditable = true; this.colorDepthBox.IsReadOnly = true; this.colorDepthBox.Background = System.Windows.Media.Brushes.Transparent; this.colorDepthBox.BorderThickness = new Thickness(0); this.colorDepthBox.VerticalContentAlignment = VerticalAlignment.Center; this.colorDepthBox.FontSize = 14;
            this.colorDepthBox.ItemContainerStyle = this.GetComboItemStyle();
            this.colorDepthBox.Items.Add("🔴 最高质量 (32位)"); this.colorDepthBox.Items.Add("🟢 真彩色 (24位)"); this.colorDepthBox.Items.Add("🔵 增强色 (16位)"); this.colorDepthBox.SelectedIndex = 0;
            cb.Child = this.colorDepthBox; this.mainStack.Children.Add(cb);
            
            this.connectBtn = new System.Windows.Controls.Button();
            this.connectBtn.Content = "立 即 连 接";
            this.connectBtn.Height = 44; this.connectBtn.Margin = new Thickness(0, 10, 0, 10); this.connectBtn.Foreground = System.Windows.Media.Brushes.White; this.connectBtn.FontSize = 16; this.connectBtn.FontWeight = FontWeights.Bold; this.connectBtn.Cursor = System.Windows.Input.Cursors.Hand;
            this.connectBtn.Click += new RoutedEventHandler(OnConnectClick);
            this.connectBtn.Style = this.CreateButtonStyle();
            DropShadowEffect btnShadow = new DropShadowEffect(); btnShadow.BlurRadius = 15; btnShadow.ShadowDepth = 3; btnShadow.Opacity = 0.4; btnShadow.Color = Colors.Black;
            this.connectBtn.Effect = btnShadow; this.mainStack.Children.Add(this.connectBtn);

            this.statusTimer = new DispatcherTimer();
            this.statusTimer.Interval = TimeSpan.FromSeconds(1);
            this.statusTimer.Tick += new EventHandler(OnStatusTick);
            this.statusTimer.Start();
            this.mainWindow.Show();
        }

        private void OnAvatarClick(object s, MouseButtonEventArgs ev) { this.UploadAvatarClick(s, ev); }
        private void OnShareClick(object s, MouseButtonEventArgs ev) {
            string h = this.ipBox.Text.Trim();
            if (string.IsNullOrEmpty(h)) return;
            try { Process.Start("explorer.exe", "\\\\" + h.Split(':')[0]); } catch { }
        }
        private void OnConnectClick(object s, RoutedEventArgs ev) { this.StartConnection(); }
        private void OnStatusTick(object s, EventArgs ev) { this.UpdateConnectionStatus(); }

        private BitmapSource GetImg(string b) { 
            try { 
                using (Stream rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoteDesk.app_icon.png")) {
                    if (rs != null) {
                        BitmapImage i = new BitmapImage(); i.BeginInit(); i.StreamSource = rs; i.CacheOption = BitmapCacheOption.OnLoad; i.EndInit(); return i; 
                    }
                }
                byte[] data = Convert.FromBase64String(b); 
                using (MemoryStream ms = new MemoryStream(data)) {
                    BitmapImage i = new BitmapImage(); i.BeginInit(); i.StreamSource = ms; i.CacheOption = BitmapCacheOption.OnLoad; i.EndInit(); return i; 
                }
            } catch { return null; } 
        }

        private void UpdateThemeColor() {
            try {
                Drawing.Color a = this.GetAccentColor(); bool d = !this.IsLight(); 
                System.Windows.Media.Color wColor = System.Windows.Media.Color.FromArgb(a.A, a.R, a.G, a.B);
                Brush ab = new SolidColorBrush(wColor);
                if (this.mainWindow != null) { if (d) this.mainWindow.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 20, 22)); else this.mainWindow.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 243, 245)); }
                if (this.labels != null) { foreach (TextBlock tb in this.labels) tb.Foreground = ab; }
                if (this.fieldsBorders != null) { 
                    foreach (Border b in this.fieldsBorders) { 
                        if (d) b.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 40)); else b.Background = System.Windows.Media.Brushes.White; 
                        if (d) b.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 50, 60)); else b.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200)); 
                    } 
                }
                if (System.Windows.Application.Current != null) {
                    System.Windows.Application.Current.Resources["PopupBg"] = d ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 40)) : System.Windows.Media.Brushes.White;
                    System.Windows.Application.Current.Resources["PopupBorder"] = d ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 50, 60)) : new SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 220, 220));
                }
                if (this.ipBox != null) this.ipBox.Foreground = ab; 
                if (this.userBox != null) this.userBox.Foreground = ab;
                if (this.passBox != null) this.passBox.Foreground = ab;
                if (this.displayModeBox != null) this.displayModeBox.Foreground = ab;
                if (this.resolutionBox != null) this.resolutionBox.Foreground = ab;
                if (this.colorDepthBox != null) this.colorDepthBox.Foreground = ab;
                LinearGradientBrush grad = new LinearGradientBrush(); grad.GradientStops.Add(new GradientStop(wColor, 0.0)); grad.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb((byte)Math.Min(255, wColor.R+40), (byte)Math.Min(255, wColor.G+40), (byte)Math.Min(255, wColor.B+40)), 1.0));
                if (this.connectBtn != null) this.connectBtn.Background = grad; 
                if (this.avatarCircle != null) this.avatarCircle.Stroke = ab;
            } catch {}
        }

        private bool IsLight() { try { object v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1); return v != null && (int)v == 1; } catch { return false; } }
        private Drawing.Color GetAccentColor() { 
            object v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null); 
            if (v is int) { int abgr = (int)v; return Drawing.Color.FromArgb((byte)(abgr & 0xFF), (byte)((abgr >> 8) & 0xFF), (byte)((abgr >> 16) & 0xFF)); } 
            return Drawing.Color.FromArgb(0, 120, 215); 
        }

        private void CreateLabel(StackPanel p, string t) { TextBlock tb = new TextBlock(); tb.Text = t; tb.Margin = new Thickness(0, 5, 0, 5); tb.FontWeight = FontWeights.SemiBold; tb.FontSize = 14; this.labels.Add(tb); p.Children.Add(tb); }

        private System.Windows.Controls.TextBox CreateModernField(StackPanel p, string l) { 
            this.CreateLabel(p, l); 
            Border h = new Border(); h.CornerRadius = new CornerRadius(6); h.BorderThickness = new Thickness(1.2); h.Height = 36; h.Padding = new Thickness(10, 0, 10, 0); h.Margin = new Thickness(0, 0, 0, 10); this.fieldsBorders.Add(h); 
            System.Windows.Controls.TextBox box = new System.Windows.Controls.TextBox(); box.Background = System.Windows.Media.Brushes.Transparent; box.BorderThickness = new Thickness(0); box.VerticalContentAlignment = VerticalAlignment.Center; box.FontSize = 14; h.Child = box; p.Children.Add(h); return box; 
        }

        private ComboBox CreateModernDropdown(StackPanel p, string l) { 
            this.CreateLabel(p, l); 
            Border bh = new Border(); bh.CornerRadius = new CornerRadius(6); bh.BorderThickness = new Thickness(1.2); bh.Height = 36; bh.Padding = new Thickness(0); bh.Margin = new Thickness(0, 0, 0, 10); this.fieldsBorders.Add(bh); 
            ComboBox box = new ComboBox(); box.IsEditable = true; box.Background = System.Windows.Media.Brushes.Transparent; box.BorderThickness = new Thickness(0); box.VerticalContentAlignment = VerticalAlignment.Center; box.FontSize = 14; box.ItemsSource = this.connections; box.ItemTemplate = this.CreateItemTemplate(); 
            box.ItemContainerStyle = this.GetComboItemStyle(); box.Style = this.GetModernComboStyle();
            System.Windows.Controls.TextSearch.SetTextPath(box, "Host"); 
            box.SelectionChanged += new SelectionChangedEventHandler(OnIpChanged); 
            box.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler(OnIpTextChanged));
            bh.Child = box; p.Children.Add(bh); return box; 
        }

        private void OnIpTextChanged(object s, TextChangedEventArgs e) {
            if (this.isAutoFilling) return;
            if (!this.ipBox.IsKeyboardFocusWithin) return;
            if (this.ipBox.SelectedItem != null) return;

            this.isAutoFilling = true;
            this.userBox.Text = "";
            this.passBox.Password = "";
            this.isAutoFilling = false;
        }

        private void OnUserTextChanged(object s, TextChangedEventArgs e) {
            if (this.isAutoFilling) return;
            if (!this.userBox.IsKeyboardFocusWithin) return;

            this.isAutoFilling = true;
            this.passBox.Password = "";
            this.isAutoFilling = false;
        }

        private void OnIpChanged(object s, SelectionChangedEventArgs ev) { 
            ComboBox box = (ComboBox)s;
            ConnectionEntry en = box.SelectedItem as ConnectionEntry; 
            if (en != null) { 
                this.isAutoFilling = true;
                this.userBox.Text = en.Username; 
                this.passBox.Password = this.Decrypt(en.EncryptedPass); 
                this.displayModeBox.SelectedIndex = en.DisplayModeIndex;
                this.resolutionBox.SelectedIndex = en.ResolutionIndex;
                this.colorDepthBox.SelectedIndex = en.ColorDepthIndex;
                this.isAutoFilling = false;
                if (System.Windows.Application.Current != null) System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => { 
                    this.isAutoFilling = true;
                    box.Text = en.Host; 
                    this.isAutoFilling = false;
                }), DispatcherPriority.ContextIdle);
            } 
        }

        private Style GetModernComboStyle() {
            string x = @"<Style xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' TargetType='ComboBox'>
    <Setter Property='Template'>
        <Setter.Value>
            <ControlTemplate TargetType='ComboBox'>
                <Grid>
                    <ToggleButton x:Name='ToggleButton' Focusable='false' IsChecked='{Binding Path=IsDropDownOpen,Mode=TwoWay,RelativeSource={RelativeSource TemplatedParent}}' ClickMode='Press'>
                        <ToggleButton.Template>
                            <ControlTemplate TargetType='ToggleButton'>
                                <Border Background='Transparent' BorderThickness='0'>
                                    <Path x:Name='Arrow' Fill='{Binding Foreground, RelativeSource={RelativeSource FindAncestor, AncestorType=ComboBox}}' HorizontalAlignment='Right' VerticalAlignment='Center' Margin='0,0,10,0' Data='M 0 0 L 4 4 L 8 0 Z'/>
                                </Border>
                            </ControlTemplate>
                        </ToggleButton.Template>
                    </ToggleButton>
                    <TextBox x:Name='PART_EditableTextBox' HorizontalAlignment='Stretch' VerticalAlignment='Center' Margin='10,0,30,0' Focusable='True' Background='Transparent' BorderThickness='0' IsReadOnly='{TemplateBinding IsReadOnly}' Foreground='{TemplateBinding Foreground}' FontSize='{TemplateBinding FontSize}'/>
                    <Popup Name='Popup' Placement='Bottom' IsOpen='{TemplateBinding IsDropDownOpen}' AllowsTransparency='True' Focusable='False' PopupAnimation='Slide'>
                        <Grid Name='DropDown' SnapsToDevicePixels='True' MinWidth='{TemplateBinding ActualWidth}' MaxHeight='{TemplateBinding MaxDropDownHeight}'>
                            <Border x:Name='DropDownBorder' Background='{DynamicResource PopupBg}' BorderThickness='1.2' BorderBrush='{DynamicResource PopupBorder}' CornerRadius='6' Margin='0,4,0,15'>
                                <Border.Effect>
                                    <DropShadowEffect BlurRadius='15' ShadowDepth='3' Opacity='0.15' Color='Black'/>
                                </Border.Effect>
                                <ScrollViewer Margin='0,4,0,4' SnapsToDevicePixels='True'>
                                    <StackPanel IsItemsHost='True' KeyboardNavigation.DirectionalNavigation='Contained' />
                                </ScrollViewer>
                            </Border>
                        </Grid>
                    </Popup>
                </Grid>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>";
            return (Style)System.Windows.Markup.XamlReader.Parse(x);
        }

        private Style GetComboItemStyle() {
            Style s = new Style(typeof(ComboBoxItem));
            s.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(10, 8, 10, 8)));
            s.Setters.Add(new Setter(FrameworkElement.CursorProperty, System.Windows.Input.Cursors.Hand));
            return s;
        }

        private DataTemplate CreateItemTemplate() { 
            DataTemplate t = new DataTemplate(); 
            FrameworkElementFactory dp = new FrameworkElementFactory(typeof(DockPanel)); 
            dp.SetValue(DockPanel.LastChildFillProperty, true); 
            dp.SetValue(FrameworkElement.WidthProperty, 300.0); 
            t.VisualTree = dp; 
            
            FrameworkElementFactory btn = new FrameworkElementFactory(typeof(System.Windows.Controls.Button)); 
            btn.SetValue(DockPanel.DockProperty, System.Windows.Controls.Dock.Right); 
            btn.SetValue(System.Windows.Controls.Button.ContentProperty, "✕"); 
            btn.SetValue(System.Windows.Controls.Button.ForegroundProperty, System.Windows.Media.Brushes.Gray);
            btn.SetValue(System.Windows.Controls.Button.CursorProperty, System.Windows.Input.Cursors.Hand);
            btn.SetValue(System.Windows.Controls.Button.ToolTipProperty, "移除此记录");
            
            Style flatBtnStyle = new Style(typeof(System.Windows.Controls.Button)); 
            ControlTemplate ft = new ControlTemplate(typeof(System.Windows.Controls.Button)); 
            FrameworkElementFactory fb = new FrameworkElementFactory(typeof(Border)); 
            fb.SetValue(Border.BackgroundProperty, System.Windows.Media.Brushes.Transparent); 
            fb.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            FrameworkElementFactory fc = new FrameworkElementFactory(typeof(ContentPresenter)); 
            fc.SetValue(ContentPresenter.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center); 
            fc.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center); 
            fc.SetValue(FrameworkElement.MarginProperty, new Thickness(8, 2, 8, 2));
            fb.AppendChild(fc); ft.VisualTree = fb; 
            flatBtnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.TemplateProperty, ft)); 
            btn.SetValue(System.Windows.Controls.Button.StyleProperty, flatBtnStyle);
            
            btn.AddHandler(System.Windows.Controls.Button.ClickEvent, new RoutedEventHandler(this.DeleteAccount)); 
            dp.AppendChild(btn); 
            
            FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBlock)); 
            System.Windows.Data.Binding bnd = new System.Windows.Data.Binding("DisplayName");
            txt.SetBinding(TextBlock.TextProperty, bnd); 
            txt.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center); 
            txt.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 0, 0));
            dp.AppendChild(txt); 
            
            return t; 
        }

        private Style CreateButtonStyle() { 
            Style sw = new Style(typeof(System.Windows.Controls.Button)); ControlTemplate tw = new ControlTemplate(typeof(System.Windows.Controls.Button)); FrameworkElementFactory bw = new FrameworkElementFactory(typeof(Border)); bw.SetValue(Border.CornerRadiusProperty, new CornerRadius(10)); bw.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(System.Windows.Controls.Button.BackgroundProperty)); FrameworkElementFactory cw = new FrameworkElementFactory(typeof(ContentPresenter)); cw.SetValue(ContentPresenter.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center); cw.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center); bw.AppendChild(cw); tw.VisualTree = bw; sw.Setters.Add(new Setter(System.Windows.Controls.Button.TemplateProperty, tw)); return sw; 
        }

        private void UpdateConnectionStatus() { if (this.isConnecting) this.connectBtn.Content = "正在连接中..."; else this.connectBtn.Content = "立 即 连 接"; }

        private string Encrypt(string t) { if (string.IsNullOrEmpty(t)) return null; byte[] d = Encoding.UTF8.GetBytes(t); byte[] enc = ProtectedData.Protect(d, null, DataProtectionScope.CurrentUser); return Convert.ToBase64String(enc); }
        private string Decrypt(string t) { if (string.IsNullOrEmpty(t)) return ""; byte[] d = Convert.FromBase64String(t); try { byte[] dec = ProtectedData.Unprotect(d, null, DataProtectionScope.CurrentUser); return Encoding.UTF8.GetString(dec); } catch { return ""; } }

        private DrawingImage CreateSvgDrawing(System.Windows.Media.Color color) {
            GeometryDrawing gd = new GeometryDrawing();
            gd.Geometry = Geometry.Parse("M840.838602 147.210649H182.479867c-48.899834 0-88.428619 41.232612-88.428619 92.177038v395.799002c0 50.774043 39.528785 92.177038 88.428619 92.177038h291.865557v84.339434h-162.715474c-11.415641 0-20.616306 12.949085-20.616306 29.135441v12.949085c0 16.015973 9.200666 29.135441 20.616306 29.135441H707.9401c11.415641 0 20.616306-12.949085 20.616306-29.135441v-12.949085c0-16.015973-9.200666-29.135441-20.616306-29.135441h-157.603994V727.53411h290.672879c48.899834 0 88.428619-41.232612 88.428619-92.177038V239.387687c-0.340765-50.774043-40.039933-92.177038-88.599002-92.177038zM645.409651 587.649917c-7.326456 7.156073-19.082862 7.156073-26.750084-0.170383L432.260899 405.340433v125.231281c0 10.052579-8.689517 18.230948-18.742097 18.230948-10.393344 0-18.742097-8.519135-18.742097-18.230948v-161.352413c0-1.363062 0.170383-2.726123 0.511149-3.918802-0.340765-1.192679-0.511148-2.55574-0.511149-3.918802 0-10.052579 8.689517-18.230948 18.742097-18.230948h165.441597c10.393344 0 18.742097 8.519135 18.742097 18.230948 0 10.052579-8.689517 18.230948-18.742097 18.230949h-120.290183l186.569052 182.139101c7.496839 7.156073 7.326456 18.912479 0.170383 25.89817z");
            gd.Brush = new SolidColorBrush(color);
            return new DrawingImage(gd);
        }

        private void UpdateAvatarDisplay(Ellipse e) { 
            if (System.IO.File.Exists(this.avatarPath)) { 
                try { 
                    BitmapImage b = new BitmapImage(); b.BeginInit(); b.UriSource = new Uri(this.avatarPath); b.CacheOption = BitmapCacheOption.OnLoad; b.EndInit(); 
                    e.Fill = new ImageBrush(b) { Stretch = Stretch.UniformToFill }; 
                } catch { 
                    e.Fill = new ImageBrush(this.CreateSvgDrawing(System.Windows.Media.Color.FromRgb(2, 248, 233))) { Stretch = Stretch.UniformToFill }; 
                } 
            } else { 
                e.Fill = new ImageBrush(this.CreateSvgDrawing(System.Windows.Media.Color.FromRgb(2, 248, 233))) { Stretch = Stretch.UniformToFill }; 
            } 
        }

        private void UploadAvatarClick(object s, MouseButtonEventArgs e) { Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog(); dlg.Filter = "图片|*.png;*.jpg"; if (dlg.ShowDialog() == true) { try { System.IO.File.Copy(dlg.FileName, this.avatarPath, true); this.UpdateAvatarDisplay(this.avatarCircle); } catch {} } }

        private void LoadConnections() { 
            if (System.IO.File.Exists(this.configPath)) { try { using (FileStream fs = System.IO.File.OpenRead(this.configPath)) { DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ConnectionEntry>)); List<ConnectionEntry> list = (List<ConnectionEntry>)s.ReadObject(fs); this.connections.Clear(); if (list != null) { foreach (ConnectionEntry item in list) this.connections.Add(item); } } } catch {} } 
        }
        private void SaveConnections() { 
            try { using (FileStream fs = new FileStream(this.configPath, FileMode.Create)) { DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ConnectionEntry>)); s.WriteObject(fs, new List<ConnectionEntry>(this.connections)); } } catch {} 
        }

        private void DeleteAccount(object s, RoutedEventArgs e) { 
            System.Windows.Controls.Button b = s as System.Windows.Controls.Button;
            if (b != null) {
                ConnectionEntry en = b.DataContext as ConnectionEntry;
                if (en != null) {
                    if (System.Windows.MessageBox.Show("确定清除?", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                        this.connections.Remove(en); this.SaveConnections(); this.CmdKeyDelete(en.Host);
                    }
                }
            }
            e.Handled = true;
        }

        private void SaveSuccessfulConnection(string h, string u, string p, int dp, int res, int col) {
            ConnectionEntry target = null;
            for (int i = 0; i < this.connections.Count; i++) { if (this.connections[i].Host == h) { target = this.connections[i]; break; } }
            
            string currentHost = this.ipBox.Text;
            string currentUser = this.userBox.Text;
            string currentPass = this.passBox.Password;

            this.isAutoFilling = true;
            if (target != null) this.connections.Remove(target);
            ConnectionEntry entry = new ConnectionEntry(); entry.Host = h; entry.Username = u; entry.EncryptedPass = this.Encrypt(p);
            entry.DisplayModeIndex = dp; entry.ResolutionIndex = res; entry.ColorDepthIndex = col;
            this.connections.Insert(0, entry); this.SaveConnections();
            
            if (currentHost == h) {
                this.ipBox.SelectedItem = entry;
            }
            this.ipBox.Text = currentHost;
            this.userBox.Text = currentUser;
            this.passBox.Password = currentPass;
            this.isAutoFilling = false;

            if (!string.IsNullOrEmpty(p)) this.CmdKeySave(h, u, p);
        }

        private void StartConnection() {
            string h = this.ipBox.Text.Trim(); string u = this.userBox.Text.Trim(); string p = this.passBox.Password; if (string.IsNullOrEmpty(h) || string.IsNullOrEmpty(u)) return;
            
            foreach (var proc in Process.GetProcessesByName("mstsc")) {
                try { proc.Kill(); } catch { }
            }

            this.isConnecting = true; this.connectBtn.IsEnabled = false; 
            try { 
                StringBuilder rdpHost = new StringBuilder(); 
                rdpHost.AppendLine("full address:s:" + h); 
                rdpHost.AppendLine("username:s:" + u); 

                int dmIdx = this.displayModeBox.SelectedIndex;
                if (dmIdx == 0) { rdpHost.AppendLine("use multimon:i:1"); }
                else if (dmIdx == 1) { rdpHost.AppendLine("use multimon:i:0"); rdpHost.AppendLine("selectedmonitors:s:0"); }
                else if (dmIdx == 2) { rdpHost.AppendLine("use multimon:i:0"); rdpHost.AppendLine("selectedmonitors:s:1"); }
                
                int resIdx = this.resolutionBox.SelectedIndex;
                if (resIdx == 0) { rdpHost.AppendLine("screen mode id:i:2"); }
                else {
                    rdpHost.AppendLine("screen mode id:i:1");
                    rdpHost.AppendLine("smart sizing:i:1");
                    if (resIdx == 1) { rdpHost.AppendLine("desktopwidth:i:3840"); rdpHost.AppendLine("desktopheight:i:2160"); }
                    else if (resIdx == 2) { rdpHost.AppendLine("desktopwidth:i:2560"); rdpHost.AppendLine("desktopheight:i:1440"); }
                    else if (resIdx == 3) { rdpHost.AppendLine("desktopwidth:i:1920"); rdpHost.AppendLine("desktopheight:i:1080"); }
                    else if (resIdx == 4) { rdpHost.AppendLine("desktopwidth:i:1366"); rdpHost.AppendLine("desktopheight:i:768"); }
                    else if (resIdx == 5) { rdpHost.AppendLine("desktopwidth:i:1280"); rdpHost.AppendLine("desktopheight:i:720"); }
                    else if (resIdx == 6) { rdpHost.AppendLine("desktopwidth:i:1024"); rdpHost.AppendLine("desktopheight:i:768"); }
                }

                int colIdx = this.colorDepthBox.SelectedIndex;
                if (colIdx == 0) rdpHost.AppendLine("session bpp:i:32");
                else if (colIdx == 1) rdpHost.AppendLine("session bpp:i:24");
                else if (colIdx == 2) rdpHost.AppendLine("session bpp:i:16");

                string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RemoteDesk_Launcher.rdp"); 
                System.IO.File.WriteAllText(path, rdpHost.ToString(), Encoding.UTF8); 

                ProcessStartInfo psi = new ProcessStartInfo("mstsc.exe", "\"" + path + "\"");
                this.activeMstsc = new Process();
                this.activeMstsc.StartInfo = psi;
                this.activeMstsc.EnableRaisingEvents = true;

                this.activeMstsc.Exited += (s, ev) => {
                    if (System.Windows.Application.Current != null) {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            this.isConnecting = false; this.connectBtn.IsEnabled = true; 
                        }));
                    }
                };

                this.activeMstsc.Start();
                
                DispatcherTimer rt = new DispatcherTimer(); rt.Interval = TimeSpan.FromSeconds(3); 
                rt.Tick += (s, ev) => { 
                    rt.Stop(); 
                    if (this.activeMstsc != null && !this.activeMstsc.HasExited) {
                        this.SaveSuccessfulConnection(h, u, p, dmIdx, resIdx, colIdx);
                        this.isConnecting = false; 
                        this.connectBtn.IsEnabled = true;
                        this.mainWindow.Close();
                    }
                }; 
                rt.Start(); 
            } catch (Exception ex) { 
                this.isConnecting = false; this.connectBtn.IsEnabled = true; 
                System.Windows.MessageBox.Show("启动远程桌面失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CmdKeySave(string h, string u, string p) { ProcessStartInfo psi = new ProcessStartInfo("cmdkey", "/generic:TERMSRV/" + h + " /user:\"" + u + "\" /pass:\"" + p + "\""); psi.CreateNoWindow = true; psi.UseShellExecute = false; Process.Start(psi); }
        private void CmdKeyDelete(string h) { ProcessStartInfo psi = new ProcessStartInfo("cmdkey", "/delete:TERMSRV/" + h); psi.CreateNoWindow = true; psi.UseShellExecute = false; Process.Start(psi); }
    }
}
