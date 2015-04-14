using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ItemColor
{
    Normal,
    Times,
    Personal
}

public class GUIItem{
    public WeaponIndex index;
	public Texture2D texture;
	public string name;
    public ItemColor color;
    public int group;
    public string text;
    public bool isTimed=false;
    public DateTime timeend;
    public WeaponChar chars;
}
public class SelectPlayerGUI : MonoBehaviour {

	public _MenuElements MenuElements;

    public SmallShop shop;

	private int W; //Ширина
	private int H; //Высота

	public enum _PlayerC{None, Ennginer, Medic, Sniper, Soldier};
	public _PlayerC PlayerC;

	public enum _PlayerR{None, Red, Green, Next};
	public _PlayerR PlayerR;

	public GameObject Play;

	private float timer;

	public bool StartGame = false;

	public UIPanel ThisPanel;

    public Player LocalPlayer;

    public GAMEMODE mode;

    public UIToggle[] team;

    public int slotAmount =4;
    
    public List<GUIItem>[] listOfItems = new List<GUIItem>[4];

    public Color[] colorByType;
	// Use this for initialization
	void Start () 
	{
	
		PlayerC = _PlayerC.None;
		
		PlayerR = _PlayerR.None;
	}

    public void ReDrawAll() {
        
        DrawStims();
    }
    public void DrawStims() {
		List<SmallShopData> allStims = ItemManager.instance.GetAllStim();

        for (int i = 0; i < MenuElements.Stims.Length; i++)
        {
       
            if (allStims.Count > i)
            {
                MenuElements.Stims[i].SetObject(allStims[i]);
			}
            else
            {
                MenuElements.Stims[i].Hide();
            }
        }
    }
    public void SetLocalPlayer(Player player)
    {
        LocalPlayer = player;
        
        PrefabManager[] managers = FindObjectsOfType<PrefabManager>();
        foreach(PrefabManager manager in managers){
            if(manager.isPawns){
                playerManager=manager;
            }
        }
      
        LoadFromPrefab();
        ReDrawAll();
        SelectPlayer(1);
        SelectRobot();
      //  Debug.Log(player.team);
		if(player.team==0){
			Choice._Team = -1;
		}else{
			Choice._Team =player.team;
		}
        Debug.Log(Choice._Team );
    }
    public void Activate()
    {
        if (ThisPanel.alpha==0.0f)
        {
            ThisPanel.alpha = 1.0f;
            if(Choice._Player!=-1&&MenuElements.ClassModels.Count<Choice._Player&& MenuElements.ClassModels[Choice._Player] != null){
                MenuElements.ClassModels[Choice._Player].SetActive(true);
            }
        }
    }
    public void DeActivate()
    {
        if (ThisPanel.alpha==1.0f)
        {
            ThisPanel.alpha = 0.0f;
           
        
        }
        HideModel();
    }
	//Коректировка размеров
	public void ReSize()
	{

	}

	//Выбор класса
	public void SelectPlayer(int i)
	{
        if (LocalPlayer == null) {
            return;
        }
		switch(i)
		{
		case 1:
			PlayerC = _PlayerC.Ennginer;
			Choice._Player = (int)GameClassEnum.ENGINEER;
			break;
		case 2:
			PlayerC = _PlayerC.Medic;
			Choice._Player = (int)GameClassEnum.MEDIC;
			
			break;
		case 3:
			PlayerC = _PlayerC.Sniper;
			Choice._Player = (int)GameClassEnum.SCOUT;
			
			break;
		case 4:
			PlayerC = _PlayerC.Soldier;
			Choice._Player = (int)GameClassEnum.ASSAULT;
			break;
		}
        /*for (int j = 0; j < slotAmount; j++)
        {
            listOfItems [j] =ItemManager.instance.GetItemForSlot((GameClassEnum)Choice._Player, j);
            WeaponIndex choice = Choice.ForGuiSlot(j);
            if (!choice.IsSameIndex( WeaponIndex.Zero))
            {
                int index = listOfItems[j].FindIndex(delegate(GUIItem searchentry) { return searchentry.index.IsSameIndex(choice); });
                ChangeWeapon(j, index);
            }
            else
            {
                ChangeWeapon(j, 0);
            }
        }
        HideModel();
        if (MenuElements.ClassModels.Count<Choice._Player&& MenuElements.ClassModels[Choice._Player] != null) {
            MenuElements.ClassModels[Choice._Player].SetActive(true);
        };*/
	}

    public void HideModel() {
        foreach (GameObject obj in MenuElements.ClassModels.Values)
        {
            obj.SetActive(false);
        }
    
    }

	public void SelectRobot()
	{
		Choice._Robot = 1;
	}

	public void HideMenu(bool Z)
	{
		if (Z)
			ThisPanel.alpha = 0f;
		else
			ThisPanel.alpha = 1f;
	}

    public void AutoSelectTeam() {
        GA.API.Design.NewEvent("GUI:Select:AutoSelect"); 
        switch (mode) { 
            case GAMEMODE.PVE:
            case GAMEMODE.PVE_HOLD:

                Choice._Team = 1;
                if (team.Length > Choice._Team - 1 && team[Choice._Team - 1] != null)
                {
                    team[Choice._Team - 1].value = true;
                }
                break;

            case GAMEMODE.PVP:
            case GAMEMODE.PVPHUNT:
                 int[] teamCount = new int[2];
                List<Player> players = PlayerManager.instance.FindAllPlayer();
                foreach (Player player in players)
                {
                    if (player.team != 0)
                    {
                        teamCount[player.team - 1]++;
                    }
                }
             
                int i =0;
                if (teamCount[0] <= teamCount[1]) {
                    i=1;
                }else{
                    i=2;
                }

             
                if (i == 1 )
                    Choice._Team = 1;
                if (i == 2 )
                    Choice._Team = 2;

                team[Choice._Team - 1].value = true;
                break;

        
        }
       
    
    }

	public void SelectTeam(int i)
	{
        GA.API.Design.NewEvent("GUI:Select:SelectTeam:"+i); 
		int[] teamCount = new int[2];
		List<Player> players = PlayerManager.instance.FindAllPlayer ();
		foreach(Player player in players) {
			if(player.team!=0){
				teamCount[player.team-1]++;
			}
		}
		bool teamAblock=false,teamBblock=false;
		if(teamCount[0]>=teamCount[1]+1){
			teamAblock=true;
		}
		if(teamCount[1]>=teamCount[0]+1){
			teamBblock=true;
		}
       Debug.Log(teamAblock + "  " + teamBblock + i);
		if (teamAblock)
			Choice._Team = 2;
        else if (teamBblock)
			Choice._Team = 1;
        else
        {
            Choice._Team = i;
        }
	}

    public void NextWeapon(int TypeW) //Вперёд слайдер оружия
    {
        GA.API.Design.NewEvent("GUI:Select:NextWeapon"); 
        ChangeWeapon(TypeW, 1);
    }
    public void BackWeapon(int TypeW) //Назад слайдер оружия
    {
        GA.API.Design.NewEvent("GUI:Select:BackWeapon"); 
        ChangeWeapon(TypeW, -1);

    }
     public void ChangeWeapon(int TypeW, int delta) //Вперёд слайдер оружия
	{

        Choice._GUIChoice[Choice._Player][TypeW] += delta;

        if (Choice._GUIChoice[Choice._Player][TypeW] >= listOfItems[TypeW].Count)
              {
                  Choice._GUIChoice[Choice._Player][TypeW] = 0;
              }
            if (Choice._GUIChoice[Choice._Player][TypeW] < 0)
              {
                  Choice._GUIChoice[Choice._Player][TypeW] = listOfItems[TypeW].Count - 1;
              }
              ReDrawWeapon(TypeW, Choice._GUIChoice[Choice._Player][TypeW]);       
        
        
	}
    public void ReDrawWeapon(int TypeW, int choice)
    {
        if (listOfItems[TypeW].Count == 0) {
            return;
        }
        if (MenuElements.Weapon.Length == 0||MenuElements.Weapon[TypeW]==null)
        {
            return;
        }
        GUIItem item =  listOfItems[TypeW][choice];
        MenuElements.Weapon[TypeW].mainTexture = item.texture;


		if(colorByType.Length>(int)item.color){
			MenuElements.WeaponBack[TypeW].color = colorByType[(int)item.color];
		}

        if (MenuElements.WeaponTitle.Length > TypeW)
        {
            MenuElements.WeaponTitle[TypeW].text = item.name;
        }
        if (MenuElements.WeaponText.Length > TypeW)
        {
            MenuElements.WeaponText[TypeW].text = item.text;
        }
        if (MenuElements.WeaponBars.Length > TypeW)
        {
          
            if (item.chars != null)
            {
                MenuElements.WeaponBars[TypeW].widget.alpha = 1.0f;
                MenuElements.WeaponBars[TypeW].magazine.text = item.chars.magazine.ToString();
                MenuElements.WeaponBars[TypeW].dmg.value = item.chars.dmg;
                MenuElements.WeaponBars[TypeW].aim.value = item.chars.aim;
                MenuElements.WeaponBars[TypeW].reload.value = item.chars.reload;
                MenuElements.WeaponBars[TypeW].speed.value = item.chars.speed;
                MenuElements.WeaponBars[TypeW].mode.text = TextGenerator.instance.GetSimpleText(item.chars.gunMode);
            }
            else
            {
                MenuElements.WeaponBars[TypeW].widget.alpha = 0.0f;
            }
        }
        MenuElements.WeaponSelect[TypeW] = item;
        Choice.SetChoice(TypeW, Choice._Player, item.index);
    }


	void FixedUpdate()
	{
		
      
			timer=0;
			int Red=0;
			int Blue=0;
			List<Player> players = PlayerManager.instance.FindAllPlayer ();
			foreach(Player Player in players)
			{
				if (Player.team == 1)
					Red+=1;
				else if (Player.team == 2)
					Blue+=1;
			}
			MenuElements.Red.text = Red.ToString();
            if (MenuElements.Blue != null)
            {
                MenuElements.Blue.text = Blue.ToString();
            }
            DateTime saveNow = DateTime.Now;
            if (MenuElements.WeaponSelect.Length > 0)
            {
                for (int i = 0; i < MenuElements.WeaponText.Length; i++)
                {
                    if (MenuElements.WeaponSelect[i].isTimed)
                    {
                        TimeSpan timeSpan = MenuElements.WeaponSelect[i].timeend.Subtract(saveNow);
                        MenuElements.WeaponText[i].text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                    }

                }
            }
		
	}

	public void StartGameBut()
	{
       // Debug.Log(Choice._Player + " " + Choice._Robot + "  " + Choice._Team);  
        if (shop!=null&&shop.panel.alpha == 1.0)
        {
            return;
        }
        GA.API.Design.NewEvent("GUI:Select:Start"); 
        if (Choice._Team == -1)
        {
            AutoSelectTeam();
        }
        Debug.Log(Choice._Player + " " + Choice._Team);
        if (Choice._Player != -1 && Choice._Team != -1 )
        {

            switch (mode)
            {
                case GAMEMODE.PVE:
                    LocalPlayer.SetTeam(1);
                    break;
				case GAMEMODE.PVE_HOLD:
                    LocalPlayer.SetTeam(1);
                    break;
                default:
                    LocalPlayer.SetTeam(Choice._Team);
                    break;

            }
			 int timer = (int)LocalPlayer.GetRespawnTimer();
             if (timer <= 0)
             {
                /* for (int j = 0; j < slotAmount; j++)
                 {
                     listOfItems[j] = ItemManager.instance.GetItemForSlot((GameClassEnum)Choice._Player, j);
                     ChangeWeapon(j, 0);
                 }*/
                 LocalPlayer.selectedBot = 1;
                 LocalPlayer.selected = Choice._Player;
                 LocalPlayer.isStarted = true;
                 HideModel();
             }

        }
       
	}
    public PrefabManager playerManager;
    public Transform playerPreView;
    public void LoadFromPrefab(){
		UnityEngine.Object[] models;
        if (playerManager != null && playerManager.bundle!=null)
        {
			models = playerManager.bundle.LoadAll(typeof(ModelForGui));
		}else{
			MapLoader loader = FindObjectOfType<MapLoader>();
            if (loader == null)
            {

                return;
            }
			models = loader.pawnBundle.LoadAll(typeof(ModelForGui));
		}
			if (playerPreView != null)
			{
				foreach (UnityEngine.Object model in models)
				{
					ModelForGui obj = Instantiate(model, playerPreView.position, playerPreView.rotation) as ModelForGui;
					MenuElements.ClassModels[obj.index] = obj.gameObject;
					obj.gameObject.SetActive(false);
					obj.transform.parent = playerPreView;
					obj.transform.localScale = new Vector3(1, 1, 1);
				}
			}
		
    }
    public void RotateModel() {
       // Debug.Log("ROTATE");
    
    }
    public void ActivateStimPack(SmallShopData pack)
    {

        GA.API.Design.NewEvent("GUI:Select:ActivateStimPack:" + pack.engName); 
        if (pack.amount > 0)
        {
            LocalPlayer.ActivateStimpack(pack.itemId);
            DrawStims();
        }
        else
        {
            
            shop.Icon.mainTexture = pack.textureGUI;
            shop.Offer.text = pack.name;
           
            shop.data = pack;
            shop.Show();
        }
    }

}



[Serializable]
public class _MenuElements
{
	public int wSelectClass = 0;
	public UISprite SelectClass;
	public _PlayerClass[] PlayerClass = new _PlayerClass[4];

	public UITexture[] Weapon;
    public UISprite[] WeaponBack;
    public GUIItem[] WeaponSelect= new GUIItem[0];
    public UILabel[] WeaponText;
    public UILabel[] WeaponTitle;
    public CharSection[] WeaponBars;
    public SmallShopSlot[] Stims;
    
    public Dictionary<int,GameObject> ClassModels  = new Dictionary<int,GameObject>();

	public UILabel Red;
	public UILabel Blue;
}

[Serializable]
public class _PlayerClass
{
	public string Name;
	public UISprite Sprite;
}

public  class ChoiceSet{
    public ChoiceSet()
    {
        for (int i = 0; i < Choice.SLOT_CNT; i++)
        {
            slots[i] = WeaponIndex.Zero;
        }
    }
	public  WeaponIndex[] slots =  new WeaponIndex[Choice.SLOT_CNT];

}

public static class Choice
{
	public const int SLOT_CNT=9;
    public static int _Player = 0;
	public static int _Robot=-1;
	public static int _Team=-1;
	
	//PlayerWeapons
    public static WeaponIndex[] _Personal = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Main = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Extra = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Grenad = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _BodyArmor = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _HeadArmor = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
  

    public static WeaponIndex[] _Taunt = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
	public static WeaponIndex[] _HeadImplant = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
	public static WeaponIndex[] _BodyImplant= new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _HandImplant = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _ArmImplant = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };




    public static ChoiceSet[] ChoiceSet = new ChoiceSet[4] { new ChoiceSet(), new ChoiceSet(), new ChoiceSet(), new ChoiceSet() };
	
	public static int curSet=0;
	 
    public static int[][] _GUIChoice = new int[][] 
    {
        new int[5] ,
        new int[5],
        new int[5],
        new int[5],
    };

	public static void  ChangeSet(int set,int gameClass){
		curSet= set;
		for( int i=0;i<Choice.SLOT_CNT;i++){
			_SetChoice(i, _Player,ChoiceSet[curSet].slots[i] );
		}
	}
    public static WeaponIndex ForSaveSLot(int slot, int set)
    {
        return ChoiceSet[set].slots[slot];
    }
    public static WeaponIndex ForGuiSlot(int slot)
    {
		switch(slot){
		case 0:
			return _Personal[_Player];
			break;
		case 1:
			return _Main[_Player];
			break;
		case 2:
			return _Extra[_Player];
			break;
		case 3:
			return _Grenad[_Player];
			break;
		case 4:
			return _BodyArmor[_Player];
			break;	
		case 5:
			return _HeadArmor[_Player];
			break;	
		case 6:
				return _BodyImplant[_Player];
				break;
		case 7:
				return _HandImplant[_Player];
				break;
		case 8:
				return _ArmImplant[_Player];
				break;			
		}
        return WeaponIndex.Zero;
	}
    public static void SetChoice(int slot, int gameClass, WeaponIndex index)
    {
		ChoiceSet[curSet].slots[slot] =index;
		_SetChoice(slot, gameClass, index);
	}
	public static void SetChoice(int slot, int gameClass, WeaponIndex index,int set)
    {
		ChoiceSet[set].slots[slot] =index;
		if(set==curSet){
			_SetChoice(slot, gameClass, index);
		}
	}
	 private static void _SetChoice(int slot, int gameClass, WeaponIndex index)
    {
		
		switch(slot){
		case 0:
			
			_Personal[gameClass]=index;
			break;
		case 1:
			_Main[gameClass]=index;
			break;
		case 2:
			_Extra[gameClass]=index;
			break;
		case 3:
			_Grenad[gameClass]=index;
			break;
		case 4:
            _BodyArmor[gameClass] = index;
			break;
		case 5:
			_HeadArmor[gameClass]=index;
			break;
		case 6:
			_BodyImplant[gameClass]=index;
			break;
		case 7:
			_HandImplant[gameClass]=index;
			break;
		case 8:
			_ArmImplant[gameClass]=index;
			break;
		}
	}
	/*
	//RoboWeapons
	public static int[] _MeleeRobo = new int[3]{-1,-1,-1};
	public static int[] _MainRobo = new int[3]{-1,-1,-1};
	public static int[] _HeavyRobo = new int[3]{-1,-1,-1};
	public static int[] _TauntRobo = new int[3]{-1,-1,-1};
	public static void SetChoiceRobot(int slot, int gameClass, int index){
		switch(slot){
		case 0:
			_MeleeRobo[gameClass]=index;
			break;
		case 1:
			_MainRobo[gameClass]=index;
			break;
		case 2:
			_HeavyRobo[gameClass]=index;
			break;
		case 3:
			_TauntRobo[gameClass]=index;
			break;
			
		}
	}
	public static int ForGuiSlotRobot(int slot){
		switch(slot){
		case 0:
			return _MeleeRobo[_Robot];
			break;
		case 1:
			return _MainRobo[_Robot];
			break;
		case 2:
			return _HeavyRobo[_Robot];
			break;
		case 3:
			return _TauntRobo[_Robot];
			break;
		}
		return -1;
	}*/
}
