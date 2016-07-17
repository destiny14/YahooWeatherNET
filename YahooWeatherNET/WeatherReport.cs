using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using YahooWeatherNET.Exceptions;

namespace YahooWeatherNET
{
	public class WeatherReport
	{
		private AuthorizationProvider authorizationProvider;
		private string woeid;
		private TemperatureUnit temperatureUnit = TemperatureUnit.Celsius;
		private WebClient webClient;

		public string Woeid
		{
			get { return woeid; }
		}

		public TemperatureUnit TemperatureUnit
		{
			get { return temperatureUnit; }
			set { temperatureUnit = value; }
		}

		public WeatherReport(string _woeid, AuthorizationProvider _auth)
		{
			woeid = _woeid;
			authorizationProvider = _auth;
			webClient = new WebClient();
		}

		public static WeatherReport FromWoeid(string _woeid, AuthorizationProvider _auth)
		{
			return new WeatherReport(_woeid, _auth);
		}

		public static async Task<WeatherReport> FromLocation(string _location, AuthorizationProvider _auth)
		{
			var woeid = await LocationResolver.ResolveLocationToWoeid(_location);
			return new WeatherReport(woeid, _auth);
		}

		public async Task<WeatherItem> Query()
		{
			string query = ConstructQuery();
			string rawReport = await QueryRaw(query);
			return ParseRawReport(rawReport);
		}

		private string ConstructQuery()
		{
			var unitStr = temperatureUnit == TemperatureUnit.Celsius ? "c" : "f";
			return "select * from weather.forecast where woeid='" + woeid + "' and u='" + unitStr + "'";
		}

		private async Task<string> QueryRaw(string _query)
		{
			var uri = new Uri(string.Format(YahooWeatherProvider.yahooQueryUrl, _query));
			webClient.Headers.Add(authorizationProvider.AssembleOAuthHeader());
			return await webClient.DownloadStringTaskAsync(uri);
		}

		private WeatherItem ParseRawReport(string _report)
		{
			var reportObj = JObject.Parse(_report)["query"]["results"]["channel"] as JObject;

			CheckForValidReport(reportObj);

			try
			{
				return WeatherItem.FromJson(reportObj);
			}
			catch (Exception e)
			{
				throw new WeatherParsingException("See inner exception for details.", e);
			}
		}

		private void CheckForValidReport(JObject _jObject)
		{
			JToken tmp = new JObject();
			if (!_jObject.TryGetValue("title", out tmp))
			{
				throw new InvalidWoeidException("City for WOEID " + woeid + " not found.");
			}
		}
	}
}