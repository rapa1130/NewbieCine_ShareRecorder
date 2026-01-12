using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRecordManager : MonoBehaviour
{
    public static ScrollRecordManager instance;
    [SerializeField] public RectTransform LogicalView;
    [SerializeField] public RectTransform RenderingView;

    public int RemoveDistance = 30; 
    public int ReadyDistance = 20;

    public int GetIndexOfRecord(int index)
    {
        return RenderingView.transform.GetChild(index).GetComponentInChildren<RecordPlayer>().recordNumber;
    }
    public int GetFrontIndexOfRecords()
    {
        int recNum = RenderingView.transform.GetChild(1).GetComponentInChildren<RecordPlayer>().recordNumber;
        Debug.Log("recNum : " + recNum);
        return recNum;
    }
    public int GetLastIndexOfRecords()
    {
        Transform lastRecordTF = RenderingView.transform.GetChild(RenderingView.transform.childCount - 1);
        return lastRecordTF.GetComponentInChildren<RecordPlayer>().recordNumber;
    }
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
