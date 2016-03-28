# YahooWeatherNET
An easy-to-use .NET wrapper for the Yahoo Weather API. Now with OAuth Support!

**ATTENTION:** [Yahoo APIs ToS](https://policies.yahoo.com/us/en/yahoo/terms/product-atos/apiforydn/index.htm) and [Yahoo Attribution Guidelines](https://developer.yahoo.com/attribution/) apply if you use this library/the API within your application!

You can use the YahooWeatherNET library for accessing online weather data from within your .NET application. The library makes use of the new Async/Await functionality offered by the .NET Framework, therefore the target Framework for this library is version 4.0 (asynchronous programming is available in version 4 when using the [AsyncTargetingPack](https://www.microsoft.com/en-us/download/details.aspx?id=29576)).

**NEW:** As of March 15, 2016 Yahoo enforces authentication when accessing some portions of their API. You now need to register an application in the Yahoo Development Network and use the library with the client key and secret received by Yahoo.

### Aquiring Client Key and Secret
To aquire the client key and secret needed for authenticating with the Yahoo APIs you need to [create an application](https://developer.yahoo.com/apps/create/) inside the Yahoo Developer Network.
There is not much to fill out but three things are worth mentioning:
* As for "Application Type", select "Installed Application"
* Do **not** enter a "Callback Domain"
* Do **not** check any of the boxes below "API Permissions"

You can view your key and secret once you click the "Create Application" button.

### Using YahooWeatherNET
Using the library is quite simple:
```c#
using YahooWeatherNET;

public class Main
{
  private YahooWeatherProvider m_weatherProvider;
  private TemperatureUnit m_tempUnit = TemperatureUnit.Celsius;
  
  public void GetWeatherSync()
  {
    // the lookup is made using a location code (woeid). this method will return the location code matching your search term.
    // you can search for cities or POIs
    string woeid = YahooWeatherProvider.ResolveWeatherCode("Cologne");
  
    m_weatherProvider = new YahooWeatherProvider(woeid, m_tempUnit, "CONSUMER KEY", "CONSUMER SECRET");
    WeatherItem wItem = m_weatherProvider.GetWeather();
  }
  
  public async void GetWeatherAsync()
  {
    string woeid = YahooWeatherProvider.ResolveWeatherCode("Cologne");
  
    m_weatherProvider = new YahooWeatherProvider(woeid, m_tempUnit, "CONSUMER KEY", "CONSUMER SECRET");
    WeatherItem wItem = await m_weatherProvider.GetWeatherAsync();
  }
}
```
Replace "CONSUMER KEY" and "CONSUMER SECRET" with the strings you aquired from the Yahoo Developer Network. Resolving woeids does not need authentication.

### Class Diagram
![class diagram](https://raw.githubusercontent.com/destiny14/YahooWeatherNET/master/ClassDiagram.png)
