
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using ATL.UI.Core;
using ATL.UI.UserManager;
using ATL.UI;
using ATL.UI.DeviceOverview;
using ATL.Station;
using ATL.Common;
using ATL.MES;
using ATL.UI.MES;
using ATL.Core;
using ATL.Engine;
using XRayClient.VisionSysWrapper;
using ZY.XRayTube;
using ZY.BarCodeReader;
using ZY.Logging;
using PTF.Models;
using XRayClient.Core;
using PTF.ViewModels;
using ZY.MitutoyoReader;

namespace PTF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private static MainWindow current = new MainWindow();

        public static MainWindow Current
        {
            get
            {
                return current;
            }
        }

        public CheckParamsConfig MyCheckParamsConfig
        {
            get { return CheckParamsConfig.Instance; }
        }
        public MainWindow()
        {
            
            InitializeComponent();
            BeckhoffTwinCAT.PLC plc = new BeckhoffTwinCAT.PLC();
            loadMainPage();
            LogHelper.Info("系统启动");
            LoginManagerPage.resetMenus += StartResetMenus;
            StationStatePage.sendMessage += GotoNewPage;
            LoginManagerPage.startRun += StartRun;
            InterfaceClient.A007event += GotoInputPage;
            this.RefreshTimer.Interval = new TimeSpan(0, 0, 1);
            this.RefreshTimer.Tick += new EventHandler(this.RefreshTimer_Tick);
            this.RefreshTimer.Start();
        }
        public static void StartResetMenus()
        {
            MainWindow.Current.ResetMenus();
            MainWindow.Current.LoadMenus();
        }
        public static void StartRun()
        {
            if (AppContext.Current.Name != "Guest")
            {
                if (!Core.CheckedOK)
                {
                    CheckConfig.CheckAndStart();
                    //PTF.Communication.BySocket.Server server = new Communication.BySocket.Server(10, 1024 * 1024);
                    //server.Init();
                    //server.Start(new System.Net.IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001));
                }
                else if (Core.CheckedOK && (ID_Device.lstDevices.Count > 0 && ID_Device.lstDevices.Exists(x => !x.success)) || ID_Device.lstDevices.Count == 0)
                {
                    CheckConfig.Start();
                }
            }
        }

        public static void GotoNewPage(string url)
        {
            PageFrame.Navigate(new Uri(url, UriKind.Absolute));
        }

        private DispatcherTimer RefreshTimer = new DispatcherTimer();

        private void loadMainPage()
        {
            uri = new Uri("", UriKind.Relative);
            mainPage.Navigate(uri);
        }

        public static Frame PageFrame;

        private List<ATL.UI.Core.MenuNode> mainNodes = new List<ATL.UI.Core.MenuNode>();
        private ATL.UI.Core.MenuNode lastSelectedMainNode;
        private ATL.UI.Core.MenuNode lastSelectdChildNode;
        private Uri uri;
        private Button UserManagerButton;
        private Button UserManagerLoginButton;

        private void Window_Loaded(object sender, RoutedEventArgs e)    
        {
            this.LoadMenus();
            //mainPage.Navigate(CurMainPage);
            MainMenuButton_Click(UserManagerButton, null);
            ChildMenuButton_Click(UserManagerLoginButton, null);
            Station.Current.LabChinese = UserDefineVariableInfo.DicVariables._dict["LabChinese"].ToString();
            Station.Current.LabEnglish = UserDefineVariableInfo.DicVariables._dict["LabEnglish"].ToString();
            Station.Current.LabVersion = UserDefineVariableInfo.DicVariables._dict["LabVersion"].ToString();

            LabChineseName.DataContext = Station.Current;
            LabEnglishName.DataContext = Station.Current;
            LabVsrsionName.DataContext = Station.Current;
            PageFrame = mainPage;
            
            DataContext = Station.Current;
            ATL.UI.SystemSetting.SystemConfig.MainClose += MainClose;
            InputParaDownloadPage.InputParaDownloadEvent += downLoadInputParaToPLC;
            ATL.MES.InterfaceClient.A053event += A053;
            ThreadPool.QueueUserWorkItem(DoWork);
			
            LoggingIF.Init();
            LoggingIF.Log("系统启动", LogLevels.Info, "MainWindow");
            UserDefineVariableInfo.DicVariables["AlgoInitFinished"] = 0;

            BotIF.Init(HardwareConfig.Instance.CameraParams1);

            XRayTubeIF.Init(HardwareConfig.Instance.XRayConfig1);
            CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.ScanBarcodeIPAdress, HardwareConfig.Instance.ScanBarcodePort, 1);//扫码
            CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量
            MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig, 1);//A测厚
            MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig2, 2);//B测厚
            MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig3, 3);//A测厚
            MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig4, 4);//B测厚

            VisionSysWrapperIF.AddCamera(HardwareConfig.Instance.CameraParams1);

            int[] iResults;
            int iRet = VisionSysWrapperIF.CameraInit(out iResults);
            if (0 != iRet)
            {
                LoggingIF.Log("相机初始化失败", LogLevels.Info, "MainWindow");
                System.Windows.MessageBox.Show("相机初始化失败!", "提示");
            }
            else
            {
                LoggingIF.Log("相机初始化成功", LogLevels.Info, "MainWindow");
            }


            bool ret = false;
            InspectParams paramsinpect = new InspectParams();
            paramsinpect.total_layer = 16;
            paramsinpect.iLine = 1;
            paramsinpect.iCorner = 1;
            paramsinpect.pixel_to_mm = (float)0.0075;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ret = VisionSysWrapperIF.InitAlgo_DL(ref paramsinpect);
                if (ret == false)
                {
                    //MessageBox.Show("算法初始化失败");
                    LoggingIF.Log("算法初始化失败", LogLevels.Info, "MainWindow");
                }
                else
                {
                    //MessageBox.Show("算法初始化成功");
                    UserDefineVariableInfo.DicVariables["AlgoInitFinished"] = 1;
                    LoggingIF.Log("算法初始化成功", LogLevels.Info, "MainWindow");
                }
            }).Start();

            mainPage.Navigate(DashBoard);
        }

        private void downLoadInputParaToPLC(object sender, InputParaDownloadPage.InputParaDownloadEventArgs arg)
        {
            List<InputInformation> lstInput = arg.inputInformations;
            
            MotionControlVm.Instance.LoadWorkParams(CheckParamsConfig.Instance, HardwareConfig.Instance.CameraParams1);
        }
        
        private void A053(object sender, ATL.MES.InterfaceClient.A053EventArgs arg)
        {
            arg.root.RequestInfo.FolderPath = arg.root.RequestInfo.FolderPath.Replace("\\\\", "//");
            arg.root.RequestInfo.FolderPath = arg.root.RequestInfo.FolderPath.Replace("\\", "/");
            UserDefineVariableInfo.DicVariables["mesImgSavePath"] = arg.root.RequestInfo.FolderPath;
        }

        private void MainClose(object sender, EventArgs arg)
        {
            ModVariable.MainClose();
        }
		
        public void LoadMenus()
        {
            this.BuildMenus(AppContext.Current.GetPermissionNodesAsync());
        }

        public void ResetMenus()
        {
            menus.Children.Clear();
        }

        public void BuildMenus(IList<PermissionNode> permissionNodes)
        {
            Style style1 = this.FindResource("MainButtonStyle") as Style;
            Style style2 = this.FindResource("ChildButtonStyle") as Style;
            Style style3 = this.FindResource("ChildButtonSelectedStyle") as Style;
            Style style4 = this.FindResource("DeviceControlButtonStyle") as Style;
            double height = this.menus.Height;
            foreach (PermissionNode permissionNode in permissionNodes)
            {
                ATL.UI.Core.MenuNode menuNode1 = new ATL.UI.Core.MenuNode();
                Button button1 = new Button();
                if (permissionNode.Text == "设备控制")
                {
                    button1.Style = style4;
                }
                else
                    button1.Style = style1;
                button1.Click += new RoutedEventHandler(this.MainMenuButton_Click);
                button1.Content = permissionNode.Text;
                menuNode1.Button = button1;
                menuNode1.PermissionNode = permissionNode;
                button1.Tag = menuNode1;
                
                this.menus.Children.Add(button1);
                this.mainNodes.Add(menuNode1);
                if (permissionNode.Children.Count > 0)
                {
                    ScrollViewer scrollViewer = new ScrollViewer();
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    StackPanel stackPanel = new StackPanel();
                    scrollViewer.Content = stackPanel;
                    scrollViewer.Visibility = Visibility.Collapsed;
                    foreach (PermissionNode child in permissionNode.Children)
                    {
                        ATL.UI.Core.MenuNode menuNode2 = new MenuNode();
                        Button button2 = new Button();
                        button2.Style = style2;
                        button2.Click += new RoutedEventHandler(this.ChildMenuButton_Click);
                        button2.Content = child.Text;
                        menuNode2.Button = button2;
                        menuNode2.PermissionNode = child;
                        button2.Tag = menuNode2;
                        menuNode1.Children.Add(menuNode2);
                        stackPanel.Children.Add(button2);
                        if (permissionNode.Code == "UserManager")
                        {
                            if (child.Code == "UserManager.Login")
                            {
                                UserManagerLoginButton = button2;
                            }
                        }
                    }
                    menuNode1.ChildPanel = scrollViewer;
                    this.menus.Children.Add(scrollViewer);
                    if (permissionNode.Code == "UserManager")
                    {
                        UserManagerButton = button1;
                    }
                }
                height -= button1.Height + button1.Margin.Top + button1.Margin.Bottom;
            }
            foreach (ATL.UI.Core.MenuNode mainNode in this.mainNodes)
            {
                if (mainNode.ChildPanel != null)
                    mainNode.ChildPanel.Height = height;
            }
            if (this.mainNodes.Count <= 0)
                return;
            this.lastSelectedMainNode = this.mainNodes.First<ATL.UI.Core.MenuNode>();
            if (this.lastSelectedMainNode.ChildPanel == null)
                return;
            this.lastSelectedMainNode.ChildPanel.Visibility = Visibility.Visible;
            this.lastSelectdChildNode = this.lastSelectedMainNode.Children.First();
            this.lastSelectdChildNode.Button.Style = style3;
        }

        #region 按钮事件

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("你要退出本程序？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes && ATL.UI.Core.AppContext.Current.Name == "Guest")
            {
                MessageBox.Show("退出本程序前，请先登入账户", "提示", MessageBoxButton.OK);
                LogInDB.Info("关闭软件时，权限不够！");
                return;
            }
            else if (result == MessageBoxResult.No)
            {
                LogInDB.Info("关闭软件时，取消关闭！");
                return;
            }
            
            ATL.Core.Core.SoftClosing = true;
            Thread.Sleep(1500);
            
            CodeReaderIF.UnInit_ClientConnet();
            XRayTubeIF.UnInit();
            MitutoyoReaderIF.UnInit();
            VisionSysWrapperIF.CameraUnInit();

            ModVariable.MainClose();
            LogInDB.Info("软件关闭");
            Thread.Sleep(1500);
            Application.Current.Shutdown();
            Environment.Exit(-1);
        }

        private void DoWork(object stateInfo)
        {
            while(!ATL.Core.Core.SoftClosing)
            {
                Dispatcher.Invoke(delegate ()
                {
                    tbUserName.Text = ATL.MES.UserInfo.UserName;
                    if(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Count > 0 && ATL.Core.Core.CheckedOK)
                    {
                        tbModel.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
                    }
                });

                Thread.Sleep(5000);
            }
        }


        private void btnSwitchUser_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Application.Current.MainWindow = loginWindow;
            this.Close();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void ChildMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MenuNode tag = (MenuNode)button.Tag;
            if ((tag != this.lastSelectdChildNode) && (this.lastSelectdChildNode != null))
            {
                this.lastSelectdChildNode.Button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            this.lastSelectdChildNode = tag;
            tag.Button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ATL.UI;component/Assets/menu_child_button_bg.png", UriKind.Absolute)));
            if(tag.PermissionNode.Code == "DeviceOverview.Monitor")
            {
                if (monitor == null)
                {
                    monitor = new MonitorPage();
                    monitor.Closed += new EventHandler(monitor_Closed);
                    monitor.sendMessage += GotoNewPage;
                    monitor.ShowDialog();
                    monitor = null;
                }
            }
            //else if (tag.PermissionNode.Code == "DeviceControl.MainPage")
            //{
            //    mainPage.Navigate(CurMainPage);
            //}
            else if (tag.PermissionNode.Code == "DeviceControl.WinformMainPage")
			{
			    mainPage.Navigate(CurTestWinformPage);
			}
			else if (tag.PermissionNode.Code == "DeviceControl.WinformUserControlPage")
            {
		        mainPage.Navigate(CurTestUserControlPage);
			}
            else if (tag.PermissionNode.Code == "DeviceControl.DashBoard")
            {
                mainPage.Navigate(DashBoard);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.MotionControl")
            {
                mainPage.Navigate(MotionControl);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.XrayTubeControl")
            {
                mainPage.Navigate(XrayTubeControl);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.CameraSetting")
            {
                mainPage.Navigate(CameraSeeting);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.CameraCalibration")
            {
                CameraCalibration.DataContext = ((CameraSetingsVm)(CameraSeeting.DataContext));
                mainPage.Navigate(CameraCalibration);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.CheckParamsSettings")
            {
                mainPage.Navigate(CheckParamsSettings);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.InspectTest")
            {
                mainPage.Navigate(InspectTest);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.ManualRecheck")
            {
                UserDefineVariableInfo.DicVariables["IsRecheckMode"] = 1;
                if (xrayCheckPage == null)
                {
                    xrayCheckPage = new XrayCheckPage();
                    xrayCheckPage.Closed += new EventHandler(xrayCheckPage_Closed);
                    xrayCheckPage.ShowDialog();
                    xrayCheckPage = null;
                }
            }
            else if (tag.PermissionNode.Code == "DeviceControl.ManualRecheckFQA")
            {
                UserDefineVariableInfo.DicVariables["IsRecheckMode"] = 0;
                if (xrayCheckPage == null)
                {
                    xrayCheckPage = new XrayCheckPage();
                    xrayCheckPage.Closed += new EventHandler(xrayCheckPage_Closed);
                    xrayCheckPage.ShowDialog();
                    xrayCheckPage = null;
                }
            }
            else if (PermissionInfo.dicUrl.Keys.Contains(tag.PermissionNode.Code))
            {
                this.mainPage.Navigate(new Uri(PermissionInfo.dicUrl[tag.PermissionNode.Code], UriKind.Absolute));
            }
            ATL.UI.CheckConfig.MainWindowMenuButton_Click();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ATL.UI.Core.MenuNode menuNode = (ATL.UI.Core.MenuNode)((FrameworkElement)sender).Tag;
            if (this.lastSelectedMainNode != menuNode && this.lastSelectedMainNode.ChildPanel != null)
                this.lastSelectedMainNode.ChildPanel.Visibility = Visibility.Collapsed;

            if (menuNode.ChildPanel != null)
                menuNode.ChildPanel.Visibility = Visibility.Visible;
            this.lastSelectedMainNode = menuNode;
            if (menuNode.Children.Count != 0 || this.lastSelectdChildNode == null)
            {
                if(menuNode.Children.Count != 0)
                {
                    if (menuNode.PermissionNode.Text == "系统配置" || menuNode.PermissionNode.Code == "SystemSetting")
                    {
                        //CheckConfig.needChecked = true;
                        CheckConfig.Stop();
                        Core.CheckedOK = false;
                        LogInDB.Info("进入系统配置界面，稍后重新检查配置和启动");
                        mainPage.Navigate(new Uri(PermissionInfo.dicUrl["DeviceOverview.Version"], UriKind.Absolute));
                    }
                    else
                        StartRun();
                }
                return;
            }
            this.lastSelectdChildNode.Button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            this.lastSelectdChildNode = null;
            ATL.UI.CheckConfig.MainWindowMenuButton_Click();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        
        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            this.mainPage.Navigate(uri);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (!this.mainPage.CanGoBack)
                return;
            this.mainPage.GoBack();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.WindowState != WindowState.Normal || (!new Rect(new Point(0.0, 0.0), new Size(this.Width, this.Height * (7.0 / 225.0))).Contains(e.GetPosition(this)) || e.ChangedButton != MouseButton.Left))
                return;
            this.DragMove();
        }

        #endregion
        DateTime lastRefreshUI = DateTime.Now;
        bool PopedMonitorPage = false;
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                CurrentTime.Text = DateTime.Now.ToString();
                ListBoxMESmsg.ItemsSource = null;
                ListBoxMESmsg.ItemsSource = MESmsg.lstMESmsg.OrderByDescending(x => x.LogDateTime).Take(10).Select(x => x.translation).ToList();
                if ((DateTime.Now - lastRefreshUI).TotalMinutes > 720 && CheckComputerFreeState.GetLastInputTimeMinute() > 720 && !PopedMonitorPage)
                {
                    lastRefreshUI = DateTime.Now;
                    PopedMonitorPage = true;
                    LoginResponse loginResponse = ServiceHelper.Current.ExecuteLogin("Guest", "Guest");
                    if (!loginResponse.IsError)
                    {
                        ATL.UI.Core.AppContext.Current.Reset();
                        ATL.UI.Core.AppContext.Current.Name = loginResponse.Name;
                        ATL.UI.Core.AppContext.Current.UserId = loginResponse.UserId;

                        if (!string.IsNullOrEmpty(loginResponse.PermissionCodes))
                        {
                            ATL.UI.Core.AppContext.Current.PermissionCodes.AddRange(loginResponse.PermissionCodes.Split(','));
                        }
                    }
                    StartResetMenus();
                    if (UserDefineVariableInfo.DicVariables["AutoPopMonitorPage"].ToString() == "1")
                    {
                        if (monitor == null)
                        {
                            monitor = new MonitorPage();
                            monitor.Closed += new EventHandler(monitor_Closed);
                            monitor.sendMessage += GotoNewPage;
                            monitor.ShowDialog();
                            monitor = null;
                        } 
                    }
                }
                else
                {
                    PopedMonitorPage = false;
                }
            }
            catch(Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
        }

        private void GotoInputPage(object sender, InterfaceClient.A007EventArgs e)
        {
            if (e != null)
            {
                ATL.MES.A007.Root root = e.root;
                this.Dispatcher.Invoke(new Action(() => 
                mainPage.Navigate(new Uri("pack://application:,,,/ATL.UI;component/MES/InputParaDownloadPage.xaml", UriKind.Absolute))));

            }
        }
        private DeviceMainPage CurMainPage
        {
            get
            {
                if (ModVariable.deviceMainPage == null)
                    ModVariable.deviceMainPage = new DeviceMainPage();
                return ModVariable.deviceMainPage;
            }
        }

        public static WinformMainPage CurTestWinformPage
        {
            get
            {
                if (ModVariable.testFormPage == null)
                    ModVariable.testFormPage = new WinformMainPage();
                return ModVariable.testFormPage;
            }
        }

        public static WinformUserControlPage CurTestUserControlPage
        {
            get
            {
                if (ModVariable.testUserControlPage == null)
                    ModVariable.testUserControlPage = new WinformUserControlPage();
                return ModVariable.testUserControlPage;
            }
        }
		
        private DashBoardPage DashBoard
        {
            get
            {
                if (ModVariable.dashBoardPage == null)
                    ModVariable.dashBoardPage = new DashBoardPage();
                return ModVariable.dashBoardPage;
            }
        }

        private MotionControlPage MotionControl
        {
            get
            {
                if (ModVariable.motionControlPage == null)
                    ModVariable.motionControlPage = new MotionControlPage();
                return ModVariable.motionControlPage;
            }
        }

        private XrayTubeControlPage XrayTubeControl
        {
            get
            {
                if (ModVariable.xrayTubeControlPage == null)
                    ModVariable.xrayTubeControlPage = new XrayTubeControlPage();
                return ModVariable.xrayTubeControlPage;
            }
        }

        private CameraSettingPage CameraSeeting
        {
            get
            {
                if (ModVariable.cameraSettingPage == null)
                    ModVariable.cameraSettingPage = new CameraSettingPage();
                return ModVariable.cameraSettingPage;
            }
        }

        private CameraCaliPage CameraCalibration
        {
            get
            {
                if (ModVariable.cameraCaliPage == null)
                    ModVariable.cameraCaliPage = new CameraCaliPage();
                return ModVariable.cameraCaliPage;
            }
        }

        private CheckParamsSettingsPage CheckParamsSettings
        {
            get
            {
                if (ModVariable.checkParamsSettingsPage == null)
                    ModVariable.checkParamsSettingsPage = new CheckParamsSettingsPage();
                return ModVariable.checkParamsSettingsPage;
            }
        }

        private InspectTestPage InspectTest
        {
            get
            {
                if (ModVariable.inspectTestPage == null)
                    ModVariable.inspectTestPage = new InspectTestPage();
                return ModVariable.inspectTestPage;
            }
        }

        

        #region 生产监控 双击跳转导航事件
        private MonitorPage monitor;
        void monitor_Closed(object sender, EventArgs e)
        {
            monitor = null;
        }
        
#endregion

        private XrayCheckPage xrayCheckPage;
        void xrayCheckPage_Closed(object sender, EventArgs e)
        {
            xrayCheckPage = null;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ATL.Core.Core.SoftClosing == false)
            {
                MessageBox.Show("禁止通过这种方式关闭窗体");
                e.Cancel = true;
            }
        }
    }
}
