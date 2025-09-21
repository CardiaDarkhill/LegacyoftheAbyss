using System;

namespace InControl
{
	// Token: 0x020008D9 RID: 2265
	public class BindingListenOptions
	{
		// Token: 0x06004F0E RID: 20238 RVA: 0x0016F68A File Offset: 0x0016D88A
		public bool CallOnBindingFound(PlayerAction playerAction, BindingSource bindingSource)
		{
			return this.OnBindingFound == null || this.OnBindingFound(playerAction, bindingSource);
		}

		// Token: 0x06004F0F RID: 20239 RVA: 0x0016F6A3 File Offset: 0x0016D8A3
		public void CallOnBindingAdded(PlayerAction playerAction, BindingSource bindingSource)
		{
			if (this.OnBindingAdded != null)
			{
				this.OnBindingAdded(playerAction, bindingSource);
			}
		}

		// Token: 0x06004F10 RID: 20240 RVA: 0x0016F6BA File Offset: 0x0016D8BA
		public void CallOnBindingRejected(PlayerAction playerAction, BindingSource bindingSource, BindingSourceRejectionType bindingSourceRejectionType)
		{
			if (this.OnBindingRejected != null)
			{
				this.OnBindingRejected(playerAction, bindingSource, bindingSourceRejectionType);
			}
		}

		// Token: 0x06004F11 RID: 20241 RVA: 0x0016F6D2 File Offset: 0x0016D8D2
		public void CallOnBindingEnded(PlayerAction playerAction)
		{
			if (this.OnBindingEnded != null)
			{
				this.OnBindingEnded(playerAction);
			}
		}

		// Token: 0x04004F9B RID: 20379
		public bool IncludeControllers = true;

		// Token: 0x04004F9C RID: 20380
		public bool IncludeUnknownControllers;

		// Token: 0x04004F9D RID: 20381
		public bool IncludeNonStandardControls = true;

		// Token: 0x04004F9E RID: 20382
		public bool IncludeMouseButtons;

		// Token: 0x04004F9F RID: 20383
		public bool IncludeMouseScrollWheel;

		// Token: 0x04004FA0 RID: 20384
		public bool IncludeKeys = true;

		// Token: 0x04004FA1 RID: 20385
		public bool IncludeModifiersAsFirstClassKeys;

		// Token: 0x04004FA2 RID: 20386
		public uint MaxAllowedBindings;

		// Token: 0x04004FA3 RID: 20387
		public uint MaxAllowedBindingsPerType;

		// Token: 0x04004FA4 RID: 20388
		public bool AllowDuplicateBindingsPerSet;

		// Token: 0x04004FA5 RID: 20389
		public bool UnsetDuplicateBindingsOnSet;

		// Token: 0x04004FA6 RID: 20390
		public bool RejectRedundantBindings;

		// Token: 0x04004FA7 RID: 20391
		public BindingSource ReplaceBinding;

		// Token: 0x04004FA8 RID: 20392
		public Func<PlayerAction, BindingSource, bool> OnBindingFound;

		// Token: 0x04004FA9 RID: 20393
		public Action<PlayerAction, BindingSource> OnBindingAdded;

		// Token: 0x04004FAA RID: 20394
		public Action<PlayerAction, BindingSource, BindingSourceRejectionType> OnBindingRejected;

		// Token: 0x04004FAB RID: 20395
		public Action<PlayerAction> OnBindingEnded;
	}
}
