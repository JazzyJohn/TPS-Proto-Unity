using UnityEngine;
using System.Collections;
using System;

public class StatisticGUIEntry : MonoBehaviour {

    public string key;

    public UILabel label;

    void Awake()
    {
        label = GetComponent<UILabel>();
    }

    public void Init(int value)
    {
        if (key == "time")
        {
            TimeSpan duration = TimeSpan.FromSeconds(value);
            string formated = string.Format("{0:D2} : {1:D2} : {2:D2} ", duration.Hours, duration.Minutes, duration.Seconds);

            label.text = formated;
        }else if(key=="accuracy"){
			label.text = value.ToString()+"%";
		}
        else
        {
            label.text = value.ToString();
        }
    }
}
