using UnityEngine;
using System.Collections;

public class UpSideDown : Modifier {

	public override void Trigger ()
	{
		base.Trigger ();
		Physics.gravity *= -1;
	}

	public override void Reset ()
	{
		base.Reset ();
		Physics.gravity *= -1;
	}
}
