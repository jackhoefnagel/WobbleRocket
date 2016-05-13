using UnityEngine;
using System.Collections;

public class ZeroGravity : Modifier {

	 Vector3 defGravity;

	public override void Trigger ()
	{
		base.Trigger ();
		defGravity = Physics.gravity;
		Debug.Log(defGravity);

		Physics.gravity = Vector3.zero;
	}
	
	public override void Reset ()
	{
		base.Reset ();
		Physics.gravity = defGravity;
	}


}
