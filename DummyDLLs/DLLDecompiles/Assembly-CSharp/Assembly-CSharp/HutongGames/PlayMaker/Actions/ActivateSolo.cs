using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E9D RID: 3741
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates a GameObject and de-activates other GameObjects at the same level of the hierarchy. E.g, a single UI Screen, a single accessory etc. This action is very helpful if you often organize GameObject hierarchies in this way. \nNOTE: The targeted GameObject should have a parent. This action will not work on GameObjects at the scene root.")]
	public class ActivateSolo : FsmStateAction
	{
		// Token: 0x06006A23 RID: 27171 RVA: 0x00212DCC File Offset: 0x00210FCC
		public override void Reset()
		{
			this.gameObject = null;
			this.allowReactivate = new FsmBool
			{
				Value = true
			};
		}

		// Token: 0x06006A24 RID: 27172 RVA: 0x00212DE7 File Offset: 0x00210FE7
		public override void OnEnter()
		{
			this.DoActivateSolo();
			base.Finish();
		}

		// Token: 0x06006A25 RID: 27173 RVA: 0x00212DF8 File Offset: 0x00210FF8
		private void DoActivateSolo()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || ownerDefaultTarget.transform.parent == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform;
			foreach (object obj in ownerDefaultTarget.transform.parent.transform)
			{
				Transform transform2 = (Transform)obj;
				if (transform2 != transform)
				{
					transform2.gameObject.SetActive(false);
				}
			}
			if (this.allowReactivate.Value && Time.frameCount != this.activatedFrame)
			{
				transform.gameObject.SetActive(false);
				this.activatedFrame = Time.frameCount;
			}
			ownerDefaultTarget.SetActive(true);
		}

		// Token: 0x0400697B RID: 27003
		[RequiredField]
		[Tooltip("The GameObject to activate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400697C RID: 27004
		[Tooltip("Re-activate if already active. This means deactivating the target GameObject then activating it again. This will reset FSMs on that GameObject.")]
		public FsmBool allowReactivate;

		// Token: 0x0400697D RID: 27005
		private int activatedFrame = -1;
	}
}
