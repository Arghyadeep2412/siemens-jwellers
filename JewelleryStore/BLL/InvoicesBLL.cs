using JewelleryStore.HelperMethods;
using JewelleryStore.Models.CodeModels;
using JewelleryStore.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.BLL
{
    public class InvoicesBLL
    {
        private sakilaContext dbContext = null;
        private CustomerBLL customerBLL = null;
        public InvoicesBLL()
        {
            dbContext = new sakilaContext();
            customerBLL = new CustomerBLL();
        }
        private bool CheckInvoiceFormat(NewInvoice newInvoice, ref BaseResponse resp)
        {
            bool isFormatOkay = true;
            if(String.IsNullOrEmpty(newInvoice.ItemType))
            {
                resp.Message += "Item type is not present. ";
                isFormatOkay = false;
            }
            if(String.IsNullOrEmpty(newInvoice.WeightUnit))
            {
                resp.Message += "Please give proper weight. ";
                isFormatOkay = false;
            }
            if (!newInvoice.Weight.HasValue || newInvoice.Weight < 0 )
            {
                resp.Message += "Please give proper weight. ";
                isFormatOkay = false;
            }
            if (String.IsNullOrEmpty(newInvoice.Currency))
            {
                resp.Message += "Please provide a currency for the rate. ";
                isFormatOkay = false;
            }
            return isFormatOkay;
        }
        private bool GetRateForItemTypeAndCurrency(string item_type, string curr, ref Invoice thisInvoiceRow)
        {
            Rate thisRate = dbContext.Rates.AsQueryable()
                                            .Where(row => row.ItemType.Equals(item_type) && row.Currency.Equals(curr))
                                            .Select(row => row).FirstOrDefault();
            if(thisRate == null)
            {
                return false;
            }
            thisInvoiceRow.Rate = thisRate.RateAmount;
            thisInvoiceRow.Currency = thisRate.Currency;
            if (thisRate.Unit.Contains(Globals.Gram))
            {
                thisInvoiceRow.PricePerUnit = Globals.Gram;
            }
            else if (thisRate.Unit.Contains(Globals.KiloGram))
            {
                thisInvoiceRow.PricePerUnit = Globals.KiloGram;
            }
            else
            {
                return false;
            }
            // thisInvoiceRow.PricePerUnit = thisRate.Unit;
            return true;
        }
        private void CalculateActualPrice(ref Invoice thisInvoiceRow)
        {
            thisInvoiceRow.ActualPrice = thisInvoiceRow.Rate * thisInvoiceRow.Weight;
            if(!thisInvoiceRow.WeightUnit.Equals(thisInvoiceRow.PricePerUnit))
            {
                if(thisInvoiceRow.PricePerUnit.Equals(Globals.Gram) && thisInvoiceRow.WeightUnit.Equals(Globals.KiloGram))
                {
                    thisInvoiceRow.ActualPrice = thisInvoiceRow.Rate * thisInvoiceRow.Weight * 1000;
                }
                else if(thisInvoiceRow.PricePerUnit.Equals(Globals.KiloGram) && thisInvoiceRow.WeightUnit.Equals(Globals.Gram))
                {
                    thisInvoiceRow.ActualPrice = thisInvoiceRow.Rate * thisInvoiceRow.Weight / 1000;
                }
            }
            thisInvoiceRow.FinalPrice = thisInvoiceRow.ActualPrice;
        }
        private void CalculatePriceAterDiscount(ref Invoice thisInvoiceRow)
        {
            thisInvoiceRow.FinalPrice = thisInvoiceRow.ActualPrice - (thisInvoiceRow.DiscountPercentage.Value * thisInvoiceRow.ActualPrice / 100);
        }
        public async Task<BaseResponse> AddNewInvoice(int userId, NewInvoice newInvoice)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                if (!CheckInvoiceFormat(newInvoice, ref resp))
                {
                    // data has some issue
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }
                Invoice thisInvoiceRow = new Invoice();
                thisInvoiceRow.UserId = userId;
                newInvoice.ItemType = newInvoice.ItemType.ToLower();
                thisInvoiceRow.ItmeType = ApplicationHelper.GetProperItemType(newInvoice.ItemType);
                if(thisInvoiceRow.ItmeType.Equals(Globals.OthersItemType))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Please provide proper item type.";
                    return resp;
                }
                newInvoice.Currency = newInvoice.Currency.ToLower();
                newInvoice.WeightUnit = newInvoice.WeightUnit.ToLower();
                // get weight
                thisInvoiceRow.Weight = newInvoice.Weight.Value;
                thisInvoiceRow.WeightUnit = ApplicationHelper.GetProperUnits(newInvoice.WeightUnit);
                if(thisInvoiceRow.WeightUnit.Equals(Globals.OtherUnit))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Please provide proper weight units";
                    return resp;
                }
                // get rate for type and currency
                if (!GetRateForItemTypeAndCurrency(newInvoice.ItemType, newInvoice.Currency, ref thisInvoiceRow))
                {
                    //could not found rate in database
                    resp.ErrorCode = ErrorCode.RATE_NOT_FOUND;
                    resp.Status = ResponseStatus.Error;
                    resp.Message += "Could not found rates for given item type and currency";
                    return resp;
                }
                
                // get discount
                if(await customerBLL.CheckIfUserIsPrivileged(thisInvoiceRow.UserId))
                {
                    if(!newInvoice.DiscountPercentage.HasValue)
                    {
                        thisInvoiceRow.DiscountPercentage = Globals.DefaultDiscountPercentage;
                    }
                    else
                    {
                        thisInvoiceRow.DiscountPercentage = newInvoice.DiscountPercentage.Value;
                    }
                }
                // calculate price
                CalculateActualPrice(ref thisInvoiceRow);
                if(thisInvoiceRow.DiscountPercentage.HasValue && thisInvoiceRow.DiscountPercentage.Value > 0)
                {
                    CalculatePriceAterDiscount(ref thisInvoiceRow);
                }
                thisInvoiceRow.CreatedAt = DateTime.UtcNow;
                thisInvoiceRow.UpdatedAt = DateTime.UtcNow;
                thisInvoiceRow.PaymentStatus = newInvoice.PaymentStatus;
                // add invoice to db
                dbContext.Invoices.Add(thisInvoiceRow);
                dbContext.SaveChanges();
                if(thisInvoiceRow.InvoiceId > 0)
                {
                    newInvoice.InvoiceId = thisInvoiceRow.InvoiceId;
                    resp.Status = ResponseStatus.Success;
                    resp.Data = new
                    {
                        Invoice = thisInvoiceRow
                    };
                }
                else
                {
                    resp.ErrorCode = ErrorCode.CANNOT_ADD_ROW_TO_DATABASE;
                    resp.Status = ResponseStatus.Error;
                    resp.Message += "Could not add invoice to database";
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            
            return resp;
        }
        public async Task<BaseResponse> GetAllInvoices()
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                var allInvoicesList = dbContext.Invoices.AsQueryable()
                                                        .Where(row => true)
                                                        .Select(row => row);
                resp.Data = new
                {
                    AllInvoiceList = allInvoicesList
                };
                resp.Status = ResponseStatus.Success;
            }
            catch(Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
        public async Task<BaseResponse> GetInvoicesByInvoiceId(int invoiceId)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                var thisInvoice = dbContext.Invoices.AsQueryable()
                                                        .Where(row => row.InvoiceId == invoiceId)
                                                        .Select(row => row);
                resp.Data = new
                {
                    InvoiceDetails = thisInvoice
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
        public async Task<BaseResponse> GetInvoicesForUserId(int userId)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                var invoiceList = dbContext.Invoices.AsQueryable()
                                                        .Where(row => row.UserId == userId)
                                                        .Select(row => row);
                resp.Data = new
                {
                    InvoiceDetails = invoiceList
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
