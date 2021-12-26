using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Valute_Main.Models
{
    class Valute : BindableObject
    {

        public Valute(string name, double value, int nominal, string charcode)
        {
            this.ValuteName = name;
            this.Value = value;
            this.Nominal = nominal;
            this.CharCode = charcode;
        }

        private int _nominal;
        private string _valutename;
        private double _value;
        private string _charcode;

        public string CharCode
        {
            get => _charcode;
            set
            {
                _charcode = value;
                OnPropertyChanged();
            }
        }

        public int Nominal
        {
            get => _nominal;
            set
            {
                _nominal = value;
                OnPropertyChanged();
            }
        }

        public string ValuteName
        {
            get => _valutename;
            set
            {
                _valutename = value;
                OnPropertyChanged();
            }
        }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
