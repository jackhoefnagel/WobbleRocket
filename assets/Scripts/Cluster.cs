using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : Photon.MonoBehaviour {

	public int clusterBombsAmount = 3;
	public GameObject clusterPrefab;

	public List<object> velocities = new List<object>();
	object[] velocitiesLocal;

	Vector3 posToSpawn;

	public void SpawnCluster(Vector3 pos){
		//if(PhotonNetwork.isMasterClient){

		posToSpawn = pos;


		for (int i = 0; i < clusterBombsAmount; i++) {
			velocities.Add (new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), 0).normalized * Random.Range(15f,30f));
		}

		object[] velocitiesObj = velocities.ToArray ();

		photonView.RPC("SendClusterRPCs", PhotonTargets.All, velocitiesObj);


	//	}
	}

	IEnumerator SpawnClusterCoroutine(){

		for (int i = 0; i < clusterBombsAmount; i++) {
		Debug.Log("spawning clusterbomb");
			GameObject insCluster = (GameObject)Instantiate(clusterPrefab, posToSpawn, Quaternion.identity);
			insCluster.rigidbody.velocity = (Vector3)velocitiesLocal[i];;

			yield return new WaitForSeconds(.01f);

		}
	}

	[RPC]
	void SendClusterRPCs(object[] destinations){

		velocitiesLocal = destinations;

		StartCoroutine(SpawnClusterCoroutine());
	}

}
