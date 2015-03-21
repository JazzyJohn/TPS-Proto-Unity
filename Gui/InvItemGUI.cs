using UnityEngine;
using System.Collections;
using System;

public class InvItemGUI : MonoBehaviour {

	public InventoryGUI Shop;
    public InventorySlot item;

	public UITexture Texture;

    public Color open =Color.white;

    public Color close =Color.gray;

    public Color goldOpen = Color.yellow;

    public Color goldClose = Color.yellow;
    public UILabel name;

    public string id;

    public string[] ids;

    public InventorySlot[] items;

    public UIWidget box;

	[HideInInspector]
	public int numToItem;

    public Color[] repairColors;

    public UIWidget repairFon;

    public UISprite repair;

    public UIWidget timerFon;

    public UILabel timerLabel;

	// Use this for initialization
	void Start () {
        GetComponent<UIButton>().onClick.Add(new EventDelegate(OpenLot));
        box = GetComponent<UIWidget>();
        if (Texture == null)
        {
            Texture = GetComponentInChildren<UITexture>();
        }
        items = new InventorySlot[ids.Length];
	}

	// Update is called once per frame
	void Update () {
        if (item != null &&item.texture!=null&& Texture.mainTexture == null)
        {
            Texture.mainTexture= item.texture;
           
        }
        if (item != null && item.buyMode == BuyMode.FOR_GOLD_TIME && item.isAvailable())
        {
            TimeSpan span = new TimeSpan(item.timeEnd.Ticks - DateTime.UtcNow.Ticks);
            timerLabel.text = string.Format("{0:D3} : {1:D2} : {2:D2}", span.Hours + span.Days * 24, span.Minutes, span.Seconds);
        }
	}
    public void SetItem(InventorySlot _item, int set)
    {
        items[set] = _item;
    }

    public void UpdateItem(InventorySlot _item)
    {
        if (_item == item)
        {
            SetItem(_item);

        }
    }

    public void SetSet(int set){
        if(set<items.Length){
            SetItem(items[set]);
        }else{
            box.alpha = 0.0f;
        }
       
    }
    private void SetItem(InventorySlot _item)
    {
        if (_item == null)
        {
            box.alpha = 0.0f;
            return;
        }
        
        item = _item;
    
        Texture.mainTexture = null;
        if (name != null)
        {
            name.text = _item.name;
        }
        if (item.buyMode == BuyMode.FOR_KP)
        {
            int percent =  item.GetChargePercent();
            if (percent > 70)
            {
                repairFon.alpha = 0.0f;
            }
            else if (percent > 50)
            {
                repairFon.alpha = 1.0f;
                repair.color = repairColors[0];
            }
            else if (percent > 30)
            {
                repairFon.alpha = 1.0f;
                repair.color = repairColors[1];
            }
            else 
            {
                repairFon.alpha = 1.0f;
                repair.color = repairColors[2];
            }

        }else{
            repairFon.alpha = 0.0f;

        }
        if (item.buyMode == BuyMode.FOR_GOLD_TIME && item.isAvailable())
        {
            timerFon.alpha = 1.0f;
           
        }
        else
        {
            timerFon.alpha = 0.0f;
        }
        if (item.isAvailable())
        {
            if (item.prices[0].type == BuyPrice.KP_PRICE)
            {
                GetComponent<UIButton>().defaultColor = open;
                box.color = open;
           
            }
            else
            {
                GetComponent<UIButton>().defaultColor = goldOpen;
                box.color = goldOpen;
            }
        }
        else
        {
            if (item.prices[0].type == BuyPrice.KP_PRICE)
            {
                GetComponent<UIButton>().defaultColor = close;
                box.color = close;
            }
            else
            {
                GetComponent<UIButton>().defaultColor = goldClose;
                box.color = goldClose;
            }
        }
        
    }
    public void OpenLot()
    {
        Shop.ShowLot(this);
    }
        
}
