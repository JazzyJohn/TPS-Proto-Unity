
using UnityEngine;

public class InvItemGroupSkillSlotGUI : InvItemGroupSlotGUI
{
	protected override int slotWidth
	{
		get { return GetComponent<UISprite>().width; }
	}

	public override void Init (InvItemGroupSlot.BaseSlot slot)
	{
		var skillSlot = (InvItemGroupSlot.SkillSlot) slot;

		transform.FindChild("DaysLabel").GetComponent<UILabel>().text = GetDaysText(skillSlot.Days);
		GetComponent<UISprite>().spriteName = skillSlot.Skill.iconGUI;
	}
}
