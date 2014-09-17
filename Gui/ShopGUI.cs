using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShopGUI : MonoBehaviour {
	
	public Transform TableTransform;
	public UITable Table;
	public UIPanel ItemsPanel;
	public MainMenuGUI MainMenu;

	public UIPanel Lot;
	public UIPanel Shop;

	public ShopItems ShopItem;

	public ShopItemGUI LotGUI_item;
	public UIScrollBar Scroll;



	// Use this for initialization
	void Start () 
	{
		ShopItem.Main = this;
		Lot.alpha = 0f;
		ShopItem.EditCategory(ShopItem.SelectCategory, ShopItem.SelectClass);
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

	public void ShowLot()
	{
		Lot.alpha = 1f;
		for(int i=0; i< ShopItem.AllItems.Count; i++)
		{
			if(ShopItem.AllItems[i].id == LotGUI_item.id)
				LotGUI_item.numToItem = i;
		}
		LotGUI_item.LoadInfo();
	}

	public void Move(string value)
	{
		switch(value)
		{
		case "Next":
			if(LotGUI_item.numToItem >= ShopItem.AllItems.Count)
			{
				LotGUI_item.id = ShopItem.AllItems[0].id;
				LotGUI_item.numToItem = 0;
			}
			else
				LotGUI_item.id = ShopItem.AllItems[LotGUI_item.numToItem+1].id;
			break;
		case "Back":
			if(LotGUI_item.numToItem <= 0)
			{
				LotGUI_item.id = ShopItem.AllItems[ShopItem.AllItems.Count-1].id;
				LotGUI_item.numToItem = ShopItem.AllItems.Count-1;
			}
			else
				LotGUI_item.id = ShopItem.AllItems[LotGUI_item.numToItem-1].id;
			break;
		}
	}

	public void CloseLot()
	{
		Lot.alpha = 0f;
	}

	public void ReSIZE()
	{
		foreach(Item obj in ShopItem.AllItems)
		{
			obj.Box.width=(int)Math.Truncate((ItemsPanel.width/Table.columns)-Table.padding.x*(Table.columns+1));
			obj.Box.height=(int)Math.Truncate((ItemsPanel.height/2)-Table.padding.y*3);
		}
		Table.Reposition();
		TableTransform.localPosition = new Vector3((-1*(ItemsPanel.width/2))-Table.padding.x, (ItemsPanel.height/2)+Table.padding.y, 0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
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

	public void EditCategory(string Category)
	{
		ShopItem.EditCategory(Category, "Old");
	}

	public void EditClass(string Class)
	{
		ShopItem.EditCategory("Old", Class);
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
	
	public string SelectCategory = "";
	public string SelectClass = "";

	public List<Item> AllItems;

	public void EditCategory(string Category, string Class) //Смена закладки(категории или класса)
	{
		bool OldCategory = false;
		bool OldClass = false;

		switch(Category) // установка фильтра на категорию
		{
		case "Old":
			Category = SelectCategory;
			break;
		case "Gun":
			break;
		case "Armor":
			break;
		case "Etc":
			break;
		default:
			Category = "";
			break;
		}

		switch(Class) // установка фильтра на класс
		{
		case "Old":
			Class = SelectClass;
			break;
		case "Medic":
			break;
		case "Sniper":
			break;
		case "Enginer":
			break;
		case "Soldat":
			break;
		default:
			Class = "";
			break;
		}

		if (SelectCategory == Category)
			OldCategory = true;
		if (SelectClass == Class)
			OldClass = true;

		SelectCategory = Category;
		SelectClass = Class;

		if (Category == ""|| Class == "" || (OldClass && OldCategory))
			return;

		/*Код загузки и добавления содержимого(цикл парсинга или ещё чего)
		 * {
		 * Add(id);
		 * } 
		 */

		ItemsCount = AllItems.Count;
		if (ItemsCount != 0)
		{
			Page = (int)(Math.Ceiling(Convert.ToDouble(ItemsCount/(Main.Table.columns*2))));
			Main.Scroll.numberOfSteps = Page;
			Main.ReSIZE();
		}
	}

	public void Add(string id) //Добавление товара в панель магазина
	{
		Item NewItem = new Item();

		NewItem.Obj = Transform.Instantiate(ShablonItem) as Transform;
		NewItem.ItemInfo = NewItem.Obj.GetComponent<ShopItemGUI>();
		NewItem.ItemInfo.Shop = Main;
		NewItem.ItemInfo.id = id;
		NewItem.Box = NewItem.ItemInfo.Box;
		NewItem.Obj.localScale = new Vector3(1f, 1f, 1f);
		NewItem.Obj.localEulerAngles = new Vector3(0f, 0f, 0f);
		NewItem.Obj.localPosition = new Vector3(0f, 0f, 0f);

		NewItem.ItemInfo.LoadInfo();
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
	public string id;
	public Transform Obj;
	public UIWidget Box;
	public ShopItemGUI ItemInfo;
}
