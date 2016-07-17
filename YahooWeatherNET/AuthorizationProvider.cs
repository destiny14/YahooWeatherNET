using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YahooWeatherNET
{
	public class AuthorizationProvider
	{
		private string consumerKey;
		private string consumerSecret;

		public AuthorizationProvider(string _key, string _secret)
		{
			consumerKey = _key;
			consumerSecret = _secret;
		}

		public string AssembleOAuthHeader()
		{
			return "Authorization: OAuth " +
				   "realm=\"yahooapis.com\"," +
				   "oauth_consumer_key=\"" + consumerKey + "\"," +
				   "oauth_nonce=\"" + Guid.NewGuid() + "\"," +
				   "oauth_signature_method=\"PLAINTEXT\"," +
				   "oauth_timestamp=\"" + ((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)) +
				   "\"," +
				   "oauth_version=\"1.0\"," +
				   "oauth_signature=\"" + consumerSecret + "%26\"," +
				   "oauth_callback=\"oob\"";

		}
	}
}
