using System.Xml.Schema;

namespace GML_Tools
{
    public class XsdFile
    {
        public string FileName { get; set; }

        public void DownloadXsdImports()
        {
            XmlSchemaSet gmlSchemaSet = new XmlSchemaSet
            {
                XmlResolver = new XsdUrlResolverDownload()
            };

            gmlSchemaSet.Add(null,FileName);
        }
    }
}
