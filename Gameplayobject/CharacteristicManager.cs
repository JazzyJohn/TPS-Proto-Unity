using UnityEngine;
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
        SPRINT_SPEED,
		MAXHEALTH,
		WALLSPEED,
		JUMPHEIGHT,
		ARMOR,
		STANDFORCE,
		STANDFORCEAIR,
	    JETPACKCHARGE,
		JUGGER_DAMAGE_REDUCE,
		
		PLAYER_JUGGER_TIME,
		PLAYER_JUGGER_KILL_BONUS,
		
		//PASSIVESKILL SECTION
        MAXHEALTH_PERCENT,

		RELOAD_SPEED,
		RECOIL_ALL,
        RECOIL_DOP,
        RECOIL_MAIN,
		RECOIL_SHOOTGUN,
		RECOIL_REVOLVER,

        AIM_ALL,
        AIM_DOP,
        AIM_MAIN,

		FIRE_RATE,
        FIRE_RATE_MAIN,
		
		DAMAGE_ALL,
		DAMAGE_REDUCE_SPLASH,
		DAMAGE_REDUCE_GUN,
		DAMAGE_REDUCE_ALL,
        DAMAGE_REDUCE_HEAD,

        
        DAMAGE_ADD_DOP,
        DAMAGE_ADD_MAIN,
        
        DAMAGE_ADD_DOP_POINT,
        DAMAGE_ADD_MAIN_POINT,

        VS_ARMOR_DOP,
        VS_ARMOR_MAIN,

        DISTANCE_MAIN,

        MAX_WEAR,
        WEAR,
		
		AMMO_AMOUNT,

        KILL_REFILE,

        HEAD_BLOCK,

        REGEN,
    
        CRIT_CHANCE_ALL,
        CRIT_CHANCE_MAIN
		
		
}
public class BaseEffect{
	public int timeEnd=-1;
	public EffectType type =EffectType.ADD;
	public bool endByDeath= true;
	public bool initalEffect =false;
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
        //Debug.Log(timeUpdate + "  " + newEffect.timeEnd);
		if(newEffect.timeEnd!=-1&&(timeUpdate>newEffect.timeEnd||timeUpdate<Time.time)){
			timeUpdate = 	newEffect.timeEnd;
           // Debug.Log(timeUpdate+"  "+Time.time);
		}
		effectList.Add(newEffect);
		needUpdate= true;
	}
	public  List<BaseEffect> GetEffect(){
		return effectList;
	}
	
	public bool UpdateList(){
		if(timeUpdate>timeLastUpdate&&timeUpdate<Time.time){
          //  Debug.Log(timeUpdate + "  " + Time.time);
            int count = effectList.Count;
			timeLastUpdate = Time.time;
			effectList.RemoveAll(delegate(BaseEffect eff) {
              //  Debug.Log(eff.timeEnd + "  " + Time.time);
                return eff.timeEnd != -1 && eff.timeEnd <= Time.time;
			});
            float minTime = float.MaxValue;
            foreach (BaseEffect eff in effectList)
            {
                if (eff.timeEnd != -1 && eff.timeEnd < minTime)
                {
                    minTime = eff.timeEnd;
                }
            }
            if (minTime != float.MaxValue)
            {
                timeUpdate = minTime;
                //Debug.Log(timeUpdate + "  " + Time.time);
            }
            needUpdate = true;
            if (count != effectList.Count)
            {
                return true;
            }
          
		}
        return false;
	
	}
	public void UpdateListByDeath(){
	
			timeLastUpdate = Time.time;
			effectList.RemoveAll(delegate(BaseEffect eff) {
                if (eff.endByDeath)
                {
                  ///  Debug.Log("DELETEBY Death");
                }
                return eff.endByDeath;
			});
		
	
		
	
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
	public void Remove(T value){
		effectList.RemoveAt(effectList.FindIndex(delegate(BaseEffect eff) {
                if (!eff.initalEffect)
                {
					Effect<T> realEff = eff as Effect<T> ;
					return realEff.value.Equals(value);
                }
				return false;
				
               
			}));
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
        bool askOwner=false;
		for(int  i=0; i<allCharacteristic.Length;i++){
			if(allCharacteristic[i]!=null){
                askOwner = askOwner||allCharacteristic[i].UpdateList();
			}
		}
        if (askOwner)
        {
          
            GetComponent<Pawn>().ReloadStats();
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
	public void AddEffect(int characteristic, object value){
		
	
		FloatCharacteristic floatCharacteristic=allCharacteristic [characteristic]as FloatCharacteristic ;
        if (floatCharacteristic != null)
        {

            Effect<float> effect = new Effect<float>((float)value);	
			floatCharacteristic.AddEffect(effect);
			return;
		}
	
		BoolCharacteristic boolCharacteristic=allCharacteristic [characteristic]as BoolCharacteristic ;
        if (boolCharacteristic != null)
        {
            Effect<bool> effect = new Effect<bool>((bool)value);
            boolCharacteristic.AddEffect(effect);
			return;
		}

        IntCharacteristic intCharacteristic = allCharacteristic[characteristic] as IntCharacteristic;
        if (intCharacteristic != null)
        {
            Effect<int> effect = new Effect<int>((int)value);
            intCharacteristic.AddEffect(effect);
			return;
		}
	}
    public void AddBaseEffect(int characteristic, BaseEffect value)
    {
        if (allCharacteristic[characteristic] == null)
        {
            Effect<float> floatEffect = value as Effect<float>;
            if (floatEffect != null)
            {
                FloatCharacteristic Characteristic = new FloatCharacteristic(0);
                Characteristic.AddEffect(floatEffect);

                allCharacteristic[characteristic] = Characteristic;
                return;
            }
            Effect<int> intEffect = value as Effect<int>;
            if (intEffect != null)
            {
                IntCharacteristic Characteristic = new IntCharacteristic(0);
                Characteristic.AddEffect(intEffect);
                allCharacteristic[characteristic] = Characteristic;
                return;
            }
            Effect<bool> boolEffect = value as Effect<bool>;
            if (boolEffect != null)
            {
                BoolCharacteristic Characteristic = new BoolCharacteristic(false);
                Characteristic.AddEffect(boolEffect);
                allCharacteristic[characteristic] = Characteristic;
                return;
            }
        }
        else
        {
            FloatCharacteristic floatCharacteristic = allCharacteristic[characteristic] as FloatCharacteristic;
            if (floatCharacteristic != null)
            {
                floatCharacteristic.AddEffect((Effect<float>)value);
                return;
            }
            IntCharacteristic intCharacteristic = allCharacteristic[characteristic] as IntCharacteristic;
            if (intCharacteristic != null)
            {
                intCharacteristic.AddEffect((Effect<int>)value);
                return;
            }
            BoolCharacteristic boolCharacteristic = allCharacteristic[characteristic] as BoolCharacteristic;
            if (boolCharacteristic != null)
            {
                boolCharacteristic.AddEffect((Effect<bool>)value);
                return;
            }
        }
    }
		
	public void RemoveEffect(int characteristic, object value){
		FloatCharacteristic floatCharacteristic=allCharacteristic [characteristic]as FloatCharacteristic ;
        if (floatCharacteristic != null)
        {
			floatCharacteristic.Remove((float)value);
		}
		BoolCharacteristic boolCharacteristic=allCharacteristic [characteristic]as BoolCharacteristic ;
        if (boolCharacteristic != null)
        {
			boolCharacteristic.Remove((bool)value);
		}
        IntCharacteristic intCharacteristic = allCharacteristic[characteristic] as IntCharacteristic;
        if (intCharacteristic != null)
        {
            intCharacteristic.Remove((int)value);
		}
	}
	
	
	
	public void AddList(List<CharacteristicToAdd> effects){
		foreach(CharacteristicToAdd add in effects){

           
            if (allCharacteristic[(int)add.characteristic] == null)
            {
                Effect<float> floatEffect = add.addEffect as Effect<float>;
                if (floatEffect != null)
                {
                    FloatCharacteristic Characteristic= new FloatCharacteristic(0);
                    Characteristic.AddEffect(floatEffect);
                    
                    allCharacteristic[(int)add.characteristic] = Characteristic;
                    continue;
                }
                Effect<int> intEffect = add.addEffect as Effect<int>;
                if (intEffect != null)
                {
                    IntCharacteristic Characteristic = new IntCharacteristic(0);
                    Characteristic.AddEffect(intEffect);
                    allCharacteristic[(int)add.characteristic] = Characteristic;
                    continue;
                }
                Effect<bool> boolEffect = add.addEffect as Effect<bool>;
                if (boolEffect != null)
                {
                    BoolCharacteristic Characteristic = new BoolCharacteristic(false);
                    Characteristic.AddEffect(boolEffect);
                    allCharacteristic[(int)add.characteristic] = Characteristic;
                    continue;
                }
            }
            else
            {
                FloatCharacteristic floatCharacteristic = allCharacteristic[(int)add.characteristic] as FloatCharacteristic;
                if (floatCharacteristic != null)
                {
                    floatCharacteristic.AddEffect((Effect<float>)add.addEffect);
                    continue;
                }
                IntCharacteristic intCharacteristic = allCharacteristic[(int)add.characteristic] as IntCharacteristic;
                if (intCharacteristic != null)
                {
                    intCharacteristic.AddEffect((Effect<int>)add.addEffect);
                    continue;
                }
                BoolCharacteristic boolCharacteristic = allCharacteristic[(int)add.characteristic] as BoolCharacteristic;
                if (boolCharacteristic != null)
                {
                    boolCharacteristic.AddEffect((Effect<bool>)add.addEffect);
                    continue;
                }
            }
		}
	}
	public List<CharacteristicToAdd>  GetCharacteristick(){
		List<CharacteristicToAdd> answer = new List<CharacteristicToAdd>();
      
		for(int i=0; i<arraySize; i++ ){
      
            if (allCharacteristic[i] == null)
            {
                continue;
            }
            //Debug.Log("GetCharacteristick" + (CharacteristicList)i);
			List<BaseEffect> all =allCharacteristic[i].GetEffect();
            foreach (BaseEffect eff in all)
            {
              // Debug.Log(i + " " + eff.type);
				answer.Add(new CharacteristicToAdd((CharacteristicList)i,eff));
			}
			
		}
		return answer;		
	}
	
}
