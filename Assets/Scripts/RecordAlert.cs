using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordAlert : MonoBehaviour
{
    TextMeshProUGUI text;
    public float pesistTime = 3.0f;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Color color = text.color;
        color.a = 0;
        text.color = color;
    }

    public void SetVisible()
    {
        Color color = text.color;
        color.a = 1;
        text.color = color;
        StartCoroutine(SetInvisible());
    }

    IEnumerator SetInvisible()
    {
        yield return new WaitForSeconds(pesistTime);
        Color color = text.color;
        color.a = 0;
        text.color = color;
    }

    public void SetVisibility(bool isVisible)
    {
        Color color = text.color;
        color.a = isVisible ? 1 : 0;
        text.color = color;
    }
}
