using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.XRayTube;
using XRayClient.BatteryCheckManager;

namespace XRayClient.Core.Converters
{
    public class ConertersUtils
    {
        private static ATL.Engine.PLC plc = new ATL.Engine.PLC();
        public static List<string> BatterySeatToBatteryTestData(BatterySeat _bs, int checkType, int layerAC, int layerBD, bool isRecheck = false)
        {
            List<string> OutputParamItems = new List<string>();

            string oneitem = "";

            if (isRecheck == true)
            {
                if (checkType == 0 || checkType == 2)
                {
                    int paramID = 52166;
                    int paramID2 = 85144;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }


                    paramID = 52136;
                    paramID2 = 85092;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }
                }

                if (checkType == 1 || checkType == 2)
                {
                    int paramID = 52151;
                    int paramID2 = 85119;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }

                    paramID = 52181;
                    paramID2 = 85169;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }
                }


                //Input参数
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51740", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51741", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51011", XRayTubeIF.XRayTube1Stauts.ActualVoltage.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51012", XRayTubeIF.XRayTube1Stauts.ActualCurrent.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51856", _bs.ResultImgFileName);
                OutputParamItems.Add(oneitem);

                if (_bs.FinalResult == true)
                {
                    if (checkType == 0 || checkType == 2)
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", _bs.Corner1.InspectParams.max_length);
                    }
                    else
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", _bs.Corner2.InspectParams.max_length);
                    }

                }
                else
                {
                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "99.99");
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "-1");
                }

                OutputParamItems.Add(oneitem);
                return OutputParamItems;
            }

            if (plc.ReadByVariableName("NOOCV") == "0" && (_bs.VoltageResult == false || _bs.ResistanceResult == false))
            {
                //OCV
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "241", _bs.Voltage);
                OutputParamItems.Add(oneitem);
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "323", _bs.Resistance);
                OutputParamItems.Add(oneitem);
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "239", _bs.Temperature);
                OutputParamItems.Add(oneitem);
                return OutputParamItems;
            }
            else if ((plc.ReadByVariableName("NOMDI") == "0" || plc.ReadByVariableName("NOPPG") == "0") && (_bs.ThicknessResult == false || _bs.DimensionResult == false))
            {
                if (plc.ReadByVariableName("NOMDI") == "0")
                {
                    //CCD
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "310202", _bs.AllBatLength);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "310200", _bs.LeftLugMargin);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "310199", _bs.RightLugMargin);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78280", Math.Min(_bs.Left1WhiteGlue, _bs.Left2WhiteGlue));
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78279", Math.Min(_bs.Right1WhiteGlue, _bs.Right2WhiteGlue));
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "3303", _bs.BatWidth);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "305", _bs.BatLength);
                    OutputParamItems.Add(oneitem);
                }

                if (plc.ReadByVariableName("NOPPG") == "0")
                {
                    //PPG
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "93528", _bs.Thickness);
                    OutputParamItems.Add(oneitem);
                }
                return OutputParamItems;
            }
            else if (plc.ReadByVariableName("NOXRAY") == "0")
            {
                if (checkType == 0 || checkType == 2)
                {
                    int paramID = 52166;
                    int paramID2 = 85144;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }


                    paramID = 52136;
                    paramID2 = 85092;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }
                }

                if (checkType == 1 || checkType == 2)
                {
                    int paramID = 52151;
                    int paramID2 = 85119;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }

                    paramID = 52181;
                    paramID2 = 85169;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }
                }


                //Input参数
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51740", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51741", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51011", XRayTubeIF.XRayTube1Stauts.ActualVoltage.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51012", XRayTubeIF.XRayTube1Stauts.ActualCurrent.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51856", _bs.ResultImgFileName);
                OutputParamItems.Add(oneitem);

                if (plc.ReadByVariableName("NOMDI") == "0")
                {
                    //CCD
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "310202", _bs.AllBatLength);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "310200", _bs.LeftLugMargin);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "310199", _bs.RightLugMargin);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78280", Math.Min(_bs.Left1WhiteGlue, _bs.Left2WhiteGlue));
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78279", Math.Min(_bs.Right1WhiteGlue, _bs.Right2WhiteGlue));
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "3303", _bs.BatWidth);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "305", _bs.BatLength);
                    OutputParamItems.Add(oneitem);
                }

                if (plc.ReadByVariableName("NOThinckness") == "0")
                {
                    //PPG
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "93528", _bs.Thickness);
                    OutputParamItems.Add(oneitem);
                }
                if (plc.ReadByVariableName("NOOCV") == "0")
                {
                    //OCV
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "241", _bs.Voltage);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "323", _bs.Resistance);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "239", _bs.Temperature);
                    OutputParamItems.Add(oneitem);
                }

                if (_bs.FinalResult == true)
                {
                    if (checkType == 0)//1,3角最小值
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", Math.Min(_bs.Corner1.InspectResults.resultDataMin.fMinDis, _bs.Corner3.InspectResults.resultDataMin.fMinDis));
                    }
                    else if (checkType == 1)//2,4角最小值
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", Math.Min(_bs.Corner2.InspectResults.resultDataMin.fMinDis, _bs.Corner4.InspectResults.resultDataMin.fMinDis));
                    }
                    else//全角
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", Math.Min(Math.Min(_bs.Corner1.InspectResults.resultDataMin.fMinDis, _bs.Corner3.InspectResults.resultDataMin.fMinDis), Math.Min(_bs.Corner2.InspectResults.resultDataMin.fMinDis, _bs.Corner4.InspectResults.resultDataMin.fMinDis)));
                    }

                }
                else
                {
                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "99.99");
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "-1");
                }
                OutputParamItems.Add(oneitem);

                return OutputParamItems;
            }
            return OutputParamItems;
        }

        public static List<string> BatterySeatToBatteryTestDataByName(BatterySeat _bs, int checkType, int layerAC, int layerBD, string name, bool isRecheck = false)
        {
            List<string> OutputParamItems = new List<string>();

            string oneitem = "";

            if (isRecheck == true)
            {
                if (checkType == 0 || checkType == 2)
                {
                    int paramID = 52166;
                    int paramID2 = 85144;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }


                    paramID = 52136;
                    paramID2 = 85092;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }
                }

                if (checkType == 1 || checkType == 2)
                {
                    int paramID = 52151;
                    int paramID2 = 85119;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }

                    paramID = 52181;
                    paramID2 = 85169;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }
                }


                //Input参数
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51740", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51741", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51011", XRayTubeIF.XRayTube1Stauts.ActualVoltage.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51012", XRayTubeIF.XRayTube1Stauts.ActualCurrent.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51856", _bs.ResultImgFileName);
                OutputParamItems.Add(oneitem);

                if (_bs.FinalResult == true)
                {
                    if (checkType == 0 || checkType == 2)
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", _bs.Corner1.InspectParams.max_length);
                    }
                    else
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", _bs.Corner2.InspectParams.max_length);
                    }

                }
                else
                {
                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "99.99");
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "-1");
                }

                OutputParamItems.Add(oneitem);
                return OutputParamItems;
            }

            if (name == "OCV")
            {
                //OCV
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "241", Math.Round(_bs.Voltage, 3));
                OutputParamItems.Add(oneitem);
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "323", Math.Round(_bs.Resistance, 3));
                OutputParamItems.Add(oneitem);
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "239", Math.Round(_bs.Temperature, 3));
                OutputParamItems.Add(oneitem);
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "2474", Math.Round(_bs.EnvirementTemperature, 3));
                OutputParamItems.Add(oneitem);

                if (plc.ReadByVariableName("NOIV") == "0")
                {
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "2146", Math.Round(_bs.IvData, 3));
                    OutputParamItems.Add(oneitem);
                }
                return OutputParamItems;
            }
            else if (name == "FQI")
            {
                string pass = _bs.DimensionResult ? "OK" : "NG";
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "300739", pass);
                OutputParamItems.Add(oneitem);

                if (plc.ReadByVariableName("NOMDI") == "0")
                {
                    //CCD
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "48586", _bs.AllBatLength);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "3303", _bs.BatWidth);
                    OutputParamItems.Add(oneitem);
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "48585", _bs.BatLength);
                    OutputParamItems.Add(oneitem);
                    
                    if (CheckParamsConfig.Instance.IsAlOnLeft)
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78280", Math.Round((_bs.Left1WhiteGlue + _bs.Left2WhiteGlue) / 2, 3));
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78279", Math.Round((_bs.Right1WhiteGlue + _bs.Right2WhiteGlue) / 2, 3));
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52753", _bs.LeftLugMargin);
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52754", _bs.RightLugMargin);
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "3304", _bs.LeftLugLength);
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52886", _bs.RightLugLength);
                        OutputParamItems.Add(oneitem);
                    }
                    else
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78280", Math.Round((_bs.Right1WhiteGlue + _bs.Right2WhiteGlue) / 2, 3));
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "78279", Math.Round((_bs.Left1WhiteGlue + _bs.Left2WhiteGlue) / 2, 3));
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52753", _bs.RightLugMargin);
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52754", _bs.LeftLugMargin);
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "3304", _bs.RightLugLength);
                        OutputParamItems.Add(oneitem);
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52886", _bs.LeftLugLength);
                        OutputParamItems.Add(oneitem);
                    }
                }

                if (plc.ReadByVariableName("NOPPG") == "0")
                {
                    //PPG
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "3301", _bs.Thickness);
                    OutputParamItems.Add(oneitem);
                }
                return OutputParamItems;
            }
            else if (name == "XRAY")
            {
                if (checkType == 0 || checkType == 2)
                {
                    int paramID = 52166;
                    int paramID2 = 85144;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner3.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }
                    
                    paramID = 52136;
                    paramID2 = 85092;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner1.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }

                    }
                }

                if (checkType == 1 || checkType == 2)
                {
                    int paramID = 52151;
                    int paramID2 = 85119;
                    int n = 0;
                    for (int i = 0; i < layerAC; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner2.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }

                    paramID = 52181;
                    paramID2 = 85169;
                    n = 0;
                    for (int i = 0; i < layerBD; i++)
                    {
                        if (i <= 14)
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID + i, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                        }
                        else
                        {
                            oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", paramID2 + n, _bs.Corner4.InspectResults.vecDis[i].ToString());
                            OutputParamItems.Add(oneitem);
                            n++;
                        }
                    }

                    //int checkSides = CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2 ? 2 : 4;
                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "48083", checkSides);
                    //OutputParamItems.Add(oneitem);

                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52196", layerAC);
                    //OutputParamItems.Add(oneitem);

                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "52197", layerBD);
                    //OutputParamItems.Add(oneitem);
                }


                //Input参数
                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51740", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51741", XRayTubeIF.XRayTube1Stauts.UseHoursInTotal.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51011", XRayTubeIF.XRayTube1Stauts.ActualVoltage.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51012", XRayTubeIF.XRayTube1Stauts.ActualCurrent.ToString());
                OutputParamItems.Add(oneitem);

                oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "51856", _bs.ResultImgFileName);
                OutputParamItems.Add(oneitem);

                if (_bs.FinalResult == true)
                {
                    if (checkType == 0 || checkType == 2)
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", _bs.Corner1.InspectResults.resultDataMin.fMinDis);
                    }
                    else
                    {
                        oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", _bs.Corner2.InspectResults.resultDataMin.fMinDis);
                    }
                }
                else
                {
                    //oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "99.99");
                    oneitem = string.Format("{{\"ParamID\":\"{0}\",\"ParamValue\":\"{1}\"}}", "251", "-1");
                }
                OutputParamItems.Add(oneitem);

                return OutputParamItems;
            }
            return OutputParamItems;
        }

        public static BatteryCheck BatterySeatToBatteryCheck(BatterySeat batterySeat)
        {
            BatteryCheck batteryCheck = new BatteryCheck();

            batteryCheck.ProductSN = batterySeat.Sn;
            batteryCheck.Model = "11111111";
            batteryCheck.Quality = Convert.ToInt16(batterySeat.FinalResult);
            batteryCheck.NGreason = batterySeat.ResultCode.ToString();

            batteryCheck.ProductDate = DateTime.Now;
            batteryCheck.OutTime = Convert.ToDateTime(batterySeat.EndTime);
            batteryCheck.Operator = "00000000";

            batteryCheck.ResultPath = batterySeat.ResultImgSavePath + "\\" + batterySeat.ResultImgFileName;
            batteryCheck.MesImagePath = batterySeat.StfImgSavePath;
            batteryCheck.Thickness = batterySeat.Thickness;

            if (batterySeat.CheckMode == ECheckModes.Diagonal_1_2)
            {
                batteryCheck.CheckMode = 0;
            }
            else if (batterySeat.CheckMode == ECheckModes.Diagonal_3_4)
            {
                batteryCheck.CheckMode = 1;
            }
            else
            {
                batteryCheck.CheckMode = 2;
            }

            batteryCheck.A1_PhotographResult = batterySeat.Corner1.IsShotOK ? 1 : 0;
            batteryCheck.A1_OriginalImagePath = batterySeat.OrigImgSavePath + "\\" + batterySeat.Corner1.OrigImageFileName;
            batteryCheck.A1_ResultImagePath = batterySeat.ResultImgSavePath + "\\" + batterySeat.Corner1.ResultImgFileName;
            batteryCheck.A1_Min = batterySeat.Corner1.InspectResults.resultDataMin.fMinDis;
            batteryCheck.A1_Max = batterySeat.Corner1.InspectResults.resultDataMin.fMaxDis;
            batteryCheck.A1_Distance1 = batterySeat.Corner1.InspectResults.vecDis[0];
            batteryCheck.A1_Angle1 = batterySeat.Corner1.InspectResults.vecAngles[0];
            batteryCheck.A1_Distance2 = batterySeat.Corner1.InspectResults.vecDis[1];
            batteryCheck.A1_Angle2 = batterySeat.Corner1.InspectResults.vecAngles[1];
            batteryCheck.A1_Distance3 = batterySeat.Corner1.InspectResults.vecDis[2];
            batteryCheck.A1_Angle3 = batterySeat.Corner1.InspectResults.vecAngles[2];
            batteryCheck.A1_Distance4 = batterySeat.Corner1.InspectResults.vecDis[3];
            batteryCheck.A1_Angle4 = batterySeat.Corner1.InspectResults.vecAngles[3];
            batteryCheck.A1_Distance5 = batterySeat.Corner1.InspectResults.vecDis[4];
            batteryCheck.A1_Angle5 = batterySeat.Corner1.InspectResults.vecAngles[4];
            batteryCheck.A1_Distance6 = batterySeat.Corner1.InspectResults.vecDis[5];
            batteryCheck.A1_Angle6 = batterySeat.Corner1.InspectResults.vecAngles[5];
            batteryCheck.A1_Distance7 = batterySeat.Corner1.InspectResults.vecDis[6];
            batteryCheck.A1_Angle7 = batterySeat.Corner1.InspectResults.vecAngles[6];
            batteryCheck.A1_Distance8 = batterySeat.Corner1.InspectResults.vecDis[7];
            batteryCheck.A1_Angle8 = batterySeat.Corner1.InspectResults.vecAngles[7];
            batteryCheck.A1_Distance9 = batterySeat.Corner1.InspectResults.vecDis[8];
            batteryCheck.A1_Angle9 = batterySeat.Corner1.InspectResults.vecAngles[8];
            batteryCheck.A1_Distance10 = batterySeat.Corner1.InspectResults.vecDis[9];
            batteryCheck.A1_Angle10 = batterySeat.Corner1.InspectResults.vecAngles[9];
            batteryCheck.A1_Distance11 = batterySeat.Corner1.InspectResults.vecDis[10];
            batteryCheck.A1_Angle11 = batterySeat.Corner1.InspectResults.vecAngles[10];
            batteryCheck.A1_Distance12 = batterySeat.Corner1.InspectResults.vecDis[11];
            batteryCheck.A1_Angle12 = batterySeat.Corner1.InspectResults.vecAngles[11];
            batteryCheck.A1_Distance13 = batterySeat.Corner1.InspectResults.vecDis[12];
            batteryCheck.A1_Angle13 = batterySeat.Corner1.InspectResults.vecAngles[12];
            batteryCheck.A1_Distance14 = batterySeat.Corner1.InspectResults.vecDis[13];
            batteryCheck.A1_Angle14 = batterySeat.Corner1.InspectResults.vecAngles[13];
            batteryCheck.A1_Distance15 = batterySeat.Corner1.InspectResults.vecDis[14];
            batteryCheck.A1_Angle15 = batterySeat.Corner1.InspectResults.vecAngles[14];
            batteryCheck.A1_Distance16 = batterySeat.Corner1.InspectResults.vecDis[15];
            batteryCheck.A1_Angle16 = batterySeat.Corner1.InspectResults.vecAngles[15];
            batteryCheck.A1_Distance17 = batterySeat.Corner1.InspectResults.vecDis[16];
            batteryCheck.A1_Angle17 = batterySeat.Corner1.InspectResults.vecAngles[16];
            batteryCheck.A1_Distance18 = batterySeat.Corner1.InspectResults.vecDis[17];
            batteryCheck.A1_Angle18 = batterySeat.Corner1.InspectResults.vecAngles[17];
            batteryCheck.A1_Distance19 = batterySeat.Corner1.InspectResults.vecDis[18];
            batteryCheck.A1_Angle19 = batterySeat.Corner1.InspectResults.vecAngles[18];
            batteryCheck.A1_Distance20 = batterySeat.Corner1.InspectResults.vecDis[19];
            batteryCheck.A1_Angle20 = batterySeat.Corner1.InspectResults.vecAngles[19];
            batteryCheck.A1_Distance21 = batterySeat.Corner1.InspectResults.vecDis[20];
            batteryCheck.A1_Angle21 = batterySeat.Corner1.InspectResults.vecAngles[20];
            batteryCheck.A1_Distance22 = batterySeat.Corner1.InspectResults.vecDis[21];
            batteryCheck.A1_Angle22 = batterySeat.Corner1.InspectResults.vecAngles[21];
            batteryCheck.A1_Distance23 = batterySeat.Corner1.InspectResults.vecDis[22];
            batteryCheck.A1_Angle23 = batterySeat.Corner1.InspectResults.vecAngles[22];
            batteryCheck.A1_Distance24 = batterySeat.Corner1.InspectResults.vecDis[23];
            batteryCheck.A1_Angle24 = batterySeat.Corner1.InspectResults.vecAngles[23];
            batteryCheck.A1_Distance25 = batterySeat.Corner1.InspectResults.vecDis[24];
            batteryCheck.A1_Angle25 = batterySeat.Corner1.InspectResults.vecAngles[24];
            batteryCheck.A1_Distance26 = batterySeat.Corner1.InspectResults.vecDis[25];
            batteryCheck.A1_Angle26 = batterySeat.Corner1.InspectResults.vecAngles[25];
            batteryCheck.A1_Distance27 = batterySeat.Corner1.InspectResults.vecDis[26];
            batteryCheck.A1_Angle27 = batterySeat.Corner1.InspectResults.vecAngles[26];
            batteryCheck.A1_Distance28 = batterySeat.Corner1.InspectResults.vecDis[27];
            batteryCheck.A1_Angle28 = batterySeat.Corner1.InspectResults.vecAngles[27];
            batteryCheck.A1_Distance29 = batterySeat.Corner1.InspectResults.vecDis[28];
            batteryCheck.A1_Angle29 = batterySeat.Corner1.InspectResults.vecAngles[28];
            batteryCheck.A1_Distance30 = batterySeat.Corner1.InspectResults.vecDis[29];
            batteryCheck.A1_Angle30 = batterySeat.Corner1.InspectResults.vecAngles[29];

            batteryCheck.A2_PhotographResult = batterySeat.Corner2.IsShotOK ? 1 : 0;
            batteryCheck.A2_OriginalImagePath = batterySeat.OrigImgSavePath + "\\" + batterySeat.Corner2.OrigImageFileName;
            batteryCheck.A2_ResultImagePath = batterySeat.ResultImgSavePath + "\\" + batterySeat.Corner2.ResultImgFileName;
            batteryCheck.A2_Min = batterySeat.Corner2.InspectResults.resultDataMin.fMinDis;
            batteryCheck.A2_Max = batterySeat.Corner2.InspectResults.resultDataMin.fMaxDis;
            batteryCheck.A2_Distance1 = batterySeat.Corner2.InspectResults.vecDis[0];
            batteryCheck.A2_Angle1 = batterySeat.Corner2.InspectResults.vecAngles[0];
            batteryCheck.A2_Distance2 = batterySeat.Corner2.InspectResults.vecDis[1];
            batteryCheck.A2_Angle2 = batterySeat.Corner2.InspectResults.vecAngles[1];
            batteryCheck.A2_Distance3 = batterySeat.Corner2.InspectResults.vecDis[2];
            batteryCheck.A2_Angle3 = batterySeat.Corner2.InspectResults.vecAngles[2];
            batteryCheck.A2_Distance4 = batterySeat.Corner2.InspectResults.vecDis[3];
            batteryCheck.A2_Angle4 = batterySeat.Corner2.InspectResults.vecAngles[3];
            batteryCheck.A2_Distance5 = batterySeat.Corner2.InspectResults.vecDis[4];
            batteryCheck.A2_Angle5 = batterySeat.Corner2.InspectResults.vecAngles[4];
            batteryCheck.A2_Distance6 = batterySeat.Corner2.InspectResults.vecDis[5];
            batteryCheck.A2_Angle6 = batterySeat.Corner2.InspectResults.vecAngles[5];
            batteryCheck.A2_Distance7 = batterySeat.Corner2.InspectResults.vecDis[6];
            batteryCheck.A2_Angle7 = batterySeat.Corner2.InspectResults.vecAngles[6];
            batteryCheck.A2_Distance8 = batterySeat.Corner2.InspectResults.vecDis[7];
            batteryCheck.A2_Angle8 = batterySeat.Corner2.InspectResults.vecAngles[7];
            batteryCheck.A2_Distance9 = batterySeat.Corner2.InspectResults.vecDis[8];
            batteryCheck.A2_Angle9 = batterySeat.Corner2.InspectResults.vecAngles[8];
            batteryCheck.A2_Distance10 = batterySeat.Corner2.InspectResults.vecDis[9];
            batteryCheck.A2_Angle10 = batterySeat.Corner2.InspectResults.vecAngles[9];
            batteryCheck.A2_Distance11 = batterySeat.Corner2.InspectResults.vecDis[10];
            batteryCheck.A2_Angle11 = batterySeat.Corner2.InspectResults.vecAngles[10];
            batteryCheck.A2_Distance12 = batterySeat.Corner2.InspectResults.vecDis[11];
            batteryCheck.A2_Angle12 = batterySeat.Corner2.InspectResults.vecAngles[11];
            batteryCheck.A2_Distance13 = batterySeat.Corner2.InspectResults.vecDis[12];
            batteryCheck.A2_Angle13 = batterySeat.Corner2.InspectResults.vecAngles[12];
            batteryCheck.A2_Distance14 = batterySeat.Corner2.InspectResults.vecDis[13];
            batteryCheck.A2_Angle14 = batterySeat.Corner2.InspectResults.vecAngles[13];
            batteryCheck.A2_Distance15 = batterySeat.Corner2.InspectResults.vecDis[14];
            batteryCheck.A2_Angle15 = batterySeat.Corner2.InspectResults.vecAngles[14];
            batteryCheck.A2_Distance16 = batterySeat.Corner2.InspectResults.vecDis[15];
            batteryCheck.A2_Angle16 = batterySeat.Corner2.InspectResults.vecAngles[15];
            batteryCheck.A2_Distance17 = batterySeat.Corner2.InspectResults.vecDis[16];
            batteryCheck.A2_Angle17 = batterySeat.Corner2.InspectResults.vecAngles[16];
            batteryCheck.A2_Distance18 = batterySeat.Corner2.InspectResults.vecDis[17];
            batteryCheck.A2_Angle18 = batterySeat.Corner2.InspectResults.vecAngles[17];
            batteryCheck.A2_Distance19 = batterySeat.Corner2.InspectResults.vecDis[18];
            batteryCheck.A2_Angle19 = batterySeat.Corner2.InspectResults.vecAngles[18];
            batteryCheck.A2_Distance20 = batterySeat.Corner2.InspectResults.vecDis[19];
            batteryCheck.A2_Angle20 = batterySeat.Corner2.InspectResults.vecAngles[19];
            batteryCheck.A2_Distance21 = batterySeat.Corner2.InspectResults.vecDis[20];
            batteryCheck.A2_Angle21 = batterySeat.Corner2.InspectResults.vecAngles[20];
            batteryCheck.A2_Distance22 = batterySeat.Corner2.InspectResults.vecDis[21];
            batteryCheck.A2_Angle22 = batterySeat.Corner2.InspectResults.vecAngles[21];
            batteryCheck.A2_Distance23 = batterySeat.Corner2.InspectResults.vecDis[22];
            batteryCheck.A2_Angle23 = batterySeat.Corner2.InspectResults.vecAngles[22];
            batteryCheck.A2_Distance24 = batterySeat.Corner2.InspectResults.vecDis[23];
            batteryCheck.A2_Angle24 = batterySeat.Corner2.InspectResults.vecAngles[23];
            batteryCheck.A2_Distance25 = batterySeat.Corner2.InspectResults.vecDis[24];
            batteryCheck.A2_Angle25 = batterySeat.Corner2.InspectResults.vecAngles[24];
            batteryCheck.A2_Distance26 = batterySeat.Corner2.InspectResults.vecDis[25];
            batteryCheck.A2_Angle26 = batterySeat.Corner2.InspectResults.vecAngles[25];
            batteryCheck.A2_Distance27 = batterySeat.Corner2.InspectResults.vecDis[26];
            batteryCheck.A2_Angle27 = batterySeat.Corner2.InspectResults.vecAngles[26];
            batteryCheck.A2_Distance28 = batterySeat.Corner2.InspectResults.vecDis[27];
            batteryCheck.A2_Angle28 = batterySeat.Corner2.InspectResults.vecAngles[27];
            batteryCheck.A2_Distance29 = batterySeat.Corner2.InspectResults.vecDis[28];
            batteryCheck.A2_Angle29 = batterySeat.Corner2.InspectResults.vecAngles[28];
            batteryCheck.A2_Distance30 = batterySeat.Corner2.InspectResults.vecDis[29];
            batteryCheck.A2_Angle30 = batterySeat.Corner2.InspectResults.vecAngles[29];

            batteryCheck.A3_PhotographResult = batterySeat.Corner3.IsShotOK ? 1 : 0;
            batteryCheck.A3_OriginalImagePath = batterySeat.OrigImgSavePath + "\\" + batterySeat.Corner3.OrigImageFileName;
            batteryCheck.A3_ResultImagePath = batterySeat.ResultImgSavePath + "\\" + batterySeat.Corner3.ResultImgFileName;
            batteryCheck.A3_Min = batterySeat.Corner3.InspectResults.resultDataMin.fMinDis;
            batteryCheck.A3_Max = batterySeat.Corner3.InspectResults.resultDataMin.fMaxDis;
            batteryCheck.A3_Distance1 = batterySeat.Corner3.InspectResults.vecDis[0];
            batteryCheck.A3_Angle1 = batterySeat.Corner3.InspectResults.vecAngles[0];
            batteryCheck.A3_Distance2 = batterySeat.Corner3.InspectResults.vecDis[1];
            batteryCheck.A3_Angle2 = batterySeat.Corner3.InspectResults.vecAngles[1];
            batteryCheck.A3_Distance3 = batterySeat.Corner3.InspectResults.vecDis[2];
            batteryCheck.A3_Angle3 = batterySeat.Corner3.InspectResults.vecAngles[2];
            batteryCheck.A3_Distance4 = batterySeat.Corner3.InspectResults.vecDis[3];
            batteryCheck.A3_Angle4 = batterySeat.Corner3.InspectResults.vecAngles[3];
            batteryCheck.A3_Distance5 = batterySeat.Corner3.InspectResults.vecDis[4];
            batteryCheck.A3_Angle5 = batterySeat.Corner3.InspectResults.vecAngles[4];
            batteryCheck.A3_Distance6 = batterySeat.Corner3.InspectResults.vecDis[5];
            batteryCheck.A3_Angle6 = batterySeat.Corner3.InspectResults.vecAngles[5];
            batteryCheck.A3_Distance7 = batterySeat.Corner3.InspectResults.vecDis[6];
            batteryCheck.A3_Angle7 = batterySeat.Corner3.InspectResults.vecAngles[6];
            batteryCheck.A3_Distance8 = batterySeat.Corner3.InspectResults.vecDis[7];
            batteryCheck.A3_Angle8 = batterySeat.Corner3.InspectResults.vecAngles[7];
            batteryCheck.A3_Distance9 = batterySeat.Corner3.InspectResults.vecDis[8];
            batteryCheck.A3_Angle9 = batterySeat.Corner3.InspectResults.vecAngles[8];
            batteryCheck.A3_Distance10 = batterySeat.Corner3.InspectResults.vecDis[9];
            batteryCheck.A3_Angle10 = batterySeat.Corner3.InspectResults.vecAngles[9];
            batteryCheck.A3_Distance11 = batterySeat.Corner3.InspectResults.vecDis[10];
            batteryCheck.A3_Angle11 = batterySeat.Corner3.InspectResults.vecAngles[10];
            batteryCheck.A3_Distance12 = batterySeat.Corner3.InspectResults.vecDis[11];
            batteryCheck.A3_Angle12 = batterySeat.Corner3.InspectResults.vecAngles[11];
            batteryCheck.A3_Distance13 = batterySeat.Corner3.InspectResults.vecDis[12];
            batteryCheck.A3_Angle13 = batterySeat.Corner3.InspectResults.vecAngles[12];
            batteryCheck.A3_Distance14 = batterySeat.Corner3.InspectResults.vecDis[13];
            batteryCheck.A3_Angle14 = batterySeat.Corner3.InspectResults.vecAngles[13];
            batteryCheck.A3_Distance15 = batterySeat.Corner3.InspectResults.vecDis[14];
            batteryCheck.A3_Angle15 = batterySeat.Corner3.InspectResults.vecAngles[14];
            batteryCheck.A3_Distance16 = batterySeat.Corner3.InspectResults.vecDis[15];
            batteryCheck.A3_Angle16 = batterySeat.Corner3.InspectResults.vecAngles[15];
            batteryCheck.A3_Distance17 = batterySeat.Corner3.InspectResults.vecDis[16];
            batteryCheck.A3_Angle17 = batterySeat.Corner3.InspectResults.vecAngles[16];
            batteryCheck.A3_Distance18 = batterySeat.Corner3.InspectResults.vecDis[17];
            batteryCheck.A3_Angle18 = batterySeat.Corner3.InspectResults.vecAngles[17];
            batteryCheck.A3_Distance19 = batterySeat.Corner3.InspectResults.vecDis[18];
            batteryCheck.A3_Angle19 = batterySeat.Corner3.InspectResults.vecAngles[18];
            batteryCheck.A3_Distance20 = batterySeat.Corner3.InspectResults.vecDis[19];
            batteryCheck.A3_Angle20 = batterySeat.Corner3.InspectResults.vecAngles[19];
            batteryCheck.A3_Distance21 = batterySeat.Corner3.InspectResults.vecDis[20];
            batteryCheck.A3_Angle21 = batterySeat.Corner3.InspectResults.vecAngles[20];
            batteryCheck.A3_Distance22 = batterySeat.Corner3.InspectResults.vecDis[21];
            batteryCheck.A3_Angle22 = batterySeat.Corner3.InspectResults.vecAngles[21];
            batteryCheck.A3_Distance23 = batterySeat.Corner3.InspectResults.vecDis[22];
            batteryCheck.A3_Angle23 = batterySeat.Corner3.InspectResults.vecAngles[22];
            batteryCheck.A3_Distance24 = batterySeat.Corner3.InspectResults.vecDis[23];
            batteryCheck.A3_Angle24 = batterySeat.Corner3.InspectResults.vecAngles[23];
            batteryCheck.A3_Distance25 = batterySeat.Corner3.InspectResults.vecDis[24];
            batteryCheck.A3_Angle25 = batterySeat.Corner3.InspectResults.vecAngles[24];
            batteryCheck.A3_Distance26 = batterySeat.Corner3.InspectResults.vecDis[25];
            batteryCheck.A3_Angle26 = batterySeat.Corner3.InspectResults.vecAngles[25];
            batteryCheck.A3_Distance27 = batterySeat.Corner3.InspectResults.vecDis[26];
            batteryCheck.A3_Angle27 = batterySeat.Corner3.InspectResults.vecAngles[26];
            batteryCheck.A3_Distance28 = batterySeat.Corner3.InspectResults.vecDis[27];
            batteryCheck.A3_Angle28 = batterySeat.Corner3.InspectResults.vecAngles[27];
            batteryCheck.A3_Distance29 = batterySeat.Corner3.InspectResults.vecDis[28];
            batteryCheck.A3_Angle29 = batterySeat.Corner3.InspectResults.vecAngles[28];
            batteryCheck.A3_Distance30 = batterySeat.Corner3.InspectResults.vecDis[29];
            batteryCheck.A3_Angle30 = batterySeat.Corner3.InspectResults.vecAngles[29];

            batteryCheck.A4_PhotographResult = batterySeat.Corner4.IsShotOK ? 1 : 0;
            batteryCheck.A4_OriginalImagePath = batterySeat.OrigImgSavePath + "\\" + batterySeat.Corner4.OrigImageFileName;
            batteryCheck.A4_ResultImagePath = batterySeat.ResultImgSavePath + "\\" + batterySeat.Corner4.ResultImgFileName;
            batteryCheck.A4_Min = batterySeat.Corner4.InspectResults.resultDataMin.fMinDis;
            batteryCheck.A4_Max = batterySeat.Corner4.InspectResults.resultDataMin.fMaxDis;
            batteryCheck.A4_Distance1 = batterySeat.Corner4.InspectResults.vecDis[0];
            batteryCheck.A4_Angle1 = batterySeat.Corner4.InspectResults.vecAngles[0];
            batteryCheck.A4_Distance2 = batterySeat.Corner4.InspectResults.vecDis[1];
            batteryCheck.A4_Angle2 = batterySeat.Corner4.InspectResults.vecAngles[1];
            batteryCheck.A4_Distance3 = batterySeat.Corner4.InspectResults.vecDis[2];
            batteryCheck.A4_Angle3 = batterySeat.Corner4.InspectResults.vecAngles[2];
            batteryCheck.A4_Distance4 = batterySeat.Corner4.InspectResults.vecDis[3];
            batteryCheck.A4_Angle4 = batterySeat.Corner4.InspectResults.vecAngles[3];
            batteryCheck.A4_Distance5 = batterySeat.Corner4.InspectResults.vecDis[4];
            batteryCheck.A4_Angle5 = batterySeat.Corner4.InspectResults.vecAngles[4];
            batteryCheck.A4_Distance6 = batterySeat.Corner4.InspectResults.vecDis[5];
            batteryCheck.A4_Angle6 = batterySeat.Corner4.InspectResults.vecAngles[5];
            batteryCheck.A4_Distance7 = batterySeat.Corner4.InspectResults.vecDis[6];
            batteryCheck.A4_Angle7 = batterySeat.Corner4.InspectResults.vecAngles[6];
            batteryCheck.A4_Distance8 = batterySeat.Corner4.InspectResults.vecDis[7];
            batteryCheck.A4_Angle8 = batterySeat.Corner4.InspectResults.vecAngles[7];
            batteryCheck.A4_Distance9 = batterySeat.Corner4.InspectResults.vecDis[8];
            batteryCheck.A4_Angle9 = batterySeat.Corner4.InspectResults.vecAngles[8];
            batteryCheck.A4_Distance10 = batterySeat.Corner4.InspectResults.vecDis[9];
            batteryCheck.A4_Angle10 = batterySeat.Corner4.InspectResults.vecAngles[9];
            batteryCheck.A4_Distance11 = batterySeat.Corner4.InspectResults.vecDis[10];
            batteryCheck.A4_Angle11 = batterySeat.Corner4.InspectResults.vecAngles[10];
            batteryCheck.A4_Distance12 = batterySeat.Corner4.InspectResults.vecDis[11];
            batteryCheck.A4_Angle12 = batterySeat.Corner4.InspectResults.vecAngles[11];
            batteryCheck.A4_Distance13 = batterySeat.Corner4.InspectResults.vecDis[12];
            batteryCheck.A4_Angle13 = batterySeat.Corner4.InspectResults.vecAngles[12];
            batteryCheck.A4_Distance14 = batterySeat.Corner4.InspectResults.vecDis[13];
            batteryCheck.A4_Angle14 = batterySeat.Corner4.InspectResults.vecAngles[13];
            batteryCheck.A4_Distance15 = batterySeat.Corner4.InspectResults.vecDis[14];
            batteryCheck.A4_Angle15 = batterySeat.Corner4.InspectResults.vecAngles[14];
            batteryCheck.A4_Distance16 = batterySeat.Corner4.InspectResults.vecDis[15];
            batteryCheck.A4_Angle16 = batterySeat.Corner4.InspectResults.vecAngles[15];
            batteryCheck.A4_Distance17 = batterySeat.Corner4.InspectResults.vecDis[16];
            batteryCheck.A4_Angle17 = batterySeat.Corner4.InspectResults.vecAngles[16];
            batteryCheck.A4_Distance18 = batterySeat.Corner4.InspectResults.vecDis[17];
            batteryCheck.A4_Angle18 = batterySeat.Corner4.InspectResults.vecAngles[17];
            batteryCheck.A4_Distance19 = batterySeat.Corner4.InspectResults.vecDis[18];
            batteryCheck.A4_Angle19 = batterySeat.Corner4.InspectResults.vecAngles[18];
            batteryCheck.A4_Distance20 = batterySeat.Corner4.InspectResults.vecDis[19];
            batteryCheck.A4_Angle20 = batterySeat.Corner4.InspectResults.vecAngles[19];
            batteryCheck.A4_Distance21 = batterySeat.Corner4.InspectResults.vecDis[20];
            batteryCheck.A4_Angle21 = batterySeat.Corner4.InspectResults.vecAngles[20];
            batteryCheck.A4_Distance22 = batterySeat.Corner4.InspectResults.vecDis[21];
            batteryCheck.A4_Angle22 = batterySeat.Corner4.InspectResults.vecAngles[21];
            batteryCheck.A4_Distance23 = batterySeat.Corner4.InspectResults.vecDis[22];
            batteryCheck.A4_Angle23 = batterySeat.Corner4.InspectResults.vecAngles[22];
            batteryCheck.A4_Distance24 = batterySeat.Corner4.InspectResults.vecDis[23];
            batteryCheck.A4_Angle24 = batterySeat.Corner4.InspectResults.vecAngles[23];
            batteryCheck.A4_Distance25 = batterySeat.Corner4.InspectResults.vecDis[24];
            batteryCheck.A4_Angle25 = batterySeat.Corner4.InspectResults.vecAngles[24];
            batteryCheck.A4_Distance26 = batterySeat.Corner4.InspectResults.vecDis[25];
            batteryCheck.A4_Angle26 = batterySeat.Corner4.InspectResults.vecAngles[25];
            batteryCheck.A4_Distance27 = batterySeat.Corner4.InspectResults.vecDis[26];
            batteryCheck.A4_Angle27 = batterySeat.Corner4.InspectResults.vecAngles[26];
            batteryCheck.A4_Distance28 = batterySeat.Corner4.InspectResults.vecDis[27];
            batteryCheck.A4_Angle28 = batterySeat.Corner4.InspectResults.vecAngles[27];
            batteryCheck.A4_Distance29 = batterySeat.Corner4.InspectResults.vecDis[28];
            batteryCheck.A4_Angle29 = batterySeat.Corner4.InspectResults.vecAngles[28];
            batteryCheck.A4_Distance30 = batterySeat.Corner4.InspectResults.vecDis[29];
            batteryCheck.A4_Angle30 = batterySeat.Corner4.InspectResults.vecAngles[29];


            return batteryCheck;
        }
    }
}
