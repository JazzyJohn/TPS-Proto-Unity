using UnityEngine;
using System.Collections;
using System;

public class ModuleInfo : MonoBehaviour {
	public enum TypeInfo{InfoNoIco, InfoLeftIco, InfoTwoIco, InfoDescription};

	public static bool colorNoDef = false;
	public static Color32 color = new Color32(0,0,0,0);

	public bool Visable;
	public Camera uiCameraOrtogrpahic;

	public infoNoIco InfoNoIco;
	public infoDescription InfoDescription;
	public infoLeftIco InfoLeftIco;
	public infoTwoIco InfoTwoIco;
	

	// Use this for initialization
	void Start () 
	{
		InfoNoIco.defColor = InfoNoIco.Background.color;
		InfoDescription.defColor = InfoDescription.Background.color;
		InfoLeftIco.defColor = InfoLeftIco.Background.color;
		InfoTwoIco.defColor = InfoTwoIco.Background.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Post(TypeInfo Type, string[] info)
	{
		InfoNoIco.Obj.alpha = 0f;
		Visable = true;

		float x = 0f;
		float y = 0f;

		Vector3 posNoEdit = Input.mousePosition;
      
		switch(Type)
		{
		case TypeInfo.InfoNoIco: //Без иконок (заголовок и текст)
			InfoNoIco.Obj.height = 110;
			InfoNoIco.Obj.height += 50*Mathf.FloorToInt(info[1].Length/20);


			InfoNoIco.Background.color = colorNoDef ? color : InfoNoIco.defColor;

			InfoNoIco.Show(info[0], info[1]);

			FixedPosition(posNoEdit, InfoNoIco.Obj);
			break;
		case TypeInfo.InfoDescription: //Только текст
            if (InfoDescription.resize)
            {
                InfoDescription.Obj.height = 60;
                InfoDescription.Obj.height += 50 * Mathf.FloorToInt(info[0].Length / 20);
            }

			InfoDescription.Background.color = colorNoDef ? color : InfoDescription.defColor;

			InfoDescription.Show(info[0]);
            
			FixedPosition(posNoEdit, InfoDescription.Obj);
			break;
		}
	}

	public void FixedPosition(Vector3 PosMouse, UIWidget Widget)
	{

		Vector3 NewPos = uiCameraOrtogrpahic.ScreenToWorldPoint(PosMouse);


		Transform WidgetT = Widget.transform;
		NewPos.z = WidgetT.position.z;
		WidgetT.position = NewPos;

		if(PosMouse.x + Widget.width + 20f > uiCameraOrtogrpahic.pixelWidth)
		{
			WidgetT.localPosition += Vector3.left*Widget.width/2;
		}
		else
		{
			WidgetT.localPosition -= Vector3.left*Widget.width/2;
		}
		if(WidgetT.position.y - Widget.height - 20f < 0)
			WidgetT.localPosition -= Vector3.up*Widget.height/2;
		else
			WidgetT.localPosition += Vector3.up*Widget.height/2;
		//WidgetT.localPosition-=Vector3.up*2960;

		WidgetT.localPosition = new Vector3(WidgetT.localPosition.x,
		                                    WidgetT.localPosition.y,
		                                    0f);

	}

	public void Post(TypeInfo Type, string[] info, Texture[] texture)
	{
		InfoNoIco.Obj.alpha = 0f;
		Visable = true;
		
		float x = 0f;
		float y = 0f;
		
		Vector3 pos = Input.mousePosition;
		
		switch(Type)
		{
		case TypeInfo.InfoLeftIco: //Иконка слева, заголовок и текст
			InfoLeftIco.Obj.height = 140;
			InfoLeftIco.Obj.height += 50*Mathf.FloorToInt(info[1].Length/20);

			InfoLeftIco.Background.color = colorNoDef ? color : InfoLeftIco.defColor;

			FixedPosition(pos, InfoLeftIco.Obj);
			InfoLeftIco.Show(info[0], info[1], texture[0], info[2]);
			break;
		case TypeInfo.InfoTwoIco: //Иконка слева, справа, заголовок и текст
			InfoTwoIco.Obj.height = 140;
			InfoTwoIco.Obj.height += 50*Mathf.FloorToInt(info[1].Length/20);

			InfoTwoIco.Background.color = colorNoDef ? color : InfoTwoIco.defColor;
			
			FixedPosition(pos, InfoTwoIco.Obj);
			InfoTwoIco.Show(info[0], info[1], texture[0], info[2], texture[1], info[3]);
			break;
		}
	}
		
	public void DisableInfo(TypeInfo Type)
	{
		Visable = false;
		switch(Type)
		{
		case TypeInfo.InfoNoIco:
			InfoNoIco.PlayTween.Play(false);
			break;
        case TypeInfo.InfoDescription:
            InfoDescription.PlayTween.Play(false);
            break;
		}
		//colorNoDef = false;
	}
}

[Serializable]
public class infoNoIco
{
	public UIWidget Obj;
	public UIPlayTween PlayTween;
	public UISprite Background;
	public UILabel Name;
	public UILabel Text;
	[HideInInspector] public Color32 defColor;

	public void Show(string name, string text)
	{
		Name.text = name;
		Text.text = "";
		int CharNum = 0;

		for(int i = 0; i < text.Length; i++)
		{
			if(CharNum > 20)
			{
				Text.text += "\n";
				CharNum = 0;
				Text.overflowMethod = UILabel.Overflow.ResizeFreely;
			}
			Text.text += text[i];
			CharNum++;
		}

		PlayTween.Play(true);
	}
}

[Serializable]
public class infoLeftIco
{
	public UIWidget Obj;
	public UIPlayTween PlayTween;
	public UISprite Background;
	public UILabel Name;
	public UILabel Text;
	public UITexture Texture;
	public UILabel Value;
	[HideInInspector] public Color32 defColor;
	
	public void Show(string name, string text, Texture texture, string value)
	{
		Name.text = name;
		Text.text = "";
		int CharNum = 0;
		for(int i = 0; i < text.Length; i++)
		{
			if(CharNum > 20)
			{
				Text.text += "\n";
				CharNum = 0;
				Text.overflowMethod = UILabel.Overflow.ResizeFreely;
			}
			Text.text += text[i];
			CharNum++;
		}
		Texture.mainTexture = texture;
		Value.text = value;
		PlayTween.Play(true);
	}
}

[Serializable]
public class infoTwoIco
{
	public UIWidget Obj;
	public UIPlayTween PlayTween;
	public UISprite Background;
	public UILabel Name;
	public UILabel Text;
	public UITexture TextureLeft;
	public UILabel ValueLeft;
	public UITexture TextureRight;
	public UILabel ValueRight;
	[HideInInspector] public Color32 defColor;
	
	public void Show(string name, string text, Texture textureLeft,
	                 string value1, Texture textureRight , string value2)
	{
		Name.text = name;
		Text.text = "";
		int CharNum = 0;
		for(int i = 0; i < text.Length; i++)
		{
			if(CharNum > 20)
			{
				Text.text += "\n";
				CharNum = 0;
				Text.overflowMethod = UILabel.Overflow.ResizeFreely;
			}
			Text.text += text[i];
			CharNum++;
		}
		TextureLeft.mainTexture = textureLeft;
		ValueLeft.text = value1;
		TextureRight.mainTexture = textureRight;
		ValueRight.text = value2;
		PlayTween.Play(true);
	}
}

[Serializable]
public class infoDescription
{
	public UIWidget Obj;
	public UIPlayTween PlayTween;
	public UISprite Background;
	public UILabel Text;
    public bool resize = false;
	[HideInInspector] public Color32 defColor;
	
	public void Show(string text)
	{
		
        if (resize)
        {
            Text.text = "";
            int CharNum = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (CharNum > 20)
                {
                    Text.text += "\n";
                    CharNum = 0;
                   
                        Text.overflowMethod = UILabel.Overflow.ResizeFreely;
                   
                }
                Text.text += text[i];
                CharNum++;
            }

        }
        else
        {
            Text.text = text;
        }
		PlayTween.Play(true);
	}
}
