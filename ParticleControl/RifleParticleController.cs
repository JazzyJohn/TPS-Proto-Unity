using UnityEngine;
using System.Collections;

public class RifleParticleController : MonoBehaviour
{
	//Объявляем наши компоненты
	
	private ShellparticleSystem shellParticles;
	
	public GameObject simpleRay;
	
	protected Collider owner;
	//Инициализация, получаем ссылки на необходимые нам компоненты
	protected void Start()
	{
		
		shellParticles = transform.GetComponentInChildren<ShellparticleSystem> ();
		
	}

	public void SetOwner(Collider newOwner){
		owner = newOwner;
	}
	//Функция создания самого выстрела
    public void CreateShootFlame()
    {

        if (shellParticles != null)
        {
            shellParticles.Play(owner);
        }

    }
	public void CreateRay(Vector3 start, Vector3 direction){
			Instantiate(simpleRay, start, Quaternion.LookRotation(direction)); 
	}
	public void CreateLine(Vector3 start, Vector3 point){
			Instantiate(simpleRay, start, Quaternion.LookRotation((point -start).normalized));
			//TODO:: do size logic;
	}
}
