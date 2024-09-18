using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

public sealed class Result : BaseResult
{
    public Result() {}
    
    public Result(int statusCode)
    {
        StatusCode = statusCode;
    }

    public Result(string reason, int statusCode)
    {
        ResultFailure = new() { Errors = [reason] };
        StatusCode = statusCode;
    }

    //Only for validation failures
    private Result(string[] reason)
    {
        ResultFailure = new() { Errors = reason, IsValidationFailure = true };
        StatusCode = StatusCodes.Status400BadRequest;
    }

    public ActionResult ResponseResult()
    {
        if (IsSuccess)
        {
            return new StatusCodeResult(StatusCode);
        }

        return new ObjectResult(SetProblemDetails()) { StatusCode = StatusCode };
    }

    public static implicit operator ActionResult(Result result) => result.ResponseResult();

    public static Result Success(int statusCode) => new(statusCode);
    public static Result Ok() => new(StatusCodes.Status200OK);
    public static Result Created() => new(StatusCodes.Status201Created);
    public static Result NoContent() => new(StatusCodes.Status204NoContent);
    public static Result BadRequest(string reason) => new(reason, StatusCodes.Status400BadRequest);
    public static Result BadRequestValidation(string[] reason) => new(reason);
    public static Result Unauthorized(string reason) => new(reason, StatusCodes.Status401Unauthorized);
    public static Result Forbidden(string reason) => new(reason, StatusCodes.Status403Forbidden);
    public static Result NotFound(string reason) => new(reason, StatusCodes.Status404NotFound);
    public static Result Failure(string reason, int statusCode) => new(reason, statusCode);
}


public sealed class Result<T> : BaseResult
{
    //To let the serializer know that the property should be included in the serialization,
    //otherwise it will be ignored because of it is private
    [JsonInclude]
    //private T? Payload { get; init; }
    public T? Payload { get; init; }

    public Result() {}

    public Result(T? payload, int statusCode)
    {
        Payload = payload;
        StatusCode = statusCode;
    }

    public Result(string reason, int statusCode)
    {
        ResultFailure = new() { Errors = [reason] };
        StatusCode = statusCode;
    }

    //Only for validation failures
    private Result(string[] reason)
    {
        ResultFailure = new() { Errors = reason, IsValidationFailure = true };
        StatusCode = StatusCodes.Status400BadRequest;
    }

    public ActionResult ResponseResult()
    {
        if (IsSuccess)
        {
            return new ObjectResult(Payload) {StatusCode = StatusCode};
        }

        return new ObjectResult(SetProblemDetails()) {StatusCode = StatusCode};
    }

    public static implicit operator ActionResult(Result<T> result) => result.ResponseResult();

    public static Result<T> Success(T? payload, int statusCode) => new(payload, statusCode);
    public static Result<T> Ok(T? payload) => new(payload, StatusCodes.Status200OK);
    public static Result<T> Created(T? payload) => new(payload, StatusCodes.Status201Created);
    public static Result<T> NoContent(T? payload) => new(payload, StatusCodes.Status204NoContent);
    public static Result<T> BadRequest(string reason) => new(reason, StatusCodes.Status400BadRequest);
    public static Result<T> BadRequestValidation(string[] reason) => new(reason);
    public static Result<T> Unauthorized(string reason) => new(reason, StatusCodes.Status401Unauthorized);
    public static Result<T> Forbidden(string reason) => new(reason, StatusCodes.Status403Forbidden);
    public static Result<T> NotFound(string reason) => new(reason, StatusCodes.Status404NotFound);
    public static Result<T> Failure(string reason, int statusCode) => new(reason, statusCode);
}

// public sealed class Result : BaseResult
// {
//     //To let the serializer know that the property should be included in the serialization,
//     //otherwise it will be ignored because of it is private
//     [JsonInclude]
//     private object? Payload { get; }

//     public Result() {}

//     public Result(int statusCode, object? payload = null)
//     {
//         Payload = payload;
//         StatusCode = statusCode;
//     }

//     public Result(int statusCode, string reason)
//     {
//         ResultFailure = new() { Errors = [reason] };
//         StatusCode = statusCode;
//     }

//     //Only for validation failures
//     private Result(string[] reason)
//     {
//         ResultFailure = new() { Errors = reason, IsValidationFailure = true };
//         StatusCode = StatusCodes.Status400BadRequest;
//     }

//     public ActionResult ResponseResult()
//     {
//         if (IsSuccess)
//         {
//             if (Payload == null)
//                 return new StatusCodeResult(StatusCode);

//             return new ObjectResult(Payload) {StatusCode = StatusCode};
//         }

//         return new ObjectResult(SetProblemDetails()) {StatusCode = StatusCode};
//     }

//     public static implicit operator ActionResult(Result result) => result.ResponseResult();

//     public static Result Success(int statusCode) => new(statusCode);
//     public static Result Success<T>(T? payload, int statusCode) where T : class => new(statusCode, payload);

//     public static Result Ok() => new(StatusCodes.Status200OK);
//     public static Result Ok<T>(T? payload) where T : class => new(StatusCodes.Status200OK, payload);

//     public static Result Created() => new(StatusCodes.Status201Created);
//     public static Result Created<T>(T? payload) where T : class => new(StatusCodes.Status201Created, payload);

//     public static Result NoContent() => new(StatusCodes.Status204NoContent);
//     public static Result NoContent<T>(T? payload) where T : class => new(StatusCodes.Status204NoContent, payload);
    
//     public static Result BadRequest(string reason) => new(StatusCodes.Status400BadRequest, reason);
//     public static Result BadRequestValidation(string[] reason) => new(reason);
//     public static Result Unauthorized(string reason) => new(StatusCodes.Status401Unauthorized, reason);
//     public static Result Forbidden(string reason) => new(StatusCodes.Status403Forbidden, reason);
//     public static Result NotFound(string reason) => new(StatusCodes.Status404NotFound, reason);
//     public static Result Failure(string reason, int statusCode) => new(statusCode, reason);
// }