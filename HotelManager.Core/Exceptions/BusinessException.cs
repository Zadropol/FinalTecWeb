using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; } = 400;
        public string ErrorCode { get; set; }

        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BusinessException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public BusinessException(string message, string errorCode, int statusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
