using UnityEngine;
using System.Collections;

public class RifleParticleController : MonoBehaviour
{
	//Объявляем наши компоненты
	private ParticleSystem flameParticles;
	private ShellparticleSystem shellParticles;
	private ParticleSystem smokeParticles;
	protected Collider owner;
	//Инициализация, получаем ссылки на необходимые нам компоненты
	void Start()
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
			flameParticles.Play();
		}
		if (smokeParticles != null) {
				smokeParticles.Play ();
		}
		if (shellParticles != null) {
			shellParticles.Play (owner);
		}
	}
	public void StartFlame(){
		flameParticles.Play();
	}
	public void StopFlame(){
		flameParticles.Stop();
	}
}
