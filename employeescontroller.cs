using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cascade0.Models;
using Cascade0.Controllers.DataTransferObjects;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Cascade0.Helpers;
using Cascade0.Managers;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Cascade0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly DDSPRODDBContext _context;       
        private readonly IOnPageLoadHelper _onPageLoadHelper;
        private readonly IUserHelper _userHelp;

        public EmployeesController(IUserHelper userHelp, IOnPageLoadHelper onPageLoadHelper)
        {
            _userHelp = userHelp;
            _onPageLoadHelper = onPageLoadHelper;
            _context = new DDSPRODDBContext();
        }
        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            return await _context.Employee.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        /*Returns the branch, and employee titles for the combo boxes
        used on page load*/
        [HttpGet]
        [Route("OnPageLoad")]
        public ActionResult OnPageLoad()
        {
            var branches = _onPageLoadHelper.GetBranches();
            var employeeTitles = _onPageLoadHelper.GetEmployeeTitles();
            var employeeStatuses = _onPageLoadHelper.GetClientStatuses();
            
            var onPageLoad = new
            {
                branches,
                employeeTitles,
                employeeStatuses
            };

            return Ok(onPageLoad);
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Employeeid)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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


        [HttpPut]
        [Route("Edit")]
        public async Task<ActionResult<Employee>> UpdateEmployee([FromBody] EmployeeSignUpDTO employee)
        {
       
           var EmployeeFind = _context.Employee.Find(employee.Employeeid);

           EmployeeFind.BranchId = employee.BranchId;
           EmployeeFind.Vacationdays = employee.Vacationdays;
            if (EmployeeFind != null)
            {
                _context.Employee.Update(EmployeeFind);
            }
                try
                {
            
                await _context.SaveChangesAsync();
                 
            }
                catch (DbUpdateConcurrencyException)
                {
                    Conflict("Issue adding the employee");
                }
                var EmployeeAddress = _context.EmployeeAddressBook.Find(employee.Employeeid);
            if (EmployeeAddress != null)
            {
                EmployeeAddress.Address1 = employee.Address1;
                EmployeeAddress.Address2 = employee.Address2;
                EmployeeAddress.City = employee.City;
                EmployeeAddress.State = employee.State;
                EmployeeAddress.Zip = employee.ZipCode;
                _context.EmployeeAddressBook.Update(EmployeeAddress);
            }          
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Conflict("Issue adding the employee address");
                }
                var EmpInv = _context.EmpInventory.Find(employee.Employeeid);
            if (EmpInv != null)
            {
                EmpInv.Dlexpire = employee.Dlexpire;
                EmpInv.Dlnumber = employee.Dlnumber;
                EmpInv.Inscar = employee.Inscar;
                EmpInv.Insagn = employee.Insagn;
                EmpInv.Insppers = employee.Insepers;
                EmpInv.Inspno = employee.Inspno;
                EmpInv.Insexp = employee.Insexp;
                EmpInv.Insphn = employee.Insphn;
                EmpInv.Vin = employee.Vin;

                _context.EmpInventory.Update(EmpInv);
            } 
            try
            {
                await _context.SaveChangesAsync();
                return Ok("done successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                Conflict("Issue adding the employee inventory items");
            }

            return NoContent();
        }
        [HttpGet]
        [Route("History")]
        public async Task<ActionResult<Employee>> EmployeeHistory([FromBody] EmployeeSignUpDTO employee)
        {
           
                
                var Transactions = await(
                    from EmpTerm in _context.EmpTerm
                    where EmpTerm.Employeeid == employee.Employeeid
                    select EmpTerm).ToListAsync();
                if(Transactions != null)
                return Ok(Transactions);   
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                Conflict("Issue viewing EmployeeHistory");
            
            }
            return NoContent();
        }
        [HttpPost]
        [Route("Terminate")]
        public async Task<ActionResult<Employee>> TerminateEmployee([FromBody] EmployeeSignUpDTO employee)
        {
            var TerminateEmployee = _context.Employee.Find(employee.Employeeid);
            TerminateEmployee.Status = employee.Status;
            if(TerminateEmployee != null)
            _context.Employee.Update(TerminateEmployee);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                Conflict("Issue updating employee status");
            }


            var EmpTerminateEmployee = new EmpTerm {
                Employeeid = employee.Employeeid,
                Lastday = employee.TerminationDate, 
                Sepdate = employee.SeperationDate, 
                Reason = employee.Reason, 
                Rehire = employee.Rehirable, 
                Gene = employee.GeneralReason, 
                Type = employee.Type, 
                Ins = employee.Ins, 
                Comment = employee.Comment
            };
            if(EmpTerminateEmployee != null)
                _context.EmpTerm.Add(EmpTerminateEmployee);

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                Conflict("Issue adding employee to termination history");
                }
            return NoContent();
        }
            // POST: api/Employees
            // To protect from overposting attacks, enable the specific properties you want to bind to, for
            // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        [HttpPost]
        [Route("Post")]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] EmployeeSignUpDTO employeeDTO)
        {

            UserInfo userInfo;

            try
            {
                userInfo = _userHelp.GetUser(User);
            }
            catch (Exception e)
            {
                return Conflict("Error occured processing employee.");
            }
            EmployeeAddressBook address = new EmployeeAddressBook
            {
                Phone1 = employeeDTO.Phone,
                State = employeeDTO.State,
                Address1 = employeeDTO.Address1,
                Address2 = employeeDTO.Address2,
                EmergencyPhone = employeeDTO.EmergencyPhone,
                EnteredDate = DateTime.Now,
                City = employeeDTO.City,
                Zip = employeeDTO.ZipCode,
                Phone2 = employeeDTO.Phone2,
                PrefferedPhone = employeeDTO.Phone,
                Email = employeeDTO.Email,
                Reference1 = employeeDTO.Reference

            };
            if(address != null)
            _context.EmployeeAddressBook.Add(address);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict("There was an issue adding employee to the addressbook.");
            }

            Employee employee = new Employee(); 

            employee.Firstname = employeeDTO.FIRSTNAME;
            employee.Lastname = employeeDTO.LASTNAME;
            employee.Middlename = employeeDTO.MIDDLENAME;
            employee.Vacationdays = employeeDTO.Vacationdays;
            employee.Ssn = employeeDTO.Ssn;
            employee.Homecl = employeeDTO.Homecl;
            employee.Hiredate = employeeDTO.Hiredate;
            employee.Rehiredate = employeeDTO.Rehiredate;
            employee.ModifiedDate = employeeDTO.Modified;
            employee.Anniversary = employeeDTO.Anniversary;
            employee.Pcommt = employeeDTO.Pcommt;
            employee.Startdate = employeeDTO.Startdate;
            employee.Enddate = employeeDTO.Enddate;
            employee.Addressbookid = address.AddressBookId;
            employee.Titleid = employeeDTO.Titleid;
            employee.Status = employeeDTO.Status;
            employee.Enteredby = userInfo.UserId;
            employee.BranchId = employeeDTO.BranchId;
            employee.Modifiedby = 1;
            if(employee != null)
            _context.Employee.Add(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict("Identifier issue: an issue occured while adding this employee.");
            }
            return Ok("New Employee Created at ID: " + employee.Employeeid);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            if(employee!= null)
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Employeeid == id);
        }
    }
}