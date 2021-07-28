using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.Models.CodeModels
{
    public enum ErrorCode
    {
        USER_DOES_NOT_EXIST,
        USER_ALREADY_EXIST,
        USER_NOT_INSERTED,
        USER_LOGIN_NOT_INSERTED,
        USER_NOT_PRIVILEGED,
        PASSWORD_DOES_NOT_MATCH,
        ITEM_FINISHED,
        INPUT_DOES_NOT_HAVE_PROPER_DATA,
        RATE_NOT_FOUND,
        CANNOT_ADD_ROW_TO_DATABASE
    }
    public enum ResponseStatus
    {
        Fail = 0,
        Success = 1,
        Error = 2
    }
    public class BaseResponse
    {
        public BaseResponse(ResponseStatus status)
        {
            Status = status;
        }

        public ResponseStatus Status { get; set; }
        public object Data { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public ErrorCode? ErrorCode { get; set; }
    }
}
