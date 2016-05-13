using UnityEngine;
using System.Collections.Generic; 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("get Objects listed by an OverlapSphere cast")]
	public class GetObjectsInRange : ArrayListActions
	{
		
		public FsmOwnerDefault scanOrigin;
		
		public FsmFloat scanRange;
		
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;
		
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;
		
		public FsmEvent ErrorEvent;
		
		PlayMakerArrayListProxy colliders;
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault arrayListOwner;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same 'arrayListOwner' GameObject)")]
		public FsmString arrayListReference;
		
		
		public override void Reset()
		{
			arrayListOwner = null;
			arrayListReference = null;
			
			ErrorEvent = null;
			scanRange = 125;
			layerMask = new FsmInt[0];
			invertMask = false;
		}		
		
		
		public override void OnEnter()
		{
			
			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(arrayListOwner),arrayListReference.Value) )
				FindObjectInRange();
			
			Finish();
		}
		
		void FindObjectInRange()
		{
			if (! isProxyValid() )
			{
				return;
			}
			
			GameObject go = Fsm.GetOwnerDefaultTarget(scanOrigin);
			GameObject listOwner = Fsm.GetOwnerDefaultTarget(arrayListOwner);
			
			float range = scanRange.Value;		
			
			Collider [] colliders = Physics.OverlapSphere(go.transform.position, range, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			
			if (colliders.Length == 0) {
				Fsm.Event(ErrorEvent);
			}
			
			
			foreach (Collider col in colliders)
			{
				//Debug.Log(col.transform.position);
				
				//FsmGameObject referenceObject = col.gameObject;
				//listOwner.transform.GetComponent(PlayMakerArrayListProxy).Add(referenceObject, "gameObject");
				proxy.Add(col.gameObject,"gameObject");
			}
			
			
			
			//Debug.Log(colliders);		
			
			Finish();
		}
	}
}