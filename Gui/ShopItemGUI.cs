using UnityEngine;
using System.Collections;

public class ShopItemGUI : MonoBehaviour {

	public ShopGUI Shop;
    public ShopSlot item;
	public UILabel Name;
	public UILabel PriceKP;
	public UIWidget PriceKPBox;
	public UILabel PriceGITP;
    public UILabel Description;
	public UITexture Texture;
    public UIWidget Box;
    public UILabel loading;
	public UISprite star;
	
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

    public void SetItem(ShopSlot _item)
    {
        item = _item;
        Name.text = item.name;
		if( item.cashCost ==0){
			PriceKP.text = item.cashCost + " KP";
			PriceKPBox.alpha = 0.0f;
		}
        PriceGITP.text = item.goldCost + " GITP";
        Description.text = item.description;
        Texture.mainTexture = null;
        loading.alpha = 1.0f;
		if(ItemManager.instance.IsMarked(item.id)){
			star.alpha = 1.0f;
		}else{
			star.alpha = 0.0f;
		}
    }
        
}
