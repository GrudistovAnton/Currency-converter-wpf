using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Curreny_converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private double _usdRate = 0;
        private double _eurRate = 0;
        private double _jpyRate = 0;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadRatesAsync(); 
        }

        private async Task LoadRatesAsync()
        {
            try
            {
                string json = await _httpClient.GetStringAsync("https://www.cbr-xml-daily.ru/daily_json.js");
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement valute = doc.RootElement.GetProperty("Valute");
                _usdRate = valute.GetProperty("USD").GetProperty("Value").GetDouble();
                _eurRate = valute.GetProperty("EUR").GetProperty("Value").GetDouble();
                _jpyRate = valute.GetProperty("JPY").GetProperty("Value").GetDouble();
                int jpyNominal = valute.GetProperty("JPY").GetProperty("Nominal").GetInt32();
                if (jpyNominal != 1)
                    _jpyRate = _jpyRate / jpyNominal;
            } 
            catch
            {
                _usdRate = 90;
                _eurRate = 100;
                _jpyRate = 0.6;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(textBoox1.Text, out double amount))
            {
                textbloox1.Text = "Введите число!";
                return;
            }

            if (amount <= 0)
            {
                textbloox1.Text = "Сумма должна быть больше нуля";
                return;
            }

            double result = 0;
            string currency = "";

            if (Jpy.IsChecked == true)
            {
                if (_jpyRate <= 0) { textbloox1.Text = "Курс не загружен"; return; }
                result = amount / _jpyRate;
                currency = "JPY";
            }
            else if (Euro.IsChecked == true)
            {
                if (_eurRate <= 0) { textbloox1.Text = "Курс не загружен"; return; }
                result = amount / _eurRate;
                currency = "EUR";
            }
            else if (USD.IsChecked == true)
            {
                if (_usdRate <= 0) { textbloox1.Text = "Курс не загружен"; return; }
                result = amount / _usdRate;
                currency = "USD";
            }
            else
            {
                textbloox1.Text = "Выберите валюту";
                return;
            }

            textbloox1.Text = $"{result:F2} {currency}";
        }
    }
}