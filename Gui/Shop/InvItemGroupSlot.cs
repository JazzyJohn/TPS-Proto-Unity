
using UnityEngine;

public class InvItemGroupSlot
{
    public string id { get; private set; }
	public string name { get; private set; }
	public int goldPrice { get; private set; }
	public BaseSlot[] slots { get; private set; }
	
  
	public InvItemGroupSlot(string name, int price, BaseSlot[] slots)
	{
		if(name == null)
			throw new System.ArgumentNullException("name");

		if(slots == null)
			throw new System.ArgumentNullException("slots");

		this.name = name;
		this.goldPrice = price;
		this.slots = slots;
	}
	
	public abstract class BaseSlot
	{
		public enum SlotType { Money, Item }
		
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
		public Texture2D iconTexture { get; private set; }
		public int Days { get; private set; }
		
		public ItemSlot(Texture2D texture, int days) : base(SlotType.Item)
		{
			if(texture == null)
				throw new System.ArgumentNullException("texture");

			this.iconTexture = texture;
			this.Days = days;
		}
	}
}

