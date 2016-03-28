using System;
using System.Collections.Generic;
using System.Linq;
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
using YahooWeatherNET;

namespace YahooWeatherNETSample
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CONSUMER_KEY = "";
        public static string CONSUMER_SECRET = "";
        YahooWeatherProvider m_weatherProvider;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Example for how to use the asynchronous method.
        /// </summary>
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TemperatureUnit unit = TemperatureUnit.Celisius;
            switch (cbUnit.SelectedIndex)
            {
                case 0:
                    unit = TemperatureUnit.Celisius;
                    break;
                case 1:
                    unit = TemperatureUnit.Farenheit;
                    break;
            }
            //create new YahooWeatherProvider object
            m_weatherProvider = new YahooWeatherProvider(tbLocation.Text, unit, CONSUMER_KEY, CONSUMER_SECRET);
            //local variable for storing the weather report
            WeatherItem i;
            bnGetWeather.Content = "Loading...";
            bnGetWeather.IsEnabled = false;
            //wait for the async request to finish
            i = await m_weatherProvider.GetWeatherAsync();
            bnGetWeather.Content = "Get weather";
            bnGetWeather.IsEnabled = true;
            //if invalid code was given, end here
            if (i.Title == "City not found") { WeatherBox.Text = i.Title; return; }
            //process weather information
            if (unit == TemperatureUnit.Celisius)
            {
                WeatherBox.Text = "weather forecast for " + i.WLocation.City + ", " + i.WLocation.Country + " (Lat: " + i.Location.Lat + "; Long: " + i.Location.Long + ")\n"
                    + "current conditions:\n"
                    + "wind: " + i.Wind.Speed + " km/h, direction: " + i.Wind.Direction + "\n"
                    + i.Forecasts[0].Day + ": " + i.Forecasts[0].HighTemperature + "°C, " + i.Forecasts[0].Text + " (" + i.Forecasts[0].Code + ")\n"
                    + i.Forecasts[1].Day + ": " + i.Forecasts[1].HighTemperature + "°C, " + i.Forecasts[1].Text + " (" + i.Forecasts[1].Code + ")\n";
            }
            else if (unit == TemperatureUnit.Farenheit)
            {
                WeatherBox.Text = "weather forecast for " + i.WLocation.City + ", " + i.WLocation.Country + " (Lat: " + i.Location.Lat + "; Long: " + i.Location.Long + ")\n"
                + "current conditions:\n"
                + "wind: " + i.Wind.Speed + " mp/h, direction: " + i.Wind.Direction + "\n"
                + i.Forecasts[0].Day + ": " + i.Forecasts[0].HighTemperature + "°F, " + i.Forecasts[0].Text + " (" + i.Forecasts[0].Code + ")\n"
                + i.Forecasts[1].Day + ": " + i.Forecasts[1].HighTemperature + "°F, " + i.Forecasts[1].Text + " (" + i.Forecasts[1].Code + ")\n";
            }
        }

        /// <summary>
        /// Example for how to use the synchronous method.
        /// </summary>
        private void bnGetWeatherSync_Click(object sender, RoutedEventArgs e)
        {
            TemperatureUnit unit = TemperatureUnit.Celisius;
            switch (cbUnit.SelectedIndex)
            {
                case 0:
                    unit = TemperatureUnit.Celisius;
                    break;
                case 1:
                    unit = TemperatureUnit.Farenheit;
                    break;
            }
            //create new YahooWeatherProvider object
            m_weatherProvider = new YahooWeatherProvider(tbLocation.Text, unit, CONSUMER_KEY, CONSUMER_SECRET);
            //local variable for storing the weather report
            WeatherItem i;
            //request weather report (Warning: blocks the UI tread)
            i = m_weatherProvider.GetWeather();
            //if invalid code was given, end here
            if (i.Title == "City not found") { WeatherBox.Text = i.Title; return; }
            //process weather information
            if (unit == TemperatureUnit.Celisius)
            {
                WeatherBox.Text = "weather forecast for " + i.WLocation.City + ", " + i.WLocation.Country + " (Lat: " + i.Location.Lat + "; Long: " + i.Location.Long + ")\n"
                    + "current conditions:\n"
                    + "wind: " + i.Wind.Speed + " km/h, direction: " + i.Wind.Direction + "\n"
                    + i.Forecasts[0].Day + ": " + i.Forecasts[0].HighTemperature + "°C, " + i.Forecasts[0].Text + " (" + i.Forecasts[0].Code + ")\n"
                    + i.Forecasts[1].Day + ": " + i.Forecasts[1].HighTemperature + "°C, " + i.Forecasts[1].Text + " (" + i.Forecasts[1].Code + ")\n";
            }
            else if (unit == TemperatureUnit.Farenheit)
            {
                WeatherBox.Text = "weather forecast for " + i.WLocation.City + ", " + i.WLocation.Country + " (Lat: " + i.Location.Lat + "; Long: " + i.Location.Long + ")\n"
                + "current conditions:\n"
                + "wind: " + i.Wind.Speed + " mp/h, direction: " + i.Wind.Direction + "\n"
                + i.Forecasts[0].Day + ": " + i.Forecasts[0].HighTemperature + "°F, " + i.Forecasts[0].Text + " (" + i.Forecasts[0].Code + ")\n"
                + i.Forecasts[1].Day + ": " + i.Forecasts[1].HighTemperature + "°F, " + i.Forecasts[1].Text + " (" + i.Forecasts[1].Code + ")\n";
            }
        }

        private void bnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Use the static method ResolveWeatherCode to get the WOEID code of a city, poi or weather station
            tbLocation.Text = YahooWeatherProvider.ResolveWeatherCode(tbSearch.Text);
        }
    }
}
