using UnityEngine;
using System;

public enum AMMOTYPE{PISTOL,RIFLE,ROCKETS,MACHINEGUN,SHOOTGUNSHEELL,FUEL,GRENADE};

[RequireComponent (typeof (Pawn))]
public class InventoryManager : MonoBehaviour {

	private BaseWeapon[] myWeapons = new BaseWeapon[0] ;

    private BaseArmor[] myArmor = new BaseArmor[2];
    public string[] weaponNames;

    public string[] armorNames;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
	private int grenadeSlot=-1;
	
	private int beforeGrenade;
	
	protected Pawn owner;

    private int cahcedIndex;

    public static float GRENADE_ADD_COEF = 10.0f;
	
	[Serializable]
	public class AmmoBag {
	
			public  AMMOTYPE type;
			
			public float amount;
			
			public int maxSize;

			
	}

    public void Awake()
    {
        if (owner == null)
        {
            owner = GetComponent<Pawn>();

        }
    }
	
	
	
	private AmmoBag[] allAmmo;


    public virtual void Init()
    {
        
           

            GenerateBag();


      
     
           SpawnWeaponFromNameList();
        
    }
 
	public virtual void PawnDeath(){
		foreach(BaseWeapon weapon in myWeapons){
			weapon.PawnDeath();
		}
        foreach (BaseArmor armor in myArmor)
        {
            if (armor != null)
            {
                armor.PawnDeath();
            }
        }
	}

    protected virtual void SpawnWeaponFromNameList(){

         if (weaponNames.Length > 0)
        {
            myWeapons = new BaseWeapon[weaponNames.Length];
        }
        for (int i = 0; i < weaponNames.Length; i++)
        {

            myWeapons[i] = NetworkController.Instance.WeaponSpawn(weaponNames[i], transform.position, Quaternion.identity, owner.foxView.isChildScene(), owner.foxView.viewID).GetComponent<BaseWeapon>();
            myWeapons[i].AttachWeaponToChar(owner);
            
            if (myWeapons[i].slotType == SLOTTYPE.GRENADE)
            {
                grenadeSlot = i;
            }
			 
			
        }
        for (int i = 0; i < armorNames.Length; i++)
        {

            myArmor[i] = NetworkController.Instance.ArmorSpawn(armorNames[i], transform.position, Quaternion.identity, owner.foxView.isChildScene(), owner.foxView.viewID).GetComponent<BaseArmor>();
            myArmor[i].AttachArmorToChar(owner);

        }
        for (int i = 0; i < myWeapons.Length; i++)
        {
            if (myWeapons[i].slotType == SLOTTYPE.MAIN)
            {
                NetworkController.Instance.ThisSpawnWeaponMakeInHand(i);
                return;
            }
        }
        for (int i = 0; i < myWeapons.Length; i++)
        {
            if (myWeapons[i].slotType == SLOTTYPE.PERSONAL)
            {
                NetworkController.Instance.ThisSpawnWeaponMakeInHand(i);
                return;
            }
        }
	}


	//Start Weapon generation
	public void GenerateWeaponStart(){
		if (owner.foxView.isMine) {
			for(int i=0;i<myWeapons.Length;i++){
				if(myWeapons[i].slotType==SLOTTYPE.MAIN){
						_ChangeWeapon (i);
                        return;
				}
			}
            for (int i = 0; i < myWeapons.Length; i++)
            {
                if (myWeapons[i].slotType == SLOTTYPE.PERSONAL)
                {
                    _ChangeWeapon(i);
                    return;
                }
            }
            _ChangeWeapon(0);
		}
	}
	
	public void TakeGrenade(){
		if (owner.foxView.isMine) {
			beforeGrenade =indexWeapon;
			_ChangeWeapon(grenadeSlot);
		}
	}
	public void PutGrenadeAway(){
		if (owner.foxView.isMine) {
	
			_ChangeWeapon(beforeGrenade);
		}
	}
	//Destroy weapon and make pawn empty handed
	public void TakeWeaponAway(){
		if(currentWeapon!=null){

			currentWeapon.RequestKillMe();
		}
		owner.setWeapon(null);
	}
	
	//AMMO BAG SECTION
	//Generate bas bag of all ammo
	protected void GenerateBag(){
		AmmoBag[] allTypeInGame = PlayerManager.instance.AllTypeInGame;
		allAmmo = new AmmoBag[allTypeInGame.Length];
		for(int i = 0;i<allTypeInGame.Length;i++){
			allAmmo[i] = new  AmmoBag();
			allAmmo[i].type = allTypeInGame[i].type;
			allAmmo[i].amount = allTypeInGame[i].maxSize;
			allAmmo[i].maxSize = allTypeInGame[i].maxSize;
		}
	}
    public void RewrtieMaxAmmo(int newMax, AMMOTYPE type)
    {
        if (newMax == 0)
        {
            return;
        }
        for (int i = 0; i < allAmmo.Length; i++)
        {
            if (allAmmo[i].type == type)
            {
                allAmmo[i].maxSize = newMax;
                allAmmo[i].amount = newMax;
                return;
            }
        }
    }
	//Check is there ammo in bag
	public virtual bool HasAmmo(AMMOTYPE ammo){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				return allAmmo[i].amount>0;
			}
		}
		return false;
	}
	//Get amount ammo for current ammotype
	public virtual int GetAmmo(AMMOTYPE ammo){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
                return (int)allAmmo[i].amount;
			}
		}
		return 0;
	}
	//Return ammo from bag
	public virtual int GiveAmmo(AMMOTYPE ammo,int amount){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				if(allAmmo[i].amount>amount){
					allAmmo[i].amount-=amount;
				
				}else{
					amount =(int)allAmmo[i].amount;
					allAmmo[i].amount=0;
				
				}
				return amount;
			}
		}
		return 0;
	}
	//Add Ammo to bag
	public virtual void AddAmmo(AMMOTYPE ammo,int amount){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				allAmmo[i].amount+=amount;
				if(allAmmo[i].amount>allAmmo[i].maxSize){
					allAmmo[i].amount=allAmmo[i].maxSize;
					return;
				}
			}
		}
		
	}

    public virtual void AddAmmoAll(float percent)
    {
        for (int i = 0; i < allAmmo.Length; i++)
        {

                if (allAmmo[i].type == AMMOTYPE.GRENADE && grenadeSlot != -1 )
                {
                    if( myWeapons[grenadeSlot].curAmmo >= 1){
                        continue;
                    }else{
                        allAmmo[i].amount += ((float)allAmmo[i].maxSize) * percent * GRENADE_ADD_COEF / 100;
                    }
                    Debug.Log(allAmmo[i].amount);
                }
                else
                {
                    allAmmo[i].amount += ((float)allAmmo[i].maxSize) * percent / 100;
                }
                if (allAmmo[i].amount > allAmmo[i].maxSize)
                {
                    
                    allAmmo[i].amount = allAmmo[i].maxSize;
                    if (allAmmo[i].type == AMMOTYPE.GRENADE&&grenadeSlot!=-1)
                    {
                        myWeapons[grenadeSlot].Reload();
                    }
                  
                }
            
        }

    }
	public virtual bool HasGrenade(){
		if(myWeapons[grenadeSlot].curAmmo>0){
			return true;
		}
		if(HasAmmo(AMMOTYPE.GRENADE)){
			myWeapons[grenadeSlot].Reload();
			return true;
		}
		return false;
	}
	//AMMO BAG SECTION END
	
	
	
	
	
	
	//WEAPON CHANGE SECTION

    bool isThereMain()
    {
        for (int i = 0; i < myWeapons.Length; i++)
        {
            if (myWeapons[i].slotType == SLOTTYPE.MAIN)
            {

                return true;
            }
        }
        return false;
    }
    public void SetArmorSlot(BaseArmor prefab)
    {
        int i = 0;
        switch (prefab.slotType)
        {
            case SLOTTYPE.HEAD:
                i = 1;
                break;
            default:
                i = 0;
                break;
        }
        myArmor[i] = NetworkController.Instance.ArmorSpawn(prefab.name, transform.position, Quaternion.identity, owner.foxView.isChildScene(), owner.foxView.viewID).GetComponent<BaseArmor>();
        myArmor[i].AttachArmorToChar(owner);	
    }
	
	public void SetSlot( BaseWeapon prefab){
		if(prefab==null){
			return;
		}
        GA.API.Design.NewEvent("Game:Weapon:Choice:" + prefab.SQLId);
		for(int i=0;i<myWeapons.Length;i++){
			if(myWeapons[i].slotType==prefab.slotType){
				myWeapons[i]=NetworkController.Instance.WeaponSpawn(prefab.name,transform.position, Quaternion.identity,owner.foxView.isChildScene(),owner.foxView.viewID).GetComponent<BaseWeapon>();
				if(prefab.slotType==SLOTTYPE.GRENADE){
                    grenadeSlot = i;
				}
				 if (prefab.slotType == SLOTTYPE.MAIN)
				{
					NetworkController.Instance.LastSpawnWeaponMakeInHand();
				}
                 if (!isThereMain() && prefab.slotType == SLOTTYPE.PERSONAL)
                 {
                     NetworkController.Instance.LastSpawnWeaponMakeInHand();
                 }
                myWeapons[myWeapons.Length - 1].AttachWeaponToChar(owner);
				return;
			}
		}
		BaseWeapon[] oldprefabWeapon = myWeapons;
		myWeapons = new BaseWeapon[oldprefabWeapon.Length+1];

		for(int i=0;i<oldprefabWeapon.Length;i++){
			myWeapons[i]= oldprefabWeapon[i];
		}
		myWeapons[myWeapons.Length-1] = NetworkController.Instance.WeaponSpawn(prefab.name,transform.position, Quaternion.identity,owner.foxView.isChildScene(),owner.foxView.viewID).GetComponent<BaseWeapon>();
		if(prefab.slotType==SLOTTYPE.GRENADE){
					grenadeSlot=myWeapons.Length-1;
		}
		 if (prefab.slotType == SLOTTYPE.MAIN)
				{
					NetworkController.Instance.LastSpawnWeaponMakeInHand();
				}
         if (!isThereMain() && prefab.slotType == SLOTTYPE.PERSONAL)
         {
             NetworkController.Instance.LastSpawnWeaponMakeInHand();
         }
        myWeapons[myWeapons.Length - 1].AttachWeaponToChar(owner);
	}
	
	
	//Change weapon in inventory 
	public void ChangePrefab(BaseWeapon newWeapon){
	
		return;
	}
	
	//implementation of dropping weapon on ground after picking another one 
	void DropWeapon(BaseWeapon oldWeapon){

		GameObject droppedWeapon =NetworkController.Instance.SimplePrefabSpawn(oldWeapon.pickupPrefabPrefab.name,transform.position,transform.rotation);
		WeaponPicker picker = droppedWeapon.GetComponent<WeaponPicker>();
	
		//picker.info =weaponinfo;

	}
	
	public void NextWeapon(){
		int newIndex = indexWeapon+1;
		if(newIndex==grenadeSlot){
			newIndex++;
		}
		if(newIndex>=myWeapons.Length){
			newIndex=0;
		}
		if(newIndex==grenadeSlot){
			newIndex++;
		}
		//Debug.Log ("NextWeapon"+newIndex);
        //cahcedIndex = newIndex;
       // owner.animator.SetWeaponType(prefabWeapon[cahcedIndex].animType);
		_ChangeWeapon(newIndex);
	}
	public void PrevWeapon(){
		int newIndex = indexWeapon-1;
		if(newIndex==grenadeSlot){
			newIndex--;
		}
		if(newIndex<0){
			newIndex=myWeapons.Length-1;
		}
		if(newIndex==grenadeSlot){
			newIndex--;
		}
		//Debug.Log ("PrevWeapon"+newIndex);
        //cahcedIndex = newIndex;
        //owner.animator.SetWeaponType(prefabWeapon[cahcedIndex].animType);
		_ChangeWeapon(newIndex);
	}

    public void ChangeWeapon()
    {
        _ChangeWeapon(cahcedIndex);
    }
	//Change weapon in hand
	public void ChangeWeapon(int newWeapon){
		if (indexWeapon != newWeapon) {
           // cahcedIndex = newWeapon;
			_ChangeWeapon(newWeapon);	
		}
	}

	protected void _ChangeWeapon(int newWeapon){
		if(myWeapons.Length<=newWeapon|| newWeapon<0){
			Debug.Log("Selected weapon doesn't exist in current inventory manager");
			return;
		}
		BaseWeapon firstWeapon = myWeapons[newWeapon];
     
	
	
		if(currentWeapon!=null){
			
			currentWeapon.PutAway();
		}

		//TakeWeaponAway ();
		indexWeapon=newWeapon;
		currentWeapon=firstWeapon;
		owner.setWeapon(firstWeapon);
		firstWeapon.TakeInHand();
			

	}
	//WEAPON SECTION END
}
