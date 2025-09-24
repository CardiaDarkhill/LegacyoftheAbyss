using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E0 RID: 4832
	public class CheckHeroPerformanceRegion : CheckHeroPerformanceRegionBase
	{
		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x06007E02 RID: 32258 RVA: 0x00257ED8 File Offset: 0x002560D8
		protected override bool IsActive
		{
			get
			{
				return this.ActiveBool.Value || !this.useActiveBool;
			}
		}

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x06007E03 RID: 32259 RVA: 0x00257EF2 File Offset: 0x002560F2
		protected override Transform TargetTransform
		{
			get
			{
				return this.transform;
			}
		}

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x06007E04 RID: 32260 RVA: 0x00257EFC File Offset: 0x002560FC
		protected override float NewDelay
		{
			get
			{
				return new MinMaxFloat(this.MinReactDelay.Value, this.MaxReactDelay.Value).GetRandomValue();
			}
		}

		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x06007E05 RID: 32261 RVA: 0x00257F2C File Offset: 0x0025612C
		protected override bool UseNeedolinRange
		{
			get
			{
				return !this.IgnoreNeedolinRange.Value;
			}
		}

		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x06007E06 RID: 32262 RVA: 0x00257F3C File Offset: 0x0025613C
		protected override bool IsNoiseResponder
		{
			get
			{
				return this.ActiveOuter != null || !this.StoreState.IsNone;
			}
		}

		// Token: 0x06007E07 RID: 32263 RVA: 0x00257F56 File Offset: 0x00256156
		public bool IsOnlyOnEnter()
		{
			return !this.EveryFrame;
		}

		// Token: 0x06007E08 RID: 32264 RVA: 0x00257F64 File Offset: 0x00256164
		public override void Reset()
		{
			this.Target = null;
			this.MinReactDelay = null;
			this.MaxReactDelay = null;
			this.StoreState = null;
			this.None = null;
			this.ActiveInner = null;
			this.ActiveOuter = null;
			this.IgnoreNeedolinRange = null;
			this.useActiveBool = false;
			this.ActiveBool = null;
			this.EveryFrame = true;
		}

		// Token: 0x06007E09 RID: 32265 RVA: 0x00257FC0 File Offset: 0x002561C0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.transform = (safe ? safe.transform : null);
			this.StoreState.Value = HeroPerformanceRegion.AffectedState.None;
			this.storedState = HeroPerformanceRegion.AffectedState.None;
			base.OnEnter();
			if (!safe)
			{
				base.Finish();
				return;
			}
			base.DoAction(this.EveryFrame);
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007E0A RID: 32266 RVA: 0x00258038 File Offset: 0x00256238
		protected override void OnAffectedState(HeroPerformanceRegion.AffectedState affectedState)
		{
			if (affectedState == HeroPerformanceRegion.AffectedState.ActiveInner)
			{
				this.currentEvents[0] = this.ActiveInner;
				this.currentEvents[1] = this.ActiveOuter;
				return;
			}
			if (affectedState != HeroPerformanceRegion.AffectedState.ActiveOuter)
			{
				this.currentEvents[0] = this.None;
				this.currentEvents[1] = null;
				return;
			}
			this.currentEvents[0] = this.ActiveOuter;
			this.currentEvents[1] = null;
		}

		// Token: 0x06007E0B RID: 32267 RVA: 0x0025809C File Offset: 0x0025629C
		protected override void SendEvents(HeroPerformanceRegion.AffectedState affectedState)
		{
			if (this.storedState != affectedState)
			{
				FsmEnum storeState = this.StoreState;
				this.storedState = affectedState;
				storeState.Value = affectedState;
			}
			foreach (FsmEvent fsmEvent in this.currentEvents)
			{
				if (fsmEvent != null)
				{
					base.Fsm.Event(fsmEvent);
				}
			}
		}

		// Token: 0x04007DD4 RID: 32212
		public FsmOwnerDefault Target;

		// Token: 0x04007DD5 RID: 32213
		[HideIf("IsOnlyOnEnter")]
		public FsmFloat MinReactDelay;

		// Token: 0x04007DD6 RID: 32214
		[HideIf("IsOnlyOnEnter")]
		public FsmFloat MaxReactDelay;

		// Token: 0x04007DD7 RID: 32215
		public FsmEvent None;

		// Token: 0x04007DD8 RID: 32216
		public FsmEvent ActiveInner;

		// Token: 0x04007DD9 RID: 32217
		public FsmEvent ActiveOuter;

		// Token: 0x04007DDA RID: 32218
		public FsmBool IgnoreNeedolinRange;

		// Token: 0x04007DDB RID: 32219
		public bool useActiveBool;

		// Token: 0x04007DDC RID: 32220
		[UIHint(UIHint.Variable)]
		public FsmBool ActiveBool;

		// Token: 0x04007DDD RID: 32221
		[ObjectType(typeof(HeroPerformanceRegion.AffectedState))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreState;

		// Token: 0x04007DDE RID: 32222
		public bool EveryFrame;

		// Token: 0x04007DDF RID: 32223
		private readonly FsmEvent[] currentEvents = new FsmEvent[2];

		// Token: 0x04007DE0 RID: 32224
		private Transform transform;

		// Token: 0x04007DE1 RID: 32225
		private HeroPerformanceRegion.AffectedState storedState;
	}
}
