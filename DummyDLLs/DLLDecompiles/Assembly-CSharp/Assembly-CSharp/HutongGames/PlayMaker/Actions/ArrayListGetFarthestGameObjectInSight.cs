using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B55 RID: 2901
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the farthest GameObject within an arrayList from a transform or position which does not have a collider between itself and another GameObject")]
	public class ArrayListGetFarthestGameObjectInSight : ArrayListActions
	{
		// Token: 0x06005A4D RID: 23117 RVA: 0x001C8CD0 File Offset: 0x001C6ED0
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.distanceFrom = null;
			this.orDistanceFromVector3 = null;
			this.farthestGameObject = null;
			this.farthestIndex = null;
			this.everyframe = true;
			this.fromGameObject = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
		}

		// Token: 0x06005A4E RID: 23118 RVA: 0x001C8D2D File Offset: 0x001C6F2D
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoFindFarthestGo();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A4F RID: 23119 RVA: 0x001C8D6D File Offset: 0x001C6F6D
		public override void OnUpdate()
		{
			this.DoFindFarthestGo();
		}

		// Token: 0x06005A50 RID: 23120 RVA: 0x001C8D78 File Offset: 0x001C6F78
		private void DoFindFarthestGo()
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
					if (sqrMagnitude >= num)
					{
						num = sqrMagnitude;
						this.farthestGameObject.Value = gameObject;
						this.farthestIndex.Value = num2;
					}
				}
				num2++;
			}
		}

		// Token: 0x06005A51 RID: 23121 RVA: 0x001C8E74 File Offset: 0x001C7074
		private bool DoLineCast(GameObject toGameObject)
		{
			Vector3 position = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject).transform.position;
			Vector3 position2 = toGameObject.transform.position;
			RaycastHit raycastHitInfo;
			bool result = !Physics.Linecast(position, position2, out raycastHitInfo, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			base.Fsm.RaycastHitInfo = raycastHitInfo;
			return result;
		}

		// Token: 0x040055E6 RID: 21990
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055E7 RID: 21991
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055E8 RID: 21992
		[Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
		public FsmGameObject distanceFrom;

		// Token: 0x040055E9 RID: 21993
		[Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
		public FsmVector3 orDistanceFromVector3;

		// Token: 0x040055EA RID: 21994
		public bool everyframe;

		// Token: 0x040055EB RID: 21995
		[ActionSection("Raycast Settings")]
		[Tooltip("The line start of the sweep.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x040055EC RID: 21996
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x040055ED RID: 21997
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x040055EE RID: 21998
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject farthestGameObject;

		// Token: 0x040055EF RID: 21999
		[UIHint(UIHint.Variable)]
		public FsmInt farthestIndex;
	}
}
