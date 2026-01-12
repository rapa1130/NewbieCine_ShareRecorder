using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayManager : MonoBehaviour
{
    static public RecordPlayManager instance;
    public RecordPlayer lastPlayer = null;
    

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        lastPlayer = null;
    }

}
