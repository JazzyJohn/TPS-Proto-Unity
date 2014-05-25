using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;



public class FromDBWeapon{
	//I don't see use in some enormous key tables dictionary so weaponId is just index in weaponList.
	public int weaponId;
	
	public GameClassEnum  gameClass;
	
	public BaseWeapon.SLOTTYPE gameSlot;

	public Texture2D textureGUI;

	public string name;
}
public struct DefaultItemSettings{

	public int mainIndex;
	 
	public Texture2D mainTexture;

	public int personalIndex;
	
	public Texture2D personalTexture;

	public int extraIndex;

	public Texture2D extraTexture;

}

public class ShopSlot{
	public string name;
	
	public int id;
	
	public string description;
	
	public Texture2D texture;
	
	public int cashcost;
	
	public int goldcost;
	
	public GameClassEnum[] gameClasses;
}

public class ItemManager : MonoBehaviour {

	//PLAYER ITEM SECTION
	public BaseWeapon[] weaponPrefabsListbyId;
	
	private GameClassEnum lastGameClass;
	
	private BaseWeapon.SLOTTYPE lastGameSlot;
	
	private List<GUIWeapon>  weaponList= new List<GUIWeapon>();

	public FromDBWeapon[]  weaponIndexTable;

	public DefaultItemSettings[] defSettings= new DefaultItemSettings[4];
	
	private string UID ;
	
	public void Init(string uid){
			UID = uid;
			ReoadItems();
	}


	public void ReoadItems(){
	
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		
		StartCoroutine(LoadItems (form));
	}
	protected IEnumerator LoadItems(WWWForm form){
		Debug.Log (form );
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ITEMS);
			w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.LOAD_ITEMS, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ITEMS);
			w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ITEMS, form);
		}

		yield return w;

		StartCoroutine(ParseList (w.text));
	
	}
	//parse XML string to normal Achivment Pattern
	protected IEnumerator ParseList(string XML){
	  	XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		//Debug.Log (XML);
		int i = 0;
		weaponIndexTable = new FromDBWeapon[xmlDoc.SelectNodes ("items/weapon").Count];
		foreach (XmlNode node in xmlDoc.SelectNodes("items/weapon")) {
			FromDBWeapon entry = new FromDBWeapon();
			entry.weaponId = int.Parse (node.SelectSingleNode ("weaponId").InnerText);
			entry.gameClass =  gameClassPase(node.SelectSingleNode ("gameClass").InnerText);
			entry.gameSlot =weaponPrefabsListbyId[entry.weaponId].slotType;
			entry.name =weaponPrefabsListbyId[entry.weaponId].weaponName;

			WWW www = null;
			if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
				
				//Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
				www = new WWW (StatisticHandler.STATISTIC_PHP + node.SelectSingleNode ("textureGUIName").InnerText);
			}
			else{
				//Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
				www = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
			}
			
			yield return www;
			entry.textureGUI = new Texture2D(150,80);
			www.LoadImageIntoTexture(entry.textureGUI);
			//Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
			weaponIndexTable[entry.weaponId]=entry;	

			if(bool.Parse(node.SelectSingleNode ("default").InnerText)){
				switch(entry.gameSlot){
					case BaseWeapon.SLOTTYPE.ANTITANK:
					for(int j=0;j<defSettings.Length;j++){
						defSettings[j].extraTexture = entry.textureGUI;
						defSettings[j].extraIndex = entry.weaponId;
					}
					break;
					case BaseWeapon.SLOTTYPE.PERSONAL:
					for(int j=0;j<defSettings.Length;j++){
						defSettings[j].personalTexture = entry.textureGUI;
						defSettings[j].personalIndex = entry.weaponId;
					}
					break;
					case BaseWeapon.SLOTTYPE.MAIN:
				
						defSettings[(int)entry.gameClass].mainTexture = entry.textureGUI;
						defSettings[(int)entry.gameClass].mainIndex = entry.weaponId;

					break;

				}
			}
		}
	}	

	protected static GameClassEnum gameClassPase(string text){
		switch (text) {
			case "ENGINEER":
				return  GameClassEnum.ENGINEER;
				break;
			case "ASSAULT":
				return  GameClassEnum.ASSAULT;
				break;
			case "MEDIC":
				return  GameClassEnum.MEDIC;
				break;
			case "SCOUT":
				return  GameClassEnum.SCOUT;
				break;
			case "ANY":
				return  GameClassEnum.ANY;
				break;
				
		}
		return GameClassEnum.ANY;
	
	}
	public List<GUIWeapon> GetWeaponForSlot(GameClassEnum gameClass, BaseWeapon.SLOTTYPE gameSlot){
		if(lastGameClass==gameClass&&lastGameSlot==gameSlot){	
			return weaponList;
		}
		weaponList.Clear();
		lastGameClass=gameClass;
		lastGameSlot=gameSlot;
		foreach(FromDBWeapon entry  in weaponIndexTable){

			if(entry.gameSlot==lastGameSlot&&(entry.gameClass ==GameClassEnum.ANY||entry.gameClass==gameClass)){
				GUIWeapon gui = new GUIWeapon();
				gui.index = entry.weaponId;
				gui.name= entry.name;
				gui.texture = entry.textureGUI;

				weaponList.Add(gui);
			
			}
		}
		return weaponList;
	}
	
	//ENDPLAYER ITEM SECTION
	
	
	//SHOPING SECTION 
	public bool isShopLoading;

	public List<ShopSlot> shopItems = new List<ShopSlot> ();

	public IEnumerator  LoadShop(int type=-1){

		isShopLoading = true;
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		if(type==-1){
			form.AddField ("main", "true");
		}else{
			form.AddField ("type", type);
		}
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_SHOP);
			w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.LOAD_SHOP, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ITEMS);
			w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_SHOP, form);
		}

		yield return w;
		ParseShop(w.text);
		
		
		isShopLoading = false;
	}
	public IEnumerator  ParseShop(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		shopItems.Clear();
		foreach (XmlNode node in xmlDoc.SelectNodes("items/item")) {
			ShopSlot slot = new ShopSlot();
			slot.name = node.SelectSingleNode ("name").InnerText;
			slot.id = int.Parse(node.SelectSingleNode ("id").InnerText);
			slot.cashcost = int.Parse(node.SelectSingleNode ("cashcost").InnerText);
			slot.goldcost = int.Parse(node.SelectSingleNode ("goldcost").InnerText);
			slot.gameClasses = new GameClassEnum[node.SelectNodes("items/weapon").Count];
			int i=0;
			foreach (XmlNode gameClass in node.SelectNodes("class")) {
					slot.gameClasses[i++]=	(GameClassEnum)int.Parse(gameClass.InnerText);			
			}

			WWW www = null;
			if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
				
				Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_SHOP);
				www = new WWW (StatisticHandler.STATISTIC_PHP +  node.SelectSingleNode ("imageurl").InnerText);
			}
			else{
				Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ITEMS);
				www = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("imageurl").InnerText);
			}



			// wait until the download is done
			yield return www;

			// assign the downloaded image to the texture of the slot
			www.LoadImageIntoTexture(slot.texture);
			shopItems.Add(slot);
		}		
	}
	
	public IEnumerator  BuyItem(int itemId,bool forGold){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("game_item", itemId);
		form.AddField ("forGold", forGold.ToString());
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.BUY_ITEM);
			w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.BUY_ITEM, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.BUY_ITEM);
			w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.BUY_ITEM, form);
		}

		yield return w;
		
		
	}
	//END SHOPING SECTION 
	private static ItemManager s_Instance = null;
	
	public static ItemManager instance {
		get {
			if (s_Instance == null) {
			
				s_Instance =  FindObjectOfType(typeof (ItemManager)) as ItemManager;
			}
			
			if (s_Instance == null) {
				GameObject obj = new GameObject("ItemManager");
				s_Instance = obj.AddComponent(typeof (ItemManager)) as ItemManager;
			
			}
			
			return s_Instance;
		}
	}

	
}