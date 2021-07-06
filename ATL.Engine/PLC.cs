using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ATL.Core;
using ATL.Common;
using ATL.PLC;

namespace ATL.Engine
{
    public class PLC
    {
        /// <summary>
        /// 写入自定义变量里的地址，支持R、W、NotRW
        /// </summary>
        /// <param name="VariableName">该参数不能是PLC寄存器地址，因为没有指定PLC ID的参数</param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool WriteByVariableName(string VariableName, object Value)
        {
            if (string.IsNullOrEmpty(VariableName.Trim())) return true;
            UserDefineVariableInfo Variable = UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.VariableName == VariableName).FirstOrDefault();
            if (Variable != null && (Variable.PlcRwConfig == null || (Variable.PlcRwConfig != null && Variable.PlcRwConfig.PlcRwField == "W")))//address为自定义变量：系统变量或者W类型的PLC变量
            {
                SaveToUserDefineVariableInfo(Variable, Value);
                return true;
            }
            else if(Variable != null || Variable.PlcRwConfig != null) //为读取类型的PLC变量
            {
                bool WriteSuccess = false;
                if(Variable.PlcRwConfig.PlcConfig.PLC == null)
                {
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Error($"PLC[{Variable.PlcRwConfig.PlcConfig.Address}]通信失败: 写入 {VariableName} 失败");
                    }
                    else
                    {
                        LogInDB.Error($"PLC[{Variable.PlcRwConfig.PlcConfig.Address}]Communication Failure: Write {VariableName} Failure");
                    }
                    return false;
                }
                else
                {
                    WriteSuccess = Variable.PlcRwConfig.PlcConfig.PLC.WriteByAddress(Variable.PlcAddress, Variable.ValueType, Value);
                    if (WriteSuccess && Variable.PlcRwConfig.PlcRwField == "NotRW")
                    {
                        SaveToUserDefineVariableInfo(Variable, Value);
                    }
                    return WriteSuccess;
                }
                
            }
            //LogInDB.Error($"{VariableName}地址校验错误，代码逻辑BUG");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Error($"{VariableName}地址校验错误，代码逻辑BUG");
            }
            else
            {
                LogInDB.Error($"{VariableName} address verification error ，there is a bug in the Code logic ");
            }
            return false;
        }

        private void SaveToUserDefineVariableInfo(UserDefineVariableInfo Variable, object Value)
        {
            if (Variable.VariableLength == 1)
            {
                switch (Variable.ValueType)
                {
                    case "String":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = Value.ToString();
                        break;
                    case "Int16":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = (short)float.Parse(Value.ToString());
                        break;
                    case "Int32":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = (int)float.Parse(Value.ToString());
                        break;
                    case "Int64":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = (long)float.Parse(Value.ToString());
                        break;
                    case "Float":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = float.Parse(Value.ToString());
                        break;
                    case "Double":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = double.Parse(Value.ToString());
                        break;
                    case "Bit":
                    case "Bool":
                        UserDefineVariableInfo.DicVariables[Variable.VariableName] = bool.Parse(Value.ToString());
                        break;
                }
            }
            else if (Value.GetType().ToString() == "System.String")
            {
                UserDefineVariableInfo.DicVariables[Variable.VariableName] = Value.ToString();
            }
            else
            {
                Type dataType = Value.GetType();
                switch (dataType.ToString())
                {
                    case "System.Int16[]":
                        {
                            UserDefineVariableInfo.DicVariables[Variable.VariableName] = string.Join(",", ((short[])Value));
                            break;
                        }
                    case "System.Int32[]":
                        {
                            UserDefineVariableInfo.DicVariables[Variable.VariableName] = string.Join(",", ((int[])Value));
                            break;
                        }
                    case "System.Int64[]":
                        {
                            UserDefineVariableInfo.DicVariables[Variable.VariableName] = string.Join(",", ((long[])Value));
                            break;
                        }
                    case "System.Boolean[]":
                        {
                            UserDefineVariableInfo.DicVariables[Variable.VariableName] = string.Join(",", ((bool[])Value));
                            break;
                        }
                    case "System.Single[]":
                        {
                            UserDefineVariableInfo.DicVariables[Variable.VariableName] = string.Join(",", ((float[])Value));
                            break;
                        }
                    case "System.Double[]":
                        {
                            UserDefineVariableInfo.DicVariables[Variable.VariableName] = string.Join(",", ((double[])Value));
                            break;
                        }
                    default:
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Error($"[{Variable.VariableName}]配置的长度为{Variable.VariableLength},但是目前写入的数据类型为 [{dataType.ToString()}],不是数组类型，请检测配置!!");
                            }
                            else
                            {
                                LogInDB.Error($"the [{Variable.VariableName}] length is {Variable.VariableLength},but the type is [{dataType.ToString()}], it should be array，please check config !!");
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 支持倍福PLC标签、用户自定义变量
        /// </summary>
        /// <param name="VariableName"></param>
        /// <param name="isRealTime">是否需要立即从PLC里读取到实时值</param>
        /// <returns></returns>
        public string ReadByVariableName(string VariableName)
        {
            try
            {
                if (string.IsNullOrEmpty(VariableName.Trim())) return string.Empty;
                UserDefineVariableInfo Variable = UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.VariableName == VariableName).FirstOrDefault();
                if (Variable != null)
                {
                    if (Variable.PlcRwConfig == null || (Variable.PlcRwConfig != null && Variable.PlcRwConfig.PlcRwField == "R"))
                    {
                        return UserDefineVariableInfo.DicVariables[VariableName].ToString();
                    }
                    else
                    {
                        if (Variable.PlcRwConfig.PlcConfig.PLC == null)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Error($"PLC[{Variable.PlcRwConfig.PlcConfig.Address}]通信失败: 读取 {VariableName} 失败");
                            }
                            else
                            {
                                LogInDB.Error($"PLC[{Variable.PlcRwConfig.PlcConfig.Address}]Communication Failure: Read {VariableName} Failure");
                            }
                            return string.Empty;
                        }
                        else
                        {
                            bool ReadSuccess = false;
                            string Value = string.Empty;
                            try
                            {
                                bool b = Variable.PlcRwConfig.PlcConfig.PLC.ReadByAddress(Variable.PlcAddress, Variable.ValueType, out Value, (ushort)Variable.VariableLength);
                                if (!b)
                                    Value = string.Empty;
                                else
                                    ReadSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                LogInDB.Error(ex.ToString());
                                return string.Empty;
                            }
                            finally
                            {
                            }
                            if (ReadSuccess && Variable.PlcRwConfig.PlcRwField == "NotRW")
                            {
                                switch (Variable.ValueType)
                                {
                                    case "String":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                    case "Int16":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                    case "Int32":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                    case "Int64":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                    case "Float":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                    case "Double":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                    case "Bit":
                                    case "Bool":
                                        UserDefineVariableInfo.DicVariables[VariableName] = Value;
                                        break;
                                }
                            }
                            return Value;
                        }
                    }
                }
                else  //倍福PLC标签
                {
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Error($"未在user_define_variable表里找到变量：{VariableName}");
                    }
                    else
                    {
                        LogInDB.Error($"can not find VariableName:{VariableName} in Table user_define_variable");
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                return string.Empty;
            }
        }
        
        private void writeDemo1()
        {
            string PlcAddress = "D0";
            string ValueType = "Float";
            object Value = 5.5;
            PLCClientBase PLCClientBase = PlcConfigurationInfo.lstPlcConfiguration[0].PLC;
            if (PLCClientBase != null)
            {
                bool WriteSuccess = false;
                WriteSuccess = PLCClientBase.WriteByAddress(PlcAddress, ValueType, Value);
            }
        }
        
        private void writeDemo2()
        {
            string msg = string.Empty;
            PLCClientBase PLCClientBase = PlcConfigurationInfo.lstPlcConfiguration[0].PLC;
            if (PLCClientBase != null)
            {
                bool WriteSuccess = false;
                WriteSuccess = PLCClientBase.WriteByAddress("D0", "Float", "1.1,5,2.5");
                WriteSuccess = PLCClientBase.Write("D0", new float[] { 1.1F, 2.2F, 3.3F }, ref msg);
            }
        }

        private void readDemo1()
        {
            string PlcAddress = "D0";
            string ValueType = "Float";
            string Value;
            PLCClientBase plcClientBase = PlcConfigurationInfo.lstPlcConfiguration[0].PLC;
            if (plcClientBase != null)
            {
                bool readSuccess = plcClientBase.ReadByAddress(PlcAddress, ValueType, out Value);
            }
        }

        private void readDemo2()
        {
            string msg = string.Empty;
            PLCClientBase plcClientBase = PlcConfigurationInfo.lstPlcConfiguration[0].PLC;
            if (plcClientBase != null)
            {
                object i = plcClientBase.Read<short>("D0", ref msg);
                if(i != null)
                {
                    Console.WriteLine($"读取成功！！值为：{(short)i}");
                }
                else
                {
                    Console.WriteLine(msg);
                }
                //读取长度为5的float数组
                float[] ii = (float[])plcClientBase.Read<float[]>("D0", ref msg, 5);
                if (ii != null)
                {
                    Console.WriteLine($"读取成功！！");
                }
                else
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }
}
