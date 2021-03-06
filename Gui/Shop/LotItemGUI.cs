﻿using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class PriceForLot
{
    public UILabel actualPrice;

    public UILabel oldPrice;

    public UIWidget oldPriceWid;

    public int text
    {
        set{
            actualPrice.text = value.ToString();
        }
    }

    public int discount
    {
        set
        {
            if (value ==0)
            {
                oldPriceWid.alpha = 0;
            }
            else
            {
                oldPriceWid.alpha = 1;
                oldPrice.text = "[s]"+value.ToString()+"[s]";
            }
        }
    }
}

public class LotItemGUI : MonoBehaviour
{


    public CharSection weaponSection;
    public ArmorSection armSection;
    public HTHSection hthSection;
    public InventoryGUI Shop;
    public InventorySlot item;
    public UILabel Name;
    public UILabel Description;
    public Transform gun;
    public GameObject gunModel;
    public UIWidget Box;
    public UILabel loading;
    public UILabel buyLabel;

    


	public UIWidget forGold;

    public PriceForLot[] goldPrices;
	
	public UIWidget forKP;

    public UIWidget forSpecial;

    public UILabel forSpecialLabel;

    public UIWidget buyKP;

    public UIWidget buyUnbreake;

    public UIWidget repairKP;

    public UIWidget useItem;

    public PriceForLot[] kpPrices;

    public UIWidget timer;

    public UILabel timerLabel;

    [HideInInspector]
    public int numToItem;

	private int priceKey;
	
  
    // Use this for initialization
    void Start()
    {

    }

    public void LoadInfo()
    {
        //Код загрузки инфы из xml
    }

    // Update is called once per frame
    void Update()
    {
        if (item != null && item.loadModel != null && gunModel == null)
        {
            gunModel = Instantiate(item.loadModel, gun.position, gun.rotation) as GameObject;
            gunModel.transform.parent = gun;
            gunModel.transform.rotation = Quaternion.identity;
            gunModel.transform.localScale = Vector3.one;
            gunModel.layer = gun.gameObject.layer;
            loading.alpha = 0.0f;
           
        }
        if (item != null && item.prices[0].discount)
        {
            if (item.prices[0].discoutnEnd < DateTime.Now)
            {
                for (int i = 0; i < item.prices.Length; i++)
                {
                    item.prices[i].CloseDiscount();
                }
                SetItem(item);
             
            }
            timerLabel.text = IndicatorManager.GetLeftTime(item.prices[0].discoutnEnd);
        }
    }

    public void SetItem(InventorySlot _item, bool justInfo= false)
    {
        item = _item;
        
//        Debug.Log(item.buyMode);

      

        if (item.buyMode == BuyMode.FOR_KP)
        {
            buyKP.alpha = 0.0f;
            forSpecial.alpha = 0.0f;
            buyUnbreake.alpha = 1.0f;
            repairKP.alpha = 1.0f;

        }
        else if(item.buyMode ==BuyMode.FOR_KP_UNBREAK)
        {
            buyKP.alpha = 0.0f;
            forSpecial.alpha = 0.0f;
            buyUnbreake.alpha = 0.0f;
            repairKP.alpha = 0.0f;
        }             
        else
        {
            buyUnbreake.alpha = 0.0f;
          
            if (item.prices[0].parts[0].amount < 0)
            {

                forSpecial.alpha = 1.0f;
                buyKP.alpha =0.0f;
                if (item.prices[0].parts[0].amount < 0)
                {

                    forSpecialLabel.text = TextGenerator.instance.GetSimpleText("itemForRewardID" + item.prices[0].parts[0].amount);
                }
            }
            else
            {
                buyKP.alpha = 1.0f;
                forSpecial.alpha = 0.0f;
            }
            repairKP.alpha = 0.0f;
        }
        if (item.prices[0].type == BuyPrice.KP_PRICE)
        {
            forGold.alpha = 0.0f;
            forKP.alpha = 1.0f;
           
           
                
                for (int i = 0; i < item.prices.Length; i++)
                {
                    item.prices[i].CheckDiscount();
                    kpPrices[i].text = item.prices[i].GetPrice();
                    kpPrices[i].discount = item.prices[i].GetOldPrice();
                }
           

        }
        else
        {
            if (item.buyMode == BuyMode.FOR_GOLD_FOREVER)
            {

                forGold.alpha = 0.0f;
                forKP.alpha = 0.0f;
            }
            else
            {

                forGold.alpha = 1.0f;
                forKP.alpha = 0.0f;
                for (int i = 0; i < item.prices.Length; i++)
                {
                    item.prices[i].CheckDiscount();
                    goldPrices[i].text = item.prices[i].GetPrice();
                    goldPrices[i].discount = item.prices[i].GetOldPrice();
                }
            }
        }
        if (item.prices[0].discount)
        {
            timer.alpha = 1.0f;
        }
        else
        {
            timer.alpha = 0.0f;
        }
        if (item.isAvailable())
        {
            useItem.alpha = 1.0f;

        }
        else
        {
            useItem.alpha = 0.0f;
        }

        if (justInfo)
        {
            useItem.alpha = 0.0f;
            forGold.alpha = 0.0f;
            forKP.alpha = 0.0f;
        }
		

        
        GA.API.Design.NewEvent("GUI:MainMenu:Shop:SelectItem:" + _item.engName);
        Name.text = item.name;
		
       

        if (gunModel != null)
        {
            Destroy(gunModel);
        }
        gunModel = null;
        ItemManager.instance.LoadModel(item);
        Description.text = item.description;
        weaponSection.widget.alpha = 0.0f;
        armSection.widget.alpha = 0.0f;
        hthSection.widget.alpha = 0.0f;
        if (item.type == ShopSlotType.WEAPON)
        {
            
            WeaponInventorySlot weapon = (WeaponInventorySlot)item;
            weaponSection.widget.alpha = 1.0f;
            weaponSection.magazine.text = weapon.chars.magazine.ToString();
            weaponSection.dmg.value = weapon.chars.dmg;
            weaponSection.aim.value = weapon.chars.aim;
            weaponSection.reload.value = weapon.chars.reload;
            weaponSection.speed.value = weapon.chars.speed;
            weaponSection.mode.text = TextGenerator.instance.GetSimpleText(weapon.chars.gunMode);
         
          
        } else if(item.type == ShopSlotType.ARMOR) {
            ArmorInventorySlot armor = (ArmorInventorySlot)item;
            armSection.def.value = armor.chars.def/100;
          
            armSection.widget.alpha = 1.0f;
        }
        else if (item.type == ShopSlotType.HTHWEAPON)
        {
            HTHWeaponSlot weapon = (HTHWeaponSlot)item;
            hthSection.active.text = weapon.activeBuff;
            hthSection.passive.text = weapon.passiveBuff;
            hthSection.widget.alpha = 1.0f;
        }
       
        loading.alpha = 1.0f;
    }
    public void Buy(int key)
    {
			priceKey = key;
            Shop.askWindow.action = FinishBuy;
            string text;
            if (item.prices[key].type == BuyPrice.KP_PRICE)
            {
                text = TextGenerator.instance.GetMoneyText("buyKPprice", item.prices[key].GetText());
            }
            else if (item.prices[key].type == BuyPrice.GOLD_PRICE_UNBREAKE)
            {
                text = TextGenerator.instance.GetMoneyText("buyUnbreakeprice", item.prices[key].GetText());
            }
            else {
                text = TextGenerator.instance.GetMoneyText("buyGoldPrice", item.prices[key].GetText());
			}

            Shop.askWindow.Show(text);
	}
    public void Choice()
    {
        Shop.SetItemForChoiseSet(item);
    }
	public void FinishBuy(){
        int amount = item.prices[priceKey].GetPrice();
		if(item.prices[0].type==BuyPrice.KP_PRICE){
			GA.API.Business.NewEvent("Shop:BUYItem:" + item.engName, "GASH", amount);
		}else{
		  GA.API.Business.NewEvent("Shop:BUYItem:" + item.engName, "GOLD", amount);
		}
        StartCoroutine( ItemManager.instance.BuyItem(item.prices[priceKey].id,Shop));
    }

    public void Repair()
    {
        Shop.repairGui.ShowRepair(item);
    }
    public void SetError(string error)
    {
        GA.API.Business.NewEvent("Shop:BUYError", error, 0);
        buyLabel.text = error;
        UITweener tweener = buyLabel.GetComponent<UITweener>();
        tweener.tweenFactor = 0.0f;
        tweener.PlayForward();
    }
}
[System.Serializable]
public class CharSection
{
    public UIWidget widget;
    public UILabel mode;
    public UILabel magazine;
    public UIProgressBar dmg;
    public UIProgressBar aim;
    public UIProgressBar speed;
    public UIProgressBar reload;
}
[System.Serializable]
public class ArmorSection
{
    public UIWidget widget;
 
    public UIProgressBar def;

}
[System.Serializable]
public class HTHSection
{
    public UIWidget widget;

    public UILabel passive;

    public UILabel active;

}
