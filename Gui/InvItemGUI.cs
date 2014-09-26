using UnityEngine;
using System.Collections;

public class InvItemGUI : MonoBehaviour {

	public InventoryGUI Shop;
    public InventorySlot item;
	public UILabel Name;
	public UILabel PriceKP;
	public UILabel PriceGITP;
    public UILabel Description;
	public UITexture Texture;
    public UIWidget Box;
    public UILabel loading;

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
            loading.alpha = 0.0f;
        }
	}

    public void SetItem(InventorySlot _item)
    {
        item = _item;
        Name.text = item.name;
        PriceKP.text = item.cashCost + " KP";
        PriceGITP.text = item.goldCost + " GITP";
        Description.text = item.description;
        Texture.mainTexture = null;
        loading.alpha = 1.0f;
        
    }
        
}
