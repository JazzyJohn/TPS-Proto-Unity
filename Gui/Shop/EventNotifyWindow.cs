using UnityEngine;
using System.Collections;

public enum ShopEventType
{
    CAN_TAKE,
    DISCOUNT,
    CAN_TAKE_HOLLIDAY,
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
}
public class EventNotifyWindow : AskWindow
{
    public SimpleSlot item;

    public Transform gun;

    public GameObject gunModel;

    public UILabel loading;

    public UILabel itemName;

    public UILabel actionLabel;

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
    }

    public void Show(string text, SimpleSlot item, string action )
    {
//        Debug.Log(text);
        panel.alpha = 1.0f;
        this.text.text = text;
        this.item = item;
        itemName.text = item.name;
        actionLabel.text = action;
        ItemManager.instance.LoadModel(item);
    }
}
