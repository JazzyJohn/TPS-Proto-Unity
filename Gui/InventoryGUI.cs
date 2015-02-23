using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InventoryGUI : MonoBehaviour {

	
	public GameClassEnum gameClass = GameClassEnum.ENGINEER;

	
	public MainMenuGUI MainMenu;


	
	public UIPanel Inventory;
	
	public UIWidget lotItem;



	public LotItemGUI lotItemGUI;
	
	public UIScrollBar Scroll;
	


	public RepairGUI repairGui;

	public AskWindow askWindow;

    public UIWidget[] weaponsPanels;

    public UIWidget[] itemPanels;

    public InventoryGroup group;

    public UILabel setLabel;
	
	
	
    Dictionary<string,InvItemGUI> AllItems= new Dictionary<string,InvItemGUI>();

    Dictionary<int, SelectedItemGUI> slots = new Dictionary<int, SelectedItemGUI>();

    SelectedItemGUI[] selected;
	
    public int curSet = 0;

    bool[] allowedReapair = new bool[3];
	// Use this for initialization
    void Awake(){
        ItemManager.instance.SetInventoryGui(this);
    }
	void Start () 
	{
	 
     

      
		//ShopItem.EditCategory(ShopItem.SelectCategory, ShopItem.SelectClass);
		
	}
	
	public void Init(){
		InvItemGUI[] items = GetComponentsInChildren<InvItemGUI>();
		foreach(InvItemGUI itemInfo  in items){
			itemInfo.Shop = this;
			itemInfo.SetItem(ItemManager.instance.GetItem(itemInfo.id));
			AllItems[itemInfo.id] =itemInfo;
		}
		selected = GetComponentsInChildren<SelectedItemGUI>();
		ReloadSelectedItem();
	}
	
	void ReloadSelectedItem(){
		
        foreach (SelectedItemGUI itemInfo in selected)
        {
			itemInfo.Shop = this;
			itemInfo.SetItem();
			slots[itemInfo.slot]= itemInfo;
		}
	}
    public void ChangeSetGUI(UIPopupList list)
    {
        ChangeSet(int.Parse(list.value.Split(' ')[1])-1);
    }
	public void ChangeSet(int i){
		if( PremiumManager.instance. GetSetSize()<=i){
			return;
		}else{
            setLabel.text = TextGenerator.instance.GetMoneyText("SetNumber", i + 1);
			Choice.ChangeSet(i,0);
           
			 ReloadSelectedItem();
		}
	}
    public void SetItemForChoiseSet(InventorySlot slot)
    {
		
		WeaponInventorySlot weapon = (WeaponInventorySlot) slot;
		if(weapon!=null){
			int slotType = (int)weapon.gameSlot;
			Choice.SetChoice(slotType, Choice._Player, new WeaponIndex(weapon.weaponId, ""));
			slots[slotType].SetItem();
		}
	}

    public void HideAllPanel()
    {
        switch (group)
        {
            case InventoryGroup.WEAPON:
                foreach (UIWidget panel in weaponsPanels)
                {
                    panel.alpha = 0f;
                }
                weaponsPanels[curSet].alpha = 1.0f;
             

                break;
            case InventoryGroup.STUFF:
                foreach (UIWidget panel in itemPanels)
                {
                    panel.alpha = 0f;
                }
                itemPanels[curSet].alpha = 1.0f;
              

                break;
        }
    }
		
	public void CloseRepair(){
		repairGui.Close();
	}
	public void ShowSet(int needSet){
        switch (group)
        {
            case InventoryGroup.WEAPON:
                weaponsPanels[curSet].alpha = 0.0f;
                curSet = needSet;
                weaponsPanels[curSet].alpha = 1.0f;
                break;
            case InventoryGroup.STUFF:
                itemPanels[curSet].alpha = 0.0f;
                curSet = needSet;
                itemPanels[curSet].alpha = 1.0f;
                break;
        }
	}
    public void ShowGroup(int newGroup)
    {
        group = (InventoryGroup)newGroup;
        switch (group)
        {
            case InventoryGroup.WEAPON:
                itemPanels[curSet].alpha = 0.0f;
                
                weaponsPanels[curSet].alpha = 1.0f;
                break;
            case InventoryGroup.STUFF:
                weaponsPanels[curSet].alpha = 0.0f;
                
                itemPanels[curSet].alpha = 1.0f;
                break;
        }

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
    public void UpdateLot()
    {
        lotItemGUI.SetItem(lotItemGUI.item);
    }

	public void CloseLot()
	{

        lotItemGUI.item = null;
        lotItem.alpha = 0f;
        Destroy(lotItemGUI.gunModel);
	}

	public void ShowInv()
	{
    
	
      
	}
    void Update()
    {

        
      
	
    }
	void Repair(string id){
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:Repair:" + lotItemGUI.item.engName, 1);
       
		ItemManager.instance.UseRepairKit(lotItemGUI.item.id,id,this);
	
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
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:Disentegrate:" + lotItemGUI.item.engName, 1);
        ItemManager.instance.DesintegrateItem(lotItemGUI.item.id, this);
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
       
    }






    public void TryUpdate(InventorySlot slot)
    {
        if (AllItems.ContainsKey(slot.id))
        {
            AllItems[slot.id].SetItem(slot);
        }
    }
}


public enum InventoryGroup
{
    WEAPON, ARMOR, STUFF
}
