using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008F6 RID: 2294
	[Serializable]
	public struct InputControlSource
	{
		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x06005061 RID: 20577 RVA: 0x00172AE9 File Offset: 0x00170CE9
		// (set) Token: 0x06005062 RID: 20578 RVA: 0x00172AF1 File Offset: 0x00170CF1
		public InputControlSourceType SourceType
		{
			get
			{
				return this.sourceType;
			}
			set
			{
				this.sourceType = value;
			}
		}

		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x06005063 RID: 20579 RVA: 0x00172AFA File Offset: 0x00170CFA
		// (set) Token: 0x06005064 RID: 20580 RVA: 0x00172B02 File Offset: 0x00170D02
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		// Token: 0x06005065 RID: 20581 RVA: 0x00172B0B File Offset: 0x00170D0B
		public InputControlSource(InputControlSourceType sourceType, int index)
		{
			this.sourceType = sourceType;
			this.index = index;
		}

		// Token: 0x06005066 RID: 20582 RVA: 0x00172B1B File Offset: 0x00170D1B
		public InputControlSource(KeyCode keyCode)
		{
			this = new InputControlSource(InputControlSourceType.KeyCode, (int)keyCode);
		}

		// Token: 0x06005067 RID: 20583 RVA: 0x00172B28 File Offset: 0x00170D28
		public float GetValue(InputDevice inputDevice)
		{
			switch (this.SourceType)
			{
			case InputControlSourceType.None:
				return 0f;
			case InputControlSourceType.Button:
				if (!this.GetState(inputDevice))
				{
					return 0f;
				}
				return 1f;
			case InputControlSourceType.Analog:
				return inputDevice.ReadRawAnalogValue(this.Index);
			case InputControlSourceType.KeyCode:
				if (!this.GetState(inputDevice))
				{
					return 0f;
				}
				return 1f;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06005068 RID: 20584 RVA: 0x00172B98 File Offset: 0x00170D98
		public bool GetState(InputDevice inputDevice)
		{
			switch (this.SourceType)
			{
			case InputControlSourceType.None:
				return false;
			case InputControlSourceType.Button:
				return inputDevice.ReadRawButtonState(this.Index);
			case InputControlSourceType.Analog:
				return Utility.IsNotZero(this.GetValue(inputDevice));
			case InputControlSourceType.KeyCode:
				return Input.GetKey((KeyCode)this.Index);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x00172BF4 File Offset: 0x00170DF4
		public string ToCode()
		{
			return string.Concat(new string[]
			{
				"new InputControlSource( InputControlSourceType.",
				this.SourceType.ToString(),
				", ",
				this.Index.ToString(),
				" )"
			});
		}

		// Token: 0x040050D1 RID: 20689
		[SerializeField]
		private InputControlSourceType sourceType;

		// Token: 0x040050D2 RID: 20690
		[SerializeField]
		private int index;
	}
}
