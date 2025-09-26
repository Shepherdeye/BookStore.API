using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AuthersController : ControllerBase
    {
        private readonly IRepository<Auther> _auhterRepository;

        public AuthersController(IRepository<Auther> auhterRepository)
        {
            _auhterRepository = auhterRepository;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var authers = await _auhterRepository.GetAsync();

            return Ok(authers);

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var auther = await _auhterRepository.GetOneAsync(e => e.Id == id);

            if (auther is null)
                return NotFound();

            return Ok(auther);

        }


        [HttpPost("")]
        public async Task<IActionResult> Create(AutherRequest autherRequest)
        {
            Auther auther = new Auther()
            {
                Name = autherRequest.Name,
            };

            var createdAuther = await _auhterRepository.CreateAsync(auther);
            await _auhterRepository.CommitAsync();

            return Created($"{Request.Scheme}://{Request.Host}/api/admin/authers/{createdAuther.Id}",
                new
                {
                    msg = "created successfully"
                });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, AutherRequest autherRequest)
        {
            var autherDB = await _auhterRepository.GetOneAsync(e => e.Id == id, tracked: false);
            if (autherDB is null)
                return NotFound();

            //auther.Name = autherRequest.Name;
            var auther = autherRequest.Adapt<Auther>();
            auther.Id = id;

            _auhterRepository.Update(auther);
            await _auhterRepository.CommitAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var auther = await _auhterRepository.GetOneAsync(e => e.Id == id);
            if (auther is null) return NotFound();

            _auhterRepository.Delete(auther);
            await _auhterRepository.CommitAsync();
            return NoContent();

        }

    }
}
