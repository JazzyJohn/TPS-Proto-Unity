using UnityEngine;
using System;

public enum AMMOTYPE{PISTOL,RIFLE,ROCKETS,MACHINEGUN,SHOOTGUNSHEELL,FUEL,GRENADE};

[RequireComponent (typeof (Pawn))]
public class InventoryManager : MonoBehaviour {

    private int specialSize = 1;

	private BaseWeapon[] myWeapons = new BaseWeapon[0];

    private int[] indexOfSlot = new int[]{-1,-1,-1,-1};

    private BaseArmor[] myArmor = new BaseArmor[2];
    public string[] weaponNames;

    public string[] armorNames;

    public string stdMelee;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
    private int beforeGrenade;

    private int beforeMelee;
	
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
            indexOfSlot[(int)myWeapons[i].slotType] = i;
            
        }
        for (int i = 0; i < armorNames.Length; i++)
        {

            myArmor[i] = NetworkController.Instance.ArmorSpawn(armorNames[i], transform.position, Quaternion.identity, owner.foxView.isChildScene(), owner.foxView.viewID).GetComponent<BaseArmor>();
            myArmor[i].AttachArmorToChar(owner);

        }
        if (indexOfSlot[(int)SLOTTYPE.MAIN] != -1)
        {
            NetworkController.Instance.ThisSpawnWeaponMakeInHand(indexOfSlot[(int)SLOTTYPE.MAIN]);
            return;
        }
        if (indexOfSlot[(int)SLOTTYPE.PERSONAL] != -1)
        {
            NetworkController.Instance.ThisSpawnWeaponMakeInHand(indexOfSlot[(int)SLOTTYPE.PERSONAL]);
            return;
        }
  
        
	}


	//Start Weapon generation
	public void GenerateWeaponStart(){
		if (owner.foxView.isMine) {
            if (indexOfSlot[(int)SLOTTYPE.MAIN] != -1)
            {
                _ChangeWeapon(indexOfSlot[(int)SLOTTYPE.MAIN]);
                return;
            }
            if (indexOfSlot[(int)SLOTTYPE.PERSONAL] != -1)
            {
                _ChangeWeapon(indexOfSlot[(int)SLOTTYPE.PERSONAL]);
                return;
            }
            _ChangeWeapon(0);
		}
        if (indexOfSlot[(int)SLOTTYPE.GRENADE] != -1)
        {
            specialSize++;
        }
	}
	
	public void TakeGrenade(){
		if (owner.foxView.isMine) {
            if (indexOfSlot[(int)SLOTTYPE.GRENADE] != indexWeapon)
                beforeGrenade = indexWeapon;

		

           
            _ChangeWeapon(indexOfSlot[(int)SLOTTYPE.GRENADE]);
		}
	}
  
	public void PutGrenadeAway(){
		if (owner.foxView.isMine) {
            currentWeapon.PutAway();
            owner.SetWeaponType(myWeapons[beforeGrenade].animType);
			
		}
	}
    public void TakeMelee()
    {
        if (owner.foxView.isMine)
        {
            if (indexOfSlot[(int)SLOTTYPE.MELEE]!=indexWeapon)
                beforeMelee = indexWeapon;


            _ChangeWeapon(indexOfSlot[(int)SLOTTYPE.MELEE]);
        }
    }
    public void PutMeleeAway()
    {
        if (owner.foxView.isMine)
        {
           // Debug.Log("beforeMelee"+beforeMelee);
            currentWeapon.PutAway();
            owner.SetWeaponType(myWeapons[beforeMelee].animType);
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
    public BaseArmor GetArmor()
    {
        if (myArmor.Length > 0)
        {
            return myArmor[0];
        }
        return null;
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

            if (allAmmo[i].type == AMMOTYPE.GRENADE && indexOfSlot[(int)SLOTTYPE.GRENADE] != -1)
                {
                    if (myWeapons[indexOfSlot[(int)SLOTTYPE.GRENADE]].curAmmo >= 1)
                    {
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
                    if (allAmmo[i].type == AMMOTYPE.GRENADE && indexOfSlot[(int)SLOTTYPE.GRENADE] != -1)
                    {
                        myWeapons[indexOfSlot[(int)SLOTTYPE.GRENADE]].Reload();
                    }
                  
                }
            
        }

    }
	public virtual bool HasGrenade(){
        int grenadeSlot = indexOfSlot[(int)SLOTTYPE.GRENADE];
        if (grenadeSlot < 0)
        {
            return false;
        }
		if(myWeapons[grenadeSlot].curAmmo>0){
			return true;
		}
		if(HasAmmo(AMMOTYPE.GRENADE)){
			myWeapons[grenadeSlot].Reload();
			return true;
		}
		return false;
	}

    public BaseWeapon GetCurWeapon(){
        int grenadeSlot = indexOfSlot[(int)SLOTTYPE.GRENADE];
        if (indexWeapon == grenadeSlot)
        {
            return myWeapons[beforeGrenade];
        }
        else
        {
            return myWeapons[indexWeapon];
        }
    }
    public BaseWeapon GetGrenade()
    {
        int grenadeSlot = indexOfSlot[(int)SLOTTYPE.GRENADE];
        if (grenadeSlot <0)
        {
            return null;
        }
       
        return myWeapons[grenadeSlot];
       
    }

	//AMMO BAG SECTION END

    public void ReloadStats()
    {
        for (int i = 0; i < myWeapons.Length; i++)
        {
            myWeapons[i].RecalculateStats();
        }
    }
	
	
	
	
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

    public void AddStandartMelee()
    {
        BaseWeapon[] oldprefabWeapon = myWeapons;
        myWeapons = new BaseWeapon[oldprefabWeapon.Length + 1];

        for (int i = 0; i < oldprefabWeapon.Length; i++)
        {
            myWeapons[i] = oldprefabWeapon[i];
        }
        myWeapons[myWeapons.Length - 1] = NetworkController.Instance.WeaponSpawn(stdMelee, transform.position, Quaternion.identity, owner.foxView.isChildScene(), owner.foxView.viewID).GetComponent<BaseWeapon>();
        indexOfSlot[(int)myWeapons[myWeapons.Length - 1].slotType] = myWeapons.Length - 1;
        myWeapons[myWeapons.Length - 1].AttachWeaponToChar(owner);
    }

	public void SetSlot( BaseWeapon prefab){
		if(prefab==null){
			return;
		}
        GA.API.Design.NewEvent("Game:Weapon:Choice:" + prefab.SQLId);
		for(int i=0;i<myWeapons.Length;i++){
			if(myWeapons[i].slotType==prefab.slotType){
				myWeapons[i]=NetworkController.Instance.WeaponSpawn(prefab.name,transform.position, Quaternion.identity,owner.foxView.isChildScene(),owner.foxView.viewID).GetComponent<BaseWeapon>();
                indexOfSlot[(int)myWeapons[i].slotType] = i;
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
        indexOfSlot[(int)myWeapons[myWeapons.Length - 1].slotType] = myWeapons.Length - 1;
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

    public bool IsSpecial(int newindex)
    {
        return indexOfSlot[(int)SLOTTYPE.GRENADE] == newindex || indexOfSlot[(int)SLOTTYPE.MELEE] == newindex;
    }

	public void NextWeapon(){
        if(!owner.animator.CanWeaponChange()){
            return;
        }
		int newIndex = indexWeapon+1;
		
      
		if(newIndex>=myWeapons.Length){
			newIndex=0;
		}
       
        if (IsSpecial(newIndex))
        {
            if (myWeapons.Length > specialSize)
            {
                indexWeapon=newIndex; 
                NextWeapon();
                return;
            }


        }
        owner.animator.WeaponChange();
        cahcedIndex = newIndex;
        owner.SetWeaponType(myWeapons[cahcedIndex].animType);
		//_ChangeWeapon(newIndex);
	}
	public void PrevWeapon(){
        if (!owner.animator.CanWeaponChange())
        {
            return;
        }
		int newIndex = indexWeapon-1;
		
		if(newIndex<0){
			newIndex=myWeapons.Length-1;
		}
       
        if (IsSpecial(newIndex))
        {
            if (myWeapons.Length > specialSize)
            {
                indexWeapon = newIndex; 
                PrevWeapon();
                return;
            }


        }
		//
        owner.animator.WeaponChange();
       cahcedIndex = newIndex;
       owner.SetWeaponType(myWeapons[cahcedIndex].animType);
		//_ChangeWeapon(newIndex);
	}

    public void ChangeWeapon()
    {
        _ChangeWeapon(cahcedIndex);
    }
	//Change weapon in hand
	public void ChangeWeapon(int newWeapon){
        newWeapon = indexOfSlot[newWeapon];
        if (newWeapon == -1)
        {
            return;
        }

		if (indexWeapon != newWeapon) {
            if (!owner.animator.CanWeaponChange())
            {
                return;
            }
            owner.animator.WeaponChange();
            cahcedIndex = newWeapon;
            owner.SetWeaponType(myWeapons[cahcedIndex].animType);
			//_ChangeWeapon(newWeapon);	
		}
	}

	protected void _ChangeWeapon(int newWeapon){
		if(myWeapons.Length<=newWeapon|| newWeapon<0){
			Debug.Log("Selected weapon doesn't exist in current inventory manager");
			return;
		}
        int grenadeSlot = indexOfSlot[(int)SLOTTYPE.GRENADE];
        int meleeSlot = indexOfSlot[(int)SLOTTYPE.MELEE];
        if (newWeapon == grenadeSlot || newWeapon== meleeSlot)
        {
            owner.SetWeaponType(myWeapons[newWeapon].animType);

        }
		BaseWeapon firstWeapon = myWeapons[newWeapon];



        if (currentWeapon != null && (currentWeapon.slotType == SLOTTYPE.GRENADE || currentWeapon.slotType == SLOTTYPE.MELEE))
        {
			
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
