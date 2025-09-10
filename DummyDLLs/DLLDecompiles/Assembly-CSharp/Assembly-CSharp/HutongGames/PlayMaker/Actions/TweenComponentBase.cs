using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F9 RID: 4345
	public abstract class TweenComponentBase<T> : TweenActionBase where T : Component
	{
		// Token: 0x06007579 RID: 30073 RVA: 0x0023E306 File Offset: 0x0023C506
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
		}

		// Token: 0x0600757A RID: 30074 RVA: 0x0023E318 File Offset: 0x0023C518
		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
			}
			if (!this.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
			}
		}

		// Token: 0x0600757B RID: 30075 RVA: 0x0023E35C File Offset: 0x0023C55C
		protected bool UpdateCache(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.cachedComponent == null || this.cachedGameObject != go)
			{
				this.cachedComponent = go.GetComponent<T>();
				this.cachedGameObject = go;
				if (this.cachedComponent == null)
				{
					base.LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
				}
			}
			return this.cachedComponent != null;
		}

		// Token: 0x0600757C RID: 30076 RVA: 0x0023E3F6 File Offset: 0x0023C5F6
		protected override void DoTween()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600757D RID: 30077 RVA: 0x0023E400 File Offset: 0x0023C600
		public override string ErrorCheck()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!(ownerDefaultTarget != null) || !(ownerDefaultTarget.GetComponent<T>() == null))
			{
				return "";
			}
			if (typeof(T) == typeof(RectTransform))
			{
				return "This Tween only works with UI GameObjects";
			}
			string str = "GameObject missing component:\n";
			Type typeFromHandle = typeof(T);
			return str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
		}

		// Token: 0x040075EE RID: 30190
		[DisplayOrder(0)]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Game Object to tween.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040075EF RID: 30191
		protected GameObject cachedGameObject;

		// Token: 0x040075F0 RID: 30192
		protected T cachedComponent;
	}
}
