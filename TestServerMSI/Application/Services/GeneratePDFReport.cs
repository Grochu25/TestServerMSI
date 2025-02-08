using System.Diagnostics;
using PdfSharp.Charting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.Services
{
    public class GeneratePDFReport : IGeneratePDFReport
    {
        public uint Precision { get; set; } = 10;
        public string ReportString { get; set; }
        public IOptimizationAlgorithm Alg { get; set; }
        public ITestFunction TF { get; set; }
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


        public void GenerateReport(string path, ComparisonReportGenerator comparisonReportGenerator)
        {
            if (this.Alg == null)
            {
                return;
            }
            this.FloatFormat = this.buildFloatFormat();

            this.saveToPDF(path);
            this.saveToPDFTable(path, comparisonReportGenerator);
        }

        private void saveToPDF(string path)
        {
            string algName = $"Algorithm name: {this.Alg.Name}";
            string bestString = $"Best entity: {this.stringOfDoubleArray(this.Alg.XBest)}";
            string fitnessString = $"Its fitness value: {this.Alg.FBest.ToString(this.FloatFormat)}";
            string NumEvalString = $"Number of evaluation fitness function: {this.Alg.NumberOfEvaluationFitnessFunction.ToString()}";
            string testFunction = $"Test function name: {this.TF.Name}";

            string paramsInfoString = this.getParamsInfoString(this.Alg.ParamsInfo);
            string[] memberContent = new string[] { algName, bestString, fitnessString, NumEvalString, testFunction, paramsInfoString };
            this.saveToMember(memberContent);

            List<string> fileContent = new List<string>();
            fileContent.AddRange(this.wrapText(algName));
            fileContent.AddRange(this.wrapText(bestString));
            fileContent.AddRange(this.wrapText(fitnessString));
            fileContent.AddRange(this.wrapText(NumEvalString));
            fileContent.AddRange(this.wrapText(testFunction));
            foreach (ParamInfo paramInfo in this.Alg.ParamsInfo)
            {
                fileContent.AddRange(this.stringOfParamInfoPdf(paramInfo));
            }
            this.saveToFile(path, fileContent.ToArray());
        }

        private void saveToPDFTable(string path, ComparisonReportGenerator comparisonReportGenerator)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Comparison Report";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 10, XFontStyleEx.Regular);
            XFont fontBold = new XFont("Verdana", 10, XFontStyleEx.Bold);

            double startX = 50;
            double startY = 50;
            double colWidth = 150;
            double rowHeight = 20;

            string[] headers = { "", "Archimedes", "Motyle" };
            string[] labels = { "algorytm", "funkcja", "ilosc wywolań funkcji celu", "populacja", "iteracje",
                        "fitness value", "best wym. 1", "best wym. 2", "best wym. 3",
                        "(opcjonalnie) reszta parametrów" };

            string[][] values = this.string2dOfDouble2D(comparisonReportGenerator.parameters);
            // Headers
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, startX + (i * colWidth), startY, colWidth, rowHeight);
                gfx.DrawString(headers[i], fontBold, XBrushes.Black, startX + (i * colWidth) + 5, startY + 15);
            }

            for (int row = 0; row < labels.Length; row++)
            {
                // Row Titles
                double offsetY = startY + ((row + 1) * rowHeight);
                gfx.DrawRectangle(XPens.Black, XBrushes.White, startX, offsetY, colWidth, rowHeight);
                gfx.DrawString(labels[row], font, XBrushes.Black, startX + 5, offsetY + 15);

                // Params info values
                for (int col = 0; col < 2; col++)
                {
                    double offsetX = startX + ((col + 1) * colWidth);
                    gfx.DrawRectangle(XPens.Black, XBrushes.White, offsetX, offsetY, colWidth, rowHeight);
                    gfx.DrawString(values[row][col], font, XBrushes.Black, offsetX + 5, offsetY + 15);
                }
            }

            document.Save(path);
        }

        private string[][] string2dOfDouble2D(double[][] src)
        {
            string[][] retVal = new string[src.Length][];
            for (int i = 0; i < src.Length; ++i)
            {
                for (int j = 0; j < src[i].Length; ++j)
                {
                    retVal[i][j] = src[i][j].ToString();
                }
            }
            return retVal;
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
                "{0}:\n    Description: {1}\n    Range:       [{2} - {3}]\n    Used:         {4}",
                paramInfo.Name,
                paramInfo.Description,
                paramInfo.LowerBoundary,
                paramInfo.UpperBoundary,
                this.Alg.ParametersUsedValues[paramInfo.Name]
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
            string formatted = this.stringOfParamInfo(paramInfo);
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
