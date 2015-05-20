using UnityEngine;
using System.Collections;
using System;

public enum ShopEventType
{
    CAN_TAKE,
    DISCOUNT,
    CAN_TAKE_HOLLIDAY,
    KITDISCOUNT,
}

public class ShopEvent
{
    public ShopEvent(ShopEventType type, string item, string text)
    {
        this.type = type;
        this.item = item;
        this.text = text;
    }

    public ShopEventType type;

    public string item;

    public string text;

    public DateTime end;
}
public class EventNotifyWindow : AskWindow
{
    public SimpleSlot item;

    public Transform gun;

    public GameObject gunModel;

    public UILabel loading;

    public UILabel itemName;

    public UILabel actionLabel;

    public InvItemGroupGUI slot;

    public DateTime end;

    public bool timed;

    public UIWidget timer;

    public UILabel timerLabel;

    public override void Accept()
    {
        base.Accept();
        Destroy(gunModel);
        item = null;
        MainMenu.NotifyClose();
    }
    public override void Decline()
    {
        base.Decline();
        Destroy(gunModel);
        item = null;
        MainMenu.NotifyClose();
    }
    void Update()
    {
        if (item != null && item.loadModel != null && gunModel == null)
        {
            gunModel = Instantiate(item.loadModel, gun.position, gun.rotation) as GameObject;
            gunModel.transform.parent = gun;
            gunModel.transform.rotation = Quaternion.identity;
            gunModel.transform.localScale = Vector3.one;
            gunModel.layer = gun.gameObject.layer;
            loading.alpha = 0.0f;

        }
        if (timed)
        {
            timerLabel.text = IndicatorManager.GetLeftTime(end);
        }
    }

    public bool IsActive()
    {
        return panel.alpha >0.0f;
    }
    public void Show(string text, SimpleSlot item, string action )
    {
//        Debug.Log(text);
        timed = false;
        timer.alpha = 0.0f;
        panel.alpha = 1.0f;
        this.text.text = text;
        this.item = item;
        itemName.text = item.name;
        itemName.alpha = 1.0f;
        actionLabel.text = action;
        ItemManager.instance.LoadModel(item);
        loading.alpha = 1.0f;
        slot.ResetSlot();
    }
    public void Show(string text, InvItemGroupSlot item, string action)
    {
        panel.alpha = 1.0f;
        this.text.text = text;
        loading.alpha = 0.0f;
        itemName.alpha = 0.0f;
        slot.SetSlot(item);
        actionLabel.text = action;
        ShowEnd(item.discountEnd);
        
    }
    public void ShowEnd(DateTime end)
    {
        timed = true;
        this.end = end;
        timer.alpha = 1.0f;
    }
}
