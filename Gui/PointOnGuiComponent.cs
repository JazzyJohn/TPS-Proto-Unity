using UnityEngine;
using System.Collections;

public class PointOnGuiComponent : ShowOnGuiComponent
{
    public string[] teamSprites;

    public override void SetTitle(string text, int team)
    {
        base.SetTitle(text, team);
        if (hudentry.Sprite != null)
        {
            if (teamSprites.Length > team)
            {
                string sprite = teamSprites[team];
              

                if (hudentry.Sprite.spriteName != sprite)
                {
                    Debug.Log(team + " " + sprite);
                    hudentry.Sprite.spriteName = sprite;
                    hudentry.defSpriteName = sprite;
                }
            }

        }
    }
    public override void ChangeTeamColor(bool isAlly)
    {
        if (!allyDipendColor)
        {
            return;
        }
        if (isAlly)
        {
            if (hudentry.label != null)
            {
                hudentry.label.color = ally;
            }
         
        }
        else
        {
            if (hudentry.label != null)
            {
                hudentry.label.color = enemy;
            }
           
        }
    }
}
