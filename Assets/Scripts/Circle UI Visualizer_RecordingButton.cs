using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleUIVisualizer_RecordingButton : CircleUIVisualizer
{
    public Recording recording;
    override protected void Update()
    {
        Update();
        isOn = recording.isRecording;
    }
}
