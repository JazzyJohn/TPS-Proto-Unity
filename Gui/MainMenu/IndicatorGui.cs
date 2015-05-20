using UnityEngine;
using System.Collections;

public class IndicatorGui : MonoBehaviour {
    public UIWidget mainWidget;

    public UILabel textLabel;

    public string nameOfIndicator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        int count = IndicatorManager.instance.GetCount(nameOfIndicator);
        if (count > 0)
        {
            if (mainWidget.alpha <1.0f)
            {
                mainWidget.alpha = 1.0f;
            }
            textLabel.text = count.ToString();
        }
        else
        {
            if (mainWidget.alpha > 0.0f)
            {
                mainWidget.alpha = 0.0f;
            }
           
        }
	}
}
