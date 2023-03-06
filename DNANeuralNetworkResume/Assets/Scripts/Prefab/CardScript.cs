using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexUI;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    public Flex Card;

    [SerializeField] Image image;
    [SerializeField] Text name;
    [SerializeField] Button button;
    [SerializeField] Image background;


    public Button.ButtonClickedEvent onClick;

    private void Awake()
    {
        setUI();
        onClick = button.onClick;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setUI ()
    {
        Card = new Flex(this.GetComponent<RectTransform>(), 1);

        Flex ImageHolder = new Flex(Card.getChild(0), 5, Card);
        Flex Image = new Flex(ImageHolder.getChild(0), 1, ImageHolder);

        Flex Name = new Flex(Card.getChild(1), 1, Card);

        ImageHolder.setAllPadSame(0.1f, 1);
        Image.setSquare();

        Card.setSize(new Vector2(500, 600));

    }

    public void setImage (Texture2D sprite)
    {
       image.sprite = Sprite.Create(sprite, new Rect(new Vector2(0, 0), new Vector2(sprite.width, sprite.height)), new Vector2(0, 0));
    }

    public void setName (string text)
    {
        name.text = text;
    }

    public void setColor (Texture2D sprite)
    {
        background.sprite = Sprite.Create(sprite, new Rect(new Vector2(0, 0), new Vector2(sprite.width, sprite.height)), new Vector2(0, 0));
    }




}
