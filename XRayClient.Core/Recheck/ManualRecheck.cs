using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms.VisualStyles;
using ZY.Logging;
using Shuyz.Framework.Mvvm;
using XRayClient.Core.Converters;
using XRayClient.BatteryCheckManager;
using ATL.Core;
using MessageBox = System.Windows.Forms.MessageBox;
using ATL.Station;

namespace XRayClient.Core
{
    /// <summary>
    /// 手动复检/FQA逻辑
    /// </summary>
    public class ManualRecheck : ObservableObject
    {
        private RecheckStatus _recheckStatus = new RecheckStatus();
        private List<RecheckRecord> _internalList = new List<RecheckRecord>();

        public List<RecheckRecord> WorkRecords
        {
            get { return this._internalList; }
            set
            {
                this._internalList = value;
                RaisePropertyChanged("WorkRecords");
            }
        }

        public RecheckStatus MyRecheckStatus
        {
            get { return this._recheckStatus; }
            set
            {
                this._recheckStatus = value;
                RaisePropertyChanged("MyRecheckStatus");
            }
        }

        public ImageSaveConfig MyImageSaveConfig
        {
            get { return CheckParamsConfig.Instance.MyImageSaveConfig; }
        }

        /// <summary>
        /// 模式准备同时适用于复检和FQA
        /// 刷新状态和更新数据列表
        /// </summary>
        /// <param name="isRecheckMode"></param>
        public void GetReady(bool isRecheckMode, bool isNotWaitCheck)
        {
            this.MyRecheckStatus = new RecheckStatus();
            this.MyRecheckStatus.IsRecheckMode = isRecheckMode;

            if (isRecheckMode)
            {
                // 复检
                BatteryCheckIF.RefreshRecheckList(isNotWaitCheck);
                this.WorkRecords = BatteryCheckIF.MyRecheckRecordManager.RecheckRecords;
            }
            else
            {
                // FQA
                BatteryCheckIF.RefreshFQAList();
                this.WorkRecords = BatteryCheckIF.MyRecheckRecordManager.FQARecords;
            }

            _recheckStatus.CurIndex = 0;
            _recheckStatus.SubIndex = 0;

            this.MyRecheckStatus.TotalNum = this.WorkRecords.Count();
            this.MyRecheckStatus.CheckedNum = 0;
            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1) return;
            this.MyRecheckStatus.CurBarCode = this.WorkRecords[_recheckStatus.CurIndex].BatteryBarCode;
            batModel = MyRecheckStatus.CurBarCode.Substring(0, 3);
        }
        string batModel = string.Empty;
        /// <summary>
        /// 切换到上一组并更新条码
        /// </summary>
        public void NavPrev()
        {
            this._recheckStatus.NavPrev();

            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1) return;
            this._recheckStatus.CurBarCode = this.WorkRecords[_recheckStatus.CurIndex].BatteryBarCode;
        }

        /// <summary>
        /// 切换到下一组并更新条码
        /// </summary>
        public void NavNext()
        {
            this._recheckStatus.NavNext();

            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1) return;
            this._recheckStatus.CurBarCode = this.WorkRecords[_recheckStatus.CurIndex].BatteryBarCode;
            if (batModel != _recheckStatus.CurBarCode.Substring(0, 3))
            {
                Tips tips = new Tips(_recheckStatus.CurBarCode.Substring(0, 3));
                tips.ShowDialog();
                //MessageBox.Show("电池型号改变！");
                batModel = _recheckStatus.CurBarCode.Substring(0, 3);
            }
        }

        /// <summary>
        /// 标记OK
        /// </summary>
        public void MarkOK()
        {
            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1) return;
            //获取当前保存路径的字符串长度
            int pathLength = MyImageSaveConfig.SaveTestPath.Length;
            string deletePath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "WaitCheck");

            if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
            {
                this.WorkRecords[_recheckStatus.CurIndex].BackResult = true;
                this.WorkRecords[_recheckStatus.CurIndex].FrontResult = true;

                //删除图片路径前面的保存的文件夹的字符串
                string deletePath_A1 = this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath;
                string deletePath_A2 = this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath;

                deletePath_A1.Remove(0, pathLength);
                deletePath_A2.Remove(0, pathLength);

                //给delethPath指定路径
                deletePath_A1 = deletePath + deletePath_A1;
                deletePath_A2 = deletePath + deletePath_A2;

                //删除该图片
                if (File.Exists(deletePath_A1))
                {
                    File.Delete(deletePath_A1);
                }
                if (File.Exists(deletePath_A2))
                {
                    File.Delete(deletePath_A2);
                }

            }
            else   //默认4角模式
            {
                if (this._recheckStatus.SubIndex == 0)
                {
                    this.WorkRecords[_recheckStatus.CurIndex].BackResult = true;
                    //删除图片路径前面的保存的文件夹的字符串
                    string deletePath_A3 = this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath;
                    string deletePath_A2 = this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath;
                    deletePath_A3.Remove(0, pathLength);
                    deletePath_A2.Remove(0, pathLength);
                    deletePath_A3 = deletePath + deletePath_A3;
                    deletePath_A2 = deletePath + deletePath_A2;
                    if (File.Exists(deletePath_A3))
                    {
                        File.Delete(deletePath_A3);
                    }
                    if (File.Exists(deletePath_A2))
                    {
                        File.Delete(deletePath_A2);
                    }
                }
                else
                {
                    this.WorkRecords[_recheckStatus.CurIndex].FrontResult = true;
                    //删除图片路径前面的保存的文件夹的字符串
                    string deletePath_A4 = this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath;
                    string deletePath_A1 = this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath;
                    deletePath_A4.Remove(0, pathLength);
                    deletePath_A1.Remove(0, pathLength);
                    deletePath_A4 = deletePath + deletePath_A4;
                    deletePath_A1 = deletePath + deletePath_A1;
                    if (File.Exists(deletePath_A4))
                    {
                        File.Delete(deletePath_A4);
                    }
                    if (File.Exists(deletePath_A1))
                    {
                        File.Delete(deletePath_A1);
                    }
                }

            }

            this._recheckStatus.CheckedNum = this.WorkRecords.Where(x => x.IsChecked).Count();

            _recheckStatus.CurIndex = _recheckStatus.CurIndex;
            _recheckStatus.SubIndex = _recheckStatus.SubIndex;

            //// 跳到下一组
            //this.NavNext();
        }

        /// <summary>
        /// 标记NG
        /// </summary>
        public void MarkNG()
        {
            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1) return;
            int pathLength = MyImageSaveConfig.SaveTestPath.Length;
            string deletePath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "WaitCheck");

            if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
            {
                this.WorkRecords[_recheckStatus.CurIndex].BackResult = false;
                this.WorkRecords[_recheckStatus.CurIndex].FrontResult = false;

                //删除图片路径前面的保存的文件夹的字符串
                string deletePath_A1 = this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath;
                string deletePath_A2 = this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath;
                deletePath_A1.Remove(0, pathLength);
                deletePath_A2.Remove(0, pathLength);

                //给delethPath指定路径
                deletePath_A1 = deletePath + deletePath_A1;
                deletePath_A2 = deletePath + deletePath_A2;

                //删除该图片
                if (File.Exists(deletePath_A1))
                {
                    File.Delete(deletePath_A1);
                }
                if (File.Exists(deletePath_A2))
                {
                    File.Delete(deletePath_A2);
                }

            }
            else   //默认4角模式
            {
                if (this._recheckStatus.SubIndex == 0)
                {
                    this.WorkRecords[_recheckStatus.CurIndex].BackResult = false;
                    string deletePath_A3 = this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath;
                    string deletePath_A2 = this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath;
                    deletePath_A3.Remove(0, pathLength);
                    deletePath_A2.Remove(0, pathLength);

                    //给delethPath指定路径
                    deletePath_A3 = deletePath + deletePath_A3;
                    deletePath_A2 = deletePath + deletePath_A2;

                    //删除该图片

                    if (File.Exists(deletePath_A3))
                    {
                        File.Delete(deletePath_A3);
                    }
                    if (File.Exists(deletePath_A2))
                    {
                        File.Delete(deletePath_A2);
                    }
                }
                else
                {
                    this.WorkRecords[_recheckStatus.CurIndex].FrontResult = false;
                    string deletePath_A4 = this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath;
                    string deletePath_A1 = this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath;
                    deletePath_A4.Remove(0, pathLength);
                    deletePath_A1.Remove(0, pathLength);

                    //给delethPath指定路径
                    deletePath_A4 = deletePath + deletePath_A4;
                    deletePath_A1 = deletePath + deletePath_A1;

                    //删除该图片
                    if (File.Exists(deletePath_A4))
                    {
                        File.Delete(deletePath_A4);
                    }
                    if (File.Exists(deletePath_A1))
                    {
                        File.Delete(deletePath_A1);
                    }
                }
            }

            this._recheckStatus.CheckedNum = this.WorkRecords.Where(x => x.IsChecked).Count();

            _recheckStatus.CurIndex = _recheckStatus.CurIndex;
            _recheckStatus.SubIndex = _recheckStatus.SubIndex;

            //  FQA Mark
            // 复制图片到指定目录
            if (!this.MyRecheckStatus.IsRecheckMode)
            {
                LoggingIF.Log("FQA Mark " + this.WorkRecords[_recheckStatus.CurIndex].BatteryBarCode, LogLevels.Info, "ReCheck");

                string markPath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "FQAMark");

                markPath = Path.Combine(markPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(markPath)) Directory.CreateDirectory(markPath);

                if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
                {
                    if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath) && File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath))
                    {
                        File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath, Path.Combine(markPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath)), true);
                        File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath, Path.Combine(markPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath)), true);
                    }
                }
                else  //默认4角模式
                {
                    if (this._recheckStatus.SubIndex == 0)
                    {
                        if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath) && File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath, Path.Combine(markPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath)), true);
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath, Path.Combine(markPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath)), true);
                        }
                    }
                    else
                    {
                        if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath) && File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath, Path.Combine(markPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath)), true);
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath, Path.Combine(markPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath)), true);
                        }
                    }
                }

            }

        }

        public void WaitCheck()
        {
            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1) return;

            // 生产复检
            // 复制图片到指定目录
            if (this.MyRecheckStatus.IsRecheckMode)
            {
                if (!this.WorkRecords[_recheckStatus.CurIndex].IsWaitCheck)
                {
                    _recheckStatus.WaitCheck();
                    this.WorkRecords[_recheckStatus.CurIndex].IsWaitCheck = true;
                }

                string WaitCheckPath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "WaitCheck");
                WaitCheckPath = Path.Combine(WaitCheckPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(WaitCheckPath)) Directory.CreateDirectory(WaitCheckPath);

                if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
                {
                    if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath) && File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath))
                    {
                        if (!File.Exists(Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath))))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath, Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath)), true);
                        }

                        if (!File.Exists(Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath))))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath, Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath)), true);
                        }
                    }
                }
                else  //默认4角模式
                {
                    if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath) && File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath))
                    {
                        if (!File.Exists(Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath))))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath, Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath)), true);
                        }

                        if (!File.Exists(Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath))))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath, Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath)), true);
                        }
                    }

                    if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath) && File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath))
                    {
                        if (!File.Exists(Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath))))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath, Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath)), true);
                        }
                        if (!File.Exists(Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath))))
                        {
                            File.Copy(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath, Path.Combine(WaitCheckPath, Path.GetFileName(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath)), true);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 获取当前的一组图片路径
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        public void GetImagePath(ref string img1, ref string img2)
        {
            if (_recheckStatus.CurIndex > _recheckStatus.TotalNum - 1)
            {
                img1 = string.Empty;
                img2 = string.Empty;

                return;
            }

            if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
            {
                if (this.WorkRecords[_recheckStatus.CurIndex].CheckMode == 0)
                {
                    img1 = this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath;
                    img2 = this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath;
                }
                else
                {
                    img1 = this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath;
                    img2 = this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath;
                }
            }

            else  //默认4角模式
            {
                //4角模式4张图都存在才能显示图片
                if (this._recheckStatus.SubIndex == 0)
                {
                    if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath) &&
                        File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath))
                    {
                        img1 = this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath;
                        img2 = this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath;
                    }
                }
                else
                {
                    if (File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A2_ResultImagePath) &&
                        File.Exists(this.WorkRecords[_recheckStatus.CurIndex].A3_ResultImagePath))
                    {
                        img1 = this.WorkRecords[_recheckStatus.CurIndex].A1_ResultImagePath;
                        img2 = this.WorkRecords[_recheckStatus.CurIndex].A4_ResultImagePath;
                    }
                }
            }
        }

        /// <summary>
        /// 提交复检结果：
        /// 1. 更新结果
        /// 2. 更新数据库
        /// 3. 拷贝图片到结果目录
        /// </summary>
        public void SubmitRecheckResult(ref int submittedCnt, ref int unsubmittedCnt, string userID)
        {
            LoggingIF.Log("PRD SubmitRecheckResult, User:  " + userID, LogLevels.Info, "Recheck");

            int totalCnt = this.WorkRecords.Count();

            // 将检测过的记录筛选出来更新到列表中
            // 后面将由数据库一次性更新
            LoggingIF.Log("Update checking results to list...", LogLevels.Info, "Recheck");
            List<RecheckRecord> newList = new List<RecheckRecord>();
            List<RecheckRecord> waitCheckList = new List<RecheckRecord>();
            foreach (var item in this.WorkRecords)
            {
                if (item.IsWaitCheck)
                {
                    item.RecheckTime = DateTime.Now;
                    item.RecheckUserID = userID;
                    item.RecheckState = (int)ERecheckState.WAITCHECK;
                    waitCheckList.Add(item);

                    item.FrontResult = false;
                    item.BackResult = false;
                    item.IsfrontChecked = false;
                    item.IsbackChecked = false;
                    item.IsChecked = false;
                    continue;
                }

                if (!item.IsChecked) continue;
                if (!item.IsfrontChecked || !item.IsbackChecked)      //add by fjy
                {
                    item.FrontResult = false;
                    item.BackResult = false;
                    item.IsfrontChecked = false;
                    item.IsbackChecked = false;
                    item.IsChecked = false;
                    continue;
                }

                item.RecheckTime = DateTime.Now;
                item.RecheckUserID = userID;

                if (item.FrontResult && item.BackResult)
                {
                    item.RecheckState = (int)ERecheckState.OK;
                }
                else
                {
                    item.RecheckState = (int)ERecheckState.NG;
                }

                newList.Add(item);
            }

            LoggingIF.Log("Update database for manual recheck results...", LogLevels.Info, "Recheck");
            BatteryCheckIF.MyRecheckRecordManager.RecheckRecords = newList;
            BatteryCheckIF.UpdateRecheckRecords();
            BatteryCheckIF.MyRecheckRecordManager.RecheckRecords = waitCheckList;
            BatteryCheckIF.UpdateRecheckRecords();

            string okPath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "OK");
            string ngPath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "NG");
            string ngDonePath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "Done", "NG");

            foreach (var item in newList)
            {
                LoggingIF.Log("Moving pictures of " + item.BatteryBarCode + " as " + item.RecheckState.ToString(), LogLevels.Info, "Recheck");

                string curPath = (item.RecheckState == (int)ERecheckState.OK) ? okPath : ngPath;
                curPath = Path.Combine(curPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(curPath)) Directory.CreateDirectory(curPath);

                string curDoneNGPath = ngDonePath;
                curDoneNGPath = Path.Combine(curDoneNGPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(curDoneNGPath)) Directory.CreateDirectory(curDoneNGPath);

                if (item.CheckMode == 0)
                {
                    if (File.Exists(item.A1_ResultImagePath) && File.Exists(item.A2_ResultImagePath))
                    {
                        File.Copy(item.A1_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                        File.Copy(item.A2_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A2_ResultImagePath)), true);

                        if (item.RecheckState == (int)ERecheckState.NG)
                        {
                            File.Copy(item.A1_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                            File.Copy(item.A2_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A2_ResultImagePath)), true);
                        }

                        LoggingIF.Log("File moved OK " + item.BatteryBarCode, LogLevels.Info, "ReCheck");
                    }
                    else
                    {
                        LoggingIF.Log("File moved Failed, file not exists " + item.BatteryBarCode, LogLevels.Info, "ReCheck");
                    }
                }
                else if (item.CheckMode == 1)
                {
                    if (File.Exists(item.A3_ResultImagePath) && File.Exists(item.A4_ResultImagePath))
                    {
                        File.Copy(item.A3_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                        File.Copy(item.A4_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A4_ResultImagePath)), true);

                        if (item.RecheckState == (int)ERecheckState.NG)
                        {
                            File.Copy(item.A3_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                            File.Copy(item.A4_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A4_ResultImagePath)), true);
                        }

                        LoggingIF.Log("File moved OK " + item.BatteryBarCode, LogLevels.Info, "ReCheck");
                    }
                    else
                    {
                        LoggingIF.Log("File moved Failed, file not exists " + item.BatteryBarCode, LogLevels.Info, "ReCheck");
                    }
                }
                else if (item.CheckMode == 2)
                {
                    if (File.Exists(item.A1_ResultImagePath) && File.Exists(item.A2_ResultImagePath) &&
                        File.Exists(item.A3_ResultImagePath) && File.Exists(item.A4_ResultImagePath))
                    {
                        File.Copy(item.A1_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                        File.Copy(item.A2_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A2_ResultImagePath)), true);
                        File.Copy(item.A3_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                        File.Copy(item.A4_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A4_ResultImagePath)), true);

                        if (item.RecheckState == (int)ERecheckState.NG)
                        {
                            File.Copy(item.A1_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                            File.Copy(item.A2_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A2_ResultImagePath)), true);
                            File.Copy(item.A3_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                            File.Copy(item.A4_ResultImagePath, Path.Combine(curDoneNGPath, Path.GetFileName(item.A4_ResultImagePath)), true);
                        }

                        LoggingIF.Log("File moved OK " + item.BatteryBarCode, LogLevels.Info, "ReCheck");
                    }
                    else
                    {
                        LoggingIF.Log("File moved Failed, file not exists " + item.BatteryBarCode, LogLevels.Info, "ReCheck");
                    }
                }
            }

            submittedCnt = newList.Count();
            unsubmittedCnt = totalCnt - submittedCnt;
        }

        /// <summary>
        /// 提交FQA结果
        /// 1. 更新全部结果
        /// 2. 更新数据库
        /// 3. 删除OK目录下的文件
        /// 4. 将文件导出到FQAAccept或FQAReject目录
        /// </summary>
        /// <param name="isOK">全部OK或全部NG</param>
        /// <returns></returns>
        public void SubmitFQAResult(string fqaUser)
        {
            LoggingIF.Log("FQA SubmitFQAResult, User:  " + fqaUser, LogLevels.Info, "Recheck");

            bool isOK = true;

            // 如果FQA Mark了任何一个电池，则全部NG掉
            // TODO:
            // 如果Mark了FrontResult，然后有Mark回OK，那么由于BackResult还是False仍会NG掉
            // 所以我们暂时禁用了MarkOK功能
            foreach (var item in this.WorkRecords)
            {
                if (item.IsChecked && (!item.FrontResult || !item.BackResult))
                {
                    LoggingIF.Log("FQA results is False because item marked " + item.BatteryBarCode, LogLevels.Warn, "Recheck");
                    isOK = false;
                    break;
                }
            }

            LoggingIF.Log("Update all FQA results to " + isOK.ToString(), LogLevels.Info, "Recheck");
            foreach (var item in this.WorkRecords)
            {
                item.RecheckState = isOK ? (int)ERecheckState.FQAOK : (int)ERecheckState.FQANG;

                item.FQATime = DateTime.Now;
                item.FQAUser = fqaUser;
            }

            BatteryCheckIF.MyRecheckRecordManager.FQARecords = this.WorkRecords;
            BatteryCheckIF.UpdateFQARecords();

            string okPath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "OK");
            string fqaAccept = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "FQAAccept");
            string doneokPath = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "Done", "OK");
            //string fqaReject = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "FQAReject");

            // 移除OK目录下的文件，移动至Accept或Reject目录
            if (Directory.Exists(okPath))
            {
                Directory.Delete(okPath, true);
            }

            if (!isOK)
            {
                LoggingIF.Log("FQA rejected, mark as FQA reject and remove all OK files.", LogLevels.Info, "Recheck");
                return;
            }

            foreach (var item in this.WorkRecords)
            {
                LoggingIF.Log("Moving pictures of " + item.BatteryBarCode, LogLevels.Info, "Recheck");

                string curPath = fqaAccept;
                curPath = Path.Combine(curPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(curPath)) Directory.CreateDirectory(curPath);

                string curDoneOKPath = doneokPath;
                curDoneOKPath = Path.Combine(curDoneOKPath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(curDoneOKPath)) Directory.CreateDirectory(curDoneOKPath);

                if (item.CheckMode == 0)
                {
                    if (File.Exists(item.A1_ResultImagePath) && File.Exists(item.A2_ResultImagePath))
                    {
                        File.Copy(item.A1_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                        File.Copy(item.A2_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A2_ResultImagePath)), true);

                        File.Copy(item.A1_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                        File.Copy(item.A2_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A2_ResultImagePath)), true);

                        LoggingIF.Log("File moved OK " + item.BatteryBarCode, LogLevels.Info, "Recheck");
                    }
                    else
                    {
                        LoggingIF.Log("File moved failed, one of the two files are not exists. " + item.BatteryBarCode, LogLevels.Info, "Recheck");
                    }
                }
                else if (item.CheckMode == 1)
                {
                    if (File.Exists(item.A3_ResultImagePath) && File.Exists(item.A4_ResultImagePath))
                    {
                        File.Copy(item.A3_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                        File.Copy(item.A4_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A4_ResultImagePath)), true);

                        File.Copy(item.A3_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                        File.Copy(item.A4_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A4_ResultImagePath)), true);

                        LoggingIF.Log("File moved OK " + item.BatteryBarCode, LogLevels.Info, "Recheck");
                    }
                    else
                    {
                        LoggingIF.Log("File moved failed, one of the two files are not exists. " + item.BatteryBarCode, LogLevels.Info, "Recheck");
                    }
                }
                else if (item.CheckMode == 2)
                {
                    if (File.Exists(item.A1_ResultImagePath) && File.Exists(item.A2_ResultImagePath) &&
                       File.Exists(item.A3_ResultImagePath) && File.Exists(item.A4_ResultImagePath))
                    {
                        File.Copy(item.A1_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                        File.Copy(item.A2_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A2_ResultImagePath)), true);
                        File.Copy(item.A3_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                        File.Copy(item.A4_ResultImagePath, Path.Combine(curPath, Path.GetFileName(item.A4_ResultImagePath)), true);

                        File.Copy(item.A1_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A1_ResultImagePath)), true);
                        File.Copy(item.A2_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A2_ResultImagePath)), true);
                        File.Copy(item.A3_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A3_ResultImagePath)), true);
                        File.Copy(item.A4_ResultImagePath, Path.Combine(curDoneOKPath, Path.GetFileName(item.A4_ResultImagePath)), true);

                        LoggingIF.Log("File moved OK " + item.BatteryBarCode, LogLevels.Info, "Recheck");
                    }
                    else
                    {
                        LoggingIF.Log("File moved failed, one of the four files are not exists. " + item.BatteryBarCode, LogLevels.Info, "Recheck");
                    }
                }
            }
        }

        /// <summary>
        /// 提交FQA结果到数据库
        /// 注意：调用前先刷新列表
        /// </summary>
        public void UploadFQAResult(string userName, string passWord, string staffID, ref int okCnt, ref int failCnt, ref string failMsg)
        {
            okCnt = 0;
            failCnt = 0;
            failMsg = string.Empty;
            this.UploadFQAResultDo(userName, passWord, staffID, ref okCnt, ref failCnt, BatteryCheckIF.MyRecheckRecordManager.FQANotUpload, true, ref failMsg);      //D:\XRayPic\Recheck\FQAAccept  
            this.UploadFQAResultDo(userName, passWord, staffID, ref okCnt, ref failCnt, BatteryCheckIF.MyRecheckRecordManager.FQANotUploadNG, false, ref failMsg);   //D:\XRayPic\Recheck\NG  
        }

        /// <summary>
        /// 同时处理FQANG和用户复检NG的结果
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="staffID"></param>
        /// <param name="okCnt">上传成功个数</param>
        /// <param name="failCnt">上传失败个数</param>
        /// <param name="uploadList">上传列表</param>
        /// <param name="isOK"></param>
        public void UploadFQAResultDo(string userName, string passWord, string staffID, ref int okCnt, ref int failCnt, List<RecheckRecord> uploadList, bool isOK, ref string failMsg)
        {
            //Bis bis = new Bis();
            // 上传之前删掉当前的目录
            string fqaUploadFail = Path.Combine(MyImageSaveConfig.SaveRecheckPath, DateTime.Now.ToString("yyyyMMdd"), "FQAUploadFail", isOK ? "OK" : "NG");

            LoggingIF.Log("Upload all FQA results to STF... " + isOK.ToString(), LogLevels.Info, "ManualRecheck");

            foreach (var item in uploadList)
            {
                LoggingIF.Log("Find data: " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");
                string jsondata = string.Empty;
                string keyValue = string.Empty;
                string fqauserid = string.Empty;
                string prduserid = string.Empty;

                foreach (ProductionDataXray pdata in BatteryCheckIF.MyProductionDataRecheck.ProductionDataList)
                {
                    if (pdata.ProductSN == item.BatteryBarCode)
                    {
                        jsondata = pdata.JsonData;
                        keyValue = pdata.KeyValue;
                        fqauserid = item.FQAUser;
                        prduserid = item.RecheckUserID;
                        break;
                    }
                }

                if (jsondata == string.Empty)
                {
                    LoggingIF.Log("Can not Find RecheckData from database! : " + item.BatteryBarCode, LogLevels.Error, "ManualRecheck");
                    //LoggingIF.Log("Make data " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");

                    //UploadManualUpData(item.BatteryBarCode, Path.GetFileName(item.ResultPath), isOK, "0.9");
                }
                else
                {
                    LoggingIF.Log("RecheckData found , then Send data to MES: " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");
                    LoggingIF.Log("Calling A013...", LogLevels.Info, "ManualRecheck");
                    DateTime startTime = DateTime.Now;
                    item.RecheckState = isOK ? (int)ERecheckState.FQAUPDOK : (int)ERecheckState.FQAUPDNG;
                    jsondata = jsondata.Replace("\"OperationMark\":null", "\"OperationMark\":\"XRAY\"");
                    string marking = DateTime.Now.ToString("yyyyMMddHHmmss");
                    if (!isOK)
                    {
                        //string strOut;
                        //float min = Math.Min(CheckParamsConfig.Instance.MinLengthTail, CheckParamsConfig.Instance.MinLengthHead);
                        //float max = Math.Max(CheckParamsConfig.Instance.MaxLengthTail, CheckParamsConfig.Instance.MaxLengthHead);
                        //int result = bis.BIS_TransfXRayDataNew(item.BatteryBarCode, fqauserid, UserDefineVariableInfo.DicVariables["AssetsNO"].ToString(), marking, "Op_Judge", "Op_Judge", "NG", "-7", "", "", min.ToString(), max.ToString(), "", out strOut);
                        //LoggingIF.Log("复判上传返回 " + result + ":" + strOut, LogLevels.Info, "ManualRecheck");

                        jsondata = jsondata.Replace("\"Pass\":\"OK\"", "\"Pass\":\"NG\"");
                        jsondata = jsondata.Replace("{\"ParamID\":\"251\",", "{\"ParamID\":\"1184\",");

                        string prd = "\"EmployeeNo\":\"PRD\"";
                        string prd1 = "\"EmployeeNo\":\"" + prduserid + "\"";
                        jsondata = jsondata.Replace(prd, prd1);
                    }
                    else
                    {
                        //string strOut;
                        //float min = Math.Min(CheckParamsConfig.Instance.MinLengthTail, CheckParamsConfig.Instance.MinLengthHead);
                        //float max = Math.Max(CheckParamsConfig.Instance.MaxLengthTail, CheckParamsConfig.Instance.MaxLengthHead);
                        //int result = bis.BIS_TransfXRayDataNew(item.BatteryBarCode, fqauserid, UserDefineVariableInfo.DicVariables["AssetsNO"].ToString(), marking, "Op_Judge", "Op_Judge", "OK", "", "", "", min.ToString(), max.ToString(), "", out strOut);
                        //LoggingIF.Log("复判上传返回 " + result + ":" + strOut, LogLevels.Info, "ManualRecheck");


                        string[] arr = keyValue.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        string max = arr.Length == 2 ? arr[1] : arr[0];
                        string ngdata = "{\"ParamID\":\"251\",\"ParamDesc\":null,\"ParamValue\":\"-1\",\"Result\":null";
                        string okdata = "{\"ParamID\":\"1184\",\"ParamDesc\":null,\"ParamValue\":\"" + max + "\",\"Result\":null";

                        jsondata = jsondata.Replace(ngdata, okdata);

                        string prd = "\"EmployeeNo\":\"PRD\"";
                        string prd1 = "\"EmployeeNo\":\"" + prduserid + "\"";
                        jsondata = jsondata.Replace(prd, prd1);

                        string fqa = "\"EmployeeNo1\":\"\"";
                        string fqa1 = "\"EmployeeNo1\":\"" + fqauserid + "\"";
                        jsondata = jsondata.Replace(fqa, fqa1);
                    }

                    ATL.MES.A014.Root _root = ATL.MES.InterfaceClient.Current.A013Offline(item.BatteryBarCode, jsondata);

                    if (_root == null)
                    {
                        LoggingIF.Log("上传MES失败，离线缓存", LogLevels.Info, "ManualRecheck");
                        ProductionDataXray productData = new ProductionDataXray();
                        productData.ProductSN = item.BatteryBarCode;
                        productData.JsonData = jsondata;
                        BatteryCheckIF.MyProductionDataOffline.AddProductionData(productData);
                    }
                    else
                    {
                        DateTime endTime = DateTime.Now;
                        BatteryCheckIF.UpdateFQAOnUpload(item);
                        LoggingIF.Log(string.Format("上传MES成功, 耗时 = {0} 毫秒", (endTime - startTime).TotalMilliseconds), LogLevels.Info, "ManualRecheck");
                        LoggingIF.Log("Upload picture to MES: " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");

                        if (File.Exists(item.ResultPath))
                        {
                            // 上传压缩图片到MES系统
                            item.MesImagePath = UploadPicToMes(item.BatteryBarCode, item.ResultPath, isOK);

                            LoggingIF.Log("File upload OK " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");
                        }
                        else
                        {
                            LoggingIF.Log("File upload failed, file not exists. " + item.BatteryBarCode + " " + item.ResultPath, LogLevels.Info, "ManualRecheck");
                        }
                        okCnt++;
                    }
                }

                //LoggingIF.Log("Upload picture to STF: " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");

                //if (File.Exists(item.ResultPath))
                //{
                //    // 上传压缩图片到MES系统                    
                //    string[] strArr = item.ResultPath.Split('\\');
                //    //string path = strArr[0];
                //    //for (int i = 1; i < strArr.Length-1; i++)
                //    //{
                //    //    path = path + "\\" + strArr[i];
                //    //}



                //    //string fileName = strArr[strArr.Length - 1];
                //    //string strOut;
                //    //bool result=bis.BISXRayPicUpload(item.BatteryBarCode, item.ResultPath, fileName, UserDefineVariableInfo.DicVariables["AssetsNO"].ToString(), "0", fqauserid, out strOut);
                //    //LoggingIF.Log("复判图片上传返回 " + result + ":" + strOut, LogLevels.Info, "ManualRecheck");



                //    //item.MesImagePath = UploadPicToMes(item.BatteryBarCode, item.ResultPath);

                //    LoggingIF.Log("File upload OK " + item.BatteryBarCode, LogLevels.Info, "ManualRecheck");
                //}
                //else
                //{
                //    LoggingIF.Log("File upload failed, file not exists. " + item.BatteryBarCode + " " + item.ResultPath, LogLevels.Info, "ManualRecheck");
                //}

                //item.RecheckState = isOK ? (int)ERecheckState.FQAUPDOK : (int)ERecheckState.FQAUPDNG;
                //BatteryCheckIF.UpdateFQAOnUpload(item);

                //okCnt++;

                // 持续失败20个
                if (failCnt >= 20 && okCnt == 0)
                {
                    return;
                }

            }

            string pathToDel = string.Empty;
            if (isOK)
            {
                pathToDel = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "FQAAccept");
            }
            else
            {
                pathToDel = Path.Combine(MyImageSaveConfig.SaveRecheckPath, "NG");
            }
            if (Directory.Exists(pathToDel))
            {
                Directory.Delete(pathToDel, true);
            }
        }

        public string UploadPicToMes(string barcode, string picfullName, bool isOKCell)
        {
            string mespath = string.Empty;

            if (barcode.Length == 12 || barcode.Length == 30)
            {
                DateTime startTime = DateTime.Now;
                LoggingIF.Log("开始上传图片", LogLevels.Info, "ManualRecheck");

                string picName = Path.GetFileName(picfullName);
                string mesSavePath = string.Empty;

                mesSavePath = UserDefineVariableInfo.DicVariables["mesImgSavePath"].ToString();

                mesSavePath = mesSavePath.Replace("//", "\\\\");
                mesSavePath = mesSavePath.Replace("/", "\\");


                if (mesSavePath.EndsWith("\\"))
                {
                    //
                }
                else
                {
                    mesSavePath += "\\";
                }

                mesSavePath += barcode.Length == 12 ? barcode.Substring(0, 3) : barcode.Substring(18, 3);
                mesSavePath += "\\";
                mesSavePath += Station.Current.EquipmentID;//UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();
                mesSavePath += "\\";
                mesSavePath += DateTime.Now.ToString("yyyyMMdd");
                mesSavePath += "\\";
                mesSavePath += barcode.Length == 12 ? barcode.Substring(3, 4) : barcode.Substring(21, 4);
                mesSavePath += "\\";
                mesSavePath += barcode;
                mesSavePath += "\\";

                if (!Directory.Exists(mesSavePath)) Directory.CreateDirectory(mesSavePath);
                bool ret = false;

                int flat = 30;
                try
                {
                    if (isOKCell == false)
                    {
                        flat = 60;
                    }
                    ret = PicCompress.PicCompress.GetPicThumbnail(picfullName, Path.Combine(mesSavePath, picName), flat);
                    mespath = Path.Combine(mesSavePath, picName);
                }
                catch (Exception ex)
                {
                    string msg = "sFile: " + picfullName + "dFile: " + Path.Combine(mesSavePath, picName) + "flag: " + flat;
                    LoggingIF.Log(msg, LogLevels.Info, "ManualRecheck");
                    LoggingIF.Log("压缩上传图片异常: " + ex.Message, LogLevels.Info, "ManualRecheck");
                }
                LoggingIF.Log("upload picture to " + Path.Combine(mesSavePath, picName), LogLevels.Info, "ManualRecheck");
                DateTime endTime = DateTime.Now;
                LoggingIF.Log(string.Format("上传图片完成, 耗时 = {0} 毫秒", (endTime - startTime).TotalMilliseconds), LogLevels.Info, "ManualRecheck");
            }

            return mespath;
        }
        public void UploadManualUpData(string Barcode, string PicturePath, bool isOK, string value,string operationMark)
        {
            List<string> OutputParamItems = new List<string>();

            string oneitem = "";

            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51856", PicturePath);
            OutputParamItems.Add(oneitem);

            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "1184", value);
            OutputParamItems.Add(oneitem);

            string Pass = isOK ? "OK" : "NG";
            string json = string.Empty;

            DateTime startTime = DateTime.Now;
            ATL.MES.A014_minicell.Root root1 = ATL.MES.InterfaceClient.Current.A013("Normal", Barcode, Pass, "1", OutputParamItems, operationMark, out json);
            if (root1 == null)
            {
                LoggingIF.Log("上传MES失败，离线缓存", LogLevels.Info, "RunScanBarcodeThread");
                ProductionDataXray productData = new ProductionDataXray();
                productData.ProductSN = Barcode;
                productData.JsonData = json;
                BatteryCheckIF.MyProductionDataOffline.AddProductionData(productData);
            }
            else
            {
                DateTime endTime = DateTime.Now;
                LoggingIF.Log(string.Format("上传MES成功, 耗时 = {0} 毫秒", (endTime - startTime).TotalMilliseconds), LogLevels.Info, "ManualRecheck");
            }
        }
        public string StringSpilt(string PicName)
        {
            int index = PicName.IndexOf('+') + 1;
            string last = PicName.Substring(index);
            string before = PicName.Substring(0, index - 1);
            index = before.LastIndexOf('_');
            before = before.Substring(0, index + 1);
            string newPicName = before + last;
            return newPicName;
        }
    }
}
