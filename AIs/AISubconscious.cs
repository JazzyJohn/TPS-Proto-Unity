using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public enum Action { Atack = 0, Patrolling = 1, Sleep = 2, Wait = 3 }

public struct AIActionStruct {
	public int id;
	public Action eAction;
	public AIActionStruct (int id, Action action) {
		this.id = id;
		this.eAction = action;
	}
}

public class AISubconscious : MonoBehaviour {
	public AISensors sensor;
	public AIActions action;
	public AICharacteristics characteristics;
	AIConscious consious;
	Transform CurrentTarget;
	float CurrentTargetPrioritet;
	AIActionStruct CurrentAction;
	List<Transform> Targets = new List<Transform>();
	List<float> TargetPrioritet = new List<float>();
	float[] ActionPrioritet;
	int ActionCount = 4;

	void Start()
	{
		consious.aiAction = action;
	}

	void Update()
	{
		Targets.Clear ();
		TargetPrioritet.Clear ();
		ActionPrioritet = new float[ActionCount];
		if (sensor.sCommand.Enable) CommandSensorHandler ();
		if (sensor.sCharacteristic.Enable) CharacteristicSensorHandler ();
		if (sensor.sSkin.Enable) SkinSensorHandler ();
		if (sensor.sEye.Enable) EyeSensorHandler ();
		if (sensor.sTime.Enable) TimeSensorHandler ();
	}

	void NextAction()
	{
		float PrioritetMax = 0;
		int NextActionID = 0;
		for (int i = 0; i < ActionCount; i++) {
			float Prioritet = ActionPrioritet[i];
			if (Prioritet >= PrioritetMax) {
				PrioritetMax = Prioritet;
				NextActionID = i;
			}
		}
		if (consious.Waiting > 0) {
			int ID = 0;
			AIActionStruct NewAction = new AIActionStruct(ID, (Action)NextActionID);
//			if (CurrentAction != NewAction)
			if ((Action)NextActionID == Action.Atack) {
				UpdateTargetMaxPrioritet ();
			}
		}
	}

	void SkinSensorHandler()
	{

	}
	
	void EyeSensorHandler()
	{
		Vector3 Pos = transform.position;
		Collider[] targets = Physics.OverlapSphere (Pos, sensor.sEye.SeeRange - sensor.sEye.SeeRange * characteristics.Attentiveness * 0.01f);
		if (targets.Length != 0) {
			RaycastHit hit;
			for (int i = 0; i < targets.Length; i++) {
				Transform Target = targets [i].transform;
				float Distance = Vector3.Distance (Pos, Target.position);
				if (!Physics.Raycast (Pos, Target.position - Pos, out hit, Distance) && hit.collider.GetComponent<Pawn> () == null) {
					float SeePrioritet = sensor.sEye.SeeConstant * (Distance / sensor.sEye.SeeRange);
					if (!Targets.Contains (Target)) {
						TargetPrioritet.Add(SeePrioritet);
						Targets.Add(Target);
					}
					else TargetPrioritet[Targets.IndexOf(Target)] += SeePrioritet;
				}
			}
		}
	}

	void Processing ()
	{
		if (action.attack.Length != 0) {
			bool CanAttack = false;
			foreach (AIActions.Attack Attack in action.attack)
				if (Attack.calldown <= 0) CanAttack = true;
			if (CanAttack){
				ActionPrioritet[(int)Action.Atack] = 90;
				for (int i = 0; i < Targets.Count; i++) {
					int PrioritetAttack = 0;
					float Distance = Vector3.Distance(transform.position, Targets[i].position);
					foreach (AIActions.Attack Attack in action.attack)
						if (Attack.calldown <= 0 && Distance <= Attack.Distance && Attack.Prioritet >= PrioritetAttack) PrioritetAttack = Attack.Prioritet;
					TargetPrioritet[i] += PrioritetAttack;
				}
			}
			else ActionPrioritet[(int)Action.Atack] = 0;
		}
		if (Targets.Count == 0) ActionPrioritet [(int)Action.Patrolling] = 90f;

	}

	void UpdateTargetMaxPrioritet()
	{
		float PrioritetMax = 0;
		int TargetIndex = 0;
		for (int i = 0; i < TargetPrioritet.Count; i++) {
			if (TargetPrioritet[i] >= PrioritetMax){
				PrioritetMax = TargetPrioritet[i];
				TargetIndex = i;
			}
		}
		if (PrioritetMax > CurrentTargetPrioritet) {
			CurrentTarget = Targets [TargetIndex];
			CurrentTargetPrioritet = TargetPrioritet [TargetIndex];
		}
	}

	void CurrentTargetClear()
	{
		CurrentTarget = null;
		CurrentTargetPrioritet = 0;
	}

	void TimeSensorHandler()
	{
		
	}

	void CommandSensorHandler()
	{
		
	}
	
	void CharacteristicSensorHandler()
	{
		characteristics.Cheerfulness -= Time.deltaTime * 0.5f;
		ActionPrioritet [(int)Action.Sleep] = 100f - characteristics.Cheerfulness;
	}
}

[System.Serializable]
public class AISensors {
	public SkinSensor sSkin;
	public EyeSensor sEye;
	public TimeSensor sTime;
	public CommandSensor sCommand;
	public CharacteristicSensor sCharacteristic;
	
	[System.Serializable]
	public class SkinSensor {
		public bool Enable = true;
	}
	
	[System.Serializable]
	public class EyeSensor {
		public bool Enable = true;
		public float SeeRange = 10f;
		public float SeeConstant = 50f;
	}
	
	[System.Serializable]
	public class TimeSensor {
		public bool Enable = true;
	}
	
	[System.Serializable]
	public class CommandSensor {
		public bool Enable = true;
	}
	
	[System.Serializable]
	public class CharacteristicSensor {
		public bool Enable = true;
	}
}

public class AICharacteristics {
	public float Cheerfulness = 100f; //Бодрость
//	public float Courage = 100f; //Смелость  
	public float Attentiveness = 100; //Внимательность
}

public class AIConscious : MonoBehaviour {

	AIActionStruct aiActionStruct;
	public AIActions aiAction;

	public float Waiting = 0;

	void Update()
	{
		if (Waiting > 0) Waiting -= Time.deltaTime; 
		foreach (AIActions.Attack Attack in aiAction.attack)
			Attack.CalldownDown ();
		if (aiActionStruct.eAction == Action.Wait) {
			
		} 
		else if (aiActionStruct.eAction == Action.Patrolling) {
			
		}
		else if (aiActionStruct.eAction == Action.Sleep) {
			
		}
	}
	
	public void SwichAction (AIActions NewAction)
	{
		aiAction = NewAction;
	}
	
}

[System.Serializable]
public class AIActions {
	
	public Patrolling[] patrolling;
	public Attack[] attack;
	
	[System.Serializable]
	public class Patrolling {
		
	}
	
	[System.Serializable]
	public class Attack {
		public int Prioritet;
		public float Distance;
		[HideInInspector]
		public float calldown;
		public void CalldownDown()
		{
			if (calldown > 0) calldown -= Time.deltaTime;
		}
	}
	
	public class Sleep {
		
	}

	public class Wait {
		
	}
}
