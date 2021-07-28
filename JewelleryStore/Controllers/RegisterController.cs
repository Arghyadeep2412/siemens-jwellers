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
    public class RegisterController : Controller
    {
        private RegistrationBLL registrationBll = null;

        public RegisterController()
        {
            registrationBll = new RegistrationBLL();
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> VerifyLogin([FromBody] NewLogin newLogin)
        {
            return await registrationBll.VerifyLogin(newLogin);
        }

        // to reset username and/or pswd
        [HttpPut("{userId}")]
        public async Task<ActionResult<BaseResponse>> ResetLoginCreds(int userId, [FromBody] ResetLoginCreds newCreds)
        {
            // the user can only update it's own creds
            return await registrationBll.ResetLoginCreds(userId, newCreds);
        }
    }
}
