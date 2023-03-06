using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FlexUI;
using DNANeuralNetwork;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class DigitRecScript : MonoBehaviour
{
    [SerializeField] List<string> testingImagePaths = new List<string>();
    [SerializeField] List<string> Labels;
    [SerializeField] RectTransform holder;
    [SerializeField] GameObject DispPrefab;

    [SerializeField] Image Image;
    [SerializeField] Button Next;
    [SerializeField] Text debug;
    [SerializeField] Button back;

    NeuralNetwork neuro;
    List<DataPoint> allData = new List<DataPoint>();
    List<Texture2D> allImages = new List<Texture2D>();

    Vector2Int size = new Vector2Int(28, 28);
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        setUI();
        // Load.onClick.AddListener(loadNetwork);
        loadNetwork();
        Next.onClick.AddListener(NextImage);
        back.onClick.AddListener(backToMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setUI ()
    {
        Flex Holder = new Flex(holder, 1);

        Flex ImageHolder = new Flex(Holder.getChild(0), 2f, Holder);
        Flex Img = new Flex(ImageHolder.getChild(0), 1, ImageHolder);

        Flex SideBar = new Flex(Holder.getChild(1), 1, Holder);

        Flex Results = new Flex(SideBar.getChild(0), 9, SideBar);

        Flex Guess = new Flex(SideBar.getChild(1), 1, SideBar);

        Flex Buttons = new Flex(SideBar.getChild(2), 1, SideBar);

       // Flex LoadBTN = new Flex(Buttons.getChild(0), 1, Buttons);
        Flex NextBTN = new Flex(Buttons.getChild(0), 1, Buttons);

        Results.setSpacingFlex(2f, 1);
        //Add Children
        for (int i = 0; i < Labels.Count; i ++)
        {
            GameObject ResDisp = Instantiate(DispPrefab, Results.UI);

            ResultDisplay script = ResDisp.GetComponent<ResultDisplay>();

            Results.addChild(script.Result);

            script.setLabel(Labels[i]);

        }

        Holder.setSelfHorizontalPadding(0.05f, 1, 0.05f, 1);
        Holder.setSelfVerticalPadding(0.1f, 1, 0.1f, 1);

        Img.setSquare();

        Holder.setSize(new Vector2(Screen.width, Screen.height));

    }

    void loadNetwork ()
    {
        neuro = loadNeuralNetwork("NeuralNetworks/DigitRec/DigitNetwork");
       
        neuro.SetActivationFunction(Activation.GetActivationFromType(Activation.ActivationType.ReLU), Activation.GetActivationFromType(Activation.ActivationType.Softmax));

        for (int i = 0; i < testingImagePaths.Count; i++)
        {
            //Load Fours first
            List<Texture2D> loadedImages = Resources.LoadAll<Texture2D>(testingImagePaths[i]).ToList();

            foreach (Texture2D img in loadedImages)
            {
                //Convert to Datapoint
                allData.Add(imageToData(img, i, testingImagePaths.Count));
                allImages.Add(img);
            }
        }

        //Shuffle
        List<DataPoint> shuffle = new List<DataPoint>();
        List<Texture2D> imgs = new List<Texture2D>();

        while (allData.Count > 0)
        {
            int ran = Random.Range(0, allData.Count);

            shuffle.Add(allData[ran]);
            imgs.Add(allImages[ran]);

            allData.RemoveAt(ran);
            allImages.RemoveAt(ran);
        }
        allData = shuffle;
        allImages = imgs;

        //Display First
        NextImage();
    }

    void NextImage ()
    {

        DataPoint data = allData[index];

        Texture2D img = allImages[index];

        Rect rec = new Rect(0, 0, img.width, img.height);

        Image.sprite = Sprite.Create(img, rec, new Vector2(0.5f, 0.5f), 1);

        (int label, double[] results) = neuro.Classify(data.inputs);

        for (int i = 0; i < Labels.Count; i++)
        {
            holder.GetChild(1).GetChild(0).GetChild(i).GetComponent<ResultDisplay>().setValue(results[i]);
        }

        //Display Guess
        holder.GetChild(1).GetChild(1).GetComponent<Text>().text = "The Computer thinks this is a " + Labels[label];

        index++;
    }

    public NeuralNetwork loadSaveFromPathNeuralNetwork(string path)
    {
        //This function loads the save named into the currently used save file

        //Debug.Log(path);
        string jsonData = "";
        if (File.Exists(path))
        {
            //Extract JSON Data
            jsonData = File.ReadAllText(path);
            Debug.Log(jsonData);
            return JsonUtility.FromJson<NeuralNetwork>(jsonData);
        }
        else
        {
            Debug.Log("Doesn't exist");
            return null;
        }

    }

    public NeuralNetwork loadNeuralNetwork (string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        return JsonUtility.FromJson<NeuralNetwork>(json.text);
    }    

    public DataPointFile loadDataFile(string path)
    {
        //This function loads the save named into the currently used save file

        //Debug.Log(path);
        string jsonData = "";
        if (File.Exists(path))
        {
            //Extract JSON Data
            jsonData = File.ReadAllText(path);
            Debug.Log(jsonData);
            return JsonUtility.FromJson<DataPointFile>(jsonData);
        }
        else
        {
            Debug.Log("Doesn't exist");
            return null;
        }

    }

    public Texture2D dataToImage(DataPoint data, Vector2Int size)
    {
        //Gte Image size
        //Create texture2D 

        Texture2D img = new Texture2D(size.x, size.y);

        for (int x = 0; x < img.width; x++)
        {
            for (int y = 0; y < img.height; y++)
            {
                //Get input index
                //Create color
                float colVal = (float)data.inputs[x * img.height + y];

                Color col = new Color(colVal, colVal, colVal);

                img.SetPixel(x, y, col);
            }
        }

        return img;

    }

    public DataPoint imageToData(Texture2D image, int labelIndex, int labelNum)
    {

        double[] pixels = new double[image.width * image.height];

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                Color pixelVal = image.GetPixel(x, y);

                double val = (pixelVal.r + pixelVal.g + pixelVal.b) / 3;

                pixels[x * image.width + y] = val;
            }
        }

        DataPoint data = new DataPoint(pixels, labelIndex, labelNum);

        return data;

    }

    void backToMenu ()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
