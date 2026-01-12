using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recording : MonoBehaviour
{
    public AudioClip record;
    public int MaxRecordSecond = 200;
    public bool isRecording = false;
    public RecordAlert recordAlert;
    public RecordUISoundPlayer soundPlayer;

    private int nowIndex;
    private string nowPath;
    private float startTime;
    private float endTime;
    private CircleUIVisualizer[] circleUIVisualizers;
    private int microphoneIndex = 1;

    private void Start()
    {
        circleUIVisualizers = GetComponentsInChildren<CircleUIVisualizer>();
    }

    private void Update()
    {

    }
    public void ToggleRecButton()
    {
        if (Microphone.IsRecording(Microphone.devices[microphoneIndex].ToString()))
        {
           
            StopRec();
            soundPlayer.PlaySound(false);
        }
        else
        {
            RecSnd();
            soundPlayer.PlaySound(true);
        }
    }
    public void RecSnd()
    {
        startTime = Time.realtimeSinceStartup;
        record = Microphone.Start(Microphone.devices[microphoneIndex].ToString(), true, MaxRecordSecond, 44100);
        isRecording = true;
        recordAlert.StopAllCoroutines();
        recordAlert.SetVisibility(false);
        SetCircleVisualizerVisibility(true);
        if(RecordPlayManager.instance.lastPlayer != null) RecordPlayManager.instance.lastPlayer.StopSnd();
    }

    private void SetCircleVisualizerVisibility(bool isVisible)
    {
        foreach(var cirVis in circleUIVisualizers)
        {
            cirVis.isOn = isVisible;
        }
    }

    public void StopRec()
    {
        Microphone.End(Microphone.devices[microphoneIndex].ToString());
        endTime = Time.realtimeSinceStartup;
        if(endTime - startTime > 3.0f)
        {
            WriteWavFile();
            RecordManager.instance.AddPlayerButtonToUI_Back(nowPath);
            RecordManager.instance.lastFileIndex++;
        }
        else
        {
            recordAlert.SetVisible();
        }
        
        isRecording = false;
        SetCircleVisualizerVisibility(false);
    }


    public string GetNextFilePath()
    {
        //string path = Application.persistentDataPath;
        //path += '/' + RecordManager.instance.fileNameTemplate + '_';
        //path += nowIndex.ToString() + ".wav";
        nowIndex = RecordManager.instance.GetAllFileCount();
        string path = RecordManager.instance.fileNameTemplate + '_'+ nowIndex.ToString() + ".wav";
        return path;
    }

    public void WriteWavFile()
    {
        nowPath= GetNextFilePath();
        record = SavWav.TrimSilence(record, 0.001f);
        SavWav.Save(nowPath, record);
    }
}
