using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using YahooWeatherNET.Exceptions;

namespace YahooWeatherNET
{
	/// <summary>
	/// Provides access to the Yahoo geo.places API.
	/// </summary>
	public static class LocationResolver
	{
		/// <summary>
		/// Resolve the WOEID (Weather Code) from a search term (POI, City, weather station) asynchronously. Does not require OAuth authentication.
		/// </summary>
		/// <param name="_location">search term (e.g. 'Cologne DE', 'Düsseldorf', 'Kölner Dom')</param>
		/// <returns></returns>
		public static async Task<string> ResolveLocationToWoeid(string _location)
		{
			var response = await SendResolveRequest(_location);
			return ParseResolveResponse(response);
		}

		private static async Task<string> SendResolveRequest(string _location)
		{
			string query = "select woeid from geo.places(1) where text='" + _location + "'";

			WebClient mWebClient = new WebClient();
			var response = await mWebClient.DownloadStringTaskAsync(new Uri(string.Format(YahooWeatherProvider.yahooQueryUrl, query)));
			return response;
		}

		private static string ParseResolveResponse(string _response)
		{
			var responseObj = JObject.Parse(_response);

			if (((string) responseObj["query"]["count"]) == "0")
			{
				throw new LocationResolverException("No WOEID found");
			}

			return (string)responseObj["query"]["results"]["place"]["woeid"];
		}
	}
}
