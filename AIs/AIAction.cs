using UnityEngine;
using System.Collections.Generic;

public class AIAction : MonoBehaviour {
	Sensor[] _DeactivateSensor = new Sensor[0]; 
	bool _Interfere = true;

	public virtual int GetPrioritet(Dictionary<string, object> SensorParametrs)
	{
		return 0;
	}

	public virtual bool Interfere()
	{
		return _Interfere;
	}

	public virtual Sensor[] DeactivateSensor ()
	{
		return _DeactivateSensor;
	}

	public void AIUpdate()
	{

	}
}