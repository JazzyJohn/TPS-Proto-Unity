using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
[System.Serializable]
public class NewUpdate
{
	public string title;
	public float title_x,title_y;
	public int fontSize;
	public string color;
	public string img;//for?
	public Texture2D img_tex;
}

public class NewsManager : MonoBehaviour {
	string ImagesPath = "http://vk.rakgames.ru/kaspi/";
	string XMLPath = "http://vk.rakgames.ru/kaspi/unityTest/XMLExample.xml";
	private List <NewUpdate> news = new List<NewUpdate> ();
	public static NewsManager instance;//singletone?
	public bool finished = false;
	public List<NewUpdate> getNewsList()
	{
		return news;
	}

	void Awake()
	{
		instance = this;
		getUpdate ();
	}

	public void getUpdate()
	{
		news.Clear ();
		StartCoroutine(LoadNews ());
	}
	
	protected IEnumerator LoadNews(){
		WWW w = StatisticHandler.GetMeRightWWW (StatisticHandler.NEWS_ALL);

		//FIXME: As I know WWWForm is analog for <form> in html
		/*if (string.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("NEWS HTTP SEND" + XMLPath);
			w = new WWW ();
		}
		else{
			Debug.Log ("NEWS HTTPS SEND"+ XMLPath);
			w = new WWW (XMLPath);
		}*/
		yield return w;

		IEnumerator numenator = ParseList (w.text);
		
		while(numenator.MoveNext()){
			yield return numenator.Current;
		}
	}
	

	
	//parse XML string to normal Achivment Pattern
	protected IEnumerator ParseList(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		
		foreach (XmlNode node in xmlDoc.SelectNodes("allnews/new")) {
			NewUpdate n = new NewUpdate();
			n.title = node.SelectSingleNode("title").InnerText;
			n.title_x = float.Parse(node.SelectSingleNode("titleX").InnerText);
			n.title_y = float.Parse(node.SelectSingleNode("titleY").InnerText);
			n.fontSize = int.Parse(node.SelectSingleNode("fontsize").InnerText);
			n.color = node.SelectSingleNode("color").InnerText;
			n.img = node.SelectSingleNode("img").InnerText;
		
			WWW w = StatisticHandler.GetMeRightWWW (n.img);
			yield return w;
			
			n.img_tex = new Texture2D(w.texture.width, w.texture.height);
			w.LoadImageIntoTexture(n.img_tex);
			
			news.Add(n);
		}
		finished = true;
	}
}
