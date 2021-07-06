using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZY.Logging;

namespace XRayClient.BatteryCheckManager
{
    public class BatteryCheckIF
    {
        private static BatteryCheckManage _batteryCheckManage = new BatteryCheckManage();
        private static ProductionDataManage _productionDateManager = new ProductionDataManage();
        private static ProductionRateManage _productionRateManager = new ProductionRateManage();
        private static RecheckRecordManager _recheckRecordManager = new RecheckRecordManager();
        private static TestCodeManager _testCodeManager = new TestCodeManager();
        private static ProductionDataAnalyser _productionDataAnalyser = new ProductionDataAnalyser();
        private static WorkCalenderManage _workCalenderManager = new WorkCalenderManage();
        private static ProductionDataOffline _productionDataOffline = new ProductionDataOffline();
        private static ProductionDataRecheck _productionDataRecheck = new ProductionDataRecheck();

        private static Thread _updateThread = null;
        private static bool _updateRequired = false;
        private static bool _isAppExit = false;
        private static int _refreshInterval = 5000;
        private static ManualResetEvent _waitEvent = new ManualResetEvent(false);
        private static int _plotCnt = 0;

        public static BatteryCheckManage MyBatteryCheckManage
        {
            get { return _batteryCheckManage; }
        }

        public static ProductionDataManage MyProductionDataManage
        {
            get { return _productionDateManager; }
        }

        public static ProductionRateManage MyProductionRateManage
        {
            get { return _productionRateManager; }
        }

        public static RecheckRecordManager MyRecheckRecordManager
        {
            get { return _recheckRecordManager; }
        }

        public static TestCodeManager MyTestCodeManager
        {
            get { return _testCodeManager; }
        }

        public static ProductionDataAnalyser MyProductionDataAnalyser
        {
            get { return _productionDataAnalyser; }
        }

        public static WorkCalenderManage MyWorkCalenderManage
        {
            get { return _workCalenderManager; }
        }

        public static ProductionDataOffline MyProductionDataOffline
        {
            get { return _productionDataOffline; }
        }

        public static ProductionDataRecheck MyProductionDataRecheck
        {
            get { return _productionDataRecheck; }
        }

        public static bool Init()
        {
            return true;
        }

        public static bool UnInit()
        {
            _batteryCheckManage.UnInit();
            _productionDateManager.UnInit();
            _productionRateManager.UnInit();
            _recheckRecordManager.UnInit();
            _testCodeManager.UnInit();

            _productionDataAnalyser.UnInit();

            _isAppExit = true;
            _waitEvent.Set();
            if(null != _updateThread)
            {
                _updateThread.Join();
            }

            return true;
        }

        public static bool QuickFind(string barCode, ref BatteryCheck resultBat)
        {
            return _batteryCheckManage.QuickFind(barCode, ref resultBat);
        }

        public static bool ExportReport(string fileName)
        {
            return _batteryCheckManage.OutResult(fileName);
        }

        public static bool AddBatteryCheck(BatteryCheck batteryCheck)
        {
            bool result =  _batteryCheckManage.AddBatteryCheck(batteryCheck);

            _updateRequired = true;
            return result;
        }

        public static void RefreshRecheckList(bool isNotWaitCheck)
        {
            _recheckRecordManager.RefreshRecheckList(true, false, true, isNotWaitCheck);
        }

        public static void RefreshFQAList()
        {
            _recheckRecordManager.RefreshRecheckList(false, false);
        }

        public static void RefreshFQAUploadList()
        {
            _recheckRecordManager.RefreshRecheckList(false, true);
        }

        public static void RefreshFQAUploadNGList()
        {
            _recheckRecordManager.RefreshRecheckList(false, true, false);
        }

        public static void RefreshRecheckProductDataList()
        {
            _productionDataRecheck.RefreshProductionDataList();
        }

        public static void UpdateRecheckRecords()
        {
            _recheckRecordManager.ModifyRecheckList(true);
        }

        public static void UpdateFQARecords()
        {
            _recheckRecordManager.ModifyRecheckList(false);
        }
        
        public static void UpdateFQAOnUpload(RecheckRecord record)
        {
            _recheckRecordManager.ModifyOnUpload(record);
        }

        public static bool AddTestCode(TestCode code)
        {
            return _testCodeManager.AddTestCode(code);
        }

        public static bool RemoveTestCode(int recordID)
        {
            return _testCodeManager.RemoveTestCode(recordID);
        }

        public static bool Analysis(DateTime specifiedTime)
        {
            return _productionDataAnalyser.Analysis(specifiedTime);
        }

        public static bool RefreshWorkDaysList(DateTime year)
        {
            return _workCalenderManager.RefreshWorkDaysList(year);
        }

        //public static bool RefreshBatteryCheck()
        //{
        //    return _batteryCheckManage.RefreshBatteryCheckList();
        //}

        private static void UpdateRecord()
        {
            while (true)
            {
                if (_isAppExit) break;

                if (_updateRequired)
                {
                    _updateRequired = false;

                    try {
                        //_batteryCheckManage.RefreshBatteryCheckList();
                        _productionDateManager.RefreshProductionDataList();

                        //if(_plotCnt > 5)
                        //{
                        //    _plotCnt = 0;
                        //    _productionRateManager.GetHistoryRecord(10, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), DateTime.Now);
                        //}

                        //_plotCnt++;
                    }
                    catch (System.Exception ex)
                    {
                        LoggingIF.Log("Failed to update batterycheckRecord: " + ex.Message.ToString(), LogLevels.Error, "BatteryCheckManager");
                    }
                }

                _waitEvent.WaitOne(_refreshInterval);
            }
        }
    }
}
