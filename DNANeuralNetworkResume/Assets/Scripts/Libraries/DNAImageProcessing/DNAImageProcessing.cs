using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNAImageProcessing
{

    public class DNAImageProcessing
    {

        static bool whiteBackground;
        public static void ExpandImage(Texture2D image, int scaleFactor, string name)
        {
            //Create a copy image of expanded dimensions

            Texture2D newImage = new Texture2D(image.width * scaleFactor, image.height * scaleFactor);

            for (int xIndex = 0; xIndex < newImage.width; xIndex++)
            {
                for (int yIndex = 0; yIndex < newImage.height; yIndex++)
                {

                    int xPos = Mathf.FloorToInt(xIndex / scaleFactor);
                    int yPos = Mathf.FloorToInt(yIndex / scaleFactor);

                    newImage.SetPixel(xIndex, yIndex, image.GetPixel(xPos, yPos));

                }
            }

        }

        public static Texture2D ApplyRotation(Texture2D image, float angle)
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

                    newImage.SetPixel(xIndex, yIndex, getValidPixel(image, oldX, oldY));

                }
            }
            return newImage;
        }

        public static Color getValidPixel(Texture2D image, int oldX, int oldY)
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


        public static void Compress(Texture2D image, int scaleFactor, string name)
        {
            Texture2D newImage = new Texture2D(image.width / scaleFactor, image.height / scaleFactor);

            for (int xIndex = 0; xIndex < newImage.width; xIndex++)
            {
                for (int yIndex = 0; yIndex < newImage.height; yIndex++)
                {

                    float red = 0;
                    float green = 0;
                    float blue = 0;

                    for (int i = 0; i < scaleFactor; i++)
                    {
                        for (int j = 0; j < scaleFactor; j++)
                        {
                            red += image.GetPixel(xIndex * scaleFactor + i, yIndex * scaleFactor + j).r;
                            green += image.GetPixel(xIndex * scaleFactor + i, yIndex * scaleFactor + j).g;
                            blue += image.GetPixel(xIndex * scaleFactor + i, yIndex * scaleFactor + j).b;

                        }
                    }

                    //Get the averages
                    red = red / (scaleFactor * scaleFactor);
                    green = green / (scaleFactor * scaleFactor);
                    blue = blue / (scaleFactor * scaleFactor);

                    newImage.SetPixel(xIndex, yIndex, new Color(red, green, blue));
                }

            }

        }

        public static Texture2D ApplyNoise(Texture2D image)
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

        public static Texture2D ApplyOffset(Texture2D image, int offsetX, int offsetY)
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

        public static Texture2D ApplyScale(Texture2D image, float scaleMult)
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

                    newImage.SetPixel(xIndex, yIndex, getValidPixel(image, oldX, oldY));

                }
            }
            return newImage;
        }
    }
}

