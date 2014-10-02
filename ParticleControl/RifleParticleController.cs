using UnityEngine;
using System.Collections;

public class RifleParticleController : MonoBehaviour
{
	//Объявляем наши компоненты
	private ParticleSystem flameParticles;
	private ShellparticleSystem shellParticles;
	private ParticleSystem smokeParticles;
	public GameObject simpleRay;
	
	protected Collider owner;
	//Инициализация, получаем ссылки на необходимые нам компоненты
	protected void Start()
	{
		flameParticles = transform.GetChild (0).GetComponent<ParticleSystem> ();
		shellParticles = transform.GetChild (1).GetComponent<ShellparticleSystem> ();
		smokeParticles = transform.GetChild (2).GetComponent<ParticleSystem> ();
	}

	public void SetOwner(Collider newOwner){
		owner = newOwner;
	}
	//Функция создания самого выстрела
	public void CreateShootFlame()
	{
		//Шанс получения пламени при выстреле
		if (Random.Range (0, 2) == 1)
		{
            if (flameParticles != null)
            {
                flameParticles.Play();
            }
		}
		if (smokeParticles != null) {
				smokeParticles.Play ();
		}
		if (shellParticles != null) {
			shellParticles.Play (owner);
		}
	}
	public virtual void StartFlame(){
		flameParticles.Play();
	}
    public virtual void StopFlame()
    {
		flameParticles.Stop();
	}
	
	public void CreateRay(Vector3 start, Vector3 direction){
			Instantiate(simpleRay, start, Quaternion.LookRotation(direction)); 
	}
	public void CreateLine(Vector3 start, Vector3 point){
			Instantiate(simpleRay, start, Quaternion.LookRotation((point -start).normalized));
			//TODO:: do size logic;
	}
}
