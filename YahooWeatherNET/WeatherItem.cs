using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using YahooWeatherNET.Exceptions;

namespace YahooWeatherNET
{
    /// <summary>
    /// Contains the requested weather report
    /// </summary>
    public class WeatherItem
    {
	    public WeatherItem()
	    {
		    Forecasts = new List<WeatherForecast>();
	    }

        /// <summary>
        /// Information about the location the forecast is determined to
        /// </summary>
        public WeatherLocation WLocation { get; set; }
        /// <summary>
        /// Information about the measurement units used
        /// </summary>
        public MeasurementUnits Units { get; set; }
        /// <summary>
        /// Information about the wind conditions
        /// </summary>
        public WindConditions Wind { get; set; }
        /// <summary>
        /// Information about the atmosphere conditions
        /// </summary>
        public AtmosphereConditions Atmosphere { get; set; }
        /// <summary>
        /// Information about astronomic conditions
        /// </summary>
        public AstronomyInfo Astronomy { get; set; }
        /// <summary>
        /// Title of weather report, e.g. 'Conditions for Koeln, DE at 6:00 pm CET'
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Exact position of the location the forecast is determined to
        /// </summary>
        public GeoCoordinate Location { get; set; }
        /// <summary>
        /// Link to online version
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Date of when the forecast was made
        /// </summary>
        public string Published { get; set; }
        /// <summary>
        /// Information about current weather conditions
        /// </summary>
        public WeatherCondition CurrentCondition { get; set; }
        /// <summary>
        /// Text version of weather report, formatted in HTML
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// List of forecasts. Forecast for today is element 0, forecast for tomorrow is element 1
        /// </summary>
        public List<WeatherForecast> Forecasts { get; set; }

	    public static WeatherItem FromJson(JObject _jObject)
	    {
		    var wI = new WeatherItem
		    {
			    Astronomy = AstronomyInfo.FromJson(_jObject["astronomy"] as JObject),
			    Atmosphere = AtmosphereConditions.FromJson(_jObject["atmosphere"] as JObject),
			    CurrentCondition = WeatherCondition.FromJson(_jObject["item"]["condition"] as JObject),
			    WLocation = WeatherLocation.FromJson(_jObject["location"] as JObject),
			    Units = MeasurementUnits.FromJson(_jObject["units"] as JObject),
			    Wind = WindConditions.FromJson(_jObject["wind"] as JObject),
				Location = CoordinatesFromJson(_jObject["item"] as JObject),
				Link = (string)_jObject["item"]["link"],
				Published = (string)_jObject["item"]["pubDate"],
				Title = (string)_jObject["item"]["title"],
				Description = (string)_jObject["item"]["description"]
			};

		    foreach (var forecast in _jObject["item"]["forecast"])
		    {
			    wI.Forecasts.Add(WeatherForecast.FromJson(forecast as JObject));
		    }

		    return wI;
	    }

	    private static GeoCoordinate CoordinatesFromJson(JObject _jObject)
	    {
		    double lat;
		    double lon;

			string strLat = (string)_jObject["lat"];
		    string strLon = (string) _jObject["long"];

			if (!double.TryParse(strLat, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out lat))
		    {
			    throw new GeoCoordinateConversionException("Could not convert latitude: " + _jObject["lat"]);
		    }
		    if (!double.TryParse(strLon, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out lon))
		    {
			    throw new GeoCoordinateConversionException("Could not convert longitude: " + _jObject["long"]);
		    }
		    return new GeoCoordinate(lat, lon);
	    }

	}
    /// <summary>
    /// Contains information about sunrise and sunset
    /// </summary>
    public class AstronomyInfo
    {
        /// <summary>
        /// Time of sunrise [hh:mm pm]
        /// </summary>
        public string Sunrise { get; set; }
        /// <summary>
        /// Time of sunset [hh:mm am]
        /// </summary>
        public string Sunset { get; set; }

	    public static AstronomyInfo FromJson(JObject _jObject)
	    {
			var astronomyInfo = new AstronomyInfo();
			astronomyInfo.Sunrise = (string)_jObject["sunrise"];
			astronomyInfo.Sunset = (string)_jObject["sunset"];
			return astronomyInfo;
		}
    }
    /// <summary>
    /// Detailed information about atmosphere conditions
    /// </summary>
    public class AtmosphereConditions
    {
        /// <summary>
        /// Humidity in percent
        /// </summary>
        public string Humidity { get; set; }
        /// <summary>
        /// Multiply with 100 to get visibility in miles or km (depends on Units)
        /// </summary>
        public string Visibility { get; set; }
        /// <summary>
        /// Air pressure in 'in' or 'mb' (depends on Units)
        /// </summary>
        public string Pressure { get; set; }
        /// <summary>
        /// Trend of air pressure. 0 = steady, 1 = rising, 2 = falling
        /// </summary>
        public string Rising { get; set; }

	    public static AtmosphereConditions FromJson(JObject _jObject)
	    {
			var atmoConditions = new AtmosphereConditions();
			atmoConditions.Humidity = (string)_jObject["humidity"];
			atmoConditions.Pressure = (string)_jObject["pressure"];
			atmoConditions.Rising = (string)_jObject["rising"];
			atmoConditions.Visibility = (string)_jObject["visibility"];
			return atmoConditions;
		}
    }
    /// <summary>
    /// Detailed information about wind conditions
    /// </summary>
    public class WindConditions
    {
        /// <summary>
        /// Windchill in degrees (depends on Units)
        /// </summary>
        public string Chill { get; set; }
        /// <summary>
        /// Direction of wind in degrees
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// Windspeed in mph or kph (depends on Units)
        /// </summary>
        public string Speed { get; set; }

	    public static WindConditions FromJson(JObject _jObject)
	    {
		    var windConditions = new WindConditions();
		    windConditions.Chill = (string) _jObject["chill"];
		    windConditions.Direction = (string) _jObject["direction"];
		    windConditions.Speed = (string) _jObject["speed"];
			return windConditions;
	    }
    }
    /// <summary>
    /// Further information about the location the forecast is determined to
    /// </summary>
    public class WeatherLocation
    {
        public string City { get; set; }
        /// <summary>
        /// Region-code made of 2 characters, if available
        /// </summary>
        public string Region { get; set; }
        public string Country { get; set; }

	    public static WeatherLocation FromJson(JObject _jObject)
	    {
			var weatherLocation = new WeatherLocation();
		    weatherLocation.City = (string) _jObject["city"];
		    weatherLocation.Country = (string) _jObject["country"];
		    weatherLocation.Region = (string) _jObject["region"];
		    return weatherLocation;
	    }
    }
    /// <summary>
    /// Units for forecast information
    /// </summary>
    public class MeasurementUnits
    {
        /// <summary>
        /// Degree, Celsius (c) or Fahrenheit (f)
        /// </summary>
        public string Temperature { get; set; }
        /// <summary>
        /// miles (m) or kilometer (km)
        /// </summary>
        public string Distance { get; set; }
        /// <summary>
        /// pounds per square inch (in) or millibars (mb)
        /// </summary>
        public string Pressure { get; set; }
        /// <summary>
        /// miles per hour (mph) or kilometers per hour (kph)
        /// </summary>
        public string Speed { get; set; }

	    public static MeasurementUnits FromJson(JObject _jObject)
	    {
		    var units = new MeasurementUnits();
		    units.Distance = (string) _jObject["distance"];
		    units.Pressure = (string) _jObject["pressure"];
		    units.Speed = (string) _jObject["speed"];
		    units.Temperature = (string) _jObject["temperature"];
		    return units;
	    }
    }
    /// <summary>
    /// Current weather conditions
    /// </summary>
    public class WeatherCondition
    {
        /// <summary>
        /// Description of weather conditions
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Weather condition code (see http://developer.yahoo.com/weather/#codes )
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Current temperature
        /// </summary>
        public string Temperature { get; set; }
        /// <summary>
        /// Exact date and time te forecast was made, in RFC822 Section 5 format
        /// </summary>
        public string Date { get; set; }

	    public static WeatherCondition FromJson(JObject _jObject)
	    {
		    var condition = new WeatherCondition();
		    condition.Code = (string) _jObject["code"];
		    condition.Date = (string) _jObject["date"];
		    condition.Temperature = (string) _jObject["temp"];
		    condition.Text = (string) _jObject["text"];
		    return condition;
	    }
    }
    /// <summary>
    /// Contains information about weather forecast
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// Day of Week the forecast is made for
        /// </summary>
        public string Day { get; set; }
        /// <summary>
        /// The exact Date [dd Mmm yyyy]
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Max temperature announced
        /// </summary>
        public string HighTemperature { get; set; }
        /// <summary>
        /// Min temperature announced
        /// </summary>
        public string LowTemperature { get; set; }
        /// <summary>
        /// Description of weather conditions
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Weather condition code (see http://developer.yahoo.com/weather/#codes )
        /// </summary>
        public string Code { get; set; }

	    public static WeatherForecast FromJson(JObject _jObject)
	    {
		    var forecast = new WeatherForecast();
		    forecast.Code = (string) _jObject["code"];
		    forecast.Date = (string) _jObject["date"];
		    forecast.Day = (string) _jObject["day"];
		    forecast.HighTemperature = (string) _jObject["high"];
		    forecast.LowTemperature = (string) _jObject["low"];
		    forecast.Text = (string) _jObject["text"];
		    return forecast;
	    }
    }
    /// <summary>
    /// Exact position of location
    /// </summary>
    public class GeoLocation
    {
        public string Lat{ get; set; }
        public string Long { get; set; }
    }
}
