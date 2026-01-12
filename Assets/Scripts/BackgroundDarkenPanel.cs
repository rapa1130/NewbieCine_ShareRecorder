using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BackgroundDarkenPanel : MonoBehaviour
{
    public Recording recordButton;
    private Image image;
    private TextMeshProUGUI recordingText;

    public float maxAlpha;
    public float ChangeSpeed;

    private void Start()
    {
        image = GetComponent<Image>();
        recordingText = GetComponentInChildren<TextMeshProUGUI>();
        image.raycastTarget = false;
    }
    // Update is called once per frame
    void Update()
    {

        if (recordButton.isRecording && image.color.a < maxAlpha)
        {
            Color newColor = image.color;
            newColor.a = Mathf.Clamp(newColor.a + Time.deltaTime * ChangeSpeed, 0, maxAlpha);
            image.color = newColor;

            Color textNewColor = recordingText.color;
            textNewColor.a += Time.deltaTime * ChangeSpeed;
            textNewColor.a = Mathf.Clamp(textNewColor.a + Time.deltaTime * ChangeSpeed, 0, maxAlpha);
            recordingText.color = textNewColor;
            SetRayCastTargetOfPanel(true);
        }
        else if(!recordButton.isRecording && recordingText.color.a > 0)
        {
            float newAlpha = Time.deltaTime * ChangeSpeed;

            Color newColor = image.color;
            newColor.a = Mathf.Clamp(newColor.a - newAlpha, 0, 1);
            image.color = newColor;

            Color textNewColor = recordingText.color;
            textNewColor.a = Mathf.Clamp(textNewColor.a - newAlpha, 0, 1);
            recordingText.color = textNewColor;

            SetRayCastTargetOfPanel(false);
        }
    }

    public void SetRayCastTargetOfPanel(bool isRayCast)
    {
        image.raycastTarget = isRayCast;
    }
}
