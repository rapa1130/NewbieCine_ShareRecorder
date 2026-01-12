using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Pool;

public class RecordPlayer : MonoBehaviour
{
    public string path { get; set; }
    public AudioClip clip
    {
        get;
        set;
    }
    public AudioSource aud
    {
        get;
        set;
    }
    public int recordNumber = -1;

    public GameObject starlineLeft = null;
    public GameObject starlineRight = null;
    public GameObject connectedLogicalElement = null;
    private CircleUIVisualizer[] circleUIVisualizers;
    private IEnumerator delayedSetVisibilityOfCircleUI = null;

    private void Awake()
    {
        aud = gameObject.AddComponent<AudioSource>();
        aud.playOnAwake = false;
    }
    private void Start()
    {
        circleUIVisualizers = GetComponentsInChildren<CircleUIVisualizer>();
    }

    public void PlaySnd()
    {
        if (RecordPlayManager.instance.lastPlayer   != null)
        {
            RecordPlayer prevPlayer = RecordPlayManager.instance.lastPlayer;
            if (prevPlayer.aud.isPlaying)
            {
                prevPlayer.aud.Stop();
                if(prevPlayer.delayedSetVisibilityOfCircleUI != null)
                {
                    prevPlayer.SetVisibilityOfCircleUI(false);
                    prevPlayer.StopCoroutine(prevPlayer.delayedSetVisibilityOfCircleUI);
                }
            }
        }
        RecordPlayManager.instance.lastPlayer = GetComponent<RecordPlayer>();
        aud.Play();
        SetVisibilityOfCircleUI(true);
        delayedSetVisibilityOfCircleUI = DelayedSetVisibilitOfCircleUI(aud.clip.length);
        StartCoroutine(delayedSetVisibilityOfCircleUI);
    }
    public void StopSnd()
    {
        if (aud.isPlaying)
        {
            aud.Stop();
        }
        SetVisibilityOfCircleUI(false);
        StopCoroutine(delayedSetVisibilityOfCircleUI);
    }
    

    IEnumerator DelayedSetVisibilitOfCircleUI(float time)
    {
        yield return new WaitForSeconds(time);
        SetVisibilityOfCircleUI(false);
    }
    private void SetVisibilityOfCircleUI(bool isVisible)
    {
        foreach(var circleUI in circleUIVisualizers)
        {
            circleUI.isOn = isVisible;
        }
    }

    
}
