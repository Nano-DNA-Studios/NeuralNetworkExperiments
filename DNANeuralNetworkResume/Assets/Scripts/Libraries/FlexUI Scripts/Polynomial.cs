using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexUI;


namespace FlexUI
{

    [System.Serializable]
    public class FlexPolynomial
    {
        //Variables
        public float coefficient;
        public float power;
        string variable = "x"; //Default Variable is x 

        //Constructor Function
        public FlexPolynomial(float coef, float pow, string variable = "x")
        {
            //Initial Variables
            this.coefficient = coef;
            this.power = pow;

            /*
            //Determine if the variable will be null or a certain value
            if (variable != null)
            {
                //Variable is a value
                this.variable = variable;
            }
            else if (pow >= 1)
            {
                //Variable is X
                this.variable = "x"; //First variable type
            }
            */

            if (power == 0)
            {
                //No Variable
                this.variable = "";
            }

        }

        //Make a function to return output (when x input), func for derivative

        //Returns the output when we use the polynomial as a function
        public float output(float x)
        {
            return coefficient * Mathf.Pow(x, power);
        }

        //Derives the polynomial
        public void derive()
        {
            this.coefficient = coefficient * power;
            this.power = this.power - 1;
        }

        //Derives with respect to a certain variable
        public FlexPolynomial deriveWRespect(string var)
        {
            //Checks if the variable is the same as the one being derived
            if (var == this.variable)
            {
                //The same varible so we derive with respect to it
                return new FlexPolynomial(coefficient * power, power - 1, variable);
            }
            else
            {
                //Return 0 because it acts like a constant
                return new FlexPolynomial(0, power, variable);
            }
        }

        //Returns the Variable
        public string getVariable()
        {
            return variable;
        }

        //Returns the same polynomial with a negative coefficient
        public FlexPolynomial getOppositePoly()
        {
            //Debug.Log(variable);
            return new FlexPolynomial(coefficient * -1, power, variable);
        }

        //Sets the variable of the polynomial
        public void setVariable(string var)
        {
            variable = var;
        }


    }

}
