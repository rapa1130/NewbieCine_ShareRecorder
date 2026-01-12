using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RecordManager : MonoBehaviour
{
    public static RecordManager instance = null;
    public string fileNameTemplate = "Record";

    public GameObject recordPlayerPrefab;
    [SerializeField] private GameObject recordLogicalElementPrefab;
    [SerializeField] private GameObject renderingScroll;
    [SerializeField] private RectTransform offsetScroll;
    public GameObject logicalScroll;
    public string audioPath { get; set; }
    public int lastFileIndex;
    public Canvas canvas;

    public Recording _Debug_Recording;
    

    public bool isRemoveFinisihed = true;
    


    public int GetAllFileCount()
    {
        int count = 0;
        string[] fileNames= GetAllFileNames();
        foreach (var name in fileNames)
        {
            if (GetExtensionStr(name) == "wav")
            {
                count++;
            }
        }
        return count;
    }

    private void Awake()
    {
        if (instance == null) instance = this;

        
    }

    private void Start()
    {
        audioPath = "file://" + Application.persistentDataPath + "/";
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        lastFileIndex = GetAllFileCount() - 1;
        int ReadyDistMul2 = ScrollRecordManager.instance.ReadyDistance * 2;
        int initializeLen = fileNames.Length < ReadyDistMul2 ? fileNames.Length - 1 : ReadyDistMul2; 
        InitializeRecordButtonList(initializeLen);
    }



    void InitializeRecordButtonList(int count)
    {
        for(int i = 0; i < count; i++)
        {
            addPlayerButtonToUI(GetPathByIndex(i), false);
        } 
    }



    IEnumerator DelayedRemoveFinished()
    {
        yield return new WaitForSeconds(0.01f);
        isRemoveFinisihed = true;
    }

    // To Do List : Layout Position 바뀌는 거 조정해야함 
    public void RemovePlayerUI(bool isFront)
    {
        isRemoveFinisihed = false;
        RectTransform renderScrollRect = renderingScroll.GetComponent<RectTransform>();
        if (renderScrollRect.childCount == 0) {
            Debug.LogWarning("RecordButton is zero. Can't remove");
            return;
        }
        int indexToRemove;
        if (isFront) {
            indexToRemove = 0;
            //float space = logicalScroll.GetComponent<HorizontalLayoutGroup>().spacing;
            //offsetScroll.anchoredPosition += new Vector2(space, 0);
        }
        else { indexToRemove = renderScrollRect.childCount - 1; }
        Transform tfToRemove = renderScrollRect.GetChild(indexToRemove);
        RecordPlayer rp = tfToRemove.GetComponentInChildren<RecordPlayer>();
        
        Destroy(rp.connectedLogicalElement);
       
        if (isFront)
        {
            float space = logicalScroll.GetComponent<HorizontalLayoutGroup>().spacing;
            offsetScroll.anchoredPosition += new Vector2(space, 0);
        }

        if (rp.starlineLeft != null)
        {
            rp.starlineLeft.transform.SetParent(ObjectPoolManager.instance.pool);
            LineUI lineUIofRP= rp.starlineLeft.GetComponent<LineUI>();
            lineUIofRP.leftTarget.GetComponent<RecordPlayer>().starlineRight = null;
            lineUIofRP.leftTarget.GetComponentInParent<SmoothFollower>().rightLine = null;
            rp.starlineLeft.GetComponent<PoolAble>().ReleaseObject();
        }

        if (rp.starlineRight != null)
        {
            rp.starlineRight.transform.SetParent(ObjectPoolManager.instance.pool);
            LineUI lineUIofRP = rp.starlineRight.GetComponent<LineUI>();
            lineUIofRP.rightTarget.GetComponent<RecordPlayer>().starlineLeft = null;
            lineUIofRP.rightTarget.GetComponentInParent<SmoothFollower>().rightLine = null;
            rp.starlineRight.GetComponent<PoolAble>().ReleaseObject();
        }
        rp.starlineLeft = rp.starlineRight = null;
        tfToRemove.SetParent(ObjectPoolManager.instance.pool);
        tfToRemove.GetComponent<PoolAble>().ReleaseObject();
        LayoutRebuilder.ForceRebuildLayoutImmediate(logicalScroll.GetComponent<RectTransform>());
        StartCoroutine(DelayedRemoveFinished());
    }


    private void addPlayerButtonToUI(string path, bool isFront)
    {
        string fileName = GetOnlyFileName(path);
        if (GetExtensionStr(fileName) != "wav") return;

        GameObject logicalElement = Instantiate(recordLogicalElementPrefab, logicalScroll.transform);

        GameObject rp = ObjectPoolManager.instance.GetGo("Record Player");
        rp.GetComponent<RectTransform>().SetParent(renderingScroll.GetComponent<RectTransform>());
        RecordPlayer recordPlayer = rp.GetComponentInChildren<RecordPlayer>();

        recordPlayer.starlineLeft = null;
        recordPlayer.starlineRight = null;

        if (isFront)
        {
            rp.transform.SetAsFirstSibling();
            logicalElement.transform.SetAsFirstSibling();

            LayoutRebuilder.ForceRebuildLayoutImmediate(logicalScroll.GetComponent<RectTransform>());

            float space = logicalScroll.GetComponent<HorizontalLayoutGroup>().spacing;

            offsetScroll.anchoredPosition += new Vector2(-space, 0);

            if (renderingScroll.transform.childCount != 1)
            {
                GameObject starlineGO = StarLineManager.instance.ConnectFront().gameObject;
                recordPlayer.starlineRight = starlineGO;
                RecordPlayer recordPlayerPrev = renderingScroll.transform.GetChild(1).GetComponentInChildren<RecordPlayer>();
                recordPlayerPrev.starlineLeft = starlineGO;
            }
        }
        else
        {
            if (renderingScroll.transform.childCount != 1)
            {
                GameObject starlineGO = StarLineManager.instance.ConnectLast().gameObject;
                recordPlayer.starlineLeft = starlineGO;
                if(ScrollRecordManager.instance == null)
                {
                    Debug.Log("There is no instance in ScrollRecordManager");
                }
                int lastPrevIndex = renderingScroll.transform.childCount - 2 ;
                RecordPlayer recordPlayerPrev = renderingScroll.transform.GetChild(lastPrevIndex).GetComponentInChildren<RecordPlayer>();
                recordPlayerPrev.starlineRight = starlineGO;

            }
        }

        recordPlayer.path = path;
        recordPlayer.connectedLogicalElement = logicalElement;

        rp.GetComponent<SmoothFollower>().targetRectTransform = logicalElement.GetComponent<RectTransform>();
        StartCoroutine(DelayedSetPosition(rp.GetComponent<SmoothFollower>(),logicalElement.GetComponent<RectTransform>()));
        StartCoroutine(GetAudioToRecordPlayer(audioPath + fileName, rp.GetComponentInChildren<RecordPlayer>()));
        //__Debug
        //rp.GetComponentInChildren<RecordPlayer>().aud.clip = _Debug_Recording.record;
        //__Debug
    }

    IEnumerator DelayedSetPosition(SmoothFollower sf ,RectTransform rect) {
        yield return new WaitForEndOfFrame();
        sf.SetPosition(rect);
    }



    public void AddPlayerButtonToUI_Back()
    {
        int toAddRightIndex = ScrollRecordManager.instance.GetIndexOfRecord(renderingScroll.transform.childCount - 1) + 1;
        if (toAddRightIndex == 0) return;
        if (toAddRightIndex <= lastFileIndex)
        {
            Debug.Log("toAddRightIndex : " + toAddRightIndex);
            string path = GetPathByIndex(toAddRightIndex);
            AddPlayerButtonToUI_Back(path);
        }
    }

    public void AddPlayerButtonToUI_Front()
    {
        
        int toAddLeftIndex = ScrollRecordManager.instance.GetIndexOfRecord(0) - 1;
        if (toAddLeftIndex >= 0)
        {
            Debug.Log("toAddLeftIndex : " + toAddLeftIndex);
            string path = GetPathByIndex(toAddLeftIndex);
            AddPlayerButtonToUI_Front(path);
        }
    }

    public void AddPlayerButtonToUI_Back(string path)
    {
        addPlayerButtonToUI(path, false);
    }

    public void AddPlayerButtonToUI_Front(string path)
    {
        addPlayerButtonToUI(path, true);
    }   


    private string GetOnlyFileName(string longPath)
    {
        string[] pathChunks = longPath.Split('/');
        return pathChunks[pathChunks.Length - 1];
    }

    private string GetExtensionStr(string fileName)
    {
        return fileName.Split('.')[1];
    }

    private string[] GetAllFileNames()
    {
        return Directory.GetFiles(Application.persistentDataPath);
    }

    UnityWebRequest GetAudioFromFile(string path)
    {
        UnityWebRequest request =  UnityWebRequestMultimedia.GetAudioClip(path,AudioType.WAV);
        return request;
    }

    IEnumerator GetAudioToRecordPlayer(string path,RecordPlayer player)
    {
        UnityWebRequest request = GetAudioFromFile(path);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Load Fail");
            yield break;
        }
        player.aud.clip = DownloadHandlerAudioClip.GetContent(request);
        player.aud.clip.name = GetOnlyFileName(path);
        player.recordNumber = GetNumberFromPath(path);
        
    }

    int GetNumberFromPath(string path)
    {
        string numStr = path.Split('_')[1].Split('.')[0];
        return int.Parse(numStr);
    }

    string GetPathByIndex(int index)
    {
        return audioPath + fileNameTemplate +'_'+ index + ".wav";
    }

}