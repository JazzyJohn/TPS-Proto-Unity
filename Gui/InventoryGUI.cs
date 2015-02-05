using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InventoryGUI : MonoBehaviour {

	public enum Tab{Inventory, Statistic, Skill};
	public Tab OpenTab = Tab.Inventory;
	public GameClassEnum gameClass = GameClassEnum.ENGINEER;

	public Transform TableTransform;
	public UITable Table;
	public UIPanel ItemsPanel;
	public UIScrollView ItemsScroll;
	public MainMenuGUI MainMenu;

	public UIPanel Detail;
	
	public UIPanel Inventory;
	
	public UIPanel lotItem;

	public InvItems InvItem;

    public DetailItemGUI detailItemGUI; 
	
	public LotItemGUI lotItemGUI;
	
	public UIScrollBar Scroll;
	
	public UIPanel repair;

	public RepairGUI repairGui;

	public AskWindow askWindow;

	public MainMenuStatistic statistic;

    public ShopGUI shop;

    bool[] allowedReapair = new bool[3];
	// Use this for initialization
	void Start () 
	{
		InvItem.Main = this;
        statistic.Main = this;
        Detail.alpha = 0f;
        repair.alpha = 0f;

        statistic.AllStat.AddRange(statistic.AllStatGrild.transform.GetComponentsInChildren<UILabel>());
        foreach (UILabel Label in statistic.AllStat)
		{
            statistic.DefValueStat.Add(Label.text);
		}
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
	public void SmallRepair(){
        if (!allowedReapair[0])
        {
            shop.ShowCategory(2);
            return;
        }
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:RepairKit:Small");
		Repair(ItemManager.smallRepairId);
	
	}
	public void NormalRepair(){
        if (!allowedReapair[1])
        {
            shop.ShowCategory(2);
            return;
        }
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:RepairKit:Normal");
		Repair(ItemManager.normalRepairId);
	}
	public void MaximumRepair(){
        if (!allowedReapair[2])
        {
            shop.ShowCategory(2);
            return;
        }
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:RepairKit:Big");
		Repair(ItemManager.maximumRepairId);
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

 
	public void ShowRepair(){
		repair.alpha= 1.0f;
		int[] cnt = ItemManager.instance.GetAllRepair();
        for (int i = 0; i < 3; i++)
        {
            allowedReapair[i] = cnt[i] > 0;
        }
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:ShowRepair");
         repairGui.smallCnt.text = cnt[0].ToString();
        repairGui.mediumCnt.text = cnt[1].ToString();
        repairGui.maxCnt.text = cnt[2].ToString();
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

	
	public void EditClass(int GameClass)
	{
		this.gameClass = (GameClassEnum)GameClass;
		switch(OpenTab)
		{
		case Tab.Inventory:
			InvItem.EditCategory();
			break;
		case Tab.Statistic:

			break;
		}
	}

    public void OpenList(List<InventorySlot> result)
    {
        InvItem.Clean();
        foreach (InventorySlot slot in result)
        {
            InvItem.Add(slot);
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

	public PanelInvGUI Panels;

	public void LoadStatistic()
	{
		Panels.HidePanel();
		Panels.Statistic.alpha = 1f;
		OpenTab = Tab.Statistic;
        detailItemGUI.item = null;
        Destroy(detailItemGUI.gunModel);
	}

	public void LoadInventory()
	{
		Panels.HidePanel();
		Panels.Inventory.alpha = 1f;
		OpenTab = Tab.Inventory;
	}

	public void EditCategoryStat(int num)
	{
        statistic.OpenTab = (MainMenuStatistic.Tab)num;

		switch(statistic.OpenTab)
		{
            case MainMenuStatistic.Tab.Achive:
                {
                    statistic.Clean(AchivGUI.type.Achiv);

                    List<Achievement> achivments = AchievementManager.instance.GetAchivment();
                    foreach (Achievement achiv in achivments)
                    {
                        statistic.AddObj(achiv, AchivGUI.type.Achiv);
                    }
                }
                break;
            case MainMenuStatistic.Tab.Missions:
                {

                    statistic.Clean(AchivGUI.type.Dailic);
                    List<Achievement> achivments = AchievementManager.instance.GetDaylics();
                    foreach (Achievement achiv in achivments)
                    {
                        statistic.AddObj(achiv, AchivGUI.type.Dailic);
                    }
                }
                
                break;
            case MainMenuStatistic.Tab.Stat:
            statistic.StatToDef();
            foreach (UILabel Label in statistic.AllStat)
			{
				//Label.text += полученное значение;
			}
			break;
		}
        StartCoroutine(statistic.ReSize());
	}
	
}

[Serializable]
public class PanelInvGUI
{
	public UIPanel Statistic;
	public UIPanel Inventory;

	public void HidePanel()
	{
		Statistic.alpha = 0f;
		Inventory.alpha = 0f;
	}
}

[Serializable]
public class InvItems
{
	[HideInInspector]
    public InventoryGUI Main;

	public Transform ShablonItem;

	public int Page;
	public int ItemsCount;
	
    public List<InvItem> AllItems;
    
	public void EditCategory() //Смена закладки(категории или класса)
	{
		if(Main.ItemsPanel.alpha == 1f)
			Main.ItemsPanel.alpha = 0f;        
		
		Main.StartCoroutine(ItemManager.instance.GenerateInvList(Main.gameClass, Main));
		
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
        foreach (InvItem item in AllItems)
        {
            GameObject.Destroy(item.Obj.gameObject);
        }
        AllItems.Clear();
    }
	public void Add(InventorySlot item) //Добавление товара в панель магазина
	{
        InvItem NewItem = new InvItem();

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

[Serializable]
public class MainMenuStatistic
{
    [HideInInspector]
    public InventoryGUI Main;
	public UISprite Background;
	public UITable[] Tables;
	public int CoutColl;

	public List<string> DefValueStat;
	public List<UILabel> AllStat;

	public UIGrid AllStatGrild;

	public enum Tab{Stat, Missions, Achive};
	public Tab OpenTab = Tab.Stat;

    public AchivGUI achivPerfab;

    public AchivGUI daylicPrefab;

	public UITable AchivTable;
	public UITable DailicTable;

	public void StatToDef()
	{
		for(int i = 0; i<AllStat.Count; i++)
		{
			AllStat[i].text = DefValueStat[i];
		}
	}
	
	int Width;

	public void AddObj(Achievement achiv, AchivGUI.type TypeObj)
	{
        Transform obj;
        if (TypeObj == AchivGUI.type.Achiv)
		     obj = Transform.Instantiate(achivPerfab.transform) as Transform;
        else
            obj = Transform.Instantiate(daylicPrefab.transform) as Transform;
		AchivGUI script = obj.GetComponent<AchivGUI>();
        script.Main = Main;
        script.Type = TypeObj;

        script.GetInfo(achiv);
	}

	public void GetParent(AchivGUI obj)
	{
		if(obj.Type == AchivGUI.type.Achiv)
			obj.transform.parent = AchivTable.transform;
		else
			obj.transform.parent = DailicTable.transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
     
	

		obj.Widget.alpha = 1f;
	}

	public int EditCountColl
	{
		set
		{
			CoutColl = value;
			foreach(UITable Table in Tables)
			{
				Table.columns = value;
				Table.Reposition();
			}
		}
	}

	public void Clean(AchivGUI.type Type)
	{
		switch(Type)
		{
		case AchivGUI.type.Achiv:
             foreach (Transform Obj in AchivTable.transform)
			{
				GameObject.Destroy(Obj.gameObject);
			}
			
			break;
		case AchivGUI.type.Dailic:
            foreach (Transform Obj in DailicTable.transform)
            {
                GameObject.Destroy(Obj.gameObject);
            }
			break;
		}
	}

	public IEnumerator ReSize()
	{

        yield return new WaitForEndOfFrame();
       
			AchivTable.Reposition();
			DailicTable.Reposition();
		
	}
}
