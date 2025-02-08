using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TestServerMSI.Application.Alogrithms;

namespace TestServerMSI.Application.Services
{
    public class ComparisonReportGenerator
    {
        private DirectoryInfo reportDirecotry;

        private string[] algorithms;
        private double[][] bestEntities;
        private double[] fitnessValue;
        private int[] numberOfEval;
        private string[] functionName;
        public double[][] parameters { get; set; } //Dwa pierwsze to zawsze population i iterations

        public ComparisonReportGenerator(string reportDirecotryPath)
        {
            if (!Directory.Exists($"records/{reportDirecotryPath}"))
                Directory.CreateDirectory($"records/{reportDirecotryPath}");

            reportDirecotry = new DirectoryInfo($"records/{reportDirecotryPath}");
            int numberOfReports = reportDirecotry.GetFiles().ToList().Where(x => x.Extension == ".txt").Count();

            algorithms = new string[numberOfReports];
            bestEntities = new double[numberOfReports][];
            fitnessValue = new double[numberOfReports];
            numberOfEval = new int[numberOfReports];
            functionName = new string[numberOfReports];
            parameters = new double[numberOfReports][];
        }

        public void generateComparisonReport()
        {
            pullDataFromAllRaports();
            saveToPDFTable($"{reportDirecotry.FullName}/ComparisonReport.pdf");
        }

        private void saveToPDFTable(string path)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Comparison Report";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 10, XFontStyleEx.Regular);
            XFont fontBold = new XFont("Verdana", 10, XFontStyleEx.Bold);

            double startX = 50;
            double startY = 50;
            double colWidth = 120;
            double rowHeight = 20;

            string[] labels = new string[bestEntities[0].Length + 7];
            labels[0] = "algorytm";
            labels[1] = "funkcja";
            labels[2] = "wywolań funkcji celu";
            labels[3] = "populacja";
            labels[4] = "iteracje";
            labels[5] = "fitness value";
            labels[labels.Length-1] = "reszta parametrów";

            for (int i = 0; i < bestEntities[0].Length; i++)
                labels[i + 6] = "best wym. " + (i + 1);

            string[][] values = string2dOfDouble2D();
            string[] headers = new string[algorithms.Length + 1];
            headers[0] = labels[0];
            for(int i=1; i< headers.Length; i++)
                headers[i] = values[i-1][0];

            

            // Params info values
            for (int col = -1; col < headers.Length-1; col++)
            {
                if(col>0 && col%5 == 0)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharp.PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(page);
                }

                double offsetX = startX + (((col + 1)%6) * colWidth);

                for (int row = 0; row < labels.Length; row++)
                {
                    double offsetY = startY + (row * rowHeight);
                    if (row == 0)
                    {
                        gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, offsetX, offsetY, colWidth, rowHeight);
                        gfx.DrawString(headers[col+1], fontBold, XBrushes.Black, offsetX + 5, offsetY + 15);
                    }
                    // Row Titles
                    else
                    {
                        if (col == -1)
                        {
                            gfx.DrawRectangle(XPens.Black, XBrushes.White, startX, offsetY, colWidth, rowHeight);
                            gfx.DrawString(labels[row], font, XBrushes.Black, startX + 5, offsetY + 15);
                        }
                        else
                        {
                            gfx.DrawRectangle(XPens.Black, XBrushes.White, offsetX, offsetY, colWidth, rowHeight);
                            gfx.DrawString(values[col][row], font, XBrushes.Black, offsetX + 5, offsetY + 15);
                        }
                    }
                }
            }


            document.Save(path);
        }

        private string[][] string2dOfDouble2D()
        {
            int tableHeight = bestEntities[0].Length + 7;
            string[][] retVal = new string[algorithms.Length][];
            for (int i = 0; i < algorithms.Length; ++i)
            {
                retVal[i] = new string[tableHeight];
                retVal[i][0] = algorithms[i].Split(' ')[0];
                retVal[i][1] = functionName[i];
                retVal[i][2] = numberOfEval[i].ToString();
                retVal[i][3] = parameters[i][0].ToString();
                retVal[i][4] = parameters[i][1].ToString();
                retVal[i][5] = ((decimal)fitnessValue[i]).ToString();
                for (int j = 0; j < bestEntities[i].Length; ++j)
                {
                    retVal[i][j+6] = bestEntities[i][j].ToString();
                }
                for (int j = 2; j < parameters[i].Length; ++j)
                {
                    retVal[i][retVal[i].Length-1] += parameters[i][j]+"; ";
                }
            }
            return retVal;
        }

        private void pullDataFromAllRaports()
        {
            int index = 0;

            foreach (FileInfo file in reportDirecotry.GetFiles().OrderBy(p => p.CreationTime).ToArray())
            {
                if (file.Extension == ".txt")
                {
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        readForIndex(file, index);
                    }
                    index++;
                }
            }
        }

        private void readForIndex(FileInfo file, int index)
        {
            using (StreamReader sr = new StreamReader(file.FullName))
            {
                string name = sr.ReadLine();
                algorithms[index] = name.Substring(name.IndexOf(":") + 1).Trim();
                string bestEntStr = sr.ReadLine();
                bestEntities[index] = pullBestEnt(bestEntStr.Substring(bestEntStr.IndexOf(":") + 1).Trim());
                string fitnessVal = sr.ReadLine();
                fitnessValue[index] = double.Parse(fitnessVal.Substring(fitnessVal.IndexOf(":") + 1).Trim());
                string numOfEval = sr.ReadLine();
                numberOfEval[index] = int.Parse(numOfEval.Substring(numOfEval.IndexOf(":") + 1).Trim());
                sr.ReadLine();
                string testFunName = sr.ReadLine();
                functionName[index] = testFunName.Substring(testFunName.IndexOf(":") + 1).Trim();
                sr.ReadLine();
                parameters[index] = readParametersFor(algorithms[index], sr);
            }
        }

        private double[] pullBestEnt(string endStr)
        {
            endStr = endStr.Substring(1, endStr.Length - 2);
            double[] result = new double[endStr.Count(x => x == ';') + 1];

            int i = 0;
            foreach (string oneDim in endStr.Split(';'))
            {
                result[i] = double.Parse(oneDim);
                i++;
            }

            return result;
        }

        private double[] readParametersFor(string algorithm, StreamReader sr)
        {
            int numberOfParams = Algorithms.getAlgorithm(algorithm).ParamsInfo.Length;
            double[] result = new double[numberOfParams];

            for (int i = 0; i < numberOfParams; i++)
            {
                sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
                string parameter = sr.ReadLine();
                result[i] = double.Parse(parameter.Substring(parameter.IndexOf(":") + 1).Trim());
            }

            return result;
        }
    }
}
