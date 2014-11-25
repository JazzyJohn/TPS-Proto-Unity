using UnityEngine;
using System.Collections;

public class SmallShop : MonoBehaviour {

    public SelectPlayerGUI selectorPanel;

    public UIPanel panel;
    public UITexture Icon;

    public UILabel Offer;

    public UILabel GoldBuy;

    public UILabel CreditBuy;

    public SmallShopData data;

    public UILabel MyGold;

    public UILabel MyCash;

    public UILabel descr;

    public AddShops addshops;

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
            addshops.AskMoneyShow(BuyForGold);
            return;
        }
        StartCoroutine(BuyItem(data.goldSlot));
    }
    public void BuyForCash(){
    
        StartCoroutine(BuyItem(data.cashSlot));
    }
    public void Show() {
        panel.alpha = 1.0f;
        MyGold.text = GlobalPlayer.instance.gold.ToString();
        MyCash.text = GlobalPlayer.instance.cash.ToString();
        GoldBuy.text = data.goldCost.ToString();
        CreditBuy.text = data.cashCost.ToString();
        descr.text = data.descr;
    }
    public IEnumerator BuyItem(string id) {

        IEnumerator numenator = ItemManager.instance.BuyItem(id,null);

		while(numenator.MoveNext()){
			yield return numenator.Current;
		}
        numenator = ItemManager.instance.ReLoadItemsSync();
        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }
        ItemManager.instance.ConnectToPrefab();
        numenator = GlobalPlayer.instance.ReloadStats();
        while (numenator.MoveNext())
        {
            yield return numenator.Current;
        }
       
        CloseShop();
        
        selectorPanel.ReDrawAll();
    }
}
