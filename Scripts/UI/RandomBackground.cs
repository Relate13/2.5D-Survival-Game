using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBackground : MonoBehaviour
{
    public Image background;
    public Sprite[] backgroundSprites;
    // Start is called before the first frame update
    void Start()
    {
        newRandomBackground();
    }

    public void newRandomBackground()
    {
        background.sprite= backgroundSprites[Random.Range(0,backgroundSprites.Length)];
    }
}
