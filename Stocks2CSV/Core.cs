using System.IO;
using System.Collections;
using System.Collections.Generic;
using PdfToText;

public class Core
{
    public bool debugMode = false;
    public string inDir;
    public string outDir;
    string[] allData;
    string[] data;

    public void DoWork()
    {
        string[] allFiles = Directory.GetFiles(inDir, "*.pdf");
        List<string> finalData = new List<string>();

        for(int i = 0; i < allFiles.Length; i++)
        {
            string iFile = allFiles[i];

            PDFParser pdfParser = new PDFParser();
            allData = pdfParser.ExtractTextArray(iFile);
            data = allData[0].Split('\n');


            for(int j = 0; j < data.Length; j++)
            {
                if(data[j].Length > 1) 
                {
                    data[j] = data[j].Remove(0, 1);
                    if(data[j][data[j].Length - 1] == ' ') data[j] = data[j].Remove(data[j].Length - 1, 1);
                }
            }

            // Get format
            int formatType = 0;

            int lnDate   = 0;
            int lnName   = 0;
            int lnAction = 0;
            int lnTicker = 0;
            int lnAmount = 0;
            int lnPrice  = 0;

            string fData = "";

            if(data[1].Contains("SECURITY") && data[3].Contains("COMPANY"))
            {
                formatType = 1;
            }

            if(data[1].Contains("Locked Bag") && data[5].Contains("Square"))
            {
                if (GetLine("ORDINARY FULLY PAID") >= 0) formatType = 2;
                else formatType = 3;
            }

            if(data[3].Contains("SECURITY") && data[5].Contains("COMPANY"))
            {
                formatType = 1;
            }

            if(formatType == 1)
            {
                lnDate   = GetLine("TOTAL COST") + 2;
                lnName   = lnDate + 4;
                lnAction = GetLine("TAX INVOICE") + 2;
                lnTicker = GetLine("WE HAVE BOUGHT THE FOLLOWING SECURITIES FOR YOU") + 10;
                lnAmount = lnDate + 20;
                lnPrice  = GetLine("PAYMENT METHOD - DIRECT DEBIT OF CLEARED") - 2;
            }

            if(formatType == 2)
            {
                lnDate   = GetLine("CONSIDERATION (AUD)") + 2;
                lnName   = GetLine("CONFIRMATION NO") + 2;
                lnAction = GetLine("TAX INVOICE") + 2;
                lnTicker = GetLine("ORDINARY FULLY PAID") + 4;
                lnAmount = GetLine("CONFIRMATION NO") + 6;
                lnPrice  = GetLine("AVERAGE PRICE") + 2;
            }

            // An alternate of 2
            if(formatType == 3)
            {
                lnDate = GetLine("CONSIDERATION (AUD)") + 2;
                lnName = GetLine("CONFIRMATION NO") + 2;
                lnAction = GetLine("TAX INVOICE") + 2;
                lnTicker = GetLine("ORDER COMPLETED") + 8;
                lnAmount = GetLine("CONFIRMATION NO") + 6;
                lnPrice = GetLine("AVERAGE PRICE") + 2;
            }
            
            if(formatType != 0)
            {
                data[lnName] = "C" + data[lnName];

                if(data[lnAction] == "BUY") data[lnAction] = "B";
                if(data[lnAction] == "SELL") data[lnAction] = "S";

                data[lnAmount] = data[lnAmount].Replace(",", "");

                fData = fData + "\"" + data[lnDate]   + "\"" + ",";
                fData = fData + "\"" + data[lnName]   + "\"" + ",";
                fData = fData + "\"" + data[lnAction] + "\"" + ",";
                fData = fData + "\"" + data[lnTicker] + "\"" + ",";
                fData = fData + "\"" + data[lnAmount] + "\"" + ",";
                fData = fData + "\"" + data[lnPrice]  + "\""      ;
            }

            finalData.Add(fData);
        }

        if(debugMode)
        {
            PrintDebug();
        }

        string oFile = Path.Combine(outDir, "output.txt");
        File.WriteAllLines(oFile, finalData);
    }

    public void PrintDebug()
    {
        string[] allFiles = Directory.GetFiles(inDir, "*.pdf");

        for(int i = 0; i < allFiles.Length; i++)
        {
            string iFile = allFiles[i];

            PDFParser pdfParser = new PDFParser();
            allData = pdfParser.ExtractTextArray(iFile);
            data = allData[0].Split('\n');

            for(int j = 0; j < data.Length; j++)
            {
                if(data[j].Length > 1) 
                {
                    data[j] = data[j].Remove(0, 1);
                    if(data[j][data[j].Length - 1] == ' ') data[j] = data[j].Remove(data[j].Length - 1, 1);
                }
            }
        
            string tData = "";
            for(int j = 0; j < data.Length; j++)
            {
                data[j] = data[j].Replace('\n', ' ');
                tData = tData + ("data: " + j + " | " + data[j]) + "\n";
                System.Console.WriteLine("data: " + j + " | " + data[j]);
            }
            
            string oDat = Path.Combine(outDir, Path.GetFileNameWithoutExtension(allFiles[i]) + ".txt");
            File.WriteAllText(oDat, tData);
        }
    }

    int GetLine(string f)
    {
        for(int i = 0; i < data.Length; i++)
        {
            if(data[i].Contains(f)) return i;
        }

        return -1;
    }
}
