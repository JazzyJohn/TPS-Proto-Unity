using UnityEngine;
using System.Collections;

public class ModuleInfoPost : MonoBehaviour {

	public ModuleInfo MainModule;
	public ModuleInfo.TypeInfo Type;

	public string Name;
	public string Text;
	public Texture IcoLeft;
	public string IcoLeftValue;
	public Texture IcoRight;
	public string IcoRightValue;

	// Use this for initialization
	void Start () 
	{
		if(!MainModule)
			MainModule = Transform.FindObjectOfType<ModuleInfo>();
	}

	void OnHover(bool isOver)
	{
		if(isOver)
		{
			if(!MainModule.Visable)
			{
				string[] info = new string[4];
				Texture[] texture = new Texture[2];
				switch(Type)
				{
				case ModuleInfo.TypeInfo.InfoNoIco:
					info[0] = Name;
					info[1] = Text;
					MainModule.Post(Type, info);
					break;
				case ModuleInfo.TypeInfo.InfoDescription:
					info[0] = Text;
					MainModule.Post(Type, info);
					break;
				case ModuleInfo.TypeInfo.InfoLeftIco:
					info[0] = Name;
					info[1] = Text;
					texture[0] = IcoLeft;
					info[2] = IcoLeftValue;
					MainModule.Post(Type, info, texture);
					break;
				case ModuleInfo.TypeInfo.InfoTwoIco:
					info[0] = Name;
					info[1] = Text;
					texture[0] = IcoLeft;
					info[2] = IcoLeftValue;
					texture[1] = IcoRight;
					info[3] = IcoRightValue;
					MainModule.Post(Type, info, texture);
					break;
				}
			}
		}
		else
		{
			if(MainModule.Visable)
			{
				MainModule.DisableInfo(Type);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
