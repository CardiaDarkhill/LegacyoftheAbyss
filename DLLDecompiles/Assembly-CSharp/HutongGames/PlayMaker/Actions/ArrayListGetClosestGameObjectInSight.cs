using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B53 RID: 2899
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the closest GameObject within an arrayList from a transform or position which does not have a collider between itself and another GameObject")]
	public class ArrayListGetClosestGameObjectInSight : ArrayListActions
	{
		// Token: 0x06005A42 RID: 23106 RVA: 0x001C8950 File Offset: 0x001C6B50
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.distanceFrom = null;
			this.orDistanceFromVector3 = null;
			this.closestGameObject = null;
			this.closestIndex = null;
			this.everyframe = true;
			this.fromGameObject = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
		}

		// Token: 0x06005A43 RID: 23107 RVA: 0x001C89AD File Offset: 0x001C6BAD
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoFindClosestGo();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A44 RID: 23108 RVA: 0x001C89ED File Offset: 0x001C6BED
		public override void OnUpdate()
		{
			this.DoFindClosestGo();
		}

		// Token: 0x06005A45 RID: 23109 RVA: 0x001C89F8 File Offset: 0x001C6BF8
		private void DoFindClosestGo()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			Vector3 vector = this.orDistanceFromVector3.Value;
			GameObject value = this.distanceFrom.Value;
			if (value != null)
			{
				vector += value.transform.position;
			}
			float num = float.PositiveInfinity;
			int num2 = 0;
			foreach (object obj in this.proxy.arrayList)
			{
				GameObject gameObject = (GameObject)obj;
				if (gameObject != null && this.DoLineCast(gameObject))
				{
					float sqrMagnitude = (gameObject.transform.position - vector).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						num = sqrMagnitude;
						this.closestGameObject.Value = gameObject;
						this.closestIndex.Value = num2;
					}
				}
				num2++;
			}
		}

		// Token: 0x06005A46 RID: 23110 RVA: 0x001C8AF4 File Offset: 0x001C6CF4
		private bool DoLineCast(GameObject toGameObject)
		{
			Vector3 position = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject).transform.position;
			Vector3 position2 = toGameObject.transform.position;
			RaycastHit raycastHitInfo;
			bool result = !Physics.Linecast(position, position2, out raycastHitInfo, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			base.Fsm.RaycastHitInfo = raycastHitInfo;
			return result;
		}

		// Token: 0x040055D5 RID: 21973
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055D6 RID: 21974
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055D7 RID: 21975
		[Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
		public FsmGameObject distanceFrom;

		// Token: 0x040055D8 RID: 21976
		[Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
		public FsmVector3 orDistanceFromVector3;

		// Token: 0x040055D9 RID: 21977
		public bool everyframe;

		// Token: 0x040055DA RID: 21978
		[ActionSection("Raycast Settings")]
		[Tooltip("The line start of the sweep.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x040055DB RID: 21979
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x040055DC RID: 21980
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x040055DD RID: 21981
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject closestGameObject;

		// Token: 0x040055DE RID: 21982
		[UIHint(UIHint.Variable)]
		public FsmInt closestIndex;
	}
}
