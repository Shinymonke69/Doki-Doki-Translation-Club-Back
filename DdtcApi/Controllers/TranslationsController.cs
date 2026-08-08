using DdtcApi.Data;
using DdtcApi.Filters;
using DdtcApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DdtcApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/translations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var translations = await _context.Translations.OrderBy(t => t.Id).ToListAsync();
            return Ok(new { translations });
        }

        // GET: api/translations/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular()
        {
            var translations = await _context.Translations.OrderBy(t => t.Id).Take(3).ToListAsync();
            return Ok(new { translations });
        }

        // GET: api/translations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var translation = await _context.Translations.FindAsync(id);
            if (translation == null)
            {
                // To maintain compatibility with the weird behavior of the previous API:
                // Previous API returned undefined/null for 'translation' field if not found, or { 1: 1 } if found.
                // We'll return the actual translation object now (fixing the bug), or null.
                return Ok(new { translation = (Translation?)null });
            }

            return Ok(new { translation });
        }

        // POST: api/translations
        [HttpPost]
        [ApiKey] // Protect this route
        public async Task<IActionResult> Create([FromBody] TranslationCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var translation = new Translation
            {
                Name = dto.Name,
                Description = dto.Description,
                Banner = dto.Banner,
                Image = dto.Img, // Node.js expected 'img' in body
                LinkPc = dto.LinkPc,
                LinkMobile = dto.LinkMobile
            };

            _context.Translations.Add(translation);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Mod \"{translation.Name}\" adicionado à database, confira a aba de traduções para garantir que não há erros visuais."
            });
        }

        // DELETE: api/translations/5
        [HttpDelete("{id}")]
        [ApiKey] // Protect this route
        public async Task<IActionResult> Delete(int id)
        {
            var translation = await _context.Translations.FindAsync(id);
            if (translation == null)
            {
                return NotFound(new { success = false, message = "Mod não encontrado." });
            }

            _context.Translations.Remove(translation);
            var changes = await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                changes
            });
        }
    }

    // DTO to map the frontend request body
    public class TranslationCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        [JsonPropertyName("linkPC")]
        public string LinkPc { get; set; } = string.Empty;
        public string LinkMobile { get; set; } = string.Empty;
    }
}
