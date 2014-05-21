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
public class ItemManager : MonoBehaviour {
	public BaseWeapon[] weaponPrefabsListbyId;
	
	private GameClassEnum lastGameClass;
	
	private BaseWeapon.SLOTTYPE LastGameSlot;
	
	public List<BaseWeapon>  weaponList= new List<BaseWeapon>();

	public List<FromDBWeapon>  weaponIndexTable= new List<FromDBWeapon>();
	
	private int UID ;
	
	public void Init(string uid){
			UID = uid;
			ReoadItems();
	}
	public void ReoadItems(){
	
		var form = new WWWForm ();
			
		form.AddField ("uid", uid);
		
		StartCoroutine(LoadItems (form));
	}
	protected IEnumerator LoadLvling(WWWForm form){
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
			entry.gameClass = (GameClassEnum) int.Parse (node.SelectSingleNode ("gameClass").InnerText));
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
			if(entry.gameSlot==lastGameSlot&&(entry.gameClass ==GameClassEnum.Any||entry.gameClass==gameClass)){
				weaponList.add(weaponList[entry.weaponId]);
			
			}
		}
		return weaponList;
	}
	
		
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