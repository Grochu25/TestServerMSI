using TestServerMSI.Application.DTO;

namespace TestServerMSI.Application
{
    public class QueueSavers
    {
        public static void saveOAMFdtoToFile(OneAlgorithmManyFunctionsDTO oamf)
        {
            string str = oamf.AlgorithmName + "\n";
            str += oamf.Domain[0].Length.ToString() + "\n";

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < oamf.Domain[0].Length; j++)
                    str += oamf.Domain[i][j] + " ";
                str += "\n";
            }
            str += oamf.Parameters.Length.ToString() + "\n";
            for (int i = 0; i < oamf.Parameters.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                    str += oamf.Parameters[i][j] + " ";
                str += "\n";
            }
            str += oamf.TestFunctionNames.Length + "\n";
            for (int i = 0; i < oamf.TestFunctionNames.Length; i++)
                str += oamf.TestFunctionNames[i] + "\n";

            str += 0 + "\n";
            str += 0 + "\n";
            str += CalculationProcessor.Instance.saveDirectory + "\n";
            StreamWriter sw = new StreamWriter("savedAlgorithms/OAMF.dto");
            sw.Write(str);
            sw.Close();
        }

        public static void saveOFMAdtoToFile(OneFunctionManyAlgorithmsDTO ofma)
        {
            string str = ofma.TestFunctionName + "\n";
            str += ofma.Domain[0].Length.ToString() + "\n";

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < ofma.Domain[0].Length; j++)
                    str += ofma.Domain[i][j] + " ";
                str += "\n";
            }
            str += ofma.Parameters.Length.ToString() + "\n";
            for (int i = 0; i < ofma.Parameters.Length; i++)
            {
                for (int j = 0; j < ofma.Parameters[i].Length; j++)
                    str += ofma.Parameters[i][j] + " ";
                str += "\n";
            }
            str += ofma.AlgorithmNames.Length + "\n";
            for (int i = 0; i < ofma.AlgorithmNames.Length; i++)
                str += ofma.AlgorithmNames[i] + "\n";

            str += 0 + "\n";
            str += 0 + "\n";
            str += CalculationProcessor.Instance.saveDirectory + "\n";
            StreamWriter sw = new StreamWriter("savedAlgorithms/OFMA.dto");
            sw.Write(str);
            sw.Close();
        }

        public static OneAlgorithmManyFunctionsDTOExtended? readOAMFdtoFromFile()
        {
            if (!File.Exists("savedAlgorithms/OAMF.dto")) return null;
            OneAlgorithmManyFunctionsDTOExtended oamfExtended = new OneAlgorithmManyFunctionsDTOExtended();
            OneAlgorithmManyFunctionsDTO oamf = new OneAlgorithmManyFunctionsDTO();
            StreamReader sr = new StreamReader("savedAlgorithms/OAMF.dto");

            oamf.AlgorithmName = sr.ReadLine();
            oamf.Domain = new double[2][];
            int dimensions = int.Parse(sr.ReadLine());
            for (int i = 0; i < 2; i++)
                oamf.Domain[i] = sr.ReadLine().Trim().Split(' ').Select(x => double.Parse(x)).ToArray();
            int numParam = int.Parse(sr.ReadLine());
            oamf.Parameters = new double[numParam][];
            for (int i = 0; i < numParam; i++)
                oamf.Parameters[i] = sr.ReadLine().Trim().Split(' ').Select(x => double.Parse(x)).ToArray();
            int numFun = int.Parse(sr.ReadLine());
            oamf.TestFunctionNames = new string[numFun];
            for (int i = 0; i < numFun; i++)
                oamf.TestFunctionNames[i] = sr.ReadLine();

            oamfExtended.OneAlgorithmManyFunctions = oamf;
            oamfExtended.IterationsMade = int.Parse(sr.ReadLine());
            oamfExtended.FunctionsChecked = int.Parse(sr.ReadLine());
            CalculationProcessor.Instance.saveDirectory = sr.ReadLine();

            sr.Close();
            return oamfExtended;
        }

        public static OneFunctionManyAlgorithmsDTOExtended? readOFMAdtoFromFile()
        {
            if (!File.Exists("savedAlgorithms/OFMA.dto")) return null;
            OneFunctionManyAlgorithmsDTOExtended ofmaExtended = new OneFunctionManyAlgorithmsDTOExtended();
            OneFunctionManyAlgorithmsDTO ofma = new OneFunctionManyAlgorithmsDTO();
            StreamReader sr = new StreamReader("savedAlgorithms/OFMA.dto");

            ofma.TestFunctionName = sr.ReadLine();
            ofma.Domain = new double[2][];
            int dimensions = int.Parse(sr.ReadLine());
            for (int i = 0; i < 2; i++)
                ofma.Domain[i] = sr.ReadLine().Trim().Split(' ').Select(x => double.Parse(x)).ToArray();
            int numParam = int.Parse(sr.ReadLine());
            ofma.Parameters = new double[numParam][];
            for (int i = 0; i < numParam; i++)
                ofma.Parameters[i] = sr.ReadLine().Trim().Split(' ').Select(x => double.Parse(x)).ToArray();
            int numAlg = int.Parse(sr.ReadLine());
            ofma.AlgorithmNames = new string[numAlg];
            for (int i = 0; i < numAlg; i++)
                ofma.AlgorithmNames[i] = sr.ReadLine();

            ofmaExtended.OneFunctionManyAlgorithms = ofma;
            ofmaExtended.IterationsMade = int.Parse(sr.ReadLine());
            ofmaExtended.AlgorithmsChecked = int.Parse(sr.ReadLine());
            CalculationProcessor.Instance.saveDirectory = sr.ReadLine();

            sr.Close();
            return ofmaExtended;
        }
    }
}
