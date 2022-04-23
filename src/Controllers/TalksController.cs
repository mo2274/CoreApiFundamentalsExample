using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var talks = await repository.GetTalksByMonikerAsync(moniker, includeSpeakers);

                return Ok(mapper.Map<TalkModel[]>(talks));
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Faild to retrive talks");
            }

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var talk = await repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);

                if (talk == null)
                    return NotFound();

                return Ok(mapper.Map<TalkModel>(talk));
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Faild to retrive talk");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(TalkModel model, string moniker)
        {
            try
            {
                if (model.Speaker == null)
                    return BadRequest("the speaker is required");

                var camp = await repository.GetCampAsync(moniker);
                if (camp == null)
                    return BadRequest("this camp does not exist");

                var speaker = await repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null)
                    return BadRequest("this speaker does not exist");

                var talk = mapper.Map<Talk>(model);
                talk.Camp = camp;
                talk.Speaker = speaker;

                repository.Add<Talk>(talk);

                await repository.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { moniker = moniker, id = talk.TalkId }, mapper.Map<TalkModel>(talk));
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Faild to add talk");
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(TalkModel model, string moniker, int id)
        {
            try
            {
                var existingTalk = await repository.GetTalkByMonikerAsync(moniker, id);
                if (existingTalk == null)
                    return NotFound();

                mapper.Map(model, existingTalk);

                if (model.Speaker == null)
                    return BadRequest("the speaker is required");

                var speaker = await repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null)
                    return BadRequest("this speaker does not exist");

                existingTalk.Speaker = speaker;

                if (await repository.SaveChangesAsync())
                    return Ok(mapper.Map<TalkModel>(existingTalk));
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Faild to add talk");
            }

            return BadRequest();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var existingTalk = await repository.GetTalkByMonikerAsync(moniker, id);
                if (existingTalk == null)
                    return NotFound();

                repository.Delete(existingTalk);
                await repository.SaveChangesAsync();
                return Ok();
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Faild to delete talk");
            }

        }
    }
}
