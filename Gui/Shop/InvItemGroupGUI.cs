
using System;
using System.Collections.Generic;
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
	
	[Header("Prefabs")]
	[SerializeField] GameObject itemPrefab;
	[SerializeField] GameObject skillPrefab;
	[SerializeField] GameObject moneyPrefab;
	[SerializeField] GameObject plusPrefab;

	private GameObject[] pluses;
	private InvItemGroupSlotGUI[] items;
	
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
		int totalWidth = 0;
		int plusWidth = plusPrefab.GetComponent<UITexture>().width;

		items = new InvItemGroupSlotGUI[count];
		pluses = new GameObject[count - 1];

		var prefabsDict = new Dictionary<InvItemGroupSlot.BaseSlot.SlotType, GameObject>()
		{
			{ InvItemGroupSlot.BaseSlot.SlotType.Item, itemPrefab },
			{ InvItemGroupSlot.BaseSlot.SlotType.Money, moneyPrefab },
			{ InvItemGroupSlot.BaseSlot.SlotType.Skill, skillPrefab }
		};

		for(int i = 0; i < count; i++)
		{
			var slot = groupSlot.slots[i];
			var prefab = prefabsDict[slot.type];

			items[i] = InstantiateClone(prefab).GetComponent<InvItemGroupSlotGUI>();
			items[i].Init(slot);

			totalWidth += items[i].Width;

			if(i < count-1)
			{
				pluses[i] = InstantiateClone(plusPrefab);
				totalWidth += plusWidth;
			}
		}
		
		int x_pos = -totalWidth / 2;
		
		for(int i = 0; i < count; i++)
		{
			int width = items[i].Width;
			SetPosX(items[i].gameObject, x_pos + width/2);
			x_pos += width;
			
			if(i < count-1)
			{
				width = plusWidth;
				SetPosX(pluses[i], x_pos + width/2);
				x_pos += width;
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

		foreach(var p in pluses) Destroy(p.gameObject);
		foreach(var p in items) Destroy(p.gameObject);

		pluses = null;
		items = null;

		HasSlot = false;
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