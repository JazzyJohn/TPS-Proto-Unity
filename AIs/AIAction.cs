using UnityEngine;
using System.Collections.Generic;

public class AIAction : MonoBehaviour {

	public virtual int GetPrioritet(Dictionary<string, object> SensorParametrs)
	{
		return 0;
	}

}