using DNAMath;
using System;
using System.IO;
using UnityEngine;

namespace DNANeuralNet
{
    [System.Serializable]
    public class DNALayer
    {
        [SerializeField]
        private int _numNodeIn;

        [SerializeField]
        private int _numNodeOut;

        public int ActivationIndex;

        [SerializeField]
        public IDNAActivation activation;

        public int NumNodesIn { get { return _numNodeIn; } set { _numNodeIn = value; } }

        public int NumNodesOut { get { return _numNodeOut; } set { _numNodeOut = value; } }

        public DNAMatrix weights;
        public DNAMatrix biases;

        //Cost Gradient With respect to weight and biases
        private DNAMatrix _costGradientWeight;
        private DNAMatrix _costGradientBias;

        public DNAMatrix CostGradientWeight { get { return _costGradientWeight; } set { _costGradientWeight = value; } }
        public DNAMatrix CostGradientBias { get { return _costGradientBias; } set { _costGradientBias = value; } }

        //Momentum
        private DNAMatrix _weightVelocities;
        private DNAMatrix _biasVelocities;

        public DNAGPUParallelization Parallelization { get; set; }

        public int ParallelBatchSize { get; set; } = 32;

        public DNALayer(int numNodesIn, int numNodesOut)
        {
            this.NumNodesIn = numNodesIn;
            this.NumNodesOut = numNodesOut;
            activation = new DNAActivation.Sigmoid();

            weights = new DNAMatrix(numNodesOut, numNodesIn);
            _costGradientWeight = new DNAMatrix(numNodesOut, numNodesIn);
            _weightVelocities = new DNAMatrix(numNodesOut, numNodesIn);

            biases = new DNAMatrix(numNodesOut, 1);
            _costGradientBias = new DNAMatrix(numNodesOut, 1);
            _biasVelocities = new DNAMatrix(numNodesOut, 1);

            InitializeRandomWeights(new System.Random());

            Parallelization = new DNAGPUParallelization(this);
        }

        public DNAMatrix CalculateOutputs(DNAMatrix inputs)
        {
            if (DNAGPUParallelization.LayerOutputGPU != null)
            {
                if (SystemInfo.deviceType == DeviceType.Desktop)
                    return Parallelization.LayerOutputCalculationTrainingGPU(inputs).activation;
                else
                    return Parallelization.LayerOutputCalculationTrainingGPUFloat(inputs).activation;
            }
              
            else
                return activation.Activate((weights * inputs) + biases);
        }

        public DNAMatrix CalculateOutputs(DNAMatrix inputs, DNALayerLearnData learnData)
        {
            learnData.inputs = inputs;

            if (DNAGPUParallelization.LayerOutputGPU != null)
            {
                (DNAMatrix weightedInputs, DNAMatrix activation) = Parallelization.LayerOutputCalculationTrainingGPU(inputs);

                //Calculate the outputs
                learnData.weightedInputs = weightedInputs;

                //Apply Activation Function
                learnData.activations = activation;
            }
            else
            {
                //Calculate the outputs
                learnData.weightedInputs = (weights * inputs) + biases;

                //Apply Activation Function
                learnData.activations = activation.Activate(learnData.weightedInputs);
            }

            return learnData.activations;
        }

        public double[] ParallelCalculateOutputs(double[] inputs, DNAParallelLayerLearnData learnData) //DNALayerLearnData[] learnData
        {
            (double[] weightedInputs, double[] activation) = Parallelization.ParallelLayerOutputCalculationTrainingGPU(inputs);

            //Set the Inputs
            learnData.inputs = inputs;

            //Set the Weighted Inputs
            learnData.weightedInputs = weightedInputs;

            //Set the Activations
            learnData.activations = activation;

            return activation;
        }

        public void ApplyGradients(double learnRate, double regularization, double momentum)
        {
            double weightDecay = (1 - regularization * learnRate);

            //Calculate Velocities and Apply them to the respective matrices
            _weightVelocities = _weightVelocities * momentum - _costGradientWeight * learnRate;
            weights = weights * weightDecay + _weightVelocities;

            _biasVelocities = _biasVelocities * momentum - _costGradientBias * learnRate;
            biases += _biasVelocities;

            //Reset Gradients
            _costGradientWeight = new DNAMatrix(_costGradientWeight.Height, _costGradientWeight.Width);
            _costGradientBias = new DNAMatrix(_costGradientBias.Height, _costGradientBias.Width);
        }

        public void CalculateOutputLayerNodeValues(DNALayerLearnData layerLearnData, DNAMatrix expectedOutputs, IDNACost cost)
        {
            DNAMatrix costDerivative = cost.CostDerivative(layerLearnData.activations, expectedOutputs);
            DNAMatrix activationDerivative = activation.Derivative(layerLearnData.weightedInputs);

            for (int i = 0; i < layerLearnData.nodeValues.Values.Length; i++)
                layerLearnData.nodeValues[i] = costDerivative[i] * activationDerivative[i];
        }

        public void ParallelCalculateOutputLayerNodeValues(DNAParallelLayerLearnData layerLearnData, double[] expectedOutput, IDNACost cost, DNAMatrix expectedOutputDim)
        {
            Parallelization.ParallelCalculateOutputLayerNodeValues(layerLearnData, expectedOutput, cost, expectedOutputDim);
        }

        public void CalculateHiddenLayerNodeValues(DNALayerLearnData layerLearnData, DNALayer oldLayer, DNAMatrix oldNodeValues)
        {
            DNAMatrix newNodeValues = oldLayer.weights.Transpose() * oldNodeValues;

            DNAMatrix derivative = activation.Derivative(layerLearnData.weightedInputs);

            for (int newNodeIndex = 0; newNodeIndex < newNodeValues.Values.Length; newNodeIndex++)
                newNodeValues[newNodeIndex] *= derivative[newNodeIndex];

            layerLearnData.nodeValues = newNodeValues;
        }

        public void ParallelCalculateHiddenLayerNodeValues(DNAParallelLayerLearnData layerLearnData, DNALayer oldLayer, double[] oldNodeValues)
        {
            Parallelization.ParallelHiddenLayerNodeCalc(layerLearnData, oldLayer, oldNodeValues);
        }

        public void UpdateGradients(DNALayerLearnData layerLearnData)
        {
            //Lock for Parallel Processing
            lock (_costGradientWeight)
            {
                _costGradientWeight += layerLearnData.nodeValues * layerLearnData.inputs.Transpose();
            }

            lock (_costGradientBias)
            {
                _costGradientBias += layerLearnData.nodeValues;
            }
        }

        public void ParallelUpdateGradients(DNAParallelLayerLearnData layerLearnData)
        {
            _costGradientBias += Parallelization.ParallelUpdateGradientsBiasGPU(layerLearnData);

            _costGradientWeight += Parallelization.ParallelUpdateGradientsWeightsGPU(layerLearnData);
        }

        public void SetActivationFunction(IDNAActivation activation)
        {
            ActivationIndex = activation.GetActivationFunctionIndex();
            this.activation = activation;
        }

        public void SetActivationFunction(int index)
        {
            ActivationIndex = index;
            this.activation = DNAActivation.GetActivationFromIndex(index);
        }

        public void SetSavedActivationFunction()
        {
            this.activation = DNAActivation.GetActivationFromIndex(ActivationIndex);
        }

        public void InitializeParallelization()
        {
            Parallelization = new DNAGPUParallelization(this);
        }
        public void InitializeRandomWeights(System.Random rng)
        {
            for (int weightIndex = 0; weightIndex < weights.Values.Length; weightIndex++)
            {
                weights[weightIndex] = RandomInNormalDistribution(rng, 0, 1) / Mathf.Sqrt(NumNodesIn);
            }

            double RandomInNormalDistribution(System.Random rng, double mean, double standardDeviation)
            {
                double x1 = 1 - rng.NextDouble();
                double x2 = 1 - rng.NextDouble();

                double y1 = Mathf.Sqrt(-2.0f * Mathf.Log((float)x1)) * Mathf.Cos(2.0f * Mathf.PI * (float)x2);
                return y1 * standardDeviation + mean;
            }
        }
    }
}