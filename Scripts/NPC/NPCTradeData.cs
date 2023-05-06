using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TradeItem
{
    public  ItemObject item;
    public int TradeValue;
}


[System.Serializable]
[CreateAssetMenu(fileName = "New NPC Trade data", menuName = "NPC/NPC Trade Data")]
public class NPCTradeData : ScriptableObject
{
    public TradeItem[] trades;
}
