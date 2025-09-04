using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;


namespace Function.Blending.Upload.Helpers.Multipart;

  public static class MultipartFormDataHelper
  {
    ///// <summary>
    ///// Extrae el primer archivo enviado en el cuerpo multipart/form-data.
    ///// </summary>
    //public static async Task<byte[]> ExtractFileAsync(HttpRequestData req)
    //{
    //  var boundary = GetBoundary(req.Headers);
    //  if (string.IsNullOrEmpty(boundary))
    //    throw new InvalidOperationException("No se pudo determinar el boundary del multipart.");

    //  using var ms = new MemoryStream();
    //  await req.Body.CopyToAsync(ms);
    //  var bodyBytes = ms.ToArray();
    //  var bodyString = Encoding.UTF8.GetString(bodyBytes);

    //  var delimiter = $"--{boundary}";
    //  var parts = bodyString.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

    //  foreach (var part in parts)
    //  {
    //    if (part.Contains("Content-Disposition") && part.Contains("filename="))
    //    {
    //      var index = part.IndexOf("\r\n\r\n");
    //      if (index >= 0)
    //      {
    //        var fileContent = part.Substring(index + 4);
    //        var fileBytes = Encoding.UTF8.GetBytes(fileContent.TrimEnd('\r', '\n'));
    //        return fileBytes;
    //      }
    //    }
    //  }

    //  throw new InvalidOperationException("No se encontró archivo en el cuerpo multipart.");
    //}

    public static async Task<byte[]> ExtractFileAsync(HttpRequestData req)
    {
      using var ms = new MemoryStream();
      await req.Body.CopyToAsync(ms);
      ms.Position = 0;

      var parser = MultipartFormDataParser.Parse(ms);
      var file = parser.Files.FirstOrDefault();

      if (file == null)
        throw new InvalidOperationException("No se encontró ningún archivo en el cuerpo multipart.");

      using var fileStream = file.Data;
      using var fileMs = new MemoryStream();
      await fileStream.CopyToAsync(fileMs);
      return fileMs.ToArray();
    }
    
    public static Task<byte[]> ToByteArrayAsync(XLWorkbook workbook)
    {
      var ms = new MemoryStream();
      workbook.SaveAs(ms);
      return Task.FromResult(ms.ToArray());
    }
    

    /// <summary>
    /// Extrae el boundary del encabezado Content-Type.
    /// </summary>
    private static string GetBoundary(HttpHeadersCollection headers)
    {
      var contentType = headers.GetValues("Content-Type").FirstOrDefault();
      var elements = contentType?.Split(';');
      var boundaryElement = elements?.FirstOrDefault(e => e.Trim().StartsWith("boundary="));
      return boundaryElement?.Split('=')[1].Trim('"');
    }
  }