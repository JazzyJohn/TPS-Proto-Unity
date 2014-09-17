using UnityEngine;
using System.Collections;

public class ShopItemGUI : MonoBehaviour {

	public ShopGUI Shop;
	public string id = "";
	public UILabel Name;
	public UILabel PraceKP;
	public UILabel PraceGITP;
	public UILabel Opisanie;
	public UITexture Textura;
	public UIWidget Box;

	[HideInInspector]
	public int numToItem;

	// Use this for initialization
	void Start () {
	
	}

	public void LoadInfo()
	{
		//Код загрузки инфы из xml
	}

	// Update is called once per frame
	void Update () {
	
	}
}
