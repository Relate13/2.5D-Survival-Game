using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public Image maskBar;
    public void setPercentage(float percentage)
    {
        Debug.Log("Fill Amount :"+percentage);
        maskBar.fillAmount = percentage;
    }
}
