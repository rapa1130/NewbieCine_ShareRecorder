using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineUI : PoolAble
{
    private Canvas canvas;
    private RectTransform imageRectTransform;
    public float lineWidth = 1.0f;

    public RectTransform leftTarget;
    public RectTransform rightTarget;

    //객체 비활성화시 마지막 위치고정
    //양 점 모두 화면 바깥시 삭제하는 기능 만들


    void Start () {
        imageRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(!leftTarget.gameObject.activeSelf || !rightTarget.gameObject.activeSelf) {
            Debug.Log("이거 되고있는거 맞나?");
            GetComponent<PoolAble>().ReleaseObject();
        }


        Vector3 differenceVector = rightTarget.position - leftTarget.position;
        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.position = leftTarget.position;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
        
    }

}
