# YahooWeatherNET
[![Build Status](https://travis-ci.org/destiny14/YahooWeatherNET.svg?branch=master)](https://travis-ci.org/destiny14/YahooWeatherNET)

An easy-to-use .NET wrapper for the Yahoo Weather API. Now with OAuth Support!

**IMPORTANT:** [Yahoo APIs ToS](https://policies.yahoo.com/us/en/yahoo/terms/product-atos/apiforydn/index.htm) and [Yahoo Attribution Guidelines](https://developer.yahoo.com/attribution/) apply if you use this library/the API within your application!

You can use the YahooWeatherNET library for accessing online weather data from within your .NET application. The library makes use of the new Async/Await functionality offered by the .NET Framework, therefore the target Framework for this library is version 4.0 (asynchronous programming is available in version 4 when using the [AsyncTargetingPack](https://www.microsoft.com/en-us/download/details.aspx?id=29576)).

### Usage Instructions

**IMPORTANT:** As of version 2.0, the weather data can only be fetched asynchronously. If you still need synchronous access, you may use version 1.2. Instructions for version 1.2 can be found in the [wiki](https://github.com/destiny14/YahooWeatherNET/wiki/Usage-Instructions-(v.1.2.0.0)-%5BDEPRECATED%5D).

The following snippet will get you started:

```c#
using YahooWeatherNET;

public class Main
{
  private YahooWeatherProvider m_weatherProvider;

  public Main()
  {
    m_weatherProvider = new YahooWeatherProvider();
  }
  
  public async Task GetWeatherAsync()
  {
    // Create a new weather report for the desired location. Note that this call has to be made asynchronously,
    // because the location lookup is a call to the weather API. If you already have a WOEID, use .CreateReport("the woeid");
    WeatherReport report = await m_weatherProvider.CreateReportFromLocation("Cologne");
    
    // Query the current weather conditions.
    // This call can throw an UnknownWoeidException and a WeatherParsingException. These should be caught.
    WeatherItem wItem = await report.Query();
  }

  public async Task<string> ResolveLocationToWoeid()
  {
    // To manually resolve a location (town, POI or weather station) to a WOEID consumable by the API, use the LocationResolver class.
    return await LocationResolver.ResolveLocationToWoeid("Cologne");
  } 
}
```

Currently, Yahoo does not enforce OAuth authentication for the weather and location API. If this changes, you can pass your consumer key and secret to the YahooWeatherProvider constructor. See also [OAuth setup](https://github.com/destiny14/YahooWeatherNET/wiki/OAuth-Setup)

### Class Diagram
![class diagram](https://raw.githubusercontent.com/destiny14/YahooWeatherNET/master/ClassDiagram.png)

### Version History & Changelog

* [2.0.0.0](https://github.com/destiny14/YahooWeatherNET/releases/tag/2.0)
  * API redesign
    * Added WeatherReport abstraction
    * Switched to YQL queries
    * OAuth no longer mandatory
    * Removed all synchronous methods
* [1.2.0.0](https://github.com/destiny14/YahooWeatherNET/releases/tag/1.2)
  * Added OAuth authentication
* 1.1.0.0
  * Fixed typos
  * Changed location to GeoCoordinates
