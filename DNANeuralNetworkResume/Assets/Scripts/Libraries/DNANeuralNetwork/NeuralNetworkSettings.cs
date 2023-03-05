using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNANeuralNetwork
{
	[System.Serializable]
	public class NeuralNetworkSettings
	{
		public int[] networkSize;

		public Activation.ActivationType activationType;
		public Activation.ActivationType outputActivationType;
		public Cost.CostType costType;

		public double initialLearningRate;
		public double learnRateDecay;
		public int dataPerBatch;
		public double momentum;
		public double regularization;

		public NeuralNetworkSettings()
		{
			activationType = Activation.ActivationType.ReLU;
			outputActivationType = Activation.ActivationType.Softmax;
			costType = Cost.CostType.CrossEntropy;
			initialLearningRate = 0.05;
			learnRateDecay = 0.075;
			dataPerBatch = 32;
			momentum = 0.9;
			regularization = 0.1;
		}
	}
}

