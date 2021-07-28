using JewelleryStore.BLL;
using JewelleryStore.Models.CodeModels;
using JewelleryStore.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RatesController : Controller
    {
        private RatesBLL ratesBll { get; set; }
        public RatesController()
        {
            ratesBll = new RatesBLL();
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> AddRate([FromBody] NewRateDetails newRate)
        {
            return await ratesBll.AddRate(newRate);
        }

        [HttpPut]
        public async Task<ActionResult<BaseResponse>> UpdateRate([FromBody] NewRateDetails newRateDetails)
        {
            return await ratesBll.UpdateRate(newRateDetails);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetAllRates()
        {
            return await ratesBll.GetAllRates();
        }

        [HttpGet("{rateId}")]
        public async Task<ActionResult<BaseResponse>> GetRatesById(int rateId)
        {
            return await ratesBll.GetRatesById(rateId);
        }

        [HttpGet("{itemType}")]
        public async Task<ActionResult<BaseResponse>> GetRatesForItemType(string itemType)
        {
            return await ratesBll.GetRatesForItemType(itemType);
        }

        [HttpDelete("{rateId}")]
        public async Task<ActionResult<BaseResponse>> DeleteRateById(int rateId)
        {
            return await ratesBll.DeleteRateById(rateId);
        }
    }
}
