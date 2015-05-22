
using UnityEngine;

public class InvItemGroupMoneySlotGUI : InvItemGroupSlotGUI
{
	[SerializeField] int fixedWidth = 120;

	protected override int slotWidth
	{
		get { return fixedWidth; }
		//return moneyIconWidth + moneyLetterWidth * ((ItemGroupSlot.MoneySlot)slotData).moneyCount.ToString().Length;
	}
	
	public override void Init (InvItemGroupSlot.BaseSlot slot)
	{
		var moneySlot = (InvItemGroupSlot.MoneySlot) slot;

		transform.FindChild("CountLabel").GetComponent<UILabel>().text = "" + moneySlot.moneyCount;
		transform.FindChild("G_icon").gameObject.SetActive(moneySlot.moneyType == InvItemGroupSlot.MoneySlot.MoneyType.GP);
		transform.FindChild("K_icon").gameObject.SetActive(moneySlot.moneyType == InvItemGroupSlot.MoneySlot.MoneyType.KP);
	}
}