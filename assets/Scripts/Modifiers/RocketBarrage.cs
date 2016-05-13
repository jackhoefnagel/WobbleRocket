using UnityEngine;
using System.Collections;

public class RocketBarrage : Modifier {

	public GameObject rocketPrefab;
	Vector3 direction;



	public override void Trigger ()
	{
		base.Trigger ();

		// spawn rockets for X sec
		StartCoroutine("RocketInterval");


	}

	public override void Reset ()
	{
		base.Reset ();
		StopCoroutine("RocketInterval");
	}

	IEnumerator RocketInterval(){

		while(true){

			Vector3 spawnPos = PickSpawnPos();
			Quaternion rot = Quaternion.LookRotation(direction);

			GameObject rocket = (GameObject)Instantiate(rocketPrefab, spawnPos, rot);
			rocket.rigidbody.velocity = rocket.transform.forward * 50;

			yield return new WaitForSeconds(Random.Range(0.3f, 1.3f));

		}
	}

	Vector3 PickSpawnPos(){
		
		Vector3 spawnPos = Vector3.zero;
		
		Vector3 camPos = Camera.main.transform.position;
		
		// get boundaries of what camera can see
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		
		// pick which side of the screen to spawn
		int randomN = UnityEngine.Random.Range(1, 5);
		
		// left of screen
		if(randomN == 1){
			spawnPos = new Vector3(camPos.x - horzExtent, UnityEngine.Random.Range(camPos.y - vertExtent, camPos.y + vertExtent),0);
			direction = Vector3.right;
			                    
		}
		
		// right of screen
		else if(randomN == 2){
			spawnPos = new Vector3(camPos.x + horzExtent, UnityEngine.Random.Range(camPos.y - vertExtent, camPos.y + vertExtent), 0);
			direction = Vector3.left;
		}
		
		// bottom of screen
		else if(randomN == 3){
			spawnPos = new Vector3(UnityEngine.Random.Range(camPos.x - horzExtent, camPos.x + horzExtent), camPos.y - vertExtent, 0);	
			direction = Vector3.up;
		}
		
		// top of screen
		else if(randomN == 4){
			spawnPos = new Vector3(UnityEngine.Random.Range(camPos.x - horzExtent, camPos.x + horzExtent), camPos.y + vertExtent, 0);
			direction = Vector3.down;
		}
		
		return spawnPos;
	}


}
