using UnityEngine;
using System.Collections;

public enum SocialBtn
{
    WALL_POST,
    ALBOM_SAVE

}
public class SocialButton : MonoBehaviour {

    public SocialBtn type;

	// Use this for initialization
	void Awake () {
        switch (type)
        {
            case SocialBtn.ALBOM_SAVE:
                GetComponent<UIButton>().onClick.Add(new EventDelegate( FindObjectOfType<MainMenuGUI>().TakeScreenShoot));
                break;

            case SocialBtn.WALL_POST:
                 GetComponent<UIButton>().onClick.Add(new EventDelegate( FindObjectOfType<MainMenuGUI>().TakeScreenShootWall));
                break;
        }
	}
	

}
