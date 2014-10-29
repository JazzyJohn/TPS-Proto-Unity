using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowOnGuiComponent : MonoBehaviour {
	
	public static List<ShowOnGuiComponent> allShowOnGui  = new List<ShowOnGuiComponent>();
	
	public int team =0;
	
	public Transform myTransform;
	
	public string baseTitle;

	private string title;
	
	public string spriteName;

    public string prefabName = "Standart(sprite and text)";

    public HUDText.Entry hudentry;

    public bool withArrow = true;
	
	public bool isShow= true;

    public bool showAnotherTeam = true;

    public bool alwaysFriendly = false;
	
	public bool hideInClose = false;
	
	public float distanceHide = 10.0f;
	
	public bool allyDipendColor;
	
	public Color ally =Color.green;
	
	public Color enemy =Color.red;
	
	public void Awake(){
		myTransform = transform;
		allShowOnGui.Add(this);
        title = baseTitle + " " ;
	}
	void OnDestroy(){
		allShowOnGui.Remove(this);
        if (PlayerMainGui.instance != null)
        {
            PlayerMainGui.instance.RemoveMessage(hudentry);
        }
	}
    public bool IsShow(Transform mainCamera, int inteam)
    { 
		bool basicShow = isShow && (showAnotherTeam || team==0||inteam==team);
        Debug.Log("ShowOnGuiComponent" + isShow + " " + team);
        bool addvanceShow = !hideInClose || (hideInClose && (mainCamera.position - myTransform.position).sqrMagnitude > distanceHide * distanceHide);
		return basicShow&&addvanceShow;
	}
	public void SetTitle(string text){
		title = baseTitle +" " +text;
	}
	public void SetTitle(string text,int team){
		title = baseTitle +" " +text;
		this.team =team;
	}
	public string GetTitle(){
		return title;
	}

	public void Show(){
		isShow = true;
	}
	public void Hide(){
		isShow = false;
	}
	public virtual void LocalPlayerSeeMe(float distance,int team,bool state){
			
	}
	public virtual void ChangeTeamColor(bool ally){
		if(!allyDipendColor){
			return;
		}
		if (ally)
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
	}

}