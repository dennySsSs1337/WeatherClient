using System.Text.Json;
using System.Web;

namespace WeatherApiSimple
{
	public class Program
	{
		public static async Task<int> Main(string[] argv)
		{
			const string apiKey = "10dc952f4bffae582c4ed47842d0fbc6";
			Console.Write("Введите город: ");
			string? city = Console.ReadLine();
			var builderWithCity = new UriBuilder("https://api.openweathermap.org/data/2.5/weather");
			var firstQueryParameters = HttpUtility.ParseQueryString(builderWithCity.Query);
			firstQueryParameters["q"] = city;
			firstQueryParameters["appid"] = apiKey;
			builderWithCity.Query = firstQueryParameters.ToString();

			Uri uri = builderWithCity.Uri;
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			var client = new HttpClient();
			HttpResponseMessage result = await client.SendAsync(request);
			string jsonContent = await result.Content.ReadAsStringAsync();
			JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
			double coord_lon = jsonDocument.RootElement
				.GetProperty("coord")
				.GetProperty("lon")
				.GetDouble();
			double coord_lat = jsonDocument.RootElement
				.GetProperty("coord")
				.GetProperty("lat")
				.GetDouble();

			Console.WriteLine("долгота: {0}, широта: {1}", coord_lon, coord_lat);


			var builderWithCoords = new UriBuilder("https://api.openweathermap.org/data/2.5/onecall");
			var secondQueryParameters = HttpUtility.ParseQueryString(builderWithCoords.Query);
			secondQueryParameters["lat"] = coord_lat.ToString();
			secondQueryParameters["lon"] = coord_lon.ToString();
			secondQueryParameters["appid"] = apiKey;
			builderWithCoords.Query = secondQueryParameters.ToString();

			Uri uriCoords = builderWithCoords.Uri;
			var requestCoords = new HttpRequestMessage(HttpMethod.Get, uriCoords);
			var clientCoords = new HttpClient();
			HttpResponseMessage resultCoords = await clientCoords.SendAsync(requestCoords);
			string jsonContentCoords = await resultCoords.Content.ReadAsStringAsync();
			JsonDocument jsonDocumentCoords = JsonDocument.Parse(jsonContentCoords);

			double kelvinDegrees = jsonDocumentCoords.RootElement
				.GetProperty("current")
				.GetProperty("temp")
				.GetDouble();

			string timeZone = jsonDocumentCoords.RootElement.GetProperty("timezone").GetString();


			Console.WriteLine("Температура:  {0:0.00}, date: {1}", kelvinDegrees- 273.15, timeZone);   

			return 0;
		}
	}
}
