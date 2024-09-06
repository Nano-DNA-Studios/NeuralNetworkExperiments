using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DNANeuralNet;
using DNANeuralNetwork;
using FlexUI;
using System.IO;
using System.Linq;
using DNAMath;
using UnityEngine.SceneManagement;

public class MatrixNeuralNetwork : MonoBehaviour
{
    [SerializeField] List<string> Labels;

    [SerializeField] RectTransform background;
    [SerializeField] RectTransform holder;
    [SerializeField] GameObject DispPrefab;
    [SerializeField] Image Image;
    [SerializeField] Button back;
    [SerializeField] Button clear;

    [SerializeField] DrawingProcessor drawProc;

    DNANeuralNet.DNANeuralNetwork neuro;

    int textSize;

    bool firstRun = true;

    int count = 0;

    private void Awake()
    {
        setUI();
        back.onClick.AddListener(backToMenu);

        clear.onClick.AddListener(delegate
        {
            clearImage(drawProc.image);
        });

        LoadNetwork();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadNetwork()
    {
        //Load Neural Network
        neuro = loadNeuralNetwork("NeuralNetworks/MatrixGPUNeuro/NeuroMatrixRELU");
        //neuro = loadNeuralNetwork("NeuralNetworks/Shapes/BestNeuroShapes");

        neuro.InitializeFromLoad();
    }

    void setUI()
    {
        Flex Background = new Flex(background, 1);

        Flex BackButtonHolder = new Flex(Background.getChild(0), 1, Background);

        Flex BackButton = new Flex(BackButtonHolder.getChild(0), 1, BackButtonHolder);

        Flex Holder = new Flex(holder, 10, Background);

        Flex ImageHolder = new Flex(Holder.getChild(0), 2f, Holder);

        Flex Img = new Flex(ImageHolder.getChild(0), 1, ImageHolder);

        Flex SideBar = new Flex(Holder.getChild(1), 2, Holder);

        Flex TitleBar = new Flex(SideBar.getChild(0), 1, SideBar);

        Flex Results = new Flex(SideBar.getChild(1), 9, SideBar);

        Flex Guess = new Flex(SideBar.getChild(2), 1, SideBar);

        Flex Buttons = new Flex(Background.getChild(2), 1, Background);

        // Flex LoadBTN = new Flex(Buttons.getChild(0), 1, Buttons);
        Flex NextBTN = new Flex(Buttons.getChild(0), 1, Buttons);

        Results.setSpacingFlex(1f, 1);
        Results.setSelfHorizontalPadding(0.02f, 1, 0.02f, 1);
        TitleBar.setHorizontalPadding(0.02f, 1, 0.02f, 1);
        Guess.setSelfHorizontalPadding(0.02f, 1, 0.02f, 1);

        //Add the Explainer Text first

        GameObject ResDisp = Instantiate(DispPrefab, TitleBar.UI);

        ResultDisplay script = ResDisp.GetComponent<ResultDisplay>();

        TitleBar.addChild(script.Result);

        script.setLabel("Label");
        script.setValue("Confidence");

        //Add Children
        for (int i = 0; i < Labels.Count; i++)
        {
            ResDisp = Instantiate(DispPrefab, Results.UI);

            script = ResDisp.GetComponent<ResultDisplay>();

            Results.addChild(script.Result);

            script.setLabel(Labels[i]);

        }

        ImageHolder.setAllPadSame(0.5f, 1);

        Img.setSquare();
        BackButton.setSquare();

        Background.setSize(new Vector2(Screen.width, Screen.height));

        if (ImageHolder.size.x > ImageHolder.size.y)
        {
            Img.setSize(new Vector2(ImageHolder.size.y, ImageHolder.size.y));
        }
        else
        {
            Img.setSize(new Vector2(ImageHolder.size.x, ImageHolder.size.x));
        }

        textSize = Results.getChild(0).GetChild(0).GetComponent<Text>().fontSize;
        Guess.UI.GetComponent<Text>().fontSize = textSize;
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

    void backToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public DNANeuralNet.DNANeuralNetwork loadNeuralNetwork(string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        return JsonUtility.FromJson<DNANeuralNet.DNANeuralNetwork>(json.text);
    }

    public void NextImage(Texture2D image)
    {
        DNADataPoint data = imageToData(image, 0, 10);

        (int label, DNAMatrix results) = neuro.Classify(data.inputs);

        double total = 0;

        foreach (double value in results.Values)
            total += value;

        for (int i = 0; i < 10; i++)
        {
            holder.GetChild(1).GetChild(1).GetChild(i).GetComponent<ResultDisplay>().setValue(results[i]/total);
        }

        //Display Guess
        holder.GetChild(1).GetChild(2).GetComponent<Text>().text = "Computer thinks this is a:" + Labels[label];
    }

    public DNADataPoint imageToData(Texture2D image, int labelIndex, int labelNum)
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
