using JewelleryStore.HelperMethods;
using JewelleryStore.Models.CodeModels;
using JewelleryStore.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.BLL
{
    public class RegistrationBLL
    {
        private sakilaContext dbContext = null;
        public RegistrationBLL()
        {
            dbContext = new sakilaContext();
        }
        public async Task<bool> CheckUserExistsForUserName(string userName)
        {
            UsersLoginCred userLoginRecord = dbContext.UsersLoginCreds.AsQueryable()
                                                            .Where(row => row.UserName.Equals(userName))
                                                            .Select(row => row).FirstOrDefault();

            if(userLoginRecord != null && userLoginRecord.UserId > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<BaseResponse> VerifyLogin(NewLogin newLogin)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            if (String.IsNullOrEmpty(newLogin.UserName) || String.IsNullOrEmpty(newLogin.Password))
            {
                resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                resp.Message = "Either username or pswd or both are empty";
                resp.Status = ResponseStatus.Error;
                return resp;
            }
            try
            {
                // get the user with given username
                newLogin.UserName = newLogin.UserName.ToLower();
                UsersLoginCred userLoginRecord = dbContext.UsersLoginCreds.AsQueryable()
                                                                .Where(row => row.UserName.Equals(newLogin.UserName))
                                                                .Select(row => row).FirstOrDefault();
                if(userLoginRecord == null)
                {
                    resp.ErrorCode = ErrorCode.USER_DOES_NOT_EXIST;
                    resp.Status = ResponseStatus.Error;
                    resp.Message += "There is no such user name";
                    return resp;
                }
                bool isPswdMatching = PasswordHelper.VerifyPassword(newLogin.Password, userLoginRecord.Password);
                if(!isPswdMatching)
                {
                    resp.ErrorCode = ErrorCode.PASSWORD_DOES_NOT_MATCH;
                    resp.Status = ResponseStatus.Error;
                    resp.Message += "Password does not match";
                    return resp;
                }
                resp.Data = new
                {
                    UserId = userLoginRecord.UserId
                };
                resp.Status = ResponseStatus.Success;
                resp.Message = "User verified";
            }
            catch(Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
        public async Task<BaseResponse> ResetLoginCreds(int loggedInUserId, ResetLoginCreds newCreds)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            if (String.IsNullOrEmpty(newCreds.Email) || String.IsNullOrEmpty(newCreds.UserName) || String.IsNullOrEmpty(newCreds.Password))
            {
                resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                resp.Message = "Either username or pswd or email or all are empty";
                resp.Status = ResponseStatus.Error;
                return resp;
            }
            try
            {
                Customer user = dbContext.Customers.AsQueryable()
                                            .Where(row => row.Email.Equals(newCreds.Email) && row.IsActive.Value)
                                            .Select(row => row).FirstOrDefault();
                if(user == null || user.UserId != loggedInUserId)
                {
                    // you can change creds of your account only
                    resp.Message = "you can change creds of your account only";
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }
                newCreds.UserName = newCreds.UserName.ToLower();
                UsersLoginCred creds = dbContext.UsersLoginCreds.AsQueryable()
                                                                .Where(row => row.UserName.Equals(newCreds.UserName) && row.UserId == loggedInUserId)
                                                                .Select(row => row).FirstOrDefault();
                creds.UserName = newCreds.UserName;
                creds.Password = PasswordHelper.HashPassword(newCreds.Password);
                dbContext.SaveChanges();
                
                resp.Data = new
                {
                    NewUserName = creds.UserName,
                    NewPassword = newCreds.Password
                };
                resp.Message = "Updated creds successfully";
                resp.Status = ResponseStatus.Success;
            }
            catch(Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
    }
}
