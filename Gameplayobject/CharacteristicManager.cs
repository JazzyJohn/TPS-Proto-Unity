﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//TODO GENERIC TYPE  CHARCTERISTIC CLASS + Interface Implament ob base Class

public enum EffectType{
	MULIPLIE,
	ADD,
	ASSIGN
}
public enum CharacteristicList{
		SPEED,
		MAXHEALTH,
		WALLSPEED,
		JUMPHEIGHT,
		ARMOR,
		STANDFORCE,
		STANDFORCEAIR,
	    JETPACKCHARGE,
		
		
		PLAYER_JUGGER_TIME,
		PLAYER_JUGGER_KILL_BONUS
}
public class BaseEffect{
	public int timeEnd;
	public EffectType type;
	public bool endByDeath= true;
}
public class Effect<T>:BaseEffect{
	public T value;
	public Effect(T value){
		this.value  =value;
	}
	
}
public class BaseCharacteristic{
	protected List<BaseEffect> effectList  = new List<BaseEffect>();
	protected float timeUpdate =0;
	protected float timeLastUpdate =-1;
	protected bool needUpdate = true;
	protected void AddEffect(BaseEffect newEffect){
		if(newEffect.timeEnd!=-1&&timeUpdate>newEffect.timeEnd){
			timeUpdate = 	newEffect.timeEnd;

		}
		effectList.Add(newEffect);
		needUpdate= true;
	}
	public  List<BaseEffect> GetEffect(){
		return effectList;
	}
	
	public void UpdateList(){
		if(timeUpdate>timeLastUpdate&&timeUpdate<Time.time){
			timeLastUpdate = Time.time;
			effectList.RemoveAll(delegate(BaseEffect eff) {
			return eff.timeEnd <Time.time;
			});
			needUpdate= true;
		}
		
	
	}
	public void UpdateListByDeath(){
		if(timeUpdate>timeLastUpdate&&timeUpdate<Time.time){
			timeLastUpdate = Time.time;
			effectList.RemoveAll(delegate(BaseEffect eff) {
			return  endByDeath
			});
			needUpdate= true;
		}
		
	
	}
	
}
public class Characteristic<T> : BaseCharacteristic {
	public T startValue;
	protected T tempValue;
	public Characteristic(T init){
		startValue = init;
	}
	public Characteristic(){

	}
	public void AddEffect(Effect<T> newEffect){
		base.AddEffect(newEffect);
	}

	public T GetValue(){
		if(needUpdate){
			ReCount();
			needUpdate= false;
		}
		return tempValue;
	}
	virtual public void ReCount(){
		tempValue = startValue;

	}
}
public class IntCharacteristic :Characteristic<int> {
	public IntCharacteristic(int init){
		startValue = init;
	}
	override public void ReCount(){
		tempValue = startValue;
		foreach (BaseEffect eff in effectList) {
			Effect<int> curEffect = (Effect<int>) eff;
			if(curEffect!=null){
				switch(curEffect.type){
				case EffectType.ADD:	
					tempValue=tempValue +curEffect.value;
					break;
				case EffectType.MULIPLIE:	
					tempValue*=curEffect.value;
					break;
				case EffectType.ASSIGN:
					tempValue=curEffect.value;
					return;
				}
				
			}
		}
		
	}


}
public class BoolCharacteristic :Characteristic<bool> {
	public BoolCharacteristic(bool init){
		startValue = init;
	}
	override public void ReCount(){
		tempValue = startValue;
		foreach (BaseEffect eff in effectList) {
			Effect<bool> curEffect = (Effect<bool>) eff;
			if(curEffect!=null){
				switch(curEffect.type){
				case EffectType.ADD:	
					tempValue=tempValue ||curEffect.value;
					break;
				case EffectType.MULIPLIE:	
					tempValue=tempValue&&curEffect.value;
					break;
				case EffectType.ASSIGN:
					tempValue=curEffect.value;
					return;
				}
				
			}
		}
		
	}
	
	
}
public class FloatCharacteristic :Characteristic<float> {
	public FloatCharacteristic(float init){
		startValue = init;
	}
	override public void ReCount(){
		tempValue = startValue;
		foreach (BaseEffect eff in effectList) {
			Effect<float> curEffect = (Effect<float>) eff;
			if(curEffect!=null){
				switch(curEffect.type){
				case EffectType.ADD:	
					tempValue=tempValue +curEffect.value;
					break;
				case EffectType.MULIPLIE:	
					tempValue*=curEffect.value;
					break;
				case EffectType.ASSIGN:
					tempValue=curEffect.value;
					return;
				}
				
			}
		}
		
	}
	
	
}

[Serializable]
public class StartIntCharacteristic{
	public int startValue;
	public CharacteristicList characteristic;
}
[Serializable]
public class StartBoolCharacteristic{
	public bool startValue;
	public CharacteristicList characteristic;
}
[Serializable]
public class StartFloatCharacteristic{
	public float startValue;
	public CharacteristicList characteristic;
}


public class CharacteristicToAdd{
	public CharacteristicList characteristic;
	public BaseEffect addEffect;
	public CharacteristicToAdd(CharacteristicList characteristic, BaseEffect addEffect){
		this.characteristic =characteristic;
		this.addEffect = addEffect;
	}
	public CharacteristicToAdd(){
	
	}
}
public class CharacteristicManager : MonoBehaviour {
	
	public StartIntCharacteristic[] startIntCharacteristic;
	public StartBoolCharacteristic[] startBoolCharacteristic;
	public StartFloatCharacteristic[] startFloatCharacteristic;
	
	
	protected BaseCharacteristic[] allCharacteristic;
	
	public int arraySize = 0;
	
	public 	void Init(){
		arraySize = Enum.GetValues (typeof(CharacteristicList)).Length;
		allCharacteristic = new BaseCharacteristic[arraySize];
		for(int  i=0; i<startIntCharacteristic.Length;i++){
			allCharacteristic[(int)startIntCharacteristic[i].characteristic] = new IntCharacteristic(startIntCharacteristic[i].startValue);
		}
		for(int  i=0; i<startBoolCharacteristic.Length;i++){
			allCharacteristic[(int)startBoolCharacteristic[i].characteristic] = new BoolCharacteristic(startBoolCharacteristic[i].startValue);
		}
		for(int  i=0; i<startFloatCharacteristic.Length;i++){
			allCharacteristic[(int)startFloatCharacteristic[i].characteristic] = new FloatCharacteristic(startFloatCharacteristic[i].startValue);
		}
	
	}
	void Update(){
		for(int  i=0; i<allCharacteristic.Length;i++){
			if(allCharacteristic[i]!=null){
				allCharacteristic[i].UpdateList();
			}
		}
	}
	public void DeathUpdate(){
		for(int  i=0; i<allCharacteristic.Length;i++){
			if(allCharacteristic[i]!=null){
				allCharacteristic[i].UpdateListByDeath();
			}
		}
	}
	public int GetIntChar(CharacteristicList characteristic){
			if (allCharacteristic [(int)characteristic] as IntCharacteristic == null) {
				return 0;	
			}
			return((IntCharacteristic)allCharacteristic[(int)characteristic]).GetValue();

	
	}
	public float GetFloatChar(CharacteristicList characteristic){
			if (allCharacteristic [(int)characteristic] as FloatCharacteristic == null) {
				return 0.0f;	
			}
			return((FloatCharacteristic)allCharacteristic[(int)characteristic]).GetValue();
	
	}
	public bool GetBoolChar(CharacteristicList characteristic){
			if (allCharacteristic [(int)characteristic] as BoolCharacteristic == null) {
				return false;	
			}
			return((BoolCharacteristic)allCharacteristic[(int)characteristic]).GetValue();
	
	}
	public void AddList(List<CharacteristicToAdd> effects){
		foreach(CharacteristicToAdd add in effects){
			FloatCharacteristic floatCharacteristic  = allCharacteristic [add.characteristic] as FloatCharacteristic;
			if(floatCharacteristic!=null){
				floatCharacteristic.AddEffect((Effect<float>)add.addEffect);
				continue;
			}
			IntCharacteristic intCharacteristic  = allCharacteristic [add.characteristic] as IntCharacteristic;
			if(intCharacteristic!=null){
				intCharacteristic.AddEffect((Effect<int>)add.addEffect);
				continue;
			}
			BoolCharacteristic boolCharacteristic  = allCharacteristic [add.characteristic] as BoolCharacteristic;
			if(boolCharacteristic!=null){
				boolCharacteristic.AddEffect((Effect<bool>)add.addEffect);
				continue;
			}
		
		}
	}
	public List<CharacteristicToAdd>  GetCharacteristick(){
		List<CharacteristicToAdd> answer = new List<CharacteristicToAdd>();
		for(int i=0; i<arraySize; i++ ){
			List<Effect> all =allCharacteristic[i].GetEffect();
			foreach(Effect eff in all){
			
				answer.Add(CharacteristicToAdd((CharacteristicList)i,eff));
			}
			
		}
		return answer;		
	}
	
}
