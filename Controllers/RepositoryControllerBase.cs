using Microsoft.AspNetCore.Mvc;
using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    public abstract class RepositoryControllerBase<T> : ControllerBase where T : IEntity
    {
        protected readonly IRepository<T> Repository;
        protected RepositoryControllerBase(IRepository<T> repository)
        {
            Repository = repository;
        }

        [HttpGet]
        public virtual ActionResult<IEnumerable<T>> GetAll() => Ok(Repository.GetAll());

        [HttpGet("{id}")]
        public virtual ActionResult<T> GetById(int id)
        {
            var entity = Repository.GetById(id);

            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public virtual ActionResult<T> Create(T entity)
        {
            var created = Repository.Add(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update(int id, T entity)
        {
            if (id != entity.Id) return BadRequest("Route ID and body ID must match.");

            return Repository.Update(entity) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id) => Repository.Delete(id) ? NoContent() : NotFound();
    }
}