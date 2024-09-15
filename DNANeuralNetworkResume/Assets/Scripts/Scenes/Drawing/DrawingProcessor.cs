using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

public class DrawingProcessor : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public Texture2D image;


    Vector2 imgSize;
    bool draw;
    Vector3[] corners;
    Vector2 min;
    Vector2 max;
    Vector2 size;

    bool IsCustDigit;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<BoxCollider2D>().size = this.GetComponent<RectTransform>().sizeDelta;

        Rect rec = new Rect(0, 0, image.width, image.height);
        this.GetComponent<Image>().sprite = Sprite.Create(image, rec, new Vector2(0.5f, 0.5f), 1);

        if (Camera.main.GetComponent<CustDigitScript>() != null)
            IsCustDigit = true;
        else
            IsCustDigit = false;

        if (IsCustDigit)
            Camera.main.GetComponent<CustDigitScript>().clearImage(image);
        else
            Camera.main.GetComponent<BaseNeuralNet>().clearImage(image);
    }

    // Update is called once per frame
    void Update()
    {
       if (min.x <= 0)
        {
            getInitSettings();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        draw = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Do I need the draw?
        if (draw)
        {
            //Probably coordinated

            Vector2 position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - min;

            Vector2 normalize = new Vector2(position.x / size.x, position.y / size.y);

            Vector2 pixelPos = normalize * imgSize;

            Vector2Int imgPos = new Vector2Int(Mathf.FloorToInt(pixelPos.x), Mathf.FloorToInt(pixelPos.y));

            if (imgPos.x >= 0 && imgPos.x < image.width)
            {
                if (imgPos.y >= 0 && imgPos.y < image.height)
                {
                    //Draw Pixel and Update
                    image.SetPixel(imgPos.x, imgPos.y, Color.white);
                    image.Apply();

                    Rect rec = new Rect(0, 0, image.width, image.height);
                    this.GetComponent<Image>().sprite = Sprite.Create(image, rec, new Vector2(0.5f, 0.5f), 1);

                    //Convert to Data Point and have Network Guess
                    if (IsCustDigit)
                        Camera.main.GetComponent<CustDigitScript>().NextImage(image);
                    else
                        Camera.main.GetComponent<BaseNeuralNet>().NextImage(image);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        draw = false;
    }

   public void getInitSettings ()
    {
        corners = new Vector3[4];
        this.GetComponent<RectTransform>().GetWorldCorners(corners);

        min = new Vector2(Camera.main.WorldToScreenPoint(corners[0]).x, Camera.main.WorldToScreenPoint(corners[0]).y);
        max = new Vector2(Camera.main.WorldToScreenPoint(corners[2]).x, Camera.main.WorldToScreenPoint(corners[2]).y);

        size = max - min;

        imgSize = new Vector2(image.width, image.height);

    }
}