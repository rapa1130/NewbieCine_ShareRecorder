using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordButtonImageChanger : MonoBehaviour
{
    private Image image;
    public Sprite recordSprite;
    public Sprite stopSprite;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ToggleButtonImage()
    {
        if(image.sprite == recordSprite)
        {
            SetStopImage();
        }
        else
        {
            SetRecordImage();
        }
    }
    // Update is called once per frame
    public void SetRecordImage()
    {
        image.sprite = recordSprite;
    }
    public void SetStopImage()
    {
        image.sprite = stopSprite;
    }
}
