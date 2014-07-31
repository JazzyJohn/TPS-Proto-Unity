using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Protocol.Serialization;

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

}