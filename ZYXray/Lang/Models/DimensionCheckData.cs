using Shuyz.Framework.Mvvm;
using System;


namespace ZYXray.Models
{
    public class DimensionCheckData : ObservableObject
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

        private float _batLength;
        public float batLength
        {
            get { return _batLength; }
            set
            {
                _batLength = value;
                RaisePropertyChanged("batLength");
            }
        }

        private float _batWidth;
        public float batWidth
        {
            get { return _batWidth; }
            set
            {
                _batWidth = value;
                RaisePropertyChanged("batWidth");
            }
        }

        private float _leftLug;
        public float leftLug
        {
            get { return _leftLug; }
            set
            {
                _leftLug = value;
                RaisePropertyChanged("leftLug");
            }
        }

        private float _rightLug;
        public float rightLug
        {
            get { return _rightLug; }
            set
            {
                _rightLug = value;
                RaisePropertyChanged("rightLug");
            }
        }

        private float _allBatLength;
        public float allBatLength
        {
            get { return _allBatLength; }
            set
            {
                _allBatLength = value;
                RaisePropertyChanged("allBatLength");
            }
        }

        private float _left1WhiteGlue;
        public float left1WhiteGlue
        {
            get { return _left1WhiteGlue; }
            set
            {
                _left1WhiteGlue = value;
                RaisePropertyChanged("left1WhiteGlue");
            }
        }

        private float _left2WhiteGlue;
        public float left2WhiteGlue
        {
            get { return _left2WhiteGlue; }
            set
            {
                _left2WhiteGlue = value;
                RaisePropertyChanged("left2WhiteGlue");
            }
        }

        private float _right1WhiteGlue;
        public float right1WhiteGlue
        {
            get { return _right1WhiteGlue; }
            set
            {
                _right1WhiteGlue = value;
                RaisePropertyChanged("right1WhiteGlue");
            }
        }

        private float _right2WhiteGlue;
        public float right2WhiteGlue
        {
            get { return _right2WhiteGlue; }
            set
            {
                _right2WhiteGlue = value;
                RaisePropertyChanged("right2WhiteGlue");
            }
        }

        private float _leftLugLen;
        public float leftLugLen
        {
            get { return _leftLugLen; }
            set
            {
                _leftLugLen = value;
                RaisePropertyChanged("leftLugLen");
            }
        }

        private float _rightLugLen;
        public float rightLugLen
        {
            get { return _rightLugLen; }
            set
            {
                _rightLugLen = value;
                RaisePropertyChanged("rightLugLen");
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
