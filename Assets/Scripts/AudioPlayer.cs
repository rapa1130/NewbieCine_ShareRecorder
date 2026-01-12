using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordUISoundPlayer : MonoBehaviour
{
    public AudioClip recordStartClip;
    public AudioClip recordEndClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(bool isStart)
    {
        audioSource.clip = (isStart) ? recordStartClip : recordEndClip;
        audioSource.Play();
    }
}
