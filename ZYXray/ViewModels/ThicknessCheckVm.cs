using PTF.Models;
using PTF.Utils;
using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XRayClient.Core;

namespace PTF.ViewModels
{
    public class ThicknessCheckVm : ObservableObject
    {
        private static List<ThicknessCheckData> thickData = new List<ThicknessCheckData>();

        public IEnumerable<ECalibrationBlockModel> BindableBlockModels
        {
            get
            {
                return Enum.GetValues(typeof(ECalibrationBlockModel))
                    .Cast<ECalibrationBlockModel>();
            }
        }

        public ECalibrationBlockModel BlockModel
        {
            get
            {
                return (ECalibrationBlockModel)CheckParamsConfig.Instance.ThicknessCalibrationMode;
            }
            set
            {
                CheckParamsConfig.Instance.ThicknessCalibrationMode = (int)value;

                RaisePropertyChanged("BlockModel");
            }
        }

        public List<ThicknessCheckData> ThickData
        {
            get
            {
                return thickData;
            }
            set
            {
                thickData = value;

                RaisePropertyChanged("ThickData");
            }
        }

        public ThicknessCheckVm()
        {

            for (int i = 0; i < 2; i++)
            {
                thickData.Add(new ThicknessCheckData { index = 1, time = DateTime.Now, model = "small", value = (float)0.5, result = "OK" });
            }
        }


        public ICommand StartCheck
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    ///

                    for (int i = 0; i < 2; i++)
                    {
                        thickData.Add(new ThicknessCheckData { index = 1, time = DateTime.Now, model = "small", value = (float)0.5, result = "OK" });
                    }

                    //thickData = new List<ThicknessCheckData>(new ThicknessCheckData { });

                    RaisePropertyChanged("ThickData");

                }), delegate
                {
                    return true;
                });
            }
        }

    }
}
