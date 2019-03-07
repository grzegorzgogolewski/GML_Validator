using System;
using System.IO;
using System.Net;
using System.Xml;

namespace GML_Tools
{
    class XsdUrlResolverDownload : XmlUrlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri absoluteUri = base.ResolveUri(baseUri, relativeUri);

            if (!absoluteUri.IsFile)
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\xsd\\" + absoluteUri.Host + Path.GetDirectoryName(absoluteUri.LocalPath.Replace('/','\\'));

                Directory.CreateDirectory(directory);

                string file = Path.GetFileName(absoluteUri.LocalPath);

                if (!File.Exists(directory + "\\" + file))
                {
                    Console.WriteLine(baseUri + " : " + relativeUri + " => " + absoluteUri);

                    using (WebClient web = new WebClient())
                    {
                        web.DownloadFile(absoluteUri, directory + "\\" + file);
                    }
                }
               
            }

            return absoluteUri;
        }
    }
}
