using System;
using System.CodeDom;
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
	    private AuthorizationProvider authorizationProvider;

	    public static readonly string yahooQueryUrl = "https://query.yahooapis.com/v1/public/yql?q={0}&format=json";

        /// <summary>
        /// Provides possibility to gather weather information from yahoo.com
        /// 
        /// It seems like Yahoo removed the OAuth authentication from the weather API again. Key and secret can be omitted.
        /// </summary>
        public YahooWeatherProvider(string _consumerKey = "", string _consumerSecret = "")
        {
            authorizationProvider = new AuthorizationProvider(_consumerKey, _consumerSecret);
        }

		/// <summary>
		/// Creates a new weather report and returns it to the caller.
		/// </summary>
	    public WeatherReport CreateReport(string _woeid)
	    {
		    return WeatherReport.FromWoeid(_woeid, authorizationProvider);
	    }

		/// <summary>
		/// Resolves the given location search term and creates a new weather report if the location could be resolved.
		/// Returns the weather report to the caller.
		/// </summary>
	    public async Task<WeatherReport> CreateReportFromLocation(string _location)
	    {
		    return await WeatherReport.FromLocation(_location, authorizationProvider);
	    }


        [Obsolete("Replaced by LocationResolver class", true)]
        public static string ResolveWeatherCode(string _location)
        {
	        throw new NotImplementedException("Use LocationResolver class instead");
        }
		
        [Obsolete("Replaced by GetWeatherAsync", true)]
        public WeatherItem GetWeather() 
        {
			throw new NotImplementedException("Use GetWeatherAsync instead");
        }

        [Obsolete("Replaced by CreateReport and CreateReportFromLocation.", true)]
        public async Task<WeatherItem> GetWeatherAsync()
        {
	        throw new NotImplementedException("Use CreateReport or CreateReportFromLocation instead.");
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
