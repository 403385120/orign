using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
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
using ATL.UI.SystemSetting;
using ZYXray;
using ZYXray.ViewModels;
using ZYXray.Models;
using XRayClient.VisionSysWrapper;
using ZY.XRayTube;
using ZY.BarCodeReader;
using ZY.Logging;
using XRayClient.Core;

using ZY.MitutoyoReader;
using ZY.Vision;
using ZYXray.Winform;
using System.IO;
using ZY.Systems;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ZY.DAL;
using ZY.Model;
using ZY.BLL;

namespace PTF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow current = new MainWindow();

        public static MainWindow Current
        {
            get
            {
                return current;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            //BeckhoffTwinCAT.PLC plc = new BeckhoffTwinCAT.PLC();
            loadMainPage();
            //LogHelper.Info("系统启动");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogHelper.Info("系统启动");
            }
            else
            {
                LogHelper.Info("System startup ");
            }
            LoginManagerPage.resetMenus += StartResetMenus;
            StationStatePage.sendMessage += GotoNewPage;
            InterfaceClient.A007event += GotoInputPage;
            this.RefreshTimer.Interval = new TimeSpan(0, 0, 1);
            this.RefreshTimer.Tick += new EventHandler(this.RefreshTimer_Tick);
            this.RefreshTimer.Start();
            //MainWindow.Current.mainPage.Navigate(new DataViewPage());//加载数据表格界面
            //MainWindow.Current.mainPage.Navigate(new ChannelCheckPage());//加载通道点检界面

        }
        public static void StartResetMenus()
        {
            MainWindow.Current.ResetMenus();
            MainWindow.Current.LoadMenus();
        }

        public static void GotoNewPage(string url)
        {
            PageFrame.Navigate(new Uri(url, UriKind.Absolute));
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
            frmSettings frm = new frmSettings();
            frm.DataRefresh();
            ThreadPool.QueueUserWorkItem(state => HardDiskInfo());
            InputParaDownloadPage.InputParaDownloadEvent += downLoadInputParaToPLC;
            LoginManagerPage.startRun += login;
            InterfaceClient.a039LoginEvent += A039LoginEvent;
            //SystemConfig.MainClose += ModVariable.MainClose;
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
            ThreadPool.QueueUserWorkItem(DoWork);
            //ThreadPool.QueueUserWorkItem(PlcDoWork);

            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                biMesInfo.Content = "MES消息";
                biTime.Text = "当前时间:";
                biModel.Text = "当前Model:";
                biOperator.Text = "操作员:";
            }
            else
            {
                biMesInfo.Content = "MESmsg";
                biTime.Text = "CurrentTime:";
                biModel.Text = "CurrentModel:";
                biOperator.Text = "Operator:";
            }
            LabMESstatus.FontSize = 18;

            MainWindow.Current.mainPage.Navigate(ChannelCheck);//加载通道点检界面
            MainWindow.Current.mainPage.Navigate(TestCodes);//
            mainPage.Navigate(DashBoard);
            MainWindow.Current.mainPage.Navigate(DataView);//加载数据表格界面
            MainWindow.Current.mainPage.Navigate(IVOCVParamsSettings);
            MainWindow.Current.mainPage.Navigate(MDIPPGParamsSettings);

            ZYXray.ModVariable.init();
        }
        public void HardDiskInfo()
        {
            while (true)
            {
                string dirD = Common.OrigImageFile;
                DriveInfo[] drives = DriveInfo.GetDrives();
                DirectoryInfo dir = new DirectoryInfo(dirD);
                try
                {
                    //if (DateTime.Now.Hour == 7 && DateTime.Now.Minute > 0)//每天7:30清理图片
                    //{
                    new Thread(new ThreadStart(delegate
                    {
                        if (drives[1].IsReady)
                        {
                            var val1 = (double)drives[1].TotalSize / 1024 / 1024;
                            var val2 = (double)drives[1].TotalFreeSpace / 1024 / 1024;
                            double memory = val2 / val1 * 100;
                            List<int> list = new List<int>();
                            List<int> listNum = new List<int>();
                            List<string> listStrName = new List<string>();
                            if (memory < Common.MinMemory)//硬盘、可用内存小于10%
                            {
                                if (Directory.Exists(dirD))
                                {
                                    FileSystemInfo[] files = dir.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                                                                                      //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                                    foreach (FileSystemInfo FSys in files)
                                    {
                                        list.Add(Convert.ToInt32(FSys.Name));
                                    }
                                    if (list.Count < 2)
                                    {
                                        App.Current.Dispatcher.Invoke(delegate ()
                                        {
                                            string errmsg = $"硬盘内存可用空间不足{ Common.MinMemory}%，原图只有一个文件夹,请检查其它文件夹图片并手动清理!";
                                            MessageBox.Show(errmsg);
                                        });
                                    }
                                    else
                                    {
                                        IEnumerable<int> num = from a in list orderby a select a;
                                        foreach (int item in num)
                                        {
                                            listNum.Add(item);
                                        }
                                        if (listNum.Count > 0)
                                        {
                                            string FileDel = dirD + listNum[0];
                                            DirectoryInfo dirDel = new DirectoryInfo(FileDel);
                                            FileSystemInfo[] filesDel = dirDel.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                                            foreach (FileSystemInfo FSys in filesDel)
                                            {
                                                Directory.Delete(FileDel + "\\" + FSys.Name, true);
                                                listStrName.Add(FSys.Name);
                                            }
                                            if (listStrName.Count == 0)
                                            {
                                                Directory.Delete(FileDel, true);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    })).Start();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                Thread.Sleep(30000);
            }
        }
        /// <summary>
        /// 登录界面登录事件
        /// </summary>
        private void login()
        {
            string userName = ATL.Core.Core.UserName;
            string userLever = ATL.MES.UserInfo.UserLevel;
        }

        /// <summary>
        /// A039登录事件
        /// </summary>
        /// <param name="info"></param>
        private void A039LoginEvent(ATL.MES.A040.ResponseInfo info)
        {

        }

        /// <summary>
        /// Input下发界面里下发成功后会执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void downLoadInputParaToPLC(object sender, InputParaDownloadPage.InputParaDownloadEventArgs arg)
        {
            List<InputInformation> lstInput = arg.inputInformations;

            MotionControlVm.Instance.LoadWorkParams(CheckParamsConfig.Instance, HardwareConfig.Instance.CameraParams1, HardwareConfig.Instance.CameraParams2);

            //MotionControlVm.Instance.WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_Sorting");
        }

        public void LoadMenus()
        {
            this.BuildMenus(AppContext.Current.GetPermissionNodes(AppContext.Current.PermissionCodes));
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
            this.mainNodes.Clear();
            foreach (PermissionNode permissionNode in permissionNodes)
            {
                ATL.UI.Core.MenuNode menuNode1 = new ATL.UI.Core.MenuNode();
                Button button1 = new Button();
                if (permissionNode.Text == "设备控制" || permissionNode.Text == "Machine Control")
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
            string msg = string.Empty;
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                msg = "确认要退出软件？";
            }
            else
            {
                msg = "Are you sure exit?";
            }
            MessageBoxResult result = MessageBox.Show(msg, "Warn", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes && ATL.Core.Core.UserName == "Guest")
            {
                //MessageBox.Show("退出本程序前，请先登入账户", "提示", MessageBoxButton.OK);
                //LogInDB.Info("关闭软件时，权限不够！");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    MessageBox.Show("退出本程序前，请先登入账户", "提示", MessageBoxButton.OK);
                    LogInDB.Info("关闭软件时，权限不够！");
                }
                else
                {
                    MessageBox.Show("Please log in to your account before exiting this program ", "prompt", MessageBoxButton.OK);
                    LogInDB.Info("No right to shut down the software ！");
                }
                return;
            }
            else if (result == MessageBoxResult.No)
            {
                //LogInDB.Info("关闭软件时，取消关闭！");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Info("关闭软件时，取消关闭！");
                }
                else
                {
                    LogInDB.Info("When closing the software, cancel closing ！");
                }
                return;
            }

            ATL.Core.Core.SoftClosing = true;

            CodeReaderIF.UnInit_ClientConnet();
            XRayTubeIF.UnInit();
            MitutoyoReaderIF.UnInit();

            //VisionSysWrapperIF.CameraUnInit();
            //TODO: 释放相机 ZhangKF 2021-3-18
            CameraHelper.CloseCamera();

            ModVariable.MainClose(null, null);
            ATL.Engine.PLC plc = new ATL.Engine.PLC();
            plc.WriteByVariableName("SoftwareOnState", 0);


            //LogInDB.Info("软件关闭");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Info("软件关闭");
            }
            else
            {
                LogInDB.Info("Software shutdown");
            }
            Thread.Sleep(1500);
            Application.Current.Shutdown();
            Environment.Exit(-1);
        }
        /// <summary>
        /// 参数下发线程
        /// </summary>
        /// <param name="stateInfo"></param>
        private void DoWork(object stateInfo)
        {
            while (!ATL.Core.Core.SoftClosing)
            {
                Dispatcher.Invoke(delegate ()
                {
                    tbUserName.Text = ATL.MES.UserInfo.UserName;
                    if (DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Count > 0 && ATL.Core.Core.CheckedOK)
                    {
                        tbModel.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;

                        if (Common.IsProductType)
                        {
                            List<ParametricDataArray> lst = new List<ParametricDataArray>();
                            JObject jobject = JObject.Parse(Convert.ToString(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].A007Jason));
                            var jData = jobject["RequestInfo"];
                            var ret = jData["ParameterInfo"];
                            var jsonData = JsonConvert.SerializeObject(ret);
                            JArray userArry = (JArray)JsonConvert.DeserializeObject(jsonData);
                            List<JsonObject> list = new List<JsonObject>();

                            //获取树形用户json字符串
                            ProductTypeDAL type = new ProductTypeDAL();
                            foreach (var item in userArry)
                            {
                                JObject j = JObject.Parse(item.ToString());
                                JsonObject model = new JsonObject();
                                model.ParamID = j["ParamID"].ToString();
                                model.StandardValue = j["StandardValue"].ToString();
                                model.UpperLimitValue = j["UpperLimitValue"].ToString();
                                model.LowerLimitValue = j["LowerLimitValue"].ToString();
                                model.Description = j["Description"].ToString();
                                list.Add(model);
                            }
                            var arrProductType = tbModel.Text.Split('-');
                            string productType = string.Empty;
                            string errmsg = string.Empty;
                            var listData = ProductTypeBLL.GetList($"Product_type='{tbModel.Text}'", "Iden", 200, ref errmsg);
                            ProductType p = new ProductType();
                            if (listData.Count == 0)
                            {
                                if (arrProductType.Length > 2)
                                {
                                    productType = arrProductType[2];
                                }
                            }
                            else
                            {
                                productType = tbModel.Text;
                                p = listData[0];
                            }
                           

                            errmsg = string.Empty;
                            if (!string.IsNullOrEmpty(productType))
                            {
                                p.Product_type = productType;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    switch (list[i].ParamID)
                                    {
                                        case "251":
                                            p.Define72 = double.Parse(list[i].LowerLimitValue.ToString());
                                            p.Define73 = double.Parse(list[i].UpperLimitValue);
                                            p.Define74 = double.Parse(list[i].LowerLimitValue.ToString());
                                            p.Define75 = double.Parse(list[i].UpperLimitValue.ToString()); 
                                            break;
                                        case "52196":
                                            double Define76 = 0;
                                            double.TryParse(list[i].StandardValue,out Define76);   
                                            p.Define76 = int.Parse(Define76.ToString());
                                            break;
                                        case "52197":
                                            double Define79 = 0;
                                            double.TryParse(list[i].StandardValue, out Define79);
                                            p.Define79 = int.Parse(Define79.ToString());
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                CheckParamsConfig.Instance.MinLengthHead = float.Parse(p.Define72.GetValueOrDefault(0).ToString());//头部最小值
                                CheckParamsConfig.Instance.MaxLengthHead = float.Parse(p.Define73.GetValueOrDefault(0).ToString());//头部最大值
                                CheckParamsConfig.Instance.MinLengthTail = float.Parse(p.Define74.GetValueOrDefault(0).ToString());//尾部最小值
                                CheckParamsConfig.Instance.MaxLengthTail = float.Parse(p.Define75.GetValueOrDefault(0).ToString());//尾部最大值
                                CheckParamsConfig.Instance.TotalLayer = int.Parse(p.Define76.GetValueOrDefault(0).ToString());//AC层数
                                CheckParamsConfig.Instance.TotalLayersBD = int.Parse(p.Define79.GetValueOrDefault(0).ToString());//BD层数
                                ProductTypeBLL.Update(p, ref errmsg);
                            }
                        }
                    }

                    if (Station.Current.MESState == "OK")
                    {

                    }
                    else
                    {

                    }
                });

                Thread.Sleep(5000);
            }
        }
        public class JsonObject
        {
            public string ParamID { get; set; }
            public string StandardValue { get; set; }
            public string UpperLimitValue { get; set; }
            public string LowerLimitValue { get; set; }
            public string Description { get; set; }

        }

        //private void btnSwitchUser_Click(object sender, RoutedEventArgs e)
        //{
        //    LoginWindow loginWindow = new LoginWindow();
        //    loginWindow.Show();
        //    Application.Current.MainWindow = loginWindow;
        //    this.Close();
        //}

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private DashBoardPage DashBoard
        {
            get
            {
                if (ZYXray.ModVariable.dashBoardPage == null)
                    ZYXray.ModVariable.dashBoardPage = new DashBoardPage();
                return ZYXray.ModVariable.dashBoardPage;
            }
        }

        private MotionControlPage MotionControl
        {
            get
            {
                if (ZYXray.ModVariable.motionControlPage == null)
                    ZYXray.ModVariable.motionControlPage = new MotionControlPage();
                return ZYXray.ModVariable.motionControlPage;
            }
        }

        private XrayTubeControlPage XrayTubeControl
        {
            get
            {
                if (ZYXray.ModVariable.xrayTubeControlPage == null)
                    ZYXray.ModVariable.xrayTubeControlPage = new XrayTubeControlPage();
                return ZYXray.ModVariable.xrayTubeControlPage;
            }
        }

        private CameraSettingPage CameraSeeting
        {
            get
            {
                if (ZYXray.ModVariable.cameraSettingPage == null)
                    ZYXray.ModVariable.cameraSettingPage = new CameraSettingPage();
                return ZYXray.ModVariable.cameraSettingPage;
            }
        }

        private CameraCaliPage CameraCalibration
        {
            get
            {
                if (ZYXray.ModVariable.cameraCaliPage == null)
                    ZYXray.ModVariable.cameraCaliPage = new CameraCaliPage();
                return ZYXray.ModVariable.cameraCaliPage;
            }
        }

        private CheckParamsSettingsPage CheckParamsSettings
        {
            get
            {
                if (ZYXray.ModVariable.checkParamsSettingsPage == null)
                    ZYXray.ModVariable.checkParamsSettingsPage = new CheckParamsSettingsPage();
                return ZYXray.ModVariable.checkParamsSettingsPage;
            }
        }

        private InspectTestPage InspectTest
        {
            get
            {
                if (ZYXray.ModVariable.inspectTestPage == null)
                    ZYXray.ModVariable.inspectTestPage = new InspectTestPage();
                return ZYXray.ModVariable.inspectTestPage;
            }
        }

        private ThicknessCheckPage ThicknessCheck
        {
            get
            {
                if (ZYXray.ModVariable.thicknessCheck == null)
                    ZYXray.ModVariable.thicknessCheck = new ThicknessCheckPage();
                return ZYXray.ModVariable.thicknessCheck;
            }
        }
        private DimensionCheckPage DimensionCheck
        {
            get
            {
                if (ZYXray.ModVariable.dimensionCheck == null)
                    ZYXray.ModVariable.dimensionCheck = new DimensionCheckPage();
                return ZYXray.ModVariable.dimensionCheck;
            }
        }

        private ChannelCheckPage ChannelCheck
        {
            get
            {
                if (ZYXray.ModVariable.channelCheckPage == null)
                    ZYXray.ModVariable.channelCheckPage = new ChannelCheckPage();
                return ZYXray.ModVariable.channelCheckPage;
            }
        }

        private IVOCVParamsSettingsPage IVOCVParamsSettings
        {
            get
            {
                if (ZYXray.ModVariable.iVOCVParamsSettingsPage == null)
                    ZYXray.ModVariable.iVOCVParamsSettingsPage = new IVOCVParamsSettingsPage();
                return ZYXray.ModVariable.iVOCVParamsSettingsPage;
            }
        }

        private MDIPPGParamsSettingsPage MDIPPGParamsSettings
        {
            get
            {
                if (ZYXray.ModVariable.mDIPPGParamsSettingsPage == null)
                    ZYXray.ModVariable.mDIPPGParamsSettingsPage = new MDIPPGParamsSettingsPage();
                return ZYXray.ModVariable.mDIPPGParamsSettingsPage;
            }
        }
        private MDIPPGParamsSettingsPage1 MDIPPGParamsSettings1
        {
            get
            {
                if (ZYXray.ModVariable.mDIPPGParamsSettingsPage1 == null)
                    ZYXray.ModVariable.mDIPPGParamsSettingsPage1 = new MDIPPGParamsSettingsPage1();
                return ZYXray.ModVariable.mDIPPGParamsSettingsPage1;
            }
        }

        private DataViewPage DataView
        {
            get
            {
                if (ZYXray.ModVariable.dataViewPage == null)
                    ZYXray.ModVariable.dataViewPage = new DataViewPage();
                return ZYXray.ModVariable.dataViewPage;
            }
        }
        private DataViewCommunication DataViewCommunication
        {
            get
            {
                if (ZYXray.ModVariable.DataViewCommunication == null)
                    ZYXray.ModVariable.DataViewCommunication = new DataViewCommunication();
                return ZYXray.ModVariable.DataViewCommunication;
            }
        }

        private DataViewSetting DataViewSetting
        {
            get
            {
                if (ZYXray.ModVariable.DataViewSetting == null)
                    ZYXray.ModVariable.DataViewSetting = new DataViewSetting();
                return ZYXray.ModVariable.DataViewSetting;
            }
        }
        private DataViewRecheck DataViewRecheck
        {
            get
            {
                if (ZYXray.ModVariable.DataViewRecheck == null)
                    ZYXray.ModVariable.DataViewRecheck = new DataViewRecheck();
                return ZYXray.ModVariable.DataViewRecheck;
            }
        }

        private TestCodesPage TestCodes
        {
            get
            {
                if (ZYXray.ModVariable.testCodesPage == null)
                    ZYXray.ModVariable.testCodesPage = new ZYXray.TestCodesPage();
                return ZYXray.ModVariable.testCodesPage;
            }
        }


        private TestStationInfoPage TestStationInfo
        {
            get
            {
                if (ZYXray.ModVariable.testStationInfo == null)
                    ZYXray.ModVariable.testStationInfo = new TestStationInfoPage();
                return ZYXray.ModVariable.testStationInfo;
            }
        }

        private ReModelPage ReModel
        {
            get
            {
                if (ZYXray.ModVariable.reModelPage == null)
                    ZYXray.ModVariable.reModelPage = new ReModelPage();
                return ZYXray.ModVariable.reModelPage;
            }
        }
        private XrayCheckPage xrayCheckPage;
        void xrayCheckPage_Closed(object sender, EventArgs e)
        {
            xrayCheckPage = null;
        }
        private void ChildMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MenuNode tag = (MenuNode)button.Tag;
            if (lastSelectdChildNode != null && tag != this.lastSelectdChildNode)
            {
                this.lastSelectdChildNode.Button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            this.lastSelectdChildNode = tag;
            tag.Button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ATL.UI;component/Assets/menu_child_button_bg.png", UriKind.Absolute)));
            if (tag.PermissionNode.Code == "DeviceOverview.Monitor")
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
                CameraCalibration.DataContext = ((ZYXray.ViewModels.CameraSetingsVm)(CameraSeeting.DataContext));
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
            else if (tag.PermissionNode.Code == "DeviceControl.DashBoard")
            {
                mainPage.Navigate(DashBoard);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.ManualRecheck")
            {
                UserDefineVariableInfo.DicVariables["IsRecheckMode"] = 1;
                if (xrayCheckPage == null)
                {
                    xrayCheckPage = new XrayCheckPage();
                    xrayCheckPage.Closed += new EventHandler(xrayCheckPage_Closed);
                    xrayCheckPage.Show();
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
                    xrayCheckPage.Show();
                    xrayCheckPage = null;
                }
            }
            else if (tag.PermissionNode.Code == "DeviceControl.ThicknessCheck")
            {
                mainPage.Navigate(ThicknessCheck);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.DimensionCheck")
            {
                mainPage.Navigate(DimensionCheck);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.TestCodes")
            {
                mainPage.Navigate(TestCodes);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.ChannelCheck")
            {
                mainPage.Navigate(ChannelCheck);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.DataView")
            {
                mainPage.Navigate(DataView);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.DataViewSetting")
            {
                mainPage.Navigate(DataViewSetting);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.DataViewRecheck")
            {
                mainPage.Navigate(DataViewRecheck);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.DataViewCommunication")
            {
                mainPage.Navigate(DataViewCommunication);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.IVOCVParamsSettings")
            {
                mainPage.Navigate(IVOCVParamsSettings);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.MDIPPGParamsSettings")
            {
                mainPage.Navigate(MDIPPGParamsSettings);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.MDIPPGParamsSettings1")
            {
                mainPage.Navigate(MDIPPGParamsSettings1);
            }

            else if (tag.PermissionNode.Code == "DeviceControl.TestStationInfo")
            {
                mainPage.Navigate(TestStationInfo);
            }
            else if (tag.PermissionNode.Code == "DeviceControl.ReModel")
            {
                mainPage.Navigate(ReModel);
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
            this.lastSelectdChildNode.Button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
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
                if (Station.Current.MESState == "OK")
                {
                    LabMESstatus.Content = ATL.Common.StringResources.IsDefaultLanguage ? "MES在线" : "MES Online";
                    LabMESstatus.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    LabMESstatus.Content = ATL.Common.StringResources.IsDefaultLanguage ? "MES离线" : "MES Offline";
                    LabMESstatus.Foreground = new SolidColorBrush(Colors.Red);
                }

                CurrentTime.Text = DateTime.Now.ToString();
                ListBoxMESmsg.ItemsSource = null;
                ListBoxMESmsg.ItemsSource = MESmsg.lstMESmsg.OrderByDescending(x => x.LogDateTime).Take(10).Select(x => x.translation).ToList();
                int AutoPopMonitorPageMinutes = 3;
                if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("AutoPopMonitorPageMinutes")
                    && int.TryParse(UserDefineVariableInfo.DicVariables["AutoPopMonitorPageMinutes"].ToString(), out AutoPopMonitorPageMinutes)
                    && AutoPopMonitorPageMinutes < 3)
                {
                    AutoPopMonitorPageMinutes = 3;
                }
                if ((DateTime.Now - lastRefreshUI).TotalMinutes > 600 && CheckComputerFreeState.GetLastInputTimeMinute() > AutoPopMonitorPageMinutes && !PopedMonitorPage)
                {
                    lastRefreshUI = DateTime.Now;
                    PopedMonitorPage = true;
                    LoginResponse loginResponse = ServiceHelper.Current.ExecuteLogin("Guest", "Guest");
                    if (!loginResponse.IsError)
                    {
                        ATL.UI.Core.AppContext.Current.Reset();
                        ATL.Core.Core.UserName = loginResponse.Name;
                        ATL.Core.Core.DatabaseUserId = loginResponse.UserId;

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
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
        }

        #region 生产监控 双击跳转导航事件
        private MonitorPage monitor;
        void monitor_Closed(object sender, EventArgs e)
        {
            monitor = null;
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ATL.Core.Core.SoftClosing == false)
            {
                //MessageBox.Show("禁止通过这种方式关闭窗体");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    MessageBox.Show("禁止通过这种方式关闭窗体");
                }
                else
                {
                    MessageBox.Show("Do not close the form in this way ");
                }
                e.Cancel = true;
            }
        }
    }
}
