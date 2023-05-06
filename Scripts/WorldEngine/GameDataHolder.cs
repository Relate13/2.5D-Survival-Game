using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    private static GameDataHolder instance;
    public static GameDataHolder getInstance() { return instance; }
    public GameDataHolder()
    {
        instance = this;
    }
    public ItemDatabase itemDatabase;
    public Player player;
    public DisplayInventory inventoryDisplayer;
    public PanelGroup topMenuDisplayer;
    public CraftingSystem craftingMenu;
    public NPCMenu npcMenu;
    public AudioClip[] CropSound;
    public AudioClip[] RockSound;
    public AudioClip[] TreeSound;
    public AudioClip[] CommonSound;
    public AudioClip[] pickUpSound;
    public AudioClip[] plantSound;
    public AudioClip[] plowSound;
    public AudioClip FurnaceStart;
    public AudioClip FurnaceEnd;
    public AudioClip[] SlimeJump;
    public AudioClip[] SlimeHit;
    public AudioClip[] SlimeDeath;
    public AudioClip[] RobotMove;
    public AudioClip[] RobotHit;
    public AudioClip[] RobotDeath;
    public AudioClip[] NPCFootSteps;
    public AudioClip[] SheepMove;
    public AudioClip[] SheepHit;
    public AudioClip[] SheepDeath;
    public AudioClip[] PlayerHurt;
    public AudioClip[] PlayerDeath;
    public AudioClip[] PlayerEat;
    public AudioClip NPCMenuOpen;
    public AudioClip NPCMenuClose;
    public AudioClip CraftSucceed;
    public NPCData[] NPCDatabase;
    public Dictionary<string, int> Statistic=new Dictionary<string, int>();
    public void AddToStatistic(string key,int value)
    {
        bool succ = Statistic.TryAdd(key, value);
        if (!succ)
        {
            //Debug.LogWarning("New Added:" + EntityID.ToString());
            Statistic[key] += value;
        }
    }
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.STATISTIC_FILE, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Statistic);
        stream.Close();
    }
    public void Load()
    {
        if (File.Exists(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.STATISTIC_FILE))
        {
            //try
            //{
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.STATISTIC_FILE, FileMode.Open, FileAccess.Read);
                Dictionary<string, int> savedStatistic = (Dictionary<string, int>)formatter.Deserialize(stream);
                foreach (KeyValuePair<string, int> kvp in savedStatistic)
                {
                    Statistic.Add(kvp.Key, kvp.Value);
                }
                stream.Close();
            //}
            //catch (System.Exception) { }
        }
        else
        {
            Debug.Log("FILE DOESN'T EXIST");
        }
        InitNPC();
        LoadNPC();
    }
    public void InitNPC()
    {
        foreach (NPCData npcData in NPCDatabase)
        {
            foreach (NPCTask mission in npcData.Tasks)
            {
                mission.finished = false;
            }
        }
    }
    public void LoadNPC()
    {
        foreach (NPCData npcData in NPCDatabase)
        {
            if(Statistic.TryGetValue("M"+npcData.name,out int finishedNumber))
            {
                for(int i = 0; i < finishedNumber; i++)
                {
                    npcData.Tasks[i].finished = true;
                }
            }
        }
    }
}
