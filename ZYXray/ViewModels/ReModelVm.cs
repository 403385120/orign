using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using XRayClient.Core;
using XRayClient.Core.Options;
using ZY.Logging;

namespace ZYXray.ViewModels
{
    public class ReModelVm : ObservableObject
    {
        private static ReModelVm _instance = new ReModelVm();
        public static ReModelVm Instance
        {
            get { return _instance; }
        }

        public ReModelRead MyReModelRead
        {
            get { return ReModelRead.Instance; }
        }

        public CheckParamsConfig MyCheckParamConfig
        {
            get { return CheckParamsConfig.Instance; }
        }

        private string _name;
        ObservableCollection<string> _modelList = new ObservableCollection<string>();

        public string Name
        {
            get { return _name; }

            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ObservableCollection<string> ModelList
        {
            get { return _modelList; }
        }

        /// <summary>
        /// 选择换型路径
        /// </summary>
        public ICommand SelectedReModelPath
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        dialog.Description = "请选择换型目录";
                        dialog.ShowNewFolderButton = false;
                        dialog.SelectedPath = MyCheckParamConfig.ReModelDir;

                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                        
                        if (result == DialogResult.OK)
                        {
                            this.MyCheckParamConfig.ReModelDir = dialog.SelectedPath;
                        }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        /// <summary>
        /// 刷新文件列表
        /// </summary>
        public ICommand UploadList
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(MyCheckParamConfig.ReModelDir);
                    if (!dirInfo.Exists)
                    {
                        MessageBox.Show("系统找不到指定文件夹!");
                        return;
                    }
                    FileInfo[] files = dirInfo.GetFiles();
                    ModelList.Clear();
                    foreach (FileInfo info in files)
                    {
                        ModelList.Add(Path.GetFileNameWithoutExtension(info.FullName));
                    }
                    if (ModelList.Count > 0)
                    {
                        Name = ModelList[0];
                    }
                    MyCheckParamConfig.WriteGeneralParams();
                }), delegate
                {
                    return true;
                });
            }
        }
        

        /// <summary>
        /// 调用
        /// </summary>
        public ICommand Transfer
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    List<string> filenames = MyReModelRead.GetFilesName(CheckParamsConfig.Instance.ReModelDir);
                    if (!string.IsNullOrWhiteSpace(Name))
                    {
                        foreach (string filename in filenames)
                        {
                            if (filename.Contains(Name))
                            {
                                MyReModelRead.ReadConfigParams(filename);
                            }
                        }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        /// <summary>
        /// FQI一键导入
        /// </summary>
        public ICommand UpdateFOIParams
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    CheckParamsConfig.Instance.MinBatLength = MyReModelRead.MinBatLength;
                    CheckParamsConfig.Instance.MaxBatLength = MyReModelRead.MaxBatLength;
                    CheckParamsConfig.Instance.MinBatWidth = MyReModelRead.MinBatWidth;
                    CheckParamsConfig.Instance.MaxBatWidth = MyReModelRead.MaxBatWidth;
                    CheckParamsConfig.Instance.MinThickness = MyReModelRead.MinBatThickness;
                    CheckParamsConfig.Instance.MaxThickness = MyReModelRead.MaxBatThickness;
                    CheckParamsConfig.Instance.MinLeftLugMargin = MyReModelRead.MinLeftLugMargin;
                    CheckParamsConfig.Instance.MaxLeftLugMargin = MyReModelRead.MaxLeftLugMargin;
                    CheckParamsConfig.Instance.MinRightLugMargin = MyReModelRead.MinRightLugMargin;
                    CheckParamsConfig.Instance.MaxRightLugMargin = MyReModelRead.MaxRightLugMargin;
                    CheckParamsConfig.Instance.MinLeftLugLength = MyReModelRead.MinLeftLugLength;
                    CheckParamsConfig.Instance.MaxLeftLugLength = MyReModelRead.MaxLeftLugLength;
                    CheckParamsConfig.Instance.MinRightLugLength = MyReModelRead.MinRightLugLength;
                    CheckParamsConfig.Instance.MaxRightLugLength = MyReModelRead.MaxRightLugLength;
                    CheckParamsConfig.Instance.MinLeft1WhiteGlue = MyReModelRead.MinLeft1WhiteGlue;
                    CheckParamsConfig.Instance.MaxLeft1WhiteGlue = MyReModelRead.MaxLeft1WhiteGlue;
                    CheckParamsConfig.Instance.MinLeft2WhiteGlue = MyReModelRead.MinLeft2WhiteGlue;
                    CheckParamsConfig.Instance.MaxLeft2WhiteGlue = MyReModelRead.MaxLeft2WhiteGlue;
                    CheckParamsConfig.Instance.MinRight1WhiteGlue = MyReModelRead.MinRight1WhiteGlue;
                    CheckParamsConfig.Instance.MaxRight1WhiteGlue = MyReModelRead.MaxRight1WhiteGlue;
                    CheckParamsConfig.Instance.MinRight2WhiteGlue = MyReModelRead.MinRight2WhiteGlue;
                    CheckParamsConfig.Instance.MaxRight2WhiteGlue = MyReModelRead.MaxRight2WhiteGlue;

                    CheckParamsConfig.Instance.Write();
                    System.Windows.Forms.MessageBox.Show("FQI数据导入成功");
                }), delegate
                {
                    return true;
                });
            }
        }

        private string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                // 取两个字符，每个字符都是右对齐。
                stringBuilder.AppendFormat("u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }
    }
}
