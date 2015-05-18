using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public struct RewardGUI
{
    public string text;

    public int count;

    public int amount;

    public bool isCash;
}
public class LVLAnimationKey
{
    public int lvl;

    public float startExpProcent;

    public float finishedxExpProcent;

    public float startExp;

    public float finishedxExp;
}
public class LVLAnimation
{
    public List<LVLAnimationKey> keys= new List<LVLAnimationKey>();

    public int curStage = 0;

    public bool IsActive()
    {
        if (curStage >= keys.Count)
        {
            return false;
        }
        return true;
    }

    public int GetLvl()
    {
        return keys[curStage].lvl;
    }
    public float GetStartProcent()
    {
        return keys[curStage].startExpProcent;
    }
    public float GetDiffernceProcent()
    {
        return (keys[curStage].finishedxExpProcent-keys[curStage].startExpProcent);
    }
    public float GetStart()
    {
        return keys[curStage].startExp;
    }
    public float GetDiffernce()
    {
        return (keys[curStage].finishedxExp- keys[curStage].startExp);
    }
    public void NextStage()
    {
        curStage++;
    }
}

public class GameFinished : MonoBehaviour
{

    public Transform entry;

    public StatisticMessage[] RedTeamlPlayer;

    public StatisticMessage[] BlueTeamlPlayer;

      
    public UILabel totalCashLabel;

   

    public UILabel timerRestart;

    public bool resolved = false;

    public UIPanel MainPanel;


    public UILabel killLabel;
    public UILabel deathabel;
    public UILabel ratingLabel;
    public UILabel assitsLabel;
    public UILabel lvlLabel;
    public UILabel lvlNextLabel;
    public UIWidget lvlNext;
    public UIWidget lvlMax;
    public UILabel totalMission;

    public UILabel killLabelBest;
    public UILabel deathabelBest;
    public UILabel ratingLabelBest;
    public UILabel assitsLabelBest;
    public UILabel lvlLabelBest;


    public UIRect best;
    public UIRect notBest;

    public LVLAnimation animation;

    public UILabel animationLvl;

    public UILabel animationExp;

    public UIProgressBar animationBar;


    private float timePerKey;

    private float timeOfKey;  


    public void Activate()
    {
        MainPanel.alpha = 1.0f;
        
        if (!resolved) {
            resolved = true;
            //Invoke("RewardResolve", 1.0f);
            RewardResolve();
            RefreshStatisticPlayers();
            //ResolvedExpired();
        }
    }
    public void DeActivate()
    {
        MainPanel.alpha = 0.0f;
        
    }
    public void Update()
    {
       
      
      
        float restart = GameRule.instance.GetRestartTimer();
        if (restart < 0)
        {
            timerRestart.text = TextGenerator.instance.GetSimpleText("Loading");

        }
        else
        {
            timerRestart.text = TextGenerator.instance.GetSimpleText("NextRound");
            timerRestart.text = timerRestart.text +GameRule.instance.GetRestartTimer().ToString("0.0");
        }
        UpdateAnimation();
        
    }

    public void InitAnimation(){
    
        if(animation!=null){
            timePerKey = (GameRule.RESTART_TIME-2 )/ animation.keys.Count;
            timeOfKey=0;
        }
    }
    public void UpdateAnimation()
    {
        if (!resolved)
        {
            return;
        }
        if(!animation.IsActive()){
            return;
        }
        if (animation != null)
        {
            animationLvl.text = animation.GetLvl().ToString();
            timeOfKey += Time.deltaTime;
            animationBar.value = animation.GetStartProcent() + animation.GetDiffernceProcent() * timeOfKey / timePerKey;
            animationExp.text = (animation.GetStart() + animation.GetDiffernce() * timeOfKey / timePerKey).ToString("0");
            if (timeOfKey > timePerKey)
            {
                timeOfKey = 0.0f;
                animation.NextStage();
            }
        }
    }
    void RewardResolve()
    {
       
   
       
      
//        Debug.Log(totalXP);


        animation = LevelingManager.instance.GetAnimationData();
        InitAnimation();
        List < RewardGUI> rewards = RewardManager.instance.GetAllReward();
        int totalCash = 0, totalGold = 0, totalXP  = LevelingManager.instance.GetTotalXP();
        foreach (RewardGUI reward in rewards)
        {
            if (reward.isCash)
            {
                totalCash += reward.amount;
            }
            else
            {
                totalGold += reward.amount;
            }
       
        }

        totalCashLabel.text = totalCash.ToString();
        totalMission.text = AchievementManager.instance.GetDoneInMatch().ToString();
        killLabel.text = (Player.localPlayer.Score.Kill + Player.localPlayer.Score.RobotKill + Player.localPlayer.Score.AIKill).ToString();
        deathabel.text = Player.localPlayer.Score.Death.ToString();
        ratingLabel.text = Player.localPlayer.Score.rating.ToString();
        assitsLabel.text = Player.localPlayer.Score.Assist.ToString();
        lvlLabel.text = LevelingManager.instance.playerLvl.ToString();
        if (LevelingManager.instance.HasNext())
        {
            lvlNextLabel.text = (LevelingManager.instance.playerLvl + 1).ToString();
            lvlNext.alpha = 1.0f;
            lvlMax.alpha = 0.0f;
        }
        else
        {
            lvlNext.alpha = 0.0f;
            lvlMax.alpha = 1.0f;
        }
        GA.API.Design.NewEvent("Game:MatchEnd:End", 1);
        GA.API.Design.NewEvent("Game:MatchEnd:PlayerReward:XP", totalXP);
        GA.API.Design.NewEvent("Game:MatchEnd:PlayerReward:Gold", totalGold);
        GA.API.Design.NewEvent("Game:MatchEnd:PlayerReward:Cash", totalCash);
    }

    public void RefreshStatisticPlayers()
    {
        PlayerManager.instance.Resort();
        List<Player> players = PlayerManager.instance.FindAllPlayer();

        int countRed = 0;
        int countBlue = 0;

        foreach (Player Gamer in players)
        {

            switch (Gamer.team)
            {
                case 1:
                    RedTeamlPlayer[countRed].UID = Gamer.GetUid();

                    string namer;
                    if (Gamer.GetName() != null || Gamer.GetName() != "")
                        namer = Gamer.GetName();
                    else
                        namer = "NoName";

                    RedTeamlPlayer[countRed].SetStartInfo(Gamer);
                    RedTeamlPlayer[countRed].Hide(false);
                    countRed++;
                    break;
                case 2:
                    BlueTeamlPlayer[countBlue].UID = Gamer.GetUid();

                    string nameb;
                    if (Gamer.GetName() != null || Gamer.GetName() != "")
                        nameb = Gamer.GetName();
                    else
                        nameb = "NoName";

                    BlueTeamlPlayer[countBlue].SetStartInfo(Gamer);
                    BlueTeamlPlayer[countBlue].Hide(false);
                    countBlue++;
                    break;
            }
        }

        for (; countBlue < BlueTeamlPlayer.Length; countBlue++)
        {
            BlueTeamlPlayer[countBlue].Hide(true);
        }
        for (; countRed < RedTeamlPlayer.Length; countRed++)
        {
            RedTeamlPlayer[countRed].Hide(true);
        }
        if (players[0] == Player.localPlayer)
        {
            best.alpha = 1.0f;
            notBest.alpha = 0.0f;

        }
        else
        {
            best.alpha = 0.0f;
            notBest.alpha = 1.0f;
            killLabelBest.text = (players[0].Score.Kill + players[0].Score.RobotKill + players[0].Score.AIKill).ToString();
            deathabelBest.text = players[0].Score.Death.ToString();
            ratingLabelBest.text = players[0].Score.rating.ToString();
            assitsLabelBest.text = players[0].Score.Assist.ToString();
            lvlLabelBest.text = Player.localPlayer.Score.Lvl.ToString();
        }
    }

    
  
}

