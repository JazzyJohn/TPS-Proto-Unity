using UnityEngine;
using System.Collections;

public class AchivGUI : MonoBehaviour {

	public enum type{Achiv, Dailic};
	public type Type;

	public int numObj;

	InventoryGUI Main;
	public UIWidget Widget;
	public UILabel NameAndDescription;
	public UITexture picture;

	public IEnumerator GetInfo()
	{
		return null; //Вставить получение

		switch(Type)
		{
		case type.Achiv:
			//NameAndDescription.text = Name;
			break;
		case type.Dailic:
			//NameAndDescription.text = Name+"\n"+Progres;
			break;
		}
		//picture.mainTexture = texture;
		Main.Statistic.GetParent(this);
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
