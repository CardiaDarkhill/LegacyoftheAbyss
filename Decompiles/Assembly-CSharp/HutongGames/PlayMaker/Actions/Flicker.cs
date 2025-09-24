using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E95 RID: 3733
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Randomly flickers a Game Object on/off.")]
	public class Flicker : ComponentAction<Renderer>
	{
		// Token: 0x060069F6 RID: 27126 RVA: 0x00212360 File Offset: 0x00210560
		public override void Reset()
		{
			this.gameObject = null;
			this.frequency = 0.1f;
			this.amountOn = 0.5f;
			this.rendererOnly = true;
			this.realTime = false;
		}

		// Token: 0x060069F7 RID: 27127 RVA: 0x00212397 File Offset: 0x00210597
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x060069F8 RID: 27128 RVA: 0x002123B0 File Offset: 0x002105B0
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer > this.frequency.Value)
			{
				bool flag = Random.Range(0f, 1f) < this.amountOn.Value;
				if (this.rendererOnly)
				{
					if (base.UpdateCache(ownerDefaultTarget))
					{
						base.renderer.enabled = flag;
					}
				}
				else
				{
					ownerDefaultTarget.SetActive(flag);
				}
				this.startTime = this.timer;
				this.timer = 0f;
			}
		}

		// Token: 0x0400694D RID: 26957
		[RequiredField]
		[Tooltip("The GameObject to flicker.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400694E RID: 26958
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The frequency of the flicker in seconds.")]
		public FsmFloat frequency;

		// Token: 0x0400694F RID: 26959
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Amount of time flicker is On (0-1). E.g. Use 0.95 for an occasional flicker.")]
		public FsmFloat amountOn;

		// Token: 0x04006950 RID: 26960
		[Tooltip("Only effect the renderer, leaving other components active.")]
		public bool rendererOnly;

		// Token: 0x04006951 RID: 26961
		[Tooltip("Ignore time scale. Useful if flickering UI when the game is paused.")]
		public bool realTime;

		// Token: 0x04006952 RID: 26962
		private float startTime;

		// Token: 0x04006953 RID: 26963
		private float timer;
	}
}
