using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E1 RID: 4833
	public class CheckHeroPerformanceRegionV2 : CheckHeroPerformanceRegionBase
	{
		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x06007E0D RID: 32269 RVA: 0x00258108 File Offset: 0x00256308
		protected override bool IsActive
		{
			get
			{
				return this.ActiveBool.Value || !this.UseActiveBool.Value;
			}
		}

		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x06007E0E RID: 32270 RVA: 0x00258127 File Offset: 0x00256327
		protected override Transform TargetTransform
		{
			get
			{
				return this.transform;
			}
		}

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x06007E0F RID: 32271 RVA: 0x0025812F File Offset: 0x0025632F
		protected override float TargetRadius
		{
			get
			{
				return this.Radius.Value;
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x06007E10 RID: 32272 RVA: 0x0025813C File Offset: 0x0025633C
		protected override float NewDelay
		{
			get
			{
				return new MinMaxFloat(this.MinReactDelay.Value, this.MaxReactDelay.Value).GetRandomValue();
			}
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x06007E11 RID: 32273 RVA: 0x0025816C File Offset: 0x0025636C
		protected override bool UseNeedolinRange
		{
			get
			{
				return !this.IgnoreNeedolinRange.Value;
			}
		}

		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x06007E12 RID: 32274 RVA: 0x0025817C File Offset: 0x0025637C
		protected override bool IsNoiseResponder
		{
			get
			{
				return this.ActiveOuter != null || !this.StoreState.IsNone;
			}
		}

		// Token: 0x06007E13 RID: 32275 RVA: 0x00258196 File Offset: 0x00256396
		public bool IsOnlyOnEnter()
		{
			return !this.EveryFrame;
		}

		// Token: 0x06007E14 RID: 32276 RVA: 0x002581A4 File Offset: 0x002563A4
		public override void Reset()
		{
			this.Target = null;
			this.Radius = null;
			this.MinReactDelay = null;
			this.MaxReactDelay = null;
			this.StoreState = null;
			this.None = null;
			this.ActiveInner = null;
			this.ActiveOuter = null;
			this.IgnoreNeedolinRange = null;
			this.UseActiveBool = false;
			this.ActiveBool = null;
			this.EveryFrame = true;
		}

		// Token: 0x06007E15 RID: 32277 RVA: 0x0025820C File Offset: 0x0025640C
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

		// Token: 0x06007E16 RID: 32278 RVA: 0x00258284 File Offset: 0x00256484
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

		// Token: 0x06007E17 RID: 32279 RVA: 0x002582E8 File Offset: 0x002564E8
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

		// Token: 0x04007DE2 RID: 32226
		public FsmOwnerDefault Target;

		// Token: 0x04007DE3 RID: 32227
		public FsmFloat Radius;

		// Token: 0x04007DE4 RID: 32228
		[HideIf("IsOnlyOnEnter")]
		public FsmFloat MinReactDelay;

		// Token: 0x04007DE5 RID: 32229
		[HideIf("IsOnlyOnEnter")]
		public FsmFloat MaxReactDelay;

		// Token: 0x04007DE6 RID: 32230
		public FsmEvent None;

		// Token: 0x04007DE7 RID: 32231
		public FsmEvent ActiveInner;

		// Token: 0x04007DE8 RID: 32232
		public FsmEvent ActiveOuter;

		// Token: 0x04007DE9 RID: 32233
		public FsmBool IgnoreNeedolinRange;

		// Token: 0x04007DEA RID: 32234
		public FsmBool UseActiveBool;

		// Token: 0x04007DEB RID: 32235
		[UIHint(UIHint.Variable)]
		public FsmBool ActiveBool;

		// Token: 0x04007DEC RID: 32236
		[ObjectType(typeof(HeroPerformanceRegion.AffectedState))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreState;

		// Token: 0x04007DED RID: 32237
		public bool EveryFrame;

		// Token: 0x04007DEE RID: 32238
		private readonly FsmEvent[] currentEvents = new FsmEvent[2];

		// Token: 0x04007DEF RID: 32239
		private Transform transform;

		// Token: 0x04007DF0 RID: 32240
		private HeroPerformanceRegion.AffectedState storedState;
	}
}
