using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils
{
	// Token: 0x02000B11 RID: 2833
	[Serializable]
	public class PlayMakerEvent
	{
		// Token: 0x06005936 RID: 22838 RVA: 0x001C44D5 File Offset: 0x001C26D5
		public PlayMakerEvent()
		{
		}

		// Token: 0x06005937 RID: 22839 RVA: 0x001C44DD File Offset: 0x001C26DD
		public PlayMakerEvent(string defaultEventName)
		{
			this.defaultEventName = defaultEventName;
			this.eventName = defaultEventName;
		}

		// Token: 0x06005938 RID: 22840 RVA: 0x001C44F4 File Offset: 0x001C26F4
		public bool SendEvent(PlayMakerFSM fromFsm, PlayMakerEventTarget eventTarget)
		{
			if (eventTarget.eventTarget == ProxyEventTarget.BroadCastAll)
			{
				PlayMakerFSM.BroadcastEvent(this.eventName);
			}
			else if (eventTarget.eventTarget == ProxyEventTarget.Owner || eventTarget.eventTarget == ProxyEventTarget.GameObject)
			{
				PlayMakerUtils.SendEventToGameObject(fromFsm, eventTarget.gameObject, this.eventName, eventTarget.includeChildren);
			}
			else if (eventTarget.eventTarget == ProxyEventTarget.FsmComponent)
			{
				eventTarget.fsmComponent.SendEvent(this.eventName);
			}
			return true;
		}

		// Token: 0x04005489 RID: 21641
		public string eventName;

		// Token: 0x0400548A RID: 21642
		public bool allowLocalEvents;

		// Token: 0x0400548B RID: 21643
		public string defaultEventName;
	}
}
