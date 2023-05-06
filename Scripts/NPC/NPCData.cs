using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New NPC Data", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public string NPCName;
    public Sprite potrait;
    public NPCDialog DialogData;
    public NPCTradeData BuyData;
    public NPCTradeData SellData;
    public NPCTask[] Tasks;
}

