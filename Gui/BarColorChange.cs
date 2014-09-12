using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarColorChange : MonoBehaviour {

    [System.Serializable]
    public class ValueColorPair
    {
        public float value;
        public Color color;
    }

    public List<ValueColorPair> allColor= new List<ValueColorPair>();

    public UIProgressBar bar;

    public UISprite foreground;
    public void OnChange()
    {
        foreach(ValueColorPair pair in allColor){
            if (pair.value < bar.value)
            {
                foreground.color = pair.color;
                return;
            }
        }

    }
}
