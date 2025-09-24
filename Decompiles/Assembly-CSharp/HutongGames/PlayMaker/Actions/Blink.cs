using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E94 RID: 3732
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Turns a Game Object on/off in a regular repeating pattern.")]
	public class Blink : ComponentAction<Renderer>
	{
		// Token: 0x060069F1 RID: 27121 RVA: 0x002121E0 File Offset: 0x002103E0
		public override void Reset()
		{
			this.gameObject = null;
			this.timeOff = 0.5f;
			this.timeOn = 0.5f;
			this.rendererOnly = true;
			this.startOn = false;
			this.realTime = false;
		}

		// Token: 0x060069F2 RID: 27122 RVA: 0x0021222E File Offset: 0x0021042E
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
			this.UpdateBlinkState(this.startOn.Value);
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x00212258 File Offset: 0x00210458
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.blinkOn && this.timer > this.timeOn.Value)
			{
				this.UpdateBlinkState(false);
			}
			if (!this.blinkOn && this.timer > this.timeOff.Value)
			{
				this.UpdateBlinkState(true);
			}
		}

		// Token: 0x060069F4 RID: 27124 RVA: 0x002122D8 File Offset: 0x002104D8
		private void UpdateBlinkState(bool state)
		{
			GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (gameObject == null)
			{
				return;
			}
			if (this.rendererOnly)
			{
				if (base.UpdateCache(gameObject))
				{
					base.renderer.enabled = state;
				}
			}
			else
			{
				gameObject.SetActive(state);
			}
			this.blinkOn = state;
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x04006944 RID: 26948
		[RequiredField]
		[Tooltip("The Game Object to blink on/off.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006945 RID: 26949
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to stay off in seconds.")]
		public FsmFloat timeOff;

		// Token: 0x04006946 RID: 26950
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to stay on in seconds.")]
		public FsmFloat timeOn;

		// Token: 0x04006947 RID: 26951
		[Tooltip("Start in the on state.")]
		public FsmBool startOn;

		// Token: 0x04006948 RID: 26952
		[Tooltip("Only effect the renderer, keeping other components active.")]
		public bool rendererOnly;

		// Token: 0x04006949 RID: 26953
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x0400694A RID: 26954
		private float startTime;

		// Token: 0x0400694B RID: 26955
		private float timer;

		// Token: 0x0400694C RID: 26956
		private bool blinkOn;
	}
}
