using UnityEngine;
using System.Collections;

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

    public UIWidget box;

	[HideInInspector]
	public int numToItem;

	// Use this for initialization
	void Start () {
        GetComponent<UIButton>().onClick.Add(new EventDelegate(OpenLot));
        box = GetComponent<UIWidget>();
	}

	// Update is called once per frame
	void Update () {
        if (item != null &&item.texture!=null&& Texture.mainTexture == null)
        {
            Texture.mainTexture= item.texture;
           
        }
	}

    public void SetItem(InventorySlot _item)
    {
        if (_item == null)
        {
            return;
        }
        
        item = _item;
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:SelectItem:" + _item.engName);
        Texture.mainTexture = null;
        if (name != null)
        {
            name.text = _item.name;
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
