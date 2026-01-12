using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public float distance;
    public float movingSpeed;

    private float initialZ;

    
    // Start is called before the first frame update
    void Start()
    {
        initialZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        float newZ = initialZ + distance * Mathf.Sin(Time.realtimeSinceStartup * movingSpeed);
        transform.position = new Vector3(transform.position.x,transform.position.y,newZ);
    }
}
