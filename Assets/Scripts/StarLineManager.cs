using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarLineManager : MonoBehaviour
{
    public static StarLineManager instance = null;
    [SerializeField] private RectTransform renderingView;
    [SerializeField] private RectTransform LineUIPanel;
    [SerializeField] private LineUI lineUI_Prefab;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public LineUI ConnectLast()
    {
        Transform lastElement = renderingView.GetChild(renderingView.childCount - 1);
        Transform lastSecondElement = renderingView.GetChild(renderingView.childCount - 2);
        LineUI lineUI = MakeLine(lastSecondElement.gameObject, lastElement.gameObject);
        lastElement.GetComponent<SmoothFollower>().leftLine = lineUI;
        lastSecondElement.GetComponent<SmoothFollower>().rightLine = lineUI;
        return lineUI;
    }

    public LineUI ConnectFront()
    {
        Transform lastElement = renderingView.GetChild(0);
        Transform lastSecondElement = renderingView.GetChild(1);

        LineUI lineUI = MakeLine(lastElement.gameObject, lastSecondElement.gameObject);
        lastSecondElement.GetComponent<SmoothFollower>().leftLine = lineUI;
        lastElement.GetComponent<SmoothFollower>().rightLine = lineUI;
        return lineUI;
    }


    public LineUI MakeLine(GameObject panelA, GameObject panelB) {
        LineUI lineUI = ObjectPoolManager.instance.GetGo("Line Renderer").GetComponent<LineUI>();
        lineUI.GetComponent<RectTransform>().SetParent(LineUIPanel);
        //LineUI lineUI = Instantiate(lineUI_Prefab,LineUIPanel);

        lineUI.leftTarget = GetButtonRectTransform(panelA);
        lineUI.rightTarget = GetButtonRectTransform(panelB);
        return lineUI;
    }

    RectTransform GetButtonRectTransform(GameObject panel)
    {
        return panel.GetComponentInChildren<Button>().GetComponent<RectTransform>();
    }



}
