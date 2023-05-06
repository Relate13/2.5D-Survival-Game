using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;


public interface Observer
{
    public void ObserverUpdate();
}
public class ObservingSubject
{
    public List<Observer> observers=new List<Observer>();
    public void AttachObserver(Observer o) {
        observers.Add(o);
    }
    public void DettachObserver(Observer o) { 
        observers.Remove(o);
    }
    public void Notify()
    {
        foreach (Observer o in observers)
            o.ObserverUpdate();
    }
}



[CreateAssetMenu(fileName ="Inventory",menuName ="Inventory/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public ObservingSubject ObserveSystem = new ObservingSubject();
    public ItemDatabase database;
    public string savePath;
    public Inventory Container;
    public AudioClip ScrollSound;
    public int SelectedItem = 0;
    public ItemObject[] InitialItems;
    //public int Money = 0;
    public void Initialize()
    {
        ObserveSystem=new ObservingSubject();
        ObserveSystem.AttachObserver(GameDataHolder.getInstance().inventoryDisplayer);
        ObserveSystem.AttachObserver(GameDataHolder.getInstance().craftingMenu);
        //switch (SaveManager.CURRENT_GAME_STARTMODE)
        //{
        //    case GameStartMode.CREATE:
        //        {
        //            //Clear();
        //            Save();
        //            break;
        //        }
        //    case GameStartMode.LOAD:
        //        {
        //            Load();
        //            break;
        //        }
        //}
    }
    public void AddInitialItems()
    {
        foreach(ItemObject item in InitialItems)
        {
            AddItem(item.ID, 1);
        }
    }
    public void AddMoney(int amount)
    {
        Container.money += amount;
        ObserveSystem.Notify();
    }
    public bool TryBuy(int price)
    {
        if (Container.money >= price)
        {
            Container.money -= price;
            return true;
        }
        else return false;
    }
    public bool AddItem(int itemID,int amount)
    {
        foreach (InventorySlot slot in Container.items)
        {
            if (slot/*.item*/.ID == itemID)
            {
                slot.AddAmount(amount);
                ObserveSystem.Notify();
                return true;
            }
        }
        if (SetFirstEmptySlot(itemID, amount) != null)
        {
            ObserveSystem.Notify();
            return true;
        }
        return false;
        
    }
    public bool HasItem(int itemID,int amount)
    {
        int total = 0;
        for (int i = 0; i < Container.items.Length; i++)
        {
            if (Container.items[i].ID == itemID)
                total+=Container.items[i].amount;
        }
        return total >= amount;
    }
    public bool RemoveItem(int itemID,int amount)
    {
        if (HasItem(itemID, amount))
        {
            int total = amount;
            for (int i = 0; i < Container.items.Length && total > 0; i++) 
            {
                if (Container.items[i].ID == itemID)
                {
                    if (total >= Container.items[i].amount)
                    {
                        total -= Container.items[i].amount;
                        Container.items[i].SetEmpty();
                    }
                    else
                    {
                        Container.items[i].amount-=total;
                    }
                }
            }
            ObserveSystem.Notify();
            return true;
        }
        else return false;
    }
    public void SelectNext()
    {
        SelectedItem += 1;
        SelectedItem %= Inventory.INVENTORY_WIDTH;
        ObserveSystem.Notify();
        SoundManager.GetInstance().PlaySound(ScrollSound);
    }
    public void SelectPrev()
    {
        SelectedItem -= 1;
        SelectedItem = (SelectedItem + Inventory.INVENTORY_WIDTH) % Inventory.INVENTORY_WIDTH;
        ObserveSystem.Notify();
        SoundManager.GetInstance().PlaySound(ScrollSound);
    }
    public int GetSelected()
    {
        if(Container==null)
            Debug.LogWarning("Container is NULL!!!");
        return Container.items[SelectedItem].ID;
    }
    public InventorySlot SetFirstEmptySlot(int itemID,int amount)
    {
        for(int i=0;i<Container.items.Length;i++)
        {
            if(Container.items[i].ID < 0)
            {
                Container.items[i].UpdateSlot(itemID/*, item*/, amount);
                return Container.items[i];
            }
        }
        return null;
    }

    public void SwapItem(InventorySlot item1,InventorySlot item2)
    {
        InventorySlot itemTemp = new InventorySlot(item1.ID, /*item1.item,*/ item1.amount);
        item1.UpdateSlot(item2.ID,/* item2.item,*/ item2.amount);
        item2.UpdateSlot(itemTemp.ID,/*itemTemp.item,*/itemTemp.amount);
        ObserveSystem.Notify();
    }

    [ContextMenu("Save")]
    public void Save()
    {
        /*string saveData=JsonUtility.ToJson(this,true);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        binaryFormatter.Serialize(file,saveData);
        file.Close();*/
        IFormatter formatter=new BinaryFormatter();
        Stream stream = new FileStream(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + savePath, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + savePath))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + savePath, FileMode.Open, FileAccess.Read);
            Inventory newContainer=(Inventory)formatter.Deserialize(stream);
            
            if (Container.items == null)
                Debug.LogWarning("Container is null!");
            for(int i = 0; i < Container.items.Length; i++)
            {
                Container.items[i].UpdateSlot(newContainer.items[i].ID/*, newContainer.items[i].item*/, newContainer.items[i].amount);
            }
            Container.money = newContainer.money;
            stream.Close();
        }
        else
        {
            Debug.Log("ERROR, FILE DOESN'T EXIST");
        }
        ObserveSystem.Notify();
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        foreach(var item in Container.items)
        {
            item.ID = -1;
            item.amount = 0;
        }
        Container.money = 0;
    }
}
[System.Serializable]
public class Inventory
{
    public static readonly int INVENTORY_SIZE = 30;
    public static readonly int INVENTORY_WIDTH = 10;
    public int money;
    public InventorySlot[] items = new InventorySlot[INVENTORY_SIZE];
    public Inventory()
    {
        items = new InventorySlot[INVENTORY_SIZE];
    }
}

[System.Serializable]
public class InventorySlot
{
    //public Item item;
    public int amount;
    public int ID;
    public InventorySlot()
    {
        ID = -1;
        //item = new Item();
        amount = 0;
    }
    public InventorySlot(int ID,/*Item item,*/int amount)
    {
        this.ID = ID;
        //this.item = item;
        this.amount = amount;
    }
    public void SetEmpty()
    {
        ID = -1;
        //item = new Item();
        amount = 0;
    }
    public void UpdateSlot(int ID, /*Item item,*/ int amount)
    {
        this.ID = ID;
        //this.item = item;
        this.amount = amount;
    }
    public void AddAmount(int amount)
    {
        this.amount += amount;
    }
}
