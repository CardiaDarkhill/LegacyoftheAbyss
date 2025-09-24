using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E81 RID: 3713
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Draws a line from a Start point to an End point. Specify the points as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.\n\nNotes:\n- Enable/disable Gizmos in the Game View toolbar.\n- Set how long debug lines are visible for in Preferences > Debugging.")]
	public class DrawDebugLine : FsmStateAction
	{
		// Token: 0x060069A1 RID: 27041 RVA: 0x002110F5 File Offset: 0x0020F2F5
		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		// Token: 0x060069A2 RID: 27042 RVA: 0x00211100 File Offset: 0x0020F300
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
			this.toObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.toPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.color = Color.white;
		}

		// Token: 0x060069A3 RID: 27043 RVA: 0x00211168 File Offset: 0x0020F368
		public override void OnUpdate()
		{
			Vector3 position = ActionHelpers.GetPosition(this.fromObject, this.fromPosition);
			Vector3 position2 = ActionHelpers.GetPosition(this.toObject, this.toPosition);
			Debug.DrawLine(position, position2, this.color.Value);
		}

		// Token: 0x060069A4 RID: 27044 RVA: 0x002111AC File Offset: 0x0020F3AC
		public override void OnExit()
		{
			Vector3 position = ActionHelpers.GetPosition(this.fromObject, this.fromPosition);
			Vector3 position2 = ActionHelpers.GetPosition(this.toObject, this.toPosition);
			Debug.DrawLine(position, position2, this.color.Value, PlayMakerPrefs.DebugLinesDuration);
		}

		// Token: 0x040068D0 RID: 26832
		[Tooltip("Draw line from a GameObject.")]
		public FsmGameObject fromObject;

		// Token: 0x040068D1 RID: 26833
		[Tooltip("Draw line from a world position, or local offset from GameObject if provided.")]
		public FsmVector3 fromPosition;

		// Token: 0x040068D2 RID: 26834
		[Tooltip("Draw line to a GameObject.")]
		public FsmGameObject toObject;

		// Token: 0x040068D3 RID: 26835
		[Tooltip("Draw line to a world position, or local offset from GameObject if provided.")]
		public FsmVector3 toPosition;

		// Token: 0x040068D4 RID: 26836
		[Tooltip("The color of the line.")]
		public FsmColor color;
	}
}
