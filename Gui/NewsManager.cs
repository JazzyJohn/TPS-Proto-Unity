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

	public List<NewUpdate> getNewsList()
	{
		return news;
	}

	void Awake()
	{
		instance = this;
	}

	public void getUpdate()
	{
		news.Clear ();
		StartCoroutine(LoadNews ());
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
	}
}
