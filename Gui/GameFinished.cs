﻿using UnityEngine;
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
public class GameFinished : Statistic
{

    public Transform entry;

    public UILabel totalGoldLabel;

    public UILabel totalCashLabel;

    public UILabel totalXpLabel;

    public UITable lvlTableWid;

    public UIPanel lvlScroll;

    public Transform lvlTable;

    public UITable moneyTableWid;

    public Transform moneyTable;

    public UIPanel moneyScroll;

    public UILabel timerRestart;

    public UILabel winner;

    public bool resolved = false;

    public override void Activate()
    {
        MainPanel.alpha = 1.0f;
        active = true;
        if (!resolved) {
            resolved = true;
            RewardResolve();
        }
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

        
    }
    void RewardResolve()
    {
        List<RewardGUI> rewards = LevelingManager.instance.GetAllReward();
        int totalXP=0;
        foreach (RewardGUI reward in rewards)
        {
            totalXP += reward.amount;
            Transform newTrans = Instantiate(entry) as Transform;
            newTrans.parent = lvlTable;
            newTrans.localScale = new Vector3(1f, 1f, 1f);
            newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
            newTrans.localPosition = new Vector3(0f, 0f, 0f);
            NGUIReward script =  newTrans.GetComponent<NGUIReward>();
            script.text.text = reward.text;
            script.max.text = "" + reward.amount.ToString();
            script.count.text = "x" + reward.count.ToString();
            script.box.width = (int)Math.Truncate((lvlScroll.width));
           
        }
//        Debug.Log(totalXP);
        totalXpLabel.text = totalXP.ToString();
        lvlTable.localPosition = new Vector3((-1 * (lvlScroll.width / 2)) + 1, (lvlScroll.height / 2) + lvlTableWid.padding.y, 0f);
        lvlTableWid.Reposition();

        rewards = RewardManager.instance.GetAllReward();
        int totalCash = 0,totalGold = 0;
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
          
            Transform newTrans = Instantiate(entry) as Transform;
            newTrans.parent = moneyTable;
            newTrans.localScale = new Vector3(1f, 1f, 1f);
            newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
            newTrans.localPosition = new Vector3(0f, 0f, 0f);
            NGUIReward script = newTrans.GetComponent<NGUIReward>();
            script.text.text = reward.text;
            script.max.text = "" + reward.amount.ToString();
            script.count.text = "x" + reward.count.ToString();
            script.box.width = (int)Math.Truncate((moneyScroll.width));
        }
        totalGoldLabel.text = totalGold.ToString();
        totalCashLabel.text = totalCash.ToString();
        moneyTable.localPosition = new Vector3((-1 * (moneyScroll.width / 2)) + 1, (moneyScroll.height / 2) + moneyTableWid.padding.y, 0f);
        moneyTableWid.Reposition();

        winner.text = TextGenerator.instance.GetSimpleText("Team" + GameRule.instance.Winner());

    }
}
