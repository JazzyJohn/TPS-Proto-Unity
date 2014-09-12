using UnityEngine;
using System.Collections;

public class PawnOnGuiComponent : ShowOnGuiComponent {

    public float startWidth=100.0f;

    public float sizeCoef=1.0f;

    Pawn pawn;
    public override void LocalPlayerSeeMe(float distance,int team, bool state){
    
        if(hudentry==null){
            return;
        }
        float width = startWidth -distance*sizeCoef;
        Debug.Log((int)width);
        hudentry.PerfabInfo.Sprite.width = (int)width;
        if (pawn.team != team && pawn.team!=0)
        {
            if (hudentry.PerfabInfo.ProgressBar.alpha != 0)
            {
                hudentry.PerfabInfo.ProgressBar.alpha = 0;
            }
        }
        
        isShow = state;


    }
    public void UpdateGui()
    {
        if (hudentry == null)
        {
            return;
        }
        float health = pawn.health;
        if (health<0)
        {
            health = 0;
        }
       
        hudentry.PerfabInfo.ProgressBar.value = health / pawn.GetMaxHealth();



        
    }

    public void SetPawn(Pawn pawn)
    {
        this.pawn = pawn;
    }
}
