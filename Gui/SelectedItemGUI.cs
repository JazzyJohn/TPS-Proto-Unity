using UnityEngine;
using System.Collections;

public class SelectedItemGUI : MonoBehaviour {

	public InventoryGUI Shop;
    public InventorySlot item;

	public UITexture Texture;
  
    public UILabel name;

    public int slot;

    public int close = 2;

    public UIWidget closeBox;
    public UIWidget openBox;

    public UIWidget box;

    public Color open;

	[HideInInspector]
	public int numToItem;
    public void Awake(){
        box = GetComponent<UIWidget>();
    }
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
        if (item != null &&item.texture!=null&& Texture.mainTexture == null)
        {
            Texture.mainTexture= item.texture;
           
        }
	}
    public void TryOpen()
    {
        if (close != 1)
        {
            if (close <= GlobalPlayer.instance.open_set)
            {
                closeBox.alpha = 0.0f;
                openBox.alpha = 1.0f;
                box.color = open;

            }
        }
    }

    public bool SetItem()
    {
		InventorySlot _item=null;
        bool needSave = false;
		WeaponIndex choice = Choice.ForGuiSlot(slot);
		if (!choice.IsSameIndex( WeaponIndex.Zero))
		{
            switch (slot)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    _item = ItemManager.instance.GetWeaponSlotbByID(choice);
                    break;
                case 4:
                case 5:
                    _item = ItemManager.instance.GetArmorSlotbByID(choice);
                    break;
            }
            
		}
		if(_item==null)
		{
			_item =ItemManager.instance.GetFirstItemForSlot((GameClassEnum)Choice._Player, slot);
            needSave = true;
            WeaponInventorySlot weapon = _item as WeaponInventorySlot;
            if (weapon != null)
            {
                Choice.SetChoice(slot, Choice._Player, new WeaponIndex(weapon.weaponId, ""));
            }
            ArmorInventorySlot armor = _item as ArmorInventorySlot;
            if (armor != null)
            {
                Choice.SetChoice(slot, Choice._Player, new WeaponIndex(armor.armorId, ""));
            }
		}
		if(_item==null){
            name.alpha = 1.0f;
            return  false;
		}
        item = _item;
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:SelectItem:" + _item.engName);
        Texture.mainTexture = null;
        name.alpha = 0.0f;
        return needSave;
    }
        
}
