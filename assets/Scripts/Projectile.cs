using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public LayerMask mask;
	public float collisionRadius = .3f;
	public float aliveTime = .5f;

	public GameObject explosionPrefab;

	void Start(){
		Invoke("DoImpact", aliveTime);
	}


	void Update () {
		CheckForCollision();
	}

	void CheckForCollision(){
		
		Collider[] hits = Physics.OverlapSphere (transform.position, collisionRadius, mask);
		foreach (Collider col in hits) {

			if(col.rigidbody !=null )
				col.rigidbody.AddExplosionForce(100, transform.position, 20);

			DoImpact();
		}
		
	}
	
	void DoImpact(){
		
		// spawn explosion
		GameObject insExpl = (GameObject)Instantiate (explosionPrefab, transform.position, Quaternion.identity);
		Destroy (insExpl, .1f);
		
		Destroy(this.gameObject);
		
	}

}
