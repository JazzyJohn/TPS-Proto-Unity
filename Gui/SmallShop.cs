using UnityEngine;
using System.Collections;

public class SmallShop : MonoBehaviour {

    public SelectPlayerGUI selectorPanel;

    public UIPanel panel;
    public UITexture Icon;

    public UILabel Offer;

    public UILabel GoldBuy;

    public UILabel CreditBuy;

    public string mysqlId;

    public UILabel MyGold;

    public UILabel MyCash;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void CloseShop()
    {
        panel.alpha = 0.0f;
    }
    public void BuyForGold()
    {
        if (GlobalPlayer.instance.gold <= 0)
        {
            GlobalPlayer.instance.AskJsForMagazine("gitp_5");
            return;
        }
        StartCoroutine(BuyItem(true));
    }
    public void BuyForCash()
    {
        StartCoroutine(BuyItem(false));
    }
    public void Show() {
        panel.alpha = 1.0f;
        MyGold.text = GlobalPlayer.instance.gold + " GITP";
        MyCash.text = GlobalPlayer.instance.cash + " KP";
        
    }
    public IEnumerator BuyItem(bool forGold) {

        IEnumerator numenator = ItemManager.instance.BuyItem(mysqlId, forGold);

		while(numenator.MoveNext()){
			yield return numenator.Current;
		}
        numenator = ItemManager.instance.ReoadItemsSync();
        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }
        numenator = GlobalPlayer.instance.ReloadStats();
        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }
       
        CloseShop();
        
        selectorPanel.ReDrawAll();
    }
}
