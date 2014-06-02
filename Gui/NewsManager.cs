using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class NewUpdate
{
	public string title;
	public int title_x,title_y;
	public string img;//for?
	public Texture2D img_tex;
}

public class NewsManager : MonoBehaviour {
	string ImagesPath = "http://vk.rakgames.ru/kaspi/";
	string XMLPath = "http://vk.rakgames.ru/kaspi/unityTest/XMLExample.xml";
	List <NewUpdate> news = new List<NewUpdate> ();
	public static NewsManager instance;//singletone?
	private int currentPage;
	private NewUpdate currentNew;

	public void SetPage(int p)
	{
		currentPage = p;
		
		if (news.Count <= 0)
			return;
		
		currentNew = news[p];
	}
	
	public void NextPage()
	{
		if (currentPage + 1 >= news.Count)
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
	
	void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	void Start () 
	{
		currentNew = null;
		currentPage = 0;

		//TODO: move it to another step;
		StartCoroutine(LoadNews ());
	}
	
	void OnGUI()
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
			StartCoroutine(LoadNews());
		}

		//TODO: draw selected new.
		if (currentNew == null || !currentNew.img_tex) 
		{
			return;
		}

		Rect newRect = new Rect (0, 32, 256,256);
		float text_x,text_y;
		text_x = newRect.x + currentNew.title_x;
		text_y = newRect.y + currentNew.title_y;

		GUI.DrawTexture(newRect, currentNew.img_tex, ScaleMode.ScaleToFit, true);
		GUI.Label (new Rect (text_x, text_y, newRect.width, newRect.height), currentNew.title);
	}
	
	protected IEnumerator LoadNews(){
		WWW w = null;

		//FIXME: As I know WWWForm is analog for <form> in html
		if (string.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("NEWS HTTP SEND" + XMLPath);
			w = new WWW (XMLPath);
		}
		else{
			Debug.Log ("NEWS HTTPS SEND"+ XMLPath);
			w = new WWW (XMLPath);
		}
		yield return w;

		ParseList (w.text);
	}
	
	protected IEnumerator LoadImage(string img,NewUpdate New)
	{
		string imagePath = ImagesPath + img;
		
		WWW w = new WWW (imagePath);
		yield return w;
		
		New.img = imagePath;
		New.img_tex = w.texture;
	}
	
	//parse XML string to normal Achivment Pattern
	protected void ParseList(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		
		foreach (XmlNode node in xmlDoc.SelectNodes("allnews/new")) {
			NewUpdate n = new NewUpdate();
			n.title = node.SelectSingleNode("title").InnerText;
			n.title_x = int.Parse(node.SelectSingleNode("titleX").InnerText);
			n.title_y = int.Parse(node.SelectSingleNode("titleY").InnerText);
			n.img = node.SelectSingleNode("img").InnerText;
			StartCoroutine(LoadImage(n.img,n));
			news.Add(n);
		}
		
		SetPage(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
