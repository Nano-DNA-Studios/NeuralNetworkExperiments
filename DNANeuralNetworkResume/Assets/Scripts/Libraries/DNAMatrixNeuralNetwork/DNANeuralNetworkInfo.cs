using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DNANeuralNetwork
{
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
       

        //public LayerTypes type;

        // public ActivationLayerInfo activation;
        //public FilterLayerInfo filter;
        public NeuralLayerInfo neural;
        //public PoolingLayerInfo pooling;
    }

    [System.Serializable]
    public struct NeuralLayerInfo
    {
        public int outputSize;
        public Activation.ActivationType activationType;
    }
}

