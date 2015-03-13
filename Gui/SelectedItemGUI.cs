using UnityEngine;
using System.Collections;

public class SelectedItemGUI : MonoBehaviour {

	public InventoryGUI Shop;
    public InventorySlot item;

	public UITexture Texture;
  
    public UILabel name;

    public int slot;
    

	[HideInInspector]
	public int numToItem;

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

    public void SetItem()
    {
		InventorySlot _item=null;
		WeaponIndex choice = Choice.ForGuiSlot(slot);
		if (!choice.IsSameIndex( WeaponIndex.Zero))
		{
            _item = ItemManager.instance.GetWeaponSlotbByID(choice);
		}
		if(_item==null)
		{
			_item =ItemManager.instance.GetFirstItemForSlot((GameClassEnum)Choice._Player, slot);
            WeaponInventorySlot weapon = (WeaponInventorySlot)_item;
            if (weapon != null)
            {
                Choice.SetChoice(slot, Choice._Player, new WeaponIndex(weapon.weaponId, ""));
            }
		}
		if(_item==null){
            name.alpha = 1.0f;
			return;
		}
        item = _item;
        GA.API.Design.NewEvent("GUI:MainMenu:Inventory:SelectItem:" + _item.engName);
        Texture.mainTexture = null;
        name.alpha = 0.0f;
        
    }
        
}
