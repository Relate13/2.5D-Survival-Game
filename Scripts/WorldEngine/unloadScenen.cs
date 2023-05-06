using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class unloadScenen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Trying Unload InGame");
        SceneManager.UnloadSceneAsync("InGame");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
