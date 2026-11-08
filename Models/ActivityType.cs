using System.Text.Json.Serialization;

namespace _4VGymAPIEF.Models
{
    public class ActivityType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int NumberMonitors { get; set; } = 1;

        // Propiedad de navegación inversa (1 Tipo de actividad -> Muchas Actividades)
        [JsonIgnore]
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();




    }

}
