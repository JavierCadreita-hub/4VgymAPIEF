using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _4VGymAPIEF.Controllers
{
    [ApiController]
    [Route("activity-types")]
    public class ActivityTypesController : ControllerBase
    {
        private readonly ActivityTypeRepository _repository;

        // Inyectamos el repositorio con EF (en 4VGymAPI era InMemoryActivityTypeRepository)
        public ActivityTypesController(ActivityTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /activity-types
        /// Devuelve el listado de tipos de actividades de 4VGym.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ActivityTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ActivityTypeDto>> FindActivityTypes()
        {
            try
            {
                IEnumerable<ActivityTypeDto> activityTypes = _repository.GetAll();

                if (activityTypes == null || activityTypes.Count() == 0)
                {
                    return BadRequest(new ErrorResponse(2, "No activity types found"));
                }

                return Ok(activityTypes);
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse(1, "Any problem in the Server"));
            }
        }

        /// <summary>
        /// POST /activity-types
        /// Añade un nuevo tipo de actividad en 4VGym.
        /// </summary>
        /// <param name="activityType">Tipo de actividad enviado en el cuerpo de la petición (JSON).</param>
        [HttpPost]
        [ProducesResponseType(typeof(ActivityTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<ActivityTypeDto> CreateActivityType([FromBody] ActivityTypeDto activityType)
        {
            try
            {
                // 1. Condiciones de barrera: objeto no nulo y nombre obligatorio
                if (activityType == null)
                {
                    return BadRequest(new ErrorResponse(3, "Invalid activity type data"));
                }
                if (String.IsNullOrEmpty(activityType.Name))
                {
                    return BadRequest(new ErrorResponse(4, "The name of the activity is mandatory"));
                }

                // No puede haber tipos de actividad con el mismo nombre (se consulta en la BBDD)
                if (_repository.GetByName(activityType.Name) != null)
                {
                    return BadRequest(new ErrorResponse(5, "The name is unique, there is already another ActivityType with the same Name"));
                }

                // 2. Guardar en la BBDD usando el repositorio
                ActivityTypeDto createdActivityType = _repository.Add(activityType);

                // 3. Respuesta 201 Created con el objeto creado (con el Id de SQL Server)
                return StatusCode(StatusCodes.Status201Created, createdActivityType);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}
