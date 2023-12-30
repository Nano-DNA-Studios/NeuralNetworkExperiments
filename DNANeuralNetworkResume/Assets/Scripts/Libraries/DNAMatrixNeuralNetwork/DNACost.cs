using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNAMath;
using static System.Math;

namespace DNANeuralNet
{
    public class DNACost
    {
		public enum CostType
		{
			MeanSquareError,
			CrossEntropy
		}

		public static IDNACost GetCostFromType(CostType type)
		{
			switch (type)
			{
				case CostType.MeanSquareError:
					return new MeanSquaredError();
				case CostType.CrossEntropy:
					return new CrossEntropy();
				default:
					UnityEngine.Debug.LogError("Unhandled cost type");
					return new MeanSquaredError();
			}
		}

		public class MeanSquaredError : IDNACost
		{
			public double CostFunction(DNAMatrix predictedOutputs, DNAMatrix expectedOutputs)
			{
				// cost is sum (for all x,y pairs) of: 0.5 * (x-y)^2
				double cost = 0;
				for (int i = 0; i < predictedOutputs.Length; i++)
				{
					double error = predictedOutputs[i] - expectedOutputs[i];
					cost += error * error;
				}
				return 0.5 * cost;
			}

			public DNAMatrix CostDerivative(DNAMatrix predictedOutput, DNAMatrix expectedOutput)
			{
				return predictedOutput - expectedOutput;
			}

			public CostType CostFunctionType()
			{
				return CostType.MeanSquareError;
			}

			public int GetCostIndex()
			{
				return 1;
			}
		}

		public class CrossEntropy : IDNACost
		{
			// Note: expected outputs are expected to all be either 0 or 1
			public double CostFunction(DNAMatrix predictedOutputs, DNAMatrix expectedOutputs)
			{
				// cost is sum (for all x,y pairs) of: 0.5 * (x-y)^2
				double cost = 0;
				for (int i = 0; i < predictedOutputs.Length; i++)
				{
					double x = predictedOutputs[i];
					double y = expectedOutputs[i];
					double v = (y == 1) ? -Log(x) : -Log(1 - x);
					cost += double.IsNaN(v) ? 0 : v;
				}
				return cost;
			}

			public DNAMatrix CostDerivative(DNAMatrix predictedOutput, DNAMatrix expectedOutput)
			{
				DNAMatrix cost = new DNAMatrix(predictedOutput.Height, predictedOutput.Width);
				for (int i = 0; i < cost.Length; i ++)
                {
					double x = predictedOutput[i];
					double y = expectedOutput[i];
					if (x == 0 || x == 1)
					{
						cost[i] = 0;
					}
					cost[i] = (-x + y) / (x * (x - 1));
				}

				return cost;
			}

			public CostType CostFunctionType()
			{
				return CostType.CrossEntropy;
			}

			public int GetCostIndex ()
            {
				return 2;
            }
		}
	}
}

