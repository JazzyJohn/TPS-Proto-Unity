using UnityEngine;
using System.Collections;

public class AchievHUD : MonoBehaviour {
    Achievement achiev;
    public UIWidget Widget;
    
    public UILabel progress;
    public UITexture picture;
	// Use this for initialization
    public void Init(Achievement achiev)
    {
        this.achiev = achiev;
        if (this.achiev == null)
        {
            Widget.alpha = 0.0f;
        }
        else
        {
            Widget.alpha = 1.0f;
            picture.mainTexture = achiev.textureIcon;
          
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (achiev != null)
        {
            progress.text = achiev.GetProgress();
        }
	}
}
