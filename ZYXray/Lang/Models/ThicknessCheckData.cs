using Shuyz.Framework.Mvvm;
using System;

namespace ZYXray.Models
{
    public class ThicknessCheckData : ObservableObject
    {
        private int _index;
        public int index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged("index");
            }
        }
        private string _module;
        public string module
        {
            get { return _module; }
            set
            {
                _module = value;
                RaisePropertyChanged("module");
            }
        }
        private DateTime _time;
        public DateTime time
        {
            get { return _time; }
            set
            {
                _time = value;
                RaisePropertyChanged("time");
            }
        }
        private string _model;
        public string model
        {
            get { return _model; }
            set
            {
                _model = value;
                RaisePropertyChanged("model");
            }
        }
        private double _value;
        public double value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("value");
            }
        }
        private string _result;
        public string result
        {
            get { return _result; }
            set
            {
                _result = value;
                RaisePropertyChanged("result");
            }
        }
    }
}
