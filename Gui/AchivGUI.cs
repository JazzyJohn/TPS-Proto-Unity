using UnityEngine;
using System.Collections;

public class AchivGUI : MonoBehaviour {

	public enum type{Achiv, Dailic};
	public type Type;

	public int numObj;

    public StatisticGUI Main;
	public UIWidget Widget;
	public UILabel nameAndDescription;
    public UILabel progress;
	public UITexture picture;

	public void GetInfo(Achievement achivment)
	{



        nameAndDescription.text = achivment.description;
        if (achivment.isMultiplie)
        {
            progress.text = achivment.GetProgress();
            //
        }
       
        picture.mainTexture = achivment.textureIcon;
		Main.statistic.GetParent(this);
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
