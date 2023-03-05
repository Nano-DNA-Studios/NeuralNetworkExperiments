using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNANeuralNetwork
{
	public interface IActivation
	{
		double Activate(double[] inputs, int index);

		double Derivative(double[] inputs, int index);

		Activation.ActivationType GetActivationType();
	}
}

