using UnityEngine;
using System.Collections;

public class InvItemGUI : MonoBehaviour {

	public InventoryGUI Shop;
    public InventorySlot item;

	public UITexture Texture;
    public UIWidget Box;
    public UILabel chargeLabel;
    

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

    public void SetItem(InventorySlot _item)
    {
        item = _item;
       
        Texture.mainTexture = null;

        if (_item.personal)
        {
            chargeLabel.text = _item.charge.ToString();
        }
        else
        {
            if (_item.isTimed)
            {
                chargeLabel.text = _item.timeEnd.ToString("g");
            }
            else
            {
                chargeLabel.text = "";
            }
            
        }
        
    }
        
}
