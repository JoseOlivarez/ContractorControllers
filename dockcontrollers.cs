using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cascade0.Models;
using Cascade0.Controllers.DataTransferObjects;

namespace Cascade0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocksController : ControllerBase
    {
        private readonly DDSPRODDBContext _context;

        public DocksController()
        {
            _context = new DDSPRODDBContext();
        }

        // GET: api/Docks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dock>>> GetDock()
        {
            return await _context.Dock.ToListAsync();
        }

        // GET: api/Docks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dock>> GetDock(int id)
        {
            var dock = await _context.Dock.FindAsync(id);

            if (dock == null)
            {
                return NotFound();
            }

            return dock;
        }

        // PUT: api/Docks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDock(int id, Dock dock)
        {
            if (id != dock.Dockid)
            {
                return BadRequest();
            }

            _context.Entry(dock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DockExists(id))
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

        // POST: api/Docks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Dock>> PostDock([FromBody] DocksDTO dockDTO)
        {
            Dock dock = new Dock();
            dock.Dockid = dockDTO.Dockid;
            dock.Branchid = dockDTO.Branchid;
            dock.Dockid = dockDTO.Dockid;
            dock.Branchid = dockDTO.Branchid;
            dock.Locid = dockDTO.Locid;
            dock.Company = dockDTO.Company;
            dock.Name = dockDTO.Name;
            dock.Code = dockDTO.Code;
            dock.Brnman = dockDTO.Brnman;
            dock.EnteredDate = dockDTO.Entered;

            dock.Enteredby = dockDTO.Enterby;
            _context.Dock.Add(dock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDock", new { id = dock.Dockid }, dock);
        }

        // DELETE: api/Docks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dock>> DeleteDock(int id)
        {
            var dock = await _context.Dock.FindAsync(id);
            if (dock == null)
            {
                return NotFound();
            }

            _context.Dock.Remove(dock);
            await _context.SaveChangesAsync();

            return dock;
        }

        private bool DockExists(int id)
        {
            return _context.Dock.Any(e => e.Dockid == id);
        }
    }
}
