using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleUIVisualizer : MonoBehaviour
{
    public float maxAlpha;
    public float ChangeAlphaSpeed;
    public float MaxSize;
    public bool isOn;

    private float sizeRandomizer;
    private float speedRandomizer;
    private Image image;
    private RectTransform rectTransform;


    virtual protected void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        sizeRandomizer = Random.Range(-100, 100);
        speedRandomizer = Random.Range(0.5f, 1f);
    }



    virtual protected void Update()
    {
        if (isOn && image.color.a < maxAlpha)
        {
            Color newColor = image.color;
            newColor.a += Time.deltaTime * ChangeAlphaSpeed;
            image.color = newColor;

        }
        else if (!isOn && image.color.a > 0)
        {
            float newAlpha = Time.deltaTime * ChangeAlphaSpeed;
            Color newColor = image.color;
            newColor.a = Mathf.Clamp(newColor.a - newAlpha, 0, 1);
            image.color = newColor;
        }
        if(image.color.a > 0)
        {
            WiggleSize();
        }
    }

    private void WiggleSize()
    {
        float absSin = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * speedRandomizer + sizeRandomizer));
        float newSize = MaxSize * absSin * 100;
        rectTransform.sizeDelta = new Vector2(newSize,newSize);
    }
}
