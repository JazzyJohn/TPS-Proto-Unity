using UnityEngine;
using System.Collections;
using System;

public class ModuleInfo : MonoBehaviour {
	public enum TypeInfo{InfoNoIco, InfoLeftIco, InfoTwoIco, InfoDescription};

	public bool Visable;
	public Camera uiCameraOrtogrpahic;

	public infoNoIco InfoNoIco;
	public infoDescription InfoDescription;
	public infoLeftIco InfoLeftIco;
	public infoTwoIco InfoTwoIco;

	// Use this for initialization
	void Start () {
	
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

		Vector3 pos = Input.mousePosition;

		pos = uiCameraOrtogrpahic.ScreenToWorldPoint(pos);
		pos.z = 0f;
		
		switch(Type)
		{
		case TypeInfo.InfoNoIco: //Без иконок (заголовок и текст)
			InfoNoIco.Obj.height = 110;
			InfoNoIco.Obj.height += 50*Mathf.FloorToInt(info[1].Length/20);

			if(Input.mousePosition.x + (InfoNoIco.Obj.width/2) > uiCameraOrtogrpahic.pixelWidth)
			{
				if(Input.mousePosition.y - (InfoNoIco.Obj.height/2) < 0)
					InfoNoIco.Obj.pivot = UIWidget.Pivot.BottomRight;
				else
					InfoNoIco.Obj.pivot = UIWidget.Pivot.TopRight;
			}
			else
			{
				if(Input.mousePosition.y - (InfoNoIco.Obj.height/2) < 0)
					InfoNoIco.Obj.pivot = UIWidget.Pivot.BottomLeft;
				else
					InfoNoIco.Obj.pivot = UIWidget.Pivot.TopLeft;
			}

			InfoNoIco.Obj.transform.position = pos;
			InfoNoIco.Show(info[0], info[1]);
			break;
		case TypeInfo.InfoDescription: //Только текст
			InfoDescription.Obj.height = 60;
			InfoDescription.Obj.height += 50*Mathf.FloorToInt(info[0].Length/20);

			if(Input.mousePosition.x + (InfoDescription.Obj.width/2) > uiCameraOrtogrpahic.pixelWidth)
			{
				if(Input.mousePosition.y - (InfoDescription.Obj.height/2) < 0)
					InfoDescription.Obj.pivot = UIWidget.Pivot.BottomRight;
				else
					InfoDescription.Obj.pivot = UIWidget.Pivot.TopRight;
			}
			else
			{
				if(Input.mousePosition.y - (InfoDescription.Obj.height/2) < 0)
					InfoDescription.Obj.pivot = UIWidget.Pivot.BottomLeft;
				else
					InfoDescription.Obj.pivot = UIWidget.Pivot.TopLeft;
			}
			
			InfoDescription.Obj.transform.position = pos;
			InfoDescription.Show(info[0]);
			break;
		}
	}

	public void Post(TypeInfo Type, string[] info, Texture[] texture)
	{
		InfoNoIco.Obj.alpha = 0f;
		Visable = true;
		
		float x = 0f;
		float y = 0f;
		
		Vector3 pos = Input.mousePosition;
		
		pos = uiCameraOrtogrpahic.ScreenToWorldPoint(pos);
		pos.z = 0f;
		
		switch(Type)
		{
		case TypeInfo.InfoLeftIco: //Иконка слева, заголовок и текст
			InfoLeftIco.Obj.height = 140;
			InfoLeftIco.Obj.height += 50*Mathf.FloorToInt(info[1].Length/20);

			if(Input.mousePosition.x + (InfoLeftIco.Obj.width/2) > uiCameraOrtogrpahic.pixelWidth)
			{
				if(Input.mousePosition.y - (InfoLeftIco.Obj.height/2) < 0)
					InfoLeftIco.Obj.pivot = UIWidget.Pivot.BottomRight;
				else
					InfoLeftIco.Obj.pivot = UIWidget.Pivot.TopRight;
			}
			else
			{
				if(Input.mousePosition.y - (InfoLeftIco.Obj.height/2) < 0)
					InfoLeftIco.Obj.pivot = UIWidget.Pivot.BottomLeft;
				else
					InfoLeftIco.Obj.pivot = UIWidget.Pivot.TopLeft;
			}
			
			InfoLeftIco.Obj.transform.position = pos;
			InfoLeftIco.Show(info[0], info[1], texture[0], info[2]);
			break;
		case TypeInfo.InfoTwoIco: //Иконка слева, справа, заголовок и текст
			InfoTwoIco.Obj.height = 140;
			InfoTwoIco.Obj.height += 50*Mathf.FloorToInt(info[1].Length/20);

			if(Input.mousePosition.x + (InfoTwoIco.Obj.width/2) > uiCameraOrtogrpahic.pixelWidth)
			{
				if(Input.mousePosition.y - (InfoTwoIco.Obj.height/2) < 0)
					InfoTwoIco.Obj.pivot = UIWidget.Pivot.BottomRight;
				else
					InfoTwoIco.Obj.pivot = UIWidget.Pivot.TopRight;
			}
			else
			{
				if(Input.mousePosition.y - (InfoTwoIco.Obj.height/2) < 0)
					InfoTwoIco.Obj.pivot = UIWidget.Pivot.BottomLeft;
				else
					InfoTwoIco.Obj.pivot = UIWidget.Pivot.TopLeft;
			}
			
			InfoTwoIco.Obj.transform.position = pos;
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
		}
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
	
	public void Show(string text)
	{
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
