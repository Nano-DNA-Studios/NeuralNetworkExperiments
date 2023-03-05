using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
[System.Serializable]
public class NeuralNetwork 
{

    public Layer[] layers;
    public double inputVal;
    NetworkLearnData[] batchLearnData;

   
  
    public NeuralNetwork (int[] layerSize, Activation hidden, Activation output, Cost costType)
    {
        layers = new Layer[layerSize.Length - 1];
        for (int i = 0; i < layerSize.Length - 1; i ++)
        {
            if (i == layerSize.Length - 2)
            {
                layers[i] = new Layer(layerSize[i], layerSize[i + 1], output, costType);
            } else
            {
                layers[i] = new Layer(layerSize[i], layerSize[i + 1], hidden, costType);
            }
           
        }
    }

    public NeuralNetwork ()
    {

    }

    //Run the entire neural network and get the outputs
    double[] CalculateOutput (double[] inputs)
    {
        foreach (Layer layer in layers)
        {
            inputs = layer.CalcOutput(inputs);
        }
        return inputs;
    }

    //Get the index of the output of the neural network with highest value
    public int Classify (double[] inputs)
    {
        double[] output = CalculateOutput(inputs);

        return IndexOfMax(output);
    }

    public double[] Classify2 (double[] inputs)
    {
        return CalculateOutput(inputs);
    }


    public int IndexOfMax (double[] inputs)
    {
        int index = 0;
        double maxVal = inputs[0];

        for (int i = 0; i < inputs.Length; i ++)
        {
            if (inputs[i] >= maxVal)
            {
                index = i;
            }
        }
        return index;
    }

    public double Cost(DataPoint dataPoint)
    {
        double[] outputs = CalculateOutput(dataPoint.inputs);
        Layer outputLayer = layers[layers.Length - 1];
        
        double cost = outputLayer.NodeCost(outputs, dataPoint.expectedOutputs);
        
        return cost;

    }

    public double Cost (DataPoint[] data)
    {
        double totalCost = 0;
        foreach(DataPoint dataPoint in data)
        {
            totalCost += Cost(dataPoint);
        }

        //Return Average
        return totalCost / data.Length;

    }

    public void Learn (DataPoint[] trainingBatch, double learnRate, double regularization = 0, double momentum = 0)
    {

        // int trainingIndex = 0;
        if (batchLearnData == null || batchLearnData.Length != trainingBatch.Length)
        {
            batchLearnData = new NetworkLearnData[trainingBatch.Length];

            for (int i = 0; i < batchLearnData.Length; i++)
            {
                batchLearnData[i] = new NetworkLearnData(layers);
            }
        }
           

        System.Threading.Tasks.Parallel.For(0, trainingBatch.Length, (i) =>
        {
            // trainingIndex++;
            UpdateAllGradients(trainingBatch[i], batchLearnData[i]);
        });

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].ApplyGradients(learnRate / trainingBatch.Length, regularization, momentum);
        }


        //Reset All gradients

    }

    void UpdateAllGradients (DataPoint dataPoint, NetworkLearnData learnData)
    {
        //Backwards Propogation Algorithm

        double[] inputsToNextLayer = dataPoint.inputs;

        for (int i = 0; i <  layers.Length; i ++)
        {
            inputsToNextLayer = layers[i].CalcOutput(inputsToNextLayer, learnData.layerData[i]);
        }


        // -- Backpropogation --
        int outputLayerIndex = layers.Length - 1;
        Layer outputLayer = layers[outputLayerIndex];
        LayerLearnData outputLearnData = learnData.layerData[outputLayerIndex];

        //Update output layer gradients
        outputLayer.CalculateOutputLayerNodeValues(outputLearnData, dataPoint.expectedOutputs);
        outputLayer.UpdateGradients(outputLearnData);


        //Update Hidden layer
        for (int i = outputLayerIndex - 1; i >= 0; i --)
        {
            LayerLearnData layerData = learnData.layerData[i];
            Layer hiddenLayer = layers[i];

            hiddenLayer.CalculateHiddenNodeValues(layerData, layers[i + 1], learnData.layerData[i + 1].nodeValues);
            hiddenLayer.UpdateGradients(layerData);
        }


        //Need to get the outputVals
        // CalculateOutput(dataPoint.inputs);

        
        //Update the Final Layers Gradients
        Layer outputLayer = layers[layers.Length - 1];
        double[] nodeValues = outputLayer.CalculateOutputLayerNodeValues(dataPoint.expectedOutputs);
        outputLayer.UpdateGradients(nodeValues);

        //Update Gradients of hidden layers
        for (int hiddenLayerIndex = layers.Length - 2; hiddenLayerIndex >= 0; hiddenLayerIndex--)
        {
            Layer hiddenLayer = layers[hiddenLayerIndex];
            nodeValues = hiddenLayer.CalculateHiddenNodeValues(layers[hiddenLayerIndex + 1], nodeValues);
            hiddenLayer.UpdateGradients(nodeValues);
        }
        

    }


}


public class LayerLearnData
{
    public double[] inputs;
    public double[] weightedInputs;
    public double[] activations;
    public double[] nodeValues;

    public LayerLearnData(Layer layer)
    {
        weightedInputs = new double[layer.numNodesOut];
        activations = new double[layer.numNodesOut];
        nodeValues = new double[layer.numNodesOut];
    }

}

public class NetworkLearnData
{
    public LayerLearnData[] layerData;

    public NetworkLearnData(Layer[] layers)
    {
        layerData = new LayerLearnData[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            layerData[i] = new LayerLearnData(layers[i]);
        }
    }
}
*/

namespace DNANeuralNetwork
{
    [System.Serializable]
    public class NeuralNetwork
    {
        public Layer[] layers;
        public int[] layerSizes;

        public ICost cost;
        System.Random rng;
        NetworkLearnData[] batchLearnData;

        //Create Neural Network

        public NeuralNetwork(int[] layerSizes)
        {
            this.layerSizes = layerSizes;
            rng = new System.Random();

            layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], rng);
            }

            cost = new Cost.MeanSquaredError();
        }

        public NeuralNetwork(int[] layerSizes, IActivation activation, IActivation outputLayerActivation, ICost cost)
        {
            this.layerSizes = layerSizes;
            rng = new System.Random();

            layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], rng);
            }

            cost = new Cost.MeanSquaredError();

            SetActivationFunction(activation, outputLayerActivation);
            SetCostFunction(cost);
        }

        //Have the Neural Network predict an answer
        public (int predictedClass, double[] outputs) Classify(double[] inputs)
        {
            var outputs = CalculateOutputs(inputs);
            int predictedClass = MaxValueIndex(outputs);
            return (predictedClass, outputs);
        }

        public double[] CalculateOutputs(double[] inputs)
        {
            foreach (Layer layer in layers)
            {
                inputs = layer.CalculateOutputs(inputs);
            }
            return inputs;
        }

        public void Learn(DataPoint[] trainingData, double learnRate, double regularization = 0, double momentum = 0)
        {
            if (batchLearnData == null || batchLearnData.Length != trainingData.Length)
            {
                batchLearnData = new NetworkLearnData[trainingData.Length];
                for (int i = 0; i < batchLearnData.Length; i++)
                {
                    batchLearnData[i] = new NetworkLearnData(layers);
                }
            }

            System.Threading.Tasks.Parallel.For(0, trainingData.Length, (i) =>
            {
                UpdateGradients(trainingData[i], batchLearnData[i]);
            });


            // Update weights and biases based on the calculated gradients
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].ApplyGradients(learnRate / trainingData.Length, regularization, momentum);
            }
        }

        void UpdateGradients(DataPoint data, NetworkLearnData learnData)
        {
            // Feed data through the network to calculate outputs.
            // Save all inputs/weightedinputs/activations along the way to use for backpropagation.

            double[] inputsToNextLayer = data.inputs;

            for (int i = 0; i < layers.Length; i++)
            {
                inputsToNextLayer = layers[i].CalculateOutputs(inputsToNextLayer, learnData.layerData[i]);
            }

            //Backpropogation
            int outputLayerIndex = layers.Length - 1;
            Layer outputLayer = layers[outputLayerIndex];
            LayerLearnData outputLearnData = learnData.layerData[outputLayerIndex];

            //Update output layer gradients
            outputLayer.CalculateOutputLayerNodeValues(outputLearnData, data.expectedOutputs, cost);
            outputLayer.UpdateGradients(outputLearnData);

            //Update All Hidden laer gradients
            for (int i = outputLayerIndex - 1; i >= 0; i--)
            {
                LayerLearnData layerLearnData = learnData.layerData[i];
                Layer hiddenLayer = layers[i];

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

        public int MaxValueIndex(double[] values)
        {
            double maxValue = double.MinValue;
            int index = 0;
            for (int i = 0; i < values.Length; i++)
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

    public class LayerLearnData
    {
        public double[] inputs;
        public double[] weightedInputs;
        public double[] activations;
        public double[] nodeValues;

        public LayerLearnData(Layer layer)
        {
            weightedInputs = new double[layer.numNodesOut];
            activations = new double[layer.numNodesOut];
            nodeValues = new double[layer.numNodesOut];
        }

    }

    public class NetworkLearnData
    {
        public LayerLearnData[] layerData;

        public NetworkLearnData(Layer[] layers)
        {
            layerData = new LayerLearnData[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layerData[i] = new LayerLearnData(layers[i]);
            }
        }
    }
}