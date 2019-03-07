using System;
using System.IO;
using GML_Tools;

namespace GML_Validator
{
    class Program
    {

        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            XsdFile xsdFile = new XsdFile();
            XsdFile xsd = xsdFile;

            xsd.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "GESUT", "GESUT.xsd");
            //xsd.DownloadXsdImports();

            GmlFile gmlFile = new GmlFile("C:\\Temp\\046301_1.0026.gml");
            gmlFile.SetSchema("GESUT");
            gmlFile.Validate();


            Console.WriteLine("Press any key!");
            Console.ReadKey();

        }

    }
}
