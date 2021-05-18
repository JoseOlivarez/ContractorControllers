using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cascade0.Models;
using Cascade0.Controllers.DataTransferObjects;
using AutoMapper;

namespace Cascade0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTypesController : ControllerBase
    {
        private readonly DDSPRODDBContext _context;
        private IMapper _mapper;
       

        public ServiceTypesController(IMapper mapper)
        {
            _mapper = mapper;
            _context =new DDSPRODDBContext();
        }

        // GET: api/ServiceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceType>>> GetServiceType()
        {
            return await _context.ServiceType.ToListAsync();
        }

        // GET: api/ServiceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceType>> GetServiceType(int id)
        {
            var serviceType = await _context.ServiceType.FindAsync(id);

            if (serviceType == null)
            {
                return NotFound();
            }

            return serviceType;
        }

        // PUT: api/ServiceTypes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceType(int id, ServiceType serviceType)
        {
            if (id != serviceType.ServiceTypeId)
            {
                return BadRequest();
            }

            _context.Entry(serviceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(id))
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

        // POST: api/ServiceTypes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ServiceType>> PostServiceType([FromBody] ServiceTypeDTO serviceTypeDTO)
        {
            ServiceType serviceType = new ServiceType();
            serviceType.ServiceTypeName = serviceTypeDTO.ServiceTypeName;
            _context.ServiceType.Add(serviceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceType", new { id = serviceType.ServiceTypeId }, serviceType);
        }

        // DELETE: api/ServiceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceType>> DeleteServiceType(int id)
        {
            var serviceType = await _context.ServiceType.FindAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }

            _context.ServiceType.Remove(serviceType);
            await _context.SaveChangesAsync();

            return serviceType;
        }

        private bool ServiceTypeExists(int id)
        {
            return _context.ServiceType.Any(e => e.ServiceTypeId == id);
        }
    }
}
