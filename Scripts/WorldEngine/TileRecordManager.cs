using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class TileRecord
{
    public Vector2Int chunkID;
    public Vector2Int inChunkPos;
    public int tileType;
    public TileRecord(Vector2Int chunkID, Vector2Int inChunkPos, int tileType)
    {
        this.chunkID = chunkID;
        this.inChunkPos = inChunkPos;
        this.tileType = tileType;
    }
    public string GenerateSaveString()
    {
        return JsonUtility.ToJson(this);
    }
    public static TileRecord GenerateFromString(string saveString){
        return JsonUtility.FromJson<TileRecord>(saveString);
    }
    public void print()
    {
        Debug.Log("" + chunkID + inChunkPos + tileType);
    }
}


public class TileRecordManager : MonoBehaviour
{
    public List<TileRecord> TileRecords = new List<TileRecord>();
    private static TileRecordManager instance;
    public static TileRecordManager GetInstance()
    {
        return instance;
    }
    TileRecordManager()
    {
        instance = this;
    }
    public void LoadTileRecord()
    {
        TileRecords = new List<TileRecord>();
        try
        {
            string saveURL = SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.TILE_RECORDS;
            string[] data = File.ReadAllLines(saveURL);
            for (int i = 0; i < data.Length; i++)
            {
                TileRecords.Add(TileRecord.GenerateFromString(data[i]));
            }
        }catch (Exception)
        {
            return;
        }
    }
    public void SaveTileRecord()
    {

        string saveURL = SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.TILE_RECORDS;
        string[] data=new string[TileRecords.Count];
        for (int i = 0; i < TileRecords.Count; i++)
        {
            Debug.LogWarning(TileRecords[i].GenerateSaveString());
            data[i] = TileRecords[i].GenerateSaveString();
        }
        File.WriteAllLines(saveURL, data);
    }
    public void AddTileRecord(Vector2Int chunkID, Vector2Int inChunkPos, int tileType)
    {
        TileRecords.Add(new TileRecord(chunkID, inChunkPos, tileType));
    }
    public void RecoverTile()
    {
        for (int i = 0; i < TileRecords.Count; i++)
        {
            TileRecord t= TileRecords[i];
            TileTerrain.GetInstance().RecoverMapTile(t.chunkID, t.inChunkPos, t.tileType);
        }
    }
}
