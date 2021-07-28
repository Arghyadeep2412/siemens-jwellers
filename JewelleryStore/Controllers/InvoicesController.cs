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
    public class InvoicesController : Controller
    {
        private InvoicesBLL invoiceBLL = null;
        public InvoicesController()
        {
            invoiceBLL = new InvoicesBLL();
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<BaseResponse>> AddNewInvoice(int userId, [FromBody] NewInvoice newInvoice)
        {
            return await invoiceBLL.AddNewInvoice(userId, newInvoice);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetAllInvoices()
        {
            return await invoiceBLL.GetAllInvoices();
        }

        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<BaseResponse>> GetInvoicesByInvoiceId(int invoiceId)
        {
            return await invoiceBLL.GetInvoicesByInvoiceId(invoiceId);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<BaseResponse>> GetInvoicesForUserId(int userId)
        {
            return await invoiceBLL.GetInvoicesForUserId(userId);
        }
    }
}
