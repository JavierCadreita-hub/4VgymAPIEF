using System.Text.Json.Serialization;

namespace _4VGymAPIEF.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhotoUrl { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        //// Relación M:N con Activity
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    }
}
