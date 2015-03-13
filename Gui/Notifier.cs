using UnityEngine;
using System.Collections;

public class Notifier : MonoBehaviour {

    public UIRect panel;
	
    public UITweener tweener;

    public UILabel label;

    public UILabel titleLabel;

    public UIWidget textureWid;

    public UITexture iconTexture;


    public UIWidget spriteWid;

    public UISprite iconSprite;




    public void ShowIconMessage(string title, string text, string icon)
    {
        label.text = text;
        titleLabel.text = title;
        spriteWid.alpha = 1.0f;
        textureWid.alpha = 0.0f;
        iconSprite.spriteName = icon;
        tweener.tweenFactor = 0;
        tweener.PlayForward();
    }
    public void ShowIconMessage(string title,string text, Texture2D icon)
    {
        label.text = text;
        titleLabel.text = title;
        spriteWid.alpha = 0.0f;
        textureWid.alpha = 1.0f;
        iconTexture.mainTexture = icon;
        tweener.tweenFactor = 0;
        tweener.PlayForward();
    }

    public void FastClose()
    {
        tweener.tweenFactor = 1;
        
    }
}
