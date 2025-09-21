using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B52 RID: 2898
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the closest GameObject within an arrayList from a transform or position.")]
	public class ArrayListGetClosestGameObject : ArrayListActions
	{
		// Token: 0x06005A3D RID: 23101 RVA: 0x001C87DD File Offset: 0x001C69DD
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.distanceFrom = null;
			this.orDistanceFromVector3 = null;
			this.closestGameObject = null;
			this.closestIndex = null;
			this.everyframe = true;
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x001C8810 File Offset: 0x001C6A10
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

		// Token: 0x06005A3F RID: 23103 RVA: 0x001C8850 File Offset: 0x001C6A50
		public override void OnUpdate()
		{
			this.DoFindClosestGo();
		}

		// Token: 0x06005A40 RID: 23104 RVA: 0x001C8858 File Offset: 0x001C6A58
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
				if (gameObject != null)
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

		// Token: 0x040055CE RID: 21966
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055CF RID: 21967
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055D0 RID: 21968
		[Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
		public FsmGameObject distanceFrom;

		// Token: 0x040055D1 RID: 21969
		[Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
		public FsmVector3 orDistanceFromVector3;

		// Token: 0x040055D2 RID: 21970
		public bool everyframe;

		// Token: 0x040055D3 RID: 21971
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject closestGameObject;

		// Token: 0x040055D4 RID: 21972
		[UIHint(UIHint.Variable)]
		public FsmInt closestIndex;
	}
}
