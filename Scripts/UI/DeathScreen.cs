using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    private static DeathScreen instance;
    public static DeathScreen GetInstance()
    {
        return instance;
    }
    DeathScreen()
    {
        instance = this;
    }
    public void DeclareDeath()
    {
        gameObject.SetActive(true);
    }
}
