using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA3 RID: 4003
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets info on the last RaycastAll and store in array variables.")]
	public class GetRaycastAllInfo : FsmStateAction
	{
		// Token: 0x06006E9D RID: 28317 RVA: 0x00223CE9 File Offset: 0x00221EE9
		public override void Reset()
		{
			this.storeHitObjects = null;
			this.points = null;
			this.normals = null;
			this.distances = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E9E RID: 28318 RVA: 0x00223D10 File Offset: 0x00221F10
		private void StoreRaycastAllInfo()
		{
			if (RaycastAll.RaycastAllHitInfo == null)
			{
				return;
			}
			this.storeHitObjects.Resize(RaycastAll.RaycastAllHitInfo.Length);
			this.points.Resize(RaycastAll.RaycastAllHitInfo.Length);
			this.normals.Resize(RaycastAll.RaycastAllHitInfo.Length);
			this.distances.Resize(RaycastAll.RaycastAllHitInfo.Length);
			for (int i = 0; i < RaycastAll.RaycastAllHitInfo.Length; i++)
			{
				this.storeHitObjects.Values[i] = RaycastAll.RaycastAllHitInfo[i].collider.gameObject;
				this.points.Values[i] = RaycastAll.RaycastAllHitInfo[i].point;
				this.normals.Values[i] = RaycastAll.RaycastAllHitInfo[i].normal;
				this.distances.Values[i] = RaycastAll.RaycastAllHitInfo[i].distance;
			}
		}

		// Token: 0x06006E9F RID: 28319 RVA: 0x00223E0D File Offset: 0x0022200D
		public override void OnEnter()
		{
			this.StoreRaycastAllInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EA0 RID: 28320 RVA: 0x00223E23 File Offset: 0x00222023
		public override void OnUpdate()
		{
			this.StoreRaycastAllInfo();
		}

		// Token: 0x04006E3C RID: 28220
		[Tooltip("Store the GameObjects hit in an array variable.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray storeHitObjects;

		// Token: 0x04006E3D RID: 28221
		[Tooltip("Get the world position of all ray hit point and store them in an array variable.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Vector3, "", 0, 0, 65536)]
		public FsmArray points;

		// Token: 0x04006E3E RID: 28222
		[Tooltip("Get the normal at all hit points and store them in an array variable.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Vector3, "", 0, 0, 65536)]
		public FsmArray normals;

		// Token: 0x04006E3F RID: 28223
		[Tooltip("Get the distance along the ray to all hit points and store them in an array variable.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		public FsmArray distances;

		// Token: 0x04006E40 RID: 28224
		[Tooltip("Repeat every frame. Warning, this could be affecting performances")]
		public bool everyFrame;
	}
}
