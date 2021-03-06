﻿using UnityEngine;
using System.Collections;

public class PawnOnGuiComponent : ShowOnGuiComponent {

    public float startWidth=100.0f;

    public float sizeCoef=1.0f;
	
	public bool teamColor=false;

    Pawn pawn;
    public override void LocalPlayerSeeMe(float distance,int team, bool state){
    
        if(hudentry==null){
            return;
        }
        float width = startWidth -distance*sizeCoef;
      //  Debug.Log((int)width);
        hudentry.PerfabInfo.Sprite.width = (int)width;
     //   Debug.Log("PawnOnGuiComponent" +pawn.team+" "+team);
        if (pawn.team != team && pawn.team!=0)
        {
            if (hudentry.PerfabInfo.ProgressBar != null && hudentry.PerfabInfo.ProgressBar.alpha != 0)
            {
                hudentry.PerfabInfo.ProgressBar.alpha = 0;
            }
        }
//        Debug.Log(state);
        isShow = state;


    }
    public void UpdateGui()
    {
        if (hudentry == null)
        {
            return;
        }
        float health = pawn.health;
        if (teamColor&&team != pawn.team)
        {
            if (pawn.team == 1)
			{
				if(hudentry.label!=null){
					hudentry.label.color = ally;
				}
				if(hudentry.Sprite!=null){
					hudentry.Sprite.color = ally;
				}
			}
			else
			{
				if(hudentry.label!=null){
					hudentry.label.color = enemy;
				}
				if(hudentry.Sprite!=null){
					hudentry.Sprite.color = enemy;
				}
			}
            if (hudentry.label != null)
            {
                hudentry.label.alpha = 0;
            }
			
		}
       
        team = pawn.team;
        if (health<0)
        {
            health = 0;
        }
        if (hudentry.PerfabInfo.ProgressBar != null)
        {

            hudentry.PerfabInfo.ProgressBar.value = health / pawn.GetMaxHealth();
        }


        if (pawn.player != null)
        {
            SetFullTitle(pawn.player.PlayerName);
        }
        
    }

    public void SetPawn(Pawn pawn)
    {
        this.pawn = pawn;

    }
	
	public override void ChangeTeamColor(bool ally){
		if(teamColor){
			return;
		}
		base.ChangeTeamColor(ally);
	}
	public void HideName(){
		hudentry.label.alpha = 0.0f;
	}
	public void ShowName(){
		hudentry.label.alpha = 1.0f;
	}
}
