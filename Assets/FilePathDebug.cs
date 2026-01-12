using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;

public class FilePathDebug : MonoBehaviour
{
    [ContextMenu("ListPersistentFiles")]
    public void ListPersistentFiles()
    {
        string dir = Application.persistentDataPath;
        Debug.Log("persistentDataPath: " + dir);
        if (Directory.Exists(dir))
        {
            var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            foreach (var f in files) Debug.Log("FILE: " + f);
        }
        else Debug.Log("Persistent dir not found!");
    }
}