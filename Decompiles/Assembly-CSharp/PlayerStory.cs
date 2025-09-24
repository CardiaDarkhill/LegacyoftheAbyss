using System;
using System.Collections.Generic;

// Token: 0x0200047A RID: 1146
public static class PlayerStory
{
	// Token: 0x0600298B RID: 10635 RVA: 0x000B4E30 File Offset: 0x000B3030
	public static void RecordEvent(PlayerStory.EventTypes eventTypes)
	{
		if (eventTypes == PlayerStory.EventTypes.None)
		{
			return;
		}
		GameManager instance = GameManager.instance;
		PlayerData playerData2;
		PlayerData playerData = playerData2 = instance.playerData;
		if (playerData2.StoryEvents == null)
		{
			playerData2.StoryEvents = new List<PlayerStory.EventInfo>();
		}
		playerData.StoryEvents.Add(new PlayerStory.EventInfo
		{
			EventType = eventTypes,
			SceneName = instance.GetSceneNameString(),
			PlayTime = instance.PlayTime
		});
	}

	// Token: 0x02001787 RID: 6023
	public enum EventTypes
	{
		// Token: 0x04008E60 RID: 36448
		None = -1,
		// Token: 0x04008E61 RID: 36449
		HeartPiece,
		// Token: 0x04008E62 RID: 36450
		SpoolPiece,
		// Token: 0x04008E63 RID: 36451
		SimpleKey,
		// Token: 0x04008E64 RID: 36452
		MemoryLocket
	}

	// Token: 0x02001788 RID: 6024
	[Serializable]
	public struct EventInfo
	{
		// Token: 0x04008E65 RID: 36453
		public PlayerStory.EventTypes EventType;

		// Token: 0x04008E66 RID: 36454
		public string SceneName;

		// Token: 0x04008E67 RID: 36455
		public float PlayTime;
	}
}
