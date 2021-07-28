using JewelleryStore.BLL;
using JewelleryStore.Models.CodeModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]
    public class CustomerController : Controller
    {
        private CustomerBLL customerBLL = null;
        public CustomerController()
        {
            customerBLL = new CustomerBLL();
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> AddNewCustomer([FromBody] NewCustomer newCustomer)
        {
            return await customerBLL.AddNewCustomer(newCustomer);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetAllCustomers()
        {
            return await customerBLL.GetAllCustomers();
        }

        [HttpGet("{custId}")]
        public async Task<ActionResult<BaseResponse>> GetCustomerById(int custId)
        {
            return await customerBLL.GetCustomerById(custId);
        }
    }
}
