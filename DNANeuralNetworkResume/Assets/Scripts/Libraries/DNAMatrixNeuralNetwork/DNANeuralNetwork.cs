using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DNAMath;
using DNANeuralNetwork;

namespace DNANeuralNet
{
    [System.Serializable]
    public class DNANeuralNetwork
    {
        public DNALayer[] layers;
        public int[] layerSizes;

        public ICost cost;
        System.Random rng;
        DNANetworkLearnData[] batchLearnData;

        public DNANeuralNetwork(int[] layerSizes)
        {
            this.layerSizes = layerSizes;
            rng = new System.Random();

            layers = new DNALayer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new DNALayer(layerSizes[i], layerSizes[i + 1]);
            }

            cost = new Cost.MeanSquaredError();
        }

        public DNANeuralNetwork(int[] layerSizes, IActivation activation, IActivation outputLayerActivation, ICost cost)
        {
            this.layerSizes = layerSizes;
            rng = new System.Random();

            layers = new DNALayer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new DNALayer(layerSizes[i], layerSizes[i + 1]);
            }

            cost = new Cost.MeanSquaredError();

            SetActivationFunction(activation, outputLayerActivation);
            SetCostFunction(cost);
        }

        //Have the Neural Network predict an answer
        public (int predictedClass, DNAMatrix outputs) Classify(DNAMatrix inputs)
        {
            var outputs = CalculateOutputs(inputs);
            int predictedClass = MaxValueIndex(outputs);
            return (predictedClass, outputs);
        }

        public DNAMatrix CalculateOutputs(DNAMatrix inputs)
        {
            foreach (DNALayer layer in layers)
            {
                inputs = layer.CalculateOutputs(inputs);
            }
            return inputs;
        }

        public double GetCost(DNADataPoint[] data)
        {
            double costVal = 0;
            foreach (DNADataPoint d in data)
            {
                (int predictedClass, DNAMatrix outputs) = Classify(d.inputs);
                costVal += cost.CostFunction(outputs.Values, d.expectedOutputs.Values);

            }
            costVal = costVal / data.Length;

            return costVal;
        }

        public void Learn(DNADataPoint[] trainingData, double learnRate, double regularization = 0, double momentum = 0)
        {
            if (batchLearnData == null || batchLearnData.Length != trainingData.Length)
            {
                batchLearnData = new DNANetworkLearnData[trainingData.Length];
                for (int i = 0; i < batchLearnData.Length; i++)
                {
                    batchLearnData[i] = new DNANetworkLearnData(layers);
                }
            }

            for (int i = 0; i < trainingData.Length; i++)
            {
                UpdateGradients(trainingData[i], batchLearnData[i]);
            }

            // Update weights and biases based on the calculated gradients
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].ApplyGradients(learnRate / trainingData.Length, regularization, momentum);
            }
        }

        void UpdateGradients(DNADataPoint data, DNANetworkLearnData learnData)
        {
            DNAMatrix inputsToNextLayer = data.inputs;

            for (int i = 0; i < layers.Length; i++)
            {
                inputsToNextLayer = layers[i].CalculateOutputs(inputsToNextLayer, learnData.layerData[i]);
            }

            //Backpropogation
            int outputLayerIndex = layers.Length - 1;
            DNALayer outputLayer = layers[outputLayerIndex];
            DNALayerLearnData outputLearnData = learnData.layerData[outputLayerIndex];

            //Update output layer gradients
            outputLayer.CalculateOutputLayerNodeValues(outputLearnData, data.expectedOutputs, cost);
            outputLayer.UpdateGradients(outputLearnData);

            //Update All Hidden layer gradients
            for (int i = outputLayerIndex - 1; i >= 0; i--)
            {
                DNALayerLearnData layerLearnData = learnData.layerData[i];
                DNALayer hiddenLayer = layers[i];

                hiddenLayer.CalculateHiddenLayerNodeValues(layerLearnData, layers[i + 1], learnData.layerData[i + 1].nodeValues);
                hiddenLayer.UpdateGradients(layerLearnData);
            }
        }


        public void SetCostFunction(ICost costFunction)
        {
            this.cost = costFunction;
        }

        public void SetActivationFunction(IActivation activation)
        {
            SetActivationFunction(activation, activation);
        }

        public void SetActivationFunction(IActivation activation, IActivation outputLayerActivation)
        {
            for (int i = 0; i < layers.Length - 1; i++)
            {
                layers[i].SetActivationFunction(activation);
            }
            layers[layers.Length - 1].SetActivationFunction(outputLayerActivation);
        }

        public int MaxValueIndex(DNAMatrix values)
        {
            double maxValue = double.MinValue;
            int index = 0;
            for (int i = 0; i < values.Values.Length; i++)
            {
                if (values[i] > maxValue)
                {
                    maxValue = values[i];
                    index = i;
                }
            }

            return index;
        }

    }

    public class DNALayerLearnData
    {
        public DNAMatrix inputs;
        public DNAMatrix weightedInputs;
        public DNAMatrix activations;
        public DNAMatrix nodeValues;

        public DNALayerLearnData(DNALayer layer)
        {
            weightedInputs = new DNAMatrix(layer.NumNodesOut, 1);
            activations = new DNAMatrix(layer.NumNodesOut, 1);
            nodeValues = new DNAMatrix(layer.NumNodesOut, 1);
        }

    }

    public class DNANetworkLearnData
    {
        public DNALayerLearnData[] layerData;

        public DNANetworkLearnData(DNALayer[] layers)
        {
            layerData = new DNALayerLearnData[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layerData[i] = new DNALayerLearnData(layers[i]);
            }
        }
    }
}