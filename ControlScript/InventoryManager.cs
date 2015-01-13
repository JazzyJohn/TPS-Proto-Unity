using UnityEngine;
using System;

public enum AMMOTYPE{PISTOL,RIFLE,ROCKETS,MACHINEGUN,SHOOTGUNSHEELL,FUEL};

[RequireComponent (typeof (Pawn))]
public class InventoryManager : MonoBehaviour {

	public BaseWeapon[] prefabWeapon;
    public string[] weaponNames;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
	protected Pawn owner;

    private int cahcedIndex;
	
	[Serializable]
	public class AmmoBag {
	
			public  AMMOTYPE type;
			
			public float amount;
			
			public int maxSize;

			
	}

    public void Awake()
    {
        prefabWeapon = new BaseWeapon[weaponNames.Length];
        for (int i =0;i<weaponNames.Length;i++)
        {

            GameObject resourceGameObject = null;
            if (PhotonResourceWrapper.allobject.ContainsKey(weaponNames[i]))
            {
                resourceGameObject = PhotonResourceWrapper.allobject[ weaponNames[i]];
            }
            else
            {


                resourceGameObject = (GameObject)Resources.Load( weaponNames[i], typeof(GameObject));



            }
            prefabWeapon[i] = resourceGameObject.GetComponent<BaseWeapon>() ;
        }
    }
	
	public class WeaponBackUp {
	
			public  AMMOTYPE type;
			
			public int amount;
			
			public WeaponBackUp(int inAmount, AMMOTYPE intType){
				amount =inAmount;
				type=intType;
			}
			public WeaponBackUp(WeaponBackUp oldInfo){
				amount =oldInfo.amount;
				type=oldInfo.type;
			}	
			
		
			
	}
	
	public WeaponBackUp[] weaponInfo;
	
	
	private AmmoBag[] allAmmo;
	 	

	public virtual void Init(){
		if (owner == null) {
			owner = GetComponent<Pawn> ();
			if (owner.foxView.isMine) {
				
				GenerateBag ();
				GenerateInfo ();
				
			}
		}
	}
	//Start Weapon generation
	public void GenerateWeaponStart(){
		if (owner.foxView.isMine) {
			for(int i=0;i<prefabWeapon.Length;i++){
				if(prefabWeapon[i].slotType==BaseWeapon.SLOTTYPE.MAIN){
						_ChangeWeapon (i);
				}
			}
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

            //Debug.Log(((float)allAmmo[i].maxSize) * percent/100);
                allAmmo[i].amount +=((float)allAmmo[i].maxSize) * percent/100;
                if (allAmmo[i].amount > allAmmo[i].maxSize)
                {
                    allAmmo[i].amount = allAmmo[i].maxSize;
                  
                }
            
        }

    }
	
	//AMMO BAG SECTION END
	
	
	//INGAMEINFO SECTION
	/*Generate cahche info for bag
		We don't store all weapon just important info about it when player put  weapon down
	*/


	protected void 	GenerateInfo(){
		weaponInfo = new WeaponBackUp[prefabWeapon.Length];
		for(int i=0;i<prefabWeapon.Length;i++){
			weaponInfo[i]=new WeaponBackUp(prefabWeapon[i].clipSize,prefabWeapon[i].ammoType);
		
		}
	}
	//save old info about weapon
	void SaveOldInfo(int index,BaseWeapon gun){
		//Debug.Log (index);
		weaponInfo[index].amount  = gun.curAmmo;
	
	}
	//load cur weapon info
	void LoadOldInfo(){
		LoadOldInfo(indexWeapon,currentWeapon);
	}
	//Load needed weapon info
	void LoadOldInfo(int index,BaseWeapon gun){
        if (weaponInfo[index].amount == prefabWeapon[index].clipSize)
        {
			gun.curAmmo=gun.clipSize;
		}else{
			gun.curAmmo=weaponInfo[index].amount;
		}
		
	}
	
	
	//INGAMEINFO SECTION END
	
	
	
	//WEAPON CHANGE SECTION
	
	public void SetSlot( BaseWeapon prefab){
        GA.API.Design.NewEvent("Game:Weapon:Choice:" + prefab.SQLId);
		for(int i=0;i<prefabWeapon.Length;i++){
			if(prefabWeapon[i].slotType==prefab.slotType){
				prefabWeapon[i]=prefab;
				weaponInfo[i]=new WeaponBackUp(prefabWeapon[i].clipSize, prefabWeapon[i].ammoType);
				return;
			}
		}
		BaseWeapon[] oldprefabWeapon = prefabWeapon;
		WeaponBackUp[] oldweaponInfo = weaponInfo;
		prefabWeapon = new BaseWeapon[oldprefabWeapon.Length+1];
		weaponInfo = new WeaponBackUp[prefabWeapon.Length];
		for(int i=0;i<oldprefabWeapon.Length;i++){
			prefabWeapon[i]= oldprefabWeapon[i];
			weaponInfo[i]=new WeaponBackUp(oldweaponInfo[i]);
			
		}
		prefabWeapon[prefabWeapon.Length-1] = prefab;
		weaponInfo[prefabWeapon.Length-1]=new WeaponBackUp(prefabWeapon[prefabWeapon.Length-1].clipSize, prefabWeapon[prefabWeapon.Length-1].ammoType);
	}
	
	
	//Change weapon in inventory 
	public void ChangePrefab(BaseWeapon newWeapon,WeaponBackUp weaponAddInfo){
		for(int i=0;i<prefabWeapon.Length;i++){
			if(prefabWeapon[i].slotType==newWeapon.slotType){
				DropWeapon(prefabWeapon[i],weaponInfo[i]);
				prefabWeapon[i]=newWeapon;
				weaponInfo[i]=new WeaponBackUp(prefabWeapon[i].clipSize, prefabWeapon[i].ammoType);
				_ChangeWeapon(i);
				return;
			}
		}
		BaseWeapon[] oldprefabWeapon = prefabWeapon;
		WeaponBackUp[] oldweaponInfo = weaponInfo;
		prefabWeapon = new BaseWeapon[oldprefabWeapon.Length+1];
		weaponInfo = new WeaponBackUp[prefabWeapon.Length];
		for(int i=0;i<oldprefabWeapon.Length;i++){
			prefabWeapon[i]= oldprefabWeapon[i];
			weaponInfo[i]=new WeaponBackUp(oldweaponInfo[i]);
			
		}
		prefabWeapon[prefabWeapon.Length-1] = newWeapon;
		weaponInfo[prefabWeapon.Length-1]=weaponAddInfo;;
		return;
	}
		//Change weapon in inventory 
	public void ChangePrefab(BaseWeapon newWeapon){
		ChangePrefab (newWeapon, new WeaponBackUp (newWeapon.clipSize, newWeapon.ammoType));
	}
	//implementation of dropping weapon on ground after picking another one 
	void DropWeapon(BaseWeapon oldWeapon,WeaponBackUp weaponinfo){

		GameObject droppedWeapon =NetworkController.Instance.SimplePrefabSpawn(oldWeapon.pickupPrefabPrefab.name,transform.position,transform.rotation);
		WeaponPicker picker = droppedWeapon.GetComponent<WeaponPicker>();
		picker.SetNewData (weaponinfo);
		//picker.info =weaponinfo;

	}
	
	public void NextWeapon(){
		int newIndex = indexWeapon+1;

		if(newIndex>=prefabWeapon.Length){
			newIndex=0;
		}
		//Debug.Log ("NextWeapon"+newIndex);
        //cahcedIndex = newIndex;
       // owner.animator.SetWeaponType(prefabWeapon[cahcedIndex].animType);
		_ChangeWeapon(newIndex);
	}
	public void PrevWeapon(){
		int newIndex = indexWeapon-1;
		if(newIndex<0){
			newIndex=prefabWeapon.Length-1;
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
		if(prefabWeapon.Length<=newWeapon){
			Debug.Log("Selected weapon doesn't exist in current inventory manager");
			return;
		}
		BaseWeapon firstWeapon;
        if (owner.foxView.isChildScene())
        {
            firstWeapon = NetworkController.Instance.WeaponSpawn(prefabWeapon[newWeapon].name, transform.position, Quaternion.identity,true,owner.foxView.viewID).GetComponent<BaseWeapon>();
            //Debug.Log("Turret weapon spawn");
        }
        else
        {
            firstWeapon = NetworkController.Instance.WeaponSpawn(prefabWeapon[newWeapon].name, transform.position, Quaternion.identity,false,owner.foxView.viewID).GetComponent<BaseWeapon>();
        }

	
	
		if(currentWeapon!=null){
			if(indexWeapon!=newWeapon){
				SaveOldInfo(indexWeapon,currentWeapon);
			}
			currentWeapon.RequestKillMe();
		}

		//TakeWeaponAway ();
		indexWeapon=newWeapon;
		currentWeapon=firstWeapon;
		owner.setWeapon(firstWeapon);
		LoadOldInfo();
			

	}
	//WEAPON SECTION END
}
