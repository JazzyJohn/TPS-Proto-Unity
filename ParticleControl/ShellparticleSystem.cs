using UnityEngine;
using System.Collections;

public class ShellparticleSystem : MonoBehaviour
{
	//Закидываем сюда префаб настроенной гильзы:
	//гильза должна имень rigidbody и любой коллайдер
	public GameObject shellPrefab;

	//Функция создания гильзы
	public void Play()
	{
		//Создаем гильзу
		GameObject shellObject = Instantiate (shellPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 90))) as GameObject;

		//Устанавливаем текущую угловую скорость, скорость движения
		//transform.TransformDirection используется для конвертации координат
		//ибо rigidbody.velocity поумолчанию работает только с глобальным
		shellObject.rigidbody.velocity = transform.TransformDirection(new Vector3(Random.Range(0f, 0.1f), Random.Range(0.2f, 1f), Random.Range(-1.5f, -0.5f)));
		shellObject.rigidbody.angularVelocity = - transform.TransformDirection(new Vector3(Random.Range(10f, 10f), Random.Range(10f, 10f), Random.Range(10f, 10f)));
	}
}