using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageController : MonoBehaviour
{
    public Image MessageIcon;
    public TextMeshProUGUI MessageTitle;
    public TextMeshProUGUI MessageText;
    public Image Background;
    public CanvasGroup canvasGroup;
    public float FadeSpeed;
    public float MoveSpeed;
    public float DisplayDuration;
    private float TimeCounter;
    private void Start()
    {
        TimeCounter = 0;
    }
    private void Update()
    {
        transform.position += new Vector3(0, MoveSpeed * Time.deltaTime, 0);
        TimeCounter += Time.deltaTime;
        if (TimeCounter > DisplayDuration)
        {
            canvasGroup.alpha -= FadeSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
                Destroy(gameObject);
        }
    }
    public void SetIcon(Sprite icon)
    {
        MessageIcon.sprite = icon;
    }
    public void SetTitle(string text)
    {
        MessageTitle.text = text;
    }
    public void SetText(string text)
    {
        MessageText.text = text;
    }
    public void SetBackgroundColor(Color color)
    {
        Background.color = color;
    }
    public void FadeOut()
    {

    }
}
