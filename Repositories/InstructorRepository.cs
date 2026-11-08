using _4VGymAPIEF.Data;
using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Models;

namespace _4VGymAPIEF.Repositories
{
    public class InstructorRepository
    {
        // En 4VGymAPI aquí había un Dictionary; ahora los datos están en la BBDD
        private readonly GymDbContext _context;

        public InstructorRepository(GymDbContext context)
        {
            _context = context;
        }

        // GET /instructors: todos los instructores
        public IEnumerable<InstructorDto> GetAll()
        {
            return _context.Instructors
                .Select(i => new InstructorDto(i.Id, i.Name, i.Email, i.PhotoUrl, i.Phone))
                .ToList();
        }

        // GET /instructors?id=1: un instructor por su Id (null si no existe)
        public InstructorDto? GetById(int id)
        {
            return _context.Instructors
                .Where(i => i.Id == id)
                .Select(i => new InstructorDto(i.Id, i.Name, i.Email, i.PhotoUrl, i.Phone))
                .SingleOrDefault();
        }

        // GET /instructors?first_initial=m: los que empiezan por esa inicial
        public IEnumerable<InstructorDto> GetByFirstInitial(string? firstInitial = null)
        {
            // Sin inicial devolvemos todos (igual que en 4VGymAPI)
            if (string.IsNullOrWhiteSpace(firstInitial))
            {
                return GetAll();
            }

            string initialLower = firstInitial.ToLower();
            return _context.Instructors
                .Where(i => i.Name.ToLower().StartsWith(initialLower))
                .Select(i => new InstructorDto(i.Id, i.Name, i.Email, i.PhotoUrl, i.Phone))
                .ToList();
        }

        // POST /instructors: comprueba que no se repita el email ni el nombre y lo guarda
        public InstructorDto? Add(InstructorDto dto, out ErrorResponse? errorResponse)
        {
            errorResponse = null;

            string emailLower = dto.Email.ToLower();
            string nameLower = dto.Name.ToLower();
            bool exists = _context.Instructors
                .Any(i => i.Email.ToLower() == emailLower || i.Name.ToLower() == nameLower);

            if (exists)
            {
                errorResponse = new ErrorResponse(34, "An instructor with the same email or name already exists");
                return null;
            }

            // Del DTO al modelo de BBDD. El Id no se pone: lo genera SQL Server
            Instructor instructor = new Instructor
            {
                Name = dto.Name,
                Email = dto.Email,
                PhotoUrl = dto.PhotoUrl,
                Phone = dto.Phone
            };

            _context.Instructors.Add(instructor);
            _context.SaveChanges(); // INSERT en la tabla Instructors

            return new InstructorDto(instructor.Id, instructor.Name, instructor.Email, instructor.PhotoUrl, instructor.Phone);
        }
    }
}