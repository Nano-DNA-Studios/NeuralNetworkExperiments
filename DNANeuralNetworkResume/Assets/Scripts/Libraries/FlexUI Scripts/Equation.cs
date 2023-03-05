using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexUI;

namespace FlexUI
{

    public class Equation
    {
        //Equation


        //Replace 


        //A Cleanup up version of the equation
        public List<FlexPolynomial> cleanPoly = new List<FlexPolynomial>();

        //The equation in polynomial form
        public List<FlexPolynomial> polynomials;

        //Constructor function
        public Equation()
        {
            //Creates a new list of polynomials
            polynomials = new List<FlexPolynomial>();
        }

        //Add a polynomial to the list
        public void addPolynomial(FlexPolynomial poly)
        {
            polynomials.Add(poly);
        }

        //Add a polynomial to the list
        public void addPolynomial(float flex, float pow, string variable = "x")
        {
            //Create a new flex polynomial 
            FlexPolynomial poly = new FlexPolynomial(flex, pow, variable);
            //Add the new polynomial to the list
            polynomials.Add(poly);
        }

        //Derive the equation
        //Replace this with return equation
        public void derive()
        {
            //Clean up the equation
            polyClean();
            
            //Loop through the equation and derive each term
            foreach (FlexPolynomial i in cleanPoly)
            {
                i.derive();
            }

            //Replace the equation with the clean and derived version
            polynomials = cleanPoly;
        }

        //Derive with respect to a variable
        public Equation deriveWithRespect(string var)
        {
            //Create a new equation and clean the current equation.
            Equation derEQ = new Equation();
            polyClean();
            //Loop through all polynomials 
            foreach (FlexPolynomial i in cleanPoly)
            {

               // Debug.Log(i.deriveWRespect(var).coefficient);
               //Derive with respect to the variable, add it to the new equation
                derEQ.addPolynomial(i.deriveWRespect(var));
            }
            //Return the new equation
            return derEQ;

        }


        //
        //Doesn't Apply
        //

        //Return the value of the equation when values for the variables are inputted
        public float output(List<PolyOutput> vals)
        {
            //Clean the equation
            polyClean();
            float value = 0;

            //Loop through to add the 

            //Check for power 0 first
            foreach (FlexPolynomial i in cleanPoly)
            {
                if (i.power == 0)
                {
                    value += i.coefficient;
                }
            }

            //Replace this with a dictionary method. 
            //Loop through all Polyoutput values
            foreach (PolyOutput i in vals)
            {
                //Loop through the equation. 
                foreach (FlexPolynomial j in cleanPoly)
                {
                    //Check if variables are the same
                    if (j.getVariable() == i.variable)
                    {
                        // Debug.Log("Add: " + j.coefficient + " * " + j.variable + "^" + j.power + " (" + j.variable + "= " + i.value + ") = "+  j.output(i.value));
                        //Output the value of the polynomial when the variables is replaced by the value
                        value += j.output(i.value);
                    }
                }
            }

            // Debug.Log("Output Value: " + value);
            //Return the final value
            return value;
        }

        //Add extra variables such as initial guess and stuff
        //Switch this to list of floats and also list of equations

        public float solveSingleEQ(float ans, bool debug = false)
        {
            //Replace this with a matrix solver?

            
            //Clean the polynomial. 
            polyClean();

            //Get the number of unique variables. 
            int varNum = getVarCount();

            //Create a new equation
            Equation newEQ = new Equation();
            
            //Get a viable variable
            string var = getViableVariable(getVarList());
            Equation derivative = new Equation();

            //Make an initial guess
            float x = 1;
            float newX = 0;
            float diffPer = 0;

            //Set the value to 0
            float value = 0;
            //Check how many unique variables there are
            switch (varNum)
            {
                case 0:
                    //Either only 0 degree polynomial or only x, x^2 ect, return the value
                    value = ans;

                    break;
                case 1:
                    //Only one unique variable
                    //Debug.Log("Case 1");

                    //Use Newton Raphson method to solve. (Replace with a more efficient method later)
                   
                   


                    //Set the new equation 
                    newEQ.setEquation(cleanPoly);

                    if (debug)
                    {
                        Debug.Log("Inside EQ");
                        newEQ.displayAllPoly();
                        Debug.Log("Clean");
                        newEQ.displayCleanPoly();
                    }


                    //Add the negative of the answer to say make the entire equation to 0
                    newEQ.addPolynomial(new FlexPolynomial(-ans, 0, var));
                    newEQ.polyClean();

                    //Create the derivative equation
                    derivative.setEquation(cleanPoly);
                    derivative.derive();
                    derivative.polyClean();


                    //Loop over 100 times
                    for (int i = 0; i < 100; i++)
                    {
                        //Make a new list of PolyOutputs
                        List<PolyOutput> vals = new List<PolyOutput>();
                        
                        vals.Add(new PolyOutput(var, x));

                        //Calculate new X using Newton Raphson
                        newX = x - (newEQ.output(vals) / derivative.output(vals));

                        //Calculate the difference between the last value
                        diffPer = Mathf.Abs((((float)newX - (float)x) / (float)newX) * 100);

                        //Make x = to the new value
                        x = newX;

                        //Check if the difference is less than 0.1%
                        if (diffPer < 0.001f)
                        {
                            //Finish the loop
                            i = 100;
                        }
                    }

                    //Debug.Log("X " + x);
                    //Make the value equal to the solved value for x
                    value = x;

                    break;

                    //Not figured out yet
                    //We will send it to a matrix solver. 
                    /*
                case 2:
                   
                    Debug.Log("Case 2");

                    //Ok gotta add the answer to the equation too as a negative aswell

                    //Ok let's make this like memory safe and shit

                    //One variable only, 

                    //Honestly just need to do newton raphson for this basically

                    // Equation newEQ = new Equation();
                    newEQ.setEquation(cleanPoly);
                    newEQ.addPolynomial(new FlexPolynomial(-ans, 0, var));
                    newEQ.polyClean();

                    Debug.Log(debugname);
                    newEQ.displayCleanPoly();
                    newEQ.displayAllPoly();
                    // Debug.Log("New Equation");
                    //newEQ.displayCleanPoly();


                    derivative.setEquation(cleanPoly);
                    derivative.derive();

                    // Debug.Log("Derivative");
                    // derivative.displayCleanPoly();

                    //loop for 100 or until change is under 0.1%

                    //X initial guess = 1



                    for (int i = 0; i < 100; i++)
                    {
                        List<PolyOutput> vals = new List<PolyOutput>();

                        vals.Add(new PolyOutput(var, x));

                        //Actually calculate the new X using Newton Raphson
                        newX = x - (newEQ.output(vals) / derivative.output(vals));


                        diffPer = Mathf.Abs((((float)newX - (float)x) / (float)newX) * 100);

                        x = newX;



                        // Debug.Log(diffPer);

                        if (diffPer < 0.001f && i > 5)
                        {
                            i = 100;
                        }
                    }

                    //Debug.Log(x);

                    value = x;


                    break;
                    */

            }

            return value;
        }

        //Clean the equation
        public void polyClean()
        {
            
            //Create a new list
            cleanPoly = new List<FlexPolynomial>();

            //Loop through each polynomial in the equation. 
            foreach (FlexPolynomial i in polynomials)
            {
                if (!containsPoly(cleanPoly, i))
                {
                    //displayAllPoly();
                    cleanPoly.Add(new FlexPolynomial(0, i.power, i.getVariable()));
                }
            }

            //Add the polynomials to the list
            //Loop through both lists
            //Replace this with a dictionary method
            foreach (FlexPolynomial i in cleanPoly)
            {
                foreach (FlexPolynomial j in polynomials)
                {
                    //Check if the power is 0, if it is add all the polynomials of power 0 into it since they would just be regular numbers that can be added
                    if (i.power == 0)
                    {
                        if (j.power == 0)
                        {
                            i.coefficient += j.coefficient;
                        }
                    }
                    else if (j.getVariable() == i.getVariable())
                    {
                        if (j.power == i.power)
                        {
                            i.coefficient += j.coefficient;
                        }
                    }
                }
            }

            //Find a way to remove the 0 coefficient polynomials

            polynomials = cleanPoly;

        }

        //Get the maximum power of the equation
        public float getMaxPow()
        {
            //Clean the equation
            polyClean();
            float highPow = 0;
            //Loop through each polynomial
            foreach (FlexPolynomial i in cleanPoly)
            {
                //If the power is higher than the current recorded make it the newest highest power
                if (i.power >= highPow)
                {
                    highPow = i.power;
                }
            }

            //Return it. 
            return highPow;
        }

        //Get the number of variables in the equation
        public int getVarCount()
        {
            //Make a list
            List<string> varNames = new List<string>();

            //Loop through looking for new variables
            foreach (FlexPolynomial i in polynomials)
            {
                //Check if the list doesn't contain the variable
                if (!varNames.Contains(i.getVariable()))
                {
                    //Add the variable to the list
                    varNames.Add(i.getVariable());
                }
            }

            //Check if the length is 0.
            if (varNames.Count == 1)
            {
                //Check if the variable is nothing
                if (varNames[0] == "" || varNames[0] == " " || varNames[0] == null)
                {
                    //Return 0
                    return 0;
                }
                else
                {
                    //Return 1
                    return 1;
                }
            }
            else
            {
                //Return the number
                return varNames.Count;
            }
        }

        //Return the list of variables
        public List<string> getVarList()
        {
            //Make a new list
            List<string> varNames = new List<string>();

            //Loop through all the polynomials
            foreach (FlexPolynomial i in polynomials)
            {
                //Check if the variable isn't in the list
                if (!varNames.Contains(i.getVariable()))
                {
                    //Add to the list
                    varNames.Add(i.getVariable());
                }
            }
            //Return the list
            return varNames;
        }

        //Set the equation to a new one. 
        public void setEquation(List<FlexPolynomial> eq)
        {
            this.polynomials = eq;
        }

        //Check if the equation contains a certain polynomial
        //Replace this with dictionary method
        public bool containsPoly(FlexPolynomial poly)
        {
            //Loop through all polynomials
            foreach (FlexPolynomial i in polynomials)
            {
                //Check if variables are the same
                if (i.getVariable() == poly.getVariable())
                {
                    //Check if powers are the same
                    if (i.power == poly.power)
                    {
                        //True
                        return true;
                    }
                }
            }
            //False
            return false;

        }

        //Check if a certain list of polynomials contain a certain type of polynomial
        //Replace this with dictionary method
        public static bool containsPoly(List<FlexPolynomial> list, FlexPolynomial poly)
        {
            //Loop through all polynomials
            foreach (FlexPolynomial i in list)
            {
                //Check if variable is the same
                if (i.getVariable() == poly.getVariable())
                {
                    //Check if power is the same. 
                    if (i.power == poly.power)
                    {
                        //True
                        return true;
                    }
                }
            }
            //False
            return false;

        }

        //Get the equation
        public List<FlexPolynomial> getEQ()
        {
            //Actually we need to make a new thing for this
            List<FlexPolynomial> list = new List<FlexPolynomial>();

            //Loop through all polynomials
            foreach (FlexPolynomial i in polynomials)
            {
                //Make a new polynomial and add it to the list
                FlexPolynomial newPoly = new FlexPolynomial(i.coefficient, i.power, i.getVariable());
                list.Add(newPoly);
            }
            //Return the list
            return list;
        }

        //Display all polynomials from the equation
        public void displayAllPoly()
        {
            int count = 0;
            //Loop through all polynomials
            foreach (FlexPolynomial i in polynomials)
            {
                //Display poloynomial in debug
                Debug.Log("Poly " + count + " : " + i.coefficient + i.getVariable() + " Pow : " + i.power);
                count++;
            }
        }

        //Display the cleaned up version of the equation
        public void displayCleanPoly()
        {
            int count = 0;
            //Loop through all polynomials
            foreach (FlexPolynomial i in cleanPoly)
            {
                //Display the polynomials in debug window
                Debug.Log("Poly " + count + " : " + i.coefficient + i.getVariable() + " Pow : " + i.power);
                count++;
            }
        }

        //Remove a specific Polynomial
        public void removePolynomial(FlexPolynomial poly)
        {
            //Remove it from the list
            polynomials.Remove(poly);

            //Clean the equation
            polyClean();
        }

        //Remove a polynomial with certain variable and power
        public void removePolynomial(float power, string var)
        {
            //Loop through all polynomials
            for (int i = 0; i < polynomials.Count; i++)
            {
                //Check for the variable
                if (polynomials[i].getVariable() == var)
                {
                    //Check for the power
                    if (polynomials[i].power == power)
                    {
                        //Remove the polynomial
                        polynomials.RemoveAt(i);
                    }
                }
            }
        }

        //Get a viable variable from a list
        public string getViableVariable(List<string> vars)
        {
            string var = "";
            //Loop through and make var = to the variable
            foreach (string i in vars)
            {
                if (i != "")
                {
                    var = i;
                }
            }
            //Check if variable is null
            if (var != "")
            {
                //Return variable
                return var;
            }
            else
            {
                //Make variable x
                return "x";
            }
        }



    }

}