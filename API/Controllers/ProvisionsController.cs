
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IsThereAnyoneWho.API.Models;
using IsThereAnyoneWho.Core.Models;
using IsThereAnyoneWho.Data;

namespace IsThereAnyoneWho.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProvisionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProvisionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Provisions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvisionViewModel>>> GetProvisions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var provisions = await _context.Provisions
                .Include(p => p.Service)
                .Include(p => p.Person)
                .Where(p => p.PersonId == userId)
                .Select(p => new ProvisionViewModel
                {
                    Id = p.Id,
                    ServiceId = p.ServiceId,
                    ServiceName = p.Service.Name,
                    RequestDate = p.RequestDate,
                    CompletionDate = p.CompletionDate,
                    FinalPrice = p.FinalPrice,
                    Status = p.Status.ToString(),
                    Notes = p.Notes
                })
                .ToListAsync();

            return provisions;
        }

        // GET: api/Provisions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProvisionViewModel>> GetProvision(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var provision = await _context.Provisions
                .Include(p => p.Service)
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (provision == null)
            {
                return NotFound();
            }

            // Check if user is authorized to view this provision
            if (provision.PersonId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var provisionViewModel = new ProvisionViewModel
            {
                Id = provision.Id,
                ServiceId = provision.ServiceId,
                ServiceName = provision.Service.Name,
                RequestDate = provision.RequestDate,
                CompletionDate = provision.CompletionDate,
                FinalPrice = provision.FinalPrice,
                Status = provision.Status.ToString(),
                Notes = provision.Notes
            };

            return provisionViewModel;
        }

        // POST: api/Provisions
        [HttpPost]
        [Authorize(Roles = "Consumer")]
        public async Task<ActionResult<ProvisionViewModel>> CreateProvision([FromBody] CreateProvisionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var service = await _context.Services.FindAsync(model.ServiceId);

            if (service == null)
            {
                return BadRequest("Invalid service selected");
            }

            var provision = new Provision
            {
                PersonId = userId,
                ServiceId = model.ServiceId,
                RequestDate = DateTime.Now,
                FinalPrice = service.BasePrice, // Initially set to base price
                Status = ProvisionStatus.Pending,
                Notes = model.Notes
            };

            _context.Provisions.Add(provision);
            await _context.SaveChangesAsync();

            var provisionViewModel = new ProvisionViewModel
            {
                Id = provision.Id,
                ServiceId = provision.ServiceId,
                ServiceName = service.Name,
                RequestDate = provision.RequestDate,
                FinalPrice = provision.FinalPrice,
                Status = provision.Status.ToString(),
                Notes = provision.Notes
            };

            return CreatedAtAction(nameof(GetProvision), new { id = provision.Id }, provisionViewModel);
        }

        // PUT: api/Provisions/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateProvisionStatus(int id, [FromBody] UpdateProvisionStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var provision = await _context.Provisions.FindAsync(id);
            
            if (provision == null)
            {
                return NotFound();
            }

            // Check if user is authorized to update this provision
            if (provision.PersonId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Validate status transition
            if (!IsValidStatusTransition(provision.Status, model.Status, userRole))
            {
                return BadRequest("Invalid status transition");
            }

            provision.Status = model.Status;
            
            if (model.Status == ProvisionStatus.Completed)
            {
                provision.CompletionDate = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(model.Notes))
            {
                provision.Notes = model.Notes;
            }

            _context.Entry(provision).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProvisionExists(id))
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

        private bool ProvisionExists(int id)
        {
            return _context.Provisions.Any(e => e.Id == id);
        }

        private bool IsValidStatusTransition(ProvisionStatus currentStatus, ProvisionStatus newStatus, string userRole)
        {
            // Define valid transitions based on user role and current status
            if (userRole == "Consumer")
            {
                switch (currentStatus)
                {
                    case ProvisionStatus.Pending:
                        return newStatus == ProvisionStatus.Cancelled;
                    case ProvisionStatus.Accepted:
                        return newStatus == ProvisionStatus.Cancelled;
                    case ProvisionStatus.Completed:
                        return false; // Consumers can't change completed services
                    case ProvisionStatus.InProgress:
                        return newStatus == ProvisionStatus.Cancelled;
                    default:
                        return false;
                }
            }
            else if (userRole == "Provider")
            {
                switch (currentStatus)
                {
                    case ProvisionStatus.Pending:
                        return newStatus == ProvisionStatus.Accepted || newStatus == ProvisionStatus.Cancelled;
                    case ProvisionStatus.Accepted:
                        return newStatus == ProvisionStatus.InProgress || newStatus == ProvisionStatus.Cancelled;
                    case ProvisionStatus.InProgress:
                        return newStatus == ProvisionStatus.Completed || newStatus == ProvisionStatus.Cancelled;
                    default:
                        return false;
                }
            }
            
            return false;
        }
    }
}
