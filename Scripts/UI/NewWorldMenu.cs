using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class NewWorldMenu : MonoBehaviour
{
    public TMP_InputField WorldNameInput;
    public TMP_InputField WorldSizeInput;
    public TMP_InputField WorldSeedInput;
    public static readonly int MIN_MAPSIZE = 16;
    public static readonly int MAX_MAPSIZE = 128;
    public TextMeshProUGUI ErrorMessage;
    public static readonly string BAD_CHAR_SET = "\"*<>?\\|/: ";
    // Start is called before the first frame update
    void Start()
    {
        ErrorMessage.text = "";
    }
    public void CheckInput()
    {
        // first check name
        int result;
        result = AvailableWorldName();
        switch (result)
        {
            case 1: 
                Debug.Log("name should not be empty");
                ShowErrorMessage("世界名称不能为空");
                return;
            case 2:
                ShowErrorMessage("世界名称不能包含非法字符");
                Debug.Log("names should not contain invalid character");
                return;
            case 3:
                ShowErrorMessage("同名世界已存在");
                return;
            default: break;
        }
        // then check size
        result = AvailableSizeInput();
        switch (result)
        {
            case 1:
                Debug.Log("size should not be empty");
                ShowErrorMessage("世界尺寸不能为空");
                return;
            case 2:
                Debug.Log("size should be an integer");
                ShowErrorMessage("世界尺寸应该为一个正整数");
                return;
            case 3:
                Debug.Log("size should be correct");
                ShowErrorMessage("世界尺寸应该处于合法区间内");
                return;
            default: break;
        }
        // then check seed
        result = AvailableSeedInput();
        switch (result)
        {
            case 1:
                Debug.Log("seed should be an integer");
                ShowErrorMessage("世界种子应该为一个整数");
                return;
            default : break;
        }
        Debug.Log("Check Passed");
        CreateWorld();
    }
    public void ShowErrorMessage(string Message)
    {
        ErrorMessage.text = Message;
    }
    public void CreateWorld()
    {
        ShowErrorMessage("");
        string WorldName=WorldNameInput.text;
        int WorldSize = int.Parse(WorldSizeInput.text);
        int WorldSeed;
        if (WorldSeedInput.text == "")
        {
            System.Random rand = new System.Random();
            WorldSeed = rand.Next(int.MaxValue);
        }
        else WorldSeed = int.Parse(WorldSeedInput.text);
        
        Debug.Log("WorldName:" + WorldName);
        Debug.Log("WorldSize:" + WorldSize);
        Debug.Log("WorldSeed:" + WorldSeed);

        SaveManager.SetWorldInfo(WorldName, WorldSize, WorldSeed, GameStartMode.CREATE);
        SceneManager.LoadScene("InGame");
    }
    public int AvailableWorldName()
    {
        // names should not be empty
        if (WorldNameInput.text == "")
            return 1;
        // names should not contain invalid character
        for (int i = 0; i < BAD_CHAR_SET.Length; i++) {
            if (WorldNameInput.text.Contains(BAD_CHAR_SET[i])) {
                return 2;
            }
        }
        // should not duplicate
        if (Directory.Exists(SaveManager.SAVE_PATH+"/"+ WorldNameInput.text))
        {
            return 3;
        }
        return 0;
    }
    public int AvailableSizeInput()
    {
        // size should not be empty
        if(WorldSizeInput.text == "")
            return 1;
        // input should be an integer
        try
        {
            int.Parse(WorldSizeInput.text);
        }
        catch (System.Exception)
        {
            return 2;
        }
        int size = int.Parse(WorldSizeInput.text);
        // input should be of available size
        if (size > MAX_MAPSIZE || size < MIN_MAPSIZE)
            return 3;
        return 0;
    }
    public int AvailableSeedInput()
    {
        if (WorldSeedInput.text == "")
            return 0;
        // input should be an integer
        try
        {
            int.Parse(WorldSeedInput.text);
        }
        catch (System.Exception)
        {
            return 1;
        }
        return 0;
    }
}
