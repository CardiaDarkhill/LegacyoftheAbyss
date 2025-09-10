using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF9 RID: 3321
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Remove all components from a GameObject.")]
	public class RemoveAllComponents : FsmStateAction
	{
		// Token: 0x06006276 RID: 25206 RVA: 0x001F2652 File Offset: 0x001F0852
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006277 RID: 25207 RVA: 0x001F265C File Offset: 0x001F085C
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				foreach (MonoBehaviour monoBehaviour in value.GetComponents<MonoBehaviour>())
				{
					Debug.Log(monoBehaviour.name);
					if (monoBehaviour.name != "Play Maker FSM")
					{
						monoBehaviour.name != "Persistent Bool Item";
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040060DE RID: 24798
		[RequiredField]
		[Tooltip("The GameObject to destroy.")]
		public FsmGameObject gameObject;
	}
}
