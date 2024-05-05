using Microsoft.AspNetCore.Mvc;

namespace WebApi.Common;


public static class ApiResponseHandler
{
    public static ActionResult Handle<T>(ApiResponse<T> response)
    {
        int statusCode = response.status_code;

        switch (statusCode)
        {
            case 200:
                return new OkObjectResult(response);

            case 201:
                return new CreatedResult("", response);

            case 204:
                return new NoContentResult();

            case 400:
                return new BadRequestObjectResult(response);

            case 404:
                return new NotFoundObjectResult(response);

            case 409:
                return new ConflictObjectResult(response);


            default:
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    public static ActionResult Handle<T>(ApiPaginationResponse<T> response)
    {
        int statusCode = response.status_code;

        switch (statusCode)
        {
            case 200:
                return new OkObjectResult(response);

            case 201:
                return new CreatedResult("", response);

            case 204:
                return new NoContentResult();

            case 400:
                return new BadRequestObjectResult(response);

            case 404:
                return new NotFoundObjectResult(response);

            case 409:
                return new ConflictObjectResult(response);


            default:
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    public static ActionResult Handle<T>(ApiTokenResponse<T> response)
    {
        int statusCode = response.status_code;

        switch (statusCode)
        {
            case 200:
                return new OkObjectResult(response);

            case 201:
                return new CreatedResult("", response);

            case 204:
                return new NoContentResult();

            case 400:
                return new BadRequestObjectResult(response);

            case 404:
                return new NotFoundObjectResult(response);

            case 409:
                return new ConflictObjectResult(response);


            default:
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}