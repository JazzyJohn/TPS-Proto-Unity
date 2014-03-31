using UnityEngine;
using System;

public enum AMMOTYPE{ PISTOL,RIFLE};

[RequireComponent (typeof (Pawn))]
public class InventoryManager : MonoBehaviour {

	public BaseWeapon[] prefabWeapon;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
	private Pawn owner;
	
	[Serializable]
	public class AmmoType {
	
			public  AmmoType type;
			
			public int amount;
			
			public int maxSize;

			
	}
	
	
	public class WeaponBackUp {
	
			public  AmmoType type;
			
			public int amount;
			
			public WeaponBackUp(int inAmount, AmmoType intType){
				amount =inAmount;
				type=intType;
			}
			public WeaponBackUp(WeaponBackUp oldInfo){
				amount =oldInfo.amount;
				type=oldInfo.type;
			}	
			
		
			
	}
	
	public WeaponBackUp[] weaponInfo;
	
	
	private AmmoType[] allAmmo;
	 	
	
	void Start(){
		owner= GetComponent<Pawn>;
		GenerateBag();
		GenerateInfo();
		ChangeWeaoon(0);
	}
	
	//AMMO BAG SECTION
	
	void GenerateBag(){
		AmmoType[] allTypeInGame = PlayerManager.instance.AllTypeInGame;
		allAmmo = new AmmoType[allTypeInGame.Length];
		for(int i = 0;i<allTypeInGame.Length;i++){
			allAmmo[i] = new  AmmoType();
			allAmmo[i].type = allTypeInGame[i].type;
			allAmmo[i].amount = allTypeInGame[i].maxSize;
			allAmmo[i].maxSize = allTypeInGame[i].maxSize;
		}
	}
	
	public bool HasAmmo(AMMOTYPE ammo){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				return alllAmmo[i].amount>0;
			}
		}
		return false;
	}
	public int GiveAmmo(AMMOTYPE ammo,int amount){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				if(alllAmmo[i].amount>amount){
					alllAmmo[i].amount-=amount;
					return amount;
				}else{
					alllAmmo[i].amount=0;
					return alllAmmo[i].amount;
				}
			}
		}
		return 0;
	}
	
	public void AddAmmo(AMMOTYPE ammo,int amount){
		for(int i=0;i<allAmmo.Length;i++){
			if(allAmmo[i].type ==ammo){
				alllAmmo[i].amount+=amount;
				if(alllAmmo[i].amount>alllAmmo[i].maxSize){
					alllAmmo[i].amount=alllAmmo[i].maxSize;
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
	void AddOldClip(int index,BaseWeapon gun){
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
		ChangePrefab(newWeapon,new WeaponBackUp(newWeapon.clipSize,newWeapon.ammoType))
	}
	//TODO: implementation of dropping weapon on ground after picking another one 
	void DropWeapon(BaseWeapon oldWeapon,WeaponBackUp weaponinfo){
		GameObject droppedWeapon =PhotonNetwork.Instantiate(oldWeapon.pickupPrefabPrefab,transform.position,transform.rotation,0) as GameObject;
		OldWeaponPicker picker = droppedWeapon.GetComponent<OldWeaponPicker>;
		picker.info =weaponinfo;
		picker.prefabWeapon =oldWeapon;
		picker.isOneUse = true;
	}
	
	public void NextWeapon(){
		int newIndex = indexWeapon+1;
		if(newIndex>=prefabWeapon.Length){
			newIndex=0;
		}
		ChangeWeapon(newIndex);
	}
	public void PrevWeapon(){
		int newIndex = indexWeapon-1;
		if(newIndex<0){
			newIndex=prefabWeapon.Length;
		}
		ChangeWeapon(newIndex);
	}
	//Change weapon in hand
	public void ChangeWeaoon(int newWeapon){
		if(prefabWeapon.Length<=newWeapon){
			Debug.Log("Selected weapon doesn't exist in current inventory manager");
			return;
		}
			BaseWeapon firstWeapon;
			if(Network.connections.Length==0){
				firstWeapon =Instantiate(prefabWeapon[newWeapon]) as BaseWeapon;
			}else{
				firstWeapon =Network.Instantiate(prefabWeapon[newWeapon],Vector3.zero,Quaternion.identity,0) as BaseWeapon;
			}
			indexWeapon=newWeapon;
			owner.setWeapon(firstWeapon);
			if(currentWeapon!=null){
				SaveOldInfo(indexWeapon-1,currentWeapon);
				Destroy(currentWeapon.gameObject);
			}
			currentWeapon=firstWeapon;
			owner.setWeapon(firstWeapon);
			LoadOldInfo();
			
			
		}
	}
	//WEAPON SECTION END
}
