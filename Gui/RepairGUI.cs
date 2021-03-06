using UnityEngine;
using System.Collections;

public class RepairGUI : MonoBehaviour {

	public UIWidget repairWindow;

    public InventoryGUI shop;	
	
	public UILabel repairLabel;

    public UILabel unbreakPrice;
	
	public UIScrollBar repairScroll; 
		
	public int amount;
	
    public InventorySlot item;
	
	public void ShowRepair(InventorySlot item){
     //   Debug.Log(item.maxcharge);
        if (item.maxcharge == 0)
        {
            return;
        }

       // Debug.Log(item.maxcharge);
        this.item = item;
		repairWindow.alpha = 1.0f;
        unbreakPrice.text = item.prices[1].parts[0].amount.ToString();
        repairScroll.value = (float)(item.maxcharge-item.charge) / (float)item.maxcharge;
     ///   Debug.Log(item.maxcharge);
        repairLabel.text = "0";
        amount = 0;
		
	}
	
	public void SetValue(UIScrollBar ScrollArg) //Установка звука (Текст)
	{
        if (item == null)
        {
            Debug.Log("no item");
            return;
        }
        int value = Mathf.RoundToInt( ScrollArg.value * (float)item.maxcharge);
//        Debug.Log(value);
        if (value < (item.maxcharge - item.charge))
        {
            ScrollArg.value = (float)(item.maxcharge - item.charge) / (float)item.maxcharge; 
            //ScrollArg.value = Mathf.RoundToInt((float)(item.maxcharge - item.charge) / (float)item.maxcharge);
            repairLabel.text = "0";
            amount = 0;
        }
        else
        {


            repairLabel.text = (item.repairCost * (value - (item.maxcharge - item.charge))).ToString();
            amount = (value - (item.maxcharge - item.charge));
        }
		
	}
	
	public void Close(){
		repairWindow.alpha = 0.0f;	
	}
	
	public void Repair(){
        GA.API.Business.NewEvent("Shop:RepairItem:" + item.engName, "GASH", item.repairCost * (amount - item.charge));
		  ItemManager.instance.UseRepairKit(item.id,amount,shop);
	}

	
	
}