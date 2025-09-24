using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils
{
	// Token: 0x02000B0D RID: 2829
	[AttributeUsage(AttributeTargets.All)]
	public class EventTargetVariable : Attribute
	{
		// Token: 0x06005933 RID: 22835 RVA: 0x001C44B6 File Offset: 0x001C26B6
		public EventTargetVariable(string variable)
		{
			this.variable = variable;
		}

		// Token: 0x04005483 RID: 21635
		public string variable;
	}
}
