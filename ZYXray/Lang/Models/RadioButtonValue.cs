using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using ATL.Core;

namespace ZYXray.Models
{
    class RadioButtonValue : ObservableObject
    {
        private static RadioButtonValue _instance = new RadioButtonValue();

        public static RadioButtonValue Instance
        {
            get { return _instance; }
        }

        private int _inspectType;
        public int InspectType
        {
            get
            {
                _inspectType = int.Parse(UserDefineVariableInfo.DicVariables["m_iInspectType"].ToString());
                return _inspectType;
            }
            set
            {
                _inspectType = value;
                UserDefineVariableInfo.DicVariables["m_iInspectType"] = _inspectType;
                RaisePropertyChanged("InspectType");
            }
        }
    }
}
