using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Upload.Helpers.Multipart;

public static class MultipartRequestValidator
{
    public static bool IsMultipartFormData(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Content-Type", out var contentTypes))
            return false;

        var contentType = contentTypes.FirstOrDefault();
        return contentType != null && contentType.Contains("multipart/form-data");
    }
}