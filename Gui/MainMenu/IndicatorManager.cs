using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IndicatorManager: MonoBehaviour  {

    
 private static IndicatorManager s_Instance = null;
 public static string KITS = "kits";
 public static string TASK = "task";
  public static IndicatorManager instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (IndicatorManager)) as IndicatorManager;
			}
			
			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("IndicatorManager");
				s_Instance = obj.AddComponent(typeof (IndicatorManager)) as IndicatorManager;
				
			}
			
			return s_Instance;
		}
	}
    class Counter
    {
        public DateTime end;

        public bool infinite;
    }

    class Indicator
    {
        List<Counter> list = new List<Counter>();

        public int GetCount()
        {
            return list.Count;
        }
        public void CheckBad()
        {
            list.RemoveAll(delegate(Counter v)
            {
                return !v.infinite && v.end < DateTime.Now;
            });
        }

        public void Add(bool infinite, DateTime time)
        {
            Counter counter = new Counter();
            counter.infinite = infinite;
            counter.end = time;
            list.Add(counter);
        }
        public void Remove()
        {
            if(list.Count>0)
                list.RemoveAt(0);
        }
        public void Remove (DateTime time)
        {
          list.RemoveAll(delegate(Counter v)
            {
                return !v.infinite && v.end == time;
            });
        }
    }
    Dictionary<string, Indicator> dictionary = new Dictionary<string, Indicator>();
    public int GetCount(string name)
    {
        if (dictionary.ContainsKey(name))
        {
            return dictionary[name].GetCount() ;
        }
        return 0;
    }

    float time;
    public void Update()
    {
        time += Time.deltaTime;
        if (time > 1.0f)
        {
            foreach (Indicator indicator in dictionary.Values)
            {
                indicator.CheckBad();
            }
        }
    }
    public void Add(string name, bool infinite)
    {
        Add(name, infinite, DateTime.Now);
    }
    public void Add(string name, bool infinite, DateTime time)
    {
        Indicator indicator;
        if (dictionary.ContainsKey(name))
        {
            indicator = dictionary[name];
        }
        else
        {
            indicator = new Indicator();
            dictionary[name] = indicator;
        }
        indicator.Add( infinite, time);
    }
    public void Remove(string name)
    {
      
        if (!dictionary.ContainsKey(name))
        {
            return;
        }
        Indicator indicator = dictionary[name];
        indicator.Remove();
        
    }
    public void Remove(string name, DateTime time)
    {

        if (!dictionary.ContainsKey(name))
        {
            return;
        }
        Indicator indicator = dictionary[name];
        indicator.Remove(time);

    }

    public static string GetLeftTime(DateTime end)
    {
        TimeSpan span = new TimeSpan(end.Ticks - DateTime.Now.Ticks);
       return string.Format("{0:D3} : {1:D2} : {2:D2}", span.Hours + span.Days * 24, span.Minutes, span.Seconds);
    }
}
