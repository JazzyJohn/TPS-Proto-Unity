using UnityEngine;
using System.Collections;



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

	UIPanel[] RedTeamlPlayer = new UIPanel[12];
	UIPanel[] BlueTeamlPlayer = new UIPanel[12];

	// Use this for initialization
	void Start () 
	{
		timer=0;

		for (int i=0; i<TeamBlue.transform.childCount;i++)
		{
			BlueTeamlPlayer[i] = TeamBlue.transform.GetChild(i).GetComponent<UIPanel>();
			BlueTeamlPlayer[i].alpha = 0f;
		}
		for (int i=0; i<TeamRed.transform.childCount;i++)
		{
			RedTeamlPlayer[i] = TeamRed.transform.GetChild(i).GetComponent<UIPanel>();
			RedTeamlPlayer[i].alpha = 0f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void FixedUpdate()
	{

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
		try
		{
			for (int i=0; i<TeamBlue.transform.childCount;i++)
			{
				BlueTeamlPlayer[i] = TeamBlue.transform.GetChild(i).GetComponent<UIPanel>();
				BlueTeamlPlayer[i].alpha = 0f;
			}
			for (int i=0; i<TeamRed.transform.childCount;i++)
			{
				RedTeamlPlayer[i] = TeamRed.transform.GetChild(i).GetComponent<UIPanel>();
				RedTeamlPlayer[i].alpha = 0f;
			}
			
			Player[] players = PlayerManager.instance.FindAllPlayer ();

			int countRed=0;
			int countBlue=0;

			foreach(Player Gamer in players)
			{
				Debug.Log(Gamer.team);
				switch(Gamer.team)
				{
				case 1:
					RedTeamlPlayer[countRed].alpha = 1f;
					RedTeamlPlayer[countRed].GetComponent<StatisticMessage>().UID = Gamer.GetUid();
					RedTeamlPlayer[countRed].GetComponent<StatisticMessage>().SetStartInfo(Gamer.GetName(), Gamer.Score.Kill, Gamer.Score.Death, Gamer.Score.Assist, Gamer);
					countRed++;
					break;
				case 2:
					BlueTeamlPlayer[countBlue]	.alpha = 1f;
					BlueTeamlPlayer[countBlue].GetComponent<StatisticMessage>().UID = Gamer.GetUid();
					BlueTeamlPlayer[countBlue].GetComponent<StatisticMessage>().SetStartInfo(Gamer.GetName(), Gamer.Score.Kill, Gamer.Score.Death, Gamer.Score.Assist, Gamer);
					countBlue++;
					break;
				}
			}
		}
		
		catch
		{
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
