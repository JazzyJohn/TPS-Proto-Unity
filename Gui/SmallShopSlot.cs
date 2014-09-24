using UnityEngine;
using System.Collections;

public class SmallShopSlot : MonoBehaviour {

	public UITexture texture;
	
	public UILabel text;
	
	public string itemId;
	
	
	public SmallShopData data;
	
	public void SetObject(SmallShopData data){
		this.data =data;
		texture.mainTexture = data.textureGUI;
		text.text = data.name + ": " +data.amount.ToString();
		itemId= data.itemId;
	}
	
	public void Hide(){
		texture.alpha= 0.0f;
	
	}
}
