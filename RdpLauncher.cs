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
using System.Threading.Tasks;

namespace RdpLauncher
{
    [DataContract]
    public class ConnectionEntry
    {
        [DataMember] public string Host { get; set; }
        [DataMember] public string Port { get; set; }
        [DataMember] public string Username { get; set; }
        [DataMember] public string EncryptedPass { get; set; }
        public string DisplayName { 
            get { 
                string p = (string.IsNullOrEmpty(this.Port) || this.Port == "3389") ? "" : ":" + this.Port;
                string h = this.Host + p;
                return string.IsNullOrEmpty(this.Username) ? h : h + " (" + this.Username + ")"; 
            } 
        }
        public override string ToString() { return this.Host; }
    }

    public class App : Application
    {
        private const string ICON_B64 = "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";

        private Window mainWindow;
        private ComboBox ipBox;
        private TextBox portBox;
        private TextBox userBox;
        private PasswordBox passBox;
        private ComboBox displayModeBox;
        private Grid rootGrid;
        private StackPanel mainStack;
        private Button connectBtn;
        private Ellipse avatarCircle;
        private Border guideBox;
        private Border aboutBtn;
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
            MessageBox.Show("Error: " + e.Exception.Message);
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.LoadConnections();
            this.CreateUI();
            this.themeTimer = new DispatcherTimer();
            this.themeTimer.Interval = TimeSpan.FromSeconds(1.5);
            this.themeTimer.Tick += new EventHandler(OnThemeTick);
            this.themeTimer.Start();
            if (this.connections.Count > 0) this.ipBox.SelectedIndex = 0;
        }

        private void OnThemeTick(object s, EventArgs ev) { this.UpdateThemeColor(); }

        private void CreateUI()
        {
            this.mainWindow = new Window();
            this.mainWindow.Title = "Remote Desk RDP Launcher";
            this.mainWindow.Width = 400;
            this.mainWindow.Height = 680;
            this.mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.mainWindow.ResizeMode = ResizeMode.CanResize;
            this.mainWindow.MinHeight = 500;
            this.mainWindow.MinWidth = 380;
            this.mainWindow.FontFamily = new FontFamily("Segoe UI");
            this.mainWindow.Icon = this.GetImg(ICON_B64);

            this.rootGrid = new Grid();
            this.mainWindow.Content = this.rootGrid;

            this.mainStack = new StackPanel();
            this.mainStack.Margin = new Thickness(25, 15, 25, 15);
            
            ScrollViewer mainScroll = new ScrollViewer();
            mainScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            mainScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            mainScroll.Content = this.mainStack;
            this.rootGrid.Children.Add(mainScroll);

            this.avatarCircle = new Ellipse();
            this.avatarCircle.Width = 70;
            this.avatarCircle.Height = 70;
            this.avatarCircle.Margin = new Thickness(0, 15, 0, 10);
            this.avatarCircle.HorizontalAlignment = HorizontalAlignment.Center;
            this.avatarCircle.Cursor = Cursors.Hand;
            this.avatarCircle.StrokeThickness = 2;
            
            DropShadowEffect shadow = new DropShadowEffect();
            shadow.BlurRadius = 30; shadow.ShadowDepth = 0; shadow.Opacity = 0.4; shadow.Color = Colors.Black;
            this.avatarCircle.Effect = shadow;
            RenderOptions.SetBitmapScalingMode(this.avatarCircle, BitmapScalingMode.HighQuality);
            this.avatarCircle.MouseLeftButtonDown += new MouseButtonEventHandler(OnAvatarClick);
            this.UpdateAvatarDisplay(this.avatarCircle);
            this.mainStack.Children.Add(this.avatarCircle);

            StackPanel hv = new StackPanel();
            hv.Orientation = Orientation.Horizontal;
            hv.HorizontalAlignment = HorizontalAlignment.Center;
            hv.Margin = new Thickness(0, 5, 0, 3);
            this.mainStack.Children.Add(hv);

            this.guideBox = new Border();
            this.guideBox.Width = 26; this.guideBox.Height = 26;
            this.guideBox.CornerRadius = new CornerRadius(6);
            this.guideBox.BorderThickness = new Thickness(1);
            this.guideBox.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128));
            this.guideBox.Background = Brushes.Transparent;
            this.guideBox.Cursor = Cursors.Hand;
            this.guideBox.ToolTip = "打开配置目录";
            this.guideBox.MouseLeftButtonDown += (s, e) => { try { Process.Start("explorer.exe", this.configDir); } catch { } };
            this.guideBox.MouseEnter += (s, e) => { this.guideBox.Background = new SolidColorBrush(Color.FromArgb(30, 0, 229, 255)); this.guideBox.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0, 229, 255)); };
            this.guideBox.MouseLeave += (s, e) => { this.guideBox.Background = Brushes.Transparent; this.guideBox.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)); };
            
            Image gImg = new Image(); gImg.Source = this.GetImg(ICON_B64); gImg.Width = 16; gImg.Height = 16;
            this.guideBox.Child = gImg;
            hv.Children.Add(this.guideBox);

            TextBlock hostT = new TextBlock();
            hostT.Text = "Remote Desk";
            hostT.FontSize = 20;
            hostT.FontWeight = FontWeights.ExtraBold;
            hostT.VerticalAlignment = VerticalAlignment.Center;
            hostT.Margin = new Thickness(12, 0, 10, 0);
            this.labels.Add(hostT);
            hv.Children.Add(hostT);

            this.aboutBtn = new Border();
            this.aboutBtn.Width = 26; this.aboutBtn.Height = 26;
            this.aboutBtn.CornerRadius = new CornerRadius(6);
            this.aboutBtn.BorderThickness = new Thickness(1);
            this.aboutBtn.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128));
            this.aboutBtn.Background = Brushes.Transparent;
            this.aboutBtn.Cursor = Cursors.Hand;
            this.aboutBtn.ToolTip = "关于软件";
            this.aboutBtn.Margin = new Thickness(0, 0, 8, 0);
            this.aboutBtn.MouseLeftButtonDown += (s, e) => {
                AboutWindow aw = new AboutWindow("Remote Desk RDP Launcher", "Kukie.yiqing@outlook.com", this.GetAccent(), !this.IsLight());
                aw.Owner = this.mainWindow; aw.ShowDialog();
            };
            this.aboutBtn.MouseEnter += (s, e) => { this.aboutBtn.Background = new SolidColorBrush(Color.FromArgb(30, 0, 229, 255)); this.aboutBtn.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0, 229, 255)); };
            this.aboutBtn.MouseLeave += (s, e) => { this.aboutBtn.Background = Brushes.Transparent; this.aboutBtn.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)); };
            
            System.Windows.Shapes.Path iPath = new System.Windows.Shapes.Path();
            iPath.Data = Geometry.Parse("M12,2C6.48,2,2,6.48,2,12s4.48,10,10,10,10-4.48,10-10S17.52,2,12,2ZM13,17H11V11H13V17ZM13,9H11V7H13V9Z");
            iPath.Fill = new SolidColorBrush(Color.FromRgb(0, 229, 255)); 
            Viewbox iv = new Viewbox(); iv.Width = 16; iv.Height = 16; iv.Child = iPath;
            this.aboutBtn.Child = iv;
            hv.Children.Add(this.aboutBtn);

            this.shareIcon = new Border();
            this.shareIcon.Width = 26; this.shareIcon.Height = 26;
            this.shareIcon.CornerRadius = new CornerRadius(6);
            this.shareIcon.BorderThickness = new Thickness(1);
            this.shareIcon.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128));
            this.shareIcon.Background = Brushes.Transparent; 
            this.shareIcon.Cursor = Cursors.Hand;
            this.shareIcon.ToolTip = "打开共享文件夹 (\\ip)";
            this.shareIcon.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnShareClick);
            this.shareIcon.MouseEnter += (s, ev) => { this.shareIcon.Background = new SolidColorBrush(Color.FromArgb(30, 0, 229, 255)); this.shareIcon.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0, 229, 255)); };
            this.shareIcon.MouseLeave += (s, ev) => { this.shareIcon.Background = Brushes.Transparent; this.shareIcon.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)); };

            System.Windows.Shapes.Path sPath = new System.Windows.Shapes.Path();
            sPath.Data = Geometry.Parse("M19,10 L5,10 L9,6 M5,10 L9,14 M5,18 L19,18 L15,14 M19,18 L15,22");
            sPath.Stroke = new SolidColorBrush(Color.FromRgb(0, 229, 255)); 
            sPath.StrokeThickness = 2.2;
            sPath.StrokeStartLineCap = PenLineCap.Round; sPath.StrokeEndLineCap = PenLineCap.Round;
            
            Viewbox sv = new Viewbox(); sv.Width = 16; sv.Height = 16; sv.Child = sPath;
            this.shareIcon.Child = sv;
            hv.Children.Add(this.shareIcon);

            this.subTitleTb = new TextBlock();
            this.subTitleTb.Text = "极速、一键直连远程桌面";
            this.subTitleTb.FontSize = 11;
            this.subTitleTb.Margin = new Thickness(0, -2, 0, 8);
            this.subTitleTb.HorizontalAlignment = HorizontalAlignment.Center;
            this.labels.Add(this.subTitleTb);
            this.mainStack.Children.Add(this.subTitleTb);

            this.ipBox = this.CreateModernDropdown(this.mainStack, "远程主机 IP");
            this.portBox = this.CreateModernField(this.mainStack, "远程端口");
            this.portBox.Text = "3389";
            this.userBox = this.CreateModernField(this.mainStack, "用户名");
            this.userBox.TextChanged += new TextChangedEventHandler(OnUserTextChanged);
            
            this.CreateLabel(this.mainStack, "登录密码");
            Border pb = new Border();
            pb.CornerRadius = new CornerRadius(6); pb.BorderThickness = new Thickness(1.2); pb.Height = 36; pb.Padding = new Thickness(8, 0, 8, 0); pb.Margin = new Thickness(0, 0, 0, 10);
            this.fieldsBorders.Add(pb);
            this.passBox = new PasswordBox();
            this.passBox.Background = Brushes.Transparent; this.passBox.BorderThickness = new Thickness(0); this.passBox.VerticalContentAlignment = VerticalAlignment.Center;
            pb.Child = this.passBox; this.mainStack.Children.Add(pb);

            this.CreateLabel(this.mainStack, "显示模式");
            Border mb = new Border();
            mb.CornerRadius = new CornerRadius(6); mb.BorderThickness = new Thickness(1.2); mb.Height = 36; mb.Margin = new Thickness(0, 0, 0, 10); mb.Padding = new Thickness(0);
            this.fieldsBorders.Add(mb);
            this.displayModeBox = new ComboBox();
            this.displayModeBox.Style = this.GetModernComboStyle();
            this.displayModeBox.IsEditable = true; this.displayModeBox.IsReadOnly = true; this.displayModeBox.Background = Brushes.Transparent; this.displayModeBox.BorderThickness = new Thickness(0); this.displayModeBox.VerticalContentAlignment = VerticalAlignment.Center; this.displayModeBox.FontSize = 14;
            this.displayModeBox.ItemContainerStyle = this.GetComboItemStyle();
            this.displayModeBox.Items.Add("📺 全部显示器"); this.displayModeBox.Items.Add("🖥️ 仅主显示器"); this.displayModeBox.Items.Add("💻 仅副显示器"); this.displayModeBox.SelectedIndex = 0;
            mb.Child = this.displayModeBox; this.mainStack.Children.Add(mb);
            
            this.connectBtn = new Button();
            this.connectBtn.Content = "立 即 连 接";
            this.connectBtn.Height = 44; this.connectBtn.Margin = new Thickness(0, 10, 0, 10); this.connectBtn.Foreground = Brushes.White; this.connectBtn.FontSize = 16; this.connectBtn.FontWeight = FontWeights.Bold; this.connectBtn.Cursor = Cursors.Hand;
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
                byte[] data = Convert.FromBase64String(b); 
                using (MemoryStream ms = new MemoryStream(data)) {
                    BitmapImage i = new BitmapImage(); i.BeginInit(); i.StreamSource = ms; i.CacheOption = BitmapCacheOption.OnLoad; i.EndInit(); return i; 
                }
            } catch { return null; } 
        }

        private void UpdateThemeColor() {
            try {
                Color a = this.GetAccent(); bool d = !this.IsLight(); Brush ab = new SolidColorBrush(a);
                if (this.mainWindow != null) { if (d) this.mainWindow.Background = new SolidColorBrush(Color.FromRgb(20, 20, 22)); else this.mainWindow.Background = new SolidColorBrush(Color.FromRgb(243, 243, 245)); }
                if (this.labels != null) { foreach (TextBlock tb in this.labels) tb.Foreground = ab; }
                if (this.fieldsBorders != null) { 
                    foreach (Border b in this.fieldsBorders) { 
                        if (d) b.Background = new SolidColorBrush(Color.FromRgb(35, 35, 40)); else b.Background = Brushes.White; 
                        if (d) b.BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 60)); else b.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)); 
                    } 
                }
                if (Application.Current != null) {
                    Application.Current.Resources["PopupBg"] = d ? new SolidColorBrush(Color.FromRgb(35, 35, 40)) : Brushes.White;
                    Application.Current.Resources["PopupBorder"] = d ? new SolidColorBrush(Color.FromRgb(50, 50, 60)) : new SolidColorBrush(Color.FromRgb(220, 220, 220));
                }
                if (this.ipBox != null) this.ipBox.Foreground = ab; 
                if (this.portBox != null) this.portBox.Foreground = ab;
                if (this.userBox != null) this.userBox.Foreground = ab;
                if (this.passBox != null) this.passBox.Foreground = ab;
                if (this.displayModeBox != null) this.displayModeBox.Foreground = ab;
                LinearGradientBrush grad = new LinearGradientBrush(); grad.GradientStops.Add(new GradientStop(a, 0.0)); grad.GradientStops.Add(new GradientStop(Color.FromRgb((byte)Math.Min(255, a.R+40), (byte)Math.Min(255, a.G+40), (byte)Math.Min(255, a.B+40)), 1.0));
                if (this.connectBtn != null) this.connectBtn.Background = grad; 
                if (this.avatarCircle != null) this.avatarCircle.Stroke = ab;
            } catch {}
        }

        private bool IsLight() { try { object v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1); return v != null && (int)v == 1; } catch { return false; } }
        private Color GetAccent() { object v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null); if (v is int) { int abgr = (int)v; return Color.FromRgb((byte)(abgr & 0xFF), (byte)((abgr >> 8) & 0xFF), (byte)((abgr >> 16) & 0xFF)); } return Color.FromRgb(0, 120, 215); }

        private void CreateLabel(StackPanel p, string t) { TextBlock tb = new TextBlock(); tb.Text = t; tb.Margin = new Thickness(0, 5, 0, 5); tb.FontWeight = FontWeights.SemiBold; tb.FontSize = 14; this.labels.Add(tb); p.Children.Add(tb); }

        private TextBox CreateModernField(StackPanel p, string l) { 
            this.CreateLabel(p, l); 
            Border h = new Border(); h.CornerRadius = new CornerRadius(6); h.BorderThickness = new Thickness(1.2); h.Height = 36; h.Padding = new Thickness(10, 0, 10, 0); h.Margin = new Thickness(0, 0, 0, 10); this.fieldsBorders.Add(h); 
            TextBox box = new TextBox(); box.Background = Brushes.Transparent; box.BorderThickness = new Thickness(0); box.VerticalContentAlignment = VerticalAlignment.Center; box.FontSize = 14; h.Child = box; p.Children.Add(h); return box; 
        }

        private ComboBox CreateModernDropdown(StackPanel p, string l) { 
            this.CreateLabel(p, l); 
            Border bh = new Border(); bh.CornerRadius = new CornerRadius(6); bh.BorderThickness = new Thickness(1.2); bh.Height = 36; bh.Padding = new Thickness(0); bh.Margin = new Thickness(0, 0, 0, 10); this.fieldsBorders.Add(bh); 
            ComboBox box = new ComboBox(); box.IsEditable = true; box.Background = Brushes.Transparent; box.BorderThickness = new Thickness(0); box.VerticalContentAlignment = VerticalAlignment.Center; box.FontSize = 14; box.ItemsSource = this.connections; box.ItemTemplate = this.CreateItemTemplate(); 
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
            this.portBox.Text = "3389";
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
                this.portBox.Text = string.IsNullOrEmpty(en.Port) ? "3389" : en.Port;
                this.userBox.Text = en.Username; 
                this.passBox.Password = this.Decrypt(en.EncryptedPass); 
                this.isAutoFilling = false;
                if (Application.Current != null) Application.Current.Dispatcher.BeginInvoke(new Action(() => { 
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
            s.Setters.Add(new Setter(FrameworkElement.CursorProperty, Cursors.Hand));
            return s;
        }

        private DataTemplate CreateItemTemplate() { 
            DataTemplate t = new DataTemplate(); 
            FrameworkElementFactory dp = new FrameworkElementFactory(typeof(DockPanel)); 
            dp.SetValue(DockPanel.LastChildFillProperty, true); 
            dp.SetValue(FrameworkElement.WidthProperty, 300.0); 
            t.VisualTree = dp; 
            
            FrameworkElementFactory btn = new FrameworkElementFactory(typeof(Button)); 
            btn.SetValue(DockPanel.DockProperty, Dock.Right); 
            btn.SetValue(Button.ContentProperty, "✕"); 
            btn.SetValue(Button.ForegroundProperty, new SolidColorBrush(Color.FromRgb(255, 82, 82)));
            btn.SetValue(Button.CursorProperty, Cursors.Hand);
            btn.SetValue(Button.ToolTipProperty, "移除此记录");
            
            Style flatBtnStyle = new Style(typeof(Button)); 
            ControlTemplate ft = new ControlTemplate(typeof(Button)); 
            FrameworkElementFactory fb = new FrameworkElementFactory(typeof(Border)); 
            fb.SetValue(Border.BackgroundProperty, Brushes.Transparent); 
            fb.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            FrameworkElementFactory fc = new FrameworkElementFactory(typeof(ContentPresenter)); 
            fc.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center); 
            fc.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center); 
            fc.SetValue(FrameworkElement.MarginProperty, new Thickness(8, 2, 8, 2));
            fb.AppendChild(fc); ft.VisualTree = fb; 
            flatBtnStyle.Setters.Add(new Setter(Button.TemplateProperty, ft)); 
            btn.SetValue(Button.StyleProperty, flatBtnStyle);
            
            btn.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.DeleteAccount)); 
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
            Style sw = new Style(typeof(Button)); ControlTemplate tw = new ControlTemplate(typeof(Button)); FrameworkElementFactory bw = new FrameworkElementFactory(typeof(Border)); bw.SetValue(Border.CornerRadiusProperty, new CornerRadius(10)); bw.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty)); FrameworkElementFactory cw = new FrameworkElementFactory(typeof(ContentPresenter)); cw.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center); cw.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center); bw.AppendChild(cw); tw.VisualTree = bw; sw.Setters.Add(new Setter(Button.TemplateProperty, tw)); return sw; 
        }

        private void UpdateConnectionStatus() { if (this.isConnecting) this.connectBtn.Content = "正在连接中..."; else this.connectBtn.Content = "立 即 连 接"; }

        private string Encrypt(string t) { if (string.IsNullOrEmpty(t)) return null; byte[] d = Encoding.UTF8.GetBytes(t); byte[] enc = ProtectedData.Protect(d, null, DataProtectionScope.CurrentUser); return Convert.ToBase64String(enc); }
        private string Decrypt(string t) { if (string.IsNullOrEmpty(t)) return ""; byte[] d = Convert.FromBase64String(t); byte[] dec = ProtectedData.Unprotect(d, null, DataProtectionScope.CurrentUser); return Encoding.UTF8.GetString(dec); }

        private void UpdateAvatarDisplay(Ellipse e) { if (File.Exists(this.avatarPath)) { try { BitmapImage b = new BitmapImage(); b.BeginInit(); b.UriSource = new Uri(this.avatarPath); b.CacheOption = BitmapCacheOption.OnLoad; b.EndInit(); e.Fill = new ImageBrush(b) { Stretch = Stretch.UniformToFill }; } catch { e.Fill = Brushes.Gray; } } else { e.Fill = Brushes.DarkGray; } }

        private void UploadAvatarClick(object s, MouseButtonEventArgs e) { Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog(); dlg.Filter = "图片|*.png;*.jpg"; if (dlg.ShowDialog() == true) { try { File.Copy(dlg.FileName, this.avatarPath, true); this.UpdateAvatarDisplay(this.avatarCircle); } catch {} } }

        private void LoadConnections() { 
            if (File.Exists(this.configPath)) { try { using (FileStream fs = File.OpenRead(this.configPath)) { DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ConnectionEntry>)); List<ConnectionEntry> list = (List<ConnectionEntry>)s.ReadObject(fs); this.connections.Clear(); if (list != null) { foreach (ConnectionEntry item in list) this.connections.Add(item); } } } catch {} } 
        }
        private void SaveConnections() { 
            try { using (FileStream fs = new FileStream(this.configPath, FileMode.Create)) { DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ConnectionEntry>)); s.WriteObject(fs, new List<ConnectionEntry>(this.connections)); } } catch {} 
        }

        private void DeleteAccount(object s, RoutedEventArgs e) { 
            Button b = s as Button;
            if (b != null) {
                ConnectionEntry en = b.DataContext as ConnectionEntry;
                if (en != null) {
                    ConfirmWindow cw = new ConfirmWindow("确定清除该账号记录吗？\n此操作物理删除且不可恢复。", this.GetAccent(), !this.IsLight());
                    cw.Owner = this.mainWindow;
                    if (cw.ShowDialog() == true) {
                        this.connections.Remove(en); this.SaveConnections(); this.CmdKeyDelete(en.Host);
                    }
                }
            }
            e.Handled = true;
        }

        private void StartConnection() {
            string h = this.ipBox.Text.Trim(); string prt = this.portBox.Text.Trim(); string u = this.userBox.Text.Trim(); string p = this.passBox.Password; if (string.IsNullOrEmpty(h) || string.IsNullOrEmpty(u)) return;
            if (string.IsNullOrEmpty(prt)) prt = "3389";
            
            this.isAutoFilling = true;
            ConnectionEntry target = null;
            for (int i = 0; i < this.connections.Count; i++) { if (this.connections[i].Host == h && (this.connections[i].Port == prt || (string.IsNullOrEmpty(this.connections[i].Port) && prt == "3389"))) { target = this.connections[i]; break; } }
            if (target != null) this.connections.Remove(target);
            ConnectionEntry entry = new ConnectionEntry(); entry.Host = h; entry.Port = prt; entry.Username = u; entry.EncryptedPass = this.Encrypt(p);
            this.connections.Insert(0, entry);
            this.ipBox.SelectedItem = entry;
            this.ipBox.Text = h;
            this.portBox.Text = prt;
            this.userBox.Text = u;
            this.passBox.Password = p;
            this.isAutoFilling = false;
            if (!string.IsNullOrEmpty(p)) this.CmdKeySave(h, u, p);
            
            this.VerifyAndSave(entry);

            try { 
                this.isConnecting = true; this.connectBtn.IsEnabled = false; 
                StringBuilder rdpHost = new StringBuilder(); 
                rdpHost.AppendLine("full address:s:" + h + ":" + prt); 
                rdpHost.AppendLine("username:s:" + u); 
                rdpHost.AppendLine("screen mode id:i:2");
                if (this.displayModeBox.SelectedIndex == 0) { rdpHost.AppendLine("use multimon:i:1"); }
                else if (this.displayModeBox.SelectedIndex == 1) { rdpHost.AppendLine("use multimon:i:0"); rdpHost.AppendLine("selectedmonitors:s:0"); }
                else if (this.displayModeBox.SelectedIndex == 2) { rdpHost.AppendLine("use multimon:i:0"); rdpHost.AppendLine("selectedmonitors:s:1"); }
                string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RemoteDesk_Launcher.rdp"); 
                System.IO.File.WriteAllText(path, rdpHost.ToString(), Encoding.UTF8); 
                this.activeMstsc = System.Diagnostics.Process.Start("mstsc.exe", "\"" + path + "\"");
                DispatcherTimer rt = new DispatcherTimer(); rt.Interval = TimeSpan.FromSeconds(3); rt.Tick += new EventHandler(OnResetTick); rt.Start(); 
            } catch {}
        }
        private void OnResetTick(object s, EventArgs args) { 
            DispatcherTimer rt = (DispatcherTimer)s;
            this.isConnecting = false; this.connectBtn.IsEnabled = true; 
            rt.Stop(); 
        }
        private void CmdKeySave(string h, string u, string p) { ProcessStartInfo psi = new ProcessStartInfo("cmdkey", "/generic:TERMSRV/" + h + " /user:\"" + u + "\" /pass:\"" + p + "\""); psi.CreateNoWindow = true; psi.UseShellExecute = false; Process.Start(psi); }
        private void CmdKeyDelete(string h) { ProcessStartInfo psi = new ProcessStartInfo("cmdkey", "/delete:TERMSRV/" + h); psi.CreateNoWindow = true; psi.UseShellExecute = false; Process.Start(psi); }

        private async void VerifyAndSave(ConnectionEntry entry) {
            bool ok = false;
            try {
                using (var client = new System.Net.Sockets.TcpClient()) {
                    var task = client.ConnectAsync(entry.Host, int.Parse(string.IsNullOrEmpty(entry.Port) ? "3389" : entry.Port));
                    if (await Task.WhenAny(task, Task.Delay(1800)) == task) { await task; ok = true; }
                }
            } catch { }
            if (ok) { this.SaveConnections(); }
            else { Application.Current.Dispatcher.Invoke(() => { this.connections.Remove(entry); }); }
        }
    }

    public class ConfirmWindow : Window {
        public ConfirmWindow(string msg, Color accent, bool isDark) {
            this.Title = "确认"; this.Width = 320; this.Height = 180;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize; this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true; this.Background = Brushes.Transparent;
            this.ShowInTaskbar = false;
            
            Border b = new Border(); b.CornerRadius = new CornerRadius(12);
            b.Background = isDark ? new SolidColorBrush(Color.FromRgb(30, 30, 35)) : Brushes.White;
            b.BorderThickness = new Thickness(1.5); b.BorderBrush = new SolidColorBrush(accent);
            b.Padding = new Thickness(20);
            
            StackPanel s = new StackPanel();
            TextBlock t = new TextBlock(); t.Text = msg; t.TextWrapping = TextWrapping.Wrap;
            t.Foreground = new SolidColorBrush(Color.FromRgb(255, 82, 82)); 
            t.FontSize = 14; t.FontWeight = FontWeights.SemiBold; t.Margin = new Thickness(0, 10, 0, 25); t.TextAlignment = TextAlignment.Center;
            s.Children.Add(t);
            
            Grid g = new Grid(); g.ColumnDefinitions.Add(new ColumnDefinition()); g.ColumnDefinitions.Add(new ColumnDefinition());
            Button y = new Button(); y.Content = "确定清除"; y.Height = 32; y.Margin = new Thickness(5); y.Cursor = Cursors.Hand;
            y.Background = new SolidColorBrush(Color.FromRgb(255, 82, 82)); y.Foreground = Brushes.White;
            y.Click += (se, ev) => { this.DialogResult = true; this.Close(); };
            
            Button n = new Button(); n.Content = "取消"; n.Height = 32; n.Margin = new Thickness(5); n.Cursor = Cursors.Hand;
            n.Click += (se, ev) => { this.DialogResult = false; this.Close(); };
            
            Grid.SetColumn(y, 0); Grid.SetColumn(n, 1);
            g.Children.Add(y); g.Children.Add(n);
            s.Children.Add(g);
            b.Child = s; this.Content = b;
            
            Style btnStyle = new Style(typeof(Button));
            ControlTemplate ct = new ControlTemplate(typeof(Button));
            FrameworkElementFactory br = new FrameworkElementFactory(typeof(Border));
            br.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));
            br.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            FrameworkElementFactory cp = new FrameworkElementFactory(typeof(ContentPresenter));
            cp.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            cp.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            br.AppendChild(cp); ct.VisualTree = br; btnStyle.Setters.Add(new Setter(Button.TemplateProperty, ct));
            y.Style = btnStyle; n.Style = btnStyle;
        }
    }

    public class AboutWindow : Window {
        public AboutWindow(string title, string email, Color accent, bool isDark) {
            this.Title = "关于软件"; this.Width = 360; this.Height = 280;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize; this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true; this.Background = Brushes.Transparent;
            this.ShowInTaskbar = false;
            
            Border b = new Border(); b.CornerRadius = new CornerRadius(12);
            b.Background = isDark ? new SolidColorBrush(Color.FromRgb(30, 30, 35)) : Brushes.White;
            b.BorderThickness = new Thickness(1.5); b.BorderBrush = new SolidColorBrush(accent);
            b.Padding = new Thickness(25);
            
            StackPanel s = new StackPanel();
            TextBlock t = new TextBlock(); t.Text = title; t.FontSize = 18; t.FontWeight = FontWeights.Bold; t.Foreground = new SolidColorBrush(accent); t.Margin = new Thickness(0,0,0,12); t.TextAlignment = TextAlignment.Center;
            s.Children.Add(t);
            
            TextBlock v = new TextBlock(); v.Text = "Version 1.2.5 (Build 20260402)"; v.FontSize = 11; v.Foreground = Brushes.Gray; v.Margin = new Thickness(0,0,0,15); v.TextAlignment = TextAlignment.Center;
            s.Children.Add(v);
            
            TextBlock des = new TextBlock(); 
            des.Text = "说明：本程序旨在提供安全、高效的 RDP 管理体验。所有登录凭据均通过 Windows DPAPI 强加密存储于本地，开发者承诺不收集任何隐私数据。"; 
            des.FontSize = 12; des.TextWrapping = TextWrapping.Wrap; des.Foreground = isDark ? Brushes.LightGray : Brushes.DarkSlateGray; 
            des.Margin = new Thickness(0,0,0,15); des.TextAlignment = TextAlignment.Center;
            s.Children.Add(des);

            TextBlock c = new TextBlock(); c.Text = "Copyright © 2026 Kukie Zhang. 保留所有权利。"; c.FontSize = 12; c.Foreground = isDark ? Brushes.White : Brushes.Black; c.TextAlignment = TextAlignment.Center;
            s.Children.Add(c);
            
            TextBlock e = new TextBlock(); e.Text = "联系支持: " + email; e.FontSize = 12; e.Foreground = new SolidColorBrush(Color.FromRgb(0, 229, 255)); e.Margin = new Thickness(0,5,0,20); e.TextAlignment = TextAlignment.Center;
            s.Children.Add(e);
            
            Button cl = new Button(); cl.Content = "确 定"; cl.Height = 32; cl.Width = 120; cl.Cursor = Cursors.Hand;
            cl.Background = new SolidColorBrush(accent); cl.Foreground = Brushes.White;
            cl.Click += (se, ev) => { this.Close(); };
            
            Style btnStyle = new Style(typeof(Button));
            ControlTemplate ct = new ControlTemplate(typeof(Button));
            FrameworkElementFactory br = new FrameworkElementFactory(typeof(Border));
            br.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));
            br.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            FrameworkElementFactory cp = new FrameworkElementFactory(typeof(ContentPresenter));
            cp.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            cp.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            br.AppendChild(cp); ct.VisualTree = br; btnStyle.Setters.Add(new Setter(Button.TemplateProperty, ct));
            cl.Style = btnStyle; 
            
            s.Children.Add(cl);
            b.Child = s; this.Content = b;
            this.MouseLeftButtonDown += (se, ev) => { try { this.DragMove(); } catch {} };
        }
    }
}
