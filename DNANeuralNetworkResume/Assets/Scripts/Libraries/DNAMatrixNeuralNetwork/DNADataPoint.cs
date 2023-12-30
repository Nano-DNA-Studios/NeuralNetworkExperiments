using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNANeuralNet;
using DNAMath;

namespace DNANeuralNet
{
    public class DNADataPoint
    {
        public DNAMatrix inputs;
        public DNAMatrix expectedOutputs;
        public int label;

        public DNADataPoint(DNAMatrix inputs, int label, int numLabels)
        {
            this.inputs = inputs;
            this.label = label;
            expectedOutputs = CreateOneHot(label, numLabels);
        }

        public static DNAMatrix CreateOneHot(int index, int num)
        {
            DNAMatrix expOut = new DNAMatrix(num, 1);
            expOut[index, 0] = 1;
            return expOut;
        }

        public DNADataPoint(DNAMatrix inputs, DNAMatrix outputs)
        {
            this.inputs = inputs;
            this.expectedOutputs = outputs;
            this.label = 0;
        }
    }

}
