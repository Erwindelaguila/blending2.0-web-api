using System.Xml.Linq;
namespace Function.Blending.Upload.Helpers;

public class SapXmlHelper
{
    public List<Dictionary<string, string>> ParseStockXml(string xml)
    {
        var doc = XDocument.Parse(xml);

        XNamespace atom = "http://www.w3.org/2005/Atom";
        XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        return doc.Descendants(atom + "entry")
            .Select(entry =>
                entry.Descendants(m + "properties")
                    .Select(p =>
                        p.Elements()
                            .ToDictionary(e => e.Name.LocalName, e => e.Value)
                    ).FirstOrDefault()
            )
            .Where(e => e != null)
            .ToList();
    }
}