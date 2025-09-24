using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B54 RID: 2900
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the farthest GameObject within an arrayList from a transform or position.")]
	public class ArrayListGetFarthestGameObject : ArrayListActions
	{
		// Token: 0x06005A48 RID: 23112 RVA: 0x001C8B5D File Offset: 0x001C6D5D
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.distanceFrom = null;
			this.orDistanceFromVector3 = null;
			this.farthestGameObject = null;
			this.farthestIndex = null;
			this.everyframe = true;
		}

		// Token: 0x06005A49 RID: 23113 RVA: 0x001C8B90 File Offset: 0x001C6D90
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

		// Token: 0x06005A4A RID: 23114 RVA: 0x001C8BD0 File Offset: 0x001C6DD0
		public override void OnUpdate()
		{
			this.DoFindFarthestGo();
		}

		// Token: 0x06005A4B RID: 23115 RVA: 0x001C8BD8 File Offset: 0x001C6DD8
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
				if (gameObject != null)
				{
					float sqrMagnitude = (gameObject.transform.position - vector).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						num = sqrMagnitude;
						this.farthestGameObject.Value = gameObject;
						this.farthestIndex.Value = num2;
					}
				}
				num2++;
			}
		}

		// Token: 0x040055DF RID: 21983
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055E0 RID: 21984
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055E1 RID: 21985
		[Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
		public FsmGameObject distanceFrom;

		// Token: 0x040055E2 RID: 21986
		[Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
		public FsmVector3 orDistanceFromVector3;

		// Token: 0x040055E3 RID: 21987
		public bool everyframe;

		// Token: 0x040055E4 RID: 21988
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject farthestGameObject;

		// Token: 0x040055E5 RID: 21989
		[UIHint(UIHint.Variable)]
		public FsmInt farthestIndex;
	}
}
