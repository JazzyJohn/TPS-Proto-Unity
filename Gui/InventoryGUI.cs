using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InventoryGUI : MonoBehaviour {
	
	public Transform TableTransform;
	public UITable Table;
	public UIPanel ItemsPanel;
	public UIScrollView ItemsScroll;
	public MainMenuGUI MainMenu;

	public UIPanel Detail;
	public UIPanel Inventory;

	public InvItems InvItem;

    public DetailItemGUI detailItemGUI; 
	public UIScrollBar Scroll;
	
	public UIPanel repair;

	public RepairGUI repairGui;


	// Use this for initialization
	void Start () 
	{
		InvItem.Main = this;
		Lot.alpha = 0f;
    
		//ShopItem.EditCategory(ShopItem.SelectCategory, ShopItem.SelectClass);
		
	}

	public void HideAllPanel()
	{
		Detail.alpha = 0f;
		repair.alpha = 0f;
	}
	public void ShowLot(InvItemGUI item)
	{
		Detail.alpha = 1f;
        detailItemGUI.SetItem(item.item);

		
	
	}
	public void CloseLot()
	{
		Detail.alpha = 0f;
        detailItemGUI.item = null;
        Destroy(detailItemGUI.gunModel);
	}

	public void ShowInv()
	{
    
		if (Inventory.alpha > 0f)
		{
			MainMenu.HideAllPanel();
			MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
		}
		else
		{
			MainMenu.HideAllPanel();
			HideAllPanel();
			Inventory.alpha = 1f;
			EditClass(0);
		}
      
	}
	void Repair(string id){
		StartCoroutine(ItemManager.instance.UseRepairKit(detailItemGUI.item.id,id,this));
	
	}
	public void SmallRepair(){
		Repair(ItemManager.smallRepairId);
	
	}
	public void NormalRepair(){
		Repair(ItemManager.normalRepairId);
	}
	public void MaximumRepair(){
		Repair(ItemManager.maximumRepairId);
	}
	public void HideRepair(){
		repair.alpha= 0.0f;
	}
	public void ShowRepair(){
		repair.alpha= 1.0f;
		int[] cnt = ItemManager.instance.GetAllRepair();
		repairGui.smallCnt.text =cnt[0];
		repairGui.mediumCnt.text =cnt[1];
		repairGui.maxCnt.text =cnt[2];
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
		if(InvItem.AllItems.Count > 0)
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

	
	public void EditClass(int GameClass)
	{
        InvItem.EditCategory((GameClassEnum)GameClass);
	}

    public void OpenList(List<InventorySlot> result)
    {
        InvItem.Clean();
        foreach (InventorySlot slot in result)
        {
            InvItem.Add(slot);
        }
    }
   
}

[Serializable]
public class InvItems
{
	[HideInInspector]
	public InvGUI Main;

	public Transform ShablonItem;

	public int Page;
	public int ItemsCount;

    GameClassEnum gameClass = GameClassEnum.ENGINEER;
	public List<Item> AllItems;
   
    public void EditCategory(GameClassEnum gameClass)
    {
        EditCategory( gameClass);
    }
	public void EditCategory(GameClassEnum gameClass) //Смена закладки(категории или класса)
	{
		if(Main.ItemsPanel.alpha == 1f)
			Main.ItemsPanel.alpha = 0f;

        this.gameClass = gameClass;
      
        Main.StartCoroutine( ItemManager.instance.GenerateList(gameClass),Main);

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
	public void Add(InventorySlot item) //Добавление товара в панель магазина
	{
		InvItemGUI NewItem = new InvItemGUI();

		NewItem.Obj = Transform.Instantiate(ShablonItem) as Transform;
		NewItem.ItemInfo = NewItem.Obj.GetComponent<InvItemGUI>();
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

	
}

[Serializable]
public class InvItem
{
	
	public Transform Obj;
	public UIWidget Box;
	public InvItemGUI ItemInfo;
}
