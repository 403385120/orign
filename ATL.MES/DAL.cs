using ATL.Common;
using ATL.Config.Validate;
using ATL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES
{
    public class DAL : BaseFacade
    {
        public static DAL me = new DAL();

        public void InsertMesInterfaceLog(string FunctionID, string GUID, string RequestTime, string ResponseTime, string Data, string errorMsg)
        {
            string sql;
            if (!string.IsNullOrEmpty(ResponseTime) && string.IsNullOrEmpty(RequestTime))
            {
                sql = $"insert into log_simple_mes_interface_execution (FunctionID, GUID, ResponseTime, Data, errorMsg) Values('{FunctionID}', '{GUID}', '{ResponseTime}', '{Data}', '{errorMsg}');";
            }
            else if (string.IsNullOrEmpty(ResponseTime) && !string.IsNullOrEmpty(RequestTime))
            {
                sql = $"insert into log_simple_mes_interface_execution (FunctionID, GUID, RequestTime, Data, errorMsg) Values('{FunctionID}', '{GUID}',  '{RequestTime}', '{Data}', '{errorMsg}');";
            }
            else
            {
                sql = $"insert into log_simple_mes_interface_execution (FunctionID, GUID, ResponseTime, RequestTime, Data, errorMsg) Values('{FunctionID}', '{GUID}', '{ResponseTime}', '{RequestTime}', '{Data}', '{errorMsg}');";
            }
            bool success = false;
            string exceptionMsg = string.Empty;
            try
            {
                BaseFacade baseFacade = new BaseFacade();
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                success = true;
            }
            catch (Exception _ex)
            {
                exceptionMsg = _ex.ToString();
            }
            if (!success)
            {
                LogHelper.Error($"执行错误：{sql}");
                LogHelper.Error(exceptionMsg);
            }
        }

        public void InsertToMESofflineBuffer(string EquipmentID, string FunctionID, string GUID, string Jason, List<string> lstBarcode = null)
        {
            string barcodes = lstBarcode != null ? string.Join(",", lstBarcode) : string.Empty;
            string sql = $"insert into offline_mes_interface_upload_buffer_datas (EquipmentID, FunctionID, Guid, Data, cellbarcode) Values('{EquipmentID}','{FunctionID}', '{GUID}', '{Jason}', '{barcodes}');";
            bool success = false;
            string exceptionMsg = string.Empty;
            try
            {
                BaseFacade baseFacade = new BaseFacade();
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                success = true;
            }
            catch (Exception _ex)
            {
                exceptionMsg = _ex.ToString();
            }
            if (!success)
            {
                LogHelper.Error($"执行错误：{sql}");
                LogHelper.Error(exceptionMsg);
            }
        }
        
        public void UpdateMesInterfaceLogErrorMsg(string FunctionID, string GUID, string errorMsg)
        {
            Task.Run(() =>
            {
                string sql = $"update log_simple_mes_interface_execution set errorMsg = '{errorMsg}' where FunctionID = '{FunctionID}' and GUID = '{GUID}';";
                bool success = false;
                string exceptionMsg = string.Empty;
                try
                {
                    BaseFacade baseFacade = new BaseFacade();
                    baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    success = true;
                }
                catch (Exception _ex)
                {
                    exceptionMsg = _ex.ToString();
                }
                if (!success)
                {
                    LogHelper.Error($"执行错误：{sql}");
                    LogHelper.Error(exceptionMsg);
                }
            });
        }
        
        public void UpdateAlarmRuleTable(List<DeviceAlertConfigInfo> lst)
        {
            List<DeviceAlertConfigInfo> forDeleteLst = new List<DeviceAlertConfigInfo>();
            forDeleteLst.AddRange(DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => !lst.Exists(y => y.EquipmentID == x.EquipmentID && y.UploadParamID == x.UploadParamID)));
            RemoveAlarmRules(forDeleteLst);  //删除未下发的报警
            string sql = "delete from device_alert_config where 1=1;";
            equipDB.ExecuteNonQuery(CommandType.Text, sql);
            
            List<DeviceAlertConfigInfo> lstAlarmRuleInfo = new List<DeviceAlertConfigInfo>();
            foreach (var v in lst)
            {
                try
                {
                    sql = $"insert into device_alert_config (EquipmentID, UploadParamID, ParamName, AlertLevel, AlertBitAddr) Values('{v.EquipmentID}', '{v.UploadParamID}', '{v.AlarmContent}', '{v.AlarmLevel}', '{v.AlarmBitAddr}');";
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);

                    

                    AlarmLevelInfo alarmLevelInfo = AlarmLevelInfo.lstAlarmLevelInfo.Where(x => x.LevelName == v.AlarmLevel).FirstOrDefault();
                    if(alarmLevelInfo == null)
                    {
                        //LogInDB.Error($"MES 下发的报警等级{v.AlarmLevel} 不在数据库表alarm_level里面");
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error($"MES 下发的报警等级{v.AlarmLevel} 不在数据库表alarm_level里面");
                        }
                        else
                        {
                            LogInDB.Error($"The alarm level {v.AlarmLevel} issued by MES is not in alarm_level table of database");
                        }
                        continue;
                    }

                    DeivceEquipmentidPlcInfo deivceEquipmentidPlcInfo = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.EquipmentID == v.EquipmentID).FirstOrDefault();
                    if (deivceEquipmentidPlcInfo == null)
                    {
                        //LogInDB.Error($"MES 下发的设备ID{v.EquipmentID} 不在数据库表deivce_equipmentid_plc里面");
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error($"MES 下发的设备ID{v.EquipmentID} 不在数据库表deivce_equipmentid_plc里面");
                        }
                        else
                        {
                            LogInDB.Error($"The EquipmentID {v.EquipmentID} issued by MES is not in deivce_equipmentid_plc table of database");
                        }
                        continue;
                    }

                    DeviceAlertConfigInfo rule = new DeviceAlertConfigInfo();
                    rule.EquipmentID = v.EquipmentID;
                    rule.UploadParamID = v.UploadParamID;
                    rule.AlarmContent = v.AlarmContent;
                    rule.AlarmLevel = alarmLevelInfo.LevelName;
                    rule.AlarmBitAddr = v.AlarmBitAddr;
                  
                    lstAlarmRuleInfo.Add(rule);
                    
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.ToString());
                }
            }

            PlcDAL.ThisObject.SaveAlarmRules(lstAlarmRuleInfo);


            if(ValidateHelper.Validate(DeviceAlertConfigInfo.lstDeviceAlertConfigInfos))
            {
                lock (DeviceAlertConfigInfo.alarmObj)
                {
                    foreach (var v in DeviceAlertConfigInfo.lstAlarming)
                    {
                        DeviceAlertConfigInfo a = DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => x.EquipmentID == v.EquipmentID && x.UploadParamID == v.UploadParamID).FirstOrDefault();
                        if (a != null)
                        {
                            a.isAlarming = true;
                            if (DeviceAlertConfigInfo.lstAlarming.Where(x => x.EquipmentID == a.EquipmentID && x.UploadParamID == a.UploadParamID).FirstOrDefault() == null)
                            {
                                DeviceAlertConfigInfo.lstAlarming.Add(a);
                            }
                        }
                    }
                }
            }
        }

        public void RemoveAlarmRules(List<DeviceAlertConfigInfo> lst)
        {
            foreach (var v in lst)
            {
                string sql = string.Format("delete from alarm_rule where EquipmentID = '{0}' and t.UploadParamID = '{1}';", v.EquipmentID, v.UploadParamID);
                try
                {
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.Message + Environment.NewLine + sql);
                }
            }
        }

        public void UpdateDeviceSpartConfigTable(List<DeviceSpartConfigInfo> lst)
        {
            List<DeviceSpartConfigInfo> forDeleteLst = new List<DeviceSpartConfigInfo>();
            forDeleteLst.AddRange(DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => !lst.Exists(y => y.EquipmentID == x.EquipmentID && y.UploadParamID == x.UploadParamID)));
            RemoveQuickWearParts(forDeleteLst);  //删除未下发的易损件地址信息
            PlcDAL.ThisObject.SaveQuickWearParts(lst);
        }

        public void RemoveQuickWearParts(List<DeviceSpartConfigInfo> lst)
        {
            foreach (var v in lst)
            {
                string sql = string.Empty;
                try
                {
                    sql = string.Format("delete from device_spart_config where id = {0}", v.ID);
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.Message + Environment.NewLine + sql);
                }
            }
        }

        public void UpdateDeviceInputOutputConfigTable(List<DeviceInputOutputConfigInfo> lst)
        {
            string sql = "delete from device_inputoutput_config where 1=1;";
            equipDB.ExecuteNonQuery(CommandType.Text, sql);
            
            foreach (var v in lst)
            {
                try
                {
                    sql = $"insert into device_inputoutput_config (EquipmentID, UploadParamID, ParamName, SendParamID, Type, SettingValueAddr, UpperLimitValueAddr, LowerLimitValueAddr, LimitControl, InputChangeMonitorAddr, ActualValueAddr, BycellOutputAddr, ParamValueRatio) Values('{v.EquipmentID}', '{v.UploadParamID}', '{v.ParamName}', '{v.SendParamID}', '{v.Type}',  '{v.SettingValueAddr}', '{v.UpperLimitValueAddr}', '{v.LowerLimitValueAddr}', '{v.LimitControl}', '{v.InputChangeMonitorAddr}', '{v.ActualValueAddr}', '{v.BycellOutputAddr}', {v.ParamValueRatio});";
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.ToString());
                }
            }
        }

        public void ClearJasonbufferDatabase()
        {
            string sql = string.Format("delete from offline_mes_interface_upload_buffer_datas where 1=1;");
            try
            {
                equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.Message + Environment.NewLine + sql);
            }
        }

        public void DeleteJasonbufferData(int id)
        {
            string sql = string.Format($"delete from offline_mes_interface_upload_buffer_datas where id = {id};");
            try
            {
                BaseFacade baseFacade = new BaseFacade();
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.Message + Environment.NewLine + sql);
            }
        }

        private DataTable getbackserver()
        {
            string sql = "select * from server_backpara order by 'ID'";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            return ds.Tables[0];
        }
        public void GetBackServerPara()
        {
            DataTable tb = getbackserver();
            List<backserver> lst = new List<backserver>();
            if (tb != null && tb.Rows.Count > 0)
            {
                foreach (DataRow row in tb.Rows)
                {
                    try
                    {
                        backserver info = new backserver();
                        info.ID = int.Parse(row["id"].ToString());
                        info.backupLocalTcpPortNo = (int)float.Parse(row["backupLocalTcpPortNo"].ToString().Trim());
                        info.backupLocalUdpRecvPortNo = (int)float.Parse(row["backupLocalUdpRecvPortNo"].ToString().Trim());
                        info.backupLocalUdpSendPortNo = (int)float.Parse(row["backupLocalUdpSendPortNo"].ToString().Trim());
                        info.backupServerIpAdr = row["backupServerIpAdr"].ToString().Trim();
                        info.backupServerUdpPortNo = (int)float.Parse(row["backupServerUdpPortNo"].ToString().Trim());
                        lst.Add(info);
                    }
                    catch(Exception ex)
                    {
                        LogInDB.Error(ex.ToString());
                    }
                }
            }
            InterfaceClient.BackServerPara = lst;
        }

        public void ClearMesBuffer()
        {
            string sql = string.Format($"truncate table offline_mes_interface_upload_buffer_datas;");
            try
            {
                BaseFacade baseFacade = new BaseFacade();
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.Message + Environment.NewLine + sql);
            }
        }

        private DataTable getOfflineJason()
        {
            string sql = "select * from offline_mes_interface_upload_buffer_datas";
            BaseFacade baseFacade = new BaseFacade();
            DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
            return ds.Tables[0];
        }

        public List<OfflineDataInfo> GetJasonbufferData()
        {
            DataTable tb = getOfflineJason();
            List<OfflineDataInfo> lst = new List<OfflineDataInfo>();
            if (tb != null && tb.Rows.Count > 0)
            {
                foreach (DataRow row in tb.Rows)
                {
                    OfflineDataInfo info = new OfflineDataInfo();
                    info.EquipmentID = row["EquipmentID"].ToString().Trim();
                    info.Data = row["Data"].ToString().Trim();
                    info.FunctionID = row["FunctionID"].ToString().Trim();
                    info.Guid = row["Guid"].ToString().Trim();
                    info.ID = int.Parse(row["id"].ToString().Trim());
                    info.LogDateTime = DateTime.Parse(row["logDateTime"].ToString().Trim());
                    info.Cellbarcode = row["cellbarcode"].ToString().Trim();
                    lst.Add(info);
                }
            }
            return lst;
        }
    }
}
