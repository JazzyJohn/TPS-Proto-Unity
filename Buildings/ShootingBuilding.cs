using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootingBuilding : Building
{
    private static int SHOOT_BUILDING_ID=-100;

    public float lifeTime;

    public float fireInterval;

    private float time;

    public Transform muzzle;

    public BaseWeapon.AMUNITONTYPE type;

    public GameObject projectile;

    public float radius;

    public BaseDamage damage;

    public float effAimRandCoef;

    public Transform myTransform;
    void Awake()
    {
        Invoke("DestroyBuilding", lifeTime);
        myTransform = transform;
        base.Awake();
    }

    private void DestroyBuilding(){
        if (foxView.isMine)
        {
            RequestKillMe();
        }
        Destroy(gameObject);
    }
	void Update () {
        time += Time.deltaTime;
        if (time > fireInterval)
        {
            time = 0;
            Quaternion startRotation = muzzle.rotation;
            if (type == BaseWeapon.AMUNITONTYPE.RAY)
            {
                Ray ray = new Ray(muzzle.position, startRotation * Vector3.forward);

                RaycastHit[] hits = Physics .SphereCastAll(ray, radius, (myTransform.position - muzzle.position).magnitude + 1.0f);
                List<Transform> targets = new List<Transform>();
                foreach (RaycastHit hit in hits)
                {
                    DamagebleObject obj = hit.collider.GetComponent<DamagebleObject>();
                    if (obj != null && !targets.Contains(hit.transform.root))
                    {
                        targets.Add(hit.transform.root);
                        obj.Damage(new BaseDamage(damage), player.GetCurrentPawn().gameObject);

                    }
                    if (Mathf.Abs(hit.point.z - myTransform.position.z) < 1.0f)
                        projectile.Spawn(hit.point, Quaternion.LookRotation(hit.normal));
                    }
              }
              else{
                if (effAimRandCoef > 0)
                {
                    startRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(Random.Range(-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range(-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range(-1 * effAimRandCoef, 1 * effAimRandCoef)));
                }
                GameObject proj = projectile.Spawn(muzzle.position, startRotation);
                BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
                projScript.replication = false;
                damage.shootWeapon = SHOOT_BUILDING_ID;
                projScript.fromGun = false;
                projScript.damage = new BaseDamage(damage);

                projScript.owner = player.GetCurrentPawn().gameObject;

                projScript.range = 10000f;
                projScript.minRange = 10000f;
                projScript.Init();
            }
          

           
        }
	}
}
