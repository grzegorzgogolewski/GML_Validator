using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace GML_Tools
{
    public class GmlFile
    {
        private XmlSchemaSet _gmlSchemaSet;
        private readonly ObiektType _obiektType = new ObiektType();

        private long _errorCounter;

        private readonly ErrorInfoList _errorInfoList = new ErrorInfoList();

        private bool _featureMemberStart;
        private string _featureMemberName;
        private string _featureMemberId;
        private string _lokalnyId;

        private readonly string _gmlFile;
        
        private readonly ObiektyGesut _obiektGesut = new ObiektyGesut();

        public GmlFile(string fileName)
        {
            _gmlFile = fileName;

            XmlDocument gmlFile = new XmlDocument();

            gmlFile.Load(fileName);

            if (GlobalVariables.Schema == "GESUT")
            {
                XmlNodeList nodes = gmlFile.GetElementsByTagName("gml:featureMember");

                foreach (XmlNode node in nodes)
                {
                    XmlNode obiekt = node.FirstChild;

                    string gmlId = obiekt.Attributes?["gml:id"].Value;
                    string istnienie = "";

                    foreach (XmlNode atrybut in obiekt)
                    {
                        switch (atrybut.LocalName)
                        {
                            case "istnienie":
                                istnienie = atrybut.InnerText;
                                break;
                        }
                    }

                    if (gmlId != null) _obiektGesut.Add(gmlId, istnienie);
                }

            }

        }

        public void SetSchema()
        {
            _gmlSchemaSet = new XmlSchemaSet
            {
                XmlResolver = new XsdUrlResolverReplace()
            };

            switch (GlobalVariables.Schema)
            {
                case "GESUT":
                    GlobalVariables.Schema = "GESUT";
                    _gmlSchemaSet.Add("urn:gugik:specyfikacje:gmlas:geodezyjnaEwidencjaSieciUzbrojeniaTerenu:1.0", AppDomain.CurrentDomain.BaseDirectory + "\\xsd\\GESUT\\GESUT.xsd" );
                    break;
            }

            _gmlSchemaSet.Compile();

            ObiektSlowniki obiektSlowniki = new ObiektSlowniki();

            XmlSchema xmlSchema = _gmlSchemaSet.Schemas().Cast<XmlSchema>().First();

            IEnumerable<XmlSchemaSimpleType> simpleTypes = xmlSchema.SchemaTypes.Values.OfType<XmlSchemaSimpleType>().Where(t => t.Content is XmlSchemaSimpleTypeRestriction);

            foreach (XmlSchemaSimpleType simpleType in simpleTypes)
            {
                XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
                IEnumerable<XmlSchemaEnumerationFacet> enumFacets = restriction.Facets.OfType<XmlSchemaEnumerationFacet>();

                IEnumerable<XmlSchemaEnumerationFacet> xmlSchemaEnumerationFacets = enumFacets.ToList();

                List<string> wartosc = xmlSchemaEnumerationFacets.Select(facet => facet.Value).ToList();

                obiektSlowniki.Add(simpleType.Name, wartosc);
            }

            foreach (XmlSchemaElement obiekt in xmlSchema.Elements.Values)
            {
                ObiektAtrybuty obiektAtrybuty = new ObiektAtrybuty();

                XmlSchemaComplexType complexType = obiekt.ElementSchemaType as XmlSchemaComplexType;

                if (complexType?.ContentTypeParticle is XmlSchemaSequence sequence)
                {
                    foreach (XmlSchemaObject o in sequence.Items)
                    {
                        XmlSchemaElement atrybut = (XmlSchemaElement) o;

                        if (atrybut.Name != null)
                        {

                            if (atrybut.ElementSchemaType.Name != null)
                            {
                                obiektAtrybuty.Add(atrybut.Name, obiektSlowniki.ContainsKey(atrybut.ElementSchemaType.Name) ? obiektSlowniki[atrybut.ElementSchemaType.Name] : null);
                            }
                            else
                            {
                                if (atrybut.ElementSchemaType.BaseXmlSchemaType.Name != null)
                                {
                                    obiektAtrybuty.Add(atrybut.Name, obiektSlowniki.ContainsKey(atrybut.ElementSchemaType.BaseXmlSchemaType.Name) ? obiektSlowniki[atrybut.ElementSchemaType.BaseXmlSchemaType.Name] : null);
                                }
                                else
                                {
                                    obiektAtrybuty.Add(atrybut.Name, obiektSlowniki.ContainsKey(atrybut.ElementSchemaType.TypeCode.ToString()) ? obiektSlowniki[atrybut.ElementSchemaType.TypeCode.ToString()] : null);
                                }
                            }
                        }
                    }
                }

                _obiektType.Add(obiekt.Name, obiektAtrybuty);
            }

        }

        public ErrorInfoList Validate()
        {
            XmlReaderSettings gmlReaderSettings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true,
                ValidationType = ValidationType.Schema,
                Schemas = _gmlSchemaSet,
                XmlResolver = new XsdUrlResolverReplace(),
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ReportValidationWarnings
            };

            gmlReaderSettings.ValidationEventHandler += GmlReaderSettingsOnValidationEventHandler;

            XmlReader gmlReader = XmlReader.Create(_gmlFile, gmlReaderSettings);

            while (gmlReader.Read())
            {
                if (_featureMemberStart && gmlReader.NodeType == XmlNodeType.Element)
                {
                    _featureMemberName = gmlReader.LocalName;
                    _featureMemberId = gmlReader.GetAttribute("gml:id");

                    _lokalnyId = gmlReader.GetAttribute(0)?.Split('_')[1];

                    _featureMemberStart = false;
                }

                switch (gmlReader.NodeType)
                {
                    case XmlNodeType.Element when gmlReader.Name == "gml:featureMember":
                        _featureMemberStart = true;                            
                        _featureMemberName = string.Empty;
                        _featureMemberId = string.Empty;
                        _lokalnyId = string.Empty;
                        break;
                    case XmlNodeType.EndElement when gmlReader.Name == "gml:featureMember":
                        _featureMemberStart = false;                            
                        _featureMemberName = string.Empty;
                        _featureMemberId = string.Empty;
                        _lokalnyId = string.Empty;
                        break;
                }
            }

            return _errorInfoList;
        }

        private void GmlReaderSettingsOnValidationEventHandler(object sender, ValidationEventArgs e)
        {
            string listaWartosciAtrybutu = _obiektType.GetEnumeration(_featureMemberName, ((XmlReader) sender).LocalName);

            string istnienie = _obiektGesut.GetIstnienie(_featureMemberId);

            ErrorInfo error = new ErrorInfo();

            ++_errorCounter;

            error.ErrorCounter = _errorCounter;
            error.LineNumber = e.Exception.LineNumber;
            error.ErrorType = e.Severity.ToString();
            error.FeatureMember = _featureMemberName;
            error.LokalnyId = _lokalnyId;
            error.Element = ((XmlReader) sender).LocalName;
            error.ShortInfo = e.Exception.InnerException != null ? e.Exception.InnerException.Message + " " + listaWartosciAtrybutu : "";
            error.LongInfo = e.Exception.Message;
            error.Istnienie = istnienie;

            _errorInfoList.Add(error);

        }
    }
}
