
using UnityEngine;

public abstract class InvItemGroupSlotGUI : MonoBehaviour
{
	[SerializeField] int offset;

	public int Width
	{
		get { return offset + slotWidth; }
	}

	protected abstract int slotWidth { get; }

	public abstract void Init(InvItemGroupSlot.BaseSlot slot);

	/// <summary>
	/// Конвентирует количество дней в текстовое представление (русский язык)
	/// </summary>
	protected static string GetDaysText(int days)
	{
		if(days < 0)
			return "Навсегда";
		
		return FormatToWord(days, "день", "дня", "дней");
	}
	
	/// <summary>
	/// Выбирает правилньное склонение для слова (русском языке)
	/// </summary>
	protected static string FormatToWord(int val, string one_word, string twoToFive_word, string tenToTwentyTen_and_FiveToNine_word)
	{
		string prefix = "" + val + " ";
		
		if(val >= 10 && val <= 20)
			return prefix + tenToTwentyTen_and_FiveToNine_word;
		
		if(val % 10 == 1)
			return prefix + one_word;
		
		if(val % 10 >= 2 && val % 10 <= 4)
			return prefix + twoToFive_word;
		
		return prefix + tenToTwentyTen_and_FiveToNine_word;
	}
}