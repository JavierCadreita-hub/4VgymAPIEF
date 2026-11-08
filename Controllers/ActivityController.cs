using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _4VGymAPIEF.Controllers
{
    [ApiController]
    [Route("activities")]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityRepository _repository;

        // Inyectamos el repositorio con EF (en 4VGymAPI era InMemoryActivityRepository)
        public ActivityController(ActivityRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST /activities
        /// Añade una nueva actividad en 4VGym.
        /// </summary>
        /// <param name="activity">Actividad enviada en el cuerpo de la petición (JSON).</param>
        [HttpPost]
        [ProducesResponseType(typeof(ActivityDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<ActivityDto> CreateActivity([FromBody] ActivityDto activity)
        {
            try
            {
                // Las validaciones de formato (aforo, fechas, lista de monitores) ya las han hecho
                // las DataAnnotations del DTO antes de llegar aquí.
                // Las que necesitan la BBDD (que exista el tipo y los monitores) las hace el repositorio.
                ActivityDto? activityCreated = _repository.Add(activity, out List<ErrorResponse> errors);
                if (activityCreated == null)
                {
                    return BadRequest(errors);
                }

                // 201 Created con la actividad guardada, incluido el Id de SQL Server
                return StatusCode(StatusCodes.Status201Created, activityCreated);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}
