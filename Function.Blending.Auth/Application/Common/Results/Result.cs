using System.Net;
using Function.Blending.Auth.Application.Common.Wrappers;

namespace Function.Blending.Auth.Application.Common.Results
{

    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public bool IsFailure => !IsSuccess;
        public string? ErrorMessage { get; protected set; }
        public HttpStatusCode StatusCode { get; protected set; }

        protected Result(bool isSuccess, string? errorMessage, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
            => new(false, errorMessage, statusCode);
    }


    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        private Result(bool isSuccess, T? data, string? errorMessage, HttpStatusCode statusCode)
            : base(isSuccess, errorMessage, statusCode)
        {
            Data = data;
        }

        public static Result<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(true, data, null, statusCode);

        public static new Result<T> Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(false, default, errorMessage, statusCode);


        public BaseResponse<T> ToBaseResponse()
        {
            if (IsSuccess && Data != null)
            {
                return BaseResponse<T>.Success(Data, "Operación completada exitosamente", (int)StatusCode);
            }
            else if (IsSuccess && Data == null)
            {
                return BaseResponse<T>.Success("Operación completada exitosamente", (int)StatusCode);
            }
            else
            {
                return BaseResponse<T>.Fail(ErrorMessage ?? "Error desconocido", (int)StatusCode);
            }
        }
    }
}