using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D61 RID: 3425
	[ActionCategory("Hollow Knight")]
	[Tooltip("Randomly shakes a GameObject's position by a diminishing amount over time.")]
	public class ShakePosition : FsmStateAction
	{
		// Token: 0x06006425 RID: 25637 RVA: 0x001F8ADF File Offset: 0x001F6CDF
		public override void Reset()
		{
			base.Reset();
			this.gameObject = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.UseOwner
			};
			this.duration = 1f;
			this.isLooping = false;
			this.stopEvent = null;
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x001F8B1C File Offset: 0x001F6D1C
		public override void OnEnter()
		{
			base.OnEnter();
			this.timer = 0f;
			GameObject safe = this.gameObject.GetSafe(this);
			if (safe != null)
			{
				this.target = safe.transform;
				this.startingWorldPosition = this.target.position;
			}
			else
			{
				this.target = null;
			}
			this.UpdateShaking();
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x001F8B7C File Offset: 0x001F6D7C
		public override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateShaking();
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x001F8B8A File Offset: 0x001F6D8A
		public override void OnExit()
		{
			this.StopAndReset();
			base.OnExit();
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x001F8B98 File Offset: 0x001F6D98
		private void UpdateShaking()
		{
			if (this.target != null)
			{
				bool value = this.isLooping.Value;
				float num = Mathf.Clamp01(1f - this.timer / this.duration.Value);
				Vector3 a = Vector3.Scale(this.extents.Value, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
				this.target.position = this.startingWorldPosition + a * (value ? 1f : num);
				this.timer += Time.deltaTime;
				if (!value && this.timer > this.duration.Value)
				{
					this.StopAndReset();
					base.Fsm.Event(this.stopEvent);
					base.Finish();
					return;
				}
			}
			else
			{
				this.StopAndReset();
				base.Fsm.Event(this.stopEvent);
				base.Finish();
			}
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x001F8CAD File Offset: 0x001F6EAD
		private void StopAndReset()
		{
			if (this.target != null)
			{
				this.target.position = this.startingWorldPosition;
				this.target = null;
			}
		}

		// Token: 0x0400628F RID: 25231
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006290 RID: 25232
		[RequiredField]
		public FsmVector3 extents;

		// Token: 0x04006291 RID: 25233
		public FsmFloat duration;

		// Token: 0x04006292 RID: 25234
		public FsmBool isLooping;

		// Token: 0x04006293 RID: 25235
		public FsmEvent stopEvent;

		// Token: 0x04006294 RID: 25236
		private float timer;

		// Token: 0x04006295 RID: 25237
		private Transform target;

		// Token: 0x04006296 RID: 25238
		private Vector3 startingWorldPosition;
	}
}
