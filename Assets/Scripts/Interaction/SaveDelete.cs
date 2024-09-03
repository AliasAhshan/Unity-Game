using UnityEngine;
using System.IO;

public class SaveDelete : MonoBehaviour
{
    void Start()
    {
        ResetSave();
    }

    void ResetSave()
    {
        string savePath = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file found.");
        }
    }
}
