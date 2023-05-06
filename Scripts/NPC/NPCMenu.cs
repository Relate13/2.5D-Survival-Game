using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPCMenu : MonoBehaviour
{
    public Image potrait;
    public PanelGroup panelGroup;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI chatBox;
    public GameObject BuyPanel;
    public GameObject SellPanel;
    public List<GameObject> PrefabList=new List<GameObject>();
    public GameObject BuyItemPrefab;
    public GameObject SellItemPrefab;
    public GameObject MoneyGainPrefab;
    public GameObject ItemSlotPrefab;
    public TextMeshProUGUI TaskName;
    public TextMeshProUGUI TaskConversation;
    public GameObject TaskItemDisplayPanel;
    public GameObject RewardDisplayPanel;
    public TextMeshProUGUI StatisticDiscription;
    public NPCTask CurrentTask;
    public NPCData data;

    public string NoTaskInfo;

    public PanelGroup TaskPanelGroup;

    public NPCController AssociatedNPC;

    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Close();
        }
    }
    public void AssociateNPC(NPCController nPCController)
    {
        AssociatedNPC = nPCController;
    }
    public void SetNPCData(NPCData data) {
        this.data = data;
        npcName.text = data.NPCName;
        potrait.sprite = data.potrait;
        chatBox.text = data.DialogData.dialogs[Random.Range(0, data.DialogData.dialogs.Length)];

        SetBuyPanel(data.BuyData);
        SetSellPanel(data.SellData);
        int i = 0;
        while (i < data.Tasks.Length && data.Tasks[i].finished)
        {
            ++i;
        }
        if (i >= data.Tasks.Length)
        {
            SetNoTask();
        }
        else
        {
            SetTaskPanel(data.Tasks[i]);
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        panelGroup.ShowPanel(0);
        SoundManager.GetInstance().PlaySound(GameDataHolder.getInstance().NPCMenuOpen);
    }
    public void Close()
    {
        SoundManager.GetInstance().PlaySound(GameDataHolder.getInstance().NPCMenuClose);
        GameDataHolder.getInstance().player.unfreezePlayer();
        gameObject.SetActive(false);
        if (AssociatedNPC != null) 
            AssociatedNPC.UnFreeze();
    }
    public void ClearContent()
    {
        foreach (GameObject go in PrefabList)
        {
            Destroy(go);
        }
    }
    public void SetBuyPanel(NPCTradeData tradeData)
    {
        foreach (TradeItem t in tradeData.trades)
        {
            GameObject buyItemButton = Instantiate(BuyItemPrefab, BuyPanel.transform);
            buyItemButton.GetComponent<NPCBuyItem>().SetTrade(t);
            PrefabList.Add(buyItemButton);
        }
    }
    public void SetSellPanel(NPCTradeData tradeData)
    {
        foreach (TradeItem t in tradeData.trades)
        {
            GameObject sellItemButton = Instantiate(SellItemPrefab, SellPanel.transform);
            sellItemButton.GetComponent<NPCSellItem>().SetTrade(t);
            PrefabList.Add(sellItemButton);
        }
    }
    public void SetTaskPanel(NPCTask task)
    {
        CurrentTask = task;
        TaskName.text = task.TaskName;
        TaskConversation.text = task.TaskDescription;
        switch (task.MissionType)
        {
            case TaskType.NEED_ITEM:
                {
                    TaskPanelGroup.ShowPanel(0);
                    foreach (ItemAndQuantity i in task.ItemNeeded)
                    {
                        GameObject obj = Instantiate(ItemSlotPrefab, TaskItemDisplayPanel.transform);
                        PrefabList.Add(obj);
                        ItemIconDisplay ItemSlotDisplay = obj.GetComponent<ItemIconDisplay>();
                        ItemSlotDisplay.SetItemStatus(i.itemNeeded.uiDisplay, i.QuantityNeeded, i.itemNeeded.ID);
                    }
                }
                break;
            case TaskType.STATISTIC:
                {
                    TaskPanelGroup.ShowPanel(2);
                    StatisticDiscription.text = task.StatisticDiscription;
                }
                break;
        }
    }
    public void FinishTask()
    {
        TaskPanelGroup.ShowPanel(1);
        TaskConversation.text = CurrentTask.FinishMessage;
        foreach (ItemAndQuantity i in CurrentTask.Reward)
        {
            GameObject obj = Instantiate(ItemSlotPrefab, RewardDisplayPanel.transform);
            PrefabList.Add(obj);
            ItemIconDisplay ItemSlotDisplay = obj.GetComponent<ItemIconDisplay>();
            ItemSlotDisplay.SetItemStatus(i.itemNeeded.uiDisplay, i.QuantityNeeded, i.itemNeeded.ID);
            GameDataHolder.getInstance().player.playerInventory.AddItem(i.itemNeeded.ID,i.QuantityNeeded);
            
        }
        if (CurrentTask.RewardMoney > 0)
        {
            GameObject mg= Instantiate(MoneyGainPrefab, RewardDisplayPanel.transform);
            mg.GetComponentInChildren<TextMeshProUGUI>().text=CurrentTask.RewardMoney.ToString();
            PrefabList.Add(mg);
            GameDataHolder.getInstance().player.playerInventory.AddMoney(CurrentTask.RewardMoney);
        }
        GameDataHolder.getInstance().AddToStatistic("M"+data.name, 1);
        
        CurrentTask.finished = true;
        
    }
    public void TryFinishTask()
    {
        if (!CurrentTask.finished)
        {
            switch (CurrentTask.MissionType)
            {
                case TaskType.NEED_ITEM:
                    {
                        Player player = GameDataHolder.getInstance().player;
                        bool hasEnoughItem = true;
                        foreach (var inputItem in CurrentTask.ItemNeeded)
                        {
                            if (!player.playerInventory.HasItem(inputItem.itemNeeded.ID, inputItem.QuantityNeeded))
                            {
                                hasEnoughItem = false;
                                break;
                            }
                        }
                        if (hasEnoughItem)
                        {
                            foreach (var inputItem in CurrentTask.ItemNeeded)
                            {
                                player.playerInventory.RemoveItem(inputItem.itemNeeded.ID, inputItem.QuantityNeeded);
                            }
                            MessageSystem.GetInstance().NewTipMessage("任务完成，已领取任务奖励");
                            FinishTask();
                        }
                        else MessageSystem.GetInstance().NewWarningMessage("没有完成任务所需的材料");
                    }
                    break;
                case TaskType.STATISTIC:
                    {
                        if(GameDataHolder.getInstance().Statistic.TryGetValue(CurrentTask.StatisticKey,out int data))
                        {
                            if(data>=CurrentTask.Requirement)
                            {
                                MessageSystem.GetInstance().NewTipMessage("任务完成，已领取任务奖励");
                                FinishTask();
                            }

                        }
                        else MessageSystem.GetInstance().NewWarningMessage("没有达成完成任务所需条件");
                    }
                    break;
            }
        }
    }
    public void SetNoTask() 
    {
        Debug.Log("Set no Task");
        TaskName.text = NoTaskInfo;
        TaskConversation.text = "";
        TaskPanelGroup.HideAll();
    }
}
