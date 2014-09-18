using UnityEngine;
using System.Collections;

public class LotItemGUI : MonoBehaviour
{


    public ShopGUI Shop;
    public ShopSlot item;
    public UILabel Name;
    public UILabel PriceKP;
    public UILabel PriceGITP;
    public UILabel Description;
    public Transform gun;
    public GameObject gunModel;
    public UIWidget Box;

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
           
        }
    }

    public void SetItem(ShopSlot _item)
    {
        item = _item;
        Name.text = item.name;
        PriceKP.text = item.cashCost + " KP";
        PriceGITP.text = item.goldCost + " GITP";
        Description.text = item.description;
        gunModel = null;
        ItemManager.instance.LoadModel(item);
    }
    public void KPBuy()
    {
        ItemManager.instance.BuyItem(item.cashSlot);
    }
    public void GITPBuy()
    {
        ItemManager.instance.BuyItem(item.goldSlot);
    }
}
