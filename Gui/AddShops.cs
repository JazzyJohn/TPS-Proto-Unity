using UnityEngine;
using System.Collections;

public delegate void ShowAndTellAction(string text, Texture2D texture);



public class AddShops : MonoBehaviour
{

   

    public SmallAsk ask;

    public UIPanel money;

    public UILabel annonce;

    public UITweener annonceTweener;

    bool shoodHitMoney;

    public UITexture textureAnon;
	
	public UILabel textureAnonLabel;

    public UITweener textureAnonTweener;

    public UIWidget premium;

    public UIPanel serverResponse;

    ConfirmAction noMoneyAction;

    public void AskExternalBuy(string item)
    {
        shoodHitMoney = true;
        GlobalPlayer.instance.AskJsForMagazine(item);
    }


    public void Double()
    {
        ask.panel.alpha = 1.0f;
        ask.text.text = TextGenerator.instance.GetSimpleText("DoubleExp");
        ask.action = BuyDouble;
        GA.API.Design.NewEvent("GUI:SmallShop:Double", 1);
    }


    public void BuyDouble()
    {
        if (AfterGameBonuses.done)
        {
            return;

        }
        if (GlobalPlayer.instance.gold >= PremiumManager.DOUBLECOST)
        {
            GA.API.Business.NewEvent("SmallShop:DoubleReward", "GOLD", PremiumManager.DOUBLECOST);
            StartCoroutine(PremiumManager.instance.PayForDouble(this));
        }
        else
        {

            AskMoneyShow(BuyDouble);

        }

    }

    public void AskMoneyShow(ConfirmAction action)
    {
        GA.API.Design.NewEvent("GUI:SmallShop:AskMoneyShow", 1);
        noMoneyAction = action;
        ask.panel.alpha = 1.0f;
        ask.text.text = TextGenerator.instance.GetSimpleText("NoMoney");
        ask.action = ShowMoney;
    }
    public void ShowMoney()
    {
        money.alpha = 1.0f;
    }

    public void CloseMoney()
    {
        money.alpha = 0.0f;

    }

    public void ReloadFinished()
    {
        if (shoodHitMoney)
        {
            shoodHitMoney = false;
            noMoneyAction();
        }


    }
	
	public void NewItem(string id){
        ItemManager.instance.GetItem(id,ShowItem);
	}

	public void ShowItem(string text, Texture2D texture){

        ShowIconMessage(TextGenerator.instance.GetSimpleText("OpenItem") + text, texture);
	}
    public void ShowIconMessage(string text, Texture2D texture)
    {
        textureAnonLabel.text = text;
        textureAnon.mainTexture = texture;
        textureAnonTweener.enabled = true;
        textureAnonTweener.tweenFactor = 0.0f;
        textureAnonTweener.PlayForward();
    }
    public void SetMessage(string text)
    {
        annonce.text = text;
		 annonceTweener.enabled = true;
        annonceTweener.tweenFactor = 0.0f;
        annonceTweener.PlayForward();
    }


    public void ShowDoubleReward()
    {
        annonce.text = TextGenerator.instance.GetMoneyText("Boost",AfterGameBonuses.cashBoost,AfterGameBonuses.expBoost);
        annonceTweener.enabled = true;
        annonceTweener.tweenFactor = 0.0f;
        annonceTweener.PlayForward();
    }

    public void ShowPremium()
    {
        premium.alpha = 1.0f;
    }
    public void HidePremium()
    {
        premium.alpha = 0.0f;
    }
	
	public void ShowServerWait(){
		if(serverResponse!=null){
			serverResponse.alpha = 1.0f;
		}
	
	}
	public void HideServerWait(){
		if(serverResponse!=null){
			serverResponse.alpha = 0.0f;
		}
	}
}
