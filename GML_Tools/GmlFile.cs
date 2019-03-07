using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace GML_Tools
{
    public class GmlFile
    {
        private readonly string _fileName;
        private XmlSchemaSet _gmlSchemaSet;
        private long _errorCount = 0;

        public GmlFile(string fileName)
        {
            _fileName = fileName;
        }

        public void SetSchema(string schema)
        {
            _gmlSchemaSet = new XmlSchemaSet
            {
                XmlResolver = new XsdUrlResolverReplace()
            };

            switch (schema)
            {
                case "GESUT":
                    GlobalVariables.Schema = "GESUT";
                    _gmlSchemaSet.Add("urn:gugik:specyfikacje:gmlas:geodezyjnaEwidencjaSieciUzbrojeniaTerenu:1.0", AppDomain.CurrentDomain.BaseDirectory + "\\xsd\\GESUT\\GESUT.xsd" );
                    break;
            }
        }

        public void Validate()
        {
            XmlReaderSettings gmlReaderSettings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = _gmlSchemaSet,
                XmlResolver = new XsdUrlResolverReplace(),
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings
            };

            gmlReaderSettings.ValidationEventHandler += GmlReaderSettingsOnValidationEventHandler;

            XmlReader gmlReader = XmlReader.Create(_fileName, gmlReaderSettings);

            //var doc = XDocument.Load(gmlReader);

            while (gmlReader.Read())
            {
                if (gmlReader.NodeType == XmlNodeType.Element && gmlReader.Name == "gml:featureMember")
                {
                    //Console.WriteLine(gmlReader.ReadOuterXml());
                }
            }
        }

        private void GmlReaderSettingsOnValidationEventHandler(object sender, ValidationEventArgs e)
        {
            ++_errorCount;
            Console.WriteLine(_errorCount + ": " + e.Message);
        }
    }
}
