using System;
using Function.Blending.Opt.Shared.Security;

namespace Function.Blending.Opt.Infrastructure.Security.KeyDecoders;

public sealed class KeyDecoder : IKeyDecoder
{
  public ReadOnlyMemory<byte> Decode(string s)
  {
    if (string.IsNullOrWhiteSpace(s)) return ReadOnlyMemory<byte>.Empty;
    s = s.Trim();

    // Base64: caracteres típicos ('=', '/', '+')
    if (s.Contains('=') || s.Contains('/') || s.Contains('+'))
      return Convert.FromBase64String(s);

    // Hex
    var len = s.Length / 2;
    var bytes = new byte[len];
    for (int i = 0; i < len; i++)
      bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
    return bytes;
  }
}
