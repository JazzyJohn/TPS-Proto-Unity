using UnityEngine;
using System.Collections;

public class TeamSlot{
	public UIPanel panel;
	public StatisticMessage message;
}

public class Statistic : MonoBehaviour {

	public UIPanel MainPanel;

	public bool active;

	public UILabel PlayerName;

	public GameObject TeamRed;
	public GameObject ShablonRedPlayer;

	public GameObject TeamBlue;
	public GameObject ShablonBluePlayer;

	public GameObject PlayerBox;

	public UISprite PlayerPreviev;

	public UIProgressBar Exp;
	public UILabel ExpLabel;
	public UILabel ExpNeedLabel;

	public UILabel Lvl;

	public ClassStat[] ClassStats;

	private float timer;

	TeamSlot[] RedTeamlPlayer = new TeamSlot[12];
	TeamSlot[] BlueTeamlPlayer = new TeamSlot[12];

	// Use this for initialization
	void Start () 
	{
		timer=0;

		for (int i=0; i<TeamBlue.transform.childCount;i++)
		{
			BlueTeamlPlayer[i] = new TeamSlot();

			BlueTeamlPlayer[i].panel  =TeamBlue.transform.GetChild(i).GetComponent<UIPanel>();
			BlueTeamlPlayer[i].panel.alpha = 0f;
			BlueTeamlPlayer[i].message=BlueTeamlPlayer[i].panel.GetComponent<StatisticMessage>();
		}
		for (int i=0; i<TeamRed.transform.childCount;i++)
		{
			RedTeamlPlayer[i]  = new TeamSlot(); 
			RedTeamlPlayer[i].panel  = TeamRed.transform.GetChild(i).GetComponent<UIPanel>();
			RedTeamlPlayer[i].panel .alpha = 0f;
			RedTeamlPlayer[i].message=RedTeamlPlayer[i].panel.GetComponent<StatisticMessage>();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void FixedUpdate()
	{
		if(active){
			LocalPlayerStat(LocalPlayer.GetName(), Choice._Player); // Внесения базовой инфы в статистику (+)

			AvatarPlayer(); //Установка аватара (+)
		
			RefreshStatisticPlayers(); //Обновление списока статистики игроков (+)
		}
	}

	public void ClassExpPlayer(PlayerMainGui.LevelStats stats)
	{


		for(int i=0; i<ClassStats.Length; i++)
		{
			ClassStats[i].Lvl.text = "Lvl " + stats.classLvl[i].ToString();
			ClassStats[i].Progress.value =stats.classProcent[i]/100f;
		}
	}
	                      

	public void LocalPlayerStat(string NamePlayer, int Class)
	{
		PlayerName.text = NamePlayer;
		switch((GameClassEnum)Class)
		{
		case GameClassEnum.ENGINEER:
			PlayerPreviev.spriteName="engineer";
			break;
		case GameClassEnum.MEDIC:
			PlayerPreviev.spriteName="medic";
			break;
		case GameClassEnum.ASSAULT:
			PlayerPreviev.spriteName="storm";
			break;
		case GameClassEnum.SCOUT:
			PlayerPreviev.spriteName="sniper";
			break;
		}
		PlayerMainGui.LevelStats stats = LevelingManager.instance.GetPlayerStats ();

		Lvl.text = "Lvl "+LevelingManager.instance.playerLvl;
		ExpLabel.text = LevelingManager.instance.playerExp.ToString();
		ExpNeedLabel.text = LevelingManager.instance.playerNeededExp[LevelingManager.instance.playerLvl].ToString();
		if (LevelingManager.instance.playerNeededExp[LevelingManager.instance.playerLvl] != 0)
			Exp.value = stats.playerProcent/100f;

		ClassExpPlayer(stats); //Внесение базовой инфы по классам (+)
	}

	public void RefreshExpAndLvl()
	{
		Lvl.text = "Lvl "+LevelingManager.instance.playerLvl;
		ExpLabel.text = LevelingManager.instance.playerExp.ToString();
		ExpNeedLabel.text = LevelingManager.instance.playerNeededExp[LevelingManager.instance.playerLvl].ToString();	
		if (LevelingManager.instance.playerNeededExp[LevelingManager.instance.playerLvl] != 0)
			Exp.value = (float)((LevelingManager.instance.playerExp)/(LevelingManager.instance.playerNeededExp[LevelingManager.instance.playerLvl]));
	}

	public void AvatarPlayer()
	{
		//TODO:: main avatar
		//PlayerBox.transform.FindChild("Texture").GetComponent<UITexture>().mainTexture = texture; 
	}

	public void AddExpPlayer()
	{

	}

	public void StringPlus(UILabel Label, int AddNum)
	{
		Label.text = (int.Parse(Label.text)+AddNum).ToString();
	}

	public void RefreshStatisticPlayers()
	{ 

			for (int i=0; i<TeamBlue.transform.childCount;i++)
			{
				BlueTeamlPlayer[i].panel.alpha = 0f;
			}
			for (int i=0; i<TeamRed.transform.childCount;i++)
			{
				RedTeamlPlayer[i].panel.alpha = 0f;
			}
			
			List<Player> players = PlayerManager.instance.FindAllPlayer ();

			int countRed=0;
			int countBlue=0;

			foreach(Player Gamer in players)
			{
				
				switch(Gamer.team)
				{
				case 1:
					RedTeamlPlayer[countRed].panel.alpha = 1f;
					RedTeamlPlayer[countRed].message.UID = Gamer.GetUid();
					RedTeamlPlayer[countRed].message.SetStartInfo(Gamer.GetName(), Gamer.Score.Kill, Gamer.Score.Death, Gamer.Score.Assist, Gamer);
					countRed++;
					break;
				case 2:
					BlueTeamlPlayer[countBlue].panel.alpha = 1f;
					BlueTeamlPlayer[countBlue].message.UID = Gamer.GetUid();
					BlueTeamlPlayer[countBlue].message.SetStartInfo(Gamer.GetName(), Gamer.Score.Kill, Gamer.Score.Death, Gamer.Score.Assist, Gamer);
					countBlue++;
					break;
				}
			}

		
		
	}
	public void Activate(){
		MainPanel.alpha = 1.0f;
		active = true;
	}
	public void DeActivate(){
		MainPanel.alpha = 0.0f;
		active = false;
	}
}

[System.Serializable]
public class ClassStat
{
	public string NameClass;
	public UILabel Lvl;
	public UIProgressBar Progress;
}
