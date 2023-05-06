using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
public enum GameStartMode {CREATE,LOAD }
public class SaveManager : MonoBehaviour
{
    public static readonly string SAVE_PATH = Application.dataPath + "/Saves";
    public static readonly string CHUNK_FOLDER = "/chunk";
    public static readonly string WORLD_INFO_FILE = "/info.wif";
    public static readonly string TILE_RECORDS = "/records.wif";
    public static string CURRENT_WORLD_FOLDER = "/testWorld";
    public static string STATISTIC_FILE = "/statistic.wif";
    public static readonly string PLAYER_FILE = "/player.wif";
    public static GameStartMode CURRENT_GAME_STARTMODE = GameStartMode.CREATE;
    public static int WORLD_SIZE = 16;
    public static int WORLD_SEED = 19194514;
    public static WorldInfo WORLD_INFO;
    public static void SetWorldInfo(string worldName, int size, int seed, GameStartMode mode)
    {
        WORLD_INFO=new WorldInfo(worldName,seed,size);
        CURRENT_WORLD_FOLDER = "/"+worldName;
        WORLD_SIZE = size;
        WORLD_SEED = seed;
        CURRENT_GAME_STARTMODE = mode;
    }
    public static bool CreateSavesFolder()
    {
        Directory.CreateDirectory(SAVE_PATH);
        Debug.Log("Created " + SAVE_PATH);
        return true;
    }
    public static bool CreateWorldFolder()
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            CreateSavesFolder();
        }
        if (!Directory.Exists(SAVE_PATH + CURRENT_WORLD_FOLDER))
        {
            Directory.CreateDirectory(SAVE_PATH + CURRENT_WORLD_FOLDER);
            Debug.Log("Created " + SAVE_PATH + CURRENT_WORLD_FOLDER);
        }
        if (!Directory.Exists(SAVE_PATH + CURRENT_WORLD_FOLDER + CHUNK_FOLDER))
        {
            Directory.CreateDirectory(SAVE_PATH + CURRENT_WORLD_FOLDER + CHUNK_FOLDER);
            Debug.Log("Created " + SAVE_PATH + CURRENT_WORLD_FOLDER + CHUNK_FOLDER);
        }
        File.WriteAllText(SAVE_PATH + CURRENT_WORLD_FOLDER + WORLD_INFO_FILE, JsonUtility.ToJson(WORLD_INFO));
        return true;
    }


    public static bool Save(string SaveName,string SaveFolderName,object SaveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        if (!Directory.Exists(SAVE_PATH + SaveFolderName))
        {
            Directory.CreateDirectory(SAVE_PATH + SaveFolderName);
        }
        string currentSavePath=SAVE_PATH + SaveFolderName+"/"+SaveName;
        FileStream file=File.Create(currentSavePath);
        formatter.Serialize(file, SaveData);
        file.Close();
        return true;
    }
}
