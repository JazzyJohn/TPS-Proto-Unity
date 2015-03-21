using UnityEngine;
using System.Collections;

public class BaseArmor : MonoBehaviour {   

    public SLOTTYPE slotType;

	public int usePerCharge = 30;

    public float armor;

    public float armorHp;

    protected Transform curTransform;

    protected Pawn owner;

    public int SQLId;

    private float charge;

    private int useCounter;

	// Update is called once per frame
	
    public void AttachArmorToChar(Pawn newowner)
    {
        if (curTransform == null)
        {
            curTransform = transform;
        }
        owner = newowner;
        if (owner == null)
        {
            //Destroy(photonView);
            //Debug.Log("DestoroyATTACHEs");
            Destroy(gameObject);
            return;
        }
        curTransform.parent = owner.GetSlotForItem(slotType);
        curTransform.localPosition = Vector3.zero;
        curTransform.localRotation = Quaternion.identity;
        curTransform.localScale = Vector3.one;



        charge = ItemManager.instance.GetArmorCharge(SQLId);
        useCounter = ItemManager.instance.GetUseCounter(SQLId);
        InitStats();
        owner.AddArmor(this);
       

    }
    public void InitStats(){
        if (charge < 0.75f&&charge > 0.5f)
        {
            armor = armor * 0.5f;
        }
        else if (charge < 0.75f && charge > 0.25f)
        {
            armor = armor * 0.5f;
        }
        else if (charge < 0.75f)
        {
            armor = armor * 0.25f;
        }
    }
    public virtual void Used()
    {
        useCounter++;
        if (useCounter >= usePerCharge)
        {
            useCounter = 0;
			charge = ItemManager.instance.LowerArmorCharge(SQLId);
		}
    }
    public virtual void TurnOff(){
        gameObject.SetActive(false);
    }
    public void AffectDamage(BaseDamage damage)
    {
        if (armorHp <= 0)
        {
            return;
        }
        Used();
        ActualUse(damage);
       
    }
    protected virtual void ActualUse(BaseDamage damage)
    {
        float effectiveArmor = armor - damage.vsArmor;
        if (effectiveArmor > 0)
        {
            float damageReduce =damage.Damage*effectiveArmor/100.0f;
            armorHp -= damageReduce;
            if (armorHp <= 0)
            {
                TurnOff();
            }
            damage.Damage -= damageReduce;
        }
    }
    public void PawnDeath()
    {
        if (owner.foxView.isMine && !owner.isAi)
        {
            ItemManager.instance.SetShootCount(SQLId, useCounter);

        }
    }
}
