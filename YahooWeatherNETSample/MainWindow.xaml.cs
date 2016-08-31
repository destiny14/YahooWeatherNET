using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YahooWeatherNET;
using YahooWeatherNET.Exceptions;

namespace YahooWeatherNETSample
{
    public partial class MainWindow : Window
    {
        YahooWeatherProvider m_weatherProvider;

        public MainWindow()
        {
            InitializeComponent();
			// we no longer need OAuth, secret and key can be omitted.
			// if API access stops working, try to pass secret and key.
	        m_weatherProvider = new YahooWeatherProvider();
        }
		
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bnGetWeather.Content = "Loading...";
            bnGetWeather.IsEnabled = false;

	        await GetWeatherReportForPosition();

            bnGetWeather.Content = "Get weather";
            bnGetWeather.IsEnabled = true;
        }

	    private async Task GetWeatherReportForPosition()
	    {
		    var units = GetSelectedTemperatureUnit();
			// create a new weather report
		    WeatherReport report = m_weatherProvider.CreateReport(tbLocation.Text);
		    report.TemperatureUnit = units;
		    try
		    {
			    // query the report to receive the newest weather forecasts
			    WeatherItem weatherItem = await report.Query();

			    WeatherBox.Text = FormatWeather(weatherItem);
		    }
		    catch (InvalidWoeidException _e) // WOEID unknown
		    {
			    WeatherBox.Text = _e.Message;
		    }
		    catch (WeatherParsingException _e) // server did not respond correctly
		    {
			    MessageBox.Show("Error while parsing the server response!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		    }
	    }

	    private string FormatWeather(WeatherItem _item)
	    {
		    StringBuilder stringBuilder = new StringBuilder();
            string pressureTrend = _item.Atmosphere.Rising == "0" ? "steady" : _item.Atmosphere.Rising == "1" ? "rising" : "falling";

		    stringBuilder.AppendLine(
			    $"weather report for {_item.WLocation.City}, {_item.WLocation.Country} (Lat: {_item.Location.Latitude}, Long: {_item.Location.Longitude})");
		    stringBuilder.AppendLine("current conditions:");
			// forecast [0] is the forecast for the current day
		    stringBuilder.AppendLine($"\ttemperature: {_item.CurrentCondition.Temperature} °{_item.Units.Temperature}, Min: {_item.Forecasts[0].LowTemperature} °{_item.Units.Temperature}, Max: {_item.Forecasts[0].HighTemperature} °{_item.Units.Temperature}");
		    stringBuilder.AppendLine($"\twind: {_item.Wind.Speed} {_item.Units.Speed}, direction: {_item.Wind.Direction}°");
            stringBuilder.AppendLine($"\tatmospheric pressure: {_item.Atmosphere.Pressure} {_item.Units.Pressure}, trend: {pressureTrend}");
		    stringBuilder.AppendLine("forecast:");
		    stringBuilder.AppendLine(
			    $"\t{_item.Forecasts[1].Day}, {_item.Forecasts[1].Date}: {_item.Forecasts[1].HighTemperature}/{_item.Forecasts[1].LowTemperature} °{_item.Units.Temperature}");
			stringBuilder.AppendLine(
				$"\t{_item.Forecasts[2].Day}, {_item.Forecasts[2].Date}: {_item.Forecasts[2].HighTemperature}/{_item.Forecasts[2].LowTemperature} °{_item.Units.Temperature}");
			stringBuilder.AppendLine(
				$"\t{_item.Forecasts[3].Day}, {_item.Forecasts[3].Date}: {_item.Forecasts[3].HighTemperature}/{_item.Forecasts[3].LowTemperature} °{_item.Units.Temperature}");

		    return stringBuilder.ToString();
	    }

	    private TemperatureUnit GetSelectedTemperatureUnit()
	    {
			switch (cbUnit.SelectedIndex)
			{
				case 0:
				default:
					return TemperatureUnit.Celsius;
				case 1:
					return TemperatureUnit.Fahrenheit;
			}
		}

        private async void bnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Use the static method ResolveWeatherCode to get the WOEID code of a city, poi or weather station
	        tbLocation.Text = await LocationResolver.ResolveLocationToWoeid(tbSearch.Text);
		}
    }
}
