using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA5 RID: 3749
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroys GameObjects in an array.")]
	public class DestroyObjects : FsmStateAction
	{
		// Token: 0x06006A43 RID: 27203 RVA: 0x00213685 File Offset: 0x00211885
		public override void Reset()
		{
			this.gameObjects = null;
			this.delay = 0f;
		}

		// Token: 0x06006A44 RID: 27204 RVA: 0x002136A0 File Offset: 0x002118A0
		public override void OnEnter()
		{
			if (this.gameObjects.Values != null)
			{
				foreach (GameObject gameObject in this.gameObjects.Values)
				{
					if (gameObject != null)
					{
						if (this.delay.Value <= 0f)
						{
							Object.Destroy(gameObject);
						}
						else
						{
							Object.Destroy(gameObject, this.delay.Value);
						}
						if (this.detachChildren.Value)
						{
							gameObject.transform.DetachChildren();
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400699F RID: 27039
		[RequiredField]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		[Tooltip("The GameObjects to destroy.")]
		public FsmArray gameObjects;

		// Token: 0x040069A0 RID: 27040
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Optional delay before destroying the Game Objects.")]
		public FsmFloat delay;

		// Token: 0x040069A1 RID: 27041
		[Tooltip("Detach children before destroying the Game Objects.")]
		public FsmBool detachChildren;
	}
}
