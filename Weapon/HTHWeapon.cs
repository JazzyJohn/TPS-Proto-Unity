using UnityEngine;
using System.Collections;
[System.Serializable]
public class ActiveBuff:CharacteristicForItems
{
    public float timeAdd;


}

public class HTHWeapon : BaseWeapon {

    public Transform hilt;


    public Transform edge;

    public Vector3 oldEdgePostion;

    public float radius;

    bool hitting = false;

    public ActiveBuff[] buffs;

    public float instantHeal;

    public override void StartFire()
    {
        owner.animator.StartShootAniamtion("meleehit");
        
    }
    public override void StartDamage()
    {
        StartHit();
    }
    public void StartHit()
    {
        Debug.Log("StartHit");
        oldEdgePostion = edge.position;
        hitting = true;
    }

    void Update()
    {
        if (hitting)
        {
            float distance = (oldEdgePostion - edge.position).magnitude;
            oldEdgePostion = edge.position;
            Vector3 direction =  edge.position- hilt.position;
            Ray ray = new Ray(hilt.position, direction.normalized);
            RaycastHit[] hits = Physics.RaycastAll(ray,   direction.magnitude);
            Debug.DrawRay(hilt.position, direction.normalized, Color.red, 10.0f);
            foreach (RaycastHit hit in hits)
            {
                if(hit.transform.root==owner.myTransform){
                    continue;
                }
                DamagebleObject obj = hit.collider.GetComponent<DamagebleObject>();

                if (obj != null)
                {
                    BaseDamage lDamage = GetDamage(damageAmount);
                    lDamage.pushDirection =hit.normal*-1;
                    lDamage.hitPosition = hit.point;
                    obj.Damage(lDamage, owner.gameObject);
                    hitting = false;
                    AfterHitAction();
                }
            }

        }
    }

    
    public override void PutAway()
    {
        hitting = false;
        owner.animator.MainLayerNormal();
        base.PutAway();
    }
    
    public  void  AfterHitAction(){
        if (!fullyBroken)
        {

            foreach (ActiveBuff buff in buffs)
            {
                switch (buff.type)
                {
                    case CharacteristicType.Bool:
                        {

                            Effect<bool> effect = new Effect<bool>(buff.value == 1.0f);
                            effect.endByDeath = true;
                            if (buff.timeAdd == -1)
                            {
                                effect.timeEnd = -1;
                            }
                            else
                            {
                                effect.timeEnd = (int)(buff.timeAdd + Time.time);
                                //StartCoroutine(owner.ReloadStats(buff.timeAdd));
                            }
                            effect.type = buff.effect;
                            owner.AddBaseBuff((int)buff.name, effect);
                        }
                        break;
                    case CharacteristicType.Float:
                        {

                            Effect<float> effect = new Effect<float>(buff.value);
                            effect.endByDeath = true;
                            if (buff.timeAdd == -1)
                            {
                                effect.timeEnd = -1;
                            }
                            else
                            {
                                effect.timeEnd = (int)(buff.timeAdd + Time.time);
                              //  StartCoroutine(owner.ReloadStats(buff.timeAdd));
                            }
                            effect.type = buff.effect;
                            owner.AddBaseBuff((int)buff.name, effect);
                        }
                        break;
                    case CharacteristicType.Int:
                        {

                            Effect<int> effect = new Effect<int>((int)buff.value);
                            effect.endByDeath = true;
                            if (buff.timeAdd == -1)
                            {
                                effect.timeEnd = -1;
                            }
                            else
                            {
                                effect.timeEnd = (int)(buff.timeAdd + Time.time);
                                //StartCoroutine(owner.ReloadStats(buff.timeAdd));
                            }
                            effect.type = buff.effect;
                            owner.AddBaseBuff((int)buff.name, effect);
                        }
                        break;

                }
            }
            if (instantHeal > 0)
            {
                owner.Heal(instantHeal, owner.gameObject);
            }
            owner.ReloadStats();

        }
        AfterShootLogic();
    }
    public override void TakeInHand(Transform weaponSlot, Vector3 Offset, Quaternion weaponRotator)
    {
        base.TakeInHand(weaponSlot, Offset, weaponRotator);
        owner.animator.aimPos.AimOff();
        owner.animator.MainLayerPripity();

    }
    public override bool IsMelee()
    {
        return false;
    }
}
