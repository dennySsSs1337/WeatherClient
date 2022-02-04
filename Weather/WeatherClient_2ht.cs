using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherClientWeb.OpenWeather
{
	public class OpenWeatherClient
	{
		const string defaultLanguage = "ru";
		const string apiKey = "7b6be55ecfc023f52792505653e8e278";
		const string urlTemplate = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&lang={2}";

		IMemoryCache _cache;
		public OpenWeatherClient(IMemoryCache cache)
		{
			_cache = cache;
		}

		public async ValueTask<CurrentWeatherDto> GetWeatherAsync(string cityName)
		{
			var lowerCasedCityName = cityName.ToLower();

			if (_cache.Contains(lowerCasedCityName))
			{
				return _cache.Get(lowerCasedCityName);
			}

			string currentWeatherUrl = string.Format(urlTemplate, lowerCasedCityName, apiKey, defaultLanguage);
			var httpClient = new HttpClient();

			var response = await httpClient.GetAsync(currentWeatherUrl);
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Openweathermap response has a fault code {response.StatusCode}");
			var currentWeatherJson = await response.Content.ReadAsStringAsync();

			JsonDocument currentWeatherDocument = JsonDocument.Parse(currentWeatherJson);
			var currentWeatherDto = currentWeatherDocument.Deserialize<CurrentWeatherDto>();

			CacheItem item = new CacheItem(lowerCasedCityName, currentWeatherDto);
			CacheItemPolicy policy = new CacheItemPolicy();
			policy.AbsoluteExpiration =
				DateTimeOffset.Now.AddMinutes(10);
			_cache.Set(item, policy);

			return currentWeatherDto;
		}
	}
}