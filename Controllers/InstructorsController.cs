using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace _4VGymAPIEF.Controllers
{
    [ApiController]
    [Route("instructors")]
    public class InstructorsController : ControllerBase
    {
        private readonly InstructorRepository _repository;

        // Inyectamos el repositorio con EF (en 4VGymAPI era InMemoryInstructorRepository)
        public InstructorsController(InstructorRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /instructors, /instructors?id=1, /instructors?first_initial=m
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<InstructorDto>> GetInstructors([FromQuery] int? id, [FromQuery(Name = "first_initial")] string? firstInitial)
        {
            try
            {
                // 1. Búsqueda por ID (retorna una lista de 1 solo elemento)
                if (id.HasValue)
                {
                    InstructorDto? instructor = _repository.GetById(id.Value);
                    if (instructor == null)
                    {
                        return NotFound(new ErrorResponse(2, $"No existe el instructor con ID {id.Value}"));
                    }
                    return Ok(new[] { instructor });
                }

                // 2. Búsqueda por inicial
                if (!string.IsNullOrWhiteSpace(firstInitial))
                {
                    IEnumerable<InstructorDto> instructorsByInitial = _repository.GetByFirstInitial(firstInitial);
                    return Ok(instructorsByInitial);
                }

                // 3. Sin parámetros: devuelve todos los instructores
                IEnumerable<InstructorDto> allInstructors = _repository.GetAll();
                return Ok(allInstructors);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ErrorResponse(1, $"Error interno: {ex.Message}")
                );
            }
        }

        // Expresiones regulares para validaciones (igual que en 4VGymAPI)
        private static readonly Regex SoloNumerosRegex = new Regex(@"^[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@gmail.com", RegexOptions.Compiled);

        /// <summary>
        /// POST /instructors
        /// Añade un nuevo monitor en 4VGym.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public ActionResult<InstructorDto> CreateInstructor([FromBody] InstructorDto instructorNew)
        {
            try
            {
                // Condiciones de barrera (formato de los datos, no necesitan la BBDD)
                if (instructorNew.isEmpty())
                {
                    return BadRequest(new ErrorResponse(3, "Invalid instructor"));
                }

                if (string.IsNullOrWhiteSpace(instructorNew.Name))
                {
                    return BadRequest(new ErrorResponse(4, "The name of the instructor is mandatory"));
                }

                // El teléfono es obligatorio y debe contener solo números
                if (string.IsNullOrWhiteSpace(instructorNew.Phone) || !SoloNumerosRegex.IsMatch(instructorNew.Phone))
                {
                    return BadRequest(new ErrorResponse(4, "The telephone of the instructor is mandatory and must contain only digits"));
                }

                // El email es obligatorio y debe tener formato válido
                if (string.IsNullOrWhiteSpace(instructorNew.Email) || !EmailRegex.IsMatch(instructorNew.Email))
                {
                    return BadRequest(new ErrorResponse(4, "The email is mandatory and must be a valid email address"));
                }

                // La URL de la foto debe ser válida y empezar por http o https
                if (string.IsNullOrWhiteSpace(instructorNew.PhotoUrl) ||
                    !Uri.TryCreate(instructorNew.PhotoUrl, UriKind.Absolute, out Uri? uriResult) ||
                    (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    return BadRequest(new ErrorResponse(4, "The photo URL is invalid. It must start with http:// or https://"));
                }

                // El repositorio comprueba en la BBDD que no se repitan email ni nombre y lo guarda
                InstructorDto? instructor = _repository.Add(instructorNew, out ErrorResponse? errorResponse);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }

                // 201 Created con el instructor guardado, incluido el Id de SQL Server
                return StatusCode(StatusCodes.Status201Created, instructor);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}
