using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YahooWeatherNET
{
    public class YahooWeatherProvider
    {
        private string m_consumerKey;
        private string m_consumerSecret;
        private string m_weatherCode;
        private string m_temperatureUnit;
        private static readonly XNamespace yweather = "http://xml.weather.yahoo.com/ns/rss/1.0";
        private static readonly XNamespace geo = "http://www.w3.org/2003/01/geo/wgs84_pos#";
        private static readonly XNamespace xmlns = "http://where.yahooapis.com/v1/schema.rng";

        /// <summary>
        /// Provides possibility to gather weather information from yahoo.com
        /// </summary>
        /// <param name="_weatherCode">WOEID which indicates the _location for the weather forecast</param>
        /// <param name="_tempUnit">The units which should be used (Celsius includes metric system)</param>
        public YahooWeatherProvider(string _weatherCode, TemperatureUnit _tempUnit, string _consumerKey, string _consumerSecret)
        {
            m_consumerKey = _consumerKey;
            m_consumerSecret = _consumerSecret;
            m_weatherCode = _weatherCode;
            if (_tempUnit == TemperatureUnit.Celsius)
            {
                m_temperatureUnit = "c";
            }
            else
            {
                m_temperatureUnit = "f";
            }
        }

        /// <summary>
        /// Resolve the WOEID (Weather Code) from a search term (POI, City, weather station)
        /// </summary>
        /// <param name="_location">search term (e.g. 'Cologne DE', 'Dusseldorf', 'Kölner Dom')</param>
        /// <returns></returns>
        public static string ResolveWeatherCode(string _location)
        {
            WebClient mWebClient = new WebClient();
            string loc = mWebClient.DownloadString("http://where.yahooapis.com/v1/places.q('" + _location + "')?appid=jVMO1MvV34GYE4aEqO7ZTnAKuCpPU0Adp5kiAb1oxhzcFmjRhzj6h85eRL31Q3n9tA3.4es7B9Ep1I0tAEfx.qjLqSxJiJM-");
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(loc);
            MemoryStream stream = new MemoryStream(bytes);
            XDocument _XDocument = XDocument.Load(stream);
            string woeid;
            try
            {
                woeid = (from x in _XDocument.Descendants(xmlns + "places").Descendants(xmlns + "place").Descendants(xmlns + "woeid") select x.Value).First();
            }
            catch (InvalidOperationException ex)
            {
                woeid = "0";
            }
            return woeid;
        }

        /// <summary>
        /// Requests a new weather forecast. If the Weather Code provided is invalid, the Title property of the returned WeatherItem will be set to 'City not found'
        /// </summary>
        /// <returns></returns>
        public WeatherItem GetWeather() 
        {
            WebClient client = new WebClient();
            client.Headers.Add("Authorization: OAuth " +
                "realm=\"yahooapis.com\","+
                "oauth_consumer_key=\"" + m_consumerKey + "\","+
                "oauth_nonce=\"" + Guid.NewGuid().ToString() + "\","+
                "oauth_signature_method=\"PLAINTEXT\","+
                "oauth_timestamp=\"" + ((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString() + "\"," +
                "oauth_version=\"1.0\"," +
                "oauth_signature=\"" + m_consumerSecret + "%26\","+
                "oauth_callback=\"oob\"");

            string rawForecast = client.DownloadString("http://weather.yahooapis.com/forecastrss?w=" + m_weatherCode + "&u=" + m_temperatureUnit);

            return ParseReport(rawForecast);
        }

        /// <summary>
        /// Requests a new weather forecast asynchronously. If the Weather Code provided is invalid, the Title property of the returned WeatherItem will be set to 'City not found'
        /// </summary>
        /// <returns></returns>
        public async Task<WeatherItem> GetWeatherAsync()
        {
            string query = "select * from weather.forecast where woeid=" + m_weatherCode;

            WebClient client = new WebClient();
            client.Headers.Add("Authorization: OAuth " +
                "realm=\"yahooapis.com\"," +
                "oauth_consumer_key=\"" + m_consumerKey + "\"," +
                "oauth_nonce=\"" + Guid.NewGuid().ToString() + "\"," +
                "oauth_signature_method=\"PLAINTEXT\"," +
                "oauth_timestamp=\"" + ((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString() + "\"," +
                "oauth_version=\"1.0\"," +
                "oauth_signature=\"" + m_consumerSecret + "%26\"," +
                "oauth_callback=\"oob\"");

            try
            {
                string rawForecast =
                    await
                        client.DownloadStringTaskAsync(
                            new Uri("http://weather.yahooapis.com/forecastrss?w=" + m_weatherCode + "&u=" +
                                    m_temperatureUnit));
                return ParseReport(rawForecast);
            }
            catch (WebException _ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Parses the report received from the weather API
        /// </summary>
        /// <param name="_forecast">The raw forecast data in XML</param>
        /// <returns></returns>
        private WeatherItem ParseReport(string _forecast)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(_forecast);
            MemoryStream stream = new MemoryStream(bytes);
            XDocument xDocument = XDocument.Load(stream);

            WeatherItem weatherItem = new WeatherItem();

            weatherItem.Title = (from x in xDocument.Descendants("channel").Descendants("item").Descendants("title") select x.Value).Single();

            if (weatherItem.Title == "City not found") { return weatherItem; }

            WeatherLocation weatherLocation = new WeatherLocation();
            weatherLocation.City = GetValueFromYWeatherItem("location", "city", xDocument);
            weatherLocation.Region = GetValueFromYWeatherItem("location", "region", xDocument);
            weatherLocation.Country = GetValueFromYWeatherItem("location", "country", xDocument);
            weatherItem.WLocation = weatherLocation;

            MeasurementUnits units = new MeasurementUnits();
            units.Distance = GetValueFromYWeatherItem("units", "distance", xDocument);
            units.Pressure = GetValueFromYWeatherItem("units", "pressure", xDocument);
            units.Speed = GetValueFromYWeatherItem("units", "speed", xDocument);
            units.Temperature = GetValueFromYWeatherItem("units", "temperature", xDocument);
            weatherItem.Units = units;

            WindConditions wind = new WindConditions();
            wind.Chill = GetValueFromYWeatherItem("wind", "chill", xDocument);
            wind.Direction = GetValueFromYWeatherItem("wind", "direction", xDocument);
            wind.Speed = GetValueFromYWeatherItem("wind", "speed", xDocument);
            weatherItem.Wind = wind;

            AtmosphereConditions atmosphere = new AtmosphereConditions();
            atmosphere.Humidity = GetValueFromYWeatherItem("atmosphere", "humidity", xDocument);
            atmosphere.Pressure = GetValueFromYWeatherItem("atmosphere", "pressure", xDocument);
            atmosphere.Rising = GetValueFromYWeatherItem("atmosphere", "rising", xDocument);
            atmosphere.Visibility = GetValueFromYWeatherItem("atmosphere", "visibility", xDocument);
            weatherItem.Atmosphere = atmosphere;

            AstronomyInfo astronomy = new AstronomyInfo();
            astronomy.Sunrise = GetValueFromYWeatherItem("astronomy", "sunrise", xDocument);
            astronomy.Sunset = GetValueFromYWeatherItem("astronomy", "sunset", xDocument);
            weatherItem.Astronomy = astronomy;

            GeoLocation loc = new GeoLocation();
            loc.Lat = GetValueFromGeoItem("lat", xDocument);
            loc.Long = GetValueFromGeoItem("long", xDocument);

            weatherItem.Location = loc;
            weatherItem.Link = (from x in xDocument.Descendants("channel").Descendants("item").Descendants("link") select x.Value).Single();
            weatherItem.Published = (from x in xDocument.Descendants("channel").Descendants("item").Descendants("pubDate") select x.Value).Single();

            WeatherCondition currentCondition = new WeatherCondition();
            currentCondition.Code = GetValueFromCurrentConditionItem("code", xDocument);
            currentCondition.Date = GetValueFromCurrentConditionItem("date", xDocument);
            currentCondition.Temperature = GetValueFromCurrentConditionItem("temp", xDocument);
            currentCondition.Text = GetValueFromCurrentConditionItem("text", xDocument);

            weatherItem.CurrentCondition = currentCondition;

            List<WeatherForecast> forecasts = new List<WeatherForecast>();
            var locForecast = from x in xDocument.Descendants("channel").Descendants("item").Descendants(yweather + "forecast") select x;
            foreach (XElement el in locForecast)
            {
                WeatherForecast wfc = new WeatherForecast();
                wfc.Code = el.Attribute("code").Value;
                wfc.Date = el.Attribute("date").Value;
                wfc.Day = el.Attribute("day").Value;
                wfc.HighTemperature = el.Attribute("high").Value;
                wfc.LowTemperature = el.Attribute("low").Value;
                wfc.Text = el.Attribute("text").Value;
                forecasts.Add(wfc);
            }

            weatherItem.Forecasts = forecasts;

            return weatherItem;
        }

        private string GetValueFromYWeatherItem(string _descendantName, string _attributeName, XDocument _doc)
        {
            return (from x in _doc.Descendants(yweather + _descendantName) select x.Attribute(_attributeName).Value).Single();
        }

        private string GetValueFromGeoItem(string _descendantName, XDocument _doc)
        {
            return (from x in _doc.Descendants("channel").Descendants("item").Descendants(geo + _descendantName) select x.Value).Single();
        }

        private string GetValueFromCurrentConditionItem(string _attrName, XDocument _doc)
        {
            return (from x in _doc.Descendants("channel").Descendants("item").Descendants(yweather + "condition") select x.Attribute(_attrName).Value).Single();
        }
        
    }

    /// <summary>
    /// Unit which should be used for requesting weather data
    /// </summary>
    public enum TemperatureUnit
    {
        Celsius = 0,
        Fahrenheit = 1
    }
}
