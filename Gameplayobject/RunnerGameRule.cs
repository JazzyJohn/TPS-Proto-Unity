﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class RoomScenarioEntry{
	public int maxRoomNumber;
	public BIOMS planedBiom;	
}

[Serializable]
public class RoomTypeSetting{
	public int weidth;
	public ROOMTYPE type;	
}

public class RunnerGameRule : GameRule {
    public Generation generator;
    public int StartRoomCnt;
    protected int roomCnt;
	public RoomScenarioEntry[] scenario;
	public RoomTypeSetting[] settings;
	private Dictionary<ROOMTYPE,int> typeDictionary  = new Dictionary<ROOMTYPE,int>();
	
    public override void StartGame()
    {
        generator = GetComponent<Generation>();
        generator.CacheBaseLoad();
        generator.Next(StartRoomCnt);
        LoadType();
        base.StartGame();
    }
    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats stats = new PlayerMainGui.GameStats();
        stats.gameTime = gameTime - timer;
        stats.score = new int[] { (int)roomCnt, 0 };
        stats.maxScore = maxScore;
        return stats;

    }
    public void NextRoom()
    {
        EventHolder.instance.FireEvent(typeof(GameListener), "EventRoomFinished");
        roomCnt++;
    }
    void Update()
    {
     
        if (isGameEnded)
        {

            restartTimer += Time.deltaTime;
            if (restartTimer > restartTime && !lvlChanging)
            {
                lvlChanging = true;
                FindObjectOfType<ServerHolder>().LoadNextMap();
            }
        }
        
    }
     public override void PlayerDeath()
    {
        Debug.Log("Palyer Death");
		EventHolder.instance.FireEvent(typeof(GameListener), "EventRestart");
        isGameEnded = true;
    }
	public void ChangeCondition(GeneratorCondition condition,int roomCount){
        if (condition.byTypeWeight == null)
        {
            LoadType();
        }
		condition.currentBiom = scenario[0].planedBiom;
		foreach(RoomScenarioEntry scenarioEntry in  scenario){
			if(roomCount<scenarioEntry.maxRoomNumber){
           
				condition.currentBiom = scenarioEntry.planedBiom;
				break;
			}
		}
		condition.byTypeWeight = typeDictionary;
		
	}
    void LoadType()
    {
        foreach (RoomTypeSetting typeSetting in settings)
        {
            typeDictionary[typeSetting.type] = typeSetting.weidth;
        }
    }
}
