using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DTO
{
    public class OneAlgorithmManyFunctionsDTOExtended
    {
        public OneAlgorithmManyFunctionsDTO OneAlgorithmManyFunctions { get; set; }
        public int IterationsMade { get; set; }
        public int FunctionsChecked { get; set; }

        public OneAlgorithmManyFunctionsDTOExtended() 
        {
            OneAlgorithmManyFunctions = new OneAlgorithmManyFunctionsDTO();
            FunctionsChecked = 0;
            IterationsMade = 0;
        }

        public OneAlgorithmManyFunctionsDTOExtended(OneAlgorithmManyFunctionsDTO oamf, int functionsChecked, int iterationsMade)
        {
            OneAlgorithmManyFunctions = oamf;
            FunctionsChecked = functionsChecked;
            IterationsMade = iterationsMade;
        }

    }
}
