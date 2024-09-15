using DNAMath;
using DNANeuralNet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseNeuralNet : MonoBehaviour
{
    [SerializeField] protected List<string> Labels;

    [SerializeField] protected RectTransform background;
    [SerializeField] protected RectTransform holder;
    [SerializeField] protected GameObject DispPrefab;
    [SerializeField] protected Image Image;
    [SerializeField] protected Button back;
    [SerializeField] protected Button clear;

    [SerializeField] protected DrawingProcessor drawProc;

    protected DNANeuralNet.DNANeuralNetwork neuro;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clearImage(Texture2D img)
    {
        for (int i = 0; i < img.width; i++)
        {
            for (int j = 0; j < img.height; j++)
            {
                img.SetPixel(i, j, Color.black);
            }
        }
        img.Apply();
    }

    public void NextImage(Texture2D image)
    {
        DNADataPoint data = ImageToData(image, 0, Labels.Count);

        (int label, DNAMatrix results) = neuro.Classify(data.inputs);

        double total = 0;

        foreach (double value in results.Values)
            total += value;

        for (int i = 0; i < Labels.Count; i++)
        {
            holder.GetChild(1).GetChild(1).GetChild(i).GetComponent<ResultDisplay>().setValue(results[i] / total);
        }

        //Display Guess
        holder.GetChild(1).GetChild(2).GetComponent<Text>().text = "Computer thinks this is a:" + Labels[label];
    }

    public DNADataPoint ImageToData(Texture2D image, int labelIndex, int labelNum)
    {
        DNAMatrix pixels = new DNAMatrix(image.height * image.width, 1);

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                Color pixelVal = image.GetPixel(x, y);

                double val = (pixelVal.r + pixelVal.g + pixelVal.b) / 3;

                pixels[x * image.height + y] = val;
            }
        }

        DNADataPoint data = new DNADataPoint(pixels, labelIndex, labelNum);

        return data;

    }
}
