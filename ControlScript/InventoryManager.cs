using UnityEngine;
using System;

public enum AMMOTYPE{PISTOL,RIFLE,ROCKETS,MACHINEGUN,SHOOTGUNSHEELL,FUEL};

[RequireComponent (typeof (Pawn))]
public class InventoryManager : MonoBehaviour {

	public BaseWeapon[] prefabWeapon;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
	protected Pawn owner;
	
	[Serializable]
	public class AmmoBag {
	
			public  AMMOTYPE type;
			
			public int amount;
			
			public int maxSize;

			
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
			if (owner.photonView.isMine) {
				
				GenerateBag ();
				GenerateInfo ();
				
			}
		}
	}
	//Start Weapon generation
	public void GenerateWeaponStart(){
		if (owner.photonView.isMine) {
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
	void GenerateBag(){
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
				return allAmmo[i].amount;
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
					amount =allAmmo[i].amount;
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
		gun.curAmmo=weaponInfo[index].amount;
		
	}
	
	
	//INGAMEINFO SECTION END
	
	
	
	//WEAPON CHANGE SECTION
	
	public void SetSlot( BaseWeapon prefab){
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

		GameObject droppedWeapon =PhotonNetwork.Instantiate(oldWeapon.pickupPrefabPrefab.name,transform.position,transform.rotation,0) as GameObject;
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
		_ChangeWeapon(newIndex);
	}
	public void PrevWeapon(){
		int newIndex = indexWeapon-1;
		if(newIndex<0){
			newIndex=prefabWeapon.Length-1;
		}
		//Debug.Log ("PrevWeapon"+newIndex);
		_ChangeWeapon(newIndex);
	}
	//Change weapon in hand
	public void ChangeWeapon(int newWeapon){
		if (indexWeapon != newWeapon) {
			_ChangeWeapon(newWeapon);	
		}
	}

	protected void _ChangeWeapon(int newWeapon){
		if(prefabWeapon.Length<=newWeapon){
			Debug.Log("Selected weapon doesn't exist in current inventory manager");
			return;
		}
		BaseWeapon firstWeapon;
        if (owner.photonView.isSceneView)
        {
            firstWeapon = PhotonNetwork.InstantiateSceneObject(prefabWeapon[newWeapon].name, transform.position, Quaternion.identity, 0, null).GetComponent<BaseWeapon>();
            Debug.Log("Turret weapon spawn");
        }
        else
        {
            firstWeapon = PhotonNetwork.Instantiate(prefabWeapon[newWeapon].name, transform.position, Quaternion.identity, 0).GetComponent<BaseWeapon>();
        }

	
		owner.setWeapon(firstWeapon);
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
