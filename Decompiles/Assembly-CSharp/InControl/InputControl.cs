using System;

namespace InControl
{
	// Token: 0x020008F3 RID: 2291
	public class InputControl : OneAxisInputControl
	{
		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06005036 RID: 20534 RVA: 0x00172812 File Offset: 0x00170A12
		// (set) Token: 0x06005037 RID: 20535 RVA: 0x0017281A File Offset: 0x00170A1A
		public string Handle { get; protected set; }

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06005038 RID: 20536 RVA: 0x00172823 File Offset: 0x00170A23
		// (set) Token: 0x06005039 RID: 20537 RVA: 0x0017282B File Offset: 0x00170A2B
		public InputControlType Target { get; protected set; }

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x0600503A RID: 20538 RVA: 0x00172834 File Offset: 0x00170A34
		// (set) Token: 0x0600503B RID: 20539 RVA: 0x0017283C File Offset: 0x00170A3C
		public bool IsButton { get; protected set; }

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x0600503C RID: 20540 RVA: 0x00172845 File Offset: 0x00170A45
		// (set) Token: 0x0600503D RID: 20541 RVA: 0x0017284D File Offset: 0x00170A4D
		public bool IsAnalog { get; protected set; }

		// Token: 0x0600503E RID: 20542 RVA: 0x00172856 File Offset: 0x00170A56
		private InputControl()
		{
			this.Handle = "None";
			this.Target = InputControlType.None;
			this.Passive = false;
			this.IsButton = false;
			this.IsAnalog = false;
		}

		// Token: 0x0600503F RID: 20543 RVA: 0x00172885 File Offset: 0x00170A85
		public InputControl(string handle, InputControlType target)
		{
			this.Handle = handle;
			this.Target = target;
			this.Passive = false;
			this.IsButton = Utility.TargetIsButton(target);
			this.IsAnalog = !this.IsButton;
		}

		// Token: 0x06005040 RID: 20544 RVA: 0x001728BD File Offset: 0x00170ABD
		public InputControl(string handle, InputControlType target, bool passive) : this(handle, target)
		{
			this.Passive = passive;
		}

		// Token: 0x06005041 RID: 20545 RVA: 0x001728CE File Offset: 0x00170ACE
		internal void SetZeroTick()
		{
			this.zeroTick = base.UpdateTick;
		}

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06005042 RID: 20546 RVA: 0x001728DC File Offset: 0x00170ADC
		internal bool IsOnZeroTick
		{
			get
			{
				return base.UpdateTick == this.zeroTick;
			}
		}

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06005043 RID: 20547 RVA: 0x001728EC File Offset: 0x00170AEC
		public bool IsStandard
		{
			get
			{
				return Utility.TargetIsStandard(this.Target);
			}
		}

		// Token: 0x040050B8 RID: 20664
		public static readonly InputControl Null = new InputControl
		{
			isNullControl = true
		};

		// Token: 0x040050BB RID: 20667
		public bool Passive;

		// Token: 0x040050BE RID: 20670
		private ulong zeroTick;
	}
}
