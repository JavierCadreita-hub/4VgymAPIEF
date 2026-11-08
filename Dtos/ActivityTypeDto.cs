using System.Text.Json.Serialization;

namespace _4VGymAPIEF.Dtos
{
    public class ActivityTypeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("number-monitors")]
        public int NumberMonitors { get; set; } = 1;

        public ActivityTypeDto()
        {
        }
        public ActivityTypeDto(int id, string name, int numberMonitors)
        {
            Id = id;
            Name = name;
            NumberMonitors = numberMonitors;
        }
    }

}
