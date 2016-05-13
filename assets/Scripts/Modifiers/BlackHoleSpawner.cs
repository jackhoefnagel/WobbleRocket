using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHoleSpawner : Modifier {


	public int levelWidth;
	public int levelHeight;

	public int amountToSpawn = 3;
	public GameObject blackholePrefab;

	public List<GameObject> blackHoles =  new List<GameObject>() ;

	public override void Reset ()
	{
		base.Reset ();
		if(PhotonNetwork.isMasterClient){
			foreach (GameObject go in blackHoles) {
				PhotonNetwork.Destroy(go);	
			}
		}

	}
	public override void Trigger ()
	{
		base.Trigger ();
		if(PhotonNetwork.isMasterClient)
			StartCoroutine(SpawnBlackHoles());

	}

	IEnumerator SpawnBlackHoles(){


		int blackHolesSpawned = 0;
		while(blackHolesSpawned < amountToSpawn){

			Vector3 spawnPos =  new Vector3(Random.Range(-levelWidth / 2, levelWidth/2), Random.Range(-levelHeight / 2, levelHeight/2), 0);
			GameObject insBlackHole = (GameObject)PhotonNetwork.Instantiate(blackholePrefab.name, spawnPos, Quaternion.identity,0);
			blackHoles.Add (insBlackHole);

			blackHolesSpawned++;
			yield return new WaitForSeconds(Random.Range(1,4));
		}


	}

}
