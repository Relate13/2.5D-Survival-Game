using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    private static MessageSystem instance;
    public Sprite ErrorSprite;
    public Sprite WarningSprite;
    public Sprite TipSprite;
    public GameObject MessagePanel;
    public GameObject MessagePrefab;
    public AudioClip ErrorSound, WarningSound, TipSound;
    public static MessageSystem GetInstance()
    {
        return instance;
    }
    // Start is called before the first frame update
    MessageSystem()
    {
        instance = this;
    }
    public void NewErrorMessage(string errorMessage)
    {
        MessageController message = Instantiate(MessagePrefab, MessagePanel.transform).GetComponent<MessageController>();
        message.SetText(errorMessage);
        message.SetTitle("¥ÌŒÛ");
        message.SetIcon(ErrorSprite);
        message.SetBackgroundColor(Color.red);
        SoundManager.GetInstance().PlaySound(ErrorSound);
    }
    public void NewWarningMessage(string warningMessage)
    {
        MessageController message = Instantiate(MessagePrefab, MessagePanel.transform).GetComponent<MessageController>();
        message.SetText(warningMessage);
        message.SetTitle("æØ∏Ê");
        message.SetIcon(WarningSprite);
        message.SetBackgroundColor(Color.yellow);
        SoundManager.GetInstance().PlaySound(WarningSound);
    }
    public void NewTipMessage(string TipMessage)
    {
        MessageController message = Instantiate(MessagePrefab, MessagePanel.transform).GetComponent<MessageController>();
        message.SetText(TipMessage);
        message.SetTitle("Ã· æ");
        message.SetIcon(TipSprite);
        message.SetBackgroundColor(Color.green);
        SoundManager.GetInstance().PlaySound(TipSound);
    }
}
