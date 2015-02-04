using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnidecodeSharpFork;
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


public class FromDBAnims{
	public GameClassEnum  gameClass;
	
	public AnimType animationType;
	
	public string animationId;
	
	public Texture2D textureGUI;

	public string name;

	
}
public enum ShopSlotType
{
    WEAPON, ARMOR, ETC,OFFERS,PREMIUM
}
public enum BuyMode
{
    NONE, FOR_KP,FOR_KP_UNBREAK ,FOR_GOLD_TIME, FOR_GOLD_FOREVER
}
public class PricePart{
	public BuyMode mode;
	
	public int amount;
}
public class Price{

	public BuyMode mode;
	
	public PricePart[] parts ;
}

public class SimpleSlot{
 public GameClassEnum gameClass;

	public string name;
	
	public string engName;
	
	public string id;
	
	public string description;
	
	public Texture2D texture;

    public GameObject loadModel;

    public string shopicon;

    public string model;
	
	public ShopSlotType type;
	
	public Price[] prices;
	
	
}



public class WeaponChar {
    public string gunMode;

    public float aim;

    public float dmg;

    public float speed;

    public int magazine;

    public float reload;

}
public class SmallShopData{

	public string cashSlot;

    public string goldSlot;

    public int cashCost;

    public int goldCost;
	
	public string name;
	
	public string engName;
	
    public int amount;

    public string itemId;

    public string descr;

	public Texture2D textureGUI;

}
public class InventorySlot  : SimpleSlot{

	public int charge;

	public DateTime timeEnd;
	
	public BuyMode buyMode;

	public int modslot;

	public int ingamekey;

	public int maxcharge;

	public int gameId{
		get{ return getGameType();}
		set{  _gameId = value;}
	}
	protected virtual int getGameType(){
		return _gameId;
	}
	public virtual bool isAvailable(){
		return BuyMode.NONE!=buyMode;
	}
	public virtual int UpCharge(){
		if(BuyMode.FOR_KP!=buyMode){
			if(charge<0){
				charge++;
			}
		}
		return charge;
	}
	private int _gameId;

	public int group;

	public WeaponChar chars;
   
 }
 
 public class WeaponInventorySlot : SimpleSlot{
 	public int weaponId;
	
	
	public BaseWeapon.SLOTTYPE gameSlot;

	public int group;
	
	protected override int getGameType(){
		return(int)gameSlot;
   }
    public int GetMissingCharge(){
		if(BuyMode.FOR_KP!=buyMode){
			return charge;
		}
		return 0;
	}

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

	public static string smallRepairId="18";
    public static string normalRepairId = "22";
    public static string maximumRepairId = "23";

	//blocks 

    private bool buyBlock = false;
    private bool useBlock = false;
    private bool repairBlock = false;
    private bool desintegrateBlock = false;
	

	//PLAYER ITEM SECTION
	public BaseWeapon[] weaponPrefabsListbyId;

	public List<string> markedItems = new List<string>();

	public Dictionary<int,WeaponInventorySlot>  weaponIndexTable = new Dictionary<int,FromDBWeapon>();
	
	public  Dictionary<int,FromDBAnims> animsIndexTable= new Dictionary<int,FromDBAnims>();

    public List<WeaponIndex> cachedIndex = new List<WeaponIndex>();
	
	public StimPack[] stimPackDictionary;

    public Dictionary<int, Buff> allBuff = new Dictionary<int, Buff>();
	private string UID ="";
	
	
	private Dictionary<int,int> toSendLower = new  Dictionary<int,int> ();
	
	public void Init(string uid){
			UID = uid;
            WWWForm form = new WWWForm();

            form.AddField("uid", UID);

            StartCoroutine(LoadItems(form));
           // StartCoroutine(LoadShop(null));
	}


    public void ConnectToPrefab()
    {

       
       
      
		foreach (WeaponInventorySlot weapon in weaponIndexTable.Values)
		{
			if (weaponPrefabsListbyId[weapon.ingamekey] != null)
			{
				weapon.gameSlot = weaponPrefabsListbyId[weapon.ingamekey].slotType;
			//		Debug.Log(weapon.name + " " + weapon.gameType);
				weaponPrefabsListbyId[weapon.ingamekey].HUDIcon = weapon.texture;
			}
		}

       
        foreach (WeaponIndex index in cachedIndex)
        {
           
            if (index.prefabId != -1)
            {
                if (weaponPrefabsListbyId[index.prefabId] != null)
                {
					int gameSlot = (int)weaponPrefabsListbyId[index.prefabId].slotType;
					Choice.SetChoice(gameSlot, index.gameClass, index);
				
				}
                   
            }
        }
     
    }

	public int LowerCharge(int id){
		if(!toSendLower.ContainsKey(id){
			toSendLower[id] = 0;
		}
		toSendLower[id]= toSendLower[id]+1;
		return weaponIndexTable[id].LowerCharge();
	}
	public int GetCharge(int id){
		
		return weaponIndexTable[id].charge();
	}
	public WeaponInventorySlot GetSlot(int id){
		return weaponIndexTable[id];
	}
	public void ReloadItem(){
	 StartCoroutine(ReLoadItemsSync());
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
	
		foreach (XmlNode node in xmlDoc.SelectNodes("items/slots")) {
			InventorySlot slot;
		     switch (slot.type)
            {
                case ShopSlotType.WEAPON:
					WeaponInventorySlot weaponslot = new WeaponInventorySlot();
					slot= weaponslot;
					
					
					weaponIndexTable[weaponslot.weaponId]=weaponslot;
					
					weaponslot.weaponId = int.Parse (node.SelectSingleNode ("ingame_mysqlid").InnerText);
					
					weaponslot.group =int.Parse (node.SelectSingleNode ("ingame_type").InnerText);
					if (node.SelectSingleNode("aim") != null)
                    {
                        slot.chars = new WeaponChar();
                        slot.chars.aim = float.Parse(node.SelectSingleNode("aim").InnerText);
                        slot.chars.dmg = float.Parse(node.SelectSingleNode("damage").InnerText);
                        slot.chars.speed = float.Parse(node.SelectSingleNode("speed").InnerText);
                        slot.chars.reload = float.Parse(node.SelectSingleNode("reload").InnerText);
                        slot.chars.magazine = int.Parse(node.SelectSingleNode("magazine").InnerText);
                        slot.chars.gunMode = node.SelectSingleNode("mode").InnerText;
                    }
				default:
					slot = new InventorySlot();
				break;
			}
			
			
			slot.gameClass = (GameClassEnum)Enum.Parse(typeof(GameClassEnum), node.SelectSingleNode("class").InnerText);
            slot.type = (ShopSlotType)Enum.Parse(typeof(ShopSlotType), node.SelectSingleNode("type").InnerText);
			slot.buyMode = (BuyMode)Enum.Parse(typeof(BuyMode), node.SelectSingleNode("buytype").InnerText);
            slot.id =node.SelectSingleNode("game_id").InnerText;
			slot.maxcharge= int.Parse(node.SelectSingleNode("maxcharge").InnerText);
			slot.charge= int.Parse(node.SelectSingleNode("charge").InnerText);
			if (node.SelectSingleNode("time_end").InnerText != "")
			{
				try
				{
					slot.timeEnd = DateTime.Parse(node.SelectSingleNode("time_end").InnerText);
					slot.isTimed = true;
				}
				catch (Exception)
				{
					slot.isTimed = false;
					Debug.LogError("date format  exeption");
				}

			}
            slot.name = node.SelectSingleNode("name").InnerText;
            slot.engName = slot.name.Unidecode();
            slot.description = node.SelectSingleNode("description").InnerText;
            slot.shopicon = node.SelectSingleNode("shopicon").InnerText;
            slot.model = node.SelectSingleNode("model").InnerText;
			
           
           XmlNodeList prices =xmlDoc.SelectNodes("price");
		   slot.prices = new Price[prices.Count];
		   for(int j=0;j<prices.Count;j++){
				XmlNode onePrice =prices[i];
				price = new Price();
				XmlNodeList amounts =onePrice.SelectNodes("amount");
				XmlNodeList types =onePrice.SelectNodes("type");
				
				price.parts = new  PricePart[amounts.Count];
				price.mode =(BuyMode)Enum.Parse(typeof(BuyMode),types[0].InnerText);
				for(int k=0;k<amounts.Count;k++){
						price.parts[k] = new PricePart();
						price.parts[k].amount =int.Parse(amounts[k].InnerText);
						price.parts[k].mode = (BuyMode)Enum.Parse(typeof(BuyMode),types[k].InnerText);
				}
				 slot.prices[j]= price;
		   }
            /*slot.gameClasses = new GameClassEnum[node.SelectNodes("items/weapon").Count];
            int i=0;
            foreach (XmlNode gameClass in node.SelectNodes("class")) {
                    slot.gameClasses[i++]=	(GameClassEnum)int.Parse(gameClass.InnerText);			f yt
            }

            WWW www = null;
            if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
				
                Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_SHOP);
                www = new WWW (StatisticHandler.GetSTATISTIC_PHP()+  node.SelectSingleNode ("imageurl").InnerText);
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
                shopItems[slot.type] = new List<InventorySlot>();
            }
            shopItems[slot.type].Add(slot);
			allShopSlot[slot.id] = slot;
						
			WWW www = StatisticHandler.GetMeRightWWW( node.SelectSingleNode ("textureGUIName").InnerText);
		
			yield return www;
			slot.texture= new Texture2D(www.texture.width, www.texture.height);
			www.LoadImageIntoTexture(entry.texture);
		//	Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
			
		/*	TODO: strange shit?????;
			if(bool.Parse(node.SelectSingleNode ("default").InnerText)){
				switch(slot.gameClass){
					case GameClassEnum.ANY:
			
						for(int j=0;j<Choice._Personal.Length;j++){
							Choice.SetChoice((int)slot.gameSlot, j, new WeaponIndex(slot.weaponId,""));
						}
					break;
					default:
                    Choice.SetChoice((int)slot.gameSlot, (int)slot.gameClass, new WeaponIndex(slot.weaponId, ""));
					break;

				}
			}*/
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
            if (stimPackDictionary[key]==null)
         
            {
                StimPack entry = new StimPack();
                entry.name = node.SelectSingleNode("name").InnerText;
                entry.mysqlId = int.Parse(node.SelectSingleNode("mysqlId").InnerText) ;
                entry.buffId = int.Parse(node.SelectSingleNode("buffId").InnerText);
                stimPackDictionary[key] = entry;
                WWW www = StatisticHandler.GetMeRightWWW(node.SelectSingleNode("textureGUIName").InnerText);

                yield return www;
                entry.textureGUI = new Texture2D(150, 80);
                www.LoadImageIntoTexture(entry.textureGUI);
                //Debug.Log (entry.name + " " +entry.textureGUI + " " +entry.weaponId );
             
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
                buff.listOfEffect.Clear();
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
		foreach (XmlNode node in xmlDoc.SelectNodes("items/marked")) {
			if(!markedItems.Contains(node.InnerText)){
				markedItems.Add(node.InnerText);
			}
           
		}
        Debug.Log("Inventory Loaded");
		GlobalPlayer.instance.loadingStage++;
		
	}	
	public void SetNewWeapon(BaseWeapon prefab){
		//Debug.Log (prefab);
        if (prefab.SQLId >= 0) { 
		    weaponPrefabsListbyId [prefab.SQLId] = prefab;
        }
		//weaponPrefabsListbyId[prefab.SQLId].HUDIcon = weaponIndexTable[prefab.SQLId].textureGUI;
	}
	public BaseWeapon GetWeaponprefabByID(WeaponIndex index){
        if (weaponPrefabsListbyId[index.prefabId]!=null)
        {
			return weaponPrefabsListbyId[index.prefabId];
		}else{
			return weaponPrefabsListbyId[0];
		}
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
	public void SendChargeData(){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		
		foreach (KeyValuePair<int, int> pair in toSendLower)
		{
          //  Debug.Log(Choice.ForGuiSlot(i).ToString());
          
			form.AddField ("default["+pair.Key+"]", pair.Value);
          
		}
	 	StatisticHandler.instance.StartCoroutine(StatisticHandler.SendForm (form,StatisticHandler.CHARGE_DATA));
	}
	public List<GUIItem> GetItemForSlot(GameClassEnum gameClass, int gameSlot){
		
		//Debug.Log (gameClass +"  "+ gameSlot);
			switch(gameSlot){
				//Taunt section look WeaponPlayer.cs for details
				case 5:
						return  GetAnimationForSlot( gameClass);
			    case 6:
				case 7:
				case 8:
				case 9:
						return  GetArmorForSlot( gameClass,  gameSlot);
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
    public List<GUIItem> GetArmorForSlot(GameClassEnum gameClass, int gameSlot)
    {
	    List<GUIItem> weaponList = new List<GUIItem>();
		List<InventorySlot> items = null;
        if (shopItems.ContainsKey(ShopSlotType.ARMOR))
        {
            items = shopItems[ShopSlotType.ARMOR];
        }
        
        if (items != null)
        {
            foreach (InventorySlot slot in items)
            {
				if(slot.isAvailable()){
					if (slot.gameType == gameSlot && (slot.gameClass == GameClassEnum.ANY || slot.gameClass == gameClass))
					{
					  
						GUIItem gui = new GUIItem();
						gui.index = new WeaponIndex(slot.ingamekey, slot.id);
						gui.color = ItemColor.Normal;
						gui.name = slot.name;
						gui.texture = slot.texture;
						gui.group= slot.group;
						weaponList.Add(gui);

					}
				}
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
		foreach(WeaponInventorySlot slot  in weaponIndexTable.Values){
			if(slot.isAvailable()){
				if (slot.gameSlot ==gameSlot && (slot.gameClass == MyANY || slot.gameClass == gameClass))
				{
					GUIItem gui = new GUIItem();
					gui.index = new WeaponIndex(slot.weaponId, ""); ;
					gui.name= slot.name;
					gui.text = "";
					gui.texture = slot.texture;
					gui.color = ItemColor.Normal;
					gui.group= slot.group;
					gui.chars = null;
					weaponList.Add(gui);
				
				}
			}
		}
        return weaponList;
	}
	
	public List<int> GetImplants(){
        List<int> answer = new List<int>();
		for(int i=5;i<=8;i++){
			WeaponIndex index = Choice.ForGuiSlot(i);
			if(!index.IsSameIndex(WeaponIndex.Zero)){
					answer.Add(allitems[index.itemId].ingamekey);
			}
		}
		return answer;
		
	}
	//ENDPLAYER ITEM SECTION
	
	
	//SHOPING SECTION 
	public bool isShopLoading;

    public Dictionary<ShopSlotType, List<InventorySlot>> shopItems = new Dictionary<ShopSlotType, List<InventorySlot> >();
	
	
	public Dictionary<string,InventorySlot> allShopSlot = new Dictionary<string,InventorySlot>();

    public List<InventorySlot> cachedShopList = new List<InventorySlot>();

    public ShopGUI shop;

	public IEnumerator  LoadShop(ShopGUI _shop){
        shop = _shop;		
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
    public IEnumerator GenerateMarkedList(GameClassEnum gameClass,ShopSlotType type)
    {
        List<ShopSlot> result = new List<ShopSlot>();
		foreach(string id in markedItems){
			result.Add(allShopSlot[id]);
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
	public void MarkedCost(out  int cash, out int gold){
        cash=0;
        gold= 0;
		foreach(string id in markedItems){
			cash +=allShopSlot[id].cashCost;
			gold +=allShopSlot[id].goldCost;
		}
	
	}
    public int MarkedAmount()
    {
        return markedItems.Count;
    }
	public void AddToMarkedList(string id){
		if(markedItems.Contains(id)){
			return;
		}
		markedItems.Add(id);
		StartCoroutine(_AddToMarkedList(id));
	}
	IEnumerator  _AddToMarkedList(string id){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("game_id", id);
	
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.MARK_ITEM);
	
		yield return w;
	}
	public void RemoveFromMarkedList(string id){
		if(!markedItems.Contains(id)){
			return;
		}
		markedItems.Remove(id);
		StartCoroutine(_AddToMarkedList(id));
	}
	IEnumerator  _RemoveFromMarkedList(string id){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("game_id", id);
	
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.UN_MARK_ITEM);
	
		yield return w;
	}

	public bool IsMarked(string id){
		return markedItems.Contains(id);
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
            Debug.Log("Load" + crossDomainesafeURL);
            AssetBundle bundle = null;
            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, 1))
            {
                bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, 1);
                Debug.Log("MyBundle" + bundle);

            }
            else
            {
                Debug.Log("NO BUNLDE NEEd TO LOAD" + crossDomainesafeURL);
                WWW www = new  WWW(crossDomainesafeURL);

                yield return www;
                Debug.Log("ERROR" + www.error);
                if (www.error == null)
                {


                    bundle = www.assetBundle;
                    AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL, 1);
                }
				

            }




            slot.loadModel = (GameObject)bundle.mainAsset;
        }
       
    }
	public IEnumerator BuyMarkeditems(MarkItemGui gui,bool gold){
		int totalCost = 0;
		foreach(string id in markedItems){
			if(gold){
				totalCost+=allShopSlot[id].goldCost;
			}else{
				totalCost+=allShopSlot[id].cashCost;
			}
		}
		int maxCost =0;
		if(gold)
			maxCost =GlobalPlayer.instance.gold;
		else
			maxCost = GlobalPlayer.instance.cash;
		
		if(maxCost>totalCost){
			while(markedItems.Count>0){
				string id =markedItems[0];
				markedItems.RemoveAt(0);
				WWWForm form = new WWWForm ();
			
				form.AddField ("uid", UID);
				if(gold){
					form.AddField ("shop_item", allShopSlot[id].goldSlot);
				}else{
					form.AddField ("shop_item", allShopSlot[id].cashSlot);
				}
				WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.BUY_ITEM);
			
				yield return w;
			}
            WWWForm itemForm = new WWWForm();

            itemForm.AddField("uid", UID);

            IEnumerator numenator = LoadItems(itemForm);
			
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
                gui.CloseWindow();
            }
		
		}else{
			gui.MoneyError();	
		
		}
	}
	
	public IEnumerator BuyItem(string itemId,LotItemGUI gui){
		if(buyBlock){
			yield break;		
		}
		buyBlock= true;
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("shop_item", itemId);
	
		GUIHelper.ShowConnectionStart();
	
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
                if (xmlDoc.SelectSingleNode("result/error").InnerText == "2")
                {
                    gui.Shop.MainMenu.MoneyError();
                }
                gui.SetError(xmlDoc.SelectSingleNode("result/errortext").InnerText);
            }
        }
        GUIHelper.ConnectionStop();
		buyBlock =false;
	}
	
	
    public IEnumerator UseItem(string[] itemId)
    {
		if(useBlock){
           
			  yield break;
		}
		useBlock= true;
        WWWForm form = new WWWForm();
        
        form.AddField("uid", UID);
        form.AddField("game_item", string.Join(",", itemId));
        WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.USE_ITEM);
      
        yield return w;
        useBlock = false;
       
        IEnumerator numenator = ParseInventory(w.text); 

        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }

	
    }
	public bool TryUseStim(string itemId, out int id){
        if (useBlock)
        {
            id = -1;
            return false;
        }
		id =allitems[itemId].ingamekey;
        //Debug.Log("use");
		if(!stimPackDictionary[id].active){
            allitems[itemId].charge--;
			stimPackDictionary[id].active = true;
            
            StartCoroutine(UseItem(new string[] { itemId}));
            GUIHelper.SendMessage(TextGenerator.instance.GetSimpleText("UseItem") + allitems[itemId].name, allitems[itemId].texture);
			return true;
		}
		return false;
		
	}
	
	public void UseRepairKit(string itemId,int amount,InventoryGUI gui){
		if(repairBlock){
			return;
		}
        StartCoroutine(_UseRepairKit(itemId, amount, gui));
	
	}
	
	IEnumerator _UseRepairKit(string id,int amount,InventoryGUI gui){
		repairBlock= true;
		WWWForm form = new WWWForm();
        GUIHelper.ShowConnectionStart();
        form.AddField("uid", UID);
        form.AddField("game_id", id);
		form.AddField("amount", amount);
        WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.REPAIR_ITEM);
      
        yield return w;
		
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
        Debug.Log(w.text);
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
			gui.HideRepair();
			IEnumerator numenator = ParseInventory(w.text);

			while (numenator.MoveNext())
			{
				yield return numenator.Current;
			}
            gui.ReloadCategory();
            gui.CloseLot();
        }
        else
        {
            gui.SetMessage(xmlDoc.SelectSingleNode("result/errortext").InnerText);
        }
        GUIHelper.ConnectionStop();
		repairBlock= false;
	}
	
	public void UseRepairKit(string itemId,string kit_id,InventoryGUI gui){
		if(repairBlock){
			return;
		}
        StartCoroutine(_UseRepairKit(itemId, kit_id, gui));
	
	}
	
	IEnumerator _UseRepairKit(string id,string kit_id,InventoryGUI gui){
		repairBlock= true;
		WWWForm form = new WWWForm();
        GUIHelper.ShowConnectionStart();
        form.AddField("uid", UID);
        form.AddField("game_id", id);
		form.AddField("kit_id", kit_id);
        WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.REPAIR_ITEM);
      
        yield return w;
		
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
        Debug.Log(w.text);
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
			gui.HideRepair();
			IEnumerator numenator = ParseInventory(w.text);

			while (numenator.MoveNext())
			{
				yield return numenator.Current;
			}
            RemoveInventoryItem(xmlDoc.SelectSingleNode("result/kit_id").InnerText);
            gui.ReloadCategory();
            gui.CloseLot();
        }
        else
        {
            gui.SetMessage(xmlDoc.SelectSingleNode("result/errortext").InnerText);
        }
        GUIHelper.ConnectionStop();
		repairBlock= false;
	}

    public void DesintegrateItem(string id, InventoryGUI gui)
    {
		if(desintegrateBlock){
			return;
		}
        StartCoroutine(_DesintegrateItem(id, gui));
	}
    IEnumerator _DesintegrateItem(string id, InventoryGUI gui)
    {
		desintegrateBlock= true;
		WWWForm form = new WWWForm();
        GUIHelper.ShowConnectionStart();
        form.AddField("uid", UID);
        form.AddField("game_id", id);
	    WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.DISENTEGRATE_ITEM);
      
        yield return w;
	
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
        
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
			
			IEnumerator numenator = GlobalPlayer.instance.ReloadStats();
            while (numenator.MoveNext())
            {
                yield return numenator.Current;
            }
            gui.SetMoneyMessage(xmlDoc.SelectSingleNode("result/cash").InnerText , xmlDoc.SelectSingleNode("result/gold").InnerText);
            gui.CloseLot();
            gui.HideRepair();
            RemoveInventoryItem(id);
            gui.ReloadCategory();

        }
        else
        {
            gui.SetMessage(xmlDoc.SelectSingleNode("result/errortext").InnerText);
        }
        GUIHelper.ConnectionStop();
		desintegrateBlock= false;
	}
	public List<CharacteristicToAdd> GetBuff(int id){
		if(allBuff.ContainsKey(id)){
          //  Debug.Log("size"+allBuff[id].listOfEffect.Count);
			return allBuff[id].listOfEffect;
		}else{
			return new List<CharacteristicToAdd>();
		}
		
	}
    public int GetBuffFromStim(int id)
    {
        return stimPackDictionary[id].buffId;
    }
    public Texture2D GetStimTexture(int id)
    {
        return stimPackDictionary[id].textureGUI;
    }
    
    public void RestartStimPack() {
        foreach (StimPack stim in stimPackDictionary)
        {
            stim.active = false;
        }
    
    }
	public void GetItem(string item,ShowAndTellAction action){
			StartCoroutine(_GetItem(item,action));
	}
	
	public IEnumerator _GetItem(string item,ShowAndTellAction action){
		ShopSlot slot = allShopSlot[item];
		if(slot.texture==null){
			WWW www = StatisticHandler.GetMeRightWWW(slot.shopicon);
		   
			yield return www;
			slot.texture = new Texture2D(138, 58);
			www.LoadImageIntoTexture(slot.texture);
		}
		action(slot.name,slot.texture);
	}
	
	//END SHOPING SECTION 
	
	

	

	
	//SMALL SHOP SECTION


	 public List<SmallShopData> GetAllStim(){
		List<SmallShopData> allstims = new List<SmallShopData>();
		foreach(StimPack pack in stimPackDictionary){
			SmallShopData data=null;
			if(invItems.ContainsKey(ShopSlotType.ETC)){
				foreach(InventorySlot slot in 	invItems[ShopSlotType.ETC]){
                 //   Debug.Log(slot.gameId + "   " + pack.mysqlId);
					if(slot.gameId==pack.mysqlId){
					
						if(data==null){
							data= new SmallShopData();
							data.name  = slot.name;
                            data.engName = slot.engName;
                            data.descr = slot.description;
							data.amount=0;
							data.textureGUI = pack.textureGUI;
							data.itemId =slot.id;
						}
						data.amount  += slot.charge;						
					}				
				}			
			}
			
			if(shopItems.ContainsKey(ShopSlotType.ETC)){
				foreach(ShopSlot slot in 	shopItems[ShopSlotType.ETC]){
                  
					if(slot.id==pack.mysqlId.ToString()){
                        if (data == null)
                        {
                            data = new SmallShopData();
                            data.name = slot.name;
                            data.engName = slot.engName;
                            data.amount = 0;
                             data.descr = slot.description;

                            data.textureGUI = pack.textureGUI;
                        }
						data.cashSlot= slot.cashSlot; 
						data.goldSlot= slot.goldSlot;
                        data.cashCost = slot.cashCost;
                        data.goldCost = slot.goldCost;
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