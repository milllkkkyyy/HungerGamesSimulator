using System.Text.Json;
using System.Text.Json.Serialization;

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
    }
}
