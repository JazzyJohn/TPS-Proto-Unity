using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MissionGUI : MonoBehaviour {

    public TaskGUI[] tasks;

    public UILabel time;

    public void Draw()
    {
        Achievement[] list =  AchievementManager.instance.GetTask();
        for (int i = 0; i < list.Length; i++)
        {
            tasks[i].Draw(list[i]);
        }
    }
    public void Update()
    {
        DateTime tomorrow = DateTime.Today.AddDays(1);


       TimeSpan span = new TimeSpan( tomorrow.Ticks-DateTime.UtcNow.Ticks);
       time.text= string.Format("{0:D2} : {1:D2} : {2:D2} ", span.Hours, span.Minutes, span.Seconds);
    }
    public void Skip(int id)
    {
        StartCoroutine(AchievementManager.instance.SkipAchive(tasks[id].id));
    }
    public void UpdateTask()
    {
        StartCoroutine(AchievementManager.instance.UpdateTask());
    }
}
[System.Serializable]
public class TaskGUI
{
    public UIRect main;

    public UILabel name;
    public UILabel descr;

    public UIWidget cashReward;
    public UILabel cashRewardText;

    public UIWidget goldReward;
    public UILabel goldRewardText;

    public UIWidget skillReward;
    public UILabel skillRewardText;

    public UITexture icon;

    public UIWidget next;
    public UILabel nextName;
    public UILabel nextDescr;

    public UIWidget nextCashReward;
    public UILabel nextCashRewardText;

    public UIWidget nextGoldReward;
    public UILabel nextGoldRewardText;

    public UIWidget nextSkillReward;
    public UILabel nextSkillRewardText;

    public UIWidget done;

    public int id;

    public void Draw(Achievement achiv)
    {
        if (achiv == null)
        {
            main.alpha = 0.0f;
            return;
        }
        
        if (name != null)
        {
            name.text = achiv.name;
        }
        if (achiv.isDone)
        {
            done.alpha = 1.0f;
        }
        else
        {
            done.alpha = 0.0f;
        }
        descr.text = achiv.description;
        if (achiv.reward.cash == 0)
        {
            cashReward.alpha = 0.0f;
        }
        else
        {
            cashReward.alpha = 1.0f;
            cashRewardText.text = achiv.reward.cash.ToString();
        }
        if (achiv.reward.gold == 0)
        {
            goldReward.alpha = 0.0f;
        }
        else
        {
            goldReward.alpha = 1.0f;
            goldRewardText.text = achiv.reward.gold.ToString();
        }
        if (achiv.reward.skill == 0)
        {
            skillReward.alpha = 0.0f;
        }
        else
        {
            skillReward.alpha = 1.0f;
            skillRewardText.text = achiv.reward.skill.ToString();

        }
        icon.mainTexture = achiv.textureIcon;
        if (achiv.next != null)
        {

            if ( nextName != null)
            {
                 nextName.text = achiv.next.name;
            }
           
            nextDescr.text = achiv.next.description;
            if (achiv.next.reward.cash == 0)
            {
                nextCashReward.alpha = 0.0f;
            }
            else
            {
                nextCashReward.alpha = 1.0f;
                nextCashRewardText.text = achiv.next.reward.cash.ToString();
            }
            if (achiv.next.reward.gold == 0)
            {
                nextGoldReward.alpha = 0.0f;
            }
            else
            {
                nextGoldReward.alpha = 1.0f;
                nextGoldRewardText.text = achiv.next.reward.gold.ToString();
            }
            if (achiv.next.reward.skill == 0)
            {
                nextSkillReward.alpha = 0.0f;
            }
            else
            {
                nextSkillReward.alpha = 1.0f;
                nextSkillRewardText.text = achiv.next.reward.skill.ToString();

            }


        }
        id = achiv.achievementId;
    }
}