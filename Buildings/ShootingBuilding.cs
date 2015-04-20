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

    void Awake()
    {
        Invoke("DestroyBuilding", lifeTime);
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

            GameObject proj = projectile.Spawn(muzzle.position, muzzle.rotation);
            BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
            projScript.replication = false;
          
            projScript.fromGun = false;
            projScript.damage = new BaseDamage(damage);

            projScript.owner = player.GetActivePawn().gameObject;

            projScript.range = 10000f;
            projScript.minRange = 10000f;
            projScript.Init();
        }
	}
}
