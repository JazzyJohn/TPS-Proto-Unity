using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
public struct WeaponIndex
{
    public int prefabId;
    public string itemId;
    public int gameClass;
    public static WeaponIndex Zero = new WeaponIndex(-1,"");
    public const char INDEX_SPLINTER = '_';
    public WeaponIndex(int weaponId, string itemId)
    {
        this.prefabId = weaponId;
        this.itemId = itemId;
        gameClass = 0;
    }
    public WeaponIndex(string index)
    {
        string[] indexs = index.Split(INDEX_SPLINTER);
        if (indexs.Length < 2)
        {
            this.prefabId = -1;
            this.itemId = "";
        }
        else
        {
            this.prefabId = int.Parse(indexs[0]);
            this.itemId = indexs[1];
        }
        gameClass = 0;
    }
    public override string ToString()
    {
        if (this.prefabId == -1)
        {
            return "-1";
        }
        else
        {
            return this.prefabId.ToString() + INDEX_SPLINTER + this.itemId;
        }
       
    }
    public bool IsSameIndex(WeaponIndex index){
        if (index.prefabId == -1)
        {
            return index.prefabId == prefabId;
        }
        else
        {
            return index.prefabId == prefabId && itemId == index.itemId;
        }
    }
}

public class FromDBWeapon{
	//I don't see use in some enormous key tables dictionary so weaponId is just index in weaponList.
	public int weaponId;
	
	public GameClassEnum  gameClass;
	
	public BaseWeapon.SLOTTYPE gameSlot;

	public Texture2D textureGUI;

	public string name;
	
	public int group;

}
public class FromDBAnims{
	public GameClassEnum  gameClass;
	
	public AnimType animationType;
	
	public string animationId;
	
	public Texture2D textureGUI;

	public string name;

	
}
public enum ShopSlotType
{
    WEAPON, ARMOR, ETC
}
public class SimpleSlot{
 public GameClassEnum gameClass;

	public string name;
	
	public string id;
	
	public string description;
	
	public Texture2D texture;

    public GameObject loadModel;

    public string shopicon;

    public string model;
	
	public ShopSlotType type;
}

public class ShopSlot : SimpleSlot{
   
	
	public string cashSlot;

    public string goldSlot;

    public int cashCost;

    public int goldCost;
	
	
}
public class SmallShopData{

	public string cashSlot;

    public string goldSlot;
	
	public string name;
	
    public int amount;

    public string itemId;
	
	public Texture2D textureGUI;

}
public class InventorySlot  : SimpleSlot{
   public bool personal;
   
   public int charge;
   
   public DateTime timeEnd;
   
   public int modslot;
   
   public int ingamekey;
   
   public int maxcharge;
   
   public int gameId;

   public int gameType;
   
   public int group;
   
 }
public class StimPack{
	public int amount;

   	public bool active = false;

    public int buffId;
	
	public Texture2D textureGUI;

	public string name;

    public int mysqlId;
}

public class Buff
{

    public List<CharacteristicToAdd> listOfEffect = new List<CharacteristicToAdd>();
	
}
public class ItemManager : MonoBehaviour {

	//PLAYER ITEM SECTION
	public BaseWeapon[] weaponPrefabsListbyId;



	public Dictionary<int,FromDBWeapon>  weaponIndexTable = new Dictionary<int,FromDBWeapon>();
	
	public  Dictionary<int,FromDBAnims> animsIndexTable= new Dictionary<int,FromDBAnims>();

    public List<WeaponIndex> cachedIndex = new List<WeaponIndex>();
	public StimPack[] stimPackDictionary;

    public Dictionary<int, Buff> allBuff = new Dictionary<int, Buff>();
	private string UID ="";
	
	public void Init(string uid){
			UID = uid;
            WWWForm form = new WWWForm();

            form.AddField("uid", UID);

            StartCoroutine(LoadItems(form));
	}


    public void ConnectToPrefab()
    {

       
        foreach (FromDBWeapon weapon in weaponIndexTable.Values)
        {
            weapon.gameSlot = weaponPrefabsListbyId[weapon.weaponId].slotType;
            weapon.name = weaponPrefabsListbyId[weapon.weaponId].weaponName;
            weaponPrefabsListbyId[weapon.weaponId].HUDIcon = weapon.textureGUI;
        }
        if (invItems.ContainsKey(ShopSlotType.WEAPON))
        {
            foreach (InventorySlot weapon in invItems[ShopSlotType.WEAPON])
            {
                weapon.gameType = (int)weaponPrefabsListbyId[weapon.ingamekey].slotType;
                weaponPrefabsListbyId[weapon.ingamekey].HUDIcon = weapon.texture;
            }

        }
        foreach (WeaponIndex index in cachedIndex)
        {
           
            if (index.prefabId != -1)
            {
                int gameSlot = (int)weaponPrefabsListbyId[index.prefabId].slotType;
                Choice.SetChoice(gameSlot, index.gameClass, index);
                   
            }
        }
        List<InventorySlot> toDelete = new List<InventorySlot>();
        foreach (KeyValuePair<string, InventorySlot> slot in allitems)
        {
            switch (slot.Value.type)
            {
                case ShopSlotType.ETC:
                    if (slot.Value.charge <= 0)
                    {
                        toDelete.Add(slot.Value);
                    }
                    break;
                default:
                    if (slot.Value.personal)
                    {
                        if (slot.Value.charge <= 0)
                        {
                            toDelete.Add(slot.Value);
                        }
                    }
                    else
                    {
                        if (slot.Value.timeEnd !=null&&slot.Value.timeEnd < DateTime.Now)
                        {
                            toDelete.Add(slot.Value);
                        }
                    }
                    break;

            }
        }
        foreach (InventorySlot slot in toDelete)
        {
            allitems.Remove(slot.id);
            invItems[slot.type].Remove(slot);
        }
       
    }
	public IEnumerator ReLoadItemsSync(){
		
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
			entry.group =int.Parse (node.SelectSingleNode ("ingametype").InnerText);
			
			
			WWW www = StatisticHandler.GetMeRightWWW( node.SelectSingleNode ("textureGUIName").InnerText);
		
			yield return www;
			entry.textureGUI = new Texture2D(www.texture.width, www.texture.height);
			www.LoadImageIntoTexture(entry.textureGUI);
		//	Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
			weaponIndexTable[entry.weaponId]=entry;	
			
			if(bool.Parse(node.SelectSingleNode ("default").InnerText)){
				switch(entry.gameClass){
					case GameClassEnum.ANY:
			
						for(int j=0;j<Choice._Personal.Length;j++){
							Choice.SetChoice((int)entry.gameSlot, j, new WeaponIndex(entry.weaponId,""));
						}
					break;
					default:
                    Choice.SetChoice((int)entry.gameSlot, (int)entry.gameClass, new WeaponIndex(entry.weaponId, ""));
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
        XmlNodeList stims = xmlDoc.SelectNodes("items/stim");
		if(stimPackDictionary==null){
			stimPackDictionary = new StimPack[stims.Count];
		}
		for(int j=0;j<stims.Count;j++){
            XmlNode node  = stims[j];
			int key =int.Parse(node.SelectSingleNode("group").InnerText);
            if (stimPackDictionary[key]!=null)
            {
                stimPackDictionary[key].amount = int.Parse(node.SelectSingleNode("amount").InnerText);
            }
            else
            {
                StimPack entry = new StimPack();
                entry.name = node.SelectSingleNode("name").InnerText;
                entry.mysqlId = int.Parse(node.SelectSingleNode("mysqlId").InnerText) ;
                entry.buffId = int.Parse(node.SelectSingleNode("buffId").InnerText);

                WWW www = StatisticHandler.GetMeRightWWW(node.SelectSingleNode("textureGUIName").InnerText);

                yield return www;
                entry.textureGUI = new Texture2D(150, 80);
                www.LoadImageIntoTexture(entry.textureGUI);
                //Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
                stimPackDictionary[key]=entry;
            }
		
		}
        XmlNodeList buffs = xmlDoc.SelectNodes("items/buff");
        foreach (XmlNode nodeBuff in buffs)
        {
            int id = int.Parse(nodeBuff.SelectSingleNode("buffId").InnerText);
            Buff buff;
            if (allBuff.ContainsKey(id))
            {
                buff = allBuff[id];
            }else{
                buff = new Buff();
                allBuff[id] = buff;
            }
          
            CharacteristicToAdd add = new CharacteristicToAdd();
            add.characteristic = (CharacteristicList)System.Enum.Parse(typeof(CharacteristicList), nodeBuff.SelectSingleNode("characteristic").InnerText);
            string type = nodeBuff.SelectSingleNode("type").InnerText;
            string value = nodeBuff.SelectSingleNode("value").InnerText;
            BaseEffect effect = null;
            if (type == "float")
            {

                effect = new Effect<float>(float.Parse(value));
            }
            else if (type == "int")
            {
                effect = new Effect<int>(int.Parse(value));
            }
            else
            {
                effect = new Effect<bool>(bool.Parse(value));
            }
            effect.initalEffect = true;
            effect.type = (EffectType)System.Enum.Parse(typeof(EffectType), nodeBuff.SelectSingleNode("effecttype").InnerText);
            add.addEffect = effect;
            buff.listOfEffect.Add(add);
            
           
        }
        cachedIndex.Clear();
		foreach (XmlNode node in xmlDoc.SelectNodes("items/default")) {
            WeaponIndex index = new WeaponIndex(node.SelectSingleNode("weaponId").InnerText);
            index.gameClass = int.Parse(node.SelectSingleNode("gameClass").InnerText);
           
            cachedIndex.Add(index);
           
		}
		
        IEnumerator numenator = ParseInventory(XML);

        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }

		
	}	
	public void SetNewWeapon(BaseWeapon prefab){
		//Debug.Log (prefab);
		weaponPrefabsListbyId [prefab.SQLId] = prefab;
		//weaponPrefabsListbyId[prefab.SQLId].HUDIcon = weaponIndexTable[prefab.SQLId].textureGUI;
	}
	public BaseWeapon GetWeaponprefabByID(WeaponIndex index){
        return weaponPrefabsListbyId[index.prefabId];
	}
	public static GameClassEnum gameClassPase(string text){
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
          //  Debug.Log(Choice.ForGuiSlot(i).ToString());
            WeaponIndex index = Choice.ForGuiSlot(i);
				form.AddField ("default[]", index.ToString());
            if(index.itemId!=null&&index.itemId!=""){
               // Debug.Log("ITEM"+index.itemId);
                form.AddField("usedInvItem[]", index.itemId);
            }
		}
     
        
	/*	for(int i = 0;i<4;i++){
		
			form.AddField ("defaultrobot[]", Choice. ForGuiSlotRobot(i));
			
		}*/

		StatisticHandler.instance.StartCoroutine(StatisticHandler.SendForm (form,StatisticHandler.SAVE_ITEM));
	
	}
	
	public List<GUIItem> GetItemForSlot(GameClassEnum gameClass, int gameSlot){
		
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

        List<GUIItem> weaponList = new List<GUIItem>();
		
		GameClassEnum MyANY = GameClassEnum.ANY;
		if((int)gameClass>(int) GameClassEnum.ANY){
			MyANY = GameClassEnum.ANYROBOT;
		}
		int i = 0;
		foreach(FromDBAnims entry  in animsIndexTable.Values){
	
			if(entry.animationType==AnimType.TAUNT&&(entry.gameClass ==MyANY||entry.gameClass==gameClass)){
				GUIItem gui = new GUIItem();
				gui.index = new WeaponIndex(i++,"");
				gui.name= entry.name;
				gui.texture = entry.textureGUI;

				weaponList.Add(gui);
			
			}
		}
		return weaponList;
	}

  
	public List<GUIItem> GetWeaponForSlot(GameClassEnum gameClass, BaseWeapon.SLOTTYPE gameSlot){

        List<GUIItem> weaponList = new List<GUIItem>();
		GameClassEnum MyANY = GameClassEnum.ANY;
		if((int)gameClass>(int) GameClassEnum.ANY){
			MyANY = GameClassEnum.ANYROBOT;
		}
		foreach(FromDBWeapon entry  in weaponIndexTable.Values){
			
            if (entry.gameSlot ==gameSlot && (entry.gameClass == MyANY || entry.gameClass == gameClass))
            {
				GUIItem gui = new GUIItem();
                gui.index = new WeaponIndex(entry.weaponId, ""); ;
				gui.name= entry.name;
				gui.texture = entry.textureGUI;
                gui.color = Color.white;
				gui.group= entry.group;
				weaponList.Add(gui);
			
			}
		}
        List<InventorySlot> items = null;
        if (invItems.ContainsKey(ShopSlotType.WEAPON))
        {
            items = invItems[ShopSlotType.WEAPON];
        }
        
        if (items != null)
        {
            int intGameSlot = (int)gameSlot;
            foreach (InventorySlot slot in items)
            {

                if (slot.gameType == intGameSlot && (slot.gameClass == GameClassEnum.ANY || slot.gameClass == gameClass))
                {
                  
                    GUIItem gui = new GUIItem();
                    gui.index = new WeaponIndex(slot.ingamekey, slot.id);
                    if (slot.personal)
                    {
                        gui.color = Color.blue;
                    }
                    else
                    {
                        gui.color = Color.green;
                    }
                    gui.name = slot.name;
                    gui.texture = slot.texture;
					gui.group= slot.group;
                    weaponList.Add(gui);

                }
            }
        }
		return weaponList;
	}
	
	//ENDPLAYER ITEM SECTION
	
	
	//SHOPING SECTION 
	public bool isShopLoading;

    public Dictionary<ShopSlotType, List<ShopSlot>> shopItems = new Dictionary<ShopSlotType, List<ShopSlot> >();

    public List<ShopSlot> cachedShopList = new List<ShopSlot>();

    public ShopGUI shop;

	public IEnumerator  LoadShop(ShopGUI _shop){
        shop = _shop;
		
        if (!isShopLoading)
        {
            isShopLoading = true;
		    WWWForm form = new WWWForm ();
			
		    form.AddField ("uid", UID);
		
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
        
            IEnumerator innerCoroutineEnumerator = GenerateList(GameClassEnum.ENGINEER,ShopSlotType.WEAPON);
            while (innerCoroutineEnumerator.MoveNext())
                yield return innerCoroutineEnumerator.Current;	
        }
		
	}
	public void ClearShop(){
		foreach(List<ShopSlot> list in shopItems.Values){
			foreach(ShopSlot slot in list){
				slot.texture= null;
				slot.loadModel = null;
			}
		}
		
	}
	
	public void  ParseShop(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		shopItems.Clear();
		foreach (XmlNode node in xmlDoc.SelectNodes("items/slot")) {
			ShopSlot slot = new ShopSlot();
            slot.gameClass = (GameClassEnum)Enum.Parse(typeof(GameClassEnum), node.SelectSingleNode("class").InnerText);
            slot.type = (ShopSlotType)Enum.Parse(typeof(ShopSlotType), node.SelectSingleNode("type").InnerText);
            slot.id =node.SelectSingleNode("game_id").InnerText;
            slot.name = node.SelectSingleNode("name").InnerText;
            slot.description = node.SelectSingleNode("description").InnerText;
            slot.shopicon = node.SelectSingleNode("shopicon").InnerText;
            slot.model = node.SelectSingleNode("model").InnerText;
            slot.cashCost = int.Parse(node.SelectSingleNode("kp_price").InnerText);
            slot.goldCost = int.Parse(node.SelectSingleNode("gitp_price").InnerText);
            if (slot.goldCost != 0)
            {
                slot.goldSlot = node.SelectSingleNode("gitp_shop_id").InnerText;
            }

            if (slot.cashCost != 0)
            {
                slot.cashSlot = node.SelectSingleNode("kp_shop_id").InnerText;
            }
            /*slot.gameClasses = new GameClassEnum[node.SelectNodes("items/weapon").Count];
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
            }*/



			// wait until the download is done
			/*yield return www;

			// assign the downloaded image to the texture of the slot
			www.LoadImageIntoTexture(slot.texture);*/
            if (!shopItems.ContainsKey(slot.type))
            {
                shopItems[slot.type] = new List<ShopSlot>();
            }
            shopItems[slot.type].Add(slot);
		}

		
	}
    public IEnumerator GenerateList(GameClassEnum gameClass,ShopSlotType type)
    {
        List<ShopSlot> result = new List<ShopSlot>();
        if (!shopItems.ContainsKey(type))
        {
            shop.OpenList(result);
            yield return null;
        }
        else
        {

            List<ShopSlot> list = shopItems[type];
            if (list == null)
            {
                shop.OpenList(result);
                yield return null;
            }
            else
            {

                foreach (ShopSlot slot in list)
                {
                    if (gameClass == slot.gameClass || slot.gameClass == GameClassEnum.ANY)
                    {
                        result.Add(slot);
                    }
                }
                shop.OpenList(result);
                foreach (ShopSlot slot in result)
                {
                    if (slot.texture == null)
                    {
                        WWW www = StatisticHandler.GetMeRightWWW(slot.shopicon);
                       
                        yield return www;
                        slot.texture = new Texture2D(138, 58);
                        www.LoadImageIntoTexture(slot.texture);
                    }


                }
            }
        }
    }
    public void LoadModel(SimpleSlot slot)
    {
        StartCoroutine(_LoadModel(slot));
                     
    }
    IEnumerator _LoadModel(SimpleSlot slot)
    {
        if (slot.loadModel == null)
        {
            string crossDomainesafeURL = StatisticHandler.GetNormalURL() + slot.model;
            AssetBundle bundle = null;
            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, 1))
            {
                bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, 1);
                Debug.Log("MyBundle" + bundle);

            }
            else
            {
                Debug.Log("NO BUNLDE NET TO LOAD");
                WWW www = WWW.LoadFromCacheOrDownload(crossDomainesafeURL, 1);

                yield return www;
                if (www.error == null)
                {


                    bundle = www.assetBundle;
                }
				 AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL,1);

            }




            slot.loadModel = (GameObject)bundle.mainAsset;
        }
       
    }
	public IEnumerator  BuyItem(string itemId,LotItemGUI gui){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("shop_item", itemId);
	
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.BUY_ITEM);
	
		yield return w;
        Debug.Log(w.text);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
		
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
				
			form = new WWWForm ();
			
			form.AddField ("uid", UID);
			
			IEnumerator numenator = LoadItems (form);
			
			while(numenator.MoveNext()){
				yield return numenator.Current;
			}
            numenator = GlobalPlayer.instance.ReloadStats();
            while (numenator.MoveNext())
            {
                yield return numenator.Current;
            }
            if (gui != null)
            {
                gui.Shop.CloseLot();
            }
        }
        else
        {
            if (gui != null)
            {
                gui.SetError(xmlDoc.SelectSingleNode("result/errortext").InnerText);
            }
        }
	}
    public IEnumerator UseItem(string[] itemId)
    {
        WWWForm form = new WWWForm();
        
        form.AddField("uid", UID);
        form.AddField("game_item", string.Join(",", itemId));
        WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.USE_ITEM);
      
        yield return w;

        IEnumerator numenator = ParseInventory(w.text); 

        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }

    }
	public bool TryUseStim(string itemId, out int id){
	
		id =allitems[itemId].ingamekey;
        //Debug.Log("use");
		if(!stimPackDictionary[id].active){
			stimPackDictionary[id].active = true;			
            StartCoroutine(UseItem(new string[] { itemId}));
			return true;
		}
		return false;
		
	}
	public void UseRepairKit(string itemId,string kit_id){
		 StartCoroutine(_UseRepairKit(itemId,kit_id));
	
	}
	
	IEnumerator _UseRepairKit(string id,string kit_id){
		WWWForm form = new WWWForm();
        
        form.AddField("uid", UID);
        form.AddField("game_item", id);
		form.AddField("kit_id", kit_id);
        WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.REPAIR_ITEM);
      
        yield return w;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
		
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
		
			IEnumerator numenator = ParseInventory(w.text);

			while (numenator.MoveNext())
			{
				yield return numenator.Current;
			}
		
		}
	}
	
	public void DesintegrateItem(string id){
        StartCoroutine(_DesintegrateItem(id));
	}
	IEnumerator _DesintegrateItem(string id){
		WWWForm form = new WWWForm();
        
        form.AddField("uid", UID);
        form.AddField("game_item", id);
	    WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.DISENTEGRATE_ITEM);
      
        yield return w;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
		
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
			
			IEnumerator numenator = ParseInventory(w.text);

			while (numenator.MoveNext())
			{
				yield return numenator.Current;
			}
		}
	}
	public List<CharacteristicToAdd> GetBuff(int id){
		if(allBuff.ContainsKey(id)){
			return allBuff[id].listOfEffect;
		}else{
			return new List<CharacteristicToAdd>();
		}
		
	}
    public int GetBuffFromStim(int id)
    {
        return stimPackDictionary[id].buffId;
    }
    public void RestartStimPack() {
        foreach (StimPack stim in stimPackDictionary)
        {
            stim.active = false;
        }
    
    }
	
	//END SHOPING SECTION 
	
	
	//INVENTORY SECTION 

    public Dictionary<string, InventorySlot> allitems = new Dictionary<string, InventorySlot>();
	
    public Dictionary<ShopSlotType, List<InventorySlot>>  invItems = new Dictionary<ShopSlotType, List<InventorySlot> >();

		
	public IEnumerator ParseInventory(String XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
        XmlNodeList list = xmlDoc.SelectNodes("items/inventory/item");
        if (list.Count == 0)
        {
            list =  xmlDoc.SelectNodes("result/inventory/item");
        }

        foreach (XmlNode node in list)
        {
           
			InventorySlot slot = null;
			string id = node.SelectSingleNode("id").InnerText;
			if(allitems.ContainsKey(id)){
				slot  =allitems[id];
				
			}else{
				slot = new InventorySlot();
                allitems[id] = slot;
				if (!invItems.ContainsKey(slot.type))
				{
					invItems[slot.type] = new List<InventorySlot>();
				}
				invItems[slot.type].Add(slot);
				
				slot.gameClass = (GameClassEnum)Enum.Parse(typeof(GameClassEnum), node.SelectSingleNode("class").InnerText);
				slot.type = (ShopSlotType)Enum.Parse(typeof(ShopSlotType), node.SelectSingleNode("type").InnerText);
				slot.id = id;
				slot.gameId= int.Parse(node.SelectSingleNode("game_id").InnerText);
				slot.group =int.Parse (node.SelectSingleNode ("ingametype").InnerText);
                if (node.SelectSingleNode("time_end").InnerText != "")
                {
                    try
                    {
                        slot.timeEnd = DateTime.Parse(node.SelectSingleNode("time_end").InnerText);
                    }
                    catch (Exception)
                    {

                        Debug.LogError("date format  exeption");
                    }
                   
                }
				slot.modslot= int.Parse(node.SelectSingleNode("modslot").InnerText);
				slot.ingamekey= int.Parse(node.SelectSingleNode("ingamekey").InnerText);
				slot.maxcharge= int.Parse(node.SelectSingleNode("maxcharge").InnerText);
				slot.name = node.SelectSingleNode("name").InnerText;
				slot.description = node.SelectSingleNode("description").InnerText;
				slot.shopicon = node.SelectSingleNode("shopicon").InnerText;
				slot.model = node.SelectSingleNode("model").InnerText;
                slot.personal = bool.Parse(node.SelectSingleNode("personal").InnerText);
                WWW www = StatisticHandler.GetMeRightWWW(slot.shopicon);
                       
                yield return www;
                slot.texture = new Texture2D(138, 58);
                www.LoadImageIntoTexture(slot.texture);
                    
			}
			slot.charge= int.Parse(node.SelectSingleNode("charge").InnerText);
		
			
            /*slot.gameClasses = new GameClassEnum[node.SelectNodes("items/weapon").Count];
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
            }*/



			// wait until the download is done
			/*yield return www;

			// assign the downloaded image to the texture of the slot
			www.LoadImageIntoTexture(slot.texture);*/
          
		}

	
	}
		
	public IEnumerator GenerateInvList(GameClassEnum gameClass,ShopSlotType type)
    {
        List<InventorySlot> result = new List<InventorySlot>();
        if (!invItems.ContainsKey(type))
        {
            //shop.OpenList(result);
            yield return null;
        }
        else
        {

            List<InventorySlot> list = invItems[type];
            if (list == null)
            {
               // shop.OpenList(result);
                yield return null;
            }
            else
            {

                foreach (InventorySlot slot in list)
                {
                    if (gameClass == slot.gameClass || slot.gameClass == GameClassEnum.ANY)
                    {
                        result.Add(slot);
                    }
                }
               // shop.OpenList(result);
                foreach (InventorySlot slot in result)
                {
                    if (slot.texture == null)
                    {
                        WWW www = StatisticHandler.GetMeRightWWW(slot.shopicon);
                       
                        yield return www;
                        slot.texture = new Texture2D(138, 58);
                        www.LoadImageIntoTexture(slot.texture);
                    }


                }
            }
        }
    }
	//ENDINVENTORY SECTION
	
	//SMALL SHOP SECTION


	 public List<SmallShopData> GetAllStim(){
		List<SmallShopData> allstims = new List<SmallShopData>();
		foreach(StimPack pack in stimPackDictionary){
			SmallShopData data=null;
			if(invItems.ContainsKey(ShopSlotType.ETC)){
				foreach(InventorySlot slot in 	invItems[ShopSlotType.ETC]){
					if(slot.gameId==pack.mysqlId){
					
						if(data==null){
							data= new SmallShopData();
							data.name  = slot.name;
							data.amount=0;
							data.textureGUI = pack.textureGUI;
							data.itemId =slot.id;
						}
						data.amount  += slot.charge;						
					}				
				}			
			}
			if(data!=null){
				continue;
			}
			if(shopItems.ContainsKey(ShopSlotType.ETC)){
				foreach(ShopSlot slot in 	shopItems[ShopSlotType.ETC]){	
					if(slot.id==pack.mysqlId.ToString()){
						data= new SmallShopData();
						data.name  = slot.name;
						data.amount=0;
						data.textureGUI = pack.textureGUI;
						data.cashSlot= slot.cashSlot; 
						data.goldSlot= slot.goldSlot;
						break;
						
					}
				}
			}
			if(data==null){
				continue;
			}
			allstims.Add(data);
		}
		return allstims;
	 }
	
	//END SMALL SHOP SECTION
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