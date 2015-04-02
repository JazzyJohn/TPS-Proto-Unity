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

    public int maxWeaponSet;

    public UIWidget weaponPanel;

    public int maxItemSet;

    public UIWidget itemPanel;

    public InventoryGroup group;

  //  public UILabel setLabel;

    public UIRect canBuy;

    public UILabel canBuyText;

    public UILabel canBuyPrice;

    public UIRect mustOpen;
	
    public UILabel  mustOpenText;
	
    Dictionary<string,InvItemGUI> AllItems= new Dictionary<string,InvItemGUI>();

    InvItemGUI[] items; 

    Dictionary<int, SelectedItemGUI> slots = new Dictionary<int, SelectedItemGUI>();

    SelectedItemGUI[] selected;
	
    int curSet = 1;

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
	     items = GetComponentsInChildren<InvItemGUI>();
		foreach(InvItemGUI itemInfo  in items){
			itemInfo.Shop = this;
            for(int i=0; i<itemInfo.ids.Length;i++){
                string id = itemInfo.ids[i];
                if (id == "-1")
                {
                    itemInfo.SetItem(null,i);
                }
                else
                {
                    itemInfo.SetItem(ItemManager.instance.GetItem(id), i);

                }
                itemInfo.SetSet(1);
                AllItems[itemInfo.id] = itemInfo;
            }
			
		
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
            itemInfo.TryOpen();
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
		
		WeaponInventorySlot weapon = slot as WeaponInventorySlot;
		if(weapon!=null){
			int slotType = (int)weapon.gameSlot;
			Choice.SetChoice(slotType, Choice._Player, new WeaponIndex(weapon.weaponId, ""));
			slots[slotType].SetItem();
            ItemManager.instance.SaveItemForSlot();
		}
        ArmorInventorySlot armor = slot as ArmorInventorySlot;
        if (armor != null)
        {
            int slotType = (int)armor.gameSlot;
            Choice.SetChoice(slotType, Choice._Player, new WeaponIndex(armor.armorId, ""));
            slots[slotType].SetItem();
            ItemManager.instance.SaveItemForSlot();
        }
	}

   
		
	public void CloseRepair(){
		repairGui.Close();
	}
	public void ShowSet(int needSet){
        
        switch (group)
        {
            case InventoryGroup.WEAPON:
                if (maxWeaponSet <= needSet) {
                    return;
                }
               
                curSet = needSet;

             
                break;
            case InventoryGroup.STUFF:
                if (maxItemSet <= needSet)
                {
                    return;
                }
                curSet = needSet;
                
                break;
        }
        mustOpen.alpha = 0.0f;
        canBuy.alpha = 0.0f;
        if (curSet > GlobalPlayer.instance.open_set)
        {
            if (curSet  > GlobalPlayer.instance.open_set+1)
            {
                mustOpenText.text = TextGenerator.instance.GetMoneyText("openSet", (curSet-1) * 10);
                mustOpen.alpha = 1.0f;
            }
            else
            {
                canBuyText.text = TextGenerator.instance.GetMoneyText("openSet", (curSet-1) * 10);
                canBuyPrice.text = TextGenerator.instance.GetSimpleText("openSet" + (needSet-1));
                canBuy.alpha = 1.0f;
            }

        }
        foreach (InvItemGUI gui in items)
        {
            gui.SetSet(curSet);
        }
	}
    public void ResetSet()
    {
        foreach (SelectedItemGUI slot in slots.Values)
        {
            slot.TryOpen(); 
        }
        ShowSet(curSet);
    }
    public void ShowGroup(int newGroup)
    {
        group = (InventoryGroup)newGroup;
        switch (group)
        {
            case InventoryGroup.WEAPON:
                itemPanel.alpha = 0.0f;
                
                weaponPanel.alpha = 1.0f;
                ShowSet(curSet);
                break;
            case InventoryGroup.STUFF:
                if (maxItemSet <= curSet)
                {
                    curSet = maxItemSet-1;
                }
                weaponPanel.alpha = 0.0f;
                
                itemPanel.alpha = 1.0f;
                ShowSet(curSet);
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
            AllItems[slot.id].UpdateItem(slot);
        }
    }
}


public enum InventoryGroup
{
    WEAPON, ARMOR, STUFF
}
