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

        StartHit();
        
    }
    public override void StartDamage()
    {
        //StartHit();
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
            float distance = Mathf.Max((oldEdgePostion - edge.position).magnitude,radius);
            oldEdgePostion = edge.position;
            Vector3 direction = edge.position - owner.myTransform.position;
            Ray ray = new Ray(owner.myTransform.position, direction.normalized);
            RaycastHit[] hits = Physics.SphereCastAll(ray, distance,  direction.magnitude+0.3f);
           // Debug.DrawRay(owner.myTransform.position, direction.normalized * (direction.magnitude + 0.3f), Color.red, 10.0f);
            foreach (RaycastHit hit in hits)
            {
               // Debug.Log(hit.transform.root);
                if(hit.transform.root==owner.myTransform.root){
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
                    return;
                }
            }
            Collider[]colliders =  Physics.OverlapSphere(edge.position, distance);
            foreach (Collider hit in colliders)
            {
                // Debug.Log(hit.transform.root);
                if (hit.transform.root == owner.myTransform.root)
                {
                    continue;
                }
                DamagebleObject obj = hit.collider.GetComponent<DamagebleObject>();

                if (obj != null)
                {
                    BaseDamage lDamage = GetDamage(damageAmount);
                    lDamage.pushDirection = (hit.transform.position-edge.position);
                    lDamage.hitPosition = hit.transform.position;
                    obj.Damage(lDamage, owner.gameObject);
                    hitting = false;
                    AfterHitAction();
                    return;
                }
            }
        
        }
    }
    void SpawnEffect(Vector3 position)
    {
        if (projectilePrefab.CountPooled() == 0 && projectilePrefab.CountPooled() == 0)
        {
            projectilePrefab.CreatePool(10);

        }
        if (projectilePrefab.CountPooled() == 0 && projectilePrefab.CountPooled() == 0)
        {
            return;
        }
        projectilePrefab.Spawn(position);
    }
    /*     void OnTriggerStay(Collider other)
     {
           Debug.Log(other);
           if (hitting)
           {
          
           
             
                   if (other.transform.root == owner.myTransform.root)
                   {
                       return;
                   }
                   DamagebleObject obj = other.collider.GetComponent<DamagebleObject>();

                   if (obj != null)
                   {
                       BaseDamage lDamage = GetDamage(damageAmount);
                       lDamage.pushDirection = other.transform.position - hilt.position;
                       lDamage.hitPosition = other.transform.position;
                       obj.Damage(lDamage, owner.gameObject);
                       hitting = false;
                       AfterHitAction();
                   }
          

           }
       }
       */
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
