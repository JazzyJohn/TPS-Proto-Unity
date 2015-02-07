using UnityEngine;
using System.Collections;

public class LotItemGUI : MonoBehaviour
{


    public CharSection weaponSection;

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
	
	public UILabel[] goldPrices;
	
	public UIWidget forKP;
	
	public UILabel[] kpPrices;
	
	
    [HideInInspector]
    public int numToItem;

  
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
    }

    public void SetItem(InventorySlot _item)
    {
        item = _item;
		if(item.prices[0].type==BuyPrice.KP_PRICE){
			forGold.alpha = 0.0f;
			forKP.alpha = 1.0f;
            for (int i = 0; i < item.prices.Length; i++)
            {
				kpPrices[i].text =item.prices[i].parts[0].amount.ToString();
			}
		}else{
			forGold.alpha = 1.0f;
			forKP.alpha = 0.0f;
			for(int i =0;i< item.prices.Length;i++){
                goldPrices[i].text = item.prices[i].parts[0].amount.ToString();
			}
		}
        GA.API.Design.NewEvent("GUI:MainMenu:Shop:SelectItem:" + _item.engName);
        Name.text = item.name;
		
        Description.text = item.description;

        if (gunModel != null)
        {
            Destroy(gunModel);
        }
        gunModel = null;
        ItemManager.instance.LoadModel(item);
        if (item.type == ShopSlotType.WEAPON)
        {
            weaponSection.widget.alpha = 1.0f;
            weaponSection.magazine.text = item.chars.magazine.ToString();
            weaponSection.dmg.value = item.chars.dmg;
            weaponSection.aim.value = item.chars.aim;
            weaponSection.reload.value = item.chars.reload;
            weaponSection.speed.value = item.chars.speed;
            weaponSection.mode.text = TextGenerator.instance.GetSimpleText(item.chars.gunMode);
        }
        else
        {
            weaponSection.widget.alpha = 0.0f;
        }
        loading.alpha = 1.0f;
    }
    public void Buy(int key)
    {
		int amount = item.prices[key].parts[0].amount;
		if(item.prices[0].type==BuyPrice.KP_PRICE){
			GA.API.Business.NewEvent("Shop:BUYItem:" + item.engName, "GASH", amount);
		}else{
		  GA.API.Business.NewEvent("Shop:BUYItem:" + item.engName, "GOLD", amount);
		}
        StartCoroutine( ItemManager.instance.BuyItem(item.prices[key].id,this));
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