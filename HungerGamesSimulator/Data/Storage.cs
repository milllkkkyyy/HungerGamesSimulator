using System.Diagnostics;
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
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<Weapon>>("https://localhost:7142/data/weapons.json", options);
                Debug.Assert(response != null, "response from client to retrieve weapon data JSON was null");
                return response.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Storage detected error with message, {e.Message}");
                throw;
            }
        }

        public async Task<Dictionary<string, IReadOnlyList<string>>> GetMessageCenterTemplatesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string, IReadOnlyList<string>>>("https://localhost:7142/data/Text.json");
                Debug.Assert(response != null, "response from client to retrieve message template JSON was null");
                return response;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Storage detected error with message, {e.Message}");
                throw;
            }
        }
    }
}
