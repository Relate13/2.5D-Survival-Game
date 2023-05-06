using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class DisplayInventory : MonoBehaviour, Observer
{
    private bool toolBarMode = true;
    RectTransform rt;
    public ItemOnMouse itemOnMouse = new ItemOnMouse();
    public GameObject inventoryPrefab;
    public GameObject selectedHighlight;
    public GameObject TopMenu;
    public InventoryObject inventory;
    public float START_X = -36.76324f;
    public float START_Z = -36.76324f;
    public float SPACE_X= 144.5f;
    public TextMeshProUGUI MoneyDisplay;
    public AudioClip OpenMenuSound;
    public AudioClip CloseMenuSound;
    Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot> ();
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
        TopMenu.SetActive(false);
        //itemsDisplayed.Clear();
        //inventory.Clear();
        CreateSlots();
    }
    //void Update()
    //{  
    //    UpdateDisplay();
    //}
    public void CreateSlots()
    {
        itemsDisplayed=new Dictionary<GameObject, InventorySlot> ();
        int size = inventory.Container.items.Length;
        if (toolBarMode)
            size = Inventory.INVENTORY_WIDTH;
        for (int i = 0; i <size; ++i)
        {
            GameObject obj=Instantiate(inventoryPrefab,Vector3.zero,Quaternion.identity,transform);
            obj.GetComponent<RectTransform>().localPosition = CalPosition(i);
            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            itemsDisplayed.Add(obj, inventory.Container.items[i]);
        }
        selectedHighlight.GetComponent<RectTransform>().localPosition = CalPosition(inventory.SelectedItem);
        selectedHighlight.transform.SetAsLastSibling();

    }
    public void ClearSlots()
    {
        foreach (GameObject obj in itemsDisplayed.Keys)
        {
            Destroy(obj);
        }
        itemsDisplayed.Clear();
    }

    public Vector3 CalPosition(int i)
    {
        int colums = i / Inventory.INVENTORY_WIDTH;
        int row = i % Inventory.INVENTORY_WIDTH;
        //Debug.Log("Slot "+i+" : "+new Vector3(START_X + (SPACE_X * row), -colums * SPACE_X + START_Z, 0));
        return new Vector3(START_X + (SPACE_X * row), -colums * SPACE_X + START_Z, 0);
    }

    public void SwitchToolBarMode()
    {
        SwitchToolBarMode(!toolBarMode);
        
    }
    public void SwitchToolBarMode(bool On)
    {
        toolBarMode = On;
        if (On)
        {
            TopMenu.SetActive(false);
            START_Z = 0;
            Vector2 vector2 = rt.offsetMax;
            vector2.y = -870;
            rt.offsetMax = vector2;
            GameDataHolder.getInstance().craftingMenu.ClearOverrideDatabase();//exit any craftdatabase overrides when entering toolbar mode
            GameDataHolder.getInstance().player.unfreezePlayer();//unfreeze player movements
            ItemTipSystem.getInstance().Hide();
            SoundManager.GetInstance().PlaySound(CloseMenuSound);
        }
        else
        {
            TopMenu.SetActive(true);
            START_Z = 145;
            Vector2 vector2 = rt.offsetMax;
            vector2.y = -581;
            rt.offsetMax = vector2;
            ItemTipSystem.getInstance().Hide();
            GameDataHolder.getInstance().player.freezePlayer();//freeze player movements
            SoundManager.GetInstance().PlaySound(OpenMenuSound);
        }
        ClearSlots();
        CreateSlots();
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        foreach (KeyValuePair<GameObject,InventorySlot>slot in itemsDisplayed)
        {
            ItemIconDisplay slotIcon=slot.Key.GetComponent<ItemIconDisplay>();
            if (slot.Value.ID >= 0)
            {
                var image = inventory.database.GetItem[slot.Value/*.item*/.ID].uiDisplay;
                int number = slot.Value.amount;
                slotIcon.SetItemStatus(image, number,slot.Value.ID);
                //slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.Value.item.ID].uiDisplay;
                //slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                //slot.Key.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.amount == 1 ? "" : slot.Value.amount.ToString("n0");
            }
            else
            {
                slotIcon.SetEmpty();
                //slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                //slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                //slot.Key.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
        selectedHighlight.GetComponent<RectTransform>().localPosition = CalPosition(inventory.SelectedItem);
        selectedHighlight.transform.SetAsLastSibling();
        MoneyDisplay.text = inventory.Container.money.ToString("n0");
    }
    private void AddEvent(GameObject button, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
    public void OnEnter(GameObject obj)
    {
        //Debug.Log("Entered Slot "+obj);
        itemOnMouse.hoverObject = obj;
        if (itemsDisplayed.ContainsKey(obj))
        {
            itemOnMouse.hoverItem = itemsDisplayed[obj];
        }
    }
    private void OnExit(GameObject obj)
    {
        //Debug.Log("Exited SLot" + obj);
        itemOnMouse.ClearItemHovering();
    }
    private void OnDragStart(GameObject obj)
    {
        GameObject objectOnMouse = new GameObject();
        RectTransform rect=objectOnMouse.AddComponent<RectTransform>();
        rect.sizeDelta = inventoryPrefab.transform.GetChild(0).GetComponentInChildren<RectTransform>().sizeDelta;
        objectOnMouse.transform.SetParent(transform.parent);
        if(itemsDisplayed[obj].ID >= 0)
        {
            Image image=objectOnMouse.AddComponent<Image>();
            image.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay;
            image.raycastTarget = false;
        }
        itemOnMouse.SetItemOnMouse(itemsDisplayed[obj], objectOnMouse);
    }
    private void OnDragEnd(GameObject obj)
    {
        if (itemOnMouse.hoverObject)
        {
            inventory.SwapItem(itemsDisplayed[obj], itemsDisplayed[itemOnMouse.hoverObject]);
        }
        else
        {
            InventorySlot slot = itemsDisplayed[obj];
            if (slot.ID >= 0)
            {
                Vector3 ItemPlace = GameDataHolder.getInstance().player.transform.position;
                Vector2 bias = UnityEngine.Random.insideUnitCircle;
                ItemPlace.x += bias.x;
                ItemPlace.z += bias.y;
                Instantiate(GameDataHolder.getInstance().player.playerInventory.database.GetItem[slot.ID].ItemEntity, ItemPlace, Quaternion.identity);
                inventory.RemoveItem(slot.ID, 1);
            }
        }
        Destroy(itemOnMouse.obj);
        itemOnMouse.ClearItemOnMouse();
    }
    private void OnDrag(GameObject obj)
    {
        if(itemOnMouse.obj != null)
        {
            itemOnMouse.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
    //private void Update()
    //{
    //    UpdateDisplay();
    //}
    public void ObserverUpdate()
    {
        UpdateDisplay();
    }
}
public class ItemOnMouse
{
    public InventorySlot item;
    public GameObject obj;
    public InventorySlot hoverItem;
    public GameObject hoverObject;
    public void SetItemOnMouse(InventorySlot item,GameObject obj)
    {
        this.item = item;
        this.obj = obj;
    }
    public void ClearItemOnMouse()
    {
        item = null;
        obj = null;
    }
    public void ClearItemHovering()
    {
        hoverItem = null;
        hoverObject = null;
    }
}
