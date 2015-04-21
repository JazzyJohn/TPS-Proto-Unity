using UnityEngine;
using System.Collections;

public class ShootingBuilding : Building
{

    public float lifeTime;

    public float fireInterval;

    private float time;

    public Transform muzzle;

    public GameObject projectile;

    public BaseDamage damage;

    public float effAimRandCoef;
    void Awake()
    {
        Invoke("DestroyBuilding", lifeTime);
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

          

            if (effAimRandCoef > 0)
            {
                startRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(Random.Range(-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range(-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range(-1 * effAimRandCoef, 1 * effAimRandCoef)));
            }
            GameObject proj = projectile.Spawn(muzzle.position, startRotation);
            BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
            projScript.replication = false;
          
            projScript.fromGun = false;
            projScript.damage = new BaseDamage(damage);

            projScript.owner = player.GetCurrentPawn().gameObject;

            projScript.range = 10000f;
            projScript.minRange = 10000f;
            projScript.Init();
        }
	}
}
