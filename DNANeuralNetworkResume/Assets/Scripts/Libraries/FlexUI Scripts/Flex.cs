using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace FlexUI
{

    public class Flex
    {

        //Variables 

        //Reference to the UI elements Transform
        public RectTransform UI;

        //Reference to all the children Flexes 
        public List<Flex> children = new List<Flex>();


        [Header("Flex")]
        //Name of the UI object
        public string name;
        //Flex value for the parent, (Cannot be turned into a pixel value, will only serve as a flex unit)
        public float flex;

        [Header("Self Padding")]
        //Applies to self (Not Children)
        public FlexPolynomial selfPadTopFlex;
        public FlexPolynomial selfPadBotFlex;
        public FlexPolynomial selfPadLeftFlex;
        public FlexPolynomial selfPadRightFlex;

        [Header("Padding")]
        //Applies only to objects Underneath (Children)
        public FlexPolynomial padTopFlex;
        public FlexPolynomial padBotFlex;
        public FlexPolynomial padLeftFlex;
        public FlexPolynomial padRightFlex;

        [Header("Spacing")]
        //Spacing between the objects children
        public FlexPolynomial spacingFlex;

        //Need to create functions to get these items if people want 

        [Header("Child Based Size Settings")]
        //A bool to see if it will use one of these values
        public bool useChildMulti;
        //A Bool to see if we will use Child Multi on width
        public bool useChildMultiW;
        //A Bool to see if we use Child Multi on Height
        public bool useChildMultiH;

        //A value that will determine the width of the Flex depending on number of children
        public float childMultiW;
        //A value that will determine the height of the Flex depending on number of children
        public float childMultiH;

        //All the Miscellanious settings that the flex can have 
        [Header("Misc Settings")]
        // Will make the flex a square dimension by looking at the cross axis size (Layout Group Horizontal : heightXheight, Layout Group Vertical : widthXwidth)
        public bool square;
        // Will fill in to the max dimensions of this flex's parent, doesn't work with vertical or horizontal layouts
        public bool fillParent;
        // Once clicked, the system will ignore modifying the flex
        public bool dontModify;
        // WIP, will allow you to input a custom size as a child
        public bool customDim;
        //Allow custom size for width
        public bool customDimW;
        //Allow custom size for height
        public bool customDimH;






        //Booleans to tell the program if there is a layoutgroup, and if it is Vertical or Horizontal
        bool layoutGroup;
        bool layoutGroupVert;

        //Solved values for a single Flex Unit to determine the Horizontal Flex Unit and Vertical Flex Unit value
        float hVal;
        float wVal;

        [Header("Starting Parent Settings")]
        //The size that the flex will start at
        public Vector2 size;
        [Header("Override Size")]
        //The size that a child flex will take if custom size toggle is on (WIP)
        public Vector2 customSize;

        //Constructor function
        public Flex(RectTransform UIItem, float flex, Flex parent = null)
        {
            //Assign initial values 
            this.UI = UIItem;
            this.flex = flex;
            this.size = UIItem.sizeDelta;

            //Initialize the Children List
            children = new List<Flex>();

            //Determine if there's a Vertical or Horizontal Layout Group on this UI Item
            if (UIItem.gameObject.GetComponent<HorizontalLayoutGroup>())
            {
                //Horizontal Layout Group Found
                layoutGroup = true;
                layoutGroupVert = false;
            }
            else if (UIItem.gameObject.GetComponent<VerticalLayoutGroup>())
            {
                //Vertical Layout Group Found
                layoutGroup = true;
                layoutGroupVert = true;
            }
            else
            {
                //No Layout Group Found
                layoutGroup = false;
            }

            //Set default padding
            setSelfHorizontalPadding(0, 1, 0, 1);
            setSelfVerticalPadding(0, 1, 0, 1);
            setHorizontalPadding(0, 1, 0, 1);
            setVerticalPadding(0, 1, 0, 1);
            setSpacingFlex(0, 1);

            FlexInfo info = UI.gameObject.AddComponent<FlexInfo>();
            info.flex = this;

            if (parent != null)
            {
                parent.addChild(this);
            }

        }


        //
        //All Associated functions
        //

        //Adds a child flex object to the list of children
        public void addChild(Flex child)
        {
            //Adds the child to the list
            children.Add(child);
        }

        //Sets the Flex for the spacing using a flex polynomial
        public void setSpacingFlex(FlexPolynomial flex)
        {
            spacingFlex = flex;
        }

        //Sets the spacing flex by creating a new FlexPolynomial
        public void setSpacingFlex(float coef, float power)
        {
            spacingFlex = new FlexPolynomial(coef, power);
        }

        //Sets the horizontal padding of the object
        public void setHorizontalPadding(FlexPolynomial leftFlex, FlexPolynomial rightFlex)
        {
            padLeftFlex = leftFlex;
            padRightFlex = rightFlex;
        }
        //Sets the horizontal padding of the object
        public void setHorizontalPadding(float leftCoef, float leftPow, float rightCoef, float rightPow)
        {
            padLeftFlex = new FlexPolynomial(leftCoef, leftPow);
            padRightFlex = new FlexPolynomial(rightCoef, rightPow);
        }
        //Sets the vertical padding of the object
        public void setVerticalPadding(FlexPolynomial upFlex, FlexPolynomial downFlex)
        {
            padTopFlex = upFlex;
            padBotFlex = downFlex;
        }
        //Sets the vertical padding of the object
        public void setVerticalPadding(float topCoef, float topPow, float botCoef, float botPow)
        {
            padTopFlex = new FlexPolynomial(topCoef, topPow);
            padBotFlex = new FlexPolynomial(botCoef, botPow);
        }

        //Self padding

        //Sets the self horizontal padding of the object
        public void setSelfHorizontalPadding(FlexPolynomial leftFlex, FlexPolynomial rightFlex)
        {
            selfPadLeftFlex = leftFlex;
            selfPadRightFlex = rightFlex;
        }
        //Sets the self horizontal padding of the object
        public void setSelfHorizontalPadding(float leftCoef, float leftPow, float rightCoef, float rightPow)
        {
            selfPadLeftFlex = new FlexPolynomial(leftCoef, leftPow);
            selfPadRightFlex = new FlexPolynomial(rightCoef, rightPow);
        }
        //Sets the self vertical padding of the object
        public void setSelfVerticalPadding(FlexPolynomial upFlex, FlexPolynomial downFlex)
        {
            selfPadTopFlex = upFlex;
            selfPadBotFlex = downFlex;
        }
        //Sets the self vertical padding of the object
        public void setSelfVerticalPadding(float topCoef, float topPow, float botCoef, float botPow)
        {
            selfPadTopFlex = new FlexPolynomial(topCoef, topPow);
            selfPadBotFlex = new FlexPolynomial(botCoef, botPow);
        }

        public void setSize(Vector2 thisSize)
        {

            //Check if the object won't be modified
            if (dontModify)
            {
                //set the size of the object
                size = UI.sizeDelta;
            }
            //Check if it will have a custom dimension
            else if (customDim)
            {

                if (customDimH && customDimW)
                {
                    //Use the Custom Size
                   
                    size = customSize;
                } else if (customDimH)
                {
                    //Use height
                  
                    defaultMethod(thisSize);
                    size.y = customSize.y;

                  
                }
                else if (customDimW)
                {
                   
                    //Use width
                    defaultMethod(thisSize);
                    size.x = customSize.x;

                } else
                {
                  
                    //Use the regular sizing method
                    defaultMethod(thisSize);
                }


                /*
                //Make sure the dimensions are real
                if (customSize.magnitude > 0)
                {

                    //Use the Custom Size
                    size = customSize;
                }
                else
                {
                    //Use the regular sizing method
                    defaultMethod(thisSize);
                }
                */

            }
            //Check if it will have a size dependent on the number of children.
            else if (useChildMulti)
            {
                //Apply child Multi Height
                if (useChildMultiH && childMultiH != 0)
                {
                    size = new Vector2(defaultWidthCalc(thisSize.x), children.Count * childMultiH);
                }
                //Apply child multi width
                if (useChildMultiW && childMultiW != 0)
                {
                    size = new Vector2(children.Count * childMultiW, defaultHeightCalc(thisSize.y));
                }

            }
            else
            {
                //Use the default sizing method
                defaultMethod(thisSize);
            }

            //Recalculate the size using new Equations and self padding
            //Check if there's a layout group
            if (layoutGroup)
            {
                //Check if the layout group is vertical
                if (layoutGroupVert)
                {
                    //Calculate the dimension values
                    //Calc Horizontal first
                    wVal = solveW();
                    hVal = solveH();

                    //Apply the padding and spacing to the components

                    //Apply Padding
                    //Top
                    if (padTopFlex.power == 0)
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.top = (int)(padTopFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.top = (int)(padTopFlex.coefficient * hVal);
                    //Bot
                    if (padBotFlex.power == 0)
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = (int)(padBotFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = (int)(padBotFlex.coefficient * hVal);
                    //Left
                    if (padLeftFlex.power == 0)
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.left = (int)(padLeftFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.left = (int)(padLeftFlex.coefficient * wVal);
                    //Right
                    if (padRightFlex.power == 0)
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.right = (int)(padRightFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().padding.right = (int)(padRightFlex.coefficient * wVal);

                    //Apply Spacing
                    //Spacing
                    if (spacingFlex.power == 0)
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().spacing = (int)((spacingFlex.coefficient)); // UI.gameObject.GetComponent<VerticalLayoutGroup>().spacing = (int)((spacingFlex.coefficient) / (children.Count - 1));  add a setting for this
                    else
                        UI.gameObject.GetComponent<VerticalLayoutGroup>().spacing = (int)((spacingFlex.coefficient * hVal) / (children.Count - 1));

                }
                else
                {
                    //Calculate the dimension values
                    //Calc vertical first
                    hVal = solveH();
                    wVal = solveW();

                    //Apply the padding and spacing to the components

                    //Apply padding
                    //Top
                    if (padTopFlex.power == 0)
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.top = (int)(padTopFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.top = (int)(padTopFlex.coefficient * hVal);
                    //Bot
                    if (padBotFlex.power == 0)
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.bottom = (int)(padBotFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.bottom = (int)(padBotFlex.coefficient * hVal);
                    //Left
                    if (padLeftFlex.power == 0)
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.left = (int)(padLeftFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.left = (int)(padLeftFlex.coefficient * wVal);
                    //Right
                    if (padRightFlex.power == 0)
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.right = (int)(padRightFlex.coefficient);
                    else
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().padding.right = (int)(padRightFlex.coefficient * wVal);

                    //Apply Spacing
                    //Spacing
                    if (spacingFlex.power == 0)
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().spacing = (int)((spacingFlex.coefficient)); // UI.gameObject.GetComponent<VerticalLayoutGroup>().spacing = (int)((spacingFlex.coefficient) / (children.Count - 1));  add a setting for this
                    else
                        UI.gameObject.GetComponent<HorizontalLayoutGroup>().spacing = (int)((spacingFlex.coefficient * wVal) / (children.Count - 1));

                }
            }
            else
            {
                //Calculate the dimension values
                //Calc vertical first
                hVal = solveH();
                wVal = solveW();
            }

            //Set current object and all children
            UI.sizeDelta = size;

            //Loop through children and apply the sizes to them
            for (int i = 0; i < children.Count; i++)
            {

                //Check if the child should fill the parent
                if (children[i].fillParent)
                {
                    //Set the size to the same as the parent (May have to fix with child padding)
                    children[i].setSize(size);
                }
                //Check if child must be square
                else if (children[i].square)
                {
                    //Check if the layout group is vertical
                    if (layoutGroupVert)
                    {
                        //Set the dimensions to the width
                        children[i].setSize(new Vector2(wVal, wVal));
                    }
                    else
                    {
                        //Set the dimensions to the height
                        children[i].setSize(new Vector2(hVal, hVal));
                    }


                }
                else
                {
                    //Check if the layout group is vertical
                    if (layoutGroupVert)
                    {
                        //Set the dimensions to what it will normally be
                        children[i].setSize(new Vector2(wVal, hVal * children[i].flex));
                    }
                    else
                    {
                        //Set the dimensions to what it will normally be
                        children[i].setSize(new Vector2(wVal * children[i].flex, hVal));
                    }

                }

            }

            //Now this should theoretically be ready to test
        }

        //
        //Maybe replace the setSize for chidlren to a custom version so that all the processing is faster?
        //

        //Solve for the height
        public float solveH()
        {
            //Vertical
            //Make a new equation
            Equation eqY = new Equation();

            //Add padding flexes
            eqY.addPolynomial(padTopFlex);
            eqY.addPolynomial(padBotFlex);

            //Add spacing Flexes
            if (layoutGroupVert && layoutGroup)
            {
                eqY.addPolynomial(spacingFlex.coefficient, spacingFlex.power);
            }

            //Add children flexes
            if (layoutGroupVert)
            {
                //Loop through all children
                for (int i = 0; i < children.Count; i++)
                {
                    //Check if child is square
                    if (children[i].square)
                    {
                        //Add a polynomial with the minimum dimension
                        if (layoutGroupVert)
                        {
                            eqY.addPolynomial(size.x, 0); //wVal
                        } else
                        {
                            eqY.addPolynomial(size.y, 0); //hVal
                        }
                       // Debug.Log("Square");
                    }
                    else
                    {
                        //Add the childs flex
                        eqY.addPolynomial(children[i].flex, 1);
                    }
                }
            }
            else
            {
                //Make the flex cover the entire region
                eqY.addPolynomial(1, 1);
            }

            //Return the calulated value
            return eqY.solveSingleEQ(size.y);
        }

        public float solveW()
        {
            //Horizontal
            //Create new equation
            Equation eqX = new Equation();

            //Adding padding Flexes 
            eqX.addPolynomial(padLeftFlex);
            eqX.addPolynomial(padRightFlex);


            //Add spacing flex
            if (!layoutGroupVert && layoutGroup)
            {

                if (spacingFlex.power > 0)
                {
                    spacingFlex.setVariable("x");
                }

                eqX.addPolynomial(spacingFlex.coefficient, spacingFlex.power);
            }
            //Add children flexes
            if (!layoutGroupVert)
            {
                //Loop through all children
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].square)
                    {
                        //Add polynomial with minimum dimension 
                        if (layoutGroupVert)
                        {
                            eqX.addPolynomial(size.x, 0); //wVal
                        }
                        else
                        {
                            eqX.addPolynomial(size.y, 0); //hVal
                        }
                        // Debug.Log("Square");
                    }
                    else
                    {
                        //Add the childs flex
                        eqX.addPolynomial(children[i].flex, 1);
                    }
                }
            }
            else
            {
                //Assume it cover entire space
                eqX.addPolynomial(1, 1);
            }

            //Return the calulated value
            return eqX.solveSingleEQ(size.x);
        }

        //Next week or if we get to use this create a system that allows you to make a blueprint or prefab type thing and then you can input a list of rectTransforms to instance which things will be affected

        //Create a new object
        public static Flex newObj(RectTransform UI, float flex)
        {
            return new Flex(UI, flex);
        }

        //Set the Object to be square
        public void setSquare()
        {
            square = true;
        }

        public RectTransform getChild (int index)
        {
           return UI.GetChild(index).GetComponent<RectTransform>();

        }

        //Get the reference to the childs rectTransform
        public static RectTransform getChildRect(RectTransform rect, int childNum)
        {
            
            return rect.gameObject.transform.GetChild(childNum).GetComponent<RectTransform>();
        }

        //Set the layout type
        //Maybe make 2 seperate functions to make things easier?
        public void setLayoutType(bool Vert)
        {
            layoutGroupVert = Vert;
        }

        //Set the object to fill the parent
        public void setFillParent(bool fill)
        {
            fillParent = fill;
        }

        //Set the padding to the same value
        public void setAllPadSame(FlexPolynomial pad)
        {
            //Add a option to do it in pixels for all padding and spacing for later
            padTopFlex = pad;
            padBotFlex = pad;
            padLeftFlex = pad;
            padRightFlex = pad;
        }
        //Set the padding to the same value
        public void setAllPadSame(float padCoef, float padPow)
        {
            //Add a option to do it in pixels for all padding and spacing for later
            padTopFlex = new FlexPolynomial(padCoef, padPow);
            padBotFlex = new FlexPolynomial(padCoef, padPow);
            padLeftFlex = new FlexPolynomial(padCoef, padPow);
            padRightFlex = new FlexPolynomial(padCoef, padPow);
        }

        //Set object to not be modified
        public void setDontModify()
        {
            dontModify = true;
        }

        //Set the objects custom size
        public void setCustomSize(Vector2 size)
        {
            customDim = true;

            if (size.x == 0)
            {
              
                customDimH = true;
                customDimW = false;
                customSize = size;
            } else if (size.y == 0)
            {
              
                customDimH = false;
                customDimW = true;
                customSize = size;
            } else
            {
               
                customDimH = true;
                customDimW = true;
                customSize = size;
            }

           
        }

        //Set the value to multiply by the number of children
        public void setChildMultiW (float WidthMulti)
        {
            useChildMulti = true;
            useChildMultiW = true;
            childMultiW = WidthMulti;
           
        }

        public void setChildMultiH(float HeightMulti)
        {
            useChildMulti = true;
            useChildMultiH = true;
            childMultiH = HeightMulti;
        }



        /*
        public void setChildMulti(float WidthMulti, float HeightMulti)
        {
            useChildMulti = true;
            childMultiW = WidthMulti;
            childMultiH = HeightMulti;
        }
        */


        //Use the default method of calculating the dimensions
        public void defaultMethod(Vector2 thisSize)
        {
            size = new Vector2(defaultWidthCalc(thisSize.x), defaultHeightCalc(thisSize.y));
        }

        //Default method to calculate the unit of one Flex value in Width
        public float defaultWidthCalc(float fullSize)
        {
            //Make a new equation
            Equation width = new Equation();

            //Set Padding
            width.addPolynomial(selfPadLeftFlex);
            width.addPolynomial(selfPadRightFlex);
            //Set Self flex
            width.addPolynomial(1, 1);
            //Return calculated value
            return width.solveSingleEQ(fullSize);
        }

        //Default method to calculate the unit of one Flex value in Height
        public float defaultHeightCalc(float fullSize)
        {
            //Make a new equation
            Equation height = new Equation();

            //Set Padding
            height.addPolynomial(selfPadTopFlex);
            height.addPolynomial(selfPadBotFlex);
            //Set Self flex
            height.addPolynomial(1, 1);
            //Return calculated value
            return height.solveSingleEQ(fullSize);
        }

        //set the UI reference to a certain UI
        public void setUI(RectTransform UI)
        {
            this.UI = UI;
        }

        /*
        public void getChildrenFlex()
        {
            children = new List<Flex>();
            //Check if there are any children under the parent
            if (UI.gameObject.transform.childCount > 0)
            {
                //Loop through all the children and if they have a Flex Component add it to the list of children
                foreach (Transform i in UI.gameObject.transform)
                {
                    //Check if the object has a flex component
                    if (i.gameObject.GetComponent<FlexInfo>() != null)
                    {
                        Flex childFlex = i.gameObject.GetComponent<FlexInfo>().flex;

                        //Add child to the List
                        addChild(childFlex);

                        //Get all children from the current child
                        childFlex.getChildrenFlex();

                    }
                }
            }
            else
            {
                return;
            }
        }
        */

        //Set the layout group type
        public void setLayoutGroup(bool layout, bool vert)
        {
            layoutGroup = layout;
            layoutGroupVert = vert;
        }

        //Function that deletes all Flex Children under it
        public void deleteAllChildren()
        {
            children = new List<Flex>();
        }


        public static Flex findChild (GameObject obj, Flex flex)
        {
            Flex result = null;
            foreach (Flex child in flex.children)
            {
                if (result == null)
                {
                    if (child.UI.gameObject == obj)
                    {
                        result = child;
                    }
                    else
                    {
                        result = findChild(obj, child);
                    }
                }
               
            }

            return result;
        }

    }

}
