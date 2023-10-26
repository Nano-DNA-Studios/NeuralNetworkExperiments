using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNANeuralNetwork;
using DNANeuralNet;
using DNAMath;


[System.Serializable]
public class ImageData
{

    //Expected outputs can have multiple vals I guess
    //Label can be left empty

    //Add greyscale Functionality Later

    public double[] pixelVals;
    public double[] expectedOutputs;
    public int label;
    public bool useLabel;
    public bool greyScale;
    public Vector2Int imgSize;
    public int numPixels;

    public ImageData(Texture2D image, double[] outputs, int label, bool useLabel = true, bool greyScale = false)
    {
       

        this.greyScale = greyScale;
        if (greyScale)
        {
            this.numPixels = image.width * image.height;
        } else
        {
            this.numPixels = image.width * image.height * 3;
        }
       

        imgSize = new Vector2Int(image.width, image.height);

        //Convert texture 2d to inputs
        pixelVals = TextureToPixels(image);

        //Save expected outputs
        expectedOutputs = outputs;


        this.useLabel = useLabel;
        //Save Label
        if (useLabel)
        {
            this.label = label;
        }

        Debug.Log(pixelVals.Length);
        Debug.Log(greyScale);

    }

    public ImageData (double[] pixelVals, double[] outputs, int label, Vector2Int imgSize, bool useLabel = true, bool greyScale = false)
    {
        this.numPixels = imgSize.x * imgSize.y;

        this.greyScale = greyScale;

        this.imgSize = imgSize;

        //Convert texture 2d to inputs
        this.pixelVals = pixelVals;

        //Save expected outputs
        expectedOutputs = outputs;

        this.useLabel = useLabel;
        //Save Label
        if (useLabel)
        {
            this.label = label;
        }
    }


    public double[] TextureToPixels(Texture2D image)
    {
        List<double> values = new List<double>();

        for (int i = 0; i < imgSize.x; i++)
        {
            for (int j = 0; j < imgSize.y; j++)
            {
                if (greyScale)
                {
                    values.Add(image.GetPixel(i, j).r);
                } else
                {
                    //Red
                    values.Add(image.GetPixel(i, j).r);

                    //Green 
                    values.Add(image.GetPixel(i, j).g);

                    //Blue
                    values.Add(image.GetPixel(i, j).b);
                }
               
            }
        }

        return values.ToArray();
    }

    Texture2D PixelsToImage()
    {
        int xIndex = 0;
        int yIndex = 0;

        Texture2D image = new Texture2D(imgSize.x, imgSize.y);

        if (greyScale)
        {
            for (int i = 0; i < pixelVals.Length; i++)
            {

                //get colour
                Color col = new Color((float)pixelVals[i], (float)pixelVals[i], (float)pixelVals[i]);

                //Set Pixel
                image.SetPixel(xIndex, yIndex, col);

                //Add Index
                yIndex++;

                if (yIndex == imgSize.y)
                {
                    yIndex = 0;
                    xIndex++;
                }
            }

        } else
        {
            for (int i = 0; i < pixelVals.Length; i = i + 3)
            {

                //get colour
                Color col = new Color((float)pixelVals[i], (float)pixelVals[i + 1], (float)pixelVals[i + 2]);

                //Set Pixel
                image.SetPixel(xIndex, yIndex, col);

                //Add Index
                yIndex++;

                if (yIndex == imgSize.y)
                {
                    yIndex = 0;
                    xIndex++;
                }
            }
        }

       

        return image;
    }

    public void setImage(Texture2D image)
    {
        pixelVals = TextureToPixels(image);
    }

    public Texture2D getImage()
    {
        return PixelsToImage();
    }

    public DataPoint GetDataPoint (bool singleLabel = false)
    {
        if (singleLabel)
        {
            return new DataPoint(pixelVals, label, expectedOutputs.Length);
        } else
        {
            return new DataPoint(pixelVals, expectedOutputs);
        }
        
    }

    public DNADataPoint GetDNADataPoint(bool singleLabel = false)
    {
        DNAMatrix pixelVals = new DNAMatrix(imgSize.x, imgSize.y);
        DNAMatrix expectedOutputs = new DNAMatrix(this.expectedOutputs.Length, 1);

        expectedOutputs.Values = this.expectedOutputs;
        pixelVals.Values = this.pixelVals;

        if (singleLabel)
        {
            return new DNADataPoint(pixelVals, label, expectedOutputs.Values.Length);
        }
        else
        {
            return new DNADataPoint(pixelVals, expectedOutputs);
        }

    }

    public void EditImage (bool whiteBackground)
    {
        System.Random rng = new System.Random();

        Texture2D texture = getImage();

        //Apply Image Edits

        //
        //Tomorrow, try and find a method to edit these is a thread safe way
        //

        //Maybe try converting everything into an array/list of colors (just in case no grayscale) and then it has to convert to get the index of the right pixel
        //Then you apply the edit in the same way

        double scale = 1 + RandomInNormalDistribution(rng) * 0.1;
        texture = ApplyScale(texture, (float)scale, whiteBackground);

        //Maybe edit this
        float angle = (float)RandomInNormalDistribution(rng) * 10;
        texture = ApplyRotation(texture, angle, whiteBackground);

        //Maybe put it back to /10
        int offsetX = Mathf.FloorToInt((float)RandomInNormalDistribution(rng) * (imgSize.x / 5));
        int offsetY = Mathf.FloorToInt((float)RandomInNormalDistribution(rng) * (imgSize.y / 5));
        texture = ApplyOffset(texture, offsetX, offsetY, whiteBackground);

        texture = ApplyNoise(texture);

        pixelVals = TextureToPixels(texture);
    }

    public Texture2D ApplyRotation(Texture2D image, float angle, bool whiteBackground)
    {
        float angRad = (Mathf.PI / 180) * angle;

        Texture2D newImage = new Texture2D(image.width, image.height);


        for (int xIndex = 0; xIndex < newImage.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < newImage.height; yIndex++)
            {

                int xCenter = Mathf.FloorToInt(image.width / 2);
                int yCenter = Mathf.FloorToInt(image.height / 2);

                int translatedX = xIndex - xCenter;
                int translatedY = yIndex - yCenter;


                int oldX = Mathf.FloorToInt(translatedX * Mathf.Cos(angRad) - translatedY * Mathf.Sin(angRad));

                int oldY = Mathf.FloorToInt(translatedX * Mathf.Sin(angRad) + translatedY * Mathf.Cos(angRad));

                newImage.SetPixel(xIndex, yIndex, getValidPixel(image, oldX, oldY, whiteBackground));

            }
        }
        return newImage;
    }

    public Color getValidPixel(Texture2D image, int oldX, int oldY, bool whiteBackground)
    {
        bool verdict = true;

        int x = oldX + Mathf.FloorToInt(image.width / 2);
        int y = oldY + Mathf.FloorToInt(image.height / 2);

        if ((x < image.width && x >= 0) && (y < image.height && y >= 0))
        {
            //Debug.Log("Hi");
            return image.GetPixel(x, y);
        }
        else
        {
            // Debug.Log("White");
            if (whiteBackground)
            {
                return Color.white;
            }
            else
            {
                return Color.black;
            }

        }

    }

    public Texture2D ApplyNoise(Texture2D image)
    {
        //5% of pixels get noise


        //Number determines the seed to use
        System.Random rng = new System.Random(Random.Range(0, 100000));

        double noiseProbability = (float)System.Math.Min(rng.NextDouble(), rng.NextDouble()) * 0.05f;
        double noiseStrength = (float)System.Math.Min(rng.NextDouble(), rng.NextDouble());

        Texture2D newImage = image;

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {

                if (rng.NextDouble() <= noiseProbability)
                {
                    double noiseValue = (rng.NextDouble() - 0.5) * noiseStrength;

                    float pixelVal = newImage.GetPixel(x, y).r;

                    pixelVal = System.Math.Clamp(pixelVal - (float)noiseValue, 0, 1);

                    newImage.SetPixel(x, y, new Color(pixelVal, pixelVal, pixelVal));
                }
            }
        }

        return newImage;
    }

    public Texture2D ApplyOffset(Texture2D image, int offsetX, int offsetY, bool whiteBackground)
    {
        Texture2D newImage = new Texture2D(image.width, image.height);

        for (int xIndex = 0; xIndex < newImage.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < newImage.height; yIndex++)
            {

                //Set pixel to the pixel value of the negative offset

                Color col;

                int posX = xIndex - offsetX;
                int posY = yIndex - offsetY;

                bool valid = false;

                if (posX >= 0 && posX <= newImage.width)
                {
                    if (posY >= 0 && posY <= newImage.width)
                    {
                        valid = true;
                    }
                }

                if (valid)
                {
                    col = image.GetPixel(posX, posY);
                }
                else
                {
                    if (whiteBackground)
                    {
                        col = Color.white;
                    }
                    else
                    {
                        col = Color.black;
                    }

                }

                newImage.SetPixel(xIndex, yIndex, col);

            }
        }
        return newImage;


    }

    public Texture2D ApplyScale(Texture2D image, float scaleMult, bool whiteBackground)
    {
        Texture2D newImage = new Texture2D(image.width, image.height);

        for (int xIndex = 0; xIndex < newImage.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < newImage.height; yIndex++)
            {
                int xCenter = Mathf.FloorToInt(image.width / 2);
                int yCenter = Mathf.FloorToInt(image.height / 2);

                int translatedX = xIndex - xCenter;
                int translatedY = yIndex - yCenter;

                //Get radius
                //Divide by multiplier
                float oldRadius = Mathf.Sqrt((translatedX * translatedX) + (translatedY * translatedY)) / scaleMult;

                //Get angle
                float angle = Mathf.Atan2(translatedY, translatedX);

                int oldX = Mathf.FloorToInt(oldRadius * Mathf.Cos(angle));

                int oldY = Mathf.FloorToInt(oldRadius * Mathf.Sin(angle));

                newImage.SetPixel(xIndex, yIndex, getValidPixel(image, oldX, oldY, whiteBackground));

            }
        }
        return newImage;
    }

    static double RandomInNormalDistribution(System.Random prng, double mean = 0, double standardDeviation = 1)
    {
        double x1 = 1 - prng.NextDouble();
        double x2 = 1 - prng.NextDouble();

        double y1 = System.Math.Sqrt(-2.0 * System.Math.Log(x1)) * System.Math.Cos(2.0 * System.Math.PI * x2);
        return y1 * standardDeviation + mean;
    }








}


