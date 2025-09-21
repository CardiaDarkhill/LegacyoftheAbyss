using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace InControl
{
	// Token: 0x020008E8 RID: 2280
	public abstract class PlayerActionSet
	{
		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x06004FBA RID: 20410 RVA: 0x0017134A File Offset: 0x0016F54A
		// (set) Token: 0x06004FBB RID: 20411 RVA: 0x00171352 File Offset: 0x0016F552
		public InputDevice Device { get; set; }

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06004FBC RID: 20412 RVA: 0x0017135B File Offset: 0x0016F55B
		// (set) Token: 0x06004FBD RID: 20413 RVA: 0x00171363 File Offset: 0x0016F563
		public List<InputDevice> IncludeDevices { get; private set; }

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06004FBE RID: 20414 RVA: 0x0017136C File Offset: 0x0016F56C
		// (set) Token: 0x06004FBF RID: 20415 RVA: 0x00171374 File Offset: 0x0016F574
		public List<InputDevice> ExcludeDevices { get; private set; }

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06004FC0 RID: 20416 RVA: 0x0017137D File Offset: 0x0016F57D
		// (set) Token: 0x06004FC1 RID: 20417 RVA: 0x00171385 File Offset: 0x0016F585
		public ReadOnlyCollection<PlayerAction> Actions { get; private set; }

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06004FC2 RID: 20418 RVA: 0x0017138E File Offset: 0x0016F58E
		// (set) Token: 0x06004FC3 RID: 20419 RVA: 0x00171396 File Offset: 0x0016F596
		public ulong UpdateTick { get; protected set; }

		// Token: 0x140000F6 RID: 246
		// (add) Token: 0x06004FC4 RID: 20420 RVA: 0x001713A0 File Offset: 0x0016F5A0
		// (remove) Token: 0x06004FC5 RID: 20421 RVA: 0x001713D8 File Offset: 0x0016F5D8
		public event Action<BindingSourceType> OnLastInputTypeChanged;

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06004FC6 RID: 20422 RVA: 0x0017140D File Offset: 0x0016F60D
		// (set) Token: 0x06004FC7 RID: 20423 RVA: 0x00171415 File Offset: 0x0016F615
		public bool Enabled { get; set; }

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06004FC8 RID: 20424 RVA: 0x0017141E File Offset: 0x0016F61E
		// (set) Token: 0x06004FC9 RID: 20425 RVA: 0x00171426 File Offset: 0x0016F626
		public bool PreventInputWhileListeningForBinding { get; set; }

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06004FCA RID: 20426 RVA: 0x0017142F File Offset: 0x0016F62F
		// (set) Token: 0x06004FCB RID: 20427 RVA: 0x00171437 File Offset: 0x0016F637
		public object UserData { get; set; }

		// Token: 0x06004FCC RID: 20428 RVA: 0x00171440 File Offset: 0x0016F640
		protected PlayerActionSet()
		{
			this.Enabled = true;
			this.PreventInputWhileListeningForBinding = true;
			this.Device = null;
			this.IncludeDevices = new List<InputDevice>();
			this.ExcludeDevices = new List<InputDevice>();
			this.Actions = new ReadOnlyCollection<PlayerAction>(this.actions);
			InputManager.AttachPlayerActionSet(this);
		}

		// Token: 0x06004FCD RID: 20429 RVA: 0x001714CC File Offset: 0x0016F6CC
		public void Destroy()
		{
			this.OnLastInputTypeChanged = null;
			InputManager.DetachPlayerActionSet(this);
		}

		// Token: 0x06004FCE RID: 20430 RVA: 0x001714DB File Offset: 0x0016F6DB
		protected PlayerAction CreatePlayerAction(string name)
		{
			return new PlayerAction(name, this);
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x001714E4 File Offset: 0x0016F6E4
		internal void AddPlayerAction(PlayerAction action)
		{
			action.Device = this.FindActiveDevice();
			if (this.actionsByName.ContainsKey(action.Name))
			{
				throw new InControlException("Action '" + action.Name + "' already exists in this set.");
			}
			this.actions.Add(action);
			this.actionsByName.Add(action.Name, action);
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x0017154C File Offset: 0x0016F74C
		protected PlayerOneAxisAction CreateOneAxisPlayerAction(PlayerAction negativeAction, PlayerAction positiveAction)
		{
			PlayerOneAxisAction playerOneAxisAction = new PlayerOneAxisAction(negativeAction, positiveAction);
			this.oneAxisActions.Add(playerOneAxisAction);
			return playerOneAxisAction;
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x00171570 File Offset: 0x0016F770
		protected PlayerTwoAxisAction CreateTwoAxisPlayerAction(PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction)
		{
			PlayerTwoAxisAction playerTwoAxisAction = new PlayerTwoAxisAction(negativeXAction, positiveXAction, negativeYAction, positiveYAction);
			this.twoAxisActions.Add(playerTwoAxisAction);
			return playerTwoAxisAction;
		}

		// Token: 0x17000A64 RID: 2660
		public PlayerAction this[string actionName]
		{
			get
			{
				PlayerAction result;
				if (this.actionsByName.TryGetValue(actionName, out result))
				{
					return result;
				}
				throw new KeyNotFoundException("Action '" + actionName + "' does not exist in this action set.");
			}
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x001715CC File Offset: 0x0016F7CC
		public PlayerAction GetPlayerActionByName(string actionName)
		{
			PlayerAction result;
			if (this.actionsByName.TryGetValue(actionName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x001715EC File Offset: 0x0016F7EC
		internal void Update(ulong updateTick, float deltaTime)
		{
			InputDevice device = this.Device ?? this.FindActiveDevice();
			BindingSourceType lastInputType = this.LastInputType;
			ulong lastInputTypeChangedTick = this.LastInputTypeChangedTick;
			InputDeviceClass lastDeviceClass = this.LastDeviceClass;
			InputDeviceStyle lastDeviceStyle = this.LastDeviceStyle;
			int count = this.actions.Count;
			for (int i = 0; i < count; i++)
			{
				PlayerAction playerAction = this.actions[i];
				playerAction.Update(updateTick, deltaTime, device);
				if (playerAction.UpdateTick > this.UpdateTick)
				{
					this.UpdateTick = playerAction.UpdateTick;
					this.activeDevice = playerAction.ActiveDevice;
				}
				if (playerAction.LastInputTypeChangedTick > lastInputTypeChangedTick)
				{
					lastInputType = playerAction.LastInputType;
					lastInputTypeChangedTick = playerAction.LastInputTypeChangedTick;
					lastDeviceClass = playerAction.LastDeviceClass;
					lastDeviceStyle = playerAction.LastDeviceStyle;
				}
			}
			int count2 = this.oneAxisActions.Count;
			for (int j = 0; j < count2; j++)
			{
				this.oneAxisActions[j].Update(updateTick, deltaTime);
			}
			int count3 = this.twoAxisActions.Count;
			for (int k = 0; k < count3; k++)
			{
				this.twoAxisActions[k].Update(updateTick, deltaTime);
			}
			if (lastInputTypeChangedTick > this.LastInputTypeChangedTick)
			{
				bool flag = lastInputType != this.LastInputType;
				this.LastInputType = lastInputType;
				this.LastInputTypeChangedTick = lastInputTypeChangedTick;
				this.LastDeviceClass = lastDeviceClass;
				this.LastDeviceStyle = lastDeviceStyle;
				if (this.OnLastInputTypeChanged != null && flag)
				{
					this.OnLastInputTypeChanged(lastInputType);
				}
			}
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x00171764 File Offset: 0x0016F964
		public void Reset()
		{
			int count = this.actions.Count;
			for (int i = 0; i < count; i++)
			{
				this.actions[i].ResetBindings();
			}
		}

		// Token: 0x06004FD6 RID: 20438 RVA: 0x0017179C File Offset: 0x0016F99C
		private InputDevice FindActiveDevice()
		{
			bool flag = this.IncludeDevices.Count > 0;
			bool flag2 = this.ExcludeDevices.Count > 0;
			if (flag || flag2)
			{
				InputDevice inputDevice = InputDevice.Null;
				int count = InputManager.Devices.Count;
				for (int i = 0; i < count; i++)
				{
					InputDevice inputDevice2 = InputManager.Devices[i];
					if (inputDevice2 != inputDevice && inputDevice2.LastInputAfter(inputDevice) && !inputDevice2.Passive && (!flag2 || !this.ExcludeDevices.Contains(inputDevice2)) && (!flag || this.IncludeDevices.Contains(inputDevice2)))
					{
						inputDevice = inputDevice2;
					}
				}
				return inputDevice;
			}
			return InputManager.ActiveDevice;
		}

		// Token: 0x06004FD7 RID: 20439 RVA: 0x00171844 File Offset: 0x0016FA44
		public void ClearInputState()
		{
			int count = this.actions.Count;
			for (int i = 0; i < count; i++)
			{
				this.actions[i].ClearInputState();
			}
			int count2 = this.oneAxisActions.Count;
			for (int j = 0; j < count2; j++)
			{
				this.oneAxisActions[j].ClearInputState();
			}
			int count3 = this.twoAxisActions.Count;
			for (int k = 0; k < count3; k++)
			{
				this.twoAxisActions[k].ClearInputState();
			}
		}

		// Token: 0x06004FD8 RID: 20440 RVA: 0x001718D8 File Offset: 0x0016FAD8
		public bool HasBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return false;
			}
			int count = this.actions.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.actions[i].HasBinding(binding))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004FD9 RID: 20441 RVA: 0x00171920 File Offset: 0x0016FB20
		public void RemoveBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return;
			}
			int count = this.actions.Count;
			for (int i = 0; i < count; i++)
			{
				this.actions[i].RemoveBinding(binding);
			}
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06004FDA RID: 20442 RVA: 0x00171961 File Offset: 0x0016FB61
		public bool IsListeningForBinding
		{
			get
			{
				return this.listenWithAction != null;
			}
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06004FDB RID: 20443 RVA: 0x0017196C File Offset: 0x0016FB6C
		// (set) Token: 0x06004FDC RID: 20444 RVA: 0x00171974 File Offset: 0x0016FB74
		public BindingListenOptions ListenOptions
		{
			get
			{
				return this.listenOptions;
			}
			set
			{
				this.listenOptions = (value ?? new BindingListenOptions());
			}
		}

		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06004FDD RID: 20445 RVA: 0x00171986 File Offset: 0x0016FB86
		public InputDevice ActiveDevice
		{
			get
			{
				return this.activeDevice ?? InputDevice.Null;
			}
		}

		// Token: 0x06004FDE RID: 20446 RVA: 0x00171998 File Offset: 0x0016FB98
		public byte[] SaveData()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8))
				{
					binaryWriter.Write(66);
					binaryWriter.Write(73);
					binaryWriter.Write(78);
					binaryWriter.Write(68);
					binaryWriter.Write(2);
					int count = this.actions.Count;
					binaryWriter.Write(count);
					for (int i = 0; i < count; i++)
					{
						this.actions[i].Save(binaryWriter);
					}
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x00171A50 File Offset: 0x0016FC50
		public void LoadData(byte[] data)
		{
			if (data == null)
			{
				return;
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						if (binaryReader.ReadUInt32() != 1145981250U)
						{
							throw new Exception("Unknown data format.");
						}
						ushort num = binaryReader.ReadUInt16();
						if (num < 1 || num > 2)
						{
							throw new Exception("Unknown data format version: " + num.ToString());
						}
						int num2 = binaryReader.ReadInt32();
						for (int i = 0; i < num2; i++)
						{
							PlayerAction playerAction;
							if (this.actionsByName.TryGetValue(binaryReader.ReadString(), out playerAction))
							{
								playerAction.Load(binaryReader, num);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError("Provided state could not be loaded:\n" + ex.Message);
				this.Reset();
			}
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x00171B48 File Offset: 0x0016FD48
		public string Save()
		{
			return Convert.ToBase64String(this.SaveData());
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x00171B58 File Offset: 0x0016FD58
		public void Load(string data)
		{
			if (data == null)
			{
				return;
			}
			try
			{
				this.LoadData(Convert.FromBase64String(data));
			}
			catch (Exception ex)
			{
				Logger.LogError("Provided state could not be loaded:\n" + ex.Message);
				this.Reset();
			}
		}

		// Token: 0x0400507B RID: 20603
		public BindingSourceType LastInputType;

		// Token: 0x0400507D RID: 20605
		public ulong LastInputTypeChangedTick;

		// Token: 0x0400507E RID: 20606
		public InputDeviceClass LastDeviceClass;

		// Token: 0x0400507F RID: 20607
		public InputDeviceStyle LastDeviceStyle;

		// Token: 0x04005083 RID: 20611
		private List<PlayerAction> actions = new List<PlayerAction>();

		// Token: 0x04005084 RID: 20612
		private List<PlayerOneAxisAction> oneAxisActions = new List<PlayerOneAxisAction>();

		// Token: 0x04005085 RID: 20613
		private List<PlayerTwoAxisAction> twoAxisActions = new List<PlayerTwoAxisAction>();

		// Token: 0x04005086 RID: 20614
		private Dictionary<string, PlayerAction> actionsByName = new Dictionary<string, PlayerAction>();

		// Token: 0x04005087 RID: 20615
		private BindingListenOptions listenOptions = new BindingListenOptions();

		// Token: 0x04005088 RID: 20616
		internal PlayerAction listenWithAction;

		// Token: 0x04005089 RID: 20617
		private InputDevice activeDevice;

		// Token: 0x0400508A RID: 20618
		private const ushort currentDataFormatVersion = 2;
	}
}
