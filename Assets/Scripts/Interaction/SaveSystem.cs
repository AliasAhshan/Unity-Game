using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector2 playerPosition;
    public Vector2 centipedePosition;    // Centipede position
    public int currentHealth;
    public bool hasSavedAtThisPoint;  // Track if the save point has been used
}

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(Vector2 playerPosition, Vector2 centipedePosition, int health, bool hasSavedAtThisPoint)
    {
        GameData data = new GameData();
        data.playerPosition = playerPosition;
        data.centipedePosition = centipedePosition;  // Save centipede position
        data.currentHealth = health;
        data.hasSavedAtThisPoint = hasSavedAtThisPoint;  // Save the flag

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    public static void ResetSavePointFlag()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            data.hasSavedAtThisPoint = false;  // Reset the flag
            string updatedJson = JsonUtility.ToJson(data);
            File.WriteAllText(savePath, updatedJson);
        }
    }
}
