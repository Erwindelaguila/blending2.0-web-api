using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Support.Http
{
  public static class HttpRequestDataExtensions
  {
    public static async Task<(byte[] bytes, string text)> ReadBodyCopyAsync(this HttpRequestData req)
    {
      using var ms = new MemoryStream();
      await req.Body.CopyToAsync(ms);
      var bytes = ms.ToArray();
      var text = Encoding.UTF8.GetString(bytes);
      return (bytes, text);
    }
  }
}
