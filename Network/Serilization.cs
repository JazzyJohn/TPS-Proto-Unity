using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Protocol.Serialization;
using System.Collections.Generic;

namespace nstuff.juggerfall.extension.models
{

    [Serializable]
    public class PlayerModel : SerializableSFSType
    {
        public string uid;

        public string name;

        public int kill;

        public int aikill;

        public int death;

        public int assist;

        public int robotKill;
	
	    public int team;

        public int userId;
		
		public PlayerModel()
        {

        }   
    }
  

    [Serializable]
    public class PawnModel : SerializableSFSType
    {
        public int id;
	 
        public string type;

        public int wallState;

        public int team;

        public int characterState;

        public bool active;
		
	    public bool isAiming;

		public float health;

        public Vector3Model velocity = new Vector3Model();
		
		public Vector3Model position  = new Vector3Model();
		
		public Vector3Model aimRotation  = new Vector3Model();
		
		public QuaternionModel rotation = new QuaternionModel();

        public PawnModel()
        {

        }
    }


    [Serializable]
    public class WeaponModel : SerializableSFSType
    {
        public int id;
		 
        public string type;
		
		public bool state;

        public WeaponModel()
        {

        }
    }
    [Serializable]
    public class SimpleNetModel : SerializableSFSType
    {
        public int id;
		 
        public string type;

        public Vector3Model position = new Vector3Model();

       public QuaternionModel rotation = new QuaternionModel();

        public SimpleNetModel()
        {

        }
    }
    
	[Serializable]
    public class Vector3Model : SerializableSFSType
    {
		public float x;
		
		public float y;
		
		public float z;
		
		public Vector3Model(){

		}		
		
		public Vector3Model(Vector3 vector){
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}
		public void WriteVector(Vector3 vector){
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}
		public Vector3 MakeVector(Vector3 vector3){
			vector3.x = x;
			vector3.y = y;
			vector3.z = z;
			return vector3;
		}
		public Vector3 GetVector(){
			Vector3 vector = new Vector3();
			return MakeVector(vector);
		}
	}	
	
	[Serializable]
    public class QuaternionModel : SerializableSFSType
    {
		public float x;
		
		public float y;
		
		public float z;
		
		public float w;
		
		public QuaternionModel(){

		}		
		
		public QuaternionModel(Quaternion quaternion){
			x = quaternion.x;
			y = quaternion.y;
			z = quaternion.z;
            w = quaternion.w;
		}	
		
		public void WriteQuat(Quaternion quaternion){
			x = quaternion.x;
			y = quaternion.y;
			z = quaternion.z;
            w = quaternion.w;
		}	
		public  Quaternion MakeQuaternion( Quaternion quaternion){
			quaternion.x = x;
			quaternion.y = y;
			quaternion.z = z;
			quaternion.w = w;
			return quaternion;
		}
		public Quaternion GetQuat(){
			Quaternion quat = new Quaternion();
			return MakeQuaternion(quat);
		}
	}
    public static class FlagsHelper
    {
        public static bool IsSet<T>(T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        public static void Set<T>(ref T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue | flagValue);
        }

        public static void Unset<T>(ref T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue & (~flagValue));
        }
    }
    public enum DamageModifiers
    {
        NONE=0,
        HEADSHOT= 1,
        SPLASH = 2,
        GUN = 3,
        KNOCKOUT  =4,
        CONTINIUS = 5
    }
	[Serializable]
    public class BaseDamageModel : SerializableSFSType
    {
        public float damage;
	    public Vector3Model hitPosition;
        public Vector3Model pushDirection;
		public int weaponId;
		public int damageType;
        public float vsArmor;
        public int modifiers=0;
		 
        public BaseDamageModel()
        {

        }
		public BaseDamageModel(BaseDamage damage){
			this.damage =damage.Damage;
            if (damage.knockOut)
            {
                FlagsHelper.Set(ref modifiers, (int)DamageModifiers.KNOCKOUT);
            }
            if (damage.splash)
            {
                FlagsHelper.Set(ref modifiers, (int)DamageModifiers.SPLASH);
            }
            if (damage.isHeadshoot)
            {
                FlagsHelper.Set(ref modifiers, (int)DamageModifiers.HEADSHOT);
            }
            if (damage.weapon)
            {
                FlagsHelper.Set(ref modifiers, (int)DamageModifiers.GUN);
            }
			 if (damage.isContinius)
            {
                FlagsHelper.Set(ref modifiers, (int)DamageModifiers.CONTINIUS);
            }
			
		
			this.hitPosition= new Vector3Model(damage.hitPosition);
            this.weaponId = damage.shootWeapon;
			this.damageType = (int)damage.type;
            this.pushDirection = new Vector3Model(damage.pushDirection);
            this.vsArmor = damage.vsArmor;
		}	
		public BaseDamage GetDamage(){
            BaseDamage damageClass = new BaseDamage();
            damageClass.Damage = damage;
            damageClass.pushForce = 0;
            damageClass.knockOut = FlagsHelper.IsSet(modifiers, (int)DamageModifiers.KNOCKOUT);
            damageClass.isHeadshoot = FlagsHelper.IsSet(modifiers, (int)DamageModifiers.HEADSHOT);
            damageClass.splash = FlagsHelper.IsSet(modifiers, (int)DamageModifiers.SPLASH);
            damageClass.weapon = FlagsHelper.IsSet(modifiers, (int)DamageModifiers.GUN);
            damageClass.isContinius = FlagsHelper.IsSet(modifiers, (int)DamageModifiers.CONTINIUS);
            
            damageClass.pushDirection = pushDirection.GetVector();
            damageClass.hitPosition = hitPosition.GetVector();
			damageClass.shootWeapon =weaponId;
            damageClass.type = (DamageType)damageType;
           
            damageClass.vsArmor = vsArmor;
            return damageClass;
		}	
    }
	
	
    [Serializable]
    public class GameRuleModel : SerializableSFSType
    {
    }

    [Serializable]
    public class PVPGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;

        public ArrayList teamKill;
    }
	 [Serializable]
    public class PVPHuntGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;

       
	   
    }
	 [Serializable]
    public class PointGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;

       
    }
	 [Serializable]
    public class RunnerGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;

    }
	[Serializable]
    public class PVEGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;
		
		public int vipID;
    }
	[Serializable]
    public class HoldPositionGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;
		
	}
    [Serializable]
    public class PVPJuggerFightGameRuleModel : GameRuleModel
    {
        public bool isGameEnded;

        public ArrayList teamScore;

        public ArrayList baseHealth;
    }

    [Serializable]
    public class BaseModel:  SerializableSFSType
    {
        public int team;

        public int health;
    }

    [Serializable]
    public class SimpleDestroyableModel : SerializableSFSType
    {
        public float health;

        public int id;
    }
	[Serializable]
    public class ConquestPointModel  : SerializableSFSType{
		public int id;
		
		public int scorePoint;
		
		public int owner;
		
	}
	
	[Serializable]
    public class AssaultPointModel  : SerializableSFSType{
	
		public float scorePoint;
	
		public float needPoint;
		
		public int owner;
		
		public int id;
		
		public int people;
		
		public int teamConquering;
		
		public ArrayList lockedByOneTeam;
		
		public ArrayList lockedBySecondTeam;
	}
	[Serializable]
    public class GameSettingModel  : SerializableSFSType{
		public int teamCount;

		public int maxTime;
		
		public int maxScore;
		
		public Hashtable huntTable;
		
		public bool isWithPractice;
	}
	
	
}