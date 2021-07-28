using JewelleryStore.HelperMethods;
using JewelleryStore.Models.CodeModels;
using JewelleryStore.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.BLL
{
    public class CustomerBLL
    {
        private sakilaContext dbContext = null;
        private RegistrationBLL registrationBll = null;
        public CustomerBLL()
        {
            dbContext = new sakilaContext();
            registrationBll = new RegistrationBLL();
        }

        private bool CheckCustomerObj(ref NewCustomer newCustomer, ref BaseResponse resp)
        {
            bool dataIsOkay = true;
            if(String.IsNullOrEmpty(newCustomer.UserName))
            {
                dataIsOkay = false;
                resp.Message += "Username is not present. ";
                return dataIsOkay;
            }
            if(String.IsNullOrEmpty(newCustomer.Email))
            {
                dataIsOkay = false;
                resp.Message += "Email is not present. ";
                return dataIsOkay;
            }
            if(String.IsNullOrEmpty(newCustomer.FirstName))
            {
                dataIsOkay = false;
                resp.Message += "FristName is not present. ";
                return dataIsOkay;
            }
            if(String.IsNullOrEmpty(newCustomer.Password))
            {
                dataIsOkay = false;
                resp.Message += "Password is not present. ";
                return dataIsOkay;
            }
            if(!newCustomer.CustomerType.HasValue)
            {
                newCustomer.CustomerType = Globals.NormalCustomer;
            }
            return dataIsOkay;
        }
        public async Task<bool> CheckUserExistsForEmail(string email)
        {
            bool userPresent = false;
            var user = dbContext.Customers.AsQueryable()
                                            .Where(row => row.Email.Equals(email) && row.IsActive.Value)
                                            .Select(row => row).FirstOrDefault();
            if(user != null && user.UserId > 0)
            {
                userPresent = true;
            }
            return userPresent;
        }
        public async Task<bool> CheckIfUserIsPrivileged(int userId)
        {
            Customer cust = dbContext.Customers.AsQueryable()
                                                .Where(row => row.UserId == userId && row.CustomerType == Globals.PrivilegedCustomer)
                                                .Select(row => row).FirstOrDefault();
            if (cust == null)
            {
                return false;
            }
            return true;
        }
        public async Task<BaseResponse> AddNewCustomer(NewCustomer newCustomer)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            // check if data is properly present or not
            if(!CheckCustomerObj(ref newCustomer, ref resp))
            {
                // data has some issue
                resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                resp.Status = ResponseStatus.Error;
                return resp;
            }
            
            try
            {
                // check if there is already a customer with same email and username
                newCustomer.Email = newCustomer.Email.ToLower();
                newCustomer.UserName = newCustomer.UserName.ToLower();
                bool userExists = await registrationBll.CheckUserExistsForUserName(newCustomer.UserName);
                if(userExists)
                {
                    resp.ErrorCode = ErrorCode.USER_ALREADY_EXIST;
                    resp.Message += "we have a user with same username - please choose some other username. ";
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }
                userExists = userExists || (await CheckUserExistsForEmail(newCustomer.Email));
                if (userExists)
                {
                    resp.ErrorCode = ErrorCode.USER_ALREADY_EXIST;
                    resp.Message += "we have a user with same email - please choose some other email";
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }

                // add new customer is customers table
                Customer newCustRow = new Customer();
                newCustRow.FirstName = newCustomer.FirstName;
                newCustRow.LastName = newCustomer.LastName;
                newCustRow.Email = newCustomer.Email;
                newCustRow.CustomerType = newCustomer.CustomerType;
                newCustRow.IsActive = true;
                newCustRow.CreatedAt = DateTime.UtcNow;
                newCustRow.UpdatedAt = DateTime.UtcNow;
                dbContext.Customers.Add(newCustRow);
                dbContext.SaveChanges();
                int custId = newCustRow.UserId;
                if(custId <= 0)
                {
                    resp.ErrorCode = ErrorCode.USER_NOT_INSERTED;
                    resp.Message += "There was some issue while inserting the user";
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }

                // add the customer login in user login table
                UsersLoginCred loginRow = new UsersLoginCred();
                loginRow.UserId = custId;
                loginRow.UserName = newCustomer.UserName;
                loginRow.Password = PasswordHelper.HashPassword(newCustomer.Password);
                loginRow.CreatedAt = DateTime.UtcNow;
                loginRow.UpdatedAt = DateTime.UtcNow;
                dbContext.UsersLoginCreds.Add(loginRow);
                dbContext.SaveChanges();
                if(loginRow.UserLoginId <= 0)
                {
                    resp.ErrorCode = ErrorCode.USER_LOGIN_NOT_INSERTED;
                    resp.Message += "There was some issue while inserting the user login";
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }
                resp.Status = ResponseStatus.Success;
            }
            catch (Exception ex) 
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
        public async Task<BaseResponse> GetAllCustomers()
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                var allCustomerDetails = (from cust in dbContext.Customers.AsQueryable()

                                          join custLogin in dbContext.UsersLoginCreds.AsQueryable()
                                          on cust.UserId equals custLogin.UserId into custDetailsTable
                                          from custDetailsRow in custDetailsTable.DefaultIfEmpty()

                                          select new CustomerDetails
                                          {
                                              FirstName = cust.FirstName,
                                              LastName = cust.LastName,
                                              UserId = cust.UserId,
                                              Email = cust.Email,
                                              UserName = custDetailsRow.UserName,
                                              CustomerType = cust.CustomerType
                                          }).AsQueryable();

                resp.Data = new
                {
                    AllCustomerDetails = allCustomerDetails
                };
                resp.Status = ResponseStatus.Success;

            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
        public async Task<BaseResponse> GetCustomerById(int custId)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                var customerDetails = (from cust in dbContext.Customers.AsQueryable()

                                          join custLogin in dbContext.UsersLoginCreds.AsQueryable()
                                          on cust.UserId equals custLogin.UserId into custDetailsTable
                                          from custDetailsRow in custDetailsTable.DefaultIfEmpty()

                                          where cust.UserId == custId

                                          select new CustomerDetails
                                          {
                                              FirstName = cust.FirstName,
                                              LastName = cust.LastName,
                                              UserId = cust.UserId,
                                              Email = cust.Email,
                                              UserName = custDetailsRow.UserName,
                                              CustomerType = cust.CustomerType
                                          }).AsQueryable();

                resp.Data = new
                {
                    CustomerDetails = customerDetails
                };
                resp.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
    }
}
