using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNAMath;

namespace DNANeuralNet
{
    public interface IDNAActivation
    {
        DNAMatrix Activate(DNAMatrix matrix);

        DNAMatrix Derivative(DNAMatrix matrix);

        DNAActivation.ActivationType GetActivationType();

        int GetActivationFunctionIndex();
    }
}

