using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E82 RID: 3714
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Draws a line in the Scene View from a Start point in a direction. Specify the start point as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.\n\nNotes:\n- Enable/disable Gizmos in the Game View toolbar.\n- Set how long debug lines are visible for in Preferences > Debugging.")]
	public class DrawDebugRay : FsmStateAction
	{
		// Token: 0x060069A6 RID: 27046 RVA: 0x002111FA File Offset: 0x0020F3FA
		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x00211204 File Offset: 0x0020F404
		public override void Reset()
		{
			this.fromObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.direction = new FsmVector3
			{
				UseVariable = true
			};
			this.color = Color.white;
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x00211257 File Offset: 0x0020F457
		public override void OnUpdate()
		{
			Debug.DrawRay(ActionHelpers.GetPosition(this.fromObject, this.fromPosition), this.direction.Value);
		}

		// Token: 0x060069A9 RID: 27049 RVA: 0x0021127A File Offset: 0x0020F47A
		public override void OnExit()
		{
			Debug.DrawRay(ActionHelpers.GetPosition(this.fromObject, this.fromPosition), this.direction.Value, this.color.Value, PlayMakerPrefs.DebugLinesDuration);
		}

		// Token: 0x040068D5 RID: 26837
		[Tooltip("Draw ray from a GameObject.")]
		public FsmGameObject fromObject;

		// Token: 0x040068D6 RID: 26838
		[Tooltip("Draw ray from a world position, or local offset from GameObject if provided.")]
		public FsmVector3 fromPosition;

		// Token: 0x040068D7 RID: 26839
		[Tooltip("Direction vector of ray in world space.")]
		public FsmVector3 direction;

		// Token: 0x040068D8 RID: 26840
		[Tooltip("The color of the ray.")]
		public FsmColor color;
	}
}
