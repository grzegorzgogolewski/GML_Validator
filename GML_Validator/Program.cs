using System;
using System.Collections.Generic;
using System.IO;
using GML_Tools;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GML_Validator
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 1 when args[0] == "xsd":
                
                    XsdFile xsdFile = new XsdFile();
                    XsdFile xsd = xsdFile;
    
                    Console.WriteLine("Pobieranie schematów dla BDOT...");
                    xsd.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "BDOT", "BDOT500.xsd");
                    xsd.DownloadXsdImports();

                    Console.WriteLine("Pobieranie schematów dla EGiB...");
                    xsd.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "EGiB", "EGB_OgolnyObiekt.xsd");
                    xsd.DownloadXsdImports();

                    Console.WriteLine("Pobieranie schematów dla GESUT...");
                    xsd.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "GESUT", "GESUT.xsd");
                    xsd.DownloadXsdImports();

                    Console.WriteLine("Pobieranie schematów dla OSN...");
                    xsd.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "OSN", "OS_Osnowa.xsd");
                    xsd.DownloadXsdImports();

                    Console.WriteLine("Pobieranie schematów dla RCiWN...");
                    xsd.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd", "RCiWN", "RCW_RCiWN.xsd");
                    xsd.DownloadXsdImports();

                    break;
                
                case 2 :

                    GlobalVariables.Schema = args[0];

                    Console.WriteLine("Wczytywanie pliku: {0}\n", args[1]);
                    GmlFile gmlFile = new GmlFile(args[1]);

                    Console.WriteLine("Ustawianie schematu pliku: {0}\n", GlobalVariables.Schema);

                    List<string> schamaList = new List<string>(new[] { "BDOT", "EGiB", "GESUT", "OSN", "RCiWN"});

                    if (!schamaList.Contains(GlobalVariables.Schema))
                    {
                        Console.WriteLine("Niepoprawna nazwa schematu: {0} !", GlobalVariables.Schema);
                        Console.ReadKey();
                        return;
                    }

                    gmlFile.SetSchema();

                    Console.WriteLine("Sprawdzanie pliku: {0}\n", args[1]);
                    ErrorInfoList errorList = gmlFile.Validate();


                    string outputFile = args[1].Substring(0, args[1].LastIndexOf(".", StringComparison.Ordinal)) + "_walidacja.xlsx";
                    
                    Console.WriteLine("Zapisywanie wyników do pliku: {0}\n", outputFile);

                    FileInfo xlsFile = new FileInfo(outputFile);
                    if (xlsFile.Exists) xlsFile.Delete();

                    ExcelPackage xlsWorkbook = new ExcelPackage(xlsFile);

                    xlsWorkbook.Workbook.Properties.Title = "Wynik walidacji pliku GML";
                    xlsWorkbook.Workbook.Properties.Author = "Grzegorz Gogolewski";
                    xlsWorkbook.Workbook.Properties.Comments = "Wynik walidacji pliku GML";
                    xlsWorkbook.Workbook.Properties.Company = "GISNET";

                    ExcelWorksheet xlsSheet = xlsWorkbook.Workbook.Worksheets.Add("WALIDACJA");

                    xlsSheet.Cells[1, 1].Value = "Error\nCounter";
                    xlsSheet.Cells[1, 2].Value = "Line\nNumber";
                    xlsSheet.Cells[1, 3].Value = "ErrorType";
                    xlsSheet.Cells[1, 4].Value = "FeatureMember";
                    xlsSheet.Cells[1, 5].Value = "LokalnyId";
                    xlsSheet.Cells[1, 6].Value = "Istnienie";
                    xlsSheet.Cells[1, 7].Value = "Element";
                    xlsSheet.Cells[1, 8].Value = "ShortInfo";
                    xlsSheet.Cells[1, 9].Value = "LongInfo";

                    int rowCounter = 1;

                    foreach (ErrorInfo err in errorList)
                    {
                        ++rowCounter;
                        xlsSheet.Cells[rowCounter, 1].Value = err.ErrorCounter;
                        xlsSheet.Cells[rowCounter, 2].Value = err.LineNumber;
                        xlsSheet.Cells[rowCounter, 3].Value = err.ErrorType;
                        xlsSheet.Cells[rowCounter, 4].Value = err.FeatureMember;
                        xlsSheet.Cells[rowCounter, 5].Value = err.LokalnyId;
                        xlsSheet.Cells[rowCounter, 6].Value = err.Istnienie;
                        xlsSheet.Cells[rowCounter, 7].Value = err.Element;
                        xlsSheet.Cells[rowCounter, 8].Value = err.ShortInfo;
                        xlsSheet.Cells[rowCounter, 9].Value = err.LongInfo;
                    }

                    xlsSheet.Cells["A1:I1"].Style.WrapText = true;
                    xlsSheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlsSheet.Cells["A1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    xlsSheet.Cells["A1:I" + rowCounter].AutoFilter = true;
                    xlsSheet.View.FreezePanes(2, 1);
                    xlsSheet.Cells.Style.Font.Size = 10;
                    
                    xlsSheet.Column(1).Width = 9;
                    xlsSheet.Column(2).Width = 9;
                    xlsSheet.Column(3).Width = 11;
                    xlsSheet.Column(4).Width = 35;
                    xlsSheet.Column(5).Width = 36;
                    xlsSheet.Column(6).Width = 14;
                    xlsSheet.Column(7).Width = 14;
                    xlsSheet.Column(8).Width = 50;
                    xlsSheet.Column(9).Width = 14;

                    xlsWorkbook.Save();

                    break;
            }

            Console.WriteLine("Koniec!");
            Console.ReadKey();

        }

    }
}
