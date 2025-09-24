using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000911 RID: 2321
	public interface IMouseProvider
	{
		// Token: 0x06005245 RID: 21061
		void Setup();

		// Token: 0x06005246 RID: 21062
		void Reset();

		// Token: 0x06005247 RID: 21063
		void Update();

		// Token: 0x06005248 RID: 21064
		Vector2 GetPosition();

		// Token: 0x06005249 RID: 21065
		float GetDeltaX();

		// Token: 0x0600524A RID: 21066
		float GetDeltaY();

		// Token: 0x0600524B RID: 21067
		float GetDeltaScroll();

		// Token: 0x0600524C RID: 21068
		bool GetButtonIsPressed(Mouse control);

		// Token: 0x0600524D RID: 21069
		bool GetButtonWasPressed(Mouse control);

		// Token: 0x0600524E RID: 21070
		bool GetButtonWasReleased(Mouse control);

		// Token: 0x0600524F RID: 21071
		bool HasMousePresent();
	}
}
