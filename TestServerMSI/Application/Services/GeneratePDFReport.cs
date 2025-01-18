using System.Diagnostics;
using PdfSharp.Charting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.Services
{
    public class GeneratePDFReport : IGeneratePDFReport
    {
        public uint Precision { get; set; }
        public string ReportString { get; set; }
        public IOptimizationAlgorithm Alg { get; set; }

        public string FloatFormat { get; set; }

        public GeneratePDFReport()
        {
            Debug.WriteLine(
                "No algorithm set. try:\n\t" +
                "[generator as variable].Alg = [algorithm as IOptimizationAlgorithm]\n\n" +
                "before using GenerateReport(string path)\n"
            );
            this.ReportString = String.Empty;
        }

        public void GenerateReport(string path)
        {
            if (this.Alg == null)
            {
                return;
            }
            this.FloatFormat = this.buildFloatFormat();
            string bestString = $"Best entity:                           {this.stringOfDoubleArray(this.Alg.XBest)}";
            string fitnessString = $"Its fitness value:                     {this.Alg.FBest.ToString(this.FloatFormat)}";
            string NumEvalString = $"Number of evaluation fitness function: {this.Alg.NumberOfEvaluationFitnessFunction.ToString()}\n";

            string paramsInfoString = "";
            foreach (ParamInfo paramInfo in this.Alg.ParamsInfo)
            {
                paramsInfoString += this.stringOfParamInfo(paramInfo) + "\n";
            }

            string[] content = new string[] { bestString, fitnessString, NumEvalString, paramsInfoString };
            this.saveToMember(content);


            this.saveToFile(path, content);
        }

        public string stringOfDoubleArray(double[] source)
        {
            string retVal = "[";
            for (int i = 0; i < source.Length; ++i)
            {
                retVal += source[i].ToString(this.FloatFormat);
                if (i < source.Length - 1)
                {
                    retVal += "; ";
                }
            }
            retVal += "]";
            return retVal;
        }

        public string stringOfParamInfo(ParamInfo paramInfo)
        {
            return String.Format(
                "Name: {0}:\n\tDescription: {1}\n\tRange:       [{2} - {3}]",
                paramInfo.Name,
                paramInfo.Description,
                paramInfo.LowerBoundary,
                paramInfo.UpperBoundary
            );
        }
        public string buildFloatFormat()
        {
            string retVal = "0.";
            for (uint i = 0; i < this.Precision; ++i)
            {
                retVal += "0";
            }
            return retVal;
        }

        public void saveToMember(string[] content)
        {
            this.ReportString = String.Empty;
            foreach (string line in content)
            {
                this.ReportString += line + "\n";
            }
        }

        public void saveToFile(string path, string[] content)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = this.Alg.Name + " Report";

            XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            double offsetY = 50;
            foreach (string line in content)
            {
                gfx.DrawString(line, font, XBrushes.Black, 20, offsetY);
                offsetY += 20;
            }

            document.Save(path);
        }
    }
}
