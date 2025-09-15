using System.Net;
using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Functions.Support.Routing;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Http;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Triggers.Logistics.Upload;

public class UploadExcelLogisticsFuncion
{
    private readonly ILogger<UploadExcelLogisticsFuncion> _logger;
    private readonly UploadExcelLogisticProcess _uploadExcelLogisticProcess;

    public UploadExcelLogisticsFuncion(ILogger<UploadExcelLogisticsFuncion> logger, UploadExcelLogisticProcess uploadExcelLogisticProcess)
    {
        _logger = logger;
        _uploadExcelLogisticProcess = uploadExcelLogisticProcess;
       
    }

    [Function(nameof(UploadExcelLogisticsFuncion))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Logistic.upload)]
        HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("Solicitud de carga de archivo logistica recibida.");

            var dataParced = await _uploadExcelLogisticProcess.ExecuteAsync(req);

            return await HttpResponseHelper.WriteBaseResponseAsync(req,
                BaseResponse<ExcelExtractLogisticDto>.Success(dataParced, "Archivo procesado correctamente."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en la función GetCadmio");
            return await HttpResponseHelper.WriteBaseResponseAsync(
                req,
                BaseResponse<object>.Fail(null, "Ocurrió un error inesperado.", (int)HttpStatusCode.InternalServerError)
            );
        }
    }
}