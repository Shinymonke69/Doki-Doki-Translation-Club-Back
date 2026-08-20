using System.Text.Json.Serialization;
using DdtcApi.Data;
using DdtcApi.Filters;
using DdtcApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DdtcApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/translations/all
        [HttpGet("all")]
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
                return Ok(new { translation = (Translation?)null });
            }

            return Ok(new { translation });
        }

        // POST: api/translations/new
        [HttpPost("new")]
        [ApiKey]
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
                Image = dto.Img,
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

        // POST: api/translations/remove
        [HttpPost("remove")]
        [ApiKey]
        public async Task<IActionResult> Delete([FromBody] TranslationRemoveDto dto)
        {
            var translation = await _context.Translations.FindAsync(dto.Id);
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

        // POST: api/translations/edit
        [HttpPost("edit")]
        [ApiKey]
        public async Task<IActionResult> Edit([FromBody] TranslationEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var translation = await _context.Translations.FindAsync(dto.Id);
            if (translation == null)
            {
                return NotFound(new { success = false, message = "Mod não encontrado." });
            }

            translation.Name = dto.Name;
            translation.Description = dto.Description;
            translation.Banner = dto.Banner;
            translation.Image = dto.Img;
            translation.LinkPc = dto.LinkPc;
            translation.LinkMobile = dto.LinkMobile;

            _context.Translations.Update(translation);
            var changes = await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Mod \"{translation.Name}\" atualizado com sucesso."
            });
        }
    }

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

    public class TranslationRemoveDto
    {
        public int Id { get; set; }
    }

    public class TranslationEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        [JsonPropertyName("linkPC")]
        public string LinkPc { get; set; } = string.Empty;
        public string LinkMobile { get; set; } = string.Empty;
    }
}
