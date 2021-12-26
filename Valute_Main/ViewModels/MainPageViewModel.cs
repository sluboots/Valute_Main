using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Valute_Main.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Internals;

namespace Valute_Main.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private string _selectedDate;
        private ObservableCollection<Valute> _valutes;
        private Valute _selectedValuteInput;
        private Valute _selectedValuteResult;
        private string _inputValue;
        private string _resultValue;
        private HttpClient Client { get; }

        public MainPageViewModel()
        {
            Client = new HttpClient();
            Valutes = new ObservableCollection<Valute>();
            PropertyMaximumDate = DateTime.Now;
            PropertyMinimumDate = DateTime.Now.AddDays(-31);
            LoadValutes(DateTime.Now);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public string InputValue
        {
            get => _inputValue;
            set
            {
                if (_inputValue == value) 
                    return;

                _inputValue = value;

                CalculateResultValue(value);
                OnPropertyChanged();
            }
        }

        public string ResultValue
        {
            get => _resultValue;
            set
            {
                if (_resultValue == value) 
                    return;

                _resultValue = value;

                CalculateInputValue(value);
                OnPropertyChanged();
            }
        }

        public Valute SelectedValuteInput
        {
            get => _selectedValuteInput;
            set
            {
                _selectedValuteInput = value;

                CalculateInputValue(ResultValue);
                OnPropertyChanged();
            }
        }

        public Valute SelectedValuteResult
        {
            get => _selectedValuteResult;
            set
            {
                _selectedValuteResult = value;

                CalculateResultValue(InputValue);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Valute> Valutes
        {
            get => _valutes;
            set
            {
                _valutes = value;
                OnPropertyChanged();
            }
        }

        public DateTime PropertyMaximumDate { get; }
        public DateTime PropertyMinimumDate { get; }

        private void CalculateInputValue(string value)
        {
            if (SelectedValuteInput == null || SelectedValuteResult == null || string.IsNullOrWhiteSpace(value))
            {
                _inputValue = "0";
            }
            else
            {
                var result = Convert.ToDouble(value) * SelectedValuteResult.Value / SelectedValuteResult.Nominal /
                          (SelectedValuteInput.Value / SelectedValuteInput.Nominal);

                _inputValue = result.ToString();
            }

            OnPropertyChanged(nameof(InputValue));
        }

        private void CalculateResultValue(string value)
        {
            if (SelectedValuteInput == null || SelectedValuteResult == null || string.IsNullOrWhiteSpace(value))
            {
                _resultValue = "0";
            }
            else
            {
                var result = Convert.ToDouble(value) * SelectedValuteInput.Value / SelectedValuteInput.Nominal /
                          (SelectedValuteResult.Value / SelectedValuteResult.Nominal);
                //    var res = Convert.ToDouble(value) * SelectedValuteInput.Value / SelectedValuteInput.Nominal /
                //              (SelectedValuteResult.Value / SelectedValuteResult.Nominal);

                _resultValue = result.ToString();
            }

            OnPropertyChanged(nameof(ResultValue));
        }

        public DateTime CheckDateTime(DateTime date)
        {
            if (date > DateTime.Now)
                date = DateTime.Now;
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                if (date.DayOfWeek == DayOfWeek.Saturday)
                    date = date.AddDays(-1);
                else
                    date = date.AddDays(-1);

            }
            return date;


        }
        public async Task LoadValutes(DateTime date)
        {
           
            var dateResult = CheckDateTime(date);
            var dateString = dateResult.ToString("yyyy/MM/dd");
            var url = $"https://www.cbr-xml-daily.ru/archive/{dateString}/daily_json.js";

            //using (StreamReader r = new StreamReader($"https://www.cbr-xml-daily.ru/archive/daily_json.js"))
            //{
            //    string json = r.ReadToEnd();
            //    List<Valute> items = JsonConvert.DeserializeObject<List<Valute>>(json);
            //    var valute =
            //    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Valute>>(items[0]?.ToString() ?? "");

            //    Valutes = new ObservableCollection<Valute>(Valute?.Select(x => x.Value) ?? new List<>());
            //    Valutes.Add(new Valute("Рубли", 1, 1));

            //    SelectedCurrencyLeft = Valutes.Last();
            //    SelectedCurrencyRight = Valutes.Last();

            //    OnPropertyChanged();
            //}
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream resStream = response.GetResponseStream();
            //string responseText = "";
            //WebHeaderCollection header = response.Headers;

            //var encoding = ASCIIEncoding.ASCII;
            //using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            //{
            //    responseText = reader.ReadToEnd();
            //}

            var response = await Client.GetAsync(url);

            var valuteJson = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(valuteJson);

            var valute =
                JsonConvert.DeserializeObject<Dictionary<string, Valute>>(jObject["Valute"]?.ToString() ?? "");

            Valutes = new ObservableCollection<Valute>(valute?.Select(x => x.Value) ?? new List<Valute>());
            Valutes.Add(new Valute("Российский рубль", 1, 1, "RUB"));
            OnPropertyChanged();

        }



        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}