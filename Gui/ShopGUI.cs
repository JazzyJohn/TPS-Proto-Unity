using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShopGUI : MonoBehaviour {
	
	public Transform TableTransform;
	public UITable Table;
	public UIPanel ItemsPanel;
	public UIScrollView ItemsScroll;
	public MainMenuGUI MainMenu;

	public UIPanel Lot;
	public UIPanel Shop;

	public ShopItems ShopItem;

    public LotItemGUI LotGUI_item;
	public UIScrollBar Scroll;



	// Use this for initialization
	void Start () 
	{
		ShopItem.Main = this;
		Lot.alpha = 0f;
        StartCoroutine(ItemManager.instance.LoadShop(this));
		//ShopItem.EditCategory(ShopItem.SelectCategory, ShopItem.SelectClass);
		ShopItem.EditCategory(0, 0);
	}

	public void HideAllPanel()
	{
		Lot.alpha = 0f;
	}

	public void ShowShop()
	{
    
		if (Shop.alpha > 0f)
		{
			MainMenu.HideAllPanel();
			MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
		}
		else
		{
			MainMenu.HideAllPanel();
			HideAllPanel();
			Shop.alpha = 1f;
		}
      
	}

	public void ShowLot(ShopItemGUI item)
	{
		Lot.alpha = 1f;
        for (int i = 0; i < ShopItem.AllItems.Count; i++)
        {
            if (ShopItem.AllItems[i].ItemInfo.item == LotGUI_item.item)
                LotGUI_item.numToItem = i;
        }
        LotGUI_item.SetItem(item.item);
        MainMenu.CamMove.RideTo(1);
		
	
	}

	public void Move(string value)
	{
		switch(value)
		{
		case "Next":
                LotGUI_item.numToItem++;
			if(LotGUI_item.numToItem >= ShopItem.AllItems.Count)
			{
               
				LotGUI_item.numToItem = 0;
			}
		
             LotGUI_item.SetItem(ShopItem.AllItems[LotGUI_item.numToItem].ItemInfo.item);
			break;
		case "Back":
            LotGUI_item.numToItem--;
			if(LotGUI_item.numToItem < 0)
			{
                 
				LotGUI_item.numToItem = ShopItem.AllItems.Count-1;
			}
			LotGUI_item.SetItem(ShopItem.AllItems[LotGUI_item.numToItem].ItemInfo.item);
			break;
		}
	}

	public void CloseLot()
	{
		Lot.alpha = 0f;
        LotGUI_item.item = null;
        Destroy(LotGUI_item.gunModel);
        MainMenu.CamMove.RideTo(0);
	}

	public void ReSIZE()
	{
		if(ItemsPanel.alpha == 1f)
			ItemsPanel.alpha = 0f;

		foreach(Item obj in ShopItem.AllItems)
		{
			obj.Box.width=(int)Math.Truncate((ItemsPanel.width/Table.columns)-Table.padding.x*(Table.columns+1));
			obj.Box.height=(int)Math.Truncate((ItemsPanel.height/2)-Table.padding.y*3);
		}
		StartCoroutine(Reposition());
	}

	IEnumerator Reposition()
	{
		yield return new WaitForEndOfFrame();
		Table.Reposition();
		ItemsScroll.ResetPosition();
		TableTransform.localPosition = new Vector3((-1*(ItemsPanel.width/2))+1, (ItemsPanel.height/2)+Table.padding.y, 0f);
		ItemsPanel.alpha = 1f;
	}

	bool ReSizeRedy = false;

	// Update is called once per frame
	void Update () 
	{
		if(ShopItem.AllItems.Count > 0)
			TestSizeScreen();
	}


	int xScreen=0;
	int yScreen=0;
	void TestSizeScreen()
	{
		if (xScreen != Screen.currentResolution.width && yScreen != Screen.currentResolution.height)
		{
			xScreen=Screen.currentResolution.width;
			yScreen=Screen.currentResolution.height;
			ReSIZE();
		}
	}

	public void EditCategory(int Category)
	{
		ShopItem.EditCategory((ShopSlotType) Category);
	}

	public void EditClass(int GameClass)
	{
        ShopItem.EditCategory((GameClassEnum)GameClass);
	}

    public void ShowCategory(int Category)
    {
        ShowShop();
        ShopItem.EditCategory((ShopSlotType)Category);
    }
    public void OpenList(List<ShopSlot> result)
    {
        ShopItem.Clean();
        foreach (ShopSlot slot in result)
        {
            ShopItem.Add(slot);
        }
    }
   
}

[Serializable]
public class ShopItems
{
	[HideInInspector]
	public ShopGUI Main;

	public Transform ShablonItem;

	public int Page;
	public int ItemsCount;

    ShopSlotType category = ShopSlotType.WEAPON;
    GameClassEnum gameClass = GameClassEnum.ENGINEER;
	public List<Item> AllItems;
    public void EditCategory(ShopSlotType category)
    {
        EditCategory(category,gameClass);
    }
    public void EditCategory(GameClassEnum gameClass)
    {
        EditCategory(category, gameClass);
    }
	public void EditCategory(ShopSlotType category, GameClassEnum gameClass) //Смена закладки(категории или класса)
	{
		if(Main.ItemsPanel.alpha == 1f)
			Main.ItemsPanel.alpha = 0f;

        this.gameClass = gameClass;
        this.category = category;
        Main.StartCoroutine( ItemManager.instance.GenerateList(gameClass, category));

		/*if (SelectCategory == Category)
			OldCategory = true;
		if (SelectClass == Class)
			OldClass = true;

		SelectCategory = Category;
		SelectClass = Class;

		if (Category == ""|| Class == "" || (OldClass && OldCategory))
			return;

		*/

		Page = (int)(Math.Ceiling(Convert.ToDouble(Main.TableTransform.childCount/(Main.Table.columns*2))));
		Main.Scroll.numberOfSteps = Page;
		Main.ReSIZE();
}

    public void Clean()
    {
        foreach(Item item in AllItems){
            GameObject.Destroy(item.Obj.gameObject);
        }
        AllItems.Clear();
    }
	public void Add(ShopSlot item) //Добавление товара в панель магазина
	{
		Item NewItem = new Item();

		NewItem.Obj = Transform.Instantiate(ShablonItem) as Transform;
		NewItem.ItemInfo = NewItem.Obj.GetComponent<ShopItemGUI>();
		NewItem.ItemInfo.Shop = Main;
        NewItem.ItemInfo.SetItem(item) ;
        NewItem.Obj.parent = Main.TableTransform;
		NewItem.Box = NewItem.ItemInfo.Box;
		NewItem.Obj.localScale = new Vector3(1f, 1f, 1f);
		NewItem.Obj.localEulerAngles = new Vector3(0f, 0f, 0f);
		NewItem.Obj.localPosition = new Vector3(0f, 0f, 0f);

	
		NewItem.Box.alpha = 1f;

		AllItems.Add(NewItem);
	}

	public void Buy(string valute)//покупка предмета
	{
		switch(valute)
		{
		case "KP":

			break;
		case "GITP":

			break;
		default:
			return;

			break;
		}

		//Код
	}
}

[Serializable]
public class Item
{
	
	public Transform Obj;
	public UIWidget Box;
	public ShopItemGUI ItemInfo;
}
