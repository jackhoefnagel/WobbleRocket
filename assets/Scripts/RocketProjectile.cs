using UnityEngine;
using System.Collections;

public class RocketProjectile : Photon.MonoBehaviour {


	public LayerMask mask;
	public float collisionRadius = 1f;
	public float aliveTime = .5f;
	
	public GameObject explosionPrefab;
	bool checkCollision = false;

	public AudioClip explosionSound;
	public AudioClip fireSound;

	 Vector3 impactPos;

	void Start(){
		Invoke("DoImpact", aliveTime);
		Invoke("SetCollision", .01f);

		GameManager.instance.PlaySound(fireSound);

	}

	void SetCollision(){
		checkCollision = true;
	}

	void Update(){


		if(checkCollision && photonView.isMine)
			CheckForCollision();
	}

	void CheckForCollision(){
		
		Collider[] hits = Physics.OverlapSphere (transform.position, collisionRadius, mask);
		foreach (Collider col in hits) {

			if(col.tag == "PlayerHitbox"){
				photonView.RPC ("HitPlayer", PhotonTargets.All, col.transform.parent.gameObject.GetPhotonView().viewID, col.transform.position);
				break;

			}

		}
		
	}


	[RPC]
	void HitPlayer(int hitID, Vector3 pos){

		impactPos = pos;

		GameObject hit = PhotonView.Find(hitID).gameObject;

		hit.rigidbody.AddExplosionForce(100, impactPos, 20, 0, ForceMode.Impulse);	
		hit.transform.Find("Playa_under").GetComponent<PlayMakerFSM>().SendEvent("PlayerHitByExplosion");
	
		if(PhotonNetwork.isMasterClient){
			if(hit.name == "Player1"){
				GameManager.instance.WithdrawLifeLocal(1);
			}
			else {
				GameManager.instance.WithdrawLifeLocal(2);
			}
		}

		DoImpact();

	}

	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position, collisionRadius);
	}
	
	void DoImpact(){

		GameManager.instance.PlaySound(explosionSound);
		CameraShake.instance.BigShake();

		if(impactPos == null || impactPos == Vector3.zero){
			impactPos = transform.position;
		}

		if(ClusterModifier.clusterBombsActive){
			GameManager.instance.SpawnCluster(impactPos);
		}

	
		// spawn explosion
		GameObject insExpl = (GameObject)Instantiate (explosionPrefab, impactPos, Quaternion.identity);
		Destroy (insExpl, .1f);

		if(photonView.isMine)
			PhotonNetwork.Destroy(this.gameObject);
		
	}



}
