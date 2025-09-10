using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6C RID: 3180
	public class GetHorizontallyClosestGameObject : FsmStateAction
	{
		// Token: 0x06006008 RID: 24584 RVA: 0x001E6AFA File Offset: 0x001E4CFA
		public override void Reset()
		{
			this.Target = null;
			this.GameObjects = new FsmGameObject[0];
			this.MatchEvents = new FsmEvent[0];
			this.StoreGameObject = null;
			this.EveryFrame = false;
		}

		// Token: 0x06006009 RID: 24585 RVA: 0x001E6B29 File Offset: 0x001E4D29
		public override string ErrorCheck()
		{
			if (this.GameObjects.Length == 0 && this.MatchEvents.Length == 0)
			{
				return string.Empty;
			}
			if (this.GameObjects.Length == this.MatchEvents.Length)
			{
				return string.Empty;
			}
			return "GameObjects and MatchEvents must have the same amount of elements!";
		}

		// Token: 0x0600600A RID: 24586 RVA: 0x001E6B60 File Offset: 0x001E4D60
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600600B RID: 24587 RVA: 0x001E6B76 File Offset: 0x001E4D76
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x0600600C RID: 24588 RVA: 0x001E6B80 File Offset: 0x001E4D80
		private void DoAction()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			Vector2 vector = safe.transform.position;
			int num = Mathf.Min(this.GameObjects.Length, this.MatchEvents.Length);
			if (num == 0)
			{
				return;
			}
			float num2 = float.MaxValue;
			GameObject value = null;
			FsmEvent fsmEvent = null;
			for (int i = 0; i < num; i++)
			{
				FsmGameObject fsmGameObject = this.GameObjects[i];
				if (fsmGameObject.Value)
				{
					float num3 = Mathf.Abs(fsmGameObject.Value.transform.position.x - vector.x);
					if (num3 < num2)
					{
						num2 = num3;
						value = fsmGameObject.Value;
						fsmEvent = this.MatchEvents[i];
					}
				}
			}
			this.StoreGameObject.Value = value;
			if (fsmEvent != null)
			{
				base.Fsm.Event(fsmEvent);
			}
		}

		// Token: 0x04005D60 RID: 23904
		public FsmOwnerDefault Target;

		// Token: 0x04005D61 RID: 23905
		public FsmGameObject[] GameObjects;

		// Token: 0x04005D62 RID: 23906
		public FsmEvent[] MatchEvents;

		// Token: 0x04005D63 RID: 23907
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreGameObject;

		// Token: 0x04005D64 RID: 23908
		public bool EveryFrame;
	}
}
