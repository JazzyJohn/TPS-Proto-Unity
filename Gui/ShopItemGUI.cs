using UnityEngine;
using System.Collections;

public class ShopItemGUI : MonoBehaviour {

	public ShopGUI Shop;
    public ShopSlot item;
	public UILabel Name;
	public UILabel PriceKP;
	public UILabel PriceGITP;
    public UILabel Description;
	public UITexture Texture;
    public UIWidget Box;

	[HideInInspector]
	public int numToItem;

	// Use this for initialization
	void Start () {
	
	}

	public void LoadInfo()
	{
		//Код загрузки инфы из xml
	}

	// Update is called once per frame
	void Update () {
        if (item != null &&item.texture!=null&& Texture.mainTexture == null)
        {
            Texture.mainTexture= item.texture;
        }
	}

    public void SetItem(ShopSlot _item)
    {
        item = _item;
        Name.text = item.name;
        PriceKP.text = item.cashCost + " KP";
        PriceGITP.text = item.goldCost + " GITP";
        Description.text = item.description;
        Texture.mainTexture = null;
        
        
    }
        
}
