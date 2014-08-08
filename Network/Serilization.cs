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
		
	    public bool isDead;
		
		public float health;
		
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
	
		public float health;
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
	
	
	
}