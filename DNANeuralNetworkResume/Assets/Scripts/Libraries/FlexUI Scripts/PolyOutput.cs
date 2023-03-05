using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexUI;


namespace FlexUI
{

    public class PolyOutput
    {
        //Variables
        public string variable;
        public float value;


        //Constructor function 
        public PolyOutput(string variable, float value)
        {
            //Establish the values
            this.variable = variable;
            this.value = value;
        }



    }
}

