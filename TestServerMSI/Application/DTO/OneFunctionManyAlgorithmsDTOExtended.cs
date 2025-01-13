using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.TestFunctions;

namespace TestServerMSI.Application.DTO
{
    public class OneFunctionManyAlgorithmsDTOExtended
    {
        public OneFunctionManyAlgorithmsDTO OneFunctionManyAlgorithms { get; set; }
        public int IterationsMade { get; set; }
        public int AlgorithmsChecked { get; set; }

        public OneFunctionManyAlgorithmsDTOExtended() {
            OneFunctionManyAlgorithms = new OneFunctionManyAlgorithmsDTO();
            AlgorithmsChecked = 0;
            IterationsMade = 0;
        }

        public OneFunctionManyAlgorithmsDTOExtended(OneFunctionManyAlgorithmsDTO ofma, int iterationsMade, int algorithmsChecked)
        {
            OneFunctionManyAlgorithms = ofma;
            AlgorithmsChecked = algorithmsChecked;
            IterationsMade = iterationsMade;
        }
    }
}
