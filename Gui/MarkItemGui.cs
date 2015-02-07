using UnityEngine;
using System.Collections;

public class MarkItemGui : MonoBehaviour {

	public UIPanel Lot;
	
	public MainMenuGUI MainMenu;
	
	public UILabel PriceKP;
	public UILabel PriceGITP;
	public UILabel AmountLabel;
	
	public void Init(int count){
		int cash = 0;
		int gold = 0;
		AmountLabel.text = count.ToString();
		ItemManager.instance.MarkedCost(out cash,out gold);
		PriceKP.text = cash+ " KP";
        PriceGITP.text =gold + " GITP";
	}
	
	public void BuyForGold(){
	
	
//		StartCoroutine(ItemManager.instance.BuyMarkeditems(this,true));
	}
	
	public void BuyForCash(){
	
		//StartCoroutine(ItemManager.instance.BuyMarkeditems(this,false));
	}
	public void MoneyError(){
	
        MainMenu.SetMessage("Недостаточно Денег");
   
		MainMenu.MoneyError();
	}
	public void CloseWindow(){
		Lot.alpha = 0.0f;
	}

}