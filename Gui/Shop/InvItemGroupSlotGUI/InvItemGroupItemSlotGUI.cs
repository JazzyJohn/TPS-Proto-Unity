
using UnityEngine;

public class InvItemGroupItemSlotGUI : InvItemGroupSlotGUI
{
	protected override int slotWidth
	{
		get { return GetComponent<UITexture>().width; }
	}
	
	public override void Init (InvItemGroupSlot.BaseSlot slot)
	{
		var itemSlot = (InvItemGroupSlot.ItemSlot) slot;

		transform.FindChild("DaysLabel").GetComponent<UILabel>().text = GetDaysText(itemSlot.Days);
		GetComponent<UITexture>().mainTexture = itemSlot.Item.texture;
		
		NormalizeTextureWidth(GetComponent<UITexture>());
	}
	
	/// <summary>
	/// Cжимает ширину UITexture, сохраняя пропорции UITexture.mainTexture
	/// </summary>
	private static void NormalizeTextureWidth(UITexture uiTexture)
	{
		var tex = uiTexture.mainTexture;
		float mult = (float)uiTexture.height / tex.height;
		
		uiTexture.width = (int)(tex.width * mult);
	}
}
