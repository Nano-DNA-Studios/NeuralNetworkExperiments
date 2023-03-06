using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FlexUI;

public class ResultDisplay : MonoBehaviour
{
    public Flex Result;

    [SerializeField] Text label;
    [SerializeField] Text value;


    public Button.ButtonClickedEvent onClick;

    private void Awake()
    {
        setUI();
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void setUI()
    {
        Result = new Flex(this.GetComponent<RectTransform>(), 1);

        Flex Label = new Flex(Result.getChild(0), 1, Result);
        Flex Value = new Flex(Result.getChild(1), 1, Result);

    }

    public void setLabel (string name)
    {
        label.text = name + ":";
    }

    public void setValue (double val)
    {
        value.text = (float)Mathf.FloorToInt((float)val * 1000000) / 10000 + "%";
    }

}
