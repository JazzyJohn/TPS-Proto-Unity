using UnityEngine;
using System.Collections;

public class BaseHelmet : BaseArmor{
    public float coolDown = 10.0f;

    private float lastActive = -10.0f;

    protected override void ActualUse(BaseDamage damage)
    {
        if (lastActive + coolDown > Time.time)
        {
            return;
        }
        damage.isHeadshoot = false;
        float headDamage =(damage.Damage -damage.Damage / Pawn.HEAD_SHOOT_MULTIPLIER);
        damage.Damage -= headDamage * armor;
    }
}
