using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Test_Ef_Relationship.Repository;

namespace Test_Ef_Relationship.Controllers
{
    [Route("api/[controller]")]
    public class ComputerCrudController : Controller
    {
        private readonly ComputerRepository _computerRepository;
        private readonly IFixture _fixture;

        public ComputerCrudController(ComputerRepository computerRepository, IFixture fixture)
        {
            _computerRepository = computerRepository;
            _fixture = fixture;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ComputerEntity dbComputer)
        {
            var computerEntity = _computerRepository.CreateOrUpdate(dbComputer);
            return Ok(computerEntity);
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_fixture.Create<DbComputerEntity>());
        }
    }
}