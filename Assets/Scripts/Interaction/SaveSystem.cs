using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector2 playerPosition;
    public int currentHealth;
    // Add other relevant fields
}

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(Vector2 playerPosition, int health)
    {
        GameData data = new GameData();
        data.playerPosition = playerPosition;
        data.currentHealth = health;
        // Save other relevant fields

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
