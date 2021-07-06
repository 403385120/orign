using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATL.Common;
using ATL.Core;
using HslCommunication.BasicFramework;
using System.Threading;
using ATL.PLC;
using System.Net.NetworkInformation;

namespace ATL.Engine
{
    public class ID_Device
    {
        #region 成员

        /// <summary>
        /// PLC通信状态修改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void OnConnectedChanged(object sender, ConnectChangedEventArgs arg)
        {

        }
        public PlcConfigurationInfo PlcConfig;
        private bool initReadWritingAreaSuccess = false;
        public static List<ID_Device> lstDevices = new List<ID_Device>();
        #endregion

        #region 周期读写

        /// <summary>
        /// PLC读写
        /// </summary>
        public void plcCommunication()
        {
            while (!Core.Core.SoftClosing)
            {
                Thread.Sleep(1);
                PlcConfig.plcCommunicating = true;
                if (!ATL.Core.Core.CheckedOK)
                {
                    continue;
                }
                try
                {
                    if (!initReadWritingAreaSuccess)
                    {
                        if (InitializationPlcRwAreaConfig())
                            initReadWritingAreaSuccess = true;
                    }
                    if (initReadWritingAreaSuccess)
                    {
                        flushArea();
                    }
                    else
                    {
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.ToString());
                    Thread.Sleep(3000);
                }
            }
            PlcConfig.plcCommunicating = false;
        }

        #region Tag通信

        /// <summary>
        /// 从PLC里读取数据到标签缓存中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="PlcRwAreaConfig"></param>
        /// <returns></returns>
        private bool ReadTagPlcRwAreaConfig(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            DeviceTag tag = DeviceTag.lstDeviceTags.Where(x => x.tagName == PlcRwAreaConfig.StartAddress && x.plcID == PlcRwAreaConfig.plcID).FirstOrDefault();
            if(PlcRwAreaConfig.Length > 1)
            {
                switch (tag.ValueType)
                {
                    case "Bit":
                    case "Bool":
                        {
                            return GetTagPlcRwAreaConfigValues<bool[]>(tag, PlcRwAreaConfig);
                        }
                    case "Double":
                        {
                            return GetTagPlcRwAreaConfigValues<double[]>(tag, PlcRwAreaConfig);
                        }
                    case "Float":
                        {
                            return GetTagPlcRwAreaConfigValues<float[]>(tag, PlcRwAreaConfig);
                        }
                    case "Int16":
                        {
                            return GetTagPlcRwAreaConfigValues<short[]>(tag, PlcRwAreaConfig);
                        }
                    case "Int32":
                        {
                            return GetTagPlcRwAreaConfigValues<int[]>(tag, PlcRwAreaConfig);
                        }
                    case "Int64":
                        {
                            return GetTagPlcRwAreaConfigValues<long[]>(tag, PlcRwAreaConfig);
                        }
                    case "String":
                        {
                            return GetTagPlcRwAreaConfigValues<string>(tag, PlcRwAreaConfig);
                        }
                    default:
                        {
                            LogInDB.Error($"[{tag.ValueType}] error!!");
                            return false;
                        }
                }
            }
            else
            {
                switch (tag.ValueType)
                {
                    case "Bit":
                    case "Bool":
                        {
                            return GetTagPlcRwAreaConfigValues<bool>(tag, PlcRwAreaConfig);
                        }
                    case "Double":
                        {
                            return GetTagPlcRwAreaConfigValues<double>(tag, PlcRwAreaConfig);
                        }
                    case "Float":
                        {
                            return GetTagPlcRwAreaConfigValues<float>(tag, PlcRwAreaConfig);
                        }
                    case "Int16":
                        {
                            return GetTagPlcRwAreaConfigValues<short>(tag, PlcRwAreaConfig);
                        }
                    case "Int32":
                        {
                            return GetTagPlcRwAreaConfigValues<int>(tag, PlcRwAreaConfig);
                        }
                    case "Int64":
                        {
                            return GetTagPlcRwAreaConfigValues<long>(tag, PlcRwAreaConfig);
                        }
                    case "String":
                        {
                            return GetTagPlcRwAreaConfigValues<string>(tag, PlcRwAreaConfig);
                        }
                    default:
                        {
                            LogInDB.Error($"[{tag.ValueType}] error!!");
                            return false;
                        }
                }
            }
        }

        /// <summary>
        /// 从PLC里读取数据到标签缓存中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="PlcRwAreaConfig"></param>
        /// <returns></returns>
        private bool GetTagPlcRwAreaConfigValues<T>(DeviceTag tag, PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            try
            {
                Type dataType = typeof(T);
                string msg = string.Empty;
                bool readFailed = false ;
                switch (dataType.ToString())
                {
                    case "System.Int16":
                    case "System.Int16[]":
                        {
                            PlcRwAreaConfig.Int16Values = (short[])PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length);
                            if (PlcRwAreaConfig.Int16Values == null) readFailed = true;
                            break;
                        }
                    case "System.Int32":
                    case "System.Int32[]":
                        {
                            PlcRwAreaConfig.Int32Values = (int[])PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length);
                            if (PlcRwAreaConfig.Int32Values == null) readFailed = true;
                            break;
                        }
                    case "System.Int64":
                    case "System.Int64[]":
                        {
                            PlcRwAreaConfig.Int64Values = (long[])PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length);
                            if (PlcRwAreaConfig.Int64Values == null) readFailed = true;
                            break;
                        }
                    case "System.Boolean":
                    case "System.Boolean[]":
                        {
                            PlcRwAreaConfig.BitValues = (bool[])PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length);
                            if (PlcRwAreaConfig.BitValues == null) readFailed = true;
                            break;
                        }
                    case "System.Single":
                    case "System.Single[]":
                        {
                            PlcRwAreaConfig.FloatValues = (float[])PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length);
                            if (PlcRwAreaConfig.FloatValues == null) readFailed = true;
                            break;
                        }
                    case "System.Double":
                    case "System.Double[]":
                        {
                            PlcRwAreaConfig.DoubleValues = (double[])PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length);
                            if (PlcRwAreaConfig.DoubleValues == null) readFailed = true;
                            break;
                        }
                    case "System.String":
                        {
                            PlcRwAreaConfig.StringValues = ((string)PlcRwAreaConfig.PlcConfig.PLC.Read<T>(tag.tagName, ref msg, (ushort)PlcRwAreaConfig.Length)).Trim();
                            if (PlcRwAreaConfig.StringValues == null) readFailed = true;
                            break;
                        }
                    default:
                        {
                            LogInDB.Error($"[{dataType.ToString()}] error!!");
                            return false;
                        }
                }
                if(readFailed)
                {
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Error($"读取地址段ID：{PlcRwAreaConfig.DID}，地址段名称{PlcRwAreaConfig.Name}错误：{msg}");
                    }
                    else
                    {
                        LogInDB.Error($"Read address segment ID ：{PlcRwAreaConfig.DID}，Address segment name {PlcRwAreaConfig.Name} error：{msg}");
                    }
                    return false;
                }
                else
                {
                    if(!SetTagUserDefineVariable<T>(PlcRwAreaConfig))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                Thread.Sleep(3000);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将缓存中数据解析出来刷新到UserDefineVariable中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PlcRwAreaConfig"></param>
        /// <returns></returns>
        private bool SetTagUserDefineVariable<T>(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            try
            {
                Type dataType = typeof(T);
                foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null
            && x.PlcRwConfig.DID == PlcRwAreaConfig.DID).ToList())//PLC变量
                {
                    int startIndex = int.Parse(UserDefineVariable.PlcAddressInt);
                    switch (dataType.ToString())
                    {
                        case "System.Int16":
                        case "System.Int16[]":
                            {
                                if (PlcRwAreaConfig.Int16Values.Count() < startIndex + UserDefineVariable.VariableLength)
                                {
                                    //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                    }
                                    Thread.Sleep(3000);
                                }
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = PlcRwAreaConfig.Int16Values.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.Int16Values[startIndex];
                                }
                            }
                            break;
                        case "System.Int32":
                        case "System.Int32[]":
                            {
                                if (PlcRwAreaConfig.Int32Values.Count() < startIndex + UserDefineVariable.VariableLength)
                                {
                                    //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                    }
                                    Thread.Sleep(3000);
                                }
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = PlcRwAreaConfig.Int32Values.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.Int32Values[startIndex];
                                }
                            }
                            break;
                        case "System.Int64":
                        case "System.Int64[]":
                            {
                                if (PlcRwAreaConfig.Int64Values.Count() < startIndex + UserDefineVariable.VariableLength)
                                {
                                    //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                    }
                                    Thread.Sleep(3000);
                                }
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = PlcRwAreaConfig.Int64Values.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.Int64Values[startIndex];
                                }
                            }
                            break;
                        case "System.Boolean":
                        case "System.Boolean[]":
                            {
                                if (PlcRwAreaConfig.BitValues.Count() < startIndex + UserDefineVariable.VariableLength)
                                {
                                    //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                    }
                                    Thread.Sleep(3000);
                                }
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = PlcRwAreaConfig.BitValues.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.BitValues[startIndex];
                                }
                            }
                            break;
                        case "System.Single":
                        case "System.Single[]":
                            {
                                if (PlcRwAreaConfig.FloatValues.Count() < startIndex + UserDefineVariable.VariableLength)
                                {
                                    //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                    }
                                    Thread.Sleep(3000);
                                }
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = PlcRwAreaConfig.FloatValues.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.FloatValues[startIndex];
                                }
                            }
                            break;
                        case "System.Double":
                        case "System.Double[]":
                            {
                                if (PlcRwAreaConfig.DoubleValues.Count() < startIndex + UserDefineVariable.VariableLength)
                                {
                                    //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                    }
                                    Thread.Sleep(3000);
                                }
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = PlcRwAreaConfig.DoubleValues.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.DoubleValues[startIndex];
                                }
                            }
                            break;
                        case "System.String":
                            {
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.StringValues.Trim();
                            }
                            break;
                        default:
                            {
                                LogInDB.Error($"[{dataType.ToString()}] error!!");
                                return false;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将UserDefineVariable数据写入缓存中
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void SetTagPlcRwAreaValueFromUserDefineVariable(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            DeviceTag tag = DeviceTag.lstDeviceTags.Where(x => x.tagName == PlcRwAreaConfig.StartAddress && x.plcID == PlcRwAreaConfig.plcID).FirstOrDefault();
            if (tag == null) return;
            foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null
            && x.PlcRwConfig.DID == PlcRwAreaConfig.DID).ToList())//PLC变量
            {
                switch (tag.ValueType)
                {
                    case "Bit":
                    case "Bool":
                        {
                            PlcRwAreaConfig.BitValues[int.Parse(UserDefineVariable.PlcAddressInt)] =
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString() == "True" ? true : false;
                        }
                        break;
                    case "Double":
                        {
                            PlcRwAreaConfig.DoubleValues[int.Parse(UserDefineVariable.PlcAddressInt)] =
                                double.Parse(UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString());
                        }
                        break;
                    case "Float":
                        {
                            PlcRwAreaConfig.FloatValues[int.Parse(UserDefineVariable.PlcAddressInt)] =
                                float.Parse(UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString());
                        }
                        break;
                    case "Int16":
                        {
                            PlcRwAreaConfig.Int16Values[int.Parse(UserDefineVariable.PlcAddressInt)] =
                                short.Parse(UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString());
                        }
                        break;
                    case "Int32":
                        {
                            PlcRwAreaConfig.Int32Values[int.Parse(UserDefineVariable.PlcAddressInt)] =
                                int.Parse(UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString());
                        }
                        break;
                    case "Int64":
                        {
                            PlcRwAreaConfig.Int64Values[int.Parse(UserDefineVariable.PlcAddressInt)] =
                                long.Parse(UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString());
                        }
                        break;
                    case "String":
                        {
                            PlcRwAreaConfig.StringValues = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 解析报警，报警对于标签通信，只能是Int16类型或者bool
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void GetTagAlarms(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            DeviceTag tag = DeviceTag.lstDeviceTags.Where(x => x.tagName == PlcRwAreaConfig.StartAddress && x.plcID == PlcRwAreaConfig.plcID).FirstOrDefault();
            if (tag == null) return;
            if ((tag.ValueType == "Bit" || tag.ValueType == "Bool") && PlcRwAreaConfig.BitValues != null && PlcRwAreaConfig.BitValues.Count() > 0)
            {
                foreach (var alarm in DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => x.PlcRwAreaConfigurationDID == PlcRwAreaConfig.DID
                && !string.IsNullOrEmpty(x.AlarmBitAddr)))
                {
                    alarm.IsAlarming = PlcRwAreaConfig.BitValues[(int)alarm.Array_index];
                }
            }
            else if (tag.ValueType == "Int16")
            {
                foreach (var alarm in DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => x.PlcRwAreaConfigurationDID == PlcRwAreaConfig.DID
                && !string.IsNullOrEmpty(x.AlarmBitAddr)))
                {
                    if(alarm.PlcAddressBit.HasValue)
                    {
                        int Int16Values = PlcRwAreaConfig.Int16Values[(int)alarm.Array_index];
                        byte[] bt = BitConverter.GetBytes(Int16Values);//转化后的byte[16]
                        bool[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransBool(bt, 0, 2);
                        alarm.IsAlarming = array[int.Parse(alarm.PlcAddressBit.ToString())]; //更新报警
                    }
                    else
                    {
                        alarm.IsAlarming = PlcRwAreaConfig.Int16Values[(int)alarm.Array_index] == 1;
                    }
                }
            }
            else if (tag.ValueType == "Int32")
            {
                foreach (var alarm in DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => x.PlcRwAreaConfigurationDID == PlcRwAreaConfig.DID
                && !string.IsNullOrEmpty(x.AlarmBitAddr)))
                {
                    if (alarm.PlcAddressBit.HasValue)
                    {
                        int Int32Values = PlcRwAreaConfig.Int32Values[(int)alarm.Array_index];
                        byte[] bt = BitConverter.GetBytes(Int32Values);//转化后的byte[16]
                        bool[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransBool(bt, 0, 4);
                        alarm.IsAlarming = array[int.Parse(alarm.PlcAddressBit.ToString())]; //更新报警
                    }
                    else
                    {
                        alarm.IsAlarming = PlcRwAreaConfig.Int32Values[(int)alarm.Array_index] == 1;
                    }
                }
            }
        }

        /// <summary>
        /// 解析出监控值
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void GetTagChangeMonitorValue(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            DeviceTag tag = DeviceTag.lstDeviceTags.Where(x => x.tagName == PlcRwAreaConfig.StartAddress && x.plcID == PlcRwAreaConfig.plcID).FirstOrDefault();
            if (tag == null) return;
            foreach (var deviceInputOutputConfigInfo in DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => x.InputChangeMonitorAddressInt.HasValue
            && x.InputChangeMonitorPlcRwArea != null && x.InputChangeMonitorPlcRwArea.DID == PlcRwAreaConfig.DID))
            {
                switch (deviceInputOutputConfigInfo.Type.ToUpper())
                {
                    case "FLOAT":
                        {
                            string s = deviceInputOutputConfigInfo.InputChangeMonitorAddr.Replace(PlcRwAreaConfig.StartAddress, "");
                            int s_index = 0;
                            if (s.Contains("[") && s.Contains("]") && ((s.IndexOf(']') - s.IndexOf('[')) > 1))
                            {
                                int.TryParse(s.Substring(1, s.IndexOf(']') - 1), out s_index);
                            }
                            deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.FloatValues[s_index].ToString();
                        }
                        break;
                    case "DOUBLE":
                        {
                            string s = deviceInputOutputConfigInfo.InputChangeMonitorAddr.Replace(PlcRwAreaConfig.StartAddress, "");
                            int s_index = 0;
                            if (s.Contains("[") && s.Contains("]") && ((s.IndexOf(']') - s.IndexOf('[')) > 1))
                            {
                                int.TryParse(s.Substring(1, s.IndexOf(']') - 1), out s_index);
                            }
                            deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.DoubleValues[s_index].ToString();
                        }
                        break;
                    case "INT16":
                        {
                            string s = deviceInputOutputConfigInfo.InputChangeMonitorAddr.Replace(PlcRwAreaConfig.StartAddress, "");
                            int s_index = 0;
                            if (s.Contains("[") && s.Contains("]") && ((s.IndexOf(']') - s.IndexOf('[')) > 1))
                            {
                                int.TryParse(s.Substring(1, s.IndexOf(']') - 1), out s_index);
                            }
                            deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.Int16Values[s_index].ToString();
                        }
                        break;
                    case "INT32":
                        {
                            string s = deviceInputOutputConfigInfo.InputChangeMonitorAddr.Replace(PlcRwAreaConfig.StartAddress, "");
                            int s_index = 0;
                            if (s.Contains("[") && s.Contains("]") && ((s.IndexOf(']') - s.IndexOf('[')) > 1))
                            {
                                int.TryParse(s.Substring(1, s.IndexOf(']') - 1), out s_index);
                            }
                            deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.Int32Values[s_index].ToString();
                        }
                        break;
                    case "INT64":
                        {
                            string s = deviceInputOutputConfigInfo.InputChangeMonitorAddr.Replace(PlcRwAreaConfig.StartAddress, "");
                            int s_index = 0;
                            if (s.Contains("[") && s.Contains("]") && ((s.IndexOf(']') - s.IndexOf('[')) > 1))
                            {
                                int.TryParse(s.Substring(1, s.IndexOf(']') - 1), out s_index);
                            }
                            deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.Int64Values[s_index].ToString();
                        }
                        break;
                    default:
                        {
                            LogInDB.Error($"deviceInputOutputConfigInfo.Type: [{deviceInputOutputConfigInfo.Type}] error!!");
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 从缓存中刷新数据写入PLC
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void WriteToTagPLC(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            DeviceTag tag = DeviceTag.lstDeviceTags.Where(x => x.tagName == PlcRwAreaConfig.StartAddress
            && x.plcID == PlcRwAreaConfig.plcID).FirstOrDefault();
            if (tag == null) return;
            bool writeSucess = false;
            string msg = string.Empty;
            switch (tag.ValueType)
            {
                case "Bit":
                case "Bool":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.BitValues, ref msg);
                    }
                    break;
                case "Double":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.DoubleValues, ref msg);
                    }
                    break;
                case "Float":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.FloatValues, ref msg);
                    }
                    break;
                case "Int16":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.Int16Values, ref msg);
                    }
                    break;
                case "Int32":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.Int32Values, ref msg);
                    }
                    break;
                case "Int64":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.Int64Values, ref msg);
                    }
                    break;
                case "String":
                    {
                        writeSucess = PlcRwAreaConfig.PlcConfig.PLC.Write(tag.tagName, PlcRwAreaConfig.StringValues, ref msg);
                    }
                    break;
            }
            if(!writeSucess)
            {
                LogInDB.Error($" Write{PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress} error!! {msg}");
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region 非标签通信

        /// <summary>
        /// 读取PLC里值到W类型的地址段
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        /// <returns></returns>
        private bool InitPLClcRwArea(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            if (PlcRwAreaConfig.ByteValues == null)
            {
                if (PlcRwAreaConfig.AreaName != "M")  //目前只认为M区为位区，其他的包括R区均为寄存器区，DB区为8位寄存器，其他为16位寄存器
                {
                    bool readsuccess = GetPlcRwAreaByteValues(PlcRwAreaConfig);
                    if (readsuccess)
                    {
                        ReadBytesToValues(PlcRwAreaConfig);
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    return GetPlc_M_BoolsValues(PlcRwAreaConfig);
                }
            }
            return true;
        }

        /// <summary>
        /// PLC读取的缓存bute[]转user_define_variable中字段的值
        /// </summary>
        /// <param name="ValueType"></param>
        /// <param name="read"></param>
        /// <param name="d"></param>
        /// <param name="UserDefineVariable"></param>
        /// <param name="PlcRwAreaConfig"></param>
        private void ReadBytesToValues(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            try
            {
                bool isS7plc = PlcRwAreaConfig.PlcConfig.isS7plc;
                foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null
                && x.PlcRwConfig.DID == PlcRwAreaConfig.DID).ToList())//PLC变量
                {
                    readBytesToValues(isS7plc, UserDefineVariable, PlcRwAreaConfig);
                }
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// PLC读取的缓存bute[]转user_define_variable中字段的值
        /// </summary>
        /// <param name="ValueType"></param>
        /// <param name="read"></param>
        /// <param name="d"></param>
        /// <param name="UserDefineVariable"></param>
        /// <param name="PlcRwAreaConfig"></param>
        private void readBytesToValues(bool isS7plc, UserDefineVariableInfo UserDefineVariable, PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            try
            {
                switch (UserDefineVariable.ValueType.ToUpper())
                {
                    #region

                    case "BIT":
                        {
                            if (isS7plc)
                            {
                                byte b = PlcRwAreaConfig.ByteValues[int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)];
                                bool[] Barray = SoftBasic.ByteToBoolArray(new byte[] { b });
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = Barray[int.Parse(UserDefineVariable.PlcAddressBit.ToString())]; //更新报警
                            }
                            else if (PlcRwAreaConfig.AreaName == "CIO")
                            {

                            }
                            else
                            {
                                short shortarray = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress)) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                byte[] bt = BitConverter.GetBytes(shortarray);//转化后的byte[16]
                                bool[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransBool(bt, 0, 2);
                                if (UserDefineVariable.VariableLength > 1)
                                {
                                    var res = array.Skip(UserDefineVariable.PlcAddressBit.Value).Take(UserDefineVariable.VariableLength).ToList<bool>();
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                                }
                                else
                                {   //三菱PLC D区 DCBA解码测试OK；Omron PLC D区 CDAB解码测试OK；
                                    UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array[UserDefineVariable.PlcAddressBit.Value];
                                }
                            }
                        }
                        break;
                    case "FLOAT":
                        {
                            if (UserDefineVariable.VariableLength > 1)
                            {
                                float[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransSingle(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, UserDefineVariable.VariableLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", array);
                            }
                            else
                            {   //三菱PLC D区 DCBA解码测试OK；Omron PLC D区 CDAB解码测试OK；
                                float array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransSingle(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array;
                            }
                        }
                        break;
                    case "DOUBLE":
                        {
                            if (UserDefineVariable.VariableLength > 1)
                            {
                                double[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransDouble(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, UserDefineVariable.VariableLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", array);
                            }
                            else
                            {   //三菱PLC D区 DCBA解码测试OK；Omron PLC D区 CDAB解码测试OK；
                                double array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransDouble(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array;
                            }
                        }
                        break;
                    case "INT16":
                        {
                            if (UserDefineVariable.VariableLength > 1)
                            {
                                short[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, UserDefineVariable.VariableLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", array);
                            }
                            else
                            {   //三菱PLC D区 DCBA解码测试OK；Omron PLC D区 CDAB解码测试OK；
                                short array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array;
                            }
                        }
                        break;
                    case "INT32":
                        {
                            if (UserDefineVariable.VariableLength > 1)
                            {
                                Int32[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt32(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, UserDefineVariable.VariableLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", array);
                            }
                            else
                            {   //三菱PLC D区 DCBA解码测试OK；Omron PLC D区 CDAB解码测试OK；
                                Int32 array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt32(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array;
                            }
                        }
                        break;
                    case "INT64":
                        {
                            if (UserDefineVariable.VariableLength > 1)
                            {
                                Int64[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt64(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, UserDefineVariable.VariableLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", array);
                            }
                            else
                            {
                                Int64 array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt64(PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                    (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array;
                            }
                        }
                        break;
                    case "STRING":
                        {
                            string array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransString
                                (
                                    PlcRwAreaConfig.ByteValues,
                                    (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) :
                                        (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress)))
                                        * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, UserDefineVariable.VariableLength * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, Encoding.ASCII
                                ).Split('\0')[0].Trim();
                            UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = array;
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                //LogInDB.Error("PLC读取的缓存bute[]转user_define_variable中字段的值存在异常" + ex.ToString());
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PLC读取的缓存bute[]转user_define_variable中字段的值存在异常" + ex.ToString());
                }
                else
                {
                    LogInDB.Error("There is an exception in the conversion of bute[] read by PLC to the field value in user_define_variable" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 从PLC里读取数据到Byte数组缓存中
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        /// <returns></returns>
        private bool GetPlcRwAreaByteValues(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            try
            {
                string msg = string.Empty;
                object obj = PlcRwAreaConfig.PlcConfig.PLC.Read<byte[]>(PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress, ref msg, (ushort)(PlcRwAreaConfig.Length));
                if (obj != null)
                {
                    PlcRwAreaConfig.ByteValues = (byte[])obj;
                    return true;
                }
                else
                {
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Error($"读取地址段ID：{PlcRwAreaConfig.DID}，地址段名称{PlcRwAreaConfig.Name}错误：" + msg);
                    }
                    else
                    {
                        LogInDB.Error($"Read address segment ID ：{PlcRwAreaConfig.DID}，Address segment name {PlcRwAreaConfig.Name} error：" + msg);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                if (PlcConfig.PLC == null)
                    return false;
                Thread.Sleep(3000);
                return false;
            }
        }

        /// <summary>
        /// 读取PLC里的bool类型数据，输出给UserDefineVariableInfo
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        /// <returns></returns>
        private bool GetPlc_M_BoolsValues(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            try
            {
                bool isS7plc = PlcRwAreaConfig.PlcConfig.isS7plc;
                string msg = string.Empty;
                PlcRwAreaConfig.BitValues = (bool[])PlcRwAreaConfig.PlcConfig.PLC.Read<bool[]>(PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress, ref msg, (ushort)PlcRwAreaConfig.Length);
                if (PlcRwAreaConfig.BitValues != null)
                {
                    foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null
                    && x.PlcRwConfig.DID == PlcRwAreaConfig.DID).ToList())//PLC变量
                    {
                        if (UserDefineVariable.ValueType.ToUpper() == "BOOL")
                        {
                            int startIndex = 0;
                            if (isS7plc)
                            {
                                startIndex = int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress);
                            }
                            else
                            {
                                startIndex = int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress);
                            }
                            if (PlcRwAreaConfig.BitValues.Count() < startIndex + UserDefineVariable.VariableLength)
                            {
                                //LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    LogInDB.Error($"当前读取的[{PlcRwAreaConfig.StartAddress}]数组长度不够");
                                }
                                else
                                {
                                    LogInDB.Error($"The length of array [{PlcRwAreaConfig.StartAddress}] currently read is not enough");
                                }
                                Thread.Sleep(3000);
                                return false;
                            }
                            if (UserDefineVariable.VariableLength > 1)
                            {
                                var res = PlcRwAreaConfig.BitValues.Skip(startIndex).Take(UserDefineVariable.VariableLength).ToList();
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = string.Join(",", res);
                            }
                            else
                            {
                                UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName] = PlcRwAreaConfig.BitValues[startIndex];
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    LogInDB.Error($"Read [{PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress}] Error!");
                    return false;
                }
            }
            catch(Exception ex)
            {
                LogInDB.Error(ex.ToString());
                Thread.Sleep(3000);
                return false;
            }
        }
        
        /// <summary>
        /// 解析报警
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void GetPlcAlarms(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            foreach (var alarm in DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => x.PlcRwAreaConfigurationDID == PlcRwAreaConfig.DID && !string.IsNullOrEmpty(x.AlarmBitAddr)))
            {
                if (PlcRwAreaConfig.PlcConfig.isS7plc)
                {
                    if (alarm.PlcAddressBit.Value > 7)  //一般在地址校验的时候会将大于7的校验失败。
                    {
                        //先用byteTransform转short和PLC值一致，再转转bool[] 取bool值
                        short ShortValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (int.Parse(alarm.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)));  //read.Content为Byte数组, 按从低字节到高字节来排列。相邻的两字字节转换为short值的时候, 对于同样的bool 排列，西门子PLC得出的short值与三菱PLC得出的short值是不一样的。
                                                                                                                                                                                   //所以short转bool数组的时候，西门子PLC和三菱PLC的处理方式不一样。
                        byte[] bt = HslCommunication.BasicFramework.SoftBasic.BytesReverseByWord(BitConverter.GetBytes(ShortValue));//西门子PLC，颠倒Byte数组
                        bool[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransBool(bt, 0, 2);
                        alarm.IsAlarming = array[int.Parse(alarm.PlcAddressBit.ToString())]; //更新报警
                    }
                    else
                    {
                        byte b = PlcRwAreaConfig.ByteValues[int.Parse(alarm.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)];
                        bool[] Barray = SoftBasic.ByteToBoolArray(new byte[] { b });
                        alarm.IsAlarming = Barray[int.Parse(alarm.PlcAddressBit.ToString())]; //更新报警
                    }
                }
                else if (PlcRwAreaConfig.PlcConfig.ProtocolName == "ADS_D")  //采用两个字节组成的Int16字来表示一个报警
                {
                    if(alarm.PlcAddressBit.HasValue)
                    {
                        short ShortValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (int.Parse(alarm.PlcAddressInt) -
                        int.Parse(PlcRwAreaConfig.StartAddress)) * 2);
                        byte[] bt = BitConverter.GetBytes(ShortValue);//转化后的byte[16]
                        bool[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransBool(bt, 0, 2);
                        alarm.IsAlarming = array[int.Parse(alarm.PlcAddressBit.ToString())]; //更新报警
                    }
                    else
                    {
                        byte b = PlcRwAreaConfig.ByteValues[int.Parse(alarm.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress)];
                        alarm.IsAlarming = b == 1; //更新报警
                    }
                }
                else if (!alarm.PlcAddressBit.HasValue)
                {
                    short ShortValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (int.Parse(alarm.PlcAddressInt) -
                        int.Parse(PlcRwAreaConfig.StartAddress)) * 2);
                    alarm.IsAlarming = ShortValue == 1; //更新报警
                }
                else  //一个字16位，16个报警
                {
                    //先用byteTransform转short和PLC值一致，再转转bool[] 取bool值
                    short ShortValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (int.Parse(alarm.PlcAddressInt) - 
                        int.Parse(PlcRwAreaConfig.StartAddress)) * 2);
                    byte[] bt = BitConverter.GetBytes(ShortValue);//转化后的byte[16]
                    bool[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransBool(bt, 0, 2);
                    alarm.IsAlarming = array[int.Parse(alarm.PlcAddressBit.ToString())]; //更新报警
                }
            }
        }
        
        /// <summary>
        /// 从M区解析出报警来
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void GetPlcAlarmsFrom_M_Area(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            if (PlcRwAreaConfig.BitValues == null) return;
            //报警对于标签通信，只能是Int16类型或者bool
            foreach (var alarm in DeviceAlertConfigInfo.lstDeviceAlertConfigInfos.Where(x => x.PlcRwAreaConfigurationDID == PlcRwAreaConfig.DID
            && !string.IsNullOrEmpty(x.AlarmBitAddr)))
            {
                if (!alarm.Array_index.HasValue)
                {
                    LogInDB.Error("Soft Bug!!!!!!!!");
                    continue;
                }
                alarm.IsAlarming = PlcRwAreaConfig.BitValues[(int)alarm.Array_index];
            }
        }

        /// <summary>
        /// 解析出监控值
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void GetPlcChangeMonitorValue(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            bool isS7plc = PlcRwAreaConfig.PlcConfig.isS7plc;
            foreach (var deviceInputOutputConfigInfo in DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => x.InputChangeMonitorAddressInt.HasValue 
            && x.InputChangeMonitorPlcRwArea != null && x.InputChangeMonitorPlcRwArea.DID == PlcRwAreaConfig.DID))
            {
                switch (deviceInputOutputConfigInfo.Type.ToUpper())
                {
                    case "DOUBLE":
                        deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransDouble(PlcRwAreaConfig.ByteValues, 
                            (deviceInputOutputConfigInfo.InputChangeMonitorAddressInt.Value - int.Parse(isS7plc ? PlcRwAreaConfig.S7StartAddress : PlcRwAreaConfig.StartAddress)) 
                            * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength).ToString();
                        break;
                    case "FLOAT":
                        deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransSingle(PlcRwAreaConfig.ByteValues, 
                            (deviceInputOutputConfigInfo.InputChangeMonitorAddressInt.Value - int.Parse(isS7plc ? PlcRwAreaConfig.S7StartAddress : PlcRwAreaConfig.StartAddress)) 
                            * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength).ToString();
                        break;
                    case "INT32":
                        deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt32(PlcRwAreaConfig.ByteValues, 
                            (deviceInputOutputConfigInfo.InputChangeMonitorAddressInt.Value - int.Parse(isS7plc ? PlcRwAreaConfig.S7StartAddress : PlcRwAreaConfig.StartAddress)) 
                            * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength).ToString();
                        break;
                    case "INT16":
                        {
                            deviceInputOutputConfigInfo.ChangeMonitorValue = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, 
                                (deviceInputOutputConfigInfo.InputChangeMonitorAddressInt.Value - int.Parse(isS7plc ? PlcRwAreaConfig.S7StartAddress : PlcRwAreaConfig.StartAddress)) 
                                * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength).ToString();
                        }
                        break;
                }
            }
        }
        
        /// <summary>
        /// 将bytes数组输出到PLC中
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void WriteBytesToPLC(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            string msg = string.Empty;
            if (!PlcRwAreaConfig.PlcConfig.PLC.Write(PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress, PlcRwAreaConfig.ByteValues, ref msg))
            {
                LogInDB.Error($" Write{PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress} error!! {msg}");
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 将UserDefineVariable中的值解析到地址段bytes缓存中
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void SetPlcRwAreaBytesValueFromUserDefineVariable(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            bool isS7plc = PlcRwAreaConfig.PlcConfig.isS7plc;
            foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null
                                            && x.PlcRwConfig.DID == PlcRwAreaConfig.DID).ToList())//PLC变量
            {
                switch (UserDefineVariable.ValueType.ToUpper())
                {
                    case "BIT":
                        {
                            if (isS7plc)
                            {
                                byte b = PlcRwAreaConfig.ByteValues[int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)];
                                bool[] Barray = SoftBasic.ByteToBoolArray(new byte[] { b });
                                string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                                bool[] boolarray = Array.ConvertAll(str, s => bool.Parse(s));
                                Array.Copy(boolarray, 0, Barray, int.Parse(UserDefineVariable.PlcAddressBit.ToString()), boolarray.Length);//更新字节
                                byte[] array = SoftBasic.BoolArrayToByte(Barray);
                                Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) 
                                    * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);//更新缓存
                            }
                            else if (PlcRwAreaConfig.AreaName == "CIO")
                            {

                            }
                            else
                            {
                                short shortarray = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransInt16(PlcRwAreaConfig.ByteValues, (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress)) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength);
                                byte[] bt = BitConverter.GetBytes(shortarray);//转化后的byte[16]
                                                                              //缓存的字操作 取出缓存的两个字节 更新后放回缓存
                                bool[] buffer = SoftBasic.ByteToBoolArray(bt);//要更新bool变量时，从缓存中获取更改后的缓存
                                string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                                bool[] boolarray = Array.ConvertAll(str, s => bool.Parse(s));
                                Array.Copy(boolarray, 0, buffer, int.Parse(UserDefineVariable.PlcAddressBit.ToString()), boolarray.Length);//更新字节
                                byte[] array = SoftBasic.BoolArrayToByte(buffer);
                                //array = byteTransform.BytesReverseByWord(array);
                                Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress)) 
                                    * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);//更新缓存
                            }
                        }
                        break;
                    case "FLOAT":
                        {
                            string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                            List<byte> byteSource = new List<byte>();
                            for (int i = 0; i < str.Length; i++)
                            {
                                byteSource.AddRange(PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransByte(float.Parse(str[i]))); ;
                            }
                            byte[] array = byteSource.ToArray();
                            Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : 
                                (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);
                        }
                        break;
                    case "DOUBLE":
                        {
                            string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                            List<byte> byteSource = new List<byte>();
                            for (int i = 0; i < str.Length; i++)
                            {
                                byteSource.AddRange(PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransByte(double.Parse(str[i]))); ;
                            }
                            byte[] array = byteSource.ToArray();
                            Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : 
                                (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);

                        }
                        break;
                    case "INT16":
                        {

                            string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                            List<byte> byteSource = new List<byte>();
                            for (int i = 0; i < str.Length; i++)
                            {
                                byteSource.AddRange(PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransByte(Int16.Parse(str[i]))); ;
                            }
                            byte[] array = byteSource.ToArray();
                            Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : 
                                (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);
                        }
                        break;
                    case "INT32":
                        {
                            string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                            List<byte> byteSource = new List<byte>();
                            for (int i = 0; i < str.Length; i++)
                            {
                                byteSource.AddRange(PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransByte(Int32.Parse(str[i]))); ;
                            }
                            byte[] array = byteSource.ToArray();
                            Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : 
                                (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);

                        }
                        break;
                    case "INT64":
                        {
                            string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                            List<byte> byteSource = new List<byte>();
                            for (int i = 0; i < str.Length; i++)
                            {
                                byteSource.AddRange(PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransByte(Int64.Parse(str[i]))); ;
                            }
                            byte[] array = byteSource.ToArray();
                            Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : 
                                (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);
                        }
                        break;
                    case "STRING":
                        {
                            byte[] array = PlcRwAreaConfig.PlcConfig.PLC._ByteTransform.TransByte(UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString(), Encoding.ASCII);
                            Array.Copy(array, 0, PlcRwAreaConfig.ByteValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress)) : 
                                (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))) * PlcRwAreaConfig.PlcConfig.PLC.AddressWordLength, array.Length);
                        }
                        break;
                }
            }
        }
        
        /// <summary>
        /// 输出bool数组到M区
        /// </summary>
        /// <param name="PlcRwAreaConfig"></param>
        private void Write_M_BoolsToPLC(PlcRwAreaConfigurationInfo PlcRwAreaConfig)
        {
            bool isS7plc = PlcRwAreaConfig.PlcConfig.isS7plc;
            if (PlcRwAreaConfig.BitValues == null) return;
            string msg = string.Empty;
            foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null && x.PlcRwConfig.DID == PlcRwAreaConfig.DID).ToList())//PLC变量
            {
                if (UserDefineVariable.ValueType.ToUpper() == "BOOL")
                {
                    string[] str = UserDefineVariableInfo.DicVariables[UserDefineVariable.VariableName.ToString()].ToString().Split(',');
                    bool[] arraybool = Array.ConvertAll(str, s => bool.Parse(s));
                    //byte[] array = boolTobyte(arraybool);
                    Array.Copy(arraybool, 0, PlcRwAreaConfig.BitValues, (isS7plc ? (int.Parse(UserDefineVariable.S7PlcAddressInt) - int.Parse(PlcRwAreaConfig.S7StartAddress))
                        : (int.Parse(UserDefineVariable.PlcAddressInt) - int.Parse(PlcRwAreaConfig.StartAddress))), arraybool.Length);
                }
            }
            if (!PlcRwAreaConfig.PlcConfig.PLC.Write(PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress, PlcRwAreaConfig.BitValues, ref msg))//把更新后的缓存写到PLC 如无更新把W区初始化的缓存写到PLC
            {
                LogInDB.Error($" Write{PlcRwAreaConfig.AreaName + PlcRwAreaConfig.StartAddress} error!!");
                Thread.Sleep(1000);
            }
        }

        #endregion

        private bool InitializationPlcRwAreaConfig()
        {
            foreach (var PlcRwAreaConfig in PlcRwAreaConfigurationInfo.lstPlcRwAreaConfiguration.Where(x => x.PlcRwField == "W" && x.PlcConfig.Enabled == 1))
            {
                switch (PlcRwAreaConfig.PlcConfig.ProtocolName)
                {
                    //Omron 标签读写
                    case "CIP":
                    case "ADS":
                        {
                            if (!ReadTagPlcRwAreaConfig(PlcRwAreaConfig))
                                return false;
                            break;
                        }
                    default:
                        {
                            if (!InitPLClcRwArea(PlcRwAreaConfig))
                                return false;
                            break;
                        }
                }
            }
            return true;
        }
        
        public void flushArea()
        {
            try
            {
                foreach (var PlcRwAreaConfig in PlcRwAreaConfigurationInfo.lstPlcRwAreaConfiguration.Where(x => (x.PlcRwField != "NotRW" && x.PlcConfig.Enabled == 1)))
                {
                    TimeSpan tp = DateTime.Now - PlcRwAreaConfig.lastTime;
                    if (tp.TotalMilliseconds > PlcRwAreaConfig.Cycle)//读取周期时间
                    {
                        switch (PlcRwAreaConfig.PlcConfig.ProtocolName)
                        {
                            case "CIP":
                            case "ADS":
                                {
                                    //标签读写
                                    if (PlcRwAreaConfig.PlcRwField == "R")
                                    {
                                        if (ReadTagPlcRwAreaConfig(PlcRwAreaConfig))
                                        {
                                            GetTagAlarms(PlcRwAreaConfig);
                                            GetTagChangeMonitorValue(PlcRwAreaConfig);
                                        }
                                    }
                                    else if (PlcRwAreaConfig.PlcRwField == "W")
                                    {
                                        SetTagPlcRwAreaValueFromUserDefineVariable(PlcRwAreaConfig);
                                        WriteToTagPLC(PlcRwAreaConfig);
                                    }
                                    break;
                                }
                            default:  //西门子、三菱、欧姆龙、ADS_D等非标签协议
                                #region 寄存器地址读写
                                {
                                    bool isS7plc = PlcRwAreaConfig.PlcConfig.isS7plc;
                                    if (PlcRwAreaConfig.AreaName != "M")
                                    {
                                        if (PlcRwAreaConfig.PlcRwField == "R")
                                        {
                                            //根据R地址段读PLC,读字
                                            bool readsuccess = GetPlcRwAreaByteValues(PlcRwAreaConfig);
                                            if (readsuccess)
                                            {
                                                GetPlcAlarms(PlcRwAreaConfig);
                                                GetPlcChangeMonitorValue(PlcRwAreaConfig);
                                                ReadBytesToValues(PlcRwAreaConfig);
                                            }
                                        }
                                        else if (PlcRwAreaConfig.ByteValues == null)
                                        {
                                            initReadWritingAreaSuccess = false;
                                        }
                                        else if (PlcRwAreaConfig.PlcRwField == "W" && PlcRwAreaConfig.ByteValues != null)//初始化成功后PlcRwAreaConfig.Values不为null才执行写操作
                                        {
                                            SetPlcRwAreaBytesValueFromUserDefineVariable(PlcRwAreaConfig);
                                            WriteBytesToPLC(PlcRwAreaConfig);
                                        }
                                    }
                                    else  //"M"区
                                    {
                                        #region 读写M、R区的位

                                        if (PlcRwAreaConfig.PlcRwField == "R")
                                        {
                                            GetPlc_M_BoolsValues(PlcRwAreaConfig);
                                            GetPlcAlarmsFrom_M_Area(PlcRwAreaConfig);
                                        }
                                        else if (PlcRwAreaConfig.PlcRwField == "W")
                                        {
                                            Write_M_BoolsToPLC(PlcRwAreaConfig);
                                        }
                                        #endregion
                                    }
                                    break;
                                }
                                #endregion
                        }
                        PlcRwAreaConfig.lastTime = DateTime.Now;
                    }
                }
                ATL.Core.PLC.Communicating = true;
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                Thread.Sleep(3000);
            }
        }

        #endregion

        #region 连接PLC
        public bool ConnectPLC()
        {
            string msg = string.Empty;
            try
            {
                // 连接对象
                if (PlcConfig.PLC.Connect(ref msg))
                {
                    //LogInDB.Error($"初始化连接PLC[{plcConfig.Address}]成功");
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Info($"初始化连接PLC[{PlcConfig.Address}]成功");
                    }
                    else
                    {
                        LogInDB.Info($"Initial connection to PLC[{ PlcConfig.Address }] success ");
                    }
                    if (PlcConfig.plcCommunicating)
                    {
                        return true;
                    }
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Info($"开启新的通信线程给PLC[{PlcConfig.Address}-{PlcConfig.AddressPara}]通信");
                    }
                    else
                    {
                        LogInDB.Info($"Start a new communicating thread for PLC[{PlcConfig.Address}-{PlcConfig.AddressPara}]");
                    }
                    plcCommunication();
                }
                else
                {
                    //LogInDB.Info($"初始化连接PLC[{plcConfig.Address}]failure：{connect.ToMessageShowString()}");
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Info($"初始化连接PLC[{PlcConfig.Address}]failure：{msg}");
                    }
                    else
                    {
                        LogInDB.Info($"Initial connection to PLC[{ PlcConfig.Address }] failed：{msg}");
                    }
                    return false;
                }
            }
            catch (Exception ex) {
                LogInDB.Error(ex.Message);
                return false;
            }
            return true;
        }

        #endregion

        #region ping PLC

        public void PingPLC()
        {
            while (!ATL.Core.Core.SoftClosing)
            {
                Ping ping = new Ping();
                if(PlcConfig.ProtocolName != "ADS" && PlcConfig.ProtocolName != "CIP" && PlcConfig.ProtocolName != "ADS_D")
                {
                    if (ping.Send(PlcConfig.Address).Status == IPStatus.Success)
                    {
                        if (ConnectPLC())
                            break;
                    }
                    else
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Info($"ping PLC[{PlcConfig.Address}]失败");
                        }
                        else
                        {
                            LogInDB.Info($"ping PLC[{ PlcConfig.Address }] 失败 ");
                        }
                    }
                }
                else
                {
                    if (ConnectPLC())
                        break;
                }
                Thread.Sleep(3000);
            }
        }

        #endregion

    }
}
