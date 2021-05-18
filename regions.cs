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
    public class RegionsController : ControllerBase
    {
        private readonly DDSPRODDBContext _context;

        public RegionsController()
        {
            _context = new DDSPRODDBContext();
        }

        // GET: api/Regions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Regions>>> GetRegions()
        {
            return await _context.Regions.ToListAsync();
        }

        // GET: api/Regions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Regions>> GetRegions(int id)
        {
            var regions = await _context.Regions.FindAsync(id);

            if (regions == null)
            {
                return NotFound();
            }

            return regions;
        }

        // PUT: api/Regions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegions(int id, Regions regions)
        {
            if (id != regions.Region)
            {
                return BadRequest();
            }

            _context.Entry(regions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionsExists(id))
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

        // POST: api/Regions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Regions>> PostRegions([FromBody] RegionDTO regionsDTO)
        {
            Regions regions = new Regions();
            regions.Region = regionsDTO.Region;
            regions.Rname = regionsDTO.Rname;
            regions.Ckname = regionsDTO.Ckname;
            regions.Opnotes = regionsDTO.Opnotes;
            regions.Stat = regionsDTO.Stat;
            regions.ModifiedDate = regionsDTO.Moded;
            regions.Modifiedby = regionsDTO.Modby;
            regions.Archived = regionsDTO.Archived;
            _context.Regions.Add(regions);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RegionsExists(regions.Region))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRegions", new { id = regions.Region }, regions);
        }

        // DELETE: api/Regions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Regions>> DeleteRegions(int id)
        {
            var regions = await _context.Regions.FindAsync(id);
            if (regions == null)
            {
                return NotFound();
            }

            _context.Regions.Remove(regions);
            await _context.SaveChangesAsync();

            return regions;
        }

        private bool RegionsExists(int id)
        {
            return _context.Regions.Any(e => e.Region == id);
        }
    }
}
