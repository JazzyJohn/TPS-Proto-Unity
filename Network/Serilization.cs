﻿using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Protocol.Serialization;

namespace nstuff.juggerfall.extension.player{

    [Serializable]
    public class Player : SerializableSFSType
    {
        public string uid;

        public string name;

        public int kill;

        public int death;

        public int assist;

        public int robotKill;
	
	    public int team;

        public int userId;

        public Player()
        {

        }   
    }

}

namespace nstuff.juggerfall.extension.pawn
{

    [Serializable]
    public class Pawn : SerializableSFSType
    {
		public int id;
	 
        public string type;

        public int wallState;

        public int team;

        public int characterState;

        public bool active;
		
		public int  pawnType;

        public bool isDead;

        public Pawn()
        {

        }
    }

}
namespace nstuff.juggerfall.extension.weapon
{

    [Serializable]
    public class Weapon : SerializableSFSType
    {
		public int id;
	 
        public string type;

        public Weapon()
        {

        }
    }

}