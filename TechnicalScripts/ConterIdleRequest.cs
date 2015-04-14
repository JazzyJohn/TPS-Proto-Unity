using UnityEngine;
using System.Collections;

public class ConterIdleRequest : MonoBehaviour {

	public float time=0.5f;
    public float _timer = 0.0f;
	
	// Update is called once per frame
	void Update () {
        _timer += Time.deltaTime;
        if (_timer > time)
        {
            _timer = 0.0f;
           
                NetworkController.Instance.ConterIdleRequestSend();
           
        }

	}
	
	public void Recived(){
		#if UNITY_EDITOR
			Debug.Log("ping");
		#endif
	}
}
