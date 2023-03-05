using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class Layer
{
    //Input layer doesn't count


    //Eventually replace weight stuff with a vector representation? Maybe custom library

    public int numNodesIn, numNodesOut;

    public double[] costGradientBias;
    // public double[,] costGradientWeights;
    public double[] costGradientWeights;

    public double[] biases;
    // public double[,] weights;
    public double[] weights;

    public double[] weightVolicities;
    public double[] biasVelocities;



    public double[] activations;
    public double[] weightedInputs;
    public double[] inputs;


    Activation activationFun;
    Cost costType;

    //NodesIn = number of neurons the previous layer had
    //NodesOut = number of neurons of this layer

    public Layer(int nodesIn, int nodesOut, Activation activation, Cost costType)
    {
        numNodesIn = nodesIn;
        numNodesOut = nodesOut;

        weights = new double[nodesIn * nodesOut];
        biases = new double[nodesOut];

        costGradientWeights = new double[weights.Length];
        costGradientBias = new double[biases.Length];

        weightVolicities = new double[weights.Length];
        biasVelocities = new double[biases.Length];


        weightedInputs = new double[nodesOut];

        this.activationFun = activation;
        this.costType = costType;

        InitRandomWeights();
    }

    //Might not need this anymore
    public Layer(LayerSaver laySave)
    {

        int nodesIn = laySave.numNodesIn;
        int nodesOut = laySave.numNodesOut;

        numNodesIn = nodesIn;
        numNodesOut = nodesOut;

        weights = new double[nodesIn * nodesOut];
        biases = new double[nodesOut];
        costGradientWeights = new double[nodesIn * nodesOut];
        costGradientBias = new double[nodesOut];

        weightedInputs = new double[nodesOut];

        this.biases = laySave.biases;

        //load weights 
        for (int i = 0; i < numNodesIn * numNodesOut; i++)
        {
            weights[i] = laySave.weights[i];
        }
    }

    public double[] CalcOutput(double[] input)
    {

        inputs = input;

        double[] activations = new double[numNodesOut];
        for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
        {
            double weightInput = biases[nodeOut];
            for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
            {

                weightInput += input[nodeIn] * getWeight(nodeIn, nodeOut);
            }

            weightedInputs[nodeOut] = weightInput;
            activations[nodeOut] = ActivationFunction(weightInput);
        }

        this.activations = activations;

        return activations;

    }

    //Calculate the output of the layer
    public double[] CalcOutput(double[] input, LayerLearnData learnData)
    {
        learnData.inputs = input;

        // inputs = input;

        //double[] activations = new double[numNodesOut];
        for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
        {
            double weightInput = biases[nodeOut];
            for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
            {

                weightInput += input[nodeIn] * getWeight(nodeIn, nodeOut);
            }

            learnData.weightedInputs[nodeOut] = weightInput;

            // weightedInputs[nodeOut] = weightInput;
            // activations[nodeOut] = ActivationFunction(weightInput);
        }

        //Apply Activation Function 
        for (int i = 0; i < learnData.activations.Length; i++)
        {
            learnData.activations[i] = ActivationFunction(learnData.weightedInputs, i);
        }

        // this.activations = activations;

        return learnData.activations;

    }

    double ActivationFunction(double weightedInput)
    {
        
        switch (activationFun)
        {
            case Activation.Sigmoid:
                //Sigmoid
                return 1 / (1 + Mathf.Exp(-(float)weightedInput));
            case Activation.HyperBolicTan:
                //Hyperbolic tangent
                double e2w = Mathf.Exp(2 * (float)weightedInput);
                return (e2w - 1) / (e2w + 1);
            case Activation.SiLU:
                //SiLU
                return weightedInput / (1 + Mathf.Exp(-(float)weightedInput));
            case Activation.ReLU:
                //ReLU
                return Mathf.Max(0, (float)weightedInput);
            default:
                return 0;
        }


    }

    double ActivationFunction(double[] weightedInput, int index)
    {
        switch (activationFun)
        {
            case Activation.Sigmoid:
                //Sigmoid
                return 1 / (1 + Mathf.Exp(-(float)weightedInput[index]));
            case Activation.HyperBolicTan:
                //Hyperbolic tangent
                double e2w = Mathf.Exp(2 * (float)weightedInput[index]);
                return (e2w - 1) / (e2w + 1);
            case Activation.SiLU:
                //SiLU
                return weightedInput[index] / (1 + Mathf.Exp(-(float)weightedInput[index]));
            case Activation.ReLU:
                //ReLU
                return Mathf.Max(0, (float)weightedInput[index]);
            default:
                return 0;
        }

    }

    //New NodeCosts

    public double NodeCost (double[] predictedOutputs, double[] expectedOutputs)
    {

        double cost = 0;

        switch (costType)
        {
            case Cost.MeanSquareError:
                for (int i = 0; i < predictedOutputs.Length; i++)
                {
                    double error = predictedOutputs[i] - expectedOutputs[i];
                    cost += error * error;
                }
                return 0.5 * cost;
            case Cost.CrossEntropy:
                for (int i = 0; i < predictedOutputs.Length; i++)
                {
                    double x = predictedOutputs[i];
                    double y = expectedOutputs[i];
                    double v = (y == 1) ? -Mathf.Log((float)x) : -Mathf.Log((float)(1 - x));
                    cost += double.IsNaN(v) ? 0 : v;
                }
                return cost;

                    default:
                //Mean Squared
                for (int i = 0; i < predictedOutputs.Length; i++)
                {
                    double error = predictedOutputs[i] - expectedOutputs[i];
                    cost += error * error;
                }
                return 0.5 * cost;
        }



       
    }


  //  public double NodeCost(double outputVal, double expectedVal)
   // {
   //     double error = outputVal - expectedVal;
  //      return error * error;
   // }

    public double nodeCostDerivative(double predictedVal, double expectedVal)
    {
        switch (costType)
        {
            case Cost.MeanSquareError:
                //Normally * 2, won't be for now because we divide the cost by 2
                return  (predictedVal - expectedVal);

            case Cost.CrossEntropy:
                double x = predictedVal;
                double y = expectedVal;
                if (x == 0 || x == 1)
                {
                    return 0;
                }
                return (-x + y) / (x * (x - 1));

            default:

                return (predictedVal - expectedVal);
        }
    }

    public double ActivationDerivative(double weightedVal)
    {

        switch (activationFun)
        {
            case Activation.Sigmoid:
                //Sigmoid
                double activationVal = ActivationFunction(weightedVal);
                return activationVal * (1 - activationVal);
            case Activation.HyperBolicTan:

            case Activation.SiLU:

            case Activation.ReLU:
                //ReLU
                return (weightedVal > 0) ? 1 : 0;
            default:
                return 0;
        }

    }

    public double ActivationDerivative(double[] weightedVal, int index)
    {
        switch (activationFun)
        {
            case Activation.Sigmoid:
                //Sigmoid
                double activationVal = ActivationFunction(weightedVal[index]);
                return activationVal * (1 - activationVal);
            case Activation.HyperBolicTan:

            case Activation.SiLU:

            case Activation.ReLU:
                //ReLU
                return (weightedVal[index] > 0) ? 1 : 0;
            default:
                return 0;
        }
    }

    //Update weights and biases
    public void ApplyGradients(double learnRate, double regularization, double momentum)
    {
        double weightDecay = (1 - regularization * learnRate);

        for (int i = 0; i < weights.Length; i++)
        {
            double weight = weights[i];
            double velocity = weightVolicities[i] * momentum - costGradientWeights[i] * learnRate;
            weightVolicities[i] = velocity;
            weights[i] = weight * weightDecay + velocity;
            costGradientWeights[i] = 0;
        }

        for (int i = 0; i < biases.Length; i++)
        {
            double velocity = biasVelocities[i] * momentum - costGradientBias[i] * learnRate;
            biasVelocities[i] = velocity;
            biases[i] += velocity;
            costGradientBias[i] = 0;
        }

    }


    public void InitRandomWeights()
    {
        //Apply random vals to all weights 

        System.Random rng = new System.Random();

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = RandomInNormalDistribution(rng, 0, 1) / Mathf.Sqrt(numNodesIn);
        }

        double RandomInNormalDistribution(System.Random rng, double mean, double standardDeviation)
        {
            double x1 = 1 - rng.NextDouble();
            double x2 = 1 - rng.NextDouble();

            double y1 = Mathf.Sqrt(-2.0f * Mathf.Log((float)x1)) * Mathf.Cos(2.0f * (float)Mathf.PI * (float)x2);
            return y1 * standardDeviation + mean;
        }
    }


    //
    //Derivatives
    //
   

    public double[] CalculateOutputLayerNodeValues(double[] expectedOutputs)
    {
        double[] nodeVals = new double[expectedOutputs.Length];
        for (int i = 0; i < nodeVals.Length; i++)
        {

            double costDerivative = nodeCostDerivative(activations[i], expectedOutputs[i]);
            double activationDerivative = ActivationDerivative(weightedInputs[i]);
            nodeVals[i] = costDerivative * activationDerivative;
        }
        return nodeVals;
    }

    //Seb Lague
    public double[] CalculateOutputLayerNodeValues(LayerLearnData layerData, double[] expectedOutputs)
    {
        double[] nodeVals = new double[expectedOutputs.Length];
        for (int i = 0; i < nodeVals.Length; i++)
        {

            double costDerivative = nodeCostDerivative(layerData.activations[i], expectedOutputs[i]);
            double activationDerivative = ActivationDerivative(layerData.weightedInputs, i);
            layerData.nodeValues[i] = costDerivative * activationDerivative;
        }
        return nodeVals;
    }

    public double[] CalculateHiddenNodeValues(Layer oldLayer, double[] oldNodeVals)
    {
        double[] newNodeVals = new double[numNodesOut];
        for (int newNodeIndex = 0; newNodeIndex < newNodeVals.Length; newNodeIndex++)
        {
            double newNodeVal = 0;

            for (int oldNodeIndex = 0; oldNodeIndex < oldNodeVals.Length; oldNodeIndex++)
            {
                double weightedInputDeriv = oldLayer.getWeight(newNodeIndex, oldNodeIndex);
                newNodeVal += weightedInputDeriv * oldNodeVals[oldNodeIndex];
            }

            newNodeVal *= ActivationDerivative(weightedInputs[newNodeIndex]);
            newNodeVals[newNodeIndex] = newNodeVal;

        }
        return newNodeVals;

    }

    //Seb Lague
    public void CalculateHiddenNodeValues(LayerLearnData learnData, Layer oldLayer, double[] oldNodeVals)
    {

        for (int newNodeIndex = 0; newNodeIndex < numNodesOut; newNodeIndex++)
        {
            double newNodeVal = 0;
            for (int oldNodeIndex = 0; oldNodeIndex < oldNodeVals.Length; oldNodeIndex++)
            {
                double weightedInputDeriv = oldLayer.getWeight(newNodeIndex, oldNodeIndex);
                newNodeVal += weightedInputDeriv * oldNodeVals[oldNodeIndex];
            }

            newNodeVal *= ActivationDerivative(learnData.weightedInputs, newNodeIndex);
            learnData.nodeValues[newNodeIndex] = newNodeVal;
        }
    }


    public void UpdateGradients(double[] nodeVals)
    {

        for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
        {
            for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
            {
                //Update gradients for weights
                double derivativeCostWRTWeight = inputs[nodeIn] * nodeVals[nodeOut];

                costGradientWeights[getFlatWeightIndex(nodeIn, nodeOut)] += derivativeCostWRTWeight;
            }

            //Update Gradient for biases
            double derivativeCostWRTBias = 1 * nodeVals[nodeOut];
            costGradientBias[nodeOut] += derivativeCostWRTBias;
        }

    }

    //Seb Lague
    public void UpdateGradients(LayerLearnData layerData)
    {

        //Update Weights (lock for multithreading)

        lock (costGradientWeights)
        {
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double nodeVal = layerData.nodeValues[nodeOut];
                for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    double derivativeCostWRTWeight = layerData.inputs[nodeIn] * nodeVal;

                    costGradientWeights[getFlatWeightIndex(nodeIn, nodeOut)] += derivativeCostWRTWeight;

                }
            }
        }

        //Update cost gradient Bias (lock for multithreading)
        lock (costGradientBias)
        {

            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double derivativeCostWRTBias = 1 * layerData.nodeValues[nodeOut];
                costGradientBias[nodeOut] += derivativeCostWRTBias;
            }
        }

    }

    public double getWeight(int nodeIn, int nodeOut)
    {

        return weights[nodeOut * numNodesIn + nodeIn];


    }

    public int getFlatWeightIndex(int inputNeurIndex, int outputNeurIndex)
    {
        return outputNeurIndex * numNodesIn + inputNeurIndex;

    }


}

[System.Serializable]
public enum Activation
{
    Sigmoid,
    HyperBolicTan,
    SiLU,
    ReLU,
}

public enum Cost
{
    MeanSquareError, 
    CrossEntropy
}
*/

namespace DNANeuralNetwork
{
    [System.Serializable]
    public class Layer
    {
        public int numNodesIn;
        public int numNodesOut;

        public double[] weights;
        public double[] biases;

        //Cost Gradient With respect to weight and biases
        public double[] costGradientWeight;
        public double[] costGradientBias;

        //Momentum
        public double[] weightVelocities;
        public double[] biasVelocities;

        public IActivation activation;


        public Layer(int numNodesIn, int numNodesOut, System.Random rng)
        {
            this.numNodesIn = numNodesIn;
            this.numNodesOut = numNodesOut;
            activation = new Activation.Sigmoid();

            weights = new double[numNodesIn * numNodesOut];
            costGradientWeight = new double[weights.Length];

            biases = new double[numNodesOut];
            costGradientBias = new double[biases.Length];

            weightVelocities = new double[weights.Length];
            biasVelocities = new double[biases.Length];

            InitializeRandomWeights(rng);
        }

        public double[] CalculateOutputs(double[] inputs)
        {
            double[] weightedInputs = new double[numNodesOut];

            //Get Weighted input
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double weightedInput = biases[nodeOut];

                for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    weightedInput += inputs[nodeIn] * GetWeight(nodeIn, nodeOut);
                }
                weightedInputs[nodeOut] = weightedInput;
            }

            //Apply Activation Function

            double[] activations = new double[numNodesOut];
            for (int outputNode = 0; outputNode < numNodesOut; outputNode++)
            {
                //Fix this later?
                activations[outputNode] = activation.Activate(weightedInputs, outputNode);
            }

            return activations;
        }

        public double[] CalculateOutputs(double[] inputs, LayerLearnData learnData)
        {
            learnData.inputs = inputs;

            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double weightedInput = biases[nodeOut];

                for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    weightedInput += inputs[nodeIn] * GetWeight(nodeIn, nodeOut);
                }
                learnData.weightedInputs[nodeOut] = weightedInput;
            }

            //Apply Activation Function
            for (int outputNode = 0; outputNode < numNodesOut; outputNode++)
            {
                //Fix this later?
                learnData.activations[outputNode] = activation.Activate(learnData.weightedInputs, outputNode);
            }
            return learnData.activations;
        }

        public void ApplyGradients(double learnRate, double regularization, double momentum)
        {
            double weightDecay = (1 - regularization * learnRate);

            for (int i = 0; i < weights.Length; i++)
            {
                double weight = weights[i];
                double velocity = weightVelocities[i] * momentum - costGradientWeight[i] * learnRate;
                weightVelocities[i] = velocity;
                weights[i] = weight * weightDecay + velocity;
                costGradientWeight[i] = 0;
            }

            for (int i = 0; i < biases.Length; i++)
            {
                double velocity = biasVelocities[i] * momentum - costGradientBias[i] * learnRate;
                biasVelocities[i] = velocity;
                biases[i] += velocity;
                costGradientBias[i] = 0;
            }

        }

        //Calculate Node Values for output layer
        public void CalculateOutputLayerNodeValues(LayerLearnData layerLearnData, double[] expectedOutputs, ICost cost)
        {
            for (int i = 0; i < layerLearnData.nodeValues.Length; i++)
            {
                double costDerivative = cost.CostDerivative(layerLearnData.activations[i], expectedOutputs[i]);
                double activationDerivative = activation.Derivative(layerLearnData.weightedInputs, i);
                layerLearnData.nodeValues[i] = costDerivative * activationDerivative;
            }
        }

        //Calculate Node Values for Hidden layer
        public void CalculateHiddenLayerNodeValues(LayerLearnData layerLearnData, Layer oldLayer, double[] oldNodeValues)
        {
            for (int newNodeIndex = 0; newNodeIndex < numNodesOut; newNodeIndex++)
            {
                double newNodeValue = 0;
                for (int oldNodeIndex = 0; oldNodeIndex < oldNodeValues.Length; oldNodeIndex++)
                {
                    //Get the partial derivative of weight with respect to input
                    double weightedInputDerivative = oldLayer.GetWeight(newNodeIndex, oldNodeIndex);
                    newNodeValue += weightedInputDerivative * oldNodeValues[oldNodeIndex];
                }
                newNodeValue *= activation.Derivative(layerLearnData.weightedInputs, newNodeIndex);
                layerLearnData.nodeValues[newNodeIndex] = newNodeValue;
            }
        }

        public void UpdateGradients(LayerLearnData layerLearnData)
        {
            lock (costGradientWeight)
            {
                for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
                {
                    double nodeValue = layerLearnData.nodeValues[nodeOut];
                    for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                    {
                        // Evaluate the partial derivative: cost / weight of current connection
                        double derivativeCostWrtWeight = layerLearnData.inputs[nodeIn] * nodeValue;
                        // The costGradientW array stores these partial derivatives for each weight.
                        // Note: the derivative is being added to the array here because ultimately we want
                        // to calculate the average gradient across all the data in the training batch
                        costGradientWeight[GetFlatWeightIndex(nodeIn, nodeOut)] += derivativeCostWrtWeight;
                    }
                }
            }

            // Update cost gradient with respect to biases (lock for multithreading)
            lock (costGradientBias)
            {
                for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
                {
                    // Evaluate partial derivative: cost / bias
                    double derivativeCostWrtBias = 1 * layerLearnData.nodeValues[nodeOut];
                    costGradientBias[nodeOut] += derivativeCostWrtBias;
                }
            }
        }

        public double GetWeight(int nodeIn, int nodeOut)
        {
            int flatIndex = nodeOut * numNodesIn + nodeIn;
            return weights[flatIndex];
        }

        public int GetFlatWeightIndex(int inputIndex, int outputIndex)
        {
            return outputIndex * numNodesIn + inputIndex;
        }

        public void SetActivationFunction(IActivation activation)
        {
            this.activation = activation;
        }

        public void InitializeRandomWeights(System.Random rng)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = RandomInNormalDistribution(rng, 0, 1) / Mathf.Sqrt(numNodesIn);
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

