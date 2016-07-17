using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YahooWeatherNET.Exceptions
{
	public class LocationResolverException : Exception
	{
		public LocationResolverException() : base() { }
		public LocationResolverException(string _message) : base(_message) { }
		public LocationResolverException(string _message, Exception _inner) : base(_message, _inner) { }
	}

	public class InvalidWoeidException : Exception
	{
		public InvalidWoeidException() : base() { }
		public InvalidWoeidException(string _message) : base(_message) { }
		public InvalidWoeidException(string _message, Exception _inner) : base(_message, _inner) { }
	}

	public class GeoCoordinateConversionException : Exception
	{
		public GeoCoordinateConversionException() : base() { }
		public GeoCoordinateConversionException(string _message) : base(_message) { }
		public GeoCoordinateConversionException(string _message, Exception _inner) : base(_message, _inner) { }
	}

	public class WeatherParsingException : Exception
	{
		public WeatherParsingException() : base() { }
		public WeatherParsingException(string _message) : base(_message) { }
		public WeatherParsingException(string _message, Exception _inner) : base(_message, _inner) { }
	}

}
