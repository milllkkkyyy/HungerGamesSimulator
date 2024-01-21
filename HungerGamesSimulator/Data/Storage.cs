using HungerGamesSimulator.MessageCenter;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace HungerGamesSimulator.Data
{
    public class Storage
    {
        private HttpClient _httpClient;

        public Storage( HttpClient httpClient )
        {
            _httpClient = httpClient;
        }

        public async Task<List<Weapon>> GetWeaponsAsync()
        {
            JsonSerializerOptions options = new( JsonSerializerDefaults.Web )
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };

            var response = await _httpClient.GetFromJsonAsync<List<Weapon>>( "https://localhost:7142/data/weapons.json", options );
            System.Diagnostics.Debug.Assert( response != null );
            return response;
        }

        public async Task<IReadOnlyList<GameString>> GetServiceGameStrings()
        {
            JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };

            var fileNames = await _httpClient.GetFromJsonAsync<List<string>>($"https://localhost:7142/data/designerstrings/Files.json");
            System.Diagnostics.Debug.Assert(fileNames != null, $"Failed to retrieve data from Files.json");

            List<DesignerString> serviceStrings = new();
            foreach (var service in fileNames)
            {
                var response = await _httpClient.GetFromJsonAsync<List<DesignerString>>($"https://localhost:7142/data/designerstrings/{service}", options);
                System.Diagnostics.Debug.Assert(response != null, $"Failed to retrieve data from {service}");
                serviceStrings.AddRange( response );
            }

            var conversion = MessageCenterUtils.DesignerStringToGameString(serviceStrings);
            return conversion;
        }
    }
}
