using ATL.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Esquel.BaseManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XRayClient.Core;
using ZY.BLL;
using ZY.DAL;
using ZY.Model;
using ZY.Systems;
using ZYXray.Model;

namespace ZYXray.Winform
{

    public partial class frmProductType : XtraForm
    {
        public static frmProductType Current = null;
        private string errMsg = string.Empty;

        public frmProductType()
        {
            InitializeComponent();
        }

        private ProductType GetModel()
        {
            ProductType model = new ProductType();
            model.MI = cbo_MI.Text;
            model.Product_type = txtProdyctType.Text.Trim();
            model.Define1 = double.Parse(spinEdit1.Value.ToString());
            model.Define2 = double.Parse(spinEdit2.Value.ToString());
            model.Define3 = double.Parse(spinEdit3.Value.ToString());
            model.Define4 = double.Parse(spinEdit4.Value.ToString());
            model.Define5 = double.Parse(spinEdit5.Value.ToString());
            model.Define6 = double.Parse(spinEdit6.Value.ToString());
            model.Define7 = double.Parse(spinEdit7.Value.ToString());
            model.Define8 = double.Parse(spinEdit8.Value.ToString());
            model.Define9 = double.Parse(spinEdit9.Value.ToString());
            model.Define10 = double.Parse(spinEdit10.Value.ToString());
            model.Define11 = double.Parse(spinEdit11.Value.ToString());
            model.Define12 = double.Parse(spinEdit12.Value.ToString());
            model.Define13 = double.Parse(spinEdit13.Value.ToString());
            model.Define14 = double.Parse(spinEdit14.Value.ToString());
            model.Define15 = double.Parse(spinEdit15.Value.ToString());
            model.Define16 = double.Parse(spinEdit16.Value.ToString());
            model.Define17 = double.Parse(spinEdit17.Value.ToString());
            model.Define18 = double.Parse(spinEdit18.Value.ToString());
            model.Define19 = double.Parse(spinEdit19.Value.ToString());
            model.Define20 = double.Parse(spinEdit20.Value.ToString());
            model.Define21 = double.Parse(spinEdit21.Value.ToString());
            model.Define22 = double.Parse(spinEdit22.Value.ToString());
            model.Define23 = double.Parse(spinEdit23.Value.ToString());
            model.Define24 = double.Parse(spinEdit24.Value.ToString());
            model.Define25 = double.Parse(spinEdit25.Value.ToString());
            model.Define26 = double.Parse(spinEdit26.Value.ToString());
            model.Define27 = double.Parse(spinEdit27.Value.ToString());
            model.Define28 = double.Parse(spinEdit28.Value.ToString());
            model.Define29 = double.Parse(spinEdit29.Value.ToString());
            model.Define30 = double.Parse(spinEdit30.Value.ToString());
            model.Define31 = double.Parse(spinEdit31.Value.ToString());
            model.Define32 = double.Parse(spinEdit32.Value.ToString());
            model.Define33 = double.Parse(spinEdit33.Value.ToString());
            model.Define34 = double.Parse(spinEdit34.Value.ToString());
            model.Define35 = double.Parse(spinEdit35.Value.ToString());
            model.Define36 = double.Parse(spinEdit36.Value.ToString());
            model.Define37 = double.Parse(spinEdit37.Value.ToString());
            model.Define38 = double.Parse(spinEdit38.Value.ToString());
            model.Define39 = double.Parse(spinEdit39.Value.ToString());
            model.Define40 = double.Parse(spinEdit40.Value.ToString());
            model.Define41 = double.Parse(spinEdit41.Value.ToString());
            model.Define42 = double.Parse(spinEdit42.Value.ToString());
            model.Define43 = double.Parse(spinEdit43.Value.ToString());
            model.Define44 = double.Parse(spinEdit44.Value.ToString());
            model.Define45 = int.Parse(spinEdit45.Value.ToString());
            model.Define46 = double.Parse(spinEdit46.Value.ToString());
            model.Define47 = double.Parse(spinEdit47.Value.ToString());
            model.Define48 = double.Parse(spinEdit48.Value.ToString());
            model.Define49 = double.Parse(spinEdit49.Value.ToString());
            model.Define50 = double.Parse(spinEdit50.Value.ToString());
            model.Define51 = double.Parse(spinEdit51.Value.ToString());
            model.Define52 = double.Parse(spinEdit52.Value.ToString());
            model.Define53 = double.Parse(spinEdit53.Value.ToString());
            model.Define54 = double.Parse(spinEdit54.Value.ToString());
            model.Define55 = double.Parse(spinEdit55.Value.ToString());
            model.Define56 = double.Parse(spinEdit56.Value.ToString());
            model.Define57 = double.Parse(spinEdit57.Value.ToString());
            model.Define58 = double.Parse(spinEdit58.Value.ToString());
            model.Define59 = double.Parse(spinEdit59.Value.ToString());
            model.Define60 = double.Parse(spinEdit60.Value.ToString());
            model.Define61 = double.Parse(spinEdit61.Value.ToString());
            model.Define62 = double.Parse(spinEdit62.Value.ToString());
            model.Define63 = double.Parse(spinEdit63.Value.ToString());
            model.Define64 = double.Parse(spinEdit64.Value.ToString());
            model.Define65 = double.Parse(spinEdit65.Value.ToString());
            model.Define66 = double.Parse(spinEdit66.Value.ToString());
            model.Define67 = int.Parse(spinEdit67.Value.ToString());
            model.Define68 = int.Parse(spinEdit68.Value.ToString());
            model.Define69 = int.Parse(spinEdit69.Value.ToString());
            model.Define70 = int.Parse(spinEdit70.Value.ToString());
            model.Define71 = int.Parse(spinEdit71.Value.ToString());
            model.Define72 = double.Parse(spinEdit72.Value.ToString());
            model.Define73 = double.Parse(spinEdit73.Value.ToString());
            model.Define74 = double.Parse(spinEdit74.Value.ToString());
            model.Define75 = double.Parse(spinEdit75.Value.ToString());
            model.Define76 = int.Parse(spinEdit76.Value.ToString());
            model.Define77 = int.Parse(spinEdit77.Value.ToString());
            model.Define78 = int.Parse(spinEdit78.Value.ToString());
            model.Define79 = int.Parse(spinEdit79.Value.ToString());
            model.Define80 = int.Parse(spinEdit80.Value.ToString());
            model.Define81 = int.Parse(spinEdit81.Value.ToString());
            model.CreateDate = DateTime.Now;
            model.CreateUser = ATL.MES.UserInfo.UserName;
            model.BarcodeLenth = txtBarcodeLenth.Text.Trim();
            if (!string.IsNullOrEmpty(Convert.ToString(txtProdyctType.Tag)))
                model.Iden = Convert.ToInt32(txtProdyctType.Tag);

            return model;
        }

        private ProductTypeHistory GetModelHistory()
        {
            ProductTypeHistory model = new ProductTypeHistory();
            model.Product_type = txtProdyctType.Text.Trim();
            model.MI = cbo_MI.Text;
            model.Define1 = double.Parse(spinEdit1.Value.ToString());
            model.Define2 = double.Parse(spinEdit2.Value.ToString());
            model.Define3 = double.Parse(spinEdit3.Value.ToString());
            model.Define4 = double.Parse(spinEdit4.Value.ToString());
            model.Define5 = double.Parse(spinEdit5.Value.ToString());
            model.Define6 = double.Parse(spinEdit6.Value.ToString());
            model.Define7 = double.Parse(spinEdit7.Value.ToString());
            model.Define8 = double.Parse(spinEdit8.Value.ToString());
            model.Define9 = double.Parse(spinEdit9.Value.ToString());
            model.Define10 = double.Parse(spinEdit10.Value.ToString());
            model.Define11 = double.Parse(spinEdit11.Value.ToString());
            model.Define12 = double.Parse(spinEdit12.Value.ToString());
            model.Define13 = double.Parse(spinEdit13.Value.ToString());
            model.Define14 = double.Parse(spinEdit14.Value.ToString());
            model.Define15 = double.Parse(spinEdit15.Value.ToString());
            model.Define16 = double.Parse(spinEdit16.Value.ToString());
            model.Define17 = double.Parse(spinEdit17.Value.ToString());
            model.Define18 = double.Parse(spinEdit18.Value.ToString());
            model.Define19 = double.Parse(spinEdit19.Value.ToString());
            model.Define20 = double.Parse(spinEdit20.Value.ToString());
            model.Define21 = double.Parse(spinEdit21.Value.ToString());
            model.Define22 = double.Parse(spinEdit22.Value.ToString());
            model.Define23 = double.Parse(spinEdit23.Value.ToString());
            model.Define24 = double.Parse(spinEdit24.Value.ToString());
            model.Define25 = double.Parse(spinEdit25.Value.ToString());
            model.Define26 = double.Parse(spinEdit26.Value.ToString());
            model.Define27 = double.Parse(spinEdit27.Value.ToString());
            model.Define28 = double.Parse(spinEdit28.Value.ToString());
            model.Define29 = double.Parse(spinEdit29.Value.ToString());
            model.Define30 = double.Parse(spinEdit30.Value.ToString());
            model.Define31 = double.Parse(spinEdit31.Value.ToString());
            model.Define32 = double.Parse(spinEdit32.Value.ToString());
            model.Define33 = double.Parse(spinEdit33.Value.ToString());
            model.Define34 = double.Parse(spinEdit34.Value.ToString());
            model.Define35 = double.Parse(spinEdit35.Value.ToString());
            model.Define36 = double.Parse(spinEdit36.Value.ToString());
            model.Define37 = double.Parse(spinEdit37.Value.ToString());
            model.Define38 = double.Parse(spinEdit38.Value.ToString());
            model.Define39 = double.Parse(spinEdit39.Value.ToString());
            model.Define40 = double.Parse(spinEdit40.Value.ToString());
            model.Define41 = double.Parse(spinEdit41.Value.ToString());
            model.Define42 = double.Parse(spinEdit42.Value.ToString());
            model.Define43 = double.Parse(spinEdit43.Value.ToString());
            model.Define44 = double.Parse(spinEdit44.Value.ToString());
            model.Define45 = int.Parse(spinEdit45.Value.ToString());
            model.Define46 = double.Parse(spinEdit46.Value.ToString());
            model.Define47 = double.Parse(spinEdit47.Value.ToString());
            model.Define48 = double.Parse(spinEdit48.Value.ToString());
            model.Define49 = double.Parse(spinEdit49.Value.ToString());
            model.Define50 = double.Parse(spinEdit50.Value.ToString());
            model.Define51 = double.Parse(spinEdit51.Value.ToString());
            model.Define52 = double.Parse(spinEdit52.Value.ToString());
            model.Define53 = double.Parse(spinEdit53.Value.ToString());
            model.Define54 = double.Parse(spinEdit54.Value.ToString());
            model.Define55 = double.Parse(spinEdit55.Value.ToString());
            model.Define56 = double.Parse(spinEdit56.Value.ToString());
            model.Define57 = double.Parse(spinEdit57.Value.ToString());
            model.Define58 = double.Parse(spinEdit58.Value.ToString());
            model.Define59 = double.Parse(spinEdit59.Value.ToString());
            model.Define60 = double.Parse(spinEdit60.Value.ToString());
            model.Define61 = double.Parse(spinEdit61.Value.ToString());
            model.Define62 = double.Parse(spinEdit62.Value.ToString());
            model.Define63 = double.Parse(spinEdit63.Value.ToString());
            model.Define64 = double.Parse(spinEdit64.Value.ToString());
            model.Define65 = double.Parse(spinEdit65.Value.ToString());
            model.Define66 = double.Parse(spinEdit66.Value.ToString());
            model.Define67 = int.Parse(spinEdit67.Value.ToString());
            model.Define68 = int.Parse(spinEdit68.Value.ToString());
            model.Define69 = int.Parse(spinEdit69.Value.ToString());
            model.Define70 = int.Parse(spinEdit70.Value.ToString());
            model.Define71 = int.Parse(spinEdit71.Value.ToString());
            model.Define72 = double.Parse(spinEdit72.Value.ToString());
            model.Define73 = double.Parse(spinEdit73.Value.ToString());
            model.Define74 = double.Parse(spinEdit74.Value.ToString());
            model.Define75 = double.Parse(spinEdit75.Value.ToString());
            model.Define76 = int.Parse(spinEdit76.Value.ToString());
            model.Define77 = int.Parse(spinEdit77.Value.ToString());
            model.Define78 = int.Parse(spinEdit78.Value.ToString());
            model.Define79 = int.Parse(spinEdit79.Value.ToString());
            model.Define80 = int.Parse(spinEdit80.Value.ToString());
            model.Define81 = int.Parse(spinEdit81.Value.ToString());
            model.CreateDate = DateTime.Now;
            model.CreateUser = ATL.MES.UserInfo.UserName;
            model.BarcodeLenth = txtBarcodeLenth.Text.Trim();
            if (!string.IsNullOrEmpty(Convert.ToString(txtProdyctType.Tag)))
                model.Iden = Convert.ToInt32(txtProdyctType.Tag);

            return model;
        }

        private void SetEnabled(bool iFalg)
        {
            bbiCancel.Enabled = iFalg;
            bbiSave.Enabled = iFalg;
            bbiRefresh.Enabled = !iFalg;
            bbiAddNew.Enabled = !iFalg;
            bbiModify.Enabled = !iFalg;
            bbiDelete.Enabled = !iFalg;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            try
            {
                #region 上下限设定
                //var MinWeight_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MinWeight_MinValue", "0"));//前秤
                //var MinWeight_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MinWeight_MaxValue", "0"));//前秤
                //seMinWeight.Properties.MinValue = MinWeight_MinValue;
                //seMinWeight.Properties.MaxValue = MinWeight_MaxValue;


                //var MaxWeight_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MaxWeight_MinValue", "0"));
                //var MaxWeight_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MaxWeight_MaxValue", "0"));
                //seMaxWeight.Properties.MinValue = MaxWeight_MinValue;
                //seMaxWeight.Properties.MaxValue = MaxWeight_MaxValue;


                //var AfterWeight_LSL_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "AfterWeight_LSL_MinValue", "0")); //后秤
                //var AfterWeight_LSL_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "AfterWeight_LSL_MaxValue", "0")); //后秤
                //seAfterWeight_LSL.Properties.MinValue = AfterWeight_LSL_MinValue;
                //seAfterWeight_LSL.Properties.MaxValue = AfterWeight_LSL_MaxValue;


                //var AfterWeight_USL_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "AfterWeight_USL_MinValue", "0"));
                //var AfterWeight_USL_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "AfterWeight_USL_MaxValue", "0"));
                //seAfterWeight_USL.Properties.MinValue = AfterWeight_USL_MinValue;
                //seAfterWeight_USL.Properties.MaxValue = AfterWeight_USL_MaxValue;


                //var MinScope_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MinScope_MinValue", "0"));//一次注液量
                //var MinScope_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MinScope_MaxValue", "0"));//一次注液量
                //seMinScope.Properties.MinValue = MinScope_MinValue;
                //seMinScope.Properties.MaxValue = MinScope_MaxValue;

                //var MaxScope_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MaxScope_MinValue", "0"));
                //var MaxScope_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "MaxScope_MaxValue", "0"));
                //seMaxScope.Properties.MinValue = MaxScope_MinValue;
                //seMaxScope.Properties.MaxValue = MaxScope_MaxValue;

                //var InjSecond_LSL_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjSecond_LSL_MinValue", "0")); //二次注液量
                //var InjSecond_LSL_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjSecond_LSL_MaxValue", "0")); //二次注液量
                //spinEdit2.Properties.MinValue = InjSecond_LSL_MinValue;
                //spinEdit2.Properties.MaxValue = InjSecond_LSL_MaxValue;

                //var InjSecond_USL_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjSecond_USL_MinValue", "0"));
                //var InjSecond_USL_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjSecond_USL_MaxValue", "0"));
                //spinEdit1.Properties.MinValue = InjSecond_USL_MinValue;
                //spinEdit1.Properties.MaxValue = InjSecond_USL_MaxValue;

                //var InjVolume_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjVolume_MinValue", "0")); //保液量
                //var InjVolume_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjVolume_MaxValue", "0")); //保液量
                //seInjVolume.Properties.MinValue = InjVolume_MinValue;
                //seInjVolume.Properties.MaxValue = InjVolume_MaxValue;

                //var InjSecondVolume_MinValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjSecondVolume_MinValue", "0"));//二次保液量
                //var InjSecondVolume_MaxValue = Convert.ToDecimal(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "ProductType", "InjSecondVolume_MaxValue", "0"));//二次保液量
                //seInjSecondVolume.Properties.MinValue = InjSecondVolume_MinValue;
                //seInjSecondVolume.Properties.MaxValue = InjSecondVolume_MaxValue;




                #endregion

                //var _colMinWeight = gridView5.Columns.FirstOrDefault(o => o.FieldName.Equals("MinWeight"));
                //labelControl4.Visible = _colMinWeight == null ? false : _colMinWeight.Visible;
                //seMinWeight.Visible = _colMinWeight == null ? false : _colMinWeight.Visible;
                //if (_colMinWeight != null && _colMinWeight.Visible)
                //{
                //    labelControl4.Text = _colMinWeight.Caption;
                //    labelControl4.Refresh();
                //}




                #region 自动排列表单控件
                //switch (Setting.Factory)
                //{
                //    case Common.FactoryCode.ZZFN:
                //    case Common.FactoryCode.ZJFN:

                //    default:
                //        AutoFromControls();
                //        break;
                //}



                #endregion


                //  var list = IPTS.BLL.MachineMainBLL.GetList(string.Format("Factory='{0}'", Setting.Factory), ref errMsg);

                DataRefresh();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// 自动排列表单控件
        /// </summary>
        //private void AutoFromControls()
        //{
        //    this.panel3.Controls.Clear();
        //    //var fpBase = new FlowLayoutPanel();
        //    //var fp1 = GetFlowLayoutPanel(new Control[][]
        //    //{


        //    //});
        //    //fpBase.Controls.Add(fp1);

        //    var fp2 = GetFlowLayoutPanel(new Control[][]
        //    {
        //         new Control[]{labelControl6,txtProductNo},
        //        new Control[]{labelControl8,txtDefaultBarCode},
        //        new Control[]{labelControl3, seBarCodeNo},
        //        new Control[]{labelControl5, sePrefixStartNo, sePrefixNo, txtPrefixData},
        //        new Control[]{labelControl9 , txtProductNoType},
        //        new Control[]{labelControl17, seInjVolume},
        //        new Control[]{labelControl16, seInjSecondVolume},
        //        new Control[]{labelControl49, seBarCodeNo2},
        //        new Control[]{labelControl48 , sePrefixNo2, txtPrefixData2},
        //        new Control[]{labelControl51,  seBarCodeNo3},
        //        new Control[]{labelControl50 , sePrefixNo3, txtPrefixData3},

        //         new Control[]{labelControl4,seMinWeight,},
        //         new Control[]{labelControl2,seMaxWeight             },

        //         new Control[]{labelControl18,seAfterWeight_LSL       },
        //        new Control[]{labelControl19,seAfterWeight_USL          },

        //          new Control[]{labelControl10, spinEdit1             },
        //         new Control[]{labelControl11,spinEdit2,},

        //         new Control[]{labelControl36, seInjectRetention_USL  },
        //         new Control[]{labelControl37,seInjectRetention_LSL  },

        //        new Control[]{labelControl27, sePreWeld_LSL          },
        //         new Control[]{labelControl26, sePreWeld_USL         },



        //        new Control[]{labelControl22, seRepirWeld_LSL        },
        //        new Control[]{labelControl20, seRepirWeld_USL       },

        //        new Control[]{labelControl25, seRepirWeld2_LSL       },
        //        new Control[]{labelControl24,seRepirWeld2_USL       },

        //        new Control[]{labelControl13, seThickness_LSL        },
        //        new Control[]{labelControl12, seThickness_USL       },

        //        new Control[]{labelControl39, seThickness3_LSL       },
        //        new Control[]{labelControl38,seThickness3_USL       },

        //           new Control[]{ labelControl53, seThickness5_LSL       },
        //        new Control[]{ labelControl52, seThickness5_USL       },


        //        new Control[]{labelControl7, seMinScope              },
        //        new Control[]{labelControl1,seMaxScope                  },

        //        new Control[]{labelControl15,seThicknessPressure_LSL },
        //        new Control[]{labelControl14,seThicknessPressure_USL    },
        //        new Control[]{labelControl31, sePreWeldCathode_LSL   },
        //        new Control[]{labelControl30,sePreWeldCathode_USL       },

        //        new Control[]{labelControl23,seAgainWeld_LSL        },
        //        new Control[]{labelControl21,seAgainWeld_USL        },
        //        new Control[]{labelControl29,seAgainWeldCathode_LSL  },
        //        new Control[]{labelControl28,seAgainWeldCathode_USL     },

        //          new Control[]{labelControl40, seRIlsl },
        //        new Control[]{labelControl41, seRIusl },


        //        new Control[]{labelControl33, seRepirWeldCathode_LSL },
        //        new Control[]{labelControl32,seRepirWeldCathode_USL     },
        //        new Control[]{labelControl35,seRepirWeld2Cathode_LSL },
        //        new Control[]{labelControl34,seRepirWeld2Cathode_USL    },
        //        new Control[]{labelControl43, seThickness2_LSL       },
        //        new Control[]{labelControl42,seThickness2_USL           },
        //        new Control[]{labelControl47,seThickness4_LSL        },
        //        new Control[]{labelControl46,seThickness4_USL           },
        //        new Control[]{labelControl44, seVlotlsl },
        //        new Control[]{labelControl45, seVlotusl },
        //    });
        //    //fp2.Location = new Point(fp1.Location.X + fp1.Width, 0);
        //    // fp2.Height = panel3.Controls.Count / 6 * 30;
        //    //fpBase.Controls.Add(fp2);

        //    //var fp3 = GetFlowLayoutPanel(new Control[][]
        //    //   {

        //    //   });
        //    //fpBase.Controls.Add(fp3);



        //    //var fp4 = GetFlowLayoutPanel(new Control[][]
        //    //  {

        //    //  });
        //    //fpBase.Controls.Add(fp4);

        //    //var fp5 = GetFlowLayoutPanel(new Control[][]
        //    //{

        //    //});
        //    //fpBase.Controls.Add(fp5);

        //    //int maxHeight = 0;
        //    //foreach (Control item in fpBase.Controls)
        //    //{
        //    //    if (item.Height > maxHeight)
        //    //    {
        //    //        maxHeight = item.Height;
        //    //    }
        //    //}
        //    //panel3.Height = maxHeight + 20;
        //    //fpBase.Width = panel3.Width;
        //    //fp2.AutoSize = true;

        //    var maxWidth = 0;
        //    var maxHeight = 0;

        //    foreach (Control item in fp2.Controls)
        //    {
        //        if (item.Width > maxWidth)
        //        {
        //            maxWidth = item.Width;
        //        }
        //        if (item.Height > maxHeight)
        //        {
        //            maxHeight = item.Height;
        //        }
        //    }
        //    foreach (Panel item in fp2.Controls)
        //    {
        //        item.Width = maxWidth;
        //        item.Height = maxHeight;
        //        //item.BorderStyle = BorderStyle.FixedSingle;

        //        //foreach (Control con in item.Controls)
        //        //{
        //        //    if (con is Label)
        //        //    {
        //        //        var lb = (con as Label);
        //        //        if (lb!=null)
        //        //        {
        //        //            lb.BorderStyle = BorderStyle.FixedSingle;
        //        //        }
        //        //    }
        //        //}

        //    }
        //    fp2.Width = this.Width;
        //    //fp2.BorderStyle = BorderStyle.FixedSingle;
        //    // fp2.FlowDirection = FlowDirection.TopDown;
        //    //先算能横着放多少个
        //    var xCount = fp2.Width % maxWidth == 0 ? fp2.Width / maxWidth : (fp2.Width / maxWidth) - 1;
        //    //再算竖着放多少个
        //    var yCount = fp2.Controls.Count % xCount == 0 ? fp2.Controls.Count / xCount : fp2.Controls.Count / xCount + 1;
        //    fp2.Height = yCount * maxHeight + 30;
        //    panel3.Height = fp2.Height;
        //    if (!string.IsNullOrEmpty(Common.BarcodeRemark))
        //    {
        //        panel1.Height = fp2.Height + 20;
        //    }
        //    else
        //    {
        //        panel1.Height = fp2.Height;
        //    }
        //    panel3.Visible = true;
        //    panel3.Controls.Add(fp2);
        //    //panel3.Controls.Add(fpBase);
        //}


        /// <summary>
        /// 向左填充字符串
        /// </summary>
        /// <param name="src"></param>
        /// <param name="total"></param>
        /// <param name="padStr"></param>
        /// <returns></returns>
        private string PadLeftString(string src, int total, string padStr = "  ")
        {
            if (src.Length >= total)
            {
                return src;
            }
            var count = total - src.Length;
            //var sb = new StringBuilder();
            var newStr = string.Empty;
            for (int i = 0; i < count; i++)
            {
                //sb.Append(padStr);
                newStr = newStr + padStr;
            }
            //sb.Append(src);
            //var newStr = sb.ToString();
            return newStr + src;

        }


        /// <summary>
        /// 组装FlowLayoutPanel
        /// </summary>
        /// <param name="fromItems"></param>
        /// <param name="width">-1自动</param>
        /// <param name="height">-1自动</param>
        /// <param name="x"></param>
        /// <returns></returns>
        private FlowLayoutPanel GetFlowLayoutPanel(Control[][] fromItems, int width = -1, int height = -1, int x = 0, int y = 0)
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Location = new Point(x, y);
            // flowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;


            var iFromItems = fromItems.Where(a => a.Any(b => b.Visible != false));

            //var fMaxLen = iFromItems.Max(a => a[0].Text.Trim().Length);
            var fMaxWidth = iFromItems.Max(a => a[0].Width);

            foreach (var item in iFromItems)
            {
                var lb = (item[0] as LabelControl);
                if (lb != null)
                {
                    var newLb = new Label();
                    newLb.Text = lb.Text;
                    newLb.Name = lb.Name;
                    newLb.TextAlign = ContentAlignment.MiddleRight;
                    item[0] = newLb;

                    //lb.AutoSizeMode = LabelAutoSizeMode.None;
                    //lb.Width = fMaxWidth;
                }

                //item[0].Text =item[0].Text.PadLeft(fMaxLen,' ' );
                var panel = GetPanel(item);
                flowLayoutPanel.Controls.Add(panel);
            }

            if (width == -1)
            {
                // width = iFromItems.Max(a => a.Sum(b => b.Width + b.Margin.Left + b.Margin.Right) + a.Length * 2);
                width = iFromItems.Max(a => a.Sum(b => b.Width) + a.Length * 2);
            }
            if (height == -1)
            {
                // height = iFromItems.Sum(a => a.Max(b => b.Height + b.Margin.Top + b.Margin.Bottom + 2)) + 30;
                height = iFromItems.Sum(a => a.Max(b => b.Height));
            }

            flowLayoutPanel.Size = new Size(width, height);


            return flowLayoutPanel;
        }

        /// <summary>
        /// 多个控件组装成Panel
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        private Panel GetPanel(params Control[] controls)
        {
            if (controls == null || controls.Length <= 0)
            {
                throw new ArgumentNullException("参数不能为空--frmProdictType.GetPanel(controls=null)");
            }

            controls = controls.Where(a => a.Visible == true).ToArray();

            Panel panel = new Panel();
            //panel.Width = controls.Sum(a => a.Width + a.Margin.Left + a.Margin.Right + 2);//设置panel宽为controls的宽之和
            //panel.Height = controls.Max(a => a.Height + a.Margin.Top + a.Margin.Bottom + 2);//设置panel高为controls中最高的控件高度
            panel.Width = controls.Sum(a => a.Width);//设置panel宽为controls的宽之和
            panel.Height = controls.Max(a => a.Height + 2);//设置panel高为controls中最高的控件高度

            var maxHeight = controls.Max(a => a.Height);

            //panel.BorderStyle = BorderStyle.FixedSingle;
            //自动排列控件
            for (int i = 0; i < controls.Length; i++)
            {
                var control = controls[i];
                if (i == 0)
                {
                    control.Location = new Point(0, 0);
                }
                else
                {
                    var next = controls[i - 1];
                    control.Location = new Point(next.Location.X + next.Width, 0);
                }
                control.Height = maxHeight;
                panel.Controls.Add(control);
            }
            return panel;
        }


        private void DataRefresh()
        {
            isAdd = false;
            SetEnabled(false);

            var list = ProductTypeBLL.GetList(string.Empty, ref errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                XtraMessageBox.Show(errMsg);
                return;
            }
            gridControl5.DataSource = list;
            string MiErr = string.Empty;
            List<ProductType> listFiles = new List<ProductType>();
            if (Common.IsProductType)
            {


                DirectoryInfo dir = new DirectoryInfo(Common.FQI_MI_Path);
                FileSystemInfo[] files = dir.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                                                                  //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                foreach (FileSystemInfo FSys in files)
                {
                    var arr = FSys.Name.Split('工');
                    if (arr.Length > 1)
                    {
                        ProductType ptype = new ProductType();
                        ptype.MI = arr[0];
                        listFiles.Add(ptype);
                    }
                }
                IEnumerable<ProductType> listType = from a in listFiles orderby a.MI select a;//linq排序
                cbo_MI.Properties.DataSource = listType;
                cbo_MI.Properties.DisplayMember = "MI";
                cbo_MI.Properties.ValueMember = "MI";
                cbo_MI.Properties.DropDownItemHeight = 20;
                cbo_MI.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
            }
            else
            {

                try
                {
                    //Common.XRAY_MI_Path = @"E:\MES2.0\MISpec.csv";//服务器路径
                    string fileName = Application.StartupPath + "\\MISpec.csv";//本地路径
                                                                               //防止文件打开被占用，先删除本地文件，再下载最新文件到本地，使用本地文件读取配置并加载
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    File.Copy(Common.XRAY_MI_Path, fileName, true);
                    errMsg = "";
                    var dt = new DataTable();
                    IList<ProductType> listMi = new List<ProductType>();
                    if (fileName.EndsWith("csv"))
                    {
                        dt = Esquel.ExcelHelper.ExcelHelper.OpenCSV(fileName, ref errMsg);
                    }
                    else
                    {
                        dt = Esquel.ExcelHelper.ExcelHelper.Import(fileName);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        string[] MiArr = row[0].ToString().Split(',');
                        if (!string.IsNullOrEmpty(MiArr[0]))
                        {
                            ProductType typeMi = new ProductType();
                            MiErr = MiArr[0];
                            typeMi.Define76 = int.Parse(MiArr[1]);//AC电池层数
                            typeMi.Define72 = double.Parse(MiArr[3]); //头部最小值
                            typeMi.Define73 = double.Parse(MiArr[4]);//头部最大值
                            typeMi.Define79 = int.Parse(MiArr[2]);//BD电池层数
                            typeMi.Define74 = double.Parse(MiArr[3]);//尾部最小值
                            typeMi.Define75 = double.Parse(MiArr[4]);//尾部最大值
                            typeMi.MI = MiArr[0];
                            typeMi.Define77 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width", "0"));//AC宽
                            typeMi.Define78 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height", "0"));//AC高
                            typeMi.Define80 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width_bd", "0"));//BD宽
                            typeMi.Define81 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height_bd", "0"));//BD高
                            listMi.Add(typeMi);
                        }
                    }
                    IEnumerable<ProductType> listType = from a in listMi orderby a.MI select a;//linq排序
                    cbo_MI.Properties.DataSource = listType;
                    cbo_MI.Properties.DisplayMember = "MI";
                    cbo_MI.Properties.ValueMember = "MI";
                    cbo_MI.Properties.DropDownItemHeight = 20;
                    cbo_MI.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                }
                catch (Exception ex)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(MiErr + "数据异常，请补充完整" + ex.ToString());
                }
                finally
                {

                }
            }




        }
        private void DataRefreshHistory()
        {
            isAdd = false;
            SetEnabled(false);
            //txtProductNo.ResetText();
            //sePrefixStartNo.ResetText();
            //seBarCodeNo.ResetText();
            //seBarCodeNo2.ResetText();
            //seBarCodeNo3.ResetText();
            //txtPrefixData.ResetText();
            //txtPrefixData2.ResetText();
            //txtPrefixData3.ResetText();
            //txtDefaultBarCode.ResetText();
            //sePrefixNo.ResetText();
            //sePrefixNo2.ResetText();
            //sePrefixNo3.ResetText();
            //seMinScope.ResetText();
            //seMaxScope.ResetText();
            //seMinWeight.ResetText();
            //seMaxWeight.ResetText();
            //seAfterWeight_LSL.ResetText();
            //seAfterWeight_USL.ResetText();
            //txtProductNoType.ResetText();

            var list = ProductTypeHistoryBLL.GetList(string.Empty, "Iden", 200, ref errMsg);
            gridControl5.DataSource = list;
        }
        private bool isAdd = false;

        private void bbiAddNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRefresh();
            SetEnabled(true);
            isAdd = true;
        }

        private void spinEdit3_EditValueChanged(object sender, EventArgs e)
        {
            //txtPrefixData.Properties.MaxLength = Convert.ToInt32(sePrefixNo.Value);
        }

        private void gridView5_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString().Trim();
                if (e.RowHandle < 1000)
                    gridView5.IndicatorWidth = 35;
                if (e.RowHandle >= 1000)
                    gridView5.IndicatorWidth = 40;
                if (e.RowHandle >= 10000)
                    gridView5.IndicatorWidth = 50;
                if (e.RowHandle >= 100000)

                    gridView5.IndicatorWidth = 60;
                else gridView5.IndicatorWidth = 35;
            }
        }

        private void bbiModify_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                int row = gridView5.FocusedRowHandle;
                if (row >= 0)
                {
                    isAdd = false;
                    SetEnabled(true);

                }
                else
                {
                    XtraMessageBox.Show("请选择需要修改的数据");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Common.IsProductType)
            {
                if (DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo != txtProdyctType.Text)
                {
                    var arrProductType = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo.Split('-');
                    if (arrProductType.Length > 2)
                    {
                        string productType = arrProductType[2];
                        if (productType != txtProdyctType.Text)
                        {
                            MessageBox.Show($"当前选择model型号:{txtProdyctType.Text}与MES下发model型号{DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo}不一致，不允许修改参数!请检查!");
                            return;
                        }
                    }
                }
                spinEdit73.Enabled = false;
                spinEdit74.Enabled = false;
                spinEdit75.Enabled = false;
                spinEdit76.Enabled = false;
                spinEdit79.Enabled = false;
              
            }

            Login login = new Login();
            login.ShowDialog();
            if (login.IsPermission)
            {
                if (string.IsNullOrEmpty(txtProdyctType.Text.Trim()))
                {
                    XtraMessageBox.Show("产品型号不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(txtBarcodeLenth.Text.Trim()))
                {
                    XtraMessageBox.Show("条码前缀验证不能为空");
                    return;
                }
               

                var model = GetModel();
                var modelHistory = GetModelHistory();

                try
                {

                    if (isAdd)
                    {
                        var isExist = ProductTypeBLL.IsExists(string.Format("Product_type='{0}'", model.Product_type), ref errMsg);
                        if (isExist)
                        {
                            XtraMessageBox.Show("产品型号已存在,请更换产品型号");
                            return;
                        }
                        ProductTypeBLL.Add(model, ref errMsg);
                    }
                    else
                        ProductTypeBLL.Update(model, ref errMsg);


                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinAllBatLength", model.Define1.GetValueOrDefault(0).ToString());//总长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxAllBatLength", model.Define2.GetValueOrDefault(0).ToString());//总长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatLength", model.Define3.GetValueOrDefault(0).ToString());//主体长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatLength", model.Define4.GetValueOrDefault(0).ToString());//主体长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatWidth", model.Define5.GetValueOrDefault(0).ToString());//主体宽度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatWidth", model.Define6.GetValueOrDefault(0).ToString());//主体宽度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugMargin", model.Define7.GetValueOrDefault(0).ToString());//左极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugMargin", model.Define8.GetValueOrDefault(0).ToString());//左极耳边距最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugMargin", model.Define9.GetValueOrDefault(0).ToString());//右极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugMargin", model.Define10.GetValueOrDefault(0).ToString());//右极耳边距最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft1WhiteGlue", model.Define11.GetValueOrDefault(0).ToString());//左1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft1WhiteGlue", model.Define12.GetValueOrDefault(0).ToString());//左1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft2WhiteGlue", model.Define13.GetValueOrDefault(0).ToString());//左2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft2WhiteGlue", model.Define14.GetValueOrDefault(0).ToString());//左2小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight1WhiteGlue", model.Define15.GetValueOrDefault(0).ToString());//右1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight1WhiteGlue", model.Define16.GetValueOrDefault(0).ToString());//右1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight2WhiteGlue", model.Define17.GetValueOrDefault(0).ToString());//右2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight2WhiteGlue", model.Define18.GetValueOrDefault(0).ToString());//右2小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugLength", model.Define19.GetValueOrDefault(0).ToString());//左极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugLength", model.Define20.GetValueOrDefault(0).ToString());//左极耳长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugLength", model.Define21.GetValueOrDefault(0).ToString());//右极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugLength", model.Define22.GetValueOrDefault(0).ToString());//右极耳长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugMargin", model.Define23.GetValueOrDefault(0).ToString());//中间极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugMargin", model.Define24.GetValueOrDefault(0).ToString());//中间极耳边距最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid1WhiteGlue", model.Define25.GetValueOrDefault(0).ToString());//中间1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid1WhiteGlue", model.Define26.GetValueOrDefault(0).ToString());//中间1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid2WhiteGlue", model.Define27.GetValueOrDefault(0).ToString());//中间2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid2WhiteGlue", model.Define28.GetValueOrDefault(0).ToString());//中间2小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugLength", model.Define29.GetValueOrDefault(0).ToString());//中间极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugLength", model.Define30.GetValueOrDefault(0).ToString());//中间极耳长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinThickness", model.Define31.GetValueOrDefault(0).ToString());//厚度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxThickness", model.Define32.GetValueOrDefault(0).ToString());//厚度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "calival_thickness", model.Define33.GetValueOrDefault(0).ToString());//厚度标定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ThicknessKValue", model.Define34.GetValueOrDefault(0).ToString());//厚度K值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ThicknessBValue", model.Define35.GetValueOrDefault(0).ToString());//厚度B值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue", model.Define36.GetValueOrDefault(0).ToString());//相关性K值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue2", model.Define37.GetValueOrDefault(0).ToString());//相关性K值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue3", model.Define38.GetValueOrDefault(0).ToString());//相关性K值3
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue4", model.Define39.GetValueOrDefault(0).ToString());//相关性K值4
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue", model.Define40.GetValueOrDefault(0).ToString());//相关性B值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue2", model.Define41.GetValueOrDefault(0).ToString());//相关性B值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue3", model.Define42.GetValueOrDefault(0).ToString());//相关性B值3
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue4", model.Define43.GetValueOrDefault(0).ToString());//相关性B值4
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRange", model.Define44.GetValueOrDefault(0).ToString());//各工位测厚极差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRangeNum", model.Define45.GetValueOrDefault(0).ToString());//极差检测个数
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingAverage", model.Define46.GetValueOrDefault(0).ToString());//工位均值报警
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingTolerance", model.Define47.GetValueOrDefault(0).ToString());//工位均值报警公差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinVoltage", model.Define48.GetValueOrDefault(0).ToString());//电压最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxVoltage", model.Define49.GetValueOrDefault(0).ToString());//电压最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinResistance", model.Define50.GetValueOrDefault(0).ToString());//内阻最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxResistance", model.Define51.GetValueOrDefault(0).ToString());//内阻最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinTemperature", model.Define52.GetValueOrDefault(0).ToString());//温度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxTemperature", model.Define53.GetValueOrDefault(0).ToString());//温度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "VoltageCoefficient", model.Define54.GetValueOrDefault(0).ToString());//电压补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceCoefficient", model.Define55.GetValueOrDefault(0).ToString());//内阻补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient", model.Define56.GetValueOrDefault(0).ToString());//工位1温度补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient2", model.Define57.GetValueOrDefault(0).ToString());//工位2温度补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "RangeOfTemperatrue", model.Define58.GetValueOrDefault(0).ToString());//电池温度和环境温度差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceFixedValue", model.Define59.GetValueOrDefault(0).ToString());//不测内阻时内阻设定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureFixedValue", model.Define60.GetValueOrDefault(0).ToString());//不测温度时温度设定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Source", model.Define61.GetValueOrDefault(0).ToString());//IV初始值(判断依据4)
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Range", model.Define62.GetValueOrDefault(0).ToString());//IV跳变值(判断一句5)
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxIV", model.Define63.GetValueOrDefault(0).ToString());//IV上限
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinIV", model.Define64.GetValueOrDefault(0).ToString());//IV下限
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData1", model.Define65.GetValueOrDefault(0).ToString());//IV异常值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData2", model.Define66.GetValueOrDefault(0).ToString());//IV异常值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvTestTime", model.Define67.GetValueOrDefault(0).ToString());//IV测试时间
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation1Channel", model.Define68.GetValueOrDefault(0).ToString());//工位1对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation2Channel", model.Define69.GetValueOrDefault(0).ToString());//工位2对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation3Channel", model.Define70.GetValueOrDefault(0).ToString());//工位3对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation4Channel", model.Define71.GetValueOrDefault(0).ToString());//工位4对应通道

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width", model.Define77.GetValueOrDefault(0).ToString());//AC宽
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height", model.Define78.GetValueOrDefault(0).ToString());//AC高
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width_bd", model.Define80.GetValueOrDefault(0).ToString());//BD宽
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height_bd", model.Define81.GetValueOrDefault(0).ToString());//BD高

                    ProductTypeHistoryBLL.Add(modelHistory, ref errMsg);

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        DataRefresh();
                        SetEnabled(false);
                        XtraMessageBox.Show("保存成功!");
                    }
                    else
                    {
                        XtraMessageBox.Show(errMsg);
                    }
                }
                catch (Exception ex)
                {

                    XtraMessageBox.Show(ex.ToString());

                }
            }
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRefresh();
            if (Common.IsProductType)
            {
                spinEdit73.Enabled = false;
                spinEdit74.Enabled = false;
                spinEdit75.Enabled = false;
                spinEdit76.Enabled = false;
                spinEdit79.Enabled = false;
            }
            else {
                spinEdit73.Enabled = true;
                spinEdit74.Enabled = true;
                spinEdit75.Enabled = true;
                spinEdit76.Enabled = true;
                spinEdit79.Enabled = true;
            }
        }

        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView5.FocusedRowHandle >= 0)
            {
                if (XtraMessageBox.Show("你确定要删除所选择的数据", "系统询问", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {
                    Login login = new Login();
                    login.ShowDialog();
                    if (login.IsPermission)
                    {
                        ProductType model = new ProductType();
                        model.Iden = Convert.ToInt32(gridView5.GetRowCellValue(gridView5.FocusedRowHandle, "Iden"));
                        ProductTypeBLL.Delete(model, ref errMsg);
                        DataRefresh();

                        XtraMessageBox.Show("已选择的数据删除成功");
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("请选择需要修改的数据");
            }
        }

        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRefresh();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void bbiSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRefreshHistory();
        }
        private void frmProductType_Load(object sender, EventArgs e)
        {
            if (Common.IsProductType)
            {
                spinEdit73.Enabled = false;
                spinEdit74.Enabled = false;
                spinEdit75.Enabled = false;
                spinEdit76.Enabled = false;
                spinEdit79.Enabled = false;
            }
            else {
                spinEdit73.Enabled = true;
                spinEdit74.Enabled = true;
                spinEdit75.Enabled = true;
                spinEdit76.Enabled = true;
                spinEdit79.Enabled = true;
            }
            //产品型号
            txtProdyctType.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ProductType", "0");//产品型号
            spinEdit1.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinAllBatLength", "0");//总长度最小值
            spinEdit2.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxAllBatLength", "0");//总长度最大值
            spinEdit3.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatLength", "0"); //主体长度最小值
            spinEdit4.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatLength", "0");//主体长度最大值
            spinEdit5.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatWidth", "0");//主体宽度最小值
            spinEdit6.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatWidth", "0");//主体宽度最大值
            spinEdit7.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugMargin", "0");//左极耳边距最小值
            spinEdit8.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugMargin", "0");//左极耳边距最大值
            spinEdit9.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugMargin", "0");//右极耳边距最小值
            spinEdit10.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugMargin", "0");//右极耳边距最大值
            spinEdit11.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft1WhiteGlue", "0");//左1小白胶最小值
            spinEdit12.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft1WhiteGlue", "0");//左1小白胶最大值
            spinEdit13.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft2WhiteGlue", "0");//左2小白胶最小值
            spinEdit14.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft2WhiteGlue", "0");//左2小白胶最大值
            spinEdit15.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight1WhiteGlue", "0");//右1小白胶最小值
            spinEdit16.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight1WhiteGlue", "0");//右1小白胶最大值
            spinEdit17.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight2WhiteGlue", "0");//右2小白胶最小值
            spinEdit18.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight2WhiteGlue", "0");//右2小白胶最大值
            spinEdit19.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugLength", "0");//左极耳长度最小值
            spinEdit20.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugLength", "0");//左极耳长度最大值
            spinEdit21.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugLength", "0");//右极耳长度最小值
            spinEdit22.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugLength", "0");//右极耳长度最大值
            spinEdit23.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugMargin", "0");//中间极耳边距最小值
            spinEdit24.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugMargin", "0");//中间极耳边距最大值
            spinEdit25.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid1WhiteGlue", "0");//中间1小白胶最小值
            spinEdit26.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid1WhiteGlue", "0");//中间1小白胶最大值
            spinEdit27.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid2WhiteGlue", "0");//中间2小白胶最小值
            spinEdit28.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid2WhiteGlue", "0");//中间2小白胶最大值
            spinEdit29.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugLength", "0");//中间极耳长度最小值
            spinEdit30.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugLength", "0");//中间极耳长度最大值
            spinEdit31.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "min_thickness", "0");//厚度最小值
            spinEdit32.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "max_thickness", "0");//厚度最大值
            spinEdit33.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "calival_thickness", "0");//厚度标定值
            spinEdit34.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "k_thickness", "0");//厚度K值1
            spinEdit35.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "b_thickness", "0");//厚度B值1
            spinEdit36.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue", "0");//相关性K值1
            spinEdit37.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue2", "0");//相关性K值2
            spinEdit38.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue3", "0");//相关性K值3
            spinEdit39.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue4", "0");//相关性K值4
            spinEdit40.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue", "0");//相关性B值1
            spinEdit41.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue2", "0");//相关性B值2
            spinEdit42.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue3", "0");//相关性B值3
            spinEdit43.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue4", "0");//相关性B值4
            spinEdit44.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRange", "0");//各工位测厚极差
            spinEdit45.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRangeNum", "0");//极差检测个数
            spinEdit46.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingAverage", "0");//工位均值报警
            spinEdit47.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingTolerance", "0");//工位均值报警公差
            spinEdit48.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinVoltage", "0");//电压最小值
            spinEdit49.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxVoltage", "0");//电压最大值
            spinEdit50.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinResistance", "0");//内阻最小值
            spinEdit51.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxResistance", "0");//内阻最大值
            spinEdit52.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinTemperature", "0");//温度最小值
            spinEdit53.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxTemperature", "0");//温度最大值
            spinEdit54.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "VoltageCoefficient", "0");//电压补偿
            spinEdit55.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceCoefficient", "0");//内阻补偿
            spinEdit56.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient", "0");//工位1温度补偿
            spinEdit57.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient2", "0");//工位2温度补偿
            spinEdit58.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "RangeOfTemperatrue", "0");//电池温度和环境温度差
            spinEdit59.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceFixedValue", "0");//不测内阻时内阻设定值
            spinEdit60.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureFixedValue", "0");//不测温度时温度设定值
            spinEdit61.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Source", "0");//IV初始值(判断依据4)
            spinEdit62.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Range", "0");//IV跳变值(判断一句5)
            spinEdit63.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxIV", "0");//IV上限
            spinEdit64.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinIV", "0");//IV下限
            spinEdit65.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData1", "0");//IV异常值1
            spinEdit66.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData2", "0");//IV异常值2
            spinEdit67.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvTestTime", "0");//IV测试时间
            spinEdit68.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation1Channel", "0");//工位1对应通道
            spinEdit69.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation2Channel", "0");//工位2对应通道
            spinEdit70.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation3Channel", "0");//工位3对应通道
            spinEdit71.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation4Channel", "0");//工位4对应通道
            spinEdit77.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width", "0");//AC宽
            spinEdit78.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height", "0");//AC高
            spinEdit80.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width_bd", "0");//BD宽
            spinEdit81.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height_bd", "0");//BD高
            DataRefresh();
        }

        private void gridView5_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            int row = gridView5.FocusedRowHandle;
            if (row >= 0)
            {
                txtProdyctType.Tag = gridView5.GetRowCellValue(row, "Iden");
                txtBarcodeLenth.Text = Convert.ToString(gridView5.GetRowCellValue(row, "BarcodeLenth"));
                txtProdyctType.Text = Convert.ToString(gridView5.GetRowCellValue(row, "Product_type"));
                spinEdit1.EditValue = gridView5.GetRowCellValue(row, "Define1");
                spinEdit2.EditValue = gridView5.GetRowCellValue(row, "Define2");
                spinEdit3.EditValue = gridView5.GetRowCellValue(row, "Define3");
                spinEdit4.EditValue = gridView5.GetRowCellValue(row, "Define4");
                spinEdit5.EditValue = gridView5.GetRowCellValue(row, "Define5");
                spinEdit6.EditValue = gridView5.GetRowCellValue(row, "Define6");
                spinEdit7.EditValue = gridView5.GetRowCellValue(row, "Define7");
                spinEdit8.EditValue = gridView5.GetRowCellValue(row, "Define8");
                spinEdit9.EditValue = gridView5.GetRowCellValue(row, "Define9");
                spinEdit10.EditValue = gridView5.GetRowCellValue(row, "Define10");
                spinEdit11.EditValue = gridView5.GetRowCellValue(row, "Define11");
                spinEdit12.EditValue = gridView5.GetRowCellValue(row, "Define12");
                spinEdit13.EditValue = gridView5.GetRowCellValue(row, "Define13");
                spinEdit14.EditValue = gridView5.GetRowCellValue(row, "Define14");
                spinEdit15.EditValue = gridView5.GetRowCellValue(row, "Define15");
                spinEdit16.EditValue = gridView5.GetRowCellValue(row, "Define16");
                spinEdit17.EditValue = gridView5.GetRowCellValue(row, "Define17");
                spinEdit18.EditValue = gridView5.GetRowCellValue(row, "Define18");
                spinEdit19.EditValue = gridView5.GetRowCellValue(row, "Define19");
                spinEdit20.EditValue = gridView5.GetRowCellValue(row, "Define20");
                spinEdit21.EditValue = gridView5.GetRowCellValue(row, "Define21");
                spinEdit22.EditValue = gridView5.GetRowCellValue(row, "Define22");
                spinEdit23.EditValue = gridView5.GetRowCellValue(row, "Define23");
                spinEdit24.EditValue = gridView5.GetRowCellValue(row, "Define24");
                spinEdit25.EditValue = gridView5.GetRowCellValue(row, "Define25");
                spinEdit26.EditValue = gridView5.GetRowCellValue(row, "Define26");
                spinEdit27.EditValue = gridView5.GetRowCellValue(row, "Define27");
                spinEdit28.EditValue = gridView5.GetRowCellValue(row, "Define28");
                spinEdit29.EditValue = gridView5.GetRowCellValue(row, "Define29");
                spinEdit30.EditValue = gridView5.GetRowCellValue(row, "Define30");
                spinEdit31.EditValue = gridView5.GetRowCellValue(row, "Define31");
                spinEdit32.EditValue = gridView5.GetRowCellValue(row, "Define32");
                spinEdit33.EditValue = gridView5.GetRowCellValue(row, "Define33");
                spinEdit34.EditValue = gridView5.GetRowCellValue(row, "Define34");
                spinEdit35.EditValue = gridView5.GetRowCellValue(row, "Define35");
                spinEdit36.EditValue = gridView5.GetRowCellValue(row, "Define36");
                spinEdit37.EditValue = gridView5.GetRowCellValue(row, "Define37");
                spinEdit38.EditValue = gridView5.GetRowCellValue(row, "Define38");
                spinEdit39.EditValue = gridView5.GetRowCellValue(row, "Define39");
                spinEdit40.EditValue = gridView5.GetRowCellValue(row, "Define40");
                spinEdit41.EditValue = gridView5.GetRowCellValue(row, "Define41");
                spinEdit42.EditValue = gridView5.GetRowCellValue(row, "Define42");
                spinEdit43.EditValue = gridView5.GetRowCellValue(row, "Define43");
                spinEdit44.EditValue = gridView5.GetRowCellValue(row, "Define44");
                spinEdit45.EditValue = gridView5.GetRowCellValue(row, "Define45");
                spinEdit46.EditValue = gridView5.GetRowCellValue(row, "Define46");
                spinEdit47.EditValue = gridView5.GetRowCellValue(row, "Define47");
                spinEdit48.EditValue = gridView5.GetRowCellValue(row, "Define48");
                spinEdit49.EditValue = gridView5.GetRowCellValue(row, "Define49");
                spinEdit50.EditValue = gridView5.GetRowCellValue(row, "Define50");
                spinEdit51.EditValue = gridView5.GetRowCellValue(row, "Define51");
                spinEdit52.EditValue = gridView5.GetRowCellValue(row, "Define52");
                spinEdit53.EditValue = gridView5.GetRowCellValue(row, "Define53");
                spinEdit54.EditValue = gridView5.GetRowCellValue(row, "Define54");
                spinEdit55.EditValue = gridView5.GetRowCellValue(row, "Define55");
                spinEdit56.EditValue = gridView5.GetRowCellValue(row, "Define56");
                spinEdit57.EditValue = gridView5.GetRowCellValue(row, "Define57");
                spinEdit58.EditValue = gridView5.GetRowCellValue(row, "Define58");
                spinEdit59.EditValue = gridView5.GetRowCellValue(row, "Define59");
                spinEdit60.EditValue = gridView5.GetRowCellValue(row, "Define60");
                spinEdit61.EditValue = gridView5.GetRowCellValue(row, "Define61");
                spinEdit62.EditValue = gridView5.GetRowCellValue(row, "Define62");
                spinEdit63.EditValue = gridView5.GetRowCellValue(row, "Define63");
                spinEdit64.EditValue = gridView5.GetRowCellValue(row, "Define64");
                spinEdit65.EditValue = gridView5.GetRowCellValue(row, "Define65");
                spinEdit66.EditValue = gridView5.GetRowCellValue(row, "Define66");
                spinEdit67.EditValue = gridView5.GetRowCellValue(row, "Define67");
                spinEdit68.EditValue = gridView5.GetRowCellValue(row, "Define68");
                spinEdit69.EditValue = gridView5.GetRowCellValue(row, "Define69");
                spinEdit70.EditValue = gridView5.GetRowCellValue(row, "Define70");
                spinEdit71.EditValue = gridView5.GetRowCellValue(row, "Define71");
                spinEdit72.EditValue = gridView5.GetRowCellValue(row, "Define72");
                spinEdit73.EditValue = gridView5.GetRowCellValue(row, "Define73");
                spinEdit74.EditValue = gridView5.GetRowCellValue(row, "Define74");
                spinEdit75.EditValue = gridView5.GetRowCellValue(row, "Define75");
                spinEdit76.EditValue = gridView5.GetRowCellValue(row, "Define76");
                spinEdit77.EditValue = gridView5.GetRowCellValue(row, "Define77");
                spinEdit78.EditValue = gridView5.GetRowCellValue(row, "Define78");
                spinEdit79.EditValue = gridView5.GetRowCellValue(row, "Define79");
                spinEdit80.EditValue = gridView5.GetRowCellValue(row, "Define80");
                spinEdit81.EditValue = gridView5.GetRowCellValue(row, "Define81");

            }

            else
            {
                XtraMessageBox.Show("请选择需要修改的数据");
            }
        }

        private void btiExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = $"Excel文件(*.xlsx)|*.xlsx";
                sfd.FileName = this.Text;
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    gridControl5.ExportToXlsx(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.ToString());
            }
        }


        private void cbo_MI_EditValueChanged(object sender, EventArgs e)
        {

            var model = cbo_MI.GetSelectedDataRow() as ProductType;
            spinEdit72.Text = model.Define72.GetValueOrDefault(0).ToString();
            spinEdit73.Text = model.Define73.GetValueOrDefault(0).ToString();
            spinEdit74.Text = model.Define72.GetValueOrDefault(0).ToString();
            spinEdit75.Text = model.Define73.GetValueOrDefault(0).ToString();
            spinEdit76.Text = model.Define76.GetValueOrDefault(0).ToString();
            spinEdit77.Text = model.Define77.GetValueOrDefault(0).ToString();
            spinEdit78.Text = model.Define78.GetValueOrDefault(0).ToString();
            spinEdit79.Text = model.Define79.GetValueOrDefault(0).ToString();
            spinEdit80.Text = model.Define80.GetValueOrDefault(0).ToString();
            spinEdit81.Text = model.Define81.GetValueOrDefault(0).ToString();





            #region 拆分MES下发XRAY参数

            List<ParametricDataArray> lst = new List<ParametricDataArray>();
            JObject jobject = JObject.Parse(Convert.ToString(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].A007Jason));
            var jData = jobject["RequestInfo"];
            var ret = jData["ParameterInfo"];
            var jsonData = JsonConvert.SerializeObject(ret);
            JArray userArry = (JArray)JsonConvert.DeserializeObject(jsonData);
            List<JsonObject> listJson = new List<JsonObject>();
            //获取树形用户json字符串
            ProductTypeDAL typeDAL = new ProductTypeDAL();
            foreach (var item in userArry)
            {
                JObject j = JObject.Parse(item.ToString());
                JsonObject Jsonmodel = new JsonObject();
                Jsonmodel.ParamID = j["ParamID"].ToString();
                Jsonmodel.StandardValue = j["StandardValue"].ToString();
                Jsonmodel.UpperLimitValue = j["UpperLimitValue"].ToString();
                Jsonmodel.LowerLimitValue = j["LowerLimitValue"].ToString();
                Jsonmodel.Description = j["Description"].ToString();
                listJson.Add(Jsonmodel);
            }
            var arrProductType = CheckParamsConfig.Instance.CurrentModel.Trim().Split('-');
            string productType = CheckParamsConfig.Instance.CurrentModel.Trim();
            string errmsg = string.Empty;
            var listData = ProductTypeBLL.GetList($"Product_type='{ productType}'", "Iden", 200, ref errmsg);
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
                productType = CheckParamsConfig.Instance.CurrentModel.Trim();
                p = listData[0];
            }
            double Define79 = 0;
            double Define76 = 0;
            errmsg = string.Empty;
            if (!string.IsNullOrEmpty(productType))
            {
                p.Product_type = productType;
                for (int i = 0; i < listJson.Count; i++)
                {
                    switch (listJson[i].ParamID)
                    {
                        case "251":
                            p.Define72 = double.Parse(listJson[i].LowerLimitValue.ToString());
                            p.Define73 = double.Parse(listJson[i].UpperLimitValue);
                            p.Define74 = double.Parse(listJson[i].LowerLimitValue.ToString());
                            p.Define75 = double.Parse(listJson[i].UpperLimitValue.ToString());
                            break;
                        case "52196":

                            double.TryParse(listJson[i].StandardValue, out Define76);
                            p.Define76 = int.Parse(Define76.ToString());
                            break;
                        case "52197":

                            double.TryParse(listJson[i].StandardValue, out Define79);
                            p.Define79 = int.Parse(Define79.ToString());
                            break;
                        default:
                            break;
                    }
                }
                spinEdit72.Text = p.Define72.ToString();
                spinEdit73.Text = p.Define73.ToString();
                spinEdit74.Text = p.Define74.ToString();
                spinEdit75.Text = p.Define75.ToString();
                spinEdit76.Text = Define76.ToString();
                spinEdit79.Text = Define79.ToString();
            }
            #endregion

            string Define1 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "厚度", string.Empty);//电池厚度
            if (string.IsNullOrEmpty(Define1))
            {
                MessageBox.Show(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini,配置文件不符合字段规则,请检查");
                return;
            }
            string Define2 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "宽", string.Empty);//电池主体宽
            string Define3 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "肩宽", string.Empty);
            string Define4 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "长", string.Empty);//主体长
            string Define5 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Al_tab边距_min", string.Empty);//右极耳边距
            string Define6 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Al_tab边距_max", string.Empty);//右极耳边距
            string Define7 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Ni_tab边距_min", string.Empty);//左极耳边距
            string Define8 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Ni_tab边距_max", string.Empty);//左极耳边距
            string Define9 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Al_tab长度", string.Empty);//右极耳长度
            string Define10 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Ni_tab长度", string.Empty);//左极耳长度
            string Define11 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "tab间距", string.Empty);//中间极耳间距
            string Define12 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Al_tab小白胶", string.Empty);//右1小白胶
            string Define13 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Ni_tab小白胶", string.Empty);//左1小白胶
            string Define14 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Al_tab小白胶_max", string.Empty);//右2小白胶
            string Define15 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Al_tab小白胶_min", string.Empty);//右2小白胶
            string Define16 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Ni_tab小白胶_max", string.Empty);//左2小白胶
            string Define17 = FilesManager.GetPrivateProfileString(Common.FQI_MI_Path + model.MI + "工艺参数.ini.ini", "Config", "Ni_tab小白胶_min", string.Empty);//左2小白胶

            string[] Define1_Split = Define1.Split('|');
            string[] Define2_Split = Define2.Split('|');
            string[] Define3_Split = Define3.Split('|');
            string[] Define4_Split = Define4.Split('|');
            string[] Define5_Split = Define5.Split('|');
            string[] Define6_Split = Define7.Split('|');
            string[] Define7_Split = Define7.Split('|');
            string[] Define8_Split = Define8.Split('|');
            string[] Define9_Split = Define9.Split('|');
            string[] Define10_Split = Define10.Split('|');
            string[] Define11_Split = Define11.Split('|');
            string[] Define12_Split = Define12.Split('|');
            string[] Define13_Split = Define13.Split('|');
            string[] Define14_Split = Define14.Split('|');
            string[] Define15_Split = Define15.Split('|');
            string[] Define16_Split = Define16.Split('|');
            string[] Define17_Split = Define17.Split('|');
            if (Define1_Split.Length > 1)
            {
                spinEdit31.Text = Define1_Split[0];
                spinEdit32.Text = Define1_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "电池厚度参数不完整，请检查!");
            }

            if (Define2_Split.Length > 1)
            {
                spinEdit5.Text = Define2_Split[0];
                spinEdit6.Text = Define2_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "电池主体宽参数不完整，请检查!");
            }
            if (Define4_Split.Length > 1)
            {
                spinEdit3.Text = Define4_Split[0];
                spinEdit4.Text = Define4_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "电池主体长参数不完整，请检查!");
            }

            if (Define5_Split.Length > 1)
            {
                spinEdit9.Text = Define5_Split[0];
                spinEdit10.Text = Define5_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "右极耳边距参数不完整，请检查!");
            }

            if (Define7_Split.Length > 1)
            {
                spinEdit7.Text = Define7_Split[0];
                spinEdit8.Text = Define7_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "左极耳边距参数不完整，请检查!");
            }
            if (Define9_Split.Length > 1)
            {
                spinEdit21.Text = Define9_Split[0];
                spinEdit22.Text = Define9_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "右极耳长度参数不完整，请检查!");
            }
            if (Define10_Split.Length > 1)
            {
                spinEdit19.Text = Define10_Split[0];
                spinEdit20.Text = Define10_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "左极耳长度参数不完整，请检查!");
            }

            if (Define11_Split.Length > 1)
            {
                spinEdit23.Text = Define11_Split[0];
                spinEdit24.Text = Define11_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "中间极耳边距参数不完整，请检查!");
            }

            if (Define12_Split.Length > 1)
            {
                spinEdit15.Text = Define12_Split[0];
                spinEdit16.Text = Define12_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "右1小白胶参数不完整，请检查!");
            }
            if (Define13_Split.Length > 1)
            {
                spinEdit11.Text = Define13_Split[0];
                spinEdit12.Text = Define13_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "左1小白胶参数不完整，请检查!");
            }

            if (Define14_Split.Length > 1)
            {
                spinEdit17.Text = Define14_Split[0];
                spinEdit18.Text = Define14_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "右2小白胶参数不完整，请检查!");
            }

            if (Define15_Split.Length > 1)
            {
                spinEdit13.Text = Define15_Split[0];
                spinEdit14.Text = Define15_Split[1];
            }
            else
            {
                MessageBox.Show(Common.FQI_MI_Path + "," + model.MI + "左2小白胶参数不完整，请检查!");
            }

        }
    }
}
