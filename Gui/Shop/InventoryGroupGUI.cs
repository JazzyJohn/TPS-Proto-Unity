
using UnityEngine;

/// <example>
/// 
///	slot = new InvItemGroupSlot("Название набора", price, new InvItemGroupSlot.BaseSlot[] {
///		new InvItemGroupSlot.MoneySlot(InvItemGroupSlot.MoneySlot.MoneyType.GP, moneyCount),
///		new InvItemGroupSlot.ItemSlot(inventorySlot, days),
/// 	new InvItemGroupSlot.SkillSlot(spriteName, days),
///		...
///	});
///	GetComponent<InventoryGroupGUI>().SetSlot(index, slot);
///
/// </summary>

[RequireComponent(typeof(UIWidget))]
public class InventoryGroupGUI : MonoBehaviour
{
	[SerializeField] InvItemGroupGUI[] slots;
    [SerializeField]  MainMenuGUI mainMenu;
	void Awake()
	{
		ResetSlots();
	}

	public void ToggleVisible()
	{
        if (mainMenu.inGame)
        {
            return;
        }
		var a = GetComponent<UIWidget>().alpha;
		GetComponent<UIWidget>().alpha = (Mathf.Approximately(a, 1) ? 0f : 1f);
	}
    public void Hide()
    {
        GetComponent<UIWidget>().alpha = 0.0f;
    }
	public void SetSlot(int index, InvItemGroupSlot slot)
	{
		if(index < 0 || index >= slots.Length)
			return;

		slots[index].SetSlot(slot);
	}
	
	public void ResetSlots()
	{
		foreach(var slot in slots)
		{
			slot.ResetSlot();
		}
	}
}