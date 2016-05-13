using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
	
	public int size = 10;
	public float strength = 50;
	
	void Update () {


			// only do this for the local player
			Collider[] hits = Physics.OverlapSphere(transform.position, size);
			
			foreach (Collider col in hits) {
				
				if(col.rigidbody != null && col.GetComponent<PhotonView>() != null && col.GetComponent<PhotonView>().isMine){
					Vector3 dir = transform.position - col.transform.position;
					float dist = Vector3.Distance(transform.position, col.transform.position);
					
					float speed = strength * Mathf.Pow(dist, 2);//Mathf.Pow(dist, 2);
					//float speedY = Physics.gravity.y / Mathf.Pow(dist, 2);
					
				col.rigidbody.AddForce(dir.normalized * strength );
					
				}
			}

	}
	
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position, size);
		
	}
}
