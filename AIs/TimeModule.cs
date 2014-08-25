using UnityEngine;

public class TimeModule : MonoBehaviour
{
	public float Ratio = 1f;

	WorldTime _time;

	void Update ()
	{
		_time.AddTime ((Time.deltaTime * Ratio) / 60f);
	}

	public WorldTime GetTime ()
	{
		return _time;
	}
}

public class WorldTime 
{
	int Hour;
	float Minute;

	public WorldTime (int Hour, float Minute)
	{
		this.Hour = Hour;
		this.Minute = Minute;
		RecalculateTime ();
	}

	public void AddTime (float _Add)
	{
		Minute += _Add;
		RecalculateTime ()
	}

	void RecalculateTime ()
	{
		if (Minute > 60f) {
			int AddHour = 0;
			AddHour = System.Math.Truncate (Minute / 60f);
			Minute -= 60f * AddHour;
			Hour += AddHour;
		}
		if (Hour > 23) Hour = (int)(Hour - 24 * System.Math.Truncate (Hour / 24f));
	}
}