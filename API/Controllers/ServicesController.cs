
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsThereAnyoneWho.API.Models;
using IsThereAnyoneWho.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IsThereAnyoneWho.Data;

namespace IsThereAnyoneWho.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceViewModel>>> GetServices()
        {
            var services = await _context.Services
                .Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category,
                    BasePrice = s.BasePrice
                })
                .ToListAsync();

            return services;
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceViewModel>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            var serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Category = service.Category,
                BasePrice = service.BasePrice
            };

            return serviceViewModel;
        }

        // POST: api/Services
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceViewModel>> CreateService([FromBody] CreateServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                BasePrice = model.BasePrice,
                CreatedDate = DateTime.Now
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            var serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Category = service.Category,
                BasePrice = service.BasePrice
            };

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, serviceViewModel);
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Category = model.Category;
            service.BasePrice = model.BasePrice;

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            // Check if service is being used in any provisions
            var isUsed = await _context.Provisions.AnyAsync(p => p.ServiceId == id);

            if (isUsed)
            {
                return BadRequest("Cannot delete service as it is currently in use");
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}