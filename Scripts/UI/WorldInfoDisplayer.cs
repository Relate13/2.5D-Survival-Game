using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WorldInfo
{
    public string WorldName;
    public int WorldSeed;
    public int WorldSize;
    public WorldInfo(string name, int seed, int size)
    {
        WorldName = name;
        WorldSeed = seed;
        WorldSize = size;
    }
}


public class WorldInfoDisplayer : MonoBehaviour
{
    public TextMeshProUGUI WorldName;
    public TextMeshProUGUI WorldSeed;
    public TextMeshProUGUI WorldSize;
    public WorldInfo WorldInfo;
    public void SetWorldInfo(WorldInfo info)
    {
        WorldInfo = info;
        WorldName.text = info.WorldName;
        WorldSeed.text = info.WorldSeed.ToString();
        WorldSize.text = info.WorldSize.ToString();
    }
    public void LoadWorld()
    {
        SaveManager.SetWorldInfo(WorldInfo.WorldName, WorldInfo.WorldSize, WorldInfo.WorldSeed, GameStartMode.LOAD);
        SceneManager.LoadScene("InGame");
    }
    public void DeleteWorld()
    {
        throw new System.NotImplementedException();
    }
}
