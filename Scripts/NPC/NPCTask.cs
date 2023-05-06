using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemAndQuantity
{
    public ItemObject itemNeeded;
    public int QuantityNeeded;
}
public enum TaskType
{
    NEED_ITEM,STATISTIC
}
[CreateAssetMenu(fileName = "New NPC Task Data", menuName = "NPC/NPC Task")]
public class NPCTask : ScriptableObject
{
    public bool finished;
    public string TaskDescription;
    public string TaskName;
    public string FinishMessage;
    public TaskType MissionType;
    public ItemAndQuantity[] ItemNeeded;
    public string StatisticKey;
    public string StatisticDiscription;
    public int Requirement;

    public ItemAndQuantity[] Reward;
    public int RewardMoney;
}
