using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNANeuralNetwork
{
    [System.Serializable]
    public class DataPointFile 
    {
        public List<DataPoint> data = new List<DataPoint>();

        public DataPointFile ()
        {
            data = new List<DataPoint>();
        }

        public void AddData (DataPoint dataPoint)
        {
            data.Add(dataPoint);
        }
    }
}

