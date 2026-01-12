using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarVisualizing : MonoBehaviour
{

    [SerializeField] private float interval = 70.0f;
    [SerializeField] private float rectZ = 70.0f;
    
    private RectTransform rectTrans;

    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        float ranX = Random.Range(-interval, interval);
        float ranY = Random.Range(-interval, interval);
        rectTrans.anchoredPosition = new Vector3(ranX, ranY, rectZ);
    }
}
