using UnityEngine;
using System.Collections;

public class ShellparticleSystem : MonoBehaviour
{
	//Закидываем сюда префаб настроенной гильзы:
	//гильза должна имень rigidbody и любой коллайдер
	public GameObject shellPrefab;

	static public float sheelSpeed=5.0f;

	//Функция создания гильзы
	public void Play(Collider owner)
	{
        
		//Создаем гильзу
        if (shellPrefab.CountPooled() == 0) {
            if (shellPrefab.CountSpawned() == 0)
            {
                shellPrefab.CreatePool(30);
            }
            else
            {
                return;
            }
        }
		GameObject shellObject =shellPrefab.Spawn(transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));

		//Устанавливаем текущую угловую скорость, скорость движения
		//transform.TransformDirection используется для конвертации координат
		//ибо rigidbody.velocity поумолчанию работает только с глобальным
		//Physics.IgnoreCollision(shellObject.collider, owner);
		shellObject.rigidbody.velocity = sheelSpeed*transform.TransformDirection(new Vector3(Random.Range(0f, 0.1f), Random.Range(0.2f, 1f), Random.Range(0.5f, 1.5f)));
		shellObject.rigidbody.angularVelocity = - transform.TransformDirection(new Vector3(Random.Range(10f, 10f), Random.Range(10f, 10f), Random.Range(10f, 10f)));
	}
}