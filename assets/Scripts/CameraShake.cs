using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public static CameraShake instance;

	private Vector3 originPosition;
	private Quaternion originRotation;
	float shake_decay = 0.01f;
	public float shake_intensity;

	void Awake(){
		instance = this;
	}

	void Start(){
		originPosition = transform.position;
		originRotation = transform.rotation;
	}


	void Update (){
		if (shake_intensity > 0){
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			//Debug.Log ("shake");
			/*transform.rotation = new Quaternion(
				originRotation.x + Random.Range (-shake_intensity,shake_intensity) * .2f,
				originRotation.y + Random.Range (-shake_intensity,shake_intensity) * .2f,
				originRotation.z + Random.Range (-shake_intensity,shake_intensity) * .2f,
				originRotation.w + Random.Range (-shake_intensity,shake_intensity) * .2f);*/
			shake_intensity -= shake_decay;
		}
		else {
			transform.position = originPosition;
			//transform.rotation = originRotation;
		}
	}

	
	public void Shake(){
		originPosition = transform.position;
		originRotation = transform.rotation;
		shake_intensity = 5f;
		shake_decay = 0.3f;
	}

	public void BigShake(){
		originPosition = transform.position;
		originRotation = transform.rotation;
		shake_intensity = 5f;
		shake_decay = 0.3f;
		
	}
}