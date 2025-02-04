using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.Services;

namespace TestServerMSI.Application.DLLs
{
    public class AlgorithmCapsule : IOptimizationAlgorithm
    {
        private object _algorithmFromDLL;
        private Type _algorithmType;
        private Type? _fitnessType;

        public AlgorithmCapsule(object algorithmFromDLL, Type? fitnessTypeFromDLL)
        {
            _algorithmFromDLL = algorithmFromDLL;
            _algorithmType = _algorithmFromDLL.GetType();
            _fitnessType = fitnessTypeFromDLL;

            var writerType = _algorithmType.GetProperty("writer").PropertyType;
            var readerType = _algorithmType.GetProperty("reader").PropertyType;

            var writerCapsule = Encapsulator.generateCapsule(writerType, typeof(StateWriter), "SaveToFileStateOfAlgorithm", "StateWriterCapsule");
            _algorithmType.GetProperty("writer").SetValue(_algorithmFromDLL, writerCapsule);

            var readerCapsule = Encapsulator.generateCapsule(readerType, typeof(StateReader), "LoadFromFileStateOfAlgorithm", "StateReaderCapsule");
            _algorithmType.GetProperty("reader").SetValue(_algorithmFromDLL, readerCapsule);

            stringReportGenerator = new GenerateTextReport();
            pdfReportGenerator = new GeneratePDFReport();
        }
        public string Name
        {
            get
            {
                return (string)_algorithmType.GetProperty("Name").GetValue(_algorithmFromDLL);
            }
            set
            {
                _algorithmType.GetProperty("Name").SetValue(_algorithmFromDLL, value);
            }
        }
        private ParamInfo[]? _paramInfos = null;
        public ParamInfo[] ParamsInfo
        {
            get
            {
                if (_paramInfos == null)
                {
                    _paramInfos = readParamsInfo();
                }
                return _paramInfos;
            }
            set
            {
                _algorithmType.GetProperty("ParamsInfo").SetValue(_algorithmFromDLL, value);
            }
        }
        public IStateWriter writer { get; set; }
        public IStateReader reader { get; set; }
        public IGenerateTextReport stringReportGenerator { get; set; }
        public IGeneratePDFReport pdfReportGenerator { get; set; }
        public double[] XBest
        {
            get => (double[])_algorithmType.GetProperty("XBest").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("XBest").SetValue(_algorithmFromDLL, value);
        }
        public double FBest
        {
            get => (double)_algorithmType.GetProperty("FBest").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("FBest").SetValue(_algorithmFromDLL, value);
        }
        public int NumberOfEvaluationFitnessFunction
        {
            get => (int)_algorithmType.GetProperty("NumberOfEvaluationFitnessFunction").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("NumberOfEvaluationFitnessFunction").SetValue(_algorithmFromDLL, value);
        }
        public int CurrentIteration
        {
            get => (int)_algorithmType.GetProperty("CurrentIteration").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("CurrentIteration").SetValue(_algorithmFromDLL, value);
        }
        public double[][] Population
        {
            get => (double[][])_algorithmType.GetProperty("Population").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("Population").SetValue(_algorithmFromDLL, value);
        }
        public double[] PopulationValues
        {
            get => (double[])_algorithmType.GetProperty("PopulationValues").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("PopulationValues").SetValue(_algorithmFromDLL, value);
        }
        public bool Stop
        {
            get => (bool)_algorithmType.GetProperty("Stop").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("Stop").SetValue(_algorithmFromDLL, value);
        }
        public Dictionary<string, double> ParametersUsedValues
        {
            get => (Dictionary<string, double>)_algorithmType.GetProperty("ParametersUsedValues").GetValue(_algorithmFromDLL);
            set => _algorithmType.GetProperty("ParametersUsedValues").SetValue(_algorithmFromDLL, value);
        }

        public void Solve(fitnessFunction f, double[,] domain, params double[] parameters)
        {
            MethodInfo solveMethod = _algorithmType.GetMethod("Solve");
            if (_fitnessType == null)
            {
                solveMethod.Invoke(_algorithmFromDLL, [f, domain, parameters]);
            }
            else
            {
                var ff = Delegate.CreateDelegate(_fitnessType, f.Target, f.Method);
                solveMethod.Invoke(_algorithmFromDLL, [ff, domain, parameters]);
            }

        }

        private ParamInfo[] readParamsInfo()
        {
            object[] parameters = _algorithmType.GetProperty("ParamsInfo").GetValue(_algorithmFromDLL) as object[];
            Type type = parameters[0].GetType();
            int paramsNumber = parameters.Length;
            ParamInfo[] paramInfos = new ParamInfo[paramsNumber];
            for (int i = 0; i < paramsNumber; i++)
            {
                paramInfos[i] = new ParamInfo(
                    (string)type.GetProperty("Name").GetValue(parameters[i]),
                    (string)type.GetProperty("Description").GetValue(parameters[i]),
                    (double)type.GetProperty("UpperBoundary").GetValue(parameters[i]),
                    (double)type.GetProperty("LowerBoundary").GetValue(parameters[i])
                    );
            }

            return paramInfos;
        }
    }
}
