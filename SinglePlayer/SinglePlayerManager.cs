using UnityEngine;
using System.Collections;
using System;

public class SinglePlayerManager : PlayerManager {

	// Use this for initialization

    public override Pawn SpawmPlayer(String newPalyerClass, int team, int[] stims)
    {
       
        Transform targetPos = GetSpamPosition(team);



        GameObject go = NetworkController.Instance.SpawnSinglePlayerPrefab(newPalyerClass, targetPos.position, targetPos.rotation);
        Pawn pawn = go.GetComponent<Pawn>();

        pawn.foxView.SetMine(true);
        pawn.AfterAwake();
        return pawn;
    }

    public override Transform GetSpamPosition(int team)
    {
        SpawnPoint spamPoint = FindObjectOfType<SpawnPoint>();
        return spamPoint.transform;
    }
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
