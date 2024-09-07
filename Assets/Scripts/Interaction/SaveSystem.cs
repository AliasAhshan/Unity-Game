using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector2 playerPosition;
    public Vector2 centipedePosition;    // Centipede position
    public int currentHealth;
}

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(Vector2 playerPosition, Vector2 centipedePosition, int health)
    {
        GameData data = new GameData();
        data.playerPosition = playerPosition;
        data.centipedePosition = centipedePosition;  // Save centipede position
        data.currentHealth = health;

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
}


