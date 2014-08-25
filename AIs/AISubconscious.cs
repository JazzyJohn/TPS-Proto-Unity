using UnityEngine;
using System.Collections.Generic;

public enum eSensorParams { Cheerfulness, Courage, Attentiveness, CommandActions, SeeTargetsPrioritet, SeeTargets, WorldTime }

public class AISubconscious : MonoBehaviour {
	public AISensors sensor;
	public AIAction[] actions;
	AIConscious consious;

	void Start()
	{
		sensor.owner = GetComponent<AIBase> ();
		sensor._Pawn = GetComponent<Pawn> ();
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
		if (consious.aiAction.GetPrioritet() < PrioritetMax) consious.aiAction = FullActions[NextActionID];
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

	public Dictionary<int, object> SensorParametrs = new Dictionary<int, object>();
	
	public AIBase owner;

	public Pawn _Pawn;

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
		Pawn[] Targets = _Pawn.getAllSeenPawn ().ToArray ();
		UpdateDictionaryParametrs ((int)eSensorParams.SeeTargets, Targets);
	}

	[System.Serializable]
	public class TimeSensor {
		public TimeModule _Time;
		public WorldTime GetTime ()
		{
			return _Time.GetTime ();
		}
	}

	void TimeSensorHandler()
	{
		UpdateDictionaryParametrs ((int)eSensorParams.WorldTime, sTime.GetTime());
	}

	[System.Serializable]
	public class CommandSensor {
		bool Command;
		List<AIAction> NewAIAction = new List<AIAction>();

		public void NewCommand (AIAction NewAIAction)
		{
			if (!Command) this.NewAIAction.Clear ();
			if (!this.NewAIAction.Contains(NewAIAction)) this.NewAIAction.Add (NewAIAction);
			Command = true;
		}

		public List<AIAction> GetCommand ()
		{
			return NewAIAction;
		}

		public bool IsReady 
		{
			set {
				Command = value;
			}
			get {
				return Command;
			}
		}
	}

	void CommandSensorHandler()
	{
		if (sCommand.IsReady) {
			UpdateDictionaryParametrs ((int)eSensorParams.CommandActions, sCommand.GetCommand);
			sCommand = false;
		}
	}

	[System.Serializable]
	public class CharacteristicSensor {
		public float Cheerfulness = 100f; //Бодрость
		public float Courage = 100f; //Смелость  
		public float Attentiveness = 100; //Внимательность

		public void UpdateCharacteristic(float time)
		{
			if (Cheerfulness > 0) Cheerfulness -= time * 0.5f;
		}
	}
	
	void CharacteristicSensorHandler()
	{
		sCharacteristic.UpdateCharacteristic (Time.deltaTime);
		UpdateDictionaryParametrs ((int)eSensorParams.Cheerfulness, sCharacteristic.Cheerfulness);
		UpdateDictionaryParametrs ((int)eSensorParams.Courage, sCharacteristic.Courage);
		UpdateDictionaryParametrs ((int)eSensorParams.Attentiveness, sCharacteristic.Attentiveness);
	}

	void UpdateDictionaryParametrs(string Key, object newObject)
	{
		if (!SensorParametrs.ContainsKey (Key)) SensorParametrs.Add (Key, newObject);
		else SensorParametrs [Key] = newObject;
	}

}
