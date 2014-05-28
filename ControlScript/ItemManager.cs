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
	
	private List<BaseWeapon>  weaponList= new List<BaseWeapon>();

	public List<FromDBWeapon>  weaponIndexTable= new List<FromDBWeapon>();
	
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
		//Debug.Log (w.text);
		ParseList (w.text);
	
	}
	//parse XML string to normal Achivment Pattern
	protected void ParseList(string XML){
	  		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		
		int i = 0;
		foreach (XmlNode node in xmlDoc.SelectNodes("items/weapon")) {
			FromDBWeapon entry = new FromDBWeapon();
			entry.weaponId = int.Parse (node.SelectSingleNode ("weaponId").InnerText);
			entry.gameClass = (GameClassEnum) int.Parse (node.SelectSingleNode ("gameClass").InnerText);
			entry.gameSlot =weaponPrefabsListbyId[entry.weaponId].slotType;
			weaponIndexTable.Add(entry);	
		}
	}
	public List<BaseWeapon> GetWeaponForSlot(GameClassEnum gameClass, BaseWeapon.SLOTTYPE gameSlot){
		if(lastGameClass==gameClass&&lastGameSlot==gameSlot){
			return weaponList;
		}
		weaponList.Clear();
		lastGameClass=gameClass;
		lastGameSlot=gameSlot;
		foreach(FromDBWeapon entry  in weaponIndexTable){
			if(entry.gameSlot==lastGameSlot&&(entry.gameClass ==GameClassEnum.ANY||entry.gameClass==gameClass)){
				weaponList.Add(weaponList[entry.weaponId]);
			
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
			WWW www = new WWW( node.SelectSingleNode ("imageurl").InnerText);

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