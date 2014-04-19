using UnityEngine;
using System.Collections;

public class RifleParticleController : MonoBehaviour
{
	//Объявляем наши компоненты
	private ParticleSystem flameParticles;
	private ShellparticleSystem shellParticles;
	private ParticleSystem smokeParticles;

	//Инициализация, получаем ссылки на необходимые нам компоненты
	void Start()
	{
		flameParticles = transform.GetChild (0).GetComponent<ParticleSystem> ();
		shellParticles = transform.GetChild (1).GetComponent<ShellparticleSystem> ();
		smokeParticles = transform.GetChild (2).GetComponent<ParticleSystem> ();
	}

	//Функция создания самого выстрела
	public void CreateShootFlame()
	{
		//Шанс получения пламени при выстреле
		if (Random.Range (0, 2) == 1)
		{
			flameParticles.Play();
		}

		smokeParticles.Play ();
		shellParticles.Play ();
	}
}
