using JewelleryStore.HelperMethods;
using JewelleryStore.Models.CodeModels;
using JewelleryStore.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.BLL
{
    public class RatesBLL
    {
        private sakilaContext dbContext = null;
        public RatesBLL()
        {
            dbContext = new sakilaContext();
        }
        private bool CheckRateData(NewRateDetails thisRate, ref BaseResponse resp)
        {
            bool allDataIsGood = true;
            if(String.IsNullOrEmpty(thisRate.Currency))
            {
                resp.Message += "Currency cannot be null. ";
                return false;
            }
            if(String.IsNullOrEmpty(thisRate.ItemType))
            {
                resp.Message += "Item type cannot be null. ";
                return false;
            }
            if(String.IsNullOrEmpty(thisRate.Unit))
            {
                resp.Message += "Unit cannot be null. ";
                return false;
            }
            if(!thisRate.RateAmount.HasValue || (thisRate.RateAmount.Value < 0))
            {
                resp.Message += "Amount cannot be null. ";
                return false;
            }
            return allDataIsGood;
        }
        private bool CheckCurrencyValue(string currency)
        {
            if(currency.Equals(Globals.IndianRupees))
            {
                return true;
            }
            if (currency.Equals(Globals.USDollars))
            {
                return true;
            }
            if (currency.Equals(Globals.BritishPounds))
            {
                return true;
            }
            return false;
        }
        private bool CheckUnitValue(string unit)
        {
            if (unit.Equals(Globals.Gram))
            {
                return true;
            }
            return false;
        }
        private bool CheckItemType(string itemType)
        {
            if (itemType.Equals(Globals.Gold))
            {
                return true;
            }
            if (itemType.Equals(Globals.Diamond))
            {
                return true;
            }
            if (itemType.Equals(Globals.Silver))
            {
                return true;
            }
            if (itemType.Equals(Globals.Platinum))
            {
                return true;
            }
            return false;
        }
        private bool CheckRateExists(NewRateDetails thisRate)
        {
            Rate rateRow = dbContext.Rates.Where(rate => rate.Unit.Equals(thisRate.Unit) &&
                                                         rate.Currency.Equals(thisRate.Currency) &&
                                                         rate.ItemType.Equals(thisRate.ItemType))
                                          .Select(row => row).FirstOrDefault();
            if(rateRow == null)
            {
                return false;
            }
            return true;
        }
        private async Task<int> CreateRateRecord(NewRateDetails thisRate)
        {
            Rate newRate = new Rate();
            newRate.ItemType = thisRate.ItemType;
            newRate.Unit = thisRate.Unit;
            newRate.Currency = thisRate.Currency;
            newRate.RateAmount = thisRate.RateAmount.Value;
            newRate.CreatedAt = DateTime.UtcNow;
            newRate.UpdatedAt = DateTime.UtcNow;
            dbContext.Rates.Add(newRate);
            dbContext.SaveChanges();
            return newRate.RateId;
        }
        private async Task<bool> UpdateRecord(NewRateDetails thisRate)
        {
            bool allUpdated = true;
            try
            {
                Rate rateRow = dbContext.Rates.AsQueryable()
                                                .Where(row => row.Currency.Equals(thisRate.Currency) && row.ItemType.Equals(thisRate.ItemType))
                                                .Select(row => row)
                                                .FirstOrDefault();
                if(rateRow == null)
                {
                    throw new Exception("Only RateAmount is allowed to change");
                }
                rateRow.RateAmount = thisRate.RateAmount.Value;
                rateRow.UpdatedAt = DateTime.UtcNow;
                dbContext.Rates.Update(rateRow);
                dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                allUpdated = false;
            }
            return allUpdated;
        }
        private async Task<bool> TakeAction(NewRateDetails newRateDetails)
        {
            if (CheckRateExists(newRateDetails))
            {
                // update the record
                if (await UpdateRecord(newRateDetails))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                int newRateId = await CreateRateRecord(newRateDetails);
                if (newRateId > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<BaseResponse> AddRate(NewRateDetails thisRate)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                if(!CheckRateData(thisRate, ref resp))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }
                thisRate.Currency = thisRate.Currency.ToLower();
                if(!CheckCurrencyValue(thisRate.Currency))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Currency value is not proper";
                    return resp;
                }
                thisRate.Unit = thisRate.Unit.ToLower();
                if(!CheckUnitValue(thisRate.Unit))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Unit value is not proper - it has to be only in grams";
                    return resp;
                }
                thisRate.ItemType = thisRate.ItemType.ToLower();
                if(!CheckItemType(thisRate.ItemType))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Item type value is not proper";
                    return resp;
                }

                if (await TakeAction(thisRate))
                {
                    resp.Status = ResponseStatus.Success;
                }
                else
                {
                    resp.Status = ResponseStatus.Error;
                    resp.Message += " Some issue occured while inserting/updating row into database.";
                    resp.ErrorCode = ErrorCode.CANNOT_ADD_ROW_TO_DATABASE;
                }

            }
            catch(Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
        public async Task<BaseResponse> UpdateRate(NewRateDetails newRateDetails)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                if (!newRateDetails.RateId.HasValue || newRateDetails.RateId.Value < 1 || !CheckRateData(newRateDetails, ref resp))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    return resp;
                }
                newRateDetails.Currency = newRateDetails.Currency.ToLower();
                if (!CheckCurrencyValue(newRateDetails.Currency))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Currency value is not proper";
                    return resp;
                }
                newRateDetails.Unit = newRateDetails.Unit.ToLower();
                if (!CheckUnitValue(newRateDetails.Unit))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Unit value is not proper - it has to be only in grams";
                    return resp;
                }
                newRateDetails.ItemType = newRateDetails.ItemType.ToLower();
                if (!CheckItemType(newRateDetails.ItemType))
                {
                    resp.ErrorCode = ErrorCode.INPUT_DOES_NOT_HAVE_PROPER_DATA;
                    resp.Status = ResponseStatus.Error;
                    resp.Message = "Item type value is not proper";
                    return resp;
                }
                if(await TakeAction(newRateDetails))
                {
                    resp.Status = ResponseStatus.Success;
                }
                else
                {
                    resp.Status = ResponseStatus.Error;
                    resp.Message += " Some issue occured while inserting/updating row into database.";
                    resp.ErrorCode = ErrorCode.CANNOT_ADD_ROW_TO_DATABASE;
                }
            }
            catch(Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = ResponseStatus.Error;
            }
            return resp;
        }
        public async Task<BaseResponse> GetAllRates()
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                List<Rate> allRates = dbContext.Rates.AsQueryable()
                                                        .Where(row => true)
                                                        .Select(row => row).ToList<Rate>();
                resp.Data = new
                {
                    AllRates = allRates
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
        public async Task<BaseResponse> GetRatesById(int rateId)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                Rate thisRate = dbContext.Rates.AsQueryable()
                                                        .Where(row => row.RateId == rateId)
                                                        .Select(row => row).FirstOrDefault();
                resp.Data = new
                {
                    RateDetails = thisRate
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
        public async Task<BaseResponse> GetRatesForItemType(string itemType)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                List<Rate> thisRateList = dbContext.Rates.AsQueryable()
                                                        .Where(row => row.ItemType.Equals(itemType))
                                                        .Select(row => row).ToList<Rate>();
                resp.Data = new
                {
                    ItemType = itemType,
                    AllRates = thisRateList
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
        public async Task<BaseResponse> DeleteRateById(int rateId)
        {
            BaseResponse resp = new BaseResponse(ResponseStatus.Fail);
            try
            {
                Rate thisRate = dbContext.Rates.AsQueryable()
                                                        .Where(row => row.RateId == rateId)
                                                        .Select(row => row).FirstOrDefault();
                dbContext.Rates.Remove(thisRate);
                dbContext.SaveChanges();
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
