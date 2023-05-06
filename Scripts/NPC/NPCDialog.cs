using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New NPC Dialog", menuName = "NPC/NPC Dialog")]
public class NPCDialog : ScriptableObject
{
    public string[] dialogs;
}
