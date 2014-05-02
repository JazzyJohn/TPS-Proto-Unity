using UnityEngine;
using System.Collections;

public class ShellparticleSystem : MonoBehaviour
{
	//Закидываем сюда префаб настроенной гильзы:
	//гильза должна имень rigidbody и любой коллайдер
	public GameObject shellPrefab;

	public float sheelSpeed=1.0f;

	//Функция создания гильзы
	public void Play(Collider owner)
	{
		//Создаем гильзу
		GameObject shellObject = Instantiate (shellPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 90))) as GameObject;

		//Устанавливаем текущую угловую скорость, скорость движения
		//transform.TransformDirection используется для конвертации координат
		//ибо rigidbody.velocity поумолчанию работает только с глобальным
		//Physics.IgnoreCollision(shellObject.collider, owner);
		shellObject.rigidbody.velocity = sheelSpeed*transform.TransformDirection(new Vector3(Random.Range(0f, 0.1f), Random.Range(0.2f, 1f), Random.Range(-1.5f, -0.5f)));
		shellObject.rigidbody.angularVelocity = - transform.TransformDirection(new Vector3(Random.Range(10f, 10f), Random.Range(10f, 10f), Random.Range(10f, 10f)));
	}
}