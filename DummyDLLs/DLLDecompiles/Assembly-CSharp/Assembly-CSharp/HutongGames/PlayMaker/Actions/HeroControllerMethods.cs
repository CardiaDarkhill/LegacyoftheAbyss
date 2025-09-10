using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001260 RID: 4704
	public sealed class HeroControllerMethods : FsmStateAction
	{
		// Token: 0x06007C23 RID: 31779 RVA: 0x00251C3E File Offset: 0x0024FE3E
		public override void Reset()
		{
			this.parameters = null;
			this.everyFrame = null;
			this.storeValue = new FsmVar();
			this.isTrue = null;
			this.isFalse = null;
		}

		// Token: 0x06007C24 RID: 31780 RVA: 0x00251C67 File Offset: 0x0024FE67
		public override void OnPreprocess()
		{
			this.ValidateParams();
		}

		// Token: 0x06007C25 RID: 31781 RVA: 0x00251C70 File Offset: 0x0024FE70
		public override void OnEnter()
		{
			this.hasHero = this.heroController;
			if (!this.hasHero)
			{
				this.heroController = HeroController.instance;
				this.hasHero = this.heroController;
			}
			if (!this.hasHero || !this.everyFrame.Value)
			{
				this.SendMethod();
				base.Finish();
			}
			if (!this.hasHero)
			{
				Debug.LogError("Failed to find hero controller.");
			}
		}

		// Token: 0x06007C26 RID: 31782 RVA: 0x00251CE5 File Offset: 0x0024FEE5
		public override void OnUpdate()
		{
			this.SendMethod();
		}

		// Token: 0x06007C27 RID: 31783 RVA: 0x00251CF0 File Offset: 0x0024FEF0
		private void SendMethod()
		{
			if (!this.hasHero)
			{
				return;
			}
			if (this.SendMethodFaster())
			{
				this.SendTestEvents();
				return;
			}
			HeroControllerMethods.HeroMethod heroMethod;
			if (HeroControllerMethods.HeroMethods.methodInfos.TryGetValue(this.method, out heroMethod))
			{
				try
				{
					heroMethod.SendMethod(this);
					this.SendTestEvents();
				}
				catch (Exception)
				{
					base.Finish();
				}
			}
		}

		// Token: 0x06007C28 RID: 31784 RVA: 0x00251D54 File Offset: 0x0024FF54
		private void SendTestEvents()
		{
			if (this.storeValue.Type == VariableType.Bool)
			{
				base.Fsm.Event(this.storeValue.boolValue ? this.isTrue : this.isFalse);
			}
		}

		// Token: 0x06007C29 RID: 31785 RVA: 0x00251D8C File Offset: 0x0024FF8C
		private bool GetBool(int index = 0)
		{
			bool result = false;
			if (this.parameters != null && this.parameters.Length >= index + 1)
			{
				FsmVar fsmVar = this.parameters[index];
				if (fsmVar != null)
				{
					result = fsmVar.boolValue;
				}
			}
			return result;
		}

		// Token: 0x06007C2A RID: 31786 RVA: 0x00251DC4 File Offset: 0x0024FFC4
		private GameObject GetGameObject(int index = 0)
		{
			GameObject result = null;
			if (this.parameters != null && this.parameters.Length >= index + 1)
			{
				FsmVar fsmVar = this.parameters[index];
				if (fsmVar != null)
				{
					result = fsmVar.gameObjectValue;
				}
			}
			return result;
		}

		// Token: 0x06007C2B RID: 31787 RVA: 0x00251DFC File Offset: 0x0024FFFC
		private float GetFloat(int index = 0)
		{
			float result = 0f;
			if (this.parameters != null && this.parameters.Length >= index + 1)
			{
				FsmVar fsmVar = this.parameters[index];
				if (fsmVar != null)
				{
					result = fsmVar.floatValue;
				}
			}
			return result;
		}

		// Token: 0x06007C2C RID: 31788 RVA: 0x00251E38 File Offset: 0x00250038
		private bool SendMethodFaster()
		{
			try
			{
				if (this.parameters != null)
				{
					for (int i = 0; i < this.parameters.Length; i++)
					{
						this.parameters[i].UpdateValue();
					}
				}
				switch (this.method)
				{
				case HeroControllerMethods.Method.SetAllowNailChargingWhileRelinquished:
					this.heroController.SetAllowNailChargingWhileRelinquished(this.GetBool(0));
					return true;
				case HeroControllerMethods.Method.RelinquishControlNotVelocity:
					this.heroController.RelinquishControlNotVelocity();
					return true;
				case HeroControllerMethods.Method.StopAnimationControl:
					this.heroController.StopAnimationControl();
					return true;
				case HeroControllerMethods.Method.FlipSprite:
					this.heroController.FlipSprite();
					return true;
				case HeroControllerMethods.Method.ShouldHardLand:
				{
					bool flag = this.heroController.ShouldHardLand(this.GetGameObject(0));
					this.storeValue.SetValue(flag);
					return true;
				}
				case HeroControllerMethods.Method.StartAnimationControlToIdle:
					this.heroController.StartAnimationControlToIdle();
					return true;
				case HeroControllerMethods.Method.RegainControl:
					this.heroController.RegainControl();
					return true;
				case HeroControllerMethods.Method.RelinquishControl:
					this.heroController.RelinquishControl();
					return true;
				case HeroControllerMethods.Method.DoHardLanding:
					this.heroController.DoHardLanding();
					return true;
				case HeroControllerMethods.Method.SetAllowRecoilWhileRelinquished:
					this.heroController.SetAllowRecoilWhileRelinquished(this.GetBool(0));
					return true;
				case HeroControllerMethods.Method.ResetGravity:
					this.heroController.ResetGravity();
					return true;
				case HeroControllerMethods.Method.TryFsmCancelToWallSlide:
				{
					bool flag2 = this.heroController.TryFsmCancelToWallSlide();
					this.storeValue.SetValue(flag2);
					return true;
				}
				case HeroControllerMethods.Method.DashCooldownReady:
				{
					bool flag3 = this.heroController.DashCooldownReady();
					this.storeValue.SetValue(flag3);
					return true;
				}
				case HeroControllerMethods.Method.SetStartWithDash:
					this.heroController.SetStartWithDash();
					return true;
				case HeroControllerMethods.Method.CouldJumpCancel:
				{
					bool flag4 = this.heroController.CouldJumpCancel();
					this.storeValue.SetValue(flag4);
					return true;
				}
				case HeroControllerMethods.Method.SetStartWithAnyJump:
					this.heroController.SetStartWithAnyJump();
					return true;
				case HeroControllerMethods.Method.AffectedByGravity:
					this.heroController.AffectedByGravity(this.GetBool(0));
					return true;
				case HeroControllerMethods.Method.StartAnimationControl:
					this.heroController.StartAnimationControl();
					return true;
				case HeroControllerMethods.Method.IncrementAttackCounter:
					this.heroController.IncrementAttackCounter();
					return true;
				case HeroControllerMethods.Method.AllowShuttleCock:
					this.heroController.AllowShuttleCock();
					return true;
				case HeroControllerMethods.Method.SetSilkRegenBlocked:
					this.heroController.SetSilkRegenBlocked(this.GetBool(0));
					return true;
				case HeroControllerMethods.Method.MaxHealthKeepBlue:
					this.heroController.MaxHealthKeepBlue();
					return true;
				case HeroControllerMethods.Method.StopPlayingAudio:
					this.heroController.StopPlayingAudio();
					return true;
				case HeroControllerMethods.Method.CancelAttack:
					this.heroController.CancelAttack(this.GetBool(0));
					return true;
				case HeroControllerMethods.Method.RefreshAnimationEvents:
					this.heroController.AnimCtrl.RefreshAnimationEvents();
					return true;
				case HeroControllerMethods.Method.CancelQueuedBounces:
					this.heroController.CancelQueuedBounces();
					break;
				case HeroControllerMethods.Method.SetToolCooldown:
					this.heroController.SetToolCooldown(this.GetFloat(0));
					break;
				case HeroControllerMethods.Method.ForceClampTerminalVelocity:
					this.heroController.ForceClampTerminalVelocity = this.GetBool(0);
					break;
				case HeroControllerMethods.Method.IsParryInvulnerable:
				{
					bool flag5 = this.heroController.IsParrying();
					this.storeValue.SetValue(flag5);
					break;
				}
				case HeroControllerMethods.Method.TrySpawnSoftLandEffect:
				{
					bool flag6 = this.heroController.TrySpawnSoftLandingPrefab();
					this.storeValue.SetValue(flag6);
					break;
				}
				case HeroControllerMethods.Method.IsParryingActive:
				{
					bool flag7 = this.heroController.IsParryingActive();
					this.storeValue.SetValue(flag7);
					break;
				}
				case HeroControllerMethods.Method.BlockSteepSlopes:
					this.heroController.SetBlockSteepSlopes(this.GetBool(0));
					break;
				case HeroControllerMethods.Method.PlayIdle:
					this.heroController.AnimCtrl.PlayIdle();
					break;
				case HeroControllerMethods.Method.IsHurt:
				{
					bool flag8 = this.heroController.AnimCtrl.IsHurt();
					this.storeValue.SetValue(flag8);
					break;
				}
				case HeroControllerMethods.Method.ThrowToolCooldownReady:
				{
					bool flag9 = this.heroController.ThrowToolCooldownReady();
					this.storeValue.SetValue(flag9);
					break;
				}
				case HeroControllerMethods.Method.CanTryHarpoonDash:
				{
					bool flag10 = this.heroController.CanTryHarpoonDash();
					this.storeValue.SetValue(flag10);
					break;
				}
				case HeroControllerMethods.Method.StartAnimationControlToIdleForcePlay:
					this.heroController.StartAnimationControlToIdleForcePlay();
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x06007C2D RID: 31789 RVA: 0x002522C0 File Offset: 0x002504C0
		public bool ShouldHideParameters()
		{
			HeroControllerMethods.HeroMethod heroMethod;
			return HeroControllerMethods.HeroMethods.methodInfos.TryGetValue(this.method, out heroMethod) && (this.parameters == null || this.parameters.Length == 0);
		}

		// Token: 0x06007C2E RID: 31790 RVA: 0x002522F7 File Offset: 0x002504F7
		public bool ShouldHideStoreValue()
		{
			return this.GetReturnType() == VariableType.Unknown;
		}

		// Token: 0x06007C2F RID: 31791 RVA: 0x00252302 File Offset: 0x00250502
		public bool ShouldHideTrueFalse()
		{
			return this.GetReturnType() != VariableType.Bool;
		}

		// Token: 0x06007C30 RID: 31792 RVA: 0x00252310 File Offset: 0x00250510
		public VariableType GetReturnType()
		{
			HeroControllerMethods.HeroMethod heroMethod;
			if (HeroControllerMethods.HeroMethods.methodInfos.TryGetValue(this.method, out heroMethod))
			{
				return heroMethod.returnType;
			}
			return VariableType.Unknown;
		}

		// Token: 0x06007C31 RID: 31793 RVA: 0x0025233C File Offset: 0x0025053C
		public void ValidateParams()
		{
			HeroControllerMethods.HeroMethod heroMethod;
			if (HeroControllerMethods.HeroMethods.methodInfos.TryGetValue(this.method, out heroMethod))
			{
				this.EnsureParams(heroMethod.parameters);
				this.EnsureReturnValue(heroMethod.returnType);
			}
		}

		// Token: 0x06007C32 RID: 31794 RVA: 0x00252378 File Offset: 0x00250578
		private void EnsureParams(params VariableType[] types)
		{
			if (types == null || types.Length == 0)
			{
				this.parameters = Array.Empty<FsmVar>();
				return;
			}
			if (this.parameters == null)
			{
				this.parameters = new FsmVar[types.Length];
			}
			else if (this.parameters.Length != types.Length)
			{
				Array.Resize<FsmVar>(ref this.parameters, types.Length);
			}
			for (int i = 0; i < this.parameters.Length; i++)
			{
				FsmVar fsmVar = this.parameters[i];
				if (fsmVar == null)
				{
					fsmVar = (this.parameters[i] = new FsmVar());
				}
				fsmVar.Type = types[i];
				this.parameters[i] = fsmVar;
			}
		}

		// Token: 0x06007C33 RID: 31795 RVA: 0x0025240D File Offset: 0x0025060D
		private void EnsureReturnValue(VariableType variableType)
		{
			if (this.storeValue != null)
			{
				this.storeValue.Type = variableType;
			}
		}

		// Token: 0x04007C3E RID: 31806
		public HeroControllerMethods.Method method;

		// Token: 0x04007C3F RID: 31807
		[HideIf("ShouldHideParameters")]
		public FsmVar[] parameters;

		// Token: 0x04007C40 RID: 31808
		public FsmBool everyFrame;

		// Token: 0x04007C41 RID: 31809
		[HideIf("ShouldHideStoreValue")]
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		public FsmVar storeValue;

		// Token: 0x04007C42 RID: 31810
		[ActionSection("Events")]
		[HideIf("ShouldHideTrueFalse")]
		public FsmEvent isTrue;

		// Token: 0x04007C43 RID: 31811
		[HideIf("ShouldHideTrueFalse")]
		public FsmEvent isFalse;

		// Token: 0x04007C44 RID: 31812
		private bool hasHero;

		// Token: 0x04007C45 RID: 31813
		private HeroController heroController;

		// Token: 0x02001BE0 RID: 7136
		public enum Method
		{
			// Token: 0x04009F4D RID: 40781
			SetAllowNailChargingWhileRelinquished,
			// Token: 0x04009F4E RID: 40782
			RelinquishControlNotVelocity,
			// Token: 0x04009F4F RID: 40783
			StopAnimationControl,
			// Token: 0x04009F50 RID: 40784
			FlipSprite,
			// Token: 0x04009F51 RID: 40785
			ShouldHardLand,
			// Token: 0x04009F52 RID: 40786
			StartAnimationControlToIdle,
			// Token: 0x04009F53 RID: 40787
			RegainControl,
			// Token: 0x04009F54 RID: 40788
			RelinquishControl,
			// Token: 0x04009F55 RID: 40789
			DoHardLanding,
			// Token: 0x04009F56 RID: 40790
			SetAllowRecoilWhileRelinquished,
			// Token: 0x04009F57 RID: 40791
			ResetGravity,
			// Token: 0x04009F58 RID: 40792
			TryFsmCancelToWallSlide,
			// Token: 0x04009F59 RID: 40793
			DashCooldownReady,
			// Token: 0x04009F5A RID: 40794
			SetStartWithDash,
			// Token: 0x04009F5B RID: 40795
			CouldJumpCancel,
			// Token: 0x04009F5C RID: 40796
			SetStartWithAnyJump,
			// Token: 0x04009F5D RID: 40797
			AffectedByGravity,
			// Token: 0x04009F5E RID: 40798
			StartAnimationControl,
			// Token: 0x04009F5F RID: 40799
			IncrementAttackCounter,
			// Token: 0x04009F60 RID: 40800
			AllowShuttleCock,
			// Token: 0x04009F61 RID: 40801
			SetSilkRegenBlocked,
			// Token: 0x04009F62 RID: 40802
			MaxHealthKeepBlue,
			// Token: 0x04009F63 RID: 40803
			StopPlayingAudio,
			// Token: 0x04009F64 RID: 40804
			CancelAttack,
			// Token: 0x04009F65 RID: 40805
			RefreshAnimationEvents,
			// Token: 0x04009F66 RID: 40806
			CancelQueuedBounces,
			// Token: 0x04009F67 RID: 40807
			SetToolCooldown,
			// Token: 0x04009F68 RID: 40808
			ForceClampTerminalVelocity,
			// Token: 0x04009F69 RID: 40809
			IsParryInvulnerable,
			// Token: 0x04009F6A RID: 40810
			TrySpawnSoftLandEffect,
			// Token: 0x04009F6B RID: 40811
			IsParryingActive,
			// Token: 0x04009F6C RID: 40812
			BlockSteepSlopes,
			// Token: 0x04009F6D RID: 40813
			PlayIdle,
			// Token: 0x04009F6E RID: 40814
			IsHurt,
			// Token: 0x04009F6F RID: 40815
			ThrowToolCooldownReady,
			// Token: 0x04009F70 RID: 40816
			CanTryHarpoonDash,
			// Token: 0x04009F71 RID: 40817
			StartAnimationControlToIdleForcePlay
		}

		// Token: 0x02001BE1 RID: 7137
		public static class HeroMethods
		{
			// Token: 0x06009A7B RID: 39547 RVA: 0x002B37CC File Offset: 0x002B19CC
			static HeroMethods()
			{
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.SetAllowNailChargingWhileRelinquished, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetAllowNailChargingWhileRelinquished(action.parameters[0].boolValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.RelinquishControlNotVelocity, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.RelinquishControlNotVelocity();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.StopAnimationControl, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StopAnimationControl();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.FlipSprite, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.FlipSprite();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.ShouldHardLand, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.GameObject
				}, VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.ShouldHardLand(action.parameters[0].gameObjectValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.StartAnimationControlToIdle, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StartAnimationControlToIdle();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.StartAnimationControlToIdleForcePlay, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StartAnimationControlToIdleForcePlay();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.RegainControl, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.RegainControl();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.RelinquishControl, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.RelinquishControl();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.DoHardLanding, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.DoHardLanding();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.SetAllowRecoilWhileRelinquished, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetAllowRecoilWhileRelinquished(action.parameters[0].boolValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.ResetGravity, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.ResetGravity();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.TryFsmCancelToWallSlide, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.TryFsmCancelToWallSlide();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.DashCooldownReady, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.DashCooldownReady();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.SetStartWithDash, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetStartWithDash();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.CouldJumpCancel, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.CouldJumpCancel();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.SetStartWithAnyJump, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetStartWithAnyJump();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.AffectedByGravity, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.AffectedByGravity(action.parameters[0].boolValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.StartAnimationControl, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StartAnimationControl();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.IncrementAttackCounter, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.IncrementAttackCounter();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.AllowShuttleCock, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.AllowShuttleCock();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.SetSilkRegenBlocked, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetSilkRegenBlocked(action.parameters[0].boolValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.MaxHealthKeepBlue, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.MaxHealthKeepBlue();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.StopPlayingAudio, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StopPlayingAudio();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.CancelAttack, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.CancelAttack(action.parameters[0].boolValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.RefreshAnimationEvents, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StopPlayingAudio();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.CancelQueuedBounces, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.CancelQueuedBounces();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.SetToolCooldown, new HeroControllerMethods.HeroMethod(new VariableType[1], VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetToolCooldown(action.parameters[0].floatValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.ForceClampTerminalVelocity, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.ForceClampTerminalVelocity = action.parameters[0].boolValue;
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.IsParryInvulnerable, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.IsParrying();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.TrySpawnSoftLandEffect, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.TrySpawnSoftLandingPrefab();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.IsParryingActive, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.IsParryingActive();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.BlockSteepSlopes, new HeroControllerMethods.HeroMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetBlockSteepSlopes(action.parameters[0].boolValue);
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.PlayIdle, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.AnimCtrl.PlayIdle();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.IsHurt, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.AnimCtrl.IsHurt();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.ThrowToolCooldownReady, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.ThrowToolCooldownReady();
					}
				}));
				HeroControllerMethods.HeroMethods.methodInfos.Add(HeroControllerMethods.Method.CanTryHarpoonDash, new HeroControllerMethods.HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.CanTryHarpoonDash();
					}
				}));
			}

			// Token: 0x04009F72 RID: 40818
			public static readonly Dictionary<HeroControllerMethods.Method, HeroControllerMethods.HeroMethod> methodInfos = new Dictionary<HeroControllerMethods.Method, HeroControllerMethods.HeroMethod>();
		}

		// Token: 0x02001BE2 RID: 7138
		public sealed class HeroMethod
		{
			// Token: 0x170011B5 RID: 4533
			// (get) Token: 0x06009A7C RID: 39548 RVA: 0x002B3DA6 File Offset: 0x002B1FA6
			public VariableType[] parameters { get; }

			// Token: 0x170011B6 RID: 4534
			// (get) Token: 0x06009A7D RID: 39549 RVA: 0x002B3DAE File Offset: 0x002B1FAE
			public VariableType returnType { get; }

			// Token: 0x06009A7E RID: 39550 RVA: 0x002B3DB6 File Offset: 0x002B1FB6
			public void SendMethod(HeroControllerMethods action)
			{
				Action<HeroControllerMethods> action2 = this.heroControllerAction;
				if (action2 == null)
				{
					return;
				}
				action2(action);
			}

			// Token: 0x06009A7F RID: 39551 RVA: 0x002B3DC9 File Offset: 0x002B1FC9
			public HeroMethod(VariableType[] parameters = null, VariableType returnType = VariableType.Unknown, Action<HeroControllerMethods> heroControllerAction = null)
			{
				if (parameters == null)
				{
					this.parameters = Array.Empty<VariableType>();
				}
				else
				{
					this.parameters = parameters;
				}
				this.returnType = returnType;
				this.heroControllerAction = heroControllerAction;
			}

			// Token: 0x04009F75 RID: 40821
			public Action<HeroControllerMethods> heroControllerAction;
		}
	}
}
