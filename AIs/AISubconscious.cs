using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class AISubconscious : MonoBehaviour {
	public AISensors sensor;
	public AIAction[] actions;
	AIConscious consious;

	void Start()
	{
		sensor.owner = GetComponent<AIBase> ();
		sensor.Scan ();
		NextAction ();
	}

	void Update()
	{
		if (!consious.aiAction.Interfere()) return;
		if (consious.aiAction.DeactivateSensor().Length != 0)
		{
			sensor.ScanLimited (consious.aiAction.DeactivateSensor());
		}
		else sensor.Scan ();
		NextAction ();
	}

	void NextAction()
	{
		float PrioritetMax = 0;
		int NextActionID = 0;
		AIAction[] FullActions = GetFullActions ();
		for (int i = 0; i < FullActions.Length; i++) {
			float Prioritet = FullActions[i].GetPrioritet(sensor.SensorParametrs);
			if (Prioritet >= PrioritetMax) {
				PrioritetMax = Prioritet;
				NextActionID = i;
			}
		}
		if (consious.aiAction.GetPrioritet() < PrioritetMax) consious.aiAction = FullActions[NextActionID];k
	}

	AIAction[] GetFullActions()
	{
		if (!sensor.SensorParametrs.ContainsKey ("CommandActions")) return actions;
		else {
			AIAction[] CommandActions = (AIAction[])sensor.SensorParametrs["CommandActions"];
			CommandActions.AddRange(actions);
			return CommandActions;
		}
	}

	void Processing ()
	{

	}

}

public enum Sensor { skin, eye, time, command, characteristic }

[System.Serializable]
public class AISensors {
	public bool SkinSensorEnable = true;
	public SkinSensor sSkin;

	public bool EyeSensorEnable = true;
	public EyeSensor sEye;

	public bool TimeSensorEnable = true;
	public TimeSensor sTime;

	public bool CommandEnable = true;
	public CommandSensor sCommand;

	public bool CharacteristicSensorEnable = true;
	public CharacteristicSensor sCharacteristic;

	public Dictionary<string, object> SensorParametrs = new Dictionary<string, object>();
	
	public AIBase owner;

	public void Scan ()
	{
		SensorParametrs.Clear ();
		if (CommandEnable) CommandSensorHandler ();
		if (CharacteristicSensorEnable) CharacteristicSensorHandler ();
		if (SkinSensorEnable) SkinSensorHandler ();
		if (EyeSensorEnable) EyeSensorHandler ();
		if (TimeSensorEnable) TimeSensorHandler ();
	}

	public void ScanLimited (Sensor[] Deactivated)
	{
		SensorParametrs.Clear ();
		if (!Deactivated.Contains(Sensor.command) && CommandEnable) CommandSensorHandler ();
		if (!Deactivated.Contains(Sensor.characteristic) && CharacteristicSensorEnable) CharacteristicSensorHandler ();
		if (!Deactivated.Contains(Sensor.skin) && SkinSensorEnable) SkinSensorHandler ();
		if (!Deactivated.Contains(Sensor.eye) && EyeSensorEnable) EyeSensorHandler ();
		if (!Deactivated.Contains(Sensor.time) && TimeSensorEnable) TimeSensorHandler ();
	}

	[System.Serializable]
	public class SkinSensor {

	}

	void SkinSensorHandler()
	{

	}

	[System.Serializable]
	public class EyeSensor {

	}

	void EyeSensorHandler()
	{
//		GameObject[] PlayerInRadius = owner.GetPlayerInRadius ();
//		float[] TargetsPrioritet = new float[PlayerInRadius.Length];
//		Vector3 Pos = owner.transform.position;
//		for (int i = 0; i < TargetsPrioritet.Length; i++) {
//			float Distance = Vector3.Distance(Targets[i].position, Pos);
//			TargetsPrioritet[i] += sEye.SeeConstantPrioritet * Distance / sEye.SeeRange;
//		}
//		UpdateDictionaryParametrs ("SeeTargetsPrioritet", TargetsPrioritet);
//		UpdateDictionaryParametrs ("SeeTargets", Targets);
	}

	[System.Serializable]
	public class TimeSensor {
		public void GetTime ()
		{
			
		}
	}

	void TimeSensorHandler()
	{
		
	}

	[System.Serializable]
	public class CommandSensor {
		public bool Command;
		public AIAction NewAIAction;
		public void GetCommand (AIAction NewAIAction)
		{
			this.NewAIAction = NewAIAction;
			Command = true;
		}
	}

	void CommandSensorHandler()
	{
		if (sCommand.Command) {
			if (SensorParametrs.ContainsKey ("CommandActions")) {
				List<AIAction> CommandActions = (List<AIAction>) SensorParametrs ["CommandActions"];
				CommandActions.Add (sCommand.NewAIAction);
				SensorParametrs ["CommandActions"] = CommandActions;
			} else {
				UpdateDictionaryParametrs ("CommandActions", new List<AIAction> { sCommand.NewAIAction });
			}
		}
	}

	[System.Serializable]
	public class CharacteristicSensor {
		public float Cheerfulness = 100f; //Бодрость
		public float Courage = 100f; //Смелость  
		public float Attentiveness = 100; //Внимательность

		public void UpdateCharacteristic(float time)
		{
			Cheerfulness -= time * 0.5f;
		}
	}
	
	void CharacteristicSensorHandler()
	{
		sCharacteristic.UpdateCharacteristic (Time.deltaTime);
		UpdateDictionaryParametrs ("Cheerfulness", sCharacteristic.Cheerfulness);
		UpdateDictionaryParametrs ("Courage", sCharacteristic.Courage);
		UpdateDictionaryParametrs ("Attentiveness", sCharacteristic.Attentiveness);
	}

	void UpdateDictionaryParametrs(string Key, object newObject)
	{
		if (!SensorParametrs.ContainsKey (Key)) SensorParametrs.Add (Key, newObject);
		else SensorParametrs [Key] = newObject;
	}

}
