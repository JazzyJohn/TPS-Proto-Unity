using UnityEngine;
using System.Collections;

public class DetailItemGUI : MonoBehaviour
{


    public InventoryGUI Shop;
    public InventorySlot item;
    public UILabel Name;
    public UILabel Description;
    public Transform gun;
    public GameObject gunModel;
    public UIWidget Box;
    public UILabel loading;
    public UIWidget repair;
    public UIWidget raspil;
    [HideInInspector]
    public int numToItem;

    public CharSection weaponSection;

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
	//LoL SetSetOfItemToSetOfSets
	public void SetToSet(){
		Shop.SetItemForChoiseSet(item);
	}
	
    public void SetItem(InventorySlot _item)
    {
        item = _item;
        GA.API.Design.NewEvent("GUI:MainMenu:ShowDetailItem:" + item.engName, 1); 

        Name.text = item.name;
        Description.text = item.description;

        if (gunModel != null)
        {
            Destroy(gunModel);
        }
        gunModel = null;
        ItemManager.instance.LoadModel(item);
        loading.alpha = 1.0f;
        if (item.buyMode == BuyMode.FOR_KP)
        {
            repair.alpha = 1.0f;
        }
        else
        {
            repair.alpha = 0.0f;
        }
        if (item.type == ShopSlotType.ETC)
        {
            raspil.alpha = 0.0f;
        }
        else
        {
            raspil.alpha = 1.0f;
        }
       
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
    }
  
}
