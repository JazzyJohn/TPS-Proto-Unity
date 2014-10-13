using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUIItem{
    public WeaponIndex index;
	public Texture2D texture;
	public string name;
    public Color color;
    public int group;
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

    public List<GUIItem>[] listOfItems = new List<GUIItem>[4];
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
        Choice._Team = -1;
    }
    public void Activate()
    {
        if (ThisPanel.alpha==0.0f)
        {
            ThisPanel.alpha = 1.0f;
            if(Choice._Player!=-1){
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
        for(int j = 0; j <4;j++){
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
        
        MenuElements.ClassModels[Choice._Player].SetActive(true);
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
        switch (mode) { 
            case GAMEMODE.PVE:
                Choice._Team = 1;
                team[Choice._Team - 1].value = true;
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
                if (teamCount[0] > teamCount[1]) {
                    i=2;
                }else{
                    i=1;
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
     
		int[] teamCount = new int[2];
		List<Player> players = PlayerManager.instance.FindAllPlayer ();
		foreach(Player player in players) {
			if(player.team!=0){
				teamCount[player.team-1]++;
			}
		}
		bool teamAblock=false,teamBblock=false;
		if(teamCount[0]>teamCount[1]+1){
			teamAblock=true;
		}
		if(teamCount[1]>teamCount[0]+1){
			teamBblock=true;
		}
		if (i == 1 && !teamAblock)
			Choice._Team = 1;
        else if (i == 2 && !teamBblock)
			Choice._Team = 2;
	}

    public void NextWeapon(int TypeW) //Вперёд слайдер оружия
    {
        ChangeWeapon(TypeW, 1);
    }
    public void BackWeapon(int TypeW) //Назад слайдер оружия
    {
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

        MenuElements.Weapon[TypeW].mainTexture = listOfItems[TypeW][choice].texture;
        MenuElements.WeaponBack[TypeW].color = listOfItems[TypeW][choice].color;
        Choice.SetChoice(TypeW, Choice._Player, listOfItems[TypeW][choice].index);
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
		
	}

	public void StartGameBut()
	{
       // Debug.Log(Choice._Player + " " + Choice._Robot + "  " + Choice._Team);  
        if (shop!=null&&shop.panel.alpha == 1.0)
        {
            return;
        }
      
        if (Choice._Team == -1)
        {
            AutoSelectTeam();
        }
        if (Choice._Player != -1 && Choice._Team != -1 )
        {

            switch (mode)
            {
                case GAMEMODE.PVE:
                    LocalPlayer.SetTeam(1);
                    break;
                default:
                    LocalPlayer.SetTeam(Choice._Team);
                    break;

            }
			 int timer = (int)LocalPlayer.GetRespawnTimer();
             if (timer <= 0)
             {
                 for (int j = 0; j < 4; j++)
                 {
                     listOfItems[j] = ItemManager.instance.GetItemForSlot((GameClassEnum)Choice._Player, j);
                     ChangeWeapon(j, 0);
                 }
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
        UnityEngine.Object[] models = playerManager.bundle.LoadAll(typeof(ModelForGui));
       foreach (UnityEngine.Object model in models) {
           ModelForGui obj = Instantiate(model, playerPreView.position, playerPreView.rotation) as ModelForGui;
           MenuElements.ClassModels[obj.index] = obj.gameObject;
           obj.gameObject.SetActive(false);
           obj.transform.parent = playerPreView;
           obj.transform.localScale = new Vector3(1,1,1);
       }
    }
    public void RotateModel() {
       // Debug.Log("ROTATE");
    
    }
    public void ActivateStimPack(SmallShopData pack)
    {
       
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



public static class Choice
{
    public static int _Player = -1;
	public static int _Robot=-1;
	public static int _Team=-1;
	
	//PlayerWeapons
    public static WeaponIndex[] _Personal = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Main = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Extra = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Grenad = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _Taunt = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
	public static WeaponIndex[] _HeadImplant = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
	public static WeaponIndex[] _BodyImplant= new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _HandImplant = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };
    public static WeaponIndex[] _ArmImplant = new WeaponIndex[4] { WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero, WeaponIndex.Zero };


	 
    public static int[][] _GUIChoice = new int[][] 
    {
        new int[5] ,
        new int[5],
        new int[5],
        new int[5],
    };


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
			return _Taunt[_Player];
			break;	
		case 5:
			return _HeadImplant[_Player];
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
			_Taunt[gameClass]=index;
			break;
		case 5:
			_HeadImplant[gameClass]=index;
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