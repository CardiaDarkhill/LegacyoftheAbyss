using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3A RID: 3386
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class SetParticleEmissionTemp : FsmStateAction
	{
		// Token: 0x06006383 RID: 25475 RVA: 0x001F664B File Offset: 0x001F484B
		public override void Reset()
		{
			this.gameObject = null;
			this.emission = false;
			this.time = 1f;
		}

		// Token: 0x06006384 RID: 25476 RVA: 0x001F6670 File Offset: 0x001F4870
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (this.go != null)
				{
					this.go.GetComponent<ParticleSystem>().emission.enabled = this.emission.Value;
				}
			}
			this.timer = 0f;
			this.ended = false;
		}

		// Token: 0x06006385 RID: 25477 RVA: 0x001F66E0 File Offset: 0x001F48E0
		public override void OnUpdate()
		{
			if (this.timer < this.time.Value)
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer >= this.time.Value)
			{
				this.go.GetComponent<ParticleSystem>().emission.enabled = !this.emission.Value;
				this.ended = true;
				base.Finish();
			}
		}

		// Token: 0x06006386 RID: 25478 RVA: 0x001F6758 File Offset: 0x001F4958
		public override void OnExit()
		{
			if (this.go != null && !this.ended)
			{
				this.go.GetComponent<ParticleSystem>().emission.enabled = !this.emission.Value;
			}
		}

		// Token: 0x040061D9 RID: 25049
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061DA RID: 25050
		public FsmBool emission;

		// Token: 0x040061DB RID: 25051
		public FsmFloat time;

		// Token: 0x040061DC RID: 25052
		private float timer;

		// Token: 0x040061DD RID: 25053
		private GameObject go;

		// Token: 0x040061DE RID: 25054
		private bool ended;
	}
}
