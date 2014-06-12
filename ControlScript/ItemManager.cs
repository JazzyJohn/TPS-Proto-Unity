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
public class FromDBAnims{
	public GameClassEnum  gameClass;
	
	public AnimType animationType;
	
	public string animationId;
	
	public Texture2D textureGUI;

	public string name;

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
	
	private int lastGameSlot;
	
	private List<GUIItem>  weaponList= new List<GUIItem>();

	public Dictionary<int,FromDBWeapon>  weaponIndexTable = new Dictionary<int,FromDBWeapon>();

	public  Dictionary<int,FromDBAnims> animsIndexTable= new Dictionary<int,FromDBAnims>();
	private string UID ="";
	
	public void Init(string uid){
			UID = uid;
			
	}


	public void ReoadItems(){
	
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		
		StartCoroutine(LoadItems (form));
	}
	public IEnumerator ReoadItemsSync(){
		
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", UID);
		
		IEnumerator numenator = LoadItems (form);
		
		while(numenator.MoveNext()){
			yield return numenator.Current;
		}

	}
	protected IEnumerator LoadItems(WWWForm form){
		Debug.Log (form );
	
		WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.LOAD_ITEMS);
		

		yield return w;

		IEnumerator numenator = ParseList (w.text);

		while(numenator.MoveNext()){
			yield return numenator.Current;
		}
	
	}
	//parse XML string to normal Achivment Pattern
	protected IEnumerator ParseList(string XML){
		//Debug.Log (XML);
	  	XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);

		int i = 0;
	
		foreach (XmlNode node in xmlDoc.SelectNodes("items/weapon")) {
			FromDBWeapon entry = new FromDBWeapon();
			entry.weaponId = int.Parse (node.SelectSingleNode ("weaponId").InnerText);
			entry.gameClass =  gameClassPase(node.SelectSingleNode ("gameClass").InnerText);
			entry.gameSlot =weaponPrefabsListbyId[entry.weaponId].slotType;
			entry.name =weaponPrefabsListbyId[entry.weaponId].weaponName;

			
			WWW www = StatisticHandler.GetMeRightWWW( node.SelectSingleNode ("textureGUIName").InnerText);
		
			yield return www;
			entry.textureGUI = new Texture2D(www.texture.width, www.texture.height);
			www.LoadImageIntoTexture(entry.textureGUI);
		//	Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
			weaponIndexTable[entry.weaponId]=entry;	
			weaponPrefabsListbyId[entry.weaponId].HUDIcon = entry.textureGUI;
			if(bool.Parse(node.SelectSingleNode ("default").InnerText)){
				switch(entry.gameClass){
					case GameClassEnum.ANY:
			
						for(int j=0;j<Choice._Personal.Length;j++){
							Choice.SetChoice((int)entry.gameSlot, j, entry.weaponId);
						}
					break;
					default:
						Choice.SetChoice((int)entry.gameSlot,(int)	entry.gameClass, entry.weaponId);
					break;

				}
			}
		}
	
	    i = 0;

		foreach (XmlNode node in xmlDoc.SelectNodes("items/anims")) {
			FromDBAnims entry = new FromDBAnims();
			entry.animationId = node.SelectSingleNode ("animationId").InnerText;
			entry.gameClass =  gameClassPase(node.SelectSingleNode ("gameClass").InnerText);
			entry.animationType =(AnimType)int.Parse(node.SelectSingleNode ("animType").InnerText);
			entry.name =node.SelectSingleNode ("name").InnerText;
			
			WWW www = StatisticHandler.GetMeRightWWW(node.SelectSingleNode ("textureGUIName").InnerText);
			
			yield return www;
			entry.textureGUI = new Texture2D(150,80);
			www.LoadImageIntoTexture(entry.textureGUI);
			//Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
			animsIndexTable[i++]=entry;	

		
		}

		foreach (XmlNode node in xmlDoc.SelectNodes("items/default")) {
			int index = int.Parse (node.SelectSingleNode ("weaponId").InnerText);
			int gameSlot =(int)weaponPrefabsListbyId[index].slotType;
			int gameClass = int.Parse (node.SelectSingleNode ("gameClass").InnerText);
			if(gameClass<=(int) GameClassEnum.ANY){
				//Debug.Log (gameSlot +" "+ gameClass +" "+index);
				Choice.SetChoice(gameSlot,gameClass,index);
			}else{
				Choice.SetChoiceRobot(gameSlot,gameClass,index);
			}
		}
		
	}	
	public void SetNewWeapon(BaseWeapon prefab){
		//Debug.Log (prefab);
		weaponPrefabsListbyId [prefab.SQLId] = prefab;
		//weaponPrefabsListbyId[prefab.SQLId].HUDIcon = weaponIndexTable[prefab.SQLId].textureGUI;
	}
	protected static GameClassEnum gameClassPase(string text){
		switch (text) {
			case "ENGINEER":
				return  GameClassEnum.ENGINEER;
				
			case "ASSAULT":
				return  GameClassEnum.ASSAULT;
				
			case "MEDIC":
				return  GameClassEnum.MEDIC;
			
			case "SCOUT":
				return  GameClassEnum.SCOUT;
				
			case "ANY":
				return  GameClassEnum.ANY;
				
			case "ROBOTHEAVY":
				return  GameClassEnum.ROBOTHEAVY;
				
			case "ROBOTMEDIUM":
				return  GameClassEnum.ROBOTMEDIUM;
				
			case "ROBOTLIGHT":
				return  GameClassEnum.ROBOTLIGHT;
				
			case "ANYROBOT":
				return  GameClassEnum.ANYROBOT;
				
				
				
		}
		return GameClassEnum.ANY;
	
	}
	public void SaveItemForSlot(){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("class", Choice._Player);
		form.AddField ("robotclass", Choice._Robot);
		for(int i = 0;i<4;i++){
		
				form.AddField ("default[]", Choice. ForGuiSlot(i));
		}
	
		for(int i = 0;i<4;i++){
		
			form.AddField ("defaultrobot[]", Choice. ForGuiSlotRobot(i));
			
		}

		StatisticHandler.instance.StartCoroutine(StatisticHandler.SendForm (form,StatisticHandler.SAVE_ITEM));
	
	}
	
	public List<GUIItem> GetItemForSlot(GameClassEnum gameClass, int gameSlot){
		if(lastGameClass==gameClass&&lastGameSlot==gameSlot&&weaponList.Count!=0){	
				return weaponList;
			}
		//Debug.Log (gameClass +"  "+ gameSlot);
			switch(gameSlot){
				//Taunt section look WeaponPlayer.cs for details
				case 5:
					return  GetAnimationForSlot( gameClass);
			
				default:
					return  GetWeaponForSlot( gameClass, (BaseWeapon.SLOTTYPE) gameSlot);
			
				
			
			
			}
					
		
	
	}
	public List<GUIItem> GetAnimationForSlot(GameClassEnum gameClass){
		
		weaponList.Clear();
		lastGameClass=gameClass;
		lastGameSlot=5;
		GameClassEnum MyANY = GameClassEnum.ANY;
		if((int)gameClass>(int) GameClassEnum.ANY){
			MyANY = GameClassEnum.ANYROBOT;
		}
		int i = 0;
		foreach(FromDBAnims entry  in animsIndexTable.Values){

			if(entry.animationType==AnimType.TAUNT&&(entry.gameClass ==MyANY||entry.gameClass==gameClass)){
				GUIItem gui = new GUIItem();
				gui.index = i++;
				gui.name= entry.name;
				gui.texture = entry.textureGUI;

				weaponList.Add(gui);
			
			}
		}
		return weaponList;
	}
	
	
	public List<GUIItem> GetWeaponForSlot(GameClassEnum gameClass, BaseWeapon.SLOTTYPE gameSlot){
		
		weaponList.Clear();
		lastGameClass=gameClass;
		lastGameSlot=(int)gameSlot;
		GameClassEnum MyANY = GameClassEnum.ANY;
		if((int)gameClass>(int) GameClassEnum.ANY){
			MyANY = GameClassEnum.ANYROBOT;
		}
		foreach(FromDBWeapon entry  in weaponIndexTable.Values){

			if(entry.gameSlot==(BaseWeapon.SLOTTYPE)lastGameSlot&&(entry.gameClass ==MyANY||entry.gameClass==gameClass)){
				GUIItem gui = new GUIItem();
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
		StartCoroutine(ParseShop(w.text));
		
		
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

		isShopLoading = false;		
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