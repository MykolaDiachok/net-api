using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_api.Types;

namespace net_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(string id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)] 
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Name is required.");
            }

            bool isUpdate = false;

            if (string.IsNullOrWhiteSpace(category.Id))
            {
                category.Id = Guid.NewGuid().ToString();
                context.Categories.Add(category);
            }
            else
            {
                var existing = await context.Categories.FindAsync(category.Id);
                if (existing is not null)
                {
                    existing.Name = category.Name;
                    context.Entry(existing).State = EntityState.Modified;
                    isUpdate = true;
                }
                else
                {
                    context.Categories.Add(category);
                }
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Problem("Database update failed.");
            }

            if (isUpdate)
            {
                return Ok(category); 
            }

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category); 
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}