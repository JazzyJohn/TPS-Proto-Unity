using UnityEngine;
using System.Collections;

public class AddShops : MonoBehaviour
{

   

    public SmallAsk ask;

    public UIPanel money;

    public UILabel annonce;

    public UITweener annonceTweener;

    bool shoodHitMoney;



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
    }


    public void BuyDouble()
    {
        if (AfterGameBonuses.done)
        {
            return;

        }
        if (GlobalPlayer.instance.gold >= PremiumManager.DOUBLECOST)
        {
            StartCoroutine(PremiumManager.instance.PayForDouble(this));
        }
        else
        {
            AskMoneyShow(BuyDouble);

        }

    }

    public void AskMoneyShow(ConfirmAction action)
    {
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

    public void SetMessage(string text)
    {
        annonce.text = text;
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
}
