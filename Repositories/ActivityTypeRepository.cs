using _4VGymAPIEF.Data;
using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Models;

namespace _4VGymAPIEF.Repositories
{
    public class ActivityTypeRepository
    {
        // El DbContext es la conexión con la BBDD (en 4VGymAPI aquí había una List)
        private readonly GymDbContext _context;

        public ActivityTypeRepository(GymDbContext context)
        {
            _context = context;
        }

        // GET: lee los tipos de la BBDD y los convierte en DTOs
        public IEnumerable<ActivityTypeDto> GetAll()
        {
            return _context.ActivityTypes
                .Select(at => new ActivityTypeDto(at.Id, at.Name, at.NumberMonitors))
                .ToList();
        }

        // Busca un tipo por nombre sin distinguir mayúsculas (para no repetir nombres)
        public ActivityTypeDto? GetByName(string name)
        {
            string nameLower = name.ToLower();
            return _context.ActivityTypes
                .Where(at => at.Name.ToLower() == nameLower)
                .Select(at => new ActivityTypeDto(at.Id, at.Name, at.NumberMonitors))
                .SingleOrDefault();
        }

        // POST: convierte el DTO en modelo de BBDD y lo guarda
        public ActivityTypeDto Add(ActivityTypeDto dto)
        {
            ActivityType activityType = new ActivityType
            {
                Name = dto.Name,
                NumberMonitors = dto.NumberMonitors
            };

            _context.ActivityTypes.Add(activityType);
            _context.SaveChanges(); // aquí se hace el INSERT y SQL Server genera el Id

            return new ActivityTypeDto(activityType.Id, activityType.Name, activityType.NumberMonitors);
        }
    }
}