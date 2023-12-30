using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldStuff 
{
    /*
    [System.Serializable]
    public class DNANeuralNetwork
    {
     
        public DNALayer[] layers;

        public int[] layerSizes;

        public ICost cost;
        System.Random rng;
        NetworkLearnData[] batchLearnData;

        public DNANeuralNetwork (DNANeuralNetworkInfo info)
        {
            //Initialize the neural networks

            layers = new DNALayer[info.layerInfos.Length];

            layers = createLayers(info);

            //Set the cost function
            SetCostFunction(Cost.GetCostFromType(info.costType));

        }
      
        DNALayer[] createLayers (DNANeuralNetworkInfo info)
        {
            List<DNALayer> layers = new List<DNALayer>();

            for (int i = 0; i < info.layerInfos.Length; i++)
            {
                DNALayer layer = null;
                LayerInfo layInfo = info.layerInfos[i];

                switch (info.layerInfos[i].type)
                {
                    case LayerTypes.Activation:

                        if (i - 1 >= 0)
                        {
                            layer = new ActivationLayer(layInfo.activation.activationType, layers[i - 1].outputSize);
                        } else
                        {
                            layer = new ActivationLayer(layInfo.activation.activationType, info.inputSize);
                        }
                           
                        break;
                    case LayerTypes.Filter:

                        if (i - 1 >= 0)
                        {
                            //Check if last layer has multiple outputs
                            layer = new FilterLayer(layInfo.filter.filterSize, layInfo.filter.numOfFilters, layInfo.filter.stride, layers[i - 1].outputSize, layers[i-1].outputMatNum);
                        } else
                        {
                            layer = new FilterLayer(layInfo.filter.filterSize, layInfo.filter.numOfFilters, layInfo.filter.stride, info.inputSize);
                        }
                        break;
                    case LayerTypes.Neural:

                        if (i - 1 >= 0)
                        {
                            if (i - 2 >= 0)
                            {
                                layer = new NeuralLayer(layers[i - 1].iLayer.flattenLayer(layers[i-2].outputSize), layInfo.neural.outputSize);
                            } else
                            {
                                layer = new NeuralLayer(layers[i - 1].iLayer.flattenLayer(info.inputSize), layInfo.neural.outputSize);
                            }
                            
                        } else
                        {
                            layer = new NeuralLayer(info.inputSize.x * info.inputSize.y, layInfo.neural.outputSize);
                        }

                        break;
                    case LayerTypes.Pooling:

                        if (i - 1 >= 0)
                        {
                            layer = new PoolingLayer(layInfo.pooling.size, layInfo.pooling.stride, layInfo.pooling.poolingType, layers[i - 1].outputSize, layers[i - 1].outputMatNum);
                        } else
                        {
                            layer = new PoolingLayer(layInfo.pooling.size, layInfo.pooling.stride, layInfo.pooling.poolingType, info.inputSize);
                        }

                        break;
                }

                layers.Add(layer);
            }

            return layers.ToArray();
        }

        public void SetCostFunction(ICost costFunction)
        {
            this.cost = costFunction;
        }

        public DNAMatrix[] CalculateOutputs(DNAMatrix input)
        {
            DNAMatrix[] inputs = new DNAMatrix[1];
            inputs[0] = input;

            foreach (DNALayer layer in layers)
            {
                inputs = layer.iLayer.CalculateOutputs(inputs);
            }

            return inputs;
        }

        void UpdateGradients(DNADataPoint data, DNANetworkLearnData learnData)
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

            //Update All Hidden layer gradients
            for (int i = outputLayerIndex - 1; i >= 0; i--)
            {
                LayerLearnData layerLearnData = learnData.layerData[i];
                Layer hiddenLayer = layers[i];

                hiddenLayer.CalculateHiddenLayerNodeValues(layerLearnData, layers[i + 1], learnData.layerData[i + 1].nodeValues);
                hiddenLayer.UpdateGradients(layerLearnData);
            }
            
            DNAMatrix[] inputsToNextLayer = new DNAMatrix[1];

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


        }
    }


    [System.Serializable]
    public class DNANeuralNetworkInfo
    {
        public Vector2Int inputSize;
        public LayerInfo[] layerInfos;
        public Cost.CostType costType;
    }

    [System.Serializable]
    public class LayerInfo
    {
        public LayerTypes type;

        public ActivationLayerInfo activation;
        public FilterLayerInfo filter;
        public NeuralLayerInfo neural;
        public PoolingLayerInfo pooling;
    }

    [System.Serializable]
    public enum LayerTypes
    {
        Activation,
        Filter,
        Neural, 
        Pooling,
    }

    [System.Serializable]
    public struct NeuralLayerInfo
    {
        public int outputSize;
    }

    [System.Serializable]
    public struct FilterLayerInfo 
    {
        //Determines size of the filter, (Change to int? Since the dimensions probably need to be square)
        public Vector2Int filterSize;
        public int numOfFilters;

        //Unless stride is always zero
        public int stride;
    }

    [System.Serializable]
    public struct ActivationLayerInfo
    {
        public Activation.ActivationType activationType;
    }

    [System.Serializable]
    public struct PoolingLayerInfo
    {
      

        public PoolingType poolingType;
        public Vector2Int size;
        public int stride;
    }

    public enum PoolingType
    {
        Max,
        Average,
        Min
    }

    public class DNALayerLearnData
    {
        //Theoretically only need inputs, weightedInputs? and nodeValues?
        public DNAMatrix[] inputs;
        public DNAMatrix[] weightedInputs;
        public DNAMatrix[] activations;
        public DNAMatrix[] outputs;

        //Node Values are for the derivatives
        public DNAMatrix[] nodeValues;

        public DNALayerLearnData(DNALayer layer)
        {

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
    

    //Maybe we remake all of this and we have 2 layer types, Neural and Filter type
    //Neural will apply Neural transformation + Activation layer
    //Filter will apply Filter, pooling and activation
    //Focus on making just the neural type again but in our own language
    //Train that network to make sure it works
    //Then create a filter layer version
    /// <summary>
    /// A Class representing a single layer, stores a specific type of layer that will then apply it's methods
    /// </summary>
    [System.Serializable]
    public class DNALayer
    {
        public ILayer iLayer;

        //Maybe make biases a matrix?
        public DNAMatrix[] weights;
        public double[] biases;

        public DNAMatrix[] costGradientWeight;
        public double[] costGradientBias;

        //Momentum
        public DNAMatrix[] weightVelocities;
        public double[] biasVelocities;


        public Vector2Int outputSize;
        public int outputMatNum;

        /// <summary>
        /// Gets the weight value of a certain matrix based on which nodes it is connected to
        /// </summary>
        /// <param name="nodeIn"></param>
        /// <param name="nodeOut"></param>
        /// <param name="matrixIndex"></param>
        /// <returns></returns>
        public double GetWeight(int nodeIn, int nodeOut, int matrixIndex)
        {
            int flatIndex = nodeOut * weights[matrixIndex].matrixDimensions.x + nodeIn;
            return weights[matrixIndex].values[flatIndex];
        }

        /// <summary>
        /// Gets the weight value of a certain matrix based on a Flattened list
        /// </summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        /// <returns></returns>
        public int GetFlatWeightIndex(int xIndex, int yIndex)
        {
            return yIndex * weights[0].matrixDimensions.x + xIndex;
        }

        /// <summary>
        /// Assigns each weight in each matrix with randomized values
        /// </summary>
        /// <param name="rng"></param>
        public void InitializeRandomWeights(System.Random rng)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].matrixDimensions.x * weights[i].matrixDimensions.y; j++)
                {
                    weights[i].values[j] = RandomInNormalDistribution(rng, 0, 1) / Mathf.Sqrt((float)weights[i].values.Length);
                }
            }

            double RandomInNormalDistribution(System.Random rng, double mean, double standardDeviation)
            {
                double x1 = 1 - rng.NextDouble();
                double x2 = 1 - rng.NextDouble();

                double y1 = Mathf.Sqrt(-2.0f * Mathf.Log((float)x1)) * Mathf.Cos(2.0f * Mathf.PI * (float)x2);
                return y1 * standardDeviation + mean;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="layerLearnData"></param>
        /// <returns></returns>
        public DNAMatrix[] CalculateOutputs(DNAMatrix[] inputs, DNALayerLearnData layerLearnData)
        {
            layerLearnData.inputs = inputs;

            DNAMatrix[] outputs = iLayer.CalculateOutputs(inputs);

            layerLearnData.outputs = outputs;

            return outputs;
        }

        public void CalculateOutputLayerNodeValues(DNALayerLearnData layerLearnData, DNAMatrix expectedOutputs, ICost cost)
        {
            layerLearnData.nodeValues = new DNAMatrix[1];
            layerLearnData.nodeValues[0] = new DNAMatrix(expectedOutputs.matrixDimensions);
            for (int i = 0; i < expectedOutputs.values.Length; i++)
            {
                layerLearnData.nodeValues[0].values[i] = cost.CostDerivative(layerLearnData.outputs[0].values[i], expectedOutputs.values[i]);
            }
        }

        public void UpdateGradients(DNALayerLearnData layerLearnData)
        {
            //Will need to switch depending on type of layer

            lock (costGradientWeight)
            {
                iLayer.UpdateWeightGradients(layerLearnData);
            }

            lock (costGradientBias)
            {
                iLayer.UpdateBiasGradients(layerLearnData);
            }

           
            lock (costGradientWeight)
            {
                for (int matrixIndex = 0; matrixIndex < weights.Length; matrixIndex++)
                {

                    DNAMatrix weightMatrix = layerLearnData.nodeValues[matrixIndex];

                    
                }


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


        //Functions for 

        //Update Gradients
        //CalculateHiddenLayerNodeValues
        //CalculateOutputLayerNodeValues   X
        //Apply gradients
        //Calculate outputs (Returns the matrices)  X X


        //Calculate output Node values   (This one needs to look if it needs to unflatten, and then basically take the values

        //Calculate input node values    (This is where we will apply the current layers changes




        //Maybe look at this 
        //https://towardsdatascience.com/backpropagation-in-fully-convolutional-networks-fcns-1a13b75fb56a#:~:text=Backpropagation%20has%20two%20phases.,a%20few%20Momentum%2C%20Adam%20etc%20%E2%80%A6


    }

    [System.Serializable]
    public class NeuralLayer : DNALayer, ILayer
    {
        public int numNodesIn;
        public int numNodesOut;


        public NeuralLayer(int numNodesIn, int numNodesOut)
        {
            this.iLayer = this;
            this.numNodesIn = numNodesIn;
            this.numNodesOut = numNodesOut;

            Debug.Log("Nodes in: " + numNodesIn);

            //Generate number needed for all
            weights = new DNAMatrix[1];
            costGradientWeight = new DNAMatrix[1];
            weightVelocities = new DNAMatrix[1];

            // for (int i = 0; i < 1; i++)
            // {
            weights[0] = new DNAMatrix(new Vector2Int(numNodesIn, numNodesOut));
            // }

            biases = new double[numNodesOut];
            costGradientBias = new double[numNodesOut];
            biasVelocities = new double[numNodesOut];

            InitializeRandomWeights(new System.Random());

            this.outputSize = new Vector2Int(1, numNodesOut);

            this.outputMatNum = 1;
            // Debug.Log(outputSize);
        }

        public Vector2Int getOutputSize(Vector2Int inputSize)
        {
            return new Vector2Int(1, numNodesOut);
        }

        public int flattenLayer(Vector2Int inputtedSize)
        {
            Debug.Log("Hi");
            return numNodesOut;
        }

        public DNAMatrix[] CalculateOutputs(DNAMatrix[] inputs)
        {
            //Flatten the matrices and then do the matrix multiplication
            DNAMatrix flattenedMatrix = new DNAMatrix(new Vector2Int(1, numNodesIn));

            List<double> vals = new List<double>();
            foreach (DNAMatrix mat in inputs)
            {
                foreach (double val in mat.values)
                {
                    vals.Add(val);
                }
            }

            flattenedMatrix.values = vals.ToArray();

            DNAMatrix outputMat = new DNAMatrix(new Vector2Int(1, numNodesOut));

            //Multiply the matrices
            outputMat = flattenedMatrix * weights[0];

            //Add biases
            for (int i = 0; i < outputMat.values.Length; i++)
            {
                outputMat.values[i] = outputMat.values[i] + biases[i];
            }

            DNAMatrix[] outputs = new DNAMatrix[1];

            outputs[0] = outputMat;

            return outputs;
        }

        public void CalculateHiddenLayerOutputNodeValues(DNALayerLearnData layerLearnData, DNAMatrix[] oldNodeValues)
        {
            //In the event that we need to unflatten

            int length = oldNodeValues.Length * oldNodeValues[0].matrixDimensions.x * oldNodeValues[0].matrixDimensions.y;

            DNAMatrix flatten = new DNAMatrix(new Vector2Int(1, length));

            for (int i = 0; i < oldNodeValues.Length; i++)
            {
                for (int height = 0; height < oldNodeValues[i].matrixDimensions.x; height++)
                {
                    for (int width = 0; width < oldNodeValues[i].matrixDimensions.y; width++)
                    {
                        flatten.setValue(0, height * oldNodeValues[i].matrixDimensions.y + width, oldNodeValues[i].getValue(height, width));
                    }
                }
            }

            layerLearnData.nodeValues = new DNAMatrix[1];

            layerLearnData.nodeValues[0] = flatten;
        }

        public void CalculateHiddenLayerInputNodeValues(DNALayerLearnData layerLearnData)
        {
            //Won't need to flatten

            for (int i = 0; i < layerLearnData.inputs.Length; i++)
            {
                DNAMatrix inputMatrix = new DNAMatrix(layerLearnData.inputs[i].matrixDimensions);

                //We know it will only have 

            }

        }

        //I Honestly don't know if this is it
        public void CalculateHiddenLayerNodeValues(DNALayerLearnData layerLearnData, DNALayer oldLayer, DNAMatrix[] oldNodeValues)
        {
            //Need to return list of matrices with dimensions of what was inputted
            for (int i = 0; i < numNodesOut; i++)
            {
                double newNodeVal = 0;
                for (int j = 0; j < oldNodeValues.Length; j++)
                {
                    for (int height = 0; height < oldNodeValues[j].matrixDimensions.x; height++)
                    {
                        for (int width = 0; width < oldNodeValues[j].matrixDimensions.y; width++)
                        {
                            newNodeVal += oldLayer.GetWeight(i, height * oldNodeValues[j].matrixDimensions.y + width, j);
                        }
                    }
                }
                layerLearnData.nodeValues[0].setValue(0, i, newNodeVal);
            }

            //needs to take in last layers node values and set the node values of this one to basically the output size
        }

        public void UpdateWeightGradients(DNALayerLearnData layerLearnData)
        {
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double nodeValue = layerLearnData.nodeValues[0].getValue(0, nodeOut);
                for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    // Evaluate the partial derivative: cost / weight of current connection
                    double derivativeCostWrtWeight = layerLearnData.inputs[0].getValue(nodeIn, nodeOut) * nodeValue;
                    // The costGradientW array stores these partial derivatives for each weight.
                    // Note: the derivative is being added to the array here because ultimately we want
                    // to calculate the average gradient across all the data in the training batch
                    costGradientWeight[0].setValue(nodeIn, nodeOut, derivativeCostWrtWeight);
                }
            }
        }

        public void UpdateBiasGradients(DNALayerLearnData layerLearnData)
        {
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                // Evaluate partial derivative: cost / bias
                double derivativeCostWrtBias = 1 * layerLearnData.nodeValues[0].getValue(0, nodeOut);
                costGradientBias[nodeOut] += derivativeCostWrtBias;
            }
        }

    }

    [System.Serializable]
    public class PoolingLayer : DNALayer, ILayer
    {
        Vector2Int poolSize;
        int stride;
        PoolingType poolType;
        //Max, Average or Min pooling

        //Add type pooling
        public PoolingLayer(Vector2Int poolSize, int stride, PoolingType poolType, Vector2Int lastSize, int filterNum = 1)
        {
            iLayer = this;
            this.poolSize = poolSize;
            this.stride = stride;
            this.poolType = poolType;

            weights = new DNAMatrix[filterNum];

            this.outputSize = getOutputSize(lastSize);

            //Debug.Log(outputSize);

            this.outputMatNum = filterNum;

            Debug.Log("Pool Layer: Size:" + poolSize + " Num: " + filterNum + " Stride: " + stride + "Output:" + outputSize);
        }

        public Vector2Int getOutputSize(Vector2Int inputtedSize)
        {
            //Width
            int width = 0;
            for (int i = 0; i < inputtedSize.x; i = i + stride)
            {
                width++;

                //Actually, just don't cover the last region
                if (i + (poolSize.x - 1) + stride >= inputtedSize.x)
                {
                    i = inputtedSize.x;
                }
            }

            //Height
            int height = 0;
            for (int i = 0; i < inputtedSize.y; i = i + stride)
            {
                height++;

                //Actually, just don't cover the last region
                if (i + (poolSize.y - 1) + stride >= inputtedSize.y)
                {
                    i = inputtedSize.y;
                }
            }

            Vector2Int outputtedSize = new Vector2Int(width, height);

            return outputtedSize;
        }

        public int flattenLayer(Vector2Int inputtedSize)
        {
            Vector2Int size = getOutputSize(inputtedSize);

            return weights.Length * size.x * size.y;
        }

        public DNAMatrix[] CalculateOutputs(DNAMatrix[] inputs)
        {
            DNAMatrix[] outputs = new DNAMatrix[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                //Apply pooling to each 
                outputs[i] = new DNAMatrix(outputSize);

                for (int yIndex = 0; yIndex < outputSize.y; yIndex++)
                {
                    for (int xIndex = 0; xIndex < outputSize.x; xIndex++)
                    {
                        //Get the current index, multiply each by the stride to get correct start index, then pass 

                        int trueYIndex = yIndex * stride;
                        int trueXIndex = xIndex * stride;

                        outputs[i].setValue(yIndex, xIndex, getPoolValue(trueYIndex, trueXIndex, inputs[i]));

                    }
                }
            }

            return outputs;
        }

        double getPoolValue(int yIndex, int xIndex, DNAMatrix matrix)
        {
            //  Debug.Log(matrix.matrixDimensions);

            //  Debug.Log(new Vector2Int(yIndex, xIndex));

            //Debug.Log(matrix);

            double output = matrix.getValue(yIndex, xIndex);
            switch (poolType)
            {
                case PoolingType.Max:
                    //Get the maximum value
                    for (int y = 0; y < stride; y++)
                    {
                        for (int x = 0; x < stride; x++)
                        {
                            double val = matrix.getValue(yIndex + y, xIndex + x);
                            if (val > output)
                            {
                                output = val;
                            }
                        }
                    }
                    break;
                case PoolingType.Min:
                    //Get the Minimum value
                    for (int y = 0; y < stride; y++)
                    {
                        for (int x = 0; x < stride; x++)
                        {
                            double val = matrix.getValue(yIndex + y, xIndex + x);
                            if (val < output)
                            {
                                output = val;
                            }
                        }
                    }
                    break;
                case PoolingType.Average:
                    //Get the average value
                    double total = 0;
                    for (int y = 0; y < stride; y++)
                    {
                        for (int x = 0; x < stride; x++)
                        {
                            total += matrix.getValue(yIndex + y, xIndex + x);
                        }
                    }

                    output = total / (stride * stride);
                    break;
                default:
                    //Default to Max
                    //Get the maximum value
                    for (int y = 0; y < stride; y++)
                    {
                        for (int x = 0; x < stride; x++)
                        {
                            double val = matrix.getValue(yIndex + y, xIndex + x);
                            if (val > output)
                            {
                                output = val;
                            }
                        }
                    }
                    break;
            }

            return output;
        }

        //Depending on algorithm
        public void CalculateHiddenLayerNodeValues(DNALayerLearnData layerLearnData, DNALayer oldLayer, DNAMatrix[] oldNodeValues)
        {
            for (int i = 0; i < outputMatNum; i++)
            {

            }
        }

        public void UpdateWeightGradients(DNALayerLearnData layerLearnData)
        {
            //Do nothing
        }

        public void UpdateBiasGradients(DNALayerLearnData layerLearnData)
        {
            //Do nothing
        }

    }

    [System.Serializable]
    public class FilterLayer : DNALayer, ILayer
    {

        //Gonna have to fix this
        //The number of outputted layers is equal to the number of filters. we need to add up the values from 2 layers sharing a bias



        //Variable for input size?

        int stride;
        Vector2Int filterSize;
        int filterNum;
        int lastLayNum;

        //Last layer num is the value of filternum from the last layer
        public FilterLayer(Vector2Int filterSize, int filterNum, int stride, Vector2Int lastSize, int lastLayerNum = 1)
        {
            this.iLayer = this;
            this.filterSize = filterSize;
            this.stride = stride;
            this.filterNum = filterNum;

            //Generate number needed for all
            weights = new DNAMatrix[filterNum * lastLayerNum];
            costGradientWeight = new DNAMatrix[filterNum * lastLayerNum];
            weightVelocities = new DNAMatrix[filterNum * lastLayerNum];

            for (int i = 0; i < filterNum * lastLayerNum; i++)
            {
                weights[i] = new DNAMatrix(filterSize);
            }

            biases = new double[filterNum];
            costGradientBias = new double[filterNum];
            biasVelocities = new double[filterNum];

            lastLayNum = lastLayerNum;

            InitializeRandomWeights(new System.Random());

            this.outputSize = getOutputSize(lastSize);

            this.outputMatNum = filterNum;

            //Debug.Log(outputSize);

            Debug.Log("Filter Layer: Size:" + filterSize + " Num: " + filterNum + " Stride: " + stride + " Output:" + outputSize + " FilterOutput: " + filterNum * lastLayerNum);

        }

        //Maybe filterSize and stride can be taken from when the class is created
        public Vector2Int getOutputSize(Vector2Int inputtedSize)
        {
            //Width
            int width = 0;
            for (int i = 0; i < inputtedSize.x; i = i + stride)
            {
                width++;

                //Actually, just don't cover the last region
                if (i + (filterSize.x - 1) + stride >= inputtedSize.x)
                {
                    i = inputtedSize.x;
                }
            }

            //Height
            int height = 0;
            for (int i = 0; i < inputtedSize.y; i = i + stride)
            {
                height++;

                //Actually, just don't cover the last region
                if (i + (filterSize.y - 1) + stride >= inputtedSize.y)
                {
                    i = inputtedSize.y;
                }
            }

            Vector2Int outputtedSize = new Vector2Int(width, height);

            return outputtedSize;
        }

        public int flattenLayer(Vector2Int inputtedSize)
        {
            Debug.Log("Hi");

            Vector2Int size = getOutputSize(inputtedSize);

            return (weights.Length / lastLayNum) * size.x * size.y;
        }

        public DNAMatrix[] CalculateOutputs(DNAMatrix[] inputs)
        {
            DNAMatrix[] outputs = new DNAMatrix[filterNum * inputs.Length];
            for (int j = 0; j < filterNum; j++)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    int filterIndex = j * inputs.Length + i;
                    outputs[filterIndex] = new DNAMatrix(outputSize);
                    for (int yIndex = 0; yIndex < outputSize.y; yIndex++)
                    {
                        for (int xIndex = 0; xIndex < outputSize.x; xIndex++)
                        {
                            //Get the current index, multiply each by the stride to get correct start index, then pass 

                            int trueYIndex = yIndex * stride;
                            int trueXIndex = xIndex * stride;

                            outputs[filterIndex].addValue(yIndex, xIndex, getFilterVal(trueYIndex, trueXIndex, inputs[i], j) + (biases[j] / inputs.Length));
                        }
                    }
                }
            }

            return outputs;
        }

        public double getFilterVal(int yIndex, int xIndex, DNAMatrix matrix, int filterIndex)
        {
            double total = 0;
            for (int y = 0; y < stride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    total += matrix.getValue(yIndex + y, xIndex + x) * weights[filterIndex].getValue(y, x);
                }
            }

            return total;
        }

        public void CalculateHiddenLayerNodeValues(DNALayerLearnData layerLearnData, DNALayer oldLayer, DNAMatrix[] oldNodeValues)
        {

        }

        public void UpdateWeightGradients(DNALayerLearnData layerLearnData)
        {
            /*
            DNAMatrix[] outputs = new DNAMatrix[filterNum];
            for (int filter = 0; filter < filterNum; filter++)
            {
                outputs[filter] = new DNAMatrix(outputSize);

                for (int i = 0; i < inputs.Length; i++)
                {
                    for (int yIndex = 0; yIndex < outputSize.y; yIndex++)
                    {
                        for (int xIndex = 0; xIndex < outputSize.x; xIndex++)
                        {
                            //Get the current index, multiply each by the stride to get correct start index, then pass 

                            int trueYIndex = yIndex * stride;
                            int trueXIndex = xIndex * stride;

                            outputs[filter].addValue(yIndex, xIndex, getFilterVal(trueYIndex, trueXIndex, inputs[i], filter) + (biases[filter] / inputs.Length));
                        }
                    }
                }
            }
            

            // double derivativeCostWrtWeight = layerLearnData.inputs[0].getValue(nodeIn, nodeOut) * nodeValue;


            //Loop through all current layers

            //Get number of outputted filters belonging to current filter


            for (int filter = 0; filter < filterNum; filter++)
            {

                for (int outputs = 0; outputs < lastLayNum; outputs++)
                {
                    int outputIndex = filter * lastLayNum + outputs;



                    //Now we need to go through 

                }



            }






        }

        public void UpdateBiasGradients(DNALayerLearnData layerLearnData)
        {

        }


        //

        //Array of matrices for each filter

        //Array of same length for biases


    }

    [System.Serializable]
    public class ActivationLayer : DNALayer, ILayer
    {
        public IActivation activation;

        //When inputed just go every value in the matrix and apply activation, then pass on to next layer without changing much

        public ActivationLayer(Activation.ActivationType type, Vector2Int lastSize)
        {
            switch (type)
            {
                case Activation.ActivationType.Sigmoid:
                    this.activation = new Activation.Sigmoid();
                    break;
                case Activation.ActivationType.ReLU:
                    this.activation = new Activation.ReLU();
                    break;
                case Activation.ActivationType.SiLU:
                    this.activation = new Activation.SiLU();
                    break;
                case Activation.ActivationType.TanH:
                    this.activation = new Activation.TanH();
                    break;
                case Activation.ActivationType.Softmax:
                    this.activation = new Activation.Softmax();
                    break;
            }

            this.outputSize = getOutputSize(lastSize);

        }

        public Vector2Int getOutputSize(Vector2Int inputSize)
        {
            return inputSize;
        }

        public int flattenLayer(Vector2Int inputtedSize)
        {
            Debug.Log("Hi");

            Vector2Int size = getOutputSize(inputtedSize);

            return weights.Length * size.x * size.y;
        }

        public DNAMatrix[] CalculateOutputs(DNAMatrix[] inputs)
        {
            DNAMatrix[] outputs = new DNAMatrix[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int yIndex = 0; yIndex < outputSize.y; yIndex++)
                {
                    for (int xIndex = 0; xIndex < outputSize.x; xIndex++)
                    {
                        outputs[i].setValue(yIndex, xIndex, getActivationValue(outputs[i].getValue(yIndex, xIndex)));
                    }
                }
            }

            return outputs;

        }

        public double getActivationValue(double value)
        {
            double val = value;


            return val;
        }

        public void CalculateHiddenLayerNodeValues(DNALayerLearnData layerLearnData, DNALayer oldLayer, DNAMatrix[] oldNodeValues)
        {

        }

        public void UpdateWeightGradients(DNALayerLearnData layerLearnData)
        {

        }

        public void UpdateBiasGradients(DNALayerLearnData layerLearnData)
        {

        }
    }

    public interface ILayer
    {
        public Vector2Int getOutputSize(Vector2Int inputSize);

        public int flattenLayer(Vector2Int inputtedSize);

        public DNAMatrix[] CalculateOutputs(DNAMatrix[] inputs);

        public void CalculateHiddenLayerNodeValues(DNALayerLearnData layerLearnData, DNALayer oldLayer, DNAMatrix[] oldNodeValues);

        public void UpdateWeightGradients(DNALayerLearnData layerLearnData);
        public void UpdateBiasGradients(DNALayerLearnData layerLearnData);
    }
    */
}

