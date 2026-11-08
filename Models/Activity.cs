using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using _4VGymAPIEF.Validations;

namespace _4VGymAPIEF.Models
{
    public class Activity
    {
        public int Id { get; set; }

        public int? ActivityTypeId { get; set; }

        // Aquí ya no está InstructorsId: esa lista de ids es de la API y vive en ActivityDto.
        // En la BBDD los monitores de cada actividad salen de la navegación Instructors (tabla ActivityInstructor).

        public int MaxParticipants { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        //Activity // Relación M:N con Instructor
        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();


        [ForeignKey(nameof(ActivityTypeId))]

        [JsonIgnore]
        public ActivityType? ActivityType { get; set; }
    }

}
