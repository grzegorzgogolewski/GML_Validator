using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace GML_Tools
{
    class XsdUrlResolverReplace : XmlUrlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri absoluteUri = base.ResolveUri(baseUri, relativeUri);

            if (!absoluteUri.IsFile)
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\xsd\\" + absoluteUri.Host + Path.GetDirectoryName(absoluteUri.LocalPath.Replace('/', '\\'));
                string file = Path.GetFileName(absoluteUri.LocalPath);

                string absolutePath = directory + "\\" + file;

                absoluteUri = new Uri(absolutePath);
            }

            return absoluteUri;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            switch (GlobalVariables.Schema)
            {
                case "GESUT":

                    switch (Path.GetFileName(absoluteUri.LocalPath))
                    {
                        case "BT_ModelPodstawowy.xsd":
                            absoluteUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "GESUT", "BT_ModelPodstawowy.xsd"));
                            break;
                        case "GESUT.xsd":
                            absoluteUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "GESUT", "GESUT.xsd"));
                            break;
                        case "GES_GESUT_Slowniki.xsd":
                            absoluteUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "GESUT", "GES_GESUT_Slowniki.xsd"));
                            break;
                    }

                    break;
            }

            return File.Exists(absoluteUri.LocalPath) ? base.GetEntity(absoluteUri, role, ofObjectToReturn) : null;
        }

    }
}
