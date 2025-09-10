using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C60 RID: 3168
	public class GetClosestGameObject : FsmStateAction
	{
		// Token: 0x06005FCF RID: 24527 RVA: 0x001E5D85 File Offset: 0x001E3F85
		public override void Reset()
		{
			this.Target = null;
			this.GameObjects = new FsmGameObject[0];
			this.MatchEvents = new FsmEvent[0];
			this.StoreGameObject = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005FD0 RID: 24528 RVA: 0x001E5DB4 File Offset: 0x001E3FB4
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

		// Token: 0x06005FD1 RID: 24529 RVA: 0x001E5DEB File Offset: 0x001E3FEB
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FD2 RID: 24530 RVA: 0x001E5E01 File Offset: 0x001E4001
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005FD3 RID: 24531 RVA: 0x001E5E0C File Offset: 0x001E400C
		private void DoAction()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			this.targetPosition = safe.transform.position;
			this.matches.Clear();
			int num = Mathf.Min(this.GameObjects.Length, this.MatchEvents.Length);
			if (num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				this.matches.Add(new GetClosestGameObject.Match
				{
					GameObject = this.GameObjects[i],
					Event = this.MatchEvents[i]
				});
			}
			this.matches.Sort(delegate(GetClosestGameObject.Match match1, GetClosestGameObject.Match match2)
			{
				float num2 = match1.GameObject.Value ? Vector2.Distance(this.targetPosition, match1.GameObject.Value.transform.position) : float.MaxValue;
				float value = match2.GameObject.Value ? Vector2.Distance(this.targetPosition, match2.GameObject.Value.transform.position) : float.MaxValue;
				return num2.CompareTo(value);
			});
			GetClosestGameObject.Match match = this.matches[0];
			this.StoreGameObject.Value = match.GameObject.Value;
			base.Fsm.Event(match.Event);
		}

		// Token: 0x04005D23 RID: 23843
		public FsmOwnerDefault Target;

		// Token: 0x04005D24 RID: 23844
		public FsmGameObject[] GameObjects;

		// Token: 0x04005D25 RID: 23845
		public FsmEvent[] MatchEvents;

		// Token: 0x04005D26 RID: 23846
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreGameObject;

		// Token: 0x04005D27 RID: 23847
		public bool EveryFrame;

		// Token: 0x04005D28 RID: 23848
		private Vector2 targetPosition;

		// Token: 0x04005D29 RID: 23849
		private readonly List<GetClosestGameObject.Match> matches = new List<GetClosestGameObject.Match>();

		// Token: 0x02001B82 RID: 7042
		private struct Match
		{
			// Token: 0x04009D70 RID: 40304
			public FsmGameObject GameObject;

			// Token: 0x04009D71 RID: 40305
			public FsmEvent Event;
		}
	}
}
