
using System;
using UnityEngine;

public class InvItemGroupSlot
{
    public string id { get; private set; }
	public string name { get; private set; }
	public int goldPrice { get; private set; }
	public BaseSlot[] slots { get; private set; }

    public float discount { get; private set; }

    public DateTime discountEnd;

    public InvItemGroupSlot(string name, int price, BaseSlot[] slots, string id, float discount)
	{
		if(name == null)
			throw new System.ArgumentNullException("name");

		if(slots == null)
			throw new System.ArgumentNullException("slots");

		this.name = name;
		this.goldPrice = price;
		this.slots = slots;
        this.id = id;
        this.discount = discount;
	}
    public int GetPrice()
    {
        if (IsDiscount())
        {
            
            return Mathf.RoundToInt((float)goldPrice * discount);
        }
        return goldPrice;
    }
    public bool IsDiscount()
    {
        if (discount < 1.0f)
        {
            if (DateTime.Now < discountEnd)
            {
                return true;
            }
            else
            {
                discount = 1.0f;
                return false;
            }
        }
        return false;
    }
	public abstract class BaseSlot
	{
		public enum SlotType { Money, Item, Skill }
		
		public SlotType type { get; private set; }
		
		public BaseSlot(SlotType type)
		{
			this.type = type;
		}
	}
	
	public class MoneySlot : BaseSlot
	{
		public enum MoneyType { KP, GP }
		
		public MoneyType moneyType { get; private set; }
		public int moneyCount { get; private set; }
		
		public MoneySlot(MoneyType moneyType, int count) : base(SlotType.Money)
		{
			this.moneyType = moneyType;
			this.moneyCount = count;
		}
	}
	
	public class ItemSlot : BaseSlot
	{
		public InventorySlot Item { get; private set; }
		public int Days { get; private set; }
		
		public ItemSlot(InventorySlot item, int days) : base(SlotType.Item)
		{
			if(item == null)
				throw new System.ArgumentNullException("texture");

			this.Item = item;
			this.Days = days;
		}
	}

	public class SkillSlot : BaseSlot
	{
		public PremiumSkill Skill { get; private set; }
		public int Days { get; private set; }

        public SkillSlot(PremiumSkill Skill, int days)
            : base(SlotType.Skill)
		{
			if(	Skill == null)
				throw new System.ArgumentNullException("spriteName");
			
			this. Skill= Skill;
			this.Days = days;
		}
	}
}

