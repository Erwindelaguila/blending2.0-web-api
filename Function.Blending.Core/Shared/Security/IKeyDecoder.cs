namespace Function.Blending.Core.Shared.Security;

/// <summary>Decodifica secretos Base64 o Hex a bytes.</summary>
public interface IKeyDecoder
{
  ReadOnlyMemory<byte> Decode(string secretString);
}
