using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLServerWebApiConsoleClient.Repositories;
using WebAPIModels.Models;

namespace SQLServerWebApiConsoleClient.Controllers
{
    // базовый адрес: api/customers
    [Route("api/[controller]")]
    [ApiController]
    public class ShippersController : ControllerBase
    {
        //DI конструктор, хранилище задается в Program.cs
        private IShipperRepository repo;
        public ShippersController(IShipperRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/customers
        // GET: api/customers/?country=[country]
        // всегда будет возвращать список клиентов, даже если он пуст
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Shipper>))]
        public async Task<IEnumerable<Shipper>> GetShippers(string? country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return await repo.GetAllAsync();
            }
            else
            {
                return (await repo.GetAllAsync())
                .Where(customer => customer.CompanyName == country);
            }
        }
        // GET: api/customers/[id]
        [HttpGet("{id}", Name = nameof(GetShipper))] // именованный маршрут
        [ProducesResponseType(200, Type = typeof(Shipper))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetShipper(int id)
        {
            Shipper c = await repo.GetAsync(id);
            if (c == null)
            {
                return NotFound(); // 404 Ресурс не обнаружен
            }
            return Ok(c); // 200 Возвращает ОК с клиентом в теле ответа
        }
        // POST: api/customers
        // BODY: Customer (JSON, XML)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Shipper))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Shipper c)
        {
            if (c == null)
            {
                return BadRequest(); // 400 Неверный запрос
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Неверный запрос
            }
            Shipper added = await repo.CreateAsync(c);
            return CreatedAtRoute( // 201 Создано
            routeName: nameof(GetShipper),
            routeValues: new { id = added.ShipperId },
            value: added);
        }
        // PUT: api/customers/[id]
        // BODY: Customer (JSON, XML)
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Shipper c)
        {
            if (c == null || c.ShipperId != id)
            {
                return BadRequest(); // 400 Неверный запрос
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Неверный запрос
            }
            var existing = await repo.GetAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Ресурс не обнаружен
            }
            await repo.UpdateAsync(id, c);
            return new NoContentResult(); // 204 Нет контента
        }
        // DELETE: api/customers/[id]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await repo.GetAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Ресурс не обнаружен
            }
            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value) // короткозамкнутая AND
            {
                return new NoContentResult(); // 204 Нет контента
            }
            else
            {
                return BadRequest( // 400 Неверный запрос
                $"Shipper {id} was found but failed to delete.");
            }
        }
    }
}