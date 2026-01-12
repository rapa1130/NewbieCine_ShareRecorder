using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothFollower : PoolAble
{

    public RectTransform targetRectTransform;
    public float followingSpeed;
    public float fadeSpeed = 1.0f;
    public float MoveThreshold = 0.1f;
    public float ExtraKeepingRecordLength = 2;

    public float startDelay = 0.05f;

    public LineUI leftLine;
    public LineUI rightLine;

    private Canvas canvas;
    private RectTransform rectTransform;
    private Button recordPlayButton;
    private RectTransform buttonRectTransform;
    private Transform parentTrans;

    private bool isPreviousInScreen;
    private Camera cam;
    private bool isReady = false;

    

    private void Start()
    {
       
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas_Rendering").GetComponent<Canvas>();
        cam = canvas.worldCamera;
        recordPlayButton = GetComponentInChildren<Button>();
        SetButtonAlpha(0.0f);
        buttonRectTransform = recordPlayButton.GetComponent<RectTransform>();
        SetPosition(targetRectTransform);
        parentTrans = transform.parent;
        
        StartCoroutine(StartDelay());
    }

    IEnumerator DelayedSetPostion()
    {
        yield return new WaitForSeconds(0.05f);
        SetPosition(targetRectTransform);
    }

    public void SetImageAlpha(Image image, float alpha)
    {
        Color alphaedColor = image.color;
        alphaedColor.a = alpha;
        image.color = alphaedColor;
    }

    public void SetButtonAlpha(float alpha)
    {
        Color alphaedColor = recordPlayButton.image.color;
        alphaedColor.a = alpha;
        recordPlayButton.image.color = alphaedColor;

    }
    private bool CheckHaveToMove(RectTransform target,RectTransform toMove, float threshold)
    {
        return (target.position - toMove.position).magnitude > threshold;
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        isReady = true;
        isPreviousInScreen = !IsUIElementOffScreen(buttonRectTransform, canvas);
    }

    private void Update()
    {
        if (!targetRectTransform) return;
        if (CheckHaveToMove(targetRectTransform, rectTransform, MoveThreshold))
        {
            if(RecordManager.instance.isRemoveFinisihed) LerpPositionTo(targetRectTransform);
            //rectTransform.position =
            // Vector3.Lerp(
            //     rectTransform.position,
            //     targetRectTransform.position,
            //     Time.deltaTime * followingSpeed);
        }
        
        if (recordPlayButton.image.color.a < 1)
        {
            float nextAlpha = recordPlayButton.image.color.a + Time.deltaTime * fadeSpeed;
            SetButtonAlpha(nextAlpha);
            if (leftLine) SetImageAlpha(leftLine.GetComponent<Image>(), nextAlpha);
            if (rightLine) SetImageAlpha(rightLine.GetComponent<Image>(), nextAlpha);
        }

        if (!isReady) return;

        bool isInScreen = !IsUIElementOffScreen(buttonRectTransform, canvas);
        if (isInScreen && !isPreviousInScreen)//in case
        {

            if (IsRight(GetComponent<RectTransform>()))
            {
                int nowRecordUIindex = transform.GetSiblingIndex();
                int nowlastRecordIndex = parentTrans.childCount - 1;
                int rightEnd = nowRecordUIindex + ScrollRecordManager.instance.ReadyDistance;
                StartCoroutine(DelayedButtonCreate(nowlastRecordIndex, rightEnd, false));
            }
            else // left
            {
                int nowRecordUIindex = transform.GetSiblingIndex();
                int leftEnd = nowRecordUIindex - ScrollRecordManager.instance.ReadyDistance;
                StartCoroutine(DelayedButtonCreate(0, leftEnd,true));
            }
            isPreviousInScreen = isInScreen;
        }
        if (!isInScreen && isPreviousInScreen) // out case
        {
            if (IsRight(GetComponent<RectTransform>()))
            {
                int nowRecordUIindex = transform.GetSiblingIndex();
                int nowLastRecordIndex = parentTrans.childCount - 1;
                int rightEnd = nowRecordUIindex + ScrollRecordManager.instance.RemoveDistance;
                StartCoroutine(DelayedButtonRemove(rightEnd,nowLastRecordIndex, false));
            }
            else
            {
                int nowRecordUIindex = transform.GetSiblingIndex();
                int leftEnd = nowRecordUIindex - ScrollRecordManager.instance.RemoveDistance;
                StartCoroutine(DelayedButtonRemove(leftEnd,0,true));
            }
            isPreviousInScreen = isInScreen;
        }

    }

    IEnumerator DelayedButtonCreate(int firstIndex,int lastIndex, bool isFront)
    {

        if (isFront)
        {
            for (int i = firstIndex; i > lastIndex; i--)
            {
                RecordManager.instance.AddPlayerButtonToUI_Front();
                yield return new WaitForSeconds(0.05f);
            }
        }
        else {
            for (int i = firstIndex; i < lastIndex; i++)
            {
                RecordManager.instance.AddPlayerButtonToUI_Back();
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    IEnumerator DelayedButtonRemove(int firstIndex, int lastIndex,bool isFront)
    {
        if (isFront)
        {
            for (int i = firstIndex; i > lastIndex; i--)
            {
                RecordManager.instance.RemovePlayerUI(isFront);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            for (int i = firstIndex; i < lastIndex; i++)
            {
                RecordManager.instance.RemovePlayerUI(isFront);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }


    bool IsRight(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        float sumX = 0.0f;
        for(int i = 0; i < 4; i++) sumX += corners[i].x;
        float middleXofRect = sumX / 4;
        return middleXofRect > Screen.width / 2;
    }



    bool IsUIElementOffScreen(RectTransform rectTransform, Canvas canvas)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Rect expandedRect = new Rect(
            canvasRect.rect.xMin ,
            canvasRect.rect.yMin ,
            canvasRect.rect.width,
            canvasRect.rect.height
        );

        for (int i = 0; i < 4; i++)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[i]), canvas.worldCamera, out Vector2 localPoint))
            {
                return true;
            }
            if (!expandedRect.Contains(localPoint))
            {
                return true;
            }
        }
        return false;
    }

    public void InitializePosition()
    {
        SetPosition(targetRectTransform);
    }

    public void SetPosition(RectTransform targetRect)
    {
        var worldCorners = new Vector3[4];
        targetRect.GetWorldCorners(worldCorners);
        Vector3 center=Vector3.zero;
        for(int i = 0; i < 4; i++)
        {
            center += worldCorners[i];
        }
        center /= 4;
        Vector2 worldCenter = new Vector2(center.x,center.y);
        Vector2 screenPt = RectTransformUtility.WorldToScreenPoint(cam, worldCenter);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), screenPt, cam, out Vector2 localPoint);
        rectTransform.anchoredPosition = localPoint;
    }

    public void LerpPositionTo(RectTransform targetRect)
    {
        var worldCorners = new Vector3[4];
        targetRect.GetWorldCorners(worldCorners);
        Vector3 center = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            center += worldCorners[i];
        }
        center /= 4;
        Vector2 worldCenter = new Vector2(center.x, center.y);
        Vector2 screenPt = RectTransformUtility.WorldToScreenPoint(cam, worldCenter);

        var worldCorners2 = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners2);
        center = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            center += worldCorners2[i];
        }
        center /= 4;
        worldCenter = new Vector2(center.x, center.y);
        Vector2 Dest = Vector2.Lerp(RectTransformUtility.WorldToScreenPoint(cam, worldCenter),screenPt, Time.deltaTime * followingSpeed);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Dest, cam, out Vector2 localPoint);
        rectTransform.anchoredPosition = localPoint;
    }

    public override void ReleaseObject()
    {
        RecordPlayer obj = GetComponentInChildren<RecordPlayer>();
        obj.recordNumber = -1;
        obj.aud.clip = null;
        obj.StopAllCoroutines();
    }
}
