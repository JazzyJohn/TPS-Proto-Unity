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

  //  public UILabel setLabel;

    public UIRect canBuy;

    public UILabel canBuyText;

    public UIRect mustOpen;
	
    public UILabel  mustOpenText;
	
    Dictionary<string,InvItemGUI> AllItems= new Dictionary<string,InvItemGUI>();

    Dictionary<int, SelectedItemGUI> slots = new Dictionary<int, SelectedItemGUI>();

    SelectedItemGUI[] selected;
	
    public int curSet = 0;

    bool[] allowedReapair = new bool[3];

    public bool init = false;
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
        init = true;
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
        if (!init)
        {
            return;
        }
		if( PremiumManager.instance. GetSetSize()<=i){
			return;
		}else{
            //setLabel.text = TextGenerator.instance.GetMoneyText("SetNumber", i + 1);
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
            ItemManager.instance.SaveItemForSlot();
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
                if (weaponsPanels.Length <= needSet) {
                    return;
                }
                weaponsPanels[curSet].alpha = 0.0f;
                curSet = needSet;
                weaponsPanels[curSet].alpha = 1.0f;
                break;
            case InventoryGroup.STUFF:
                if (itemPanels.Length <= curSet)
                {
                    return;
                }
                itemPanels[curSet].alpha = 0.0f;
                curSet = needSet;
                itemPanels[curSet].alpha = 1.0f;
                break;
        }
        mustOpen.alpha = 0.0f;
        canBuy.alpha = 0.0f;
        if (curSet+1 > GlobalPlayer.instance.open_set)
        {
            if (curSet+1  > GlobalPlayer.instance.open_set+1)
            {
                mustOpenText.text = TextGenerator.instance.GetMoneyText("openSet", curSet * 10);
                mustOpen.alpha = 1.0f;
            }
            else
            {
                canBuyText.text = TextGenerator.instance.GetMoneyText("openSet", curSet * 10);
                canBuy.alpha = 1.0f;
            }

        }
	}
    public void ResetSet()
    {
        ShowSet(curSet);
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
    public void BuyNextSet()
    {
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:BuySet:" +(curSet+1).ToString(), 1);

        ItemManager.instance.BuyNextSet();
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
