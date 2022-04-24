using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps")]
    [ApiController]
    [ApiVersion("1.1")]
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public Camps2Controller(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult> Get(bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsAsync(includeTalks);
                var output = new
                {
                    count = result.Count(),
                    data = mapper.Map<CampModel[]>(result)
                };
                return Ok(output);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }


        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> GetCamp(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetCampAsync(moniker, includeTalks);
                if (result == null)
                    return NotFound();
                return mapper.Map<CampModel>(result);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!result.Any())
                    return NotFound();
                return mapper.Map<CampModel[]>(result);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CampModel model)
        {
            try
            {
                // [ApiController] will do this job for you
                //if (!ModelState.IsValid || model == null)
                //    return BadRequest();

                var existingCamp = await campRepository.GetCampAsync(model.Moniker);

                if (existingCamp != null)
                {
                    //ModelState.AddModelError("", "the Moniker must be unique");
                    return BadRequest("the Moniker must be unique");
                }

                var location = linkGenerator.GetPathByAction("GetCamp", "Camps", new { moniker = model.Moniker });

                if (string.IsNullOrEmpty(location))
                    return BadRequest("Moniker is not valid");

                var camp = mapper.Map<Camp>(model);
                campRepository.Add(camp);
                await campRepository.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCamp), new { moniker = model.Moniker }, mapper.Map<CampModel>(camp));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

        }

        [HttpPut("{moniker}")]
        public async Task<IActionResult> Put(string moniker, CampModel model)
        {
            try
            {
                // [ApiController] will do this job for you
                //if (!ModelState.IsValid || model == null)
                //    return BadRequest();

                var existingCamp = await campRepository.GetCampAsync(moniker);

                if (existingCamp == null)
                    return NotFound("the Moniker does not exist");

                mapper.Map(model, existingCamp);
                if (await campRepository.SaveChangesAsync())
                    return Ok(mapper.Map<CampModel>(existingCamp));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp == null)
                    return NotFound("This Camp does not exist");

                campRepository.Delete(camp);
                if (await campRepository.SaveChangesAsync())
                    return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
    }
}
