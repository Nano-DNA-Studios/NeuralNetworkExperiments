using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNAMath;
using static System.Math;

namespace DNANeuralNet
{
	[System.Serializable]
	public struct DNAActivation
	{
		[System.Serializable]
		public enum ActivationType
		{
			Sigmoid,
			TanH,
			ReLU,
			SiLU,
			Softmax
		}

		public static IDNAActivation GetActivationFromType(ActivationType type)
		{
			switch (type)
			{
				case ActivationType.Sigmoid:
					return new Sigmoid();
				case ActivationType.TanH:
					return new TanH();
				case ActivationType.ReLU:
					return new ReLU();
				case ActivationType.SiLU:
					return new SiLU();
				case ActivationType.Softmax:
					return new Softmax();
				default:
					UnityEngine.Debug.LogError("Unhandled activation type");
					return new Sigmoid();
			}
		}

		public static IDNAActivation GetActivationFromIndex(int activationIndex)
		{
			switch (activationIndex)
			{
				case 1:
					return new Sigmoid();
				case 2:
					return new TanH();
				case 3:
					return new ReLU();
				case 4:
					return new SiLU();
				case 5:
					return new Softmax();
				default:
					UnityEngine.Debug.LogError("Unhandled activation type");
					return new Sigmoid();
			}
		}

		[System.Serializable]
		public readonly struct Sigmoid : IDNAActivation
		{
			public DNAMatrix Activate(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
					activation[i] = 1.0 / (1 + Exp(-matrix[i]));

				return activation;
			}

			public DNAMatrix Derivative(DNAMatrix matrix)
			{
				DNAMatrix activation = Activate(matrix);

				for (int i = 0; i < activation.Length; i++)
					activation[i] = activation[i] * (1 - activation[i]);

				return activation;
			}

			public ActivationType GetActivationType()
			{
				return ActivationType.Sigmoid;
			}

			public int GetActivationFunctionIndex ()
            {
				return 1;
            }
		}

		[System.Serializable]
		public readonly struct TanH : IDNAActivation
		{
			public DNAMatrix Activate(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
                {
					double e2 = Exp(2 * matrix[i]);
					activation[i] = (e2 - 1) / (e2 + 1);
				}

				return activation;
			}

			public DNAMatrix Derivative(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
                {
					double e2 = Exp(2 * matrix[i]);
					double t = (e2 - 1) / (e2 + 1);
					activation[i] = 1 - t * t;
				}

				return activation;
			}

			public ActivationType GetActivationType()
			{
				return ActivationType.TanH;
			}

			public int GetActivationFunctionIndex()
			{
				return 2;
			}
		}

		[System.Serializable]
		public readonly struct ReLU : IDNAActivation
		{
			public DNAMatrix Activate(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
					activation[i] = Max(0, matrix[i]);

				return activation;
			}

			public DNAMatrix Derivative(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
					activation[i] = (matrix[i] > 0) ? 1 : 0;

				return activation;
			}

			public ActivationType GetActivationType()
			{
				return ActivationType.ReLU;
			}

			public int GetActivationFunctionIndex()
			{
				return 3;
			}
		}

		[System.Serializable]
		public readonly struct SiLU : IDNAActivation
		{
			public DNAMatrix Activate(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
					activation[i] = matrix[i] / (1 + Exp(-matrix[i]));

				return activation;
			}

			public DNAMatrix Derivative(DNAMatrix matrix)
			{
				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
                {
					double sig = 1 / (1 + Exp(-matrix[i]));
					activation[i] = matrix[i] * sig * (1 - sig) + sig;
				}
					
				return activation;
			}

			public ActivationType GetActivationType()
			{
				return ActivationType.SiLU;
			}

			public int GetActivationFunctionIndex()
			{
				return 4;
			}
		}

		[System.Serializable]
		public readonly struct Softmax : IDNAActivation
		{
			public DNAMatrix Activate(DNAMatrix matrix)
			{
				double expSum = 0;

				for (int i = 0; i < matrix.Length; i++)
					expSum += Exp(matrix[i]);

				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
					activation[i] = Exp(matrix[i]) / expSum;

				return activation;
			}

			public DNAMatrix Derivative(DNAMatrix matrix)
			{
				double expSum = 0;

				for (int i = 0; i < matrix.Length; i++)
					expSum += Exp(matrix[i]);

				DNAMatrix activation = new DNAMatrix(matrix.Height, matrix.Width);

				for (int i = 0; i < activation.Length; i++)
                {
					double ex = Exp(matrix[i]);
					activation[i] = (ex * expSum - ex * ex) / (expSum * expSum);
				}

				return activation;
			}

			public ActivationType GetActivationType()
			{
				return ActivationType.Softmax;
			}

			public int GetActivationFunctionIndex()
			{
				return 5;
			}
		}
	}
}