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

    public UIWidget repairBarFon;

    public UISprite repairBar;

    public UIWidget timerFon;

    public UILabel timerLabel;

    public UIWidget discount;

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
          
            timerLabel.text =  IndicatorManager.GetLeftTime(item.timeEnd);
        }
        if (item != null&&item.prices[0].discount)
        {
            if (discount.alpha< 1.0f)
            {
                discount.alpha = 1.0f;

            }
            item.prices[0].CheckDiscount();
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
            repairBarFon.alpha = 1.0f;
            int percent =  item.GetChargePercent();
            repairBar.fillAmount = (float)Mathf.Max(percent,10) / 100.0f;
            if (percent > 70)
            {
                repairFon.alpha = 0.0f;
                repairBar.color = repairColors[0];
            }
            else if (percent > 50)
            {
                repairFon.alpha = 1.0f;
                repair.color = repairColors[0];
                repairBar.color = repairColors[0];
            }
            else if (percent > 30)
            {
                repairFon.alpha = 1.0f;
                repair.color = repairColors[1];
                repairBar.color = repairColors[1];
            }
            else 
            {
                repairFon.alpha = 1.0f;
                repair.color = repairColors[2];
                repairBar.color = repairColors[2];
            }

        }else{
            repairFon.alpha = 0.0f;
            repairBarFon.alpha = 0.0f;
        }
        if (item.buyMode == BuyMode.FOR_GOLD_TIME && item.isAvailable())
        {
            timerFon.alpha = 1.0f;
           
        }
        else
        {
            timerFon.alpha = 0.0f;
        }
        if (!item.prices[0].discount)
        {
            discount.alpha = 0.0f;
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
