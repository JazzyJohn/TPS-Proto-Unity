using UnityEngine;
using System.Collections;

public class PointOnGuiComponent : ShowOnGuiComponent
{
    public string[] aTeamSprites;
    public string[] bTeamSprites;
	
	public Color neutral =Color.grey;

    public void SetTitle(string text, int team,int conquerTeam)
    {
        base.SetTitle(text, team);

        if (hudentry == null)
        {
            return;
        }

        if (hudentry.Sprite != null)
        {
            string[] teamSprites;
            if (Player.localPlayer.team == 1)
            {
                teamSprites = aTeamSprites;
            }else{
                teamSprites = bTeamSprites;
            }
            if (teamSprites.Length > team)
            {
                string sprite = teamSprites[team];
              

                if (hudentry.Sprite.spriteName != sprite)
                {
                  
                    hudentry.Sprite.spriteName = sprite;
                    hudentry.defSpriteName = sprite;
                }
            }

        }
		
		if (conquerTeam ==0 )
		{
			if(hudentry.label!=null){
				hudentry.label.color = neutral;
			}
			
		}
		else if (conquerTeam ==Player.localPlayer.team )
		{
			if(hudentry.label!=null){
				hudentry.label.color = ally;
			}
		}else{
			if(hudentry.label!=null){
				hudentry.label.color = enemy;
			}
		
		}
    }
		
}
