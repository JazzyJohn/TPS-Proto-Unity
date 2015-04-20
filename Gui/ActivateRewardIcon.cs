using UnityEngine;
using System.Collections;

public class ActivateRewardIcon : MonoBehaviour {

    public UIWidget activeWidget;

    public UIWidget selectedWidget;




    public void Show(RewardState state)
    {
        switch (state)
        {
            case RewardState.ACTIVE:
                activeWidget.alpha = 1.0f;
                selectedWidget.alpha = 0.0f;
                break;
            case RewardState.NO_ACTIVE:
                activeWidget.alpha = 0.0f;
                selectedWidget.alpha = 0.0f;
                break;
            case RewardState.SELECTED:
                activeWidget.alpha = 0.0f;
                selectedWidget.alpha = 1.0f;
                break;
        }
    }
}
