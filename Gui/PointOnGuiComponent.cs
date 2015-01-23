using UnityEngine;
using System.Collections;

public class PointOnGuiComponent : ShowOnGuiComponent
{
    public string[] teamSprites;
	
	public Color neutral =Color.grey;

    public void SetTitle(string text, int team,int conquerTeam)
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
		
		if (conquerTeam ==0 )
		{
			if(hudentry.label!=null){
				hudentry.label.color = neutral;
			}
			
		}
		else if (conquerTeam ==Playr.localPlayer.team )
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
