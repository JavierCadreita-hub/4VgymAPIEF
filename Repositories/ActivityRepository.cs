using _4VGymAPIEF.Data;
using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Models;

namespace _4VGymAPIEF.Repositories
{
    public class ActivityRepository
    {
        // En 4VGymAPI este repositorio usaba los otros dos repositorios en memoria;
        // ahora todo se consulta en la BBDD a través del DbContext
        private readonly GymDbContext _context;

        public ActivityRepository(GymDbContext context)
        {
            _context = context;
        }

        // POST /activities: valida el tipo y los monitores y guarda la actividad
        public ActivityDto? Add(ActivityDto dto, out List<ErrorResponse> errorResponse)
        {
            errorResponse = new List<ErrorResponse>();

            // Validación 1: que exista el tipo de actividad
            ActivityType? activityType = _context.ActivityTypes
                .SingleOrDefault(at => at.Id == dto.ActivityTypeId);

            if (activityType == null)
            {
                errorResponse.Add(new ErrorResponse(34, "El tipo de actividad no existe"));
            }

            // Validación 2: que existan todos los monitores
            // Traemos de la BBDD los instructores cuyos Id vienen en la lista
            List<Instructor> instructors = _context.Instructors
                .Where(i => dto.InstructorsId.Contains(i.Id))
                .ToList();

            // Los Id que no han salido de la BBDD es que no existen
            foreach (int instructorId in dto.InstructorsId)
            {
                if (!instructors.Any(i => i.Id == instructorId))
                {
                    errorResponse.Add(new ErrorResponse(35, "El instructor: " + instructorId + " no existe"));
                }
            }

            if (errorResponse.Count > 0)
            {
                return null;
            }

            // Del DTO al modelo de BBDD: en vez de la lista de Id, le pasamos
            // el tipo y los instructores (objetos) a las propiedades de navegación
            Activity activity = new Activity
            {
                ActivityType = activityType,
                MaxParticipants = dto.MaxParticipants,
                DateStart = dto.DateStart,
                DateEnd = dto.DateEnd,
                Instructors = instructors
            };

            _context.Activities.Add(activity);
            _context.SaveChanges(); // INSERT en Activities y en ActivityInstructor

            // Devolvemos el DTO con el Id que ha generado SQL Server
            dto.Id = activity.Id;
            return dto;
        }
    }
}
