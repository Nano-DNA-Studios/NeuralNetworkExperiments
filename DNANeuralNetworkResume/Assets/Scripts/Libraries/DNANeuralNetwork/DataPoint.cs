using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataPoint
{
    public double[] inputs;
    public double[] expectedOutputs;
    public int label;

    //Inputs is the pixel values
    //Label is the int number of the output 
    // 0 = 4
    // 1 = 6
    //numLabels is the total number of labels
    public DataPoint(double[] inputs, int label, int numLabels)
    {
        this.inputs = inputs;
        this.label = label;
        expectedOutputs = CreateOneHot(label, numLabels);
    }

    public static double[] CreateOneHot (int index, int num)
    {
        double[] oneHot = new double[num];
        oneHot[index] = 1;
        return oneHot;
    }

}
