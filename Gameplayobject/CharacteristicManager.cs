using UnityEngine;
using System.Collections;
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
}
class BaseEffect{
	public int timeEnd;
	public EffectType type;
	
}
class Effect<T>:BaseEffect{
	public T value;
	
}
class BaseCharacteristic{
	protected List<BaseEffect> effectList  = new List<BaseEffect>();
	protected int timeUpdate =0;
	protected int timeLastUpdate =-1;
	protected void AddEffect(BaseEffect newEffect){
		if(newEffect.TimeEnd!=-1&&timeUpdate>newEffect.TimeEnd){
			timeUpdate = 	newEffect.TimeEnd;
		}
		effectList.Add(newEffect);
		
	}
	
	public void UpdateList(){
		if(timeUpdate>timeLastUpdate&&timeUpdate<Time.time){
			timeLastUpdate = Time.time;
			effectList.RemoveAll(delegate(BaseEffect eff) {
			return eff.timeEnd <Time.time;
			});
		}
		
	
	}
	
}
class Characteristic<T> : BaseCharacteristic {
	public T startValue;
	private T tempValue;
	public Characteristic<T init){
	
	}
	public AddEffect(Effect<T> newEffect){
		base.Add(newEffect);
	}
	public T GetValue(){
		if(timeUpdate>timeLastUpdate&&timeUpdate<Time.time){
			return tempValue;
		}else{
			ReCount();
		}
	}
	virtual void ReCount(){
			
	}
}
[Serializable]
class IntCharacteristic{
	public int startValue;
	public CharacteristicList characteristic;
}
[Serializable]
class BoolCharacteristic{
	public bool startValue;
	public CharacteristicList characteristic;
}
[Serializable]
class FloatCharacteristic{
	public float startValue;
	public CharacteristicList characteristic;
}
public class CharacteristicManager : MonoBehaviour {
	
	public IntCharacteristic[] startIntSharacteristic;
	public BoolCharacteristic[] startBoolSharacteristic;
	public FloatCharacteristic[] startFloatSharacteristic;
	
	
	private BaseCharacteristic[] allCharacteristic;
	
	void Init(){
		allCharacteristic = new Characteristic(Enum.GetValues(typeof(CharacteristicList)).Length);
		for(int  i=0; i<startIntSharacteristic.Lenght;i++){
			allCharacteristic[(int)startIntSharacteristic[i].characteristic] = new Characteristic<int>(startIntSharacteristic[i].startValue);
		}
		for(int  i=0; i<startBoolSharacteristic.Lenght;i++){
			allCharacteristic[(int)startBoolSharacteristic[i].characteristic] = new Characteristic<bool>(startBoolSharacteristic[i].startValue);
		}
		for(int  i=0; i<startFloatSharacteristic.Lenght;i++){
			allCharacteristic[(int)startFloatSharacteristic[i].characteristic] = new Characteristic<float>(startFloatSharacteristic[i].startValue);
		}
	
	}
	void Update(){
		for(int  i=0; i<allCharacteristic.Lenght;i++){
			allCharacteristic[i].UpdateList();
		}
	}
	public int GetIntChar(CharacteristicList characteristic){
			return((Characteristic<int>)allCharacteristic[(int)characteristic]).GetValue();

	
	}
	public float GetIntChar(CharacteristicList characteristic){
			return((Characteristic<float>)allCharacteristic[(int)characteristic]).GetValue();
	
	}
	public bool GetIntChar(CharacteristicList characteristic){
			return((Characteristic<bool>)allCharacteristic[(int)characteristic]).GetValue();
	
	}
	
}
