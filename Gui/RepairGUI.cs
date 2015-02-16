using UnityEngine;
using System.Collections;

public class RepairGUI : MonoBehaviour {

	public UIWidget repairWindow;
	
	
	public UILabel repairLabel;
	
	public UIScrollBar repairScroll; 
		
	public int amount;
	
    public InventorySlot item;
	
	public void ShowRepair(InventorySlot item){
		repairWindow.aplha = 1.0f;
		repairScroll.value = (float)item.charge/(float)item.maxCharge;
		
	}
	
	public void SetValueVolume(UIScrollBar ScrollArg) //Установка звука (Текст)
	{
		int value = ScrollArg.value*(float)item.maxCharge;
		if(	value <item.charge){
			value = item.charge;
		}
		ScrollArg.value= value;
		amount= value;
		repairLabel.text  =repairCost*(value-item.charge);
		
	}
	
	public void Close(){
		repairWindow.aplha = 0.0f;	
	}
	
	public void Repair(){
			GA.API.Business.NewEvent("Shop:RepairItem:" + item.engName, "GASH", repairCost*(amount-item.charge));
		  StartCoroutine( ItemManager.instance.UseRepairKit(item.id,amount,this));
	}

	
	
}