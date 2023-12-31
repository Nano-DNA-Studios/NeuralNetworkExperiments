using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DNAMath;
using System;

namespace DNANeuralNet
{
    [System.Serializable]
    public class DNANeuralNetwork
    {
        public int[] layerSizes;
        public DNALayer[] layers;

        public IDNACost cost;
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

            cost = new DNACost.MeanSquaredError();
        }

        public DNANeuralNetwork(int[] layerSizes, IDNAActivation activation, IDNAActivation outputLayerActivation, IDNACost cost)
        {
            this.layerSizes = layerSizes;
            rng = new System.Random();

            layers = new DNALayer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new DNALayer(layerSizes[i], layerSizes[i + 1]);
            }

            cost = new DNACost.MeanSquaredError();

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
                costVal += cost.CostFunction(outputs, d.expectedOutputs);
            }
            costVal = costVal / data.Length;

            return costVal;
        }

        public void ParallelLearn(DNADataPoint[] trainingData, double learnRate, double regularization = 0, double momentum = 0)
        {
            DNAParallelNetworkLearnData batchLearnData = new DNAParallelNetworkLearnData(layers, trainingData.Length);

            ParallelUpdateGradients(trainingData, batchLearnData);

            // Update weights and biases based on the calculated gradients
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].ApplyGradients(learnRate / trainingData.Length, regularization, momentum);
            }
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

        void ParallelUpdateGradients(DNADataPoint[] data, DNAParallelNetworkLearnData learnData)
        {
            //System.DateTime startTime = System.DateTime.Now;

            double[] inputsToNextLayer = new double[data.Length * data[0].inputs.Length];
            double[] expectedOutputs = new double[data.Length * data[0].expectedOutputs.Length];

            int countInput = 0;
            int countOutput = 0;
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(data[i].expectedOutputs.Values, 0, expectedOutputs, countOutput, data[i].expectedOutputs.Values.Length);
                countOutput += data[i].expectedOutputs.Values.Length;

                Array.Copy(data[i].inputs.Values, 0, inputsToNextLayer, countInput, data[i].inputs.Values.Length);
                countInput += data[i].inputs.Values.Length;
            }

            // System.DateTime format = System.DateTime.Now;

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].ParallelBatchSize = data.Length;
                inputsToNextLayer = layers[i].ParallelCalculateOutputs(inputsToNextLayer, learnData.layerData[i]);
            }

            int outputLayerIndex = layers.Length - 1;

            //Removing this will bring us back to good learning
            layers[outputLayerIndex].ParallelCalculateOutputLayerNodeValues(learnData.layerData[outputLayerIndex], expectedOutputs, cost, data[0].expectedOutputs);

            //Update output layer gradients
            layers[outputLayerIndex].ParallelUpdateGradients(learnData.layerData[outputLayerIndex]);

            // System.DateTime parallelOperations = System.DateTime.Now;

            //Update All Hidden layer gradients
            for (int i = outputLayerIndex - 1; i >= 0; i--)
            {
                DNALayer hiddenLayer = layers[i];

                hiddenLayer.ParallelCalculateHiddenLayerNodeValues(learnData.layerData[i], layers[i + 1], learnData.layerData[i + 1].nodeValues);
                hiddenLayer.ParallelUpdateGradients(learnData.layerData[i]);
            }

            // System.DateTime leftover = System.DateTime.Now;

            /*
            double totalTime = (leftover - startTime).TotalSeconds;

            double formatTime = 100.0 * (format - startTime).TotalSeconds / totalTime;
            double layerTime = 100.0 * (parallelOperations - format).TotalSeconds / totalTime;
            double leftOverTime = 100.0 * (leftover - parallelOperations).TotalSeconds / totalTime;
            */
            //Debug.Log($"Format:{formatTime}    Parallel Operations:{layerTime}     Left Over:{leftOverTime}");
        }

        void UpdateGradients(DNADataPoint data, DNANetworkLearnData learnData)
        {
            System.DateTime startTime = System.DateTime.Now;

            DNAMatrix inputsToNextLayer = data.inputs;

            for (int i = 0; i < layers.Length; i++)
            {
                inputsToNextLayer = layers[i].CalculateOutputs(inputsToNextLayer, learnData.layerData[i]);
            }

            System.DateTime layerOperation = System.DateTime.Now;

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

            System.DateTime leftover = System.DateTime.Now;

            double totalTime = (leftover - startTime).TotalSeconds;

            double layerTime = 100.0 * (layerOperation - startTime).TotalSeconds / totalTime;
            double leftOverTime = 100.0 * (leftover - layerOperation).TotalSeconds / totalTime;

            // Debug.Log($"Layer Operation:{layerTime}     Left Over:{leftOverTime}");
        }

        public void SetCostFunction(IDNACost costFunction)
        {
            this.cost = costFunction;
        }

        public void SetActivationFunction(IDNAActivation activation)
        {
            SetActivationFunction(activation, activation);
        }

        public void InitializeFromLoad()
        {
            SetSavedActivationFunction();
            InitializeParallelization();
        }

        public void SetSavedActivationFunction()
        {
            foreach (DNALayer layer in layers)
                layer.SetSavedActivationFunction();
        }

        public void InitializeParallelization()
        {
            foreach (DNALayer layer in layers)
                layer.InitializeParallelization();
        }

        public void SetActivationFunction(IDNAActivation activation, IDNAActivation outputLayerActivation)
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

    public class DNAParallelLayerLearnData
    {
        public double[] inputs;
        public double[] weightedInputs;
        public double[] activations;
        public double[] nodeValues;

        public DNAParallelLayerLearnData(DNALayer layer, int parallelCount)
        {
            weightedInputs = new double[layer.NumNodesOut * parallelCount];
            activations = new double[layer.NumNodesOut * parallelCount];
            nodeValues = new double[layer.NumNodesOut * parallelCount];
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

    public class DNAParallelNetworkLearnData
    {
        public DNAParallelLayerLearnData[] layerData;

        public DNAParallelNetworkLearnData(DNALayer[] layers, int parallelCount)
        {
            layerData = new DNAParallelLayerLearnData[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layerData[i] = new DNAParallelLayerLearnData(layers[i], parallelCount);
            }
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