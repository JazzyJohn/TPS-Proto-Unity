
using UnityEngine;

public class InvItemGroupGUI : MonoBehaviour
{
	[SerializeField] UILabel titleLabel;

	[Space(10)]
	[SerializeField] UIButton buyButton;
	[SerializeField] UILabel buyButtonPrice;

	[Space(10)]
	//отступ между слотами
	[SerializeField] int offset = 0;
	//ширина одного слота (если < 0 ширина слота равна ширине текстурыы)
	[SerializeField] int fixedWidth = 120;
	
	[Header("Prefabs")]
	[SerializeField] GameObject itemPrefab;
	[SerializeField] GameObject moneyPrefab;
	[SerializeField] GameObject plusPrefab;

	private GameObject[] pluses;
	private GameObject[] items;
	
	public bool HasSlot { get; private set; }

	public void SetSlot(InvItemGroupSlot groupSlot)
	{
		if(HasSlot)
			ResetSlot();

		if(groupSlot == null)
			return;

		HasSlot = true;

		titleLabel.text = groupSlot.name;
		buyButtonPrice.text = "" + groupSlot.goldPrice;

		GetComponent<UISprite>().alpha = 1;

		itemPrefab.SetActive(true);
		moneyPrefab.SetActive(true);
		plusPrefab.SetActive(true);
		
		int count = groupSlot.slots.Length;
		int totalWidth = offset;
		int plusWidth = plusPrefab.GetComponent<UITexture>().width;

		items = new GameObject[count];
		pluses = new GameObject[count - 1];

		for(int i = 0; i < count; i++)
		{
			if(groupSlot.slots[i] is InvItemGroupSlot.ItemSlot)
			{
				var itemSlot = (InvItemGroupSlot.ItemSlot) groupSlot.slots[i];

				items[i] = InstantiateClone(itemPrefab);

				items[i].transform.FindChild("DaysLabel").GetComponent<UILabel>().text = GetDaysText(itemSlot.Days);
				items[i].GetComponent<UITexture>().mainTexture = itemSlot.iconTexture;

				NormalizeTextureWidth(items[i].GetComponent<UITexture>());
			}
			else if(groupSlot.slots[i] is InvItemGroupSlot.MoneySlot)
			{
				var moneySlot = (InvItemGroupSlot.MoneySlot) groupSlot.slots[i];

				items[i] = InstantiateClone(moneyPrefab);

				items[i].transform.FindChild("CountLabel").GetComponent<UILabel>().text = "" + moneySlot.moneyCount;
				items[i].transform.FindChild("G_icon").gameObject.SetActive(moneySlot.moneyType == InvItemGroupSlot.MoneySlot.MoneyType.GP);
				items[i].transform.FindChild("K_icon").gameObject.SetActive(moneySlot.moneyType == InvItemGroupSlot.MoneySlot.MoneyType.KP);
			}
			else
			{
				Debug.LogError("NotSupported " + groupSlot.slots[i].GetType());
			}

			totalWidth += calcSlotWidth(items[i], groupSlot.slots[i]);

			if(i < count-1)
			{
				totalWidth += plusWidth + offset;
				pluses[i] = InstantiateClone(plusPrefab);
			}
		}
		
		int x_pos = -totalWidth / 2;
		
		for(int i = 0; i < count; i++)
		{
			int width = calcSlotWidth(items[i], groupSlot.slots[i]);
			SetPosX(items[i], x_pos + width/2);
			x_pos += width + offset;
			
			if(i < count-1)
			{
				width = plusWidth;
				SetPosX(pluses[i], x_pos + width/2);
				x_pos += width + offset;
			}
		}
		
		itemPrefab.SetActive(false);
		moneyPrefab.SetActive(false);
		plusPrefab.SetActive(false);
	}

	public void ResetSlot()
	{
		GetComponent<UISprite>().alpha = 0;

		if(!HasSlot)
			return;

//#if UNITY_EDITOR
//		foreach(var p in pluses) DestroyImmediate(p);
//		foreach(var p in items) DestroyImmediate(p);
//#elif
		foreach(var p in pluses) Destroy(p);
		foreach(var p in items) Destroy(p);
//#endif

		pluses = null;
		items = null;

		HasSlot = false;
	}
	
	/// <summary>
	/// Считает ширину слота
	/// </summary>
	private int calcSlotWidth(GameObject slotGO, InvItemGroupSlot.BaseSlot slotData)
	{
		if(fixedWidth > 0)
			return fixedWidth;

		if(slotData is InvItemGroupSlot.ItemSlot)
		{
			return slotGO.GetComponent<UITexture>().width;
		}
		else if(slotData is InvItemGroupSlot.MoneySlot)
		{
			return fixedWidth;

			// как считать ширину слота для монет?
//			int moneyLetterWidth = 10;
//			int moneyIconWidth = 60;
//			return moneyIconWidth + moneyLetterWidth * ((ItemGroupSlot.MoneySlot)slotData).moneyCount.ToString().Length;
		}
		return 0;
	}

	/// <summary>
	/// Конвентирует количество дней в текстовое представление (русский язык)
	/// </summary>
	private static string GetDaysText(int days)
	{
		if(days < 0)
			return "Навсегда";

		return FormatToWord(days, "день", "дня", "дней");
	}

	/// <summary>
	/// Выбирает правилньное склонение для слова (русском языке)
	/// </summary>
	private static string FormatToWord(int val, string one_word, string twoToFive_word, string tenToTwentyTen_and_FiveToNine_word)
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

	/// <summary>
	/// Cжимает ширину UITexture, сохраняя пропорции UITexture.mainTexture
	/// </summary>
	private static void NormalizeTextureWidth(UITexture uiTexture)
	{
		var tex = uiTexture.mainTexture;
		float mult = (float)uiTexture.height / tex.height;
		
		uiTexture.width = (int)(tex.width * mult);
	}

	/// <summary>
	/// Создаёт клон origin
	/// Помещает клона в иерархии рядом с origin
	/// Присваивает клону позицию, поворот и масштаб origin
	/// </summary>
	private static GameObject InstantiateClone(GameObject origin)
	{
		var go = (GameObject)GameObject.Instantiate(origin);

		var clone = go.transform;
		var originT = origin.transform;

		clone.parent = originT.parent;
		clone.localScale = originT.localScale;
		clone.localRotation = originT.localRotation;
		clone.localPosition = originT.position;

		return go;
	}

	/// <summary>
	/// Устанавливает объекту obj новую локальную позицию по оси X равную val
	/// </summary>
	private static void SetPosX(GameObject obj, int val)
	{
		var t = obj.transform;
		t.localPosition = new Vector3(val, t.localPosition.y, t.localPosition.z);
	}
}