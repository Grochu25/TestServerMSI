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
        public int marginSize = 20;

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

            string algName = $"Algorithm name: {this.Alg.Name}";
            string bestString = $"Best entity: {this.stringOfDoubleArray(this.Alg.XBest)}";
            string fitnessString = $"Its fitness value: {this.Alg.FBest.ToString(this.FloatFormat)}";
            string NumEvalString = $"Number of evaluation fitness function: {this.Alg.NumberOfEvaluationFitnessFunction.ToString()}\n";

            string paramsInfoString = this.getParamsInfoString(this.Alg.ParamsInfo);
            string[] memberContent = new string[] { algName, bestString, fitnessString, NumEvalString, paramsInfoString };
            this.saveToMember(memberContent);

            List<string> fileContent = new List<string>();
            fileContent.AddRange(this.wrapText(algName));
            fileContent.AddRange(this.wrapText(bestString));
            fileContent.AddRange(this.wrapText(fitnessString));
            fileContent.AddRange(this.wrapText(NumEvalString));
            foreach (ParamInfo paramInfo in this.Alg.ParamsInfo)
            {
                fileContent.AddRange(this.stringOfParamInfoPdf(paramInfo));
            }
            this.saveToFile(path, fileContent.ToArray());
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

        public string getParamsInfoString(ParamInfo[] paramsInfo)
        {
            string paramsInfoString = "";
            foreach (ParamInfo paramInfo in paramsInfo)
            {
                paramsInfoString += this.stringOfParamInfo(paramInfo) + "\n";
            }
            return paramsInfoString;
        }

        public string stringOfParamInfo(ParamInfo paramInfo)
        {
            return String.Format(
                "{0}:\n\tDescription: {1}\n\tRange:       [{2} - {3}]",
                paramInfo.Name,
                paramInfo.Description,
                paramInfo.LowerBoundary,
                paramInfo.UpperBoundary
            );
        }

        public void saveToMember(string[] content)
        {
            this.ReportString = String.Empty;
            foreach (string line in content)
            {
                this.ReportString += line + "\n";
            }
        }

        // TODO: Implement this if needed.
        // Function that wraps long text cuz pdf doesn't support '\n'
        // and doens't wraps by itself.
        public string[] wrapText(string text)
        {
            return new string[] { text };
        }


        public string[] stringOfParamInfoPdf(ParamInfo paramInfo)
        {
            string formatted = String.Format(
                "{0}:\n    Description: {1}\n    Range:       [{2} - {3}]",
                paramInfo.Name,
                paramInfo.Description,
                paramInfo.LowerBoundary,
                paramInfo.UpperBoundary
            );
            List<string> retVal = new List<string>();
            foreach (string line in formatted.Split("\n"))
            {
                retVal.AddRange(this.wrapText(line));
            }
            return retVal.ToArray();
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
