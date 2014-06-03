using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewsDrawer : MonoBehaviour {
	List<NewUpdate> newsList = null;
	private int currentPage;
	private NewUpdate currentNew;

	public void SetPage(int p)
	{
		currentPage = p;
		
		if (newsList.Count <= 0)
			return;
		
		currentNew = newsList[p];
	}

	public void NextPage()
	{
		if (currentPage + 1 >= newsList.Count)
			return;
		
		int np = currentPage + 1;
		SetPage (np);
	}
	
	public void PrevPage()
	{
		if (currentPage <= 0)
			return;
		
		int np = currentPage - 1;
		SetPage (np);
	}
	
	// Use this for initialization
	void Start () 
	{
		currentNew = null;
		currentPage = 0;

		NewsManager.instance.getUpdate ();
	}

	void OnGUI()
	{
		Draw ();
	}

	void Draw()
	{
		DrawUpperBar();

		if (newsList == null)
			return;
	
		DrawNew();
	}

	void DrawUpperBar()
	{
		//TODO: draw control panel
		if (GUI.Button (new Rect (0, 0, 32, 32), "<")) 
		{
			PrevPage();
		}
		if (GUI.Button (new Rect (32, 0, 32, 32), ">")) 
		{
			NextPage();
		}
		if (GUI.Button (new Rect (64, 0, 128, 32), "Update")) 
		{
			NewsManager.instance.getUpdate();
		}
	}

	void DrawNew()
	{
		if (currentNew == null || !currentNew.img_tex) 
			return;
		
		Rect newRect = new Rect (0, 32, 256,256);
		float text_x,text_y;
		text_x = newRect.x + currentNew.title_x;
		text_y = newRect.y + currentNew.title_y;
		
		GUI.DrawTexture(newRect, currentNew.img_tex, ScaleMode.ScaleToFit, true);
		GUI.Label (new Rect (text_x, text_y, newRect.width, newRect.height), currentNew.title);
	}

	// Update is called once per frame
	void Update () 
	{
		newsList = NewsManager.instance.getNewsList ();

		if (currentNew == null && newsList.Count > 0)
			currentNew = newsList [0];
	}
}
