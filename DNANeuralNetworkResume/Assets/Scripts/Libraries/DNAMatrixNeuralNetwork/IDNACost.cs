using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNAMath;

namespace DNANeuralNet
{
	public interface IDNACost
	{
		double CostFunction(DNAMatrix predictedOutputs, DNAMatrix expectedOutputs);

		DNAMatrix CostDerivative(DNAMatrix predictedOutput, DNAMatrix expectedOutput);

		DNACost.CostType CostFunctionType();

		int GetCostIndex();
	}
}

