using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexUI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] RectTransform holder;

    [SerializeField] List<string> sceneNames;
    [SerializeField] List<Sprite> backgrounds;
    [SerializeField] List<Texture2D> icon;
    [SerializeField] List<string> sceneName;

    [SerializeField] GameObject cardPrefab;




    // Start is called before the first frame update
    void Start()
    {
        setUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setUI ()
    {
        Flex Holder = new Flex(holder, 1);

        Flex Title = new Flex(Holder.getChild(0), 1, Holder);
        Flex SV = new Flex(Holder.getChild(1), 6, Holder);
        Flex GridView = new Flex(SV.getChild(0).GetChild(0).GetComponent<RectTransform>(), 1, SV);

        GridView.setSelfHorizontalPadding(0.1f, 1, 0.1f, 1);
        GridView.setSelfVerticalPadding(0.1f, 1, 0.1f, 1);

        Holder.setSpacingFlex(0.1f, 1);
       
        GridView.UI.GetComponent<GridLayoutGroup>().cellSize = new Vector2(500, 600);

        Holder.setSize(new Vector2(Screen.width, Screen.height));

        int spacingX = Mathf.FloorToInt((GridView.size.x - (500 * 3)) / 2);
        GridView.UI.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacingX, 100);

        //Add Children and set sizes
        for (int i = 0; i < sceneNames.Count; i ++)
        {
            GameObject Card = Instantiate(cardPrefab, GridView.UI);

            CardScript script = Card.GetComponent<CardScript>();

            script.setName(sceneNames[i]);
            script.setImage(icon[i]);
            script.setColor(backgrounds[i]);

            script.Card.setSize(GridView.UI.GetComponent<GridLayoutGroup>().cellSize);

            int index = i;

            script.onClick.AddListener(delegate
            {
                SceneManager.LoadScene(sceneName[index], LoadSceneMode.Single);
                //Transfer to Appropriate scene
            });

        }

    }
}
