using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sakuno;

namespace AutoSwitchING
{
    public class SwitchSetting : ModelBase
    {
        [NonSerialized]
        private string _api;
        public string Api
        {
            get
            {
                return _api;
            }
            set
            {
                if (_api != value)
                {
                    _api = value;
                    OnPropertyChanged();
                }
            }
        }

        [NonSerialized]
        private SwitchType _switchBy = SwitchType.Index;
        public SwitchType SwitchBy
        {
            get { return _switchBy; }
            set
            {
                if (_switchBy != value)
                {
                    _switchBy = value;
                    OnPropertyChanged();
                }
            }
        }

        [NonSerialized]
        private string _tabName = string.Empty;
        public string TabName
        {
            get { return _tabName; }
            set
            {
                if (_tabName != value)
                {
                    _tabName = value;
                    OnPropertyChanged();
                }
            }
        }

        [NonSerialized]
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged();
                }
            }
        }

        [NonSerialized]
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
