using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cascade0.Models;
using Cascade0.Controllers.DataTransferObjects;
using System.Text.Json;
using AutoMapper;
using Cascade0.Controllers.MappingProfile;
using Cascade0.Core;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Cascade0.Helpers;
using Cascade0.Managers;

namespace Cascade0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly DDSPRODDBContext _context;
        private IMapper _mapper;
        private readonly IUnitOfWork wow;
        private readonly IUserHelper _userHelper;
        private UserInfo userInfo;
        public BranchesController( IMapper mapper)
        {
            _mapper = mapper;
            _context = new DDSPRODDBContext();
        }
        public IEnumerable<BranchDTO> Branch()
        {
            var model = _context.Branch.ToList();
           
            //  var config = new MapperConfiguration(m => m.CreateMap<Branch, BranchDTO>());
            //    var mapper = new Mapper(config);
            return  _mapper.Map<List<Branch>, List<BranchDTO>>(model);
           
             
        }
        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranch()
        {
            return await _context.Branch.ToListAsync();
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(int id)
        {
            var branch = await _context.Branch.FindAsync(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }
        [HttpGet]
        [Route("GetBranchInfo")]
        public async Task<ActionResult<object>> GetBranchInformation(int BranchId)
        {
            var Territories = (
               from TerritoryBranch in _context.BranchTerritory
               join branchs in _context.Branch on TerritoryBranch.BranchId equals branchs.Branchid
               join ters in _context.Territory on TerritoryBranch.TerritoryId equals ters.TerritoryId
               select TerritoryBranch
               ).ToList();
           
            var SettlementApproverName = (
                 from settlementapprover in _context.SettlementApprover
                 join branchs in _context.Branch on BranchId equals branchs.Branchid
                 select settlementapprover
                 ).ToList();

            var AccountManagers = (
                from managers in _context.AccountManager
                join branchs in _context.Branch on BranchId equals branchs.Branchid
                select managers
                ).ToList();

            var reserve = new { Territories, SettlementApproverName, AccountManagers };




            return Ok(reserve) ;
        }




        // PUT: api/Branches/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, Branch branch)
        {
            if (id != branch.Branchid)
            {
                return BadRequest();
            }

            _context.Entry(branch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
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

        // POST: api/Branches
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.private readonly IMapper _mapper;
      
        public IActionResult GetUser()
        {
            Branch branch = new Branch();
            var userDTO = _mapper.Map<BranchDTO>(branch);
            return Ok();
        }
        [HttpPost]
        [Route("PostTerritory")]
        public async Task<ActionResult<BranchDTO>> PostTerritory([FromBody] BranchDTO ter)
        {
            Territory territory = new Territory();
            territory.TerritoryId = ter.TerritoryId;
            territory.TerritoryName = ter.TerritoryName;
            territory.TerritoryDescription = ter.TerritoryDescription;
            _context.Territory.Add(territory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbException)
            {
                
                    return Conflict();
           
            }
            Branch branch = (
                            from branches in _context.Branch
                            where branches.Branchid == ter.Branchid
                            select branches).SingleOrDefault();
            branch.TerritoryId = territory.TerritoryId;
            _context.SaveChanges();
            return ter;
        }

        [Route("PostSettlementApprover")]
        public async Task<ActionResult<BranchDTO>> PostSettlementApprover([FromBody] BranchDTO ter)
        {
            SettlementApprover SA = new SettlementApprover();
            SA.SettlementApproverId = ter.SettlementApproverId;
            SA.BranchId = ter.Branchid;
            SA.EmployeeId = ter.SettlementApproverEmployeeId;
            _context.SettlementApprover.Add(SA);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbException)
            {

                return Conflict();

            }
            return ter;
        }

        [Route("PostAccountManager")]
        public async Task<ActionResult<BranchDTO>> PostAccountManager([FromBody] BranchDTO ter)
        {
            AccountManager am = new AccountManager();
            am.AccountManagerId = ter.AccountManagerId;
            am.EmployeeId = Convert.ToInt32(ter.AccountManagerEmployeeId);
            am.BranchId = ter.Branchid;
            _context.AccountManager.Add(am);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbException)
            {

                return Conflict();

            }
            return ter;
        }


        [Route("Post")]
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch([FromBody] JsonElement json)
        {
            Branch mainBranch = new Branch();
            BranchDTO branch = new BranchDTO();
            string jsonString = System.Text.Json.JsonSerializer.Serialize(json);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            branch = JsonConvert.DeserializeObject<BranchDTO>(jsonString, settings);
            var userDTO = _mapper.Map<BranchDTO>(mainBranch); 
            //Branch();
            //GetUser();
            mainBranch.Branchid = branch.Branchid;

            //branch setup information
            mainBranch.Name = branch.Name;
            mainBranch.State = branch.State;
            mainBranch.Autogen = branch.Autogen;
            mainBranch.Codtrn = branch.Codtrn;
            mainBranch.Codml = branch.Codml;
            mainBranch.Gldivision = branch.Gldivision;
            mainBranch.Region = branch.Region;
            mainBranch.Company = branch.Company;
            //Noe: Please change name and type to int for Enterby
            //mainBranch.Enteredby = branch.Enterby;
            mainBranch.Glregion = branch.Glregion;
            mainBranch.PayPeriodInitialDate = branch.PayPeriodInitalDate;
            mainBranch.SettlementDate = branch.SettlementDate;
            mainBranch.ServiceDedicatedNetwork = branch.ServiceDedicatedNetwork;
            mainBranch.ServiceHealthcare = branch.ServiceHealthCare;
            mainBranch.ServiceHotShot = branch.ServiceHotShot;
            mainBranch.Status = branch.Status;
            mainBranch.Rgman = branch.Rgman;
            mainBranch.Bracnt = branch.Branct;
            mainBranch.EnteredDate = DateTime.Now;
           
            UserInfo userInfo = new UserInfo(); 
            mainBranch.Enteredby = userInfo.UserId;
            mainBranch.Modifiedby = 1;
       
            mainBranch.Afee = branch.Afee;




            _context.Branch.Add(mainBranch);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BranchExists(branch.Branchid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            Territory territory = new Territory();
            SettlementApprover sa = new SettlementApprover();
            AccountManager am = new AccountManager();
            if (territory != null)
            {
                territory.TerritoryId = branch.TerritoryId;
                territory.TerritoryName = branch.TerritoryName;
                territory.TerritoryDescription = branch.TerritoryDescription;
                territory.Enteredby = userInfo.UserId;
                territory.Modifiedby = 1;
                territory.ModifiedDate = DateTime.Now;
                territory.EnteredDate = DateTime.Now;


                _context.Territory.Add(territory);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (BranchExists(branch.Branchid))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
              
            }


            if (sa != null)
            {
                sa.SettlementApproverId = branch.SettlementApproverId;
                sa.EmployeeId = branch.SettlementApproverEmployeeId;
                sa.BranchId = branch.Branchid;
                sa.Enteredby = userInfo.UserId;
                sa.Modifiedby = 1;
                sa.ModifiedDate = DateTime.Now;
                sa.EnteredDate = DateTime.Now;
                _context.SettlementApprover.Add(sa);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (BranchExists(branch.Branchid))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
              
            }
            if (am != null)
            {
                am.AccountManagerId = branch.AccountManagerId;
                am.EmployeeId = branch.AccountManagerEmployeeId;
                am.BranchId = branch.Branchid;
                am.Enteredby = userInfo.UserId;
                am.Modifiedby = 1;
                am.ModifiedDate = DateTime.Now;
                am.EnteredDate = DateTime.Now;
                _context.AccountManager.Add(am);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (BranchExists(branch.Branchid))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return CreatedAtAction("GetBranch", new { id = branch.Branchid }, branch);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Branch>> DeleteBranch(int id)
        {
            var branch = await _context.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branch.Remove(branch);
            await _context.SaveChangesAsync();

            return branch;
        }

        private bool BranchExists(int id)
        {
            return _context.Branch.Any(e => e.Branchid == id);
        }
    }
}
