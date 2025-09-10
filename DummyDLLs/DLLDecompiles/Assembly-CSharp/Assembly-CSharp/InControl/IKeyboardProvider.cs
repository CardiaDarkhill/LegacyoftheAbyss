using System;

namespace InControl
{
	// Token: 0x0200090F RID: 2319
	public interface IKeyboardProvider
	{
		// Token: 0x06005237 RID: 21047
		void Setup();

		// Token: 0x06005238 RID: 21048
		void Reset();

		// Token: 0x06005239 RID: 21049
		void Update();

		// Token: 0x0600523A RID: 21050
		bool AnyKeyIsPressed();

		// Token: 0x0600523B RID: 21051
		bool GetKeyIsPressed(Key control);

		// Token: 0x0600523C RID: 21052
		string GetNameForKey(Key control);
	}
}
