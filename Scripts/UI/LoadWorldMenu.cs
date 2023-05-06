using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class LoadWorldMenu : MonoBehaviour
{
    public GameObject WorldItemPanel;
    public GameObject WorldItemPrefab;
    public List<GameObject> WorldItems = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        LoadWorlds();
    }
    public void UpdateDisplay()
    {
        foreach (GameObject item in WorldItems)
            Destroy(item);
        LoadWorlds();
    }
    public void LoadWorlds()
    {
        DirectoryInfo savesDir = new DirectoryInfo(SaveManager.SAVE_PATH);
        if (!savesDir.Exists)
        {
            SaveManager.CreateSavesFolder();
        }
        foreach(DirectoryInfo dir in savesDir.GetDirectories())
        {
            //Debug.Log(dir.Name);
            try
            {
                string worldInfoPath = SaveManager.SAVE_PATH + "/" + dir.Name + SaveManager.WORLD_INFO_FILE;
                Debug.Log("loading world info from " + worldInfoPath);
                string worldInfoJsonStr = File.ReadAllText(worldInfoPath);
                WorldInfo info = JsonUtility.FromJson<WorldInfo>(worldInfoJsonStr);
                Debug.Log("Loaded Successfully, seed=" + info.WorldSeed);
                //add world item to panel
                GameObject item = Instantiate(WorldItemPrefab, WorldItemPanel.transform);
                item.GetComponent<WorldInfoDisplayer>().SetWorldInfo(info);
                WorldItems.Add(item);
            }
            catch (System.Exception)
            {
                Debug.Log("Can not load this folder: " + dir.Name);
            }
        }
    }
}
