using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamSlot{
	public StatisticMessage message;
}

public class Statistic : MonoBehaviour {
    private Player LocalPlayer;

	public UIPanel MainPanel;

	public bool active;

    public UIGrid[] GrildTeam = new UIGrid[2];

	public UILabel PlayerName;


    public UITexture avatar;

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

    public AchievementUI[] achievements;

    public string achievementOpenSprite;

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
            BlueTeamlPlayer[i].message = TeamBlue.transform.GetChild(i).GetComponent<StatisticMessage>();
            BlueTeamlPlayer[i].message.Hide(true);
		}
		for (int i=0; i<TeamRed.transform.childCount;i++)
		{
			RedTeamlPlayer[i]  = new TeamSlot();
            RedTeamlPlayer[i].message = TeamRed.transform.GetChild(i).GetComponent<StatisticMessage>();
            RedTeamlPlayer[i].message.Hide(true);
		}
        avatar.mainTexture = GlobalPlayer.instance.avatar;
	}


    public void SetLocalPalyer(Player newPlayer) {
        LocalPlayer = newPlayer;
    }

    public void SetLocalPlayer(Player newPlayer)
    {
        LocalPlayer = newPlayer;
    }
    public IEnumerator LateFrameResize()
    {
        yield return new WaitForSeconds(1.0f);
        MainPanel.Invalidate(true);
    }
    public void ReSize() //Правка позиции компонентов
    {


        StartCoroutine(LateFrameResize());
    }
	void FixedUpdate()
	{
		if(active){
			LocalPlayerStat(LocalPlayer.GetName(), Choice._Player); // Внесения базовой инфы в статистику (+)

			AvatarPlayer(); //Установка аватара (+)

            ResizeGrild(); // Изменение активных слотов

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

    int countRed = 0;
    int countBlue = 0;

    int countRedSlotUse = 0;
    int countBlueSlotUse = 0;

    

    public void ResizeGrild()
    {
        List<Player> players = PlayerManager.instance.FindAllPlayer();

      

            countRed = 0;
            countBlue = 0;
            foreach (Player Gamer in players)
            {
                switch (Gamer.team)
                {
                    case 1:
                        countRed++;
                        break;
                    case 2:
                        countBlue++;
                        break;
                }
            }

            //Изменения активных слотов
            int RUse = countRedSlotUse;
            int RSlot = countRed;
            if (RSlot > RUse)
            {
                for (int i = RUse; i < RSlot; i++)
                {
                    RedTeamlPlayer[i].message.Hide(false);
                    countRedSlotUse++;
                }
            }
            else if (RSlot < RUse)
            {

                for (int i = RSlot; i < RUse; i++)
                {
                    RedTeamlPlayer[i].message.Hide(true);
                    countRedSlotUse--;
                }
            }

            int BUse = countBlueSlotUse;
            int BSlot = countBlue;
            if (BSlot > BUse)
            {
                for (int i = BUse; i < BSlot; i++)
                {
                    BlueTeamlPlayer[i].message.Hide(false);
                    countBlueSlotUse++;
                }
            }
            else if (BSlot < BUse)
            {

                for (int i = BSlot; i < BUse; i++)
                {
                    BlueTeamlPlayer[i].message.Hide(true);
                    countBlueSlotUse--;
                }
            }
            
        

    }

	public void RefreshStatisticPlayers()
	{
        List<Player> players = PlayerManager.instance.FindAllPlayer();

        countRed = 0;
        countBlue = 0;

        foreach (Player Gamer in players)
        {
            Debug.Log(Gamer.UID);
            switch (Gamer.team)
            {
                case 1:
                    RedTeamlPlayer[countRed].message.UID = Gamer.GetUid();

                    string namer;
                    if (Gamer.GetName() != null || Gamer.GetName() != "")
                        namer = Gamer.GetName();
                    else
                        namer = "NoName";

                    RedTeamlPlayer[countRed].message.SetStartInfo(namer, Gamer.Score.Kill, Gamer.Score.Death, Gamer.Score.Assist, Gamer);
                    RedTeamlPlayer[countRed].message.Hide(false);
                    countRed++;
                    break;
                case 2:
                    BlueTeamlPlayer[countBlue].message.UID = Gamer.GetUid();

                    string nameb;
                    if (Gamer.GetName() != null || Gamer.GetName() != "")
                        nameb = Gamer.GetName();
                    else
                        nameb = "NoName";

                    BlueTeamlPlayer[countBlue].message.SetStartInfo(nameb, Gamer.Score.Kill, Gamer.Score.Death, Gamer.Score.Assist, Gamer);
                    BlueTeamlPlayer[countBlue].message.Hide(false);
                    countBlue++;
                    break;
            }
        }
	}
	public virtual void Activate(){
		MainPanel.alpha = 1.0f;
		active = true;
        AquierAchievement();
	}
	public void DeActivate(){
		MainPanel.alpha = 0.0f;
		active = false;
	}
    public void AquierAchievement()
    {
        List<Achievement> list = AchievementManager.instance.GetDaylics();
        for (int i =0; i< achievements.Length;i++)
        {
            AchievementUI ui = achievements[i];
            if (list.Count <= i)
            {
                ui.main.alpha = 0.0f;
                break;
            }
            else
            {
                ui.main.alpha = 1.0f;
            }
            Achievement ach  = list[i];
            
            if (ach.isDone)
            {
                ui.finished.spriteName = achievementOpenSprite;
                ui.progress.alpha=0.0f;
            }
            ui.name.text = ach.description;
            ui.progress.text = ach.GetProgress();
        }
    }
}

[System.Serializable]
public class ClassStat
{
	public string NameClass;
	public UILabel Lvl;
	public UIProgressBar Progress;
}

[System.Serializable]
public class AchievementUI
{
    public UILabel name;
    public UILabel progress;
    public UISprite finished;
    public UIWidget main;

}