using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InventoryGUI : MonoBehaviour {

	
	public GameClassEnum gameClass = GameClassEnum.ENGINEER;

	public Transform TableTransform;
	public UITable Table;
	public UIPanel ItemsPanel;
	public UIScrollView ItemsScroll;
	public MainMenuGUI MainMenu;


	
	public UIPanel Inventory;
	
	public UIPanel lotItem;

	public InvItems InvItem;

    public DetailItemGUI detailItemGUI; 
	
	public LotItemGUI lotItemGUI;
	
	public UIScrollBar Scroll;
	
	public UIPanel repair;

	public RepairGUI repairGui;

	public AskWindow askWindow;

	public UIPanel[] allSetPanels
	
	
	
    public Dictionary<string,InvItem> AllItems= new Dictionary<string,InvItem>();
	
    public int curSet = 0;

    bool[] allowedReapair = new bool[3];
	// Use this for initialization
    void Awake(){
        ItemManager.instance.SetInventoryGui(this);
    }
	void Start () 
	{
		InvItem.Main = this;
       
      
        repair.alpha = 0f;

        InvItemGUI ItemInfo = NewItem.Obj.GetComponent<InvItemGUI>();
		ItemInfo.Shop = Main;
        ItemInfo.SetItem(item) ;
		//ShopItem.EditCategory(ShopItem.SelectCategory, ShopItem.SelectClass);
		
	}
	
	public void Init(){
		InvItemGUI[] items = GetComponentsInChildren<InvItemGUI>();
		foreach(InvItemGUI itemInfo  in items){
			itemInfo.Shop = Main;
			ItemInfo.SetItem(ItemManager.instance.GetItem(itemInfo.id));
		}
		
	}

	public void HideAllPanel()
	{
		foreach(UIPanel panel in allSetPanels){
			panel.alpha =0f;
		}
		allSetPanels[curSet].alpha=1.0f;
		repair.alpha = 0f;
	}

	public void ShowSet(int needSet){
		allSetPanels[curSet].alpha=0.0f;
		curSet = needSet;
		allSetPanels[curSet].alpha=1.0f;
	}
	
	public void ShowLot(InvItemGUI item)
	{
		lotItem.alpha = 1f;
      /*  for (int i = 0; i < ShopItem.AllItems.Count; i++)
        {
            if (ShopItem.AllItems[i].ItemInfo.item == LotGUI_item.item)
                LotGUI_item.numToItem = i;
        }*/
        lotItemGUI.SetItem(item.item);
        MainMenu.CamMove.RideTo(1);
		
	
	}

	public void CloseLot()
	{
	
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
			Inventory.alpha= 1.0f;
	
		}
      
	}
    void Update()
    {

        if (Inventory.alpha== 0f)
        {
            CloseLot();
        }
        if(InvItem.AllItems.Count > 0)
			TestSizeScreen();
	
    }
	void Repair(string id){
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:Repair:" + detailItemGUI.item.engName, 1);
       
		ItemManager.instance.UseRepairKit(detailItemGUI.item.id,id,this);
	
	}
	
	public void HideRepair(){
		repair.alpha= 0.0f;
	}
	
	public void DisentegrateAsk(){
		askWindow.text.text =  TextGenerator.instance.GetSimpleText("DisentegrateAsk");
		askWindow.panel.alpha = 1.0f;
		askWindow.action  = Disentegrate;
		askWindow.backPosition =0;
		MainMenu.CamMove.RideTo(2);
	}
	
	
    public void Disentegrate()
    {
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:Disentegrate:" + detailItemGUI.item.engName, 1);
        ItemManager.instance.DesintegrateItem(detailItemGUI.item.id,  this);
    }

 


	public void ReSIZE()
	{
		if(ItemsPanel.alpha == 1f)
			ItemsPanel.alpha = 0f;

        foreach (InvItem obj in InvItem.AllItems)
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

	
	


    public void SetMessage(string text)
    {
        MainMenu.SetMessage(text);
    }
	public void SetMoneyMessage(string cash, string gold){
		MainMenu.SetMoneyMessage(cash,gold);
	}
    public void ReloadCategory()
    {
        EditClass((int)gameClass);
    }

	

	
	
}


public enum InventoryGroup
{
    WEAPON, ARMOR, STUFF
}
