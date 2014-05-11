using UnityEngine;
using System;

public enum AMMOTYPE{PISTOL,RIFLE,ROCKETS,MACHINEGUN,SHOOTGUNSHEELL,FUEL};

[RequireComponent (typeof (Pawn))]
public class InventoryManager : MonoBehaviour {

	public BaseWeapon[] prefabWeapon;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
	private Pawn owner;
	
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
	 	

	void Start(){
		owner = GetComponent<Pawn>();
		if (owner.photonView.isMine) {
		
			GenerateBag ();
			GenerateInfo ();
			_ChangeWeapon (0);
		}
	}
	
	//AMMO BAG SECTION
	
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
	
	public bool HasAmmo(AMMOTYPE ammo){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				return allAmmo[i].amount>0;
			}
		}
		return false;
	}
	public int GetAmmo(AMMOTYPE ammo){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				return allAmmo[i].amount;
			}
		}
		return 0;
	}
	public int GiveAmmo(AMMOTYPE ammo,int amount){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				if(allAmmo[i].amount>amount){
					allAmmo[i].amount-=amount;
					return amount;
				}else{
					allAmmo[i].amount=0;
					return allAmmo[i].amount;
				}
			}
		}
		return 0;
	}
	
	public void AddAmmo(AMMOTYPE ammo,int amount){
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
	
	void 	GenerateInfo(){
		weaponInfo = new WeaponBackUp[prefabWeapon.Length];
		for(int i=0;i<prefabWeapon.Length;i++){
			weaponInfo[i]=new WeaponBackUp(prefabWeapon[i].clipSize,prefabWeapon[i].ammoType);
		
		}
	}
	void SaveOldInfo(int index,BaseWeapon gun){
		//Debug.Log (index);
		weaponInfo[index].amount  = gun.curAmmo;
	
	}
	void LoadOldInfo(){
		LoadOldInfo(indexWeapon,currentWeapon);
	}
	
	void LoadOldInfo(int index,BaseWeapon gun){
		gun.curAmmo=weaponInfo[index].amount;
		
	}
	
	
	//INGAMEINFO SECTION END
	
	
	
	//WEAPON CHANGE SECTION
	

	
	
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
	//TODO: implementation of dropping weapon on ground after picking another one 
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
		firstWeapon =PhotonNetwork.Instantiate(prefabWeapon[newWeapon].name,transform.position,Quaternion.identity,0).GetComponent<BaseWeapon>();
	
		owner.setWeapon(firstWeapon);
		if(currentWeapon!=null){
			if(indexWeapon!=newWeapon){
				SaveOldInfo(indexWeapon,currentWeapon);
			}
			currentWeapon.RequestKillMe();
		}
		indexWeapon=newWeapon;
		currentWeapon=firstWeapon;
		owner.setWeapon(firstWeapon);
		LoadOldInfo();
			
			

	}
	//WEAPON SECTION END
}
