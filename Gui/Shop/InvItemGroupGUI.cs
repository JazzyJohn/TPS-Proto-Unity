
using System;
using UnityEngine;

public class InvItemGroupGUI : MonoBehaviour
{
	[SerializeField] UILabel titleLabel;

	[Space(10)]
	[SerializeField] UIButton buyButton;
	[SerializeField] UILabel buyButtonPrice;

    [Space(10)]
    [SerializeField] UILabel discountTime;
    [SerializeField] UIWidget discount;

	[Space(10)]
	//отступ между слотами
	[SerializeField] int offset = 0;
	//ширина одного слота (если < 0 ширина слота равна ширине текстурыы)
	[SerializeField] int fixedWidth = 120;
	
	[Header("Prefabs")]
	[SerializeField] GameObject itemPrefab;
	[SerializeField] GameObject skillPrefab;
	[SerializeField] GameObject moneyPrefab;
	[SerializeField] GameObject plusPrefab;

	private GameObject[] pluses;
	private GameObject[] items;
	
	public bool HasSlot { get; private set; }

    private InvItemGroupSlot _groupSlot;

    public void Update()
    {
        if (HasSlot&& discount != null)
        {
            if (discount.alpha == 1.0f)
            {
                if (_groupSlot.IsDiscount())
                {
                   
                    discountTime.text = IndicatorManager.GetLeftTime(_groupSlot.discountEnd);
                }
                else
                {
                    discount.alpha = 0.0f;
                }

            }
        }
    }

	public void SetSlot(InvItemGroupSlot groupSlot)
	{
		if(HasSlot)
			ResetSlot();

		if(groupSlot == null)
			return;

        _groupSlot = groupSlot;
		HasSlot = true;

		titleLabel.text = groupSlot.name;
        if (buyButtonPrice != null)
        {
            buyButtonPrice.text = "" + groupSlot.GetPrice();
        }
        if (discount != null)
        {
            if (groupSlot.IsDiscount())
            {
                discount.alpha = 1.0f;
            }
            else
            {
                discount.alpha = 0.0f;
            }
        }
		GetComponent<UISprite>().alpha = 1;

		itemPrefab.SetActive(true);
		moneyPrefab.SetActive(true);
        skillPrefab.SetActive(true);
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
				items[i].GetComponent<UITexture>().mainTexture = itemSlot.Item.texture;

				NormalizeTextureWidth(items[i].GetComponent<UITexture>());
			}
			else if(groupSlot.slots[i] is InvItemGroupSlot.SkillSlot)
			{
				var skillSlot = (InvItemGroupSlot.SkillSlot) groupSlot.slots[i];
				
				items[i] = InstantiateClone(skillPrefab);
				
				items[i].transform.FindChild("DaysLabel").GetComponent<UILabel>().text = GetDaysText(skillSlot.Days);
				items[i].GetComponent<UISprite>().spriteName = skillSlot.Skill.iconGUI;
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
        skillPrefab.SetActive(false);
	}

    public void Buy()
    {
        InventoryGUI Shop = FindObjectOfType<InventoryGUI>();
        Shop.askWindow.action = FinishBuy;
        string text = TextGenerator.instance.GetMoneyText("buyKit", _groupSlot.GetPrice().ToString(), _groupSlot.name);
      
        Shop.askWindow.Show(text);
    }

    public void FinishBuy()
    {

        IndicatorManager.instance.Remove(IndicatorManager.KITS, _groupSlot.discountEnd);
        GA.API.Business.NewEvent("Shop:BUYKIT:" + _groupSlot.id, "GOLD", _groupSlot.GetPrice());

        StartCoroutine(ItemManager.instance.BuyItemKit(_groupSlot.id));
    }
	public void ResetSlot()
	{
		GetComponent<UISprite>().alpha = 0;

		if(!HasSlot)
			return;

		foreach(var p in pluses) Destroy(p);
		foreach(var p in items) Destroy(p);

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
		else if(slotData is InvItemGroupSlot.SkillSlot)
		{
			return slotGO.GetComponent<UISprite>().width;
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