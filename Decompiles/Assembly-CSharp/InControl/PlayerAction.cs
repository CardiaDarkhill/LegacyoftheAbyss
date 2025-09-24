using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008E7 RID: 2279
	public class PlayerAction : OneAxisInputControl
	{
		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06004F88 RID: 20360 RVA: 0x001705A2 File Offset: 0x0016E7A2
		// (set) Token: 0x06004F89 RID: 20361 RVA: 0x001705AA File Offset: 0x0016E7AA
		public string Name { get; private set; }

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06004F8A RID: 20362 RVA: 0x001705B3 File Offset: 0x0016E7B3
		// (set) Token: 0x06004F8B RID: 20363 RVA: 0x001705BB File Offset: 0x0016E7BB
		public PlayerActionSet Owner { get; private set; }

		// Token: 0x140000F4 RID: 244
		// (add) Token: 0x06004F8C RID: 20364 RVA: 0x001705C4 File Offset: 0x0016E7C4
		// (remove) Token: 0x06004F8D RID: 20365 RVA: 0x001705FC File Offset: 0x0016E7FC
		public event Action<BindingSourceType> OnLastInputTypeChanged;

		// Token: 0x140000F5 RID: 245
		// (add) Token: 0x06004F8E RID: 20366 RVA: 0x00170634 File Offset: 0x0016E834
		// (remove) Token: 0x06004F8F RID: 20367 RVA: 0x0017066C File Offset: 0x0016E86C
		public event Action OnBindingsChanged;

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06004F90 RID: 20368 RVA: 0x001706A1 File Offset: 0x0016E8A1
		// (set) Token: 0x06004F91 RID: 20369 RVA: 0x001706A9 File Offset: 0x0016E8A9
		public object UserData { get; set; }

		// Token: 0x06004F92 RID: 20370 RVA: 0x001706B4 File Offset: 0x0016E8B4
		public PlayerAction(string name, PlayerActionSet owner)
		{
			this.Raw = true;
			this.Name = name;
			this.Owner = owner;
			this.bindings = new ReadOnlyCollection<BindingSource>(this.visibleBindings);
			this.unfilteredBindings = new ReadOnlyCollection<BindingSource>(this.regularBindings);
			owner.AddPlayerAction(this);
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x00170754 File Offset: 0x0016E954
		public void AddDefaultBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return;
			}
			if (binding.BoundTo != null)
			{
				throw new InControlException("Binding source is already bound to action " + binding.BoundTo.Name);
			}
			if (!this.defaultBindings.Contains(binding))
			{
				this.defaultBindings.Add(binding);
				binding.BoundTo = this;
			}
			if (!this.regularBindings.Contains(binding))
			{
				this.regularBindings.Add(binding);
				binding.BoundTo = this;
				if (binding.IsValid)
				{
					this.visibleBindings.Add(binding);
				}
			}
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x001707E4 File Offset: 0x0016E9E4
		public void AddDefaultBinding(params Key[] keys)
		{
			this.AddDefaultBinding(new KeyBindingSource(keys));
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x001707F2 File Offset: 0x0016E9F2
		public void AddDefaultBinding(KeyCombo keyCombo)
		{
			this.AddDefaultBinding(new KeyBindingSource(keyCombo));
		}

		// Token: 0x06004F96 RID: 20374 RVA: 0x00170800 File Offset: 0x0016EA00
		public void AddDefaultBinding(Mouse control)
		{
			this.AddDefaultBinding(new MouseBindingSource(control));
		}

		// Token: 0x06004F97 RID: 20375 RVA: 0x0017080E File Offset: 0x0016EA0E
		public void AddDefaultBinding(InputControlType control)
		{
			this.AddDefaultBinding(new DeviceBindingSource(control));
		}

		// Token: 0x06004F98 RID: 20376 RVA: 0x0017081C File Offset: 0x0016EA1C
		public bool AddBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return false;
			}
			if (binding.BoundTo != null)
			{
				return false;
			}
			if (this.regularBindings.Contains(binding))
			{
				return false;
			}
			this.regularBindings.Add(binding);
			binding.BoundTo = this;
			if (binding.IsValid)
			{
				this.visibleBindings.Add(binding);
			}
			this.triggerBindingChanged = true;
			return true;
		}

		// Token: 0x06004F99 RID: 20377 RVA: 0x00170880 File Offset: 0x0016EA80
		public bool InsertBindingAt(int index, BindingSource binding)
		{
			if (index < 0 || index > this.visibleBindings.Count)
			{
				throw new InControlException("Index is out of range for bindings on this action.");
			}
			if (index == this.visibleBindings.Count)
			{
				return this.AddBinding(binding);
			}
			if (binding == null)
			{
				return false;
			}
			if (binding.BoundTo != null)
			{
				Logger.LogWarning("Binding source is already bound to action " + binding.BoundTo.Name);
				return false;
			}
			if (this.regularBindings.Contains(binding))
			{
				return false;
			}
			int index2 = (index == 0) ? 0 : this.regularBindings.IndexOf(this.visibleBindings[index]);
			this.regularBindings.Insert(index2, binding);
			binding.BoundTo = this;
			if (binding.IsValid)
			{
				this.visibleBindings.Insert(index, binding);
			}
			this.triggerBindingChanged = true;
			return true;
		}

		// Token: 0x06004F9A RID: 20378 RVA: 0x00170950 File Offset: 0x0016EB50
		public bool ReplaceBinding(BindingSource findBinding, BindingSource withBinding)
		{
			if (findBinding == null || withBinding == null)
			{
				return false;
			}
			if (withBinding.BoundTo != null)
			{
				Logger.LogWarning("Binding source is already bound to action " + withBinding.BoundTo.Name);
				return false;
			}
			int num = this.regularBindings.IndexOf(findBinding);
			if (num < 0)
			{
				Logger.LogWarning("Binding source to replace is not present in this action.");
				return false;
			}
			findBinding.BoundTo = null;
			this.regularBindings[num] = withBinding;
			withBinding.BoundTo = this;
			num = this.visibleBindings.IndexOf(findBinding);
			if (num >= 0)
			{
				this.visibleBindings[num] = withBinding;
			}
			this.triggerBindingChanged = true;
			return true;
		}

		// Token: 0x06004F9B RID: 20379 RVA: 0x001709F4 File Offset: 0x0016EBF4
		public bool HasBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return false;
			}
			BindingSource bindingSource = this.FindBinding(binding);
			return !(bindingSource == null) && bindingSource.BoundTo == this;
		}

		// Token: 0x06004F9C RID: 20380 RVA: 0x00170A28 File Offset: 0x0016EC28
		public BindingSource FindBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return null;
			}
			int num = this.regularBindings.IndexOf(binding);
			if (num >= 0)
			{
				return this.regularBindings[num];
			}
			return null;
		}

		// Token: 0x06004F9D RID: 20381 RVA: 0x00170A60 File Offset: 0x0016EC60
		private void HardRemoveBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return;
			}
			int num = this.regularBindings.IndexOf(binding);
			if (num >= 0)
			{
				BindingSource bindingSource = this.regularBindings[num];
				if (bindingSource.BoundTo == this)
				{
					bindingSource.BoundTo = null;
					this.regularBindings.RemoveAt(num);
					this.UpdateVisibleBindings();
					this.triggerBindingChanged = true;
				}
			}
		}

		// Token: 0x06004F9E RID: 20382 RVA: 0x00170AC0 File Offset: 0x0016ECC0
		public void RemoveBinding(BindingSource binding)
		{
			BindingSource bindingSource = this.FindBinding(binding);
			if (bindingSource != null && bindingSource.BoundTo == this)
			{
				bindingSource.BoundTo = null;
				this.triggerBindingChanged = true;
			}
		}

		// Token: 0x06004F9F RID: 20383 RVA: 0x00170AF5 File Offset: 0x0016ECF5
		public void RemoveBindingAt(int index)
		{
			if (index < 0 || index >= this.regularBindings.Count)
			{
				throw new InControlException("Index is out of range for bindings on this action.");
			}
			this.regularBindings[index].BoundTo = null;
			this.triggerBindingChanged = true;
		}

		// Token: 0x06004FA0 RID: 20384 RVA: 0x00170B30 File Offset: 0x0016ED30
		private int CountBindingsOfType(BindingSourceType bindingSourceType)
		{
			int num = 0;
			int count = this.regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = this.regularBindings[i];
				if (bindingSource.BoundTo == this && bindingSource.BindingSourceType == bindingSourceType)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06004FA1 RID: 20385 RVA: 0x00170B7C File Offset: 0x0016ED7C
		private void RemoveFirstBindingOfType(BindingSourceType bindingSourceType)
		{
			int count = this.regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = this.regularBindings[i];
				if (bindingSource.BoundTo == this && bindingSource.BindingSourceType == bindingSourceType)
				{
					bindingSource.BoundTo = null;
					this.regularBindings.RemoveAt(i);
					this.triggerBindingChanged = true;
					return;
				}
			}
		}

		// Token: 0x06004FA2 RID: 20386 RVA: 0x00170BDC File Offset: 0x0016EDDC
		private int IndexOfFirstInvalidBinding()
		{
			int count = this.regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.regularBindings[i].IsValid)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004FA3 RID: 20387 RVA: 0x00170C18 File Offset: 0x0016EE18
		public void ClearBindings()
		{
			int count = this.regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				this.regularBindings[i].BoundTo = null;
			}
			this.regularBindings.Clear();
			this.visibleBindings.Clear();
			this.triggerBindingChanged = true;
		}

		// Token: 0x06004FA4 RID: 20388 RVA: 0x00170C6C File Offset: 0x0016EE6C
		public void ResetBindings()
		{
			this.ClearBindings();
			this.regularBindings.AddRange(this.defaultBindings);
			int count = this.regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = this.regularBindings[i];
				bindingSource.BoundTo = this;
				if (bindingSource.IsValid)
				{
					this.visibleBindings.Add(bindingSource);
				}
			}
			this.triggerBindingChanged = true;
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x00170CD7 File Offset: 0x0016EED7
		public void ListenForBinding()
		{
			this.ListenForBindingReplacing(null);
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x00170CE0 File Offset: 0x0016EEE0
		public void ListenForBindingReplacing(BindingSource binding)
		{
			(this.ListenOptions ?? this.Owner.ListenOptions).ReplaceBinding = binding;
			this.Owner.listenWithAction = this;
			int num = this.bindingSourceListeners.Length;
			for (int i = 0; i < num; i++)
			{
				this.bindingSourceListeners[i].Reset();
			}
		}

		// Token: 0x06004FA7 RID: 20391 RVA: 0x00170D36 File Offset: 0x0016EF36
		public void StopListeningForBinding()
		{
			if (this.IsListeningForBinding)
			{
				this.Owner.listenWithAction = null;
				this.triggerBindingEnded = true;
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06004FA8 RID: 20392 RVA: 0x00170D53 File Offset: 0x0016EF53
		public bool IsListeningForBinding
		{
			get
			{
				return this.Owner.listenWithAction == this;
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06004FA9 RID: 20393 RVA: 0x00170D63 File Offset: 0x0016EF63
		public ReadOnlyCollection<BindingSource> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06004FAA RID: 20394 RVA: 0x00170D6B File Offset: 0x0016EF6B
		public ReadOnlyCollection<BindingSource> UnfilteredBindings
		{
			get
			{
				return this.unfilteredBindings;
			}
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x00170D74 File Offset: 0x0016EF74
		private void RemoveOrphanedBindings()
		{
			for (int i = this.regularBindings.Count - 1; i >= 0; i--)
			{
				if (this.regularBindings[i].BoundTo != this)
				{
					this.regularBindings.RemoveAt(i);
				}
			}
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x00170DBC File Offset: 0x0016EFBC
		internal void Update(ulong updateTick, float deltaTime, InputDevice device)
		{
			this.Device = device;
			this.UpdateBindings(updateTick, deltaTime);
			if (this.triggerBindingChanged)
			{
				if (this.OnBindingsChanged != null)
				{
					this.OnBindingsChanged();
				}
				this.triggerBindingChanged = false;
			}
			if (this.triggerBindingEnded)
			{
				(this.ListenOptions ?? this.Owner.ListenOptions).CallOnBindingEnded(this);
				this.triggerBindingEnded = false;
			}
			this.DetectBindings();
		}

		// Token: 0x06004FAD RID: 20397 RVA: 0x00170E2C File Offset: 0x0016F02C
		private void UpdateBindings(ulong updateTick, float deltaTime)
		{
			bool flag = this.IsListeningForBinding || (this.Owner.IsListeningForBinding && this.Owner.PreventInputWhileListeningForBinding);
			BindingSourceType bindingSourceType = this.LastInputType;
			ulong num = this.LastInputTypeChangedTick;
			ulong updateTick2 = base.UpdateTick;
			InputDeviceClass lastDeviceClass = this.LastDeviceClass;
			InputDeviceStyle lastDeviceStyle = this.LastDeviceStyle;
			int count = this.regularBindings.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				BindingSource bindingSource = this.regularBindings[i];
				if (bindingSource.BoundTo != this)
				{
					this.regularBindings.RemoveAt(i);
					this.visibleBindings.Remove(bindingSource);
					this.triggerBindingChanged = true;
				}
				else if (!flag)
				{
					float value = bindingSource.GetValue(this.Device);
					if (base.UpdateWithValue(value, updateTick, deltaTime))
					{
						bindingSourceType = bindingSource.BindingSourceType;
						num = updateTick;
						lastDeviceClass = bindingSource.DeviceClass;
						lastDeviceStyle = bindingSource.DeviceStyle;
					}
				}
			}
			if (flag || count == 0)
			{
				base.UpdateWithValue(0f, updateTick, deltaTime);
			}
			base.Commit();
			this.ownerEnabled = this.Owner.Enabled;
			if (num > this.LastInputTypeChangedTick && (bindingSourceType != BindingSourceType.MouseBindingSource || Utility.Abs(base.LastValue - base.Value) >= MouseBindingSource.JitterThreshold))
			{
				bool flag2 = bindingSourceType != this.LastInputType;
				this.LastInputType = bindingSourceType;
				this.LastInputTypeChangedTick = num;
				this.LastDeviceClass = lastDeviceClass;
				this.LastDeviceStyle = lastDeviceStyle;
				if (this.OnLastInputTypeChanged != null && flag2)
				{
					this.OnLastInputTypeChanged(bindingSourceType);
				}
			}
			if (base.UpdateTick > updateTick2)
			{
				this.activeDevice = (this.LastInputTypeIsDevice ? this.Device : null);
			}
		}

		// Token: 0x06004FAE RID: 20398 RVA: 0x00170FD8 File Offset: 0x0016F1D8
		private void DetectBindings()
		{
			if (this.IsListeningForBinding)
			{
				BindingSource bindingSource = null;
				BindingListenOptions bindingListenOptions = this.ListenOptions ?? this.Owner.ListenOptions;
				int num = this.bindingSourceListeners.Length;
				for (int i = 0; i < num; i++)
				{
					bindingSource = this.bindingSourceListeners[i].Listen(bindingListenOptions, this.device);
					if (bindingSource != null)
					{
						break;
					}
				}
				if (bindingSource == null)
				{
					return;
				}
				if (!bindingListenOptions.CallOnBindingFound(this, bindingSource))
				{
					return;
				}
				if (this.HasBinding(bindingSource))
				{
					if (bindingListenOptions.RejectRedundantBindings)
					{
						bindingListenOptions.CallOnBindingRejected(this, bindingSource, BindingSourceRejectionType.DuplicateBindingOnAction);
						return;
					}
					this.StopListeningForBinding();
					bindingListenOptions.CallOnBindingAdded(this, bindingSource);
					return;
				}
				else
				{
					if (bindingListenOptions.UnsetDuplicateBindingsOnSet)
					{
						int count = this.Owner.Actions.Count;
						for (int j = 0; j < count; j++)
						{
							this.Owner.Actions[j].HardRemoveBinding(bindingSource);
						}
					}
					if (!bindingListenOptions.AllowDuplicateBindingsPerSet && this.Owner.HasBinding(bindingSource))
					{
						bindingListenOptions.CallOnBindingRejected(this, bindingSource, BindingSourceRejectionType.DuplicateBindingOnActionSet);
						return;
					}
					this.StopListeningForBinding();
					if (bindingListenOptions.ReplaceBinding == null)
					{
						if (bindingListenOptions.MaxAllowedBindingsPerType > 0U)
						{
							while ((long)this.CountBindingsOfType(bindingSource.BindingSourceType) >= (long)((ulong)bindingListenOptions.MaxAllowedBindingsPerType))
							{
								this.RemoveFirstBindingOfType(bindingSource.BindingSourceType);
							}
						}
						else if (bindingListenOptions.MaxAllowedBindings > 0U)
						{
							while ((long)this.regularBindings.Count >= (long)((ulong)bindingListenOptions.MaxAllowedBindings))
							{
								int index = Mathf.Max(0, this.IndexOfFirstInvalidBinding());
								this.regularBindings.RemoveAt(index);
								this.triggerBindingChanged = true;
							}
						}
						this.AddBinding(bindingSource);
					}
					else
					{
						this.ReplaceBinding(bindingListenOptions.ReplaceBinding, bindingSource);
					}
					this.UpdateVisibleBindings();
					bindingListenOptions.CallOnBindingAdded(this, bindingSource);
				}
			}
		}

		// Token: 0x06004FAF RID: 20399 RVA: 0x00171188 File Offset: 0x0016F388
		private void UpdateVisibleBindings()
		{
			this.visibleBindings.Clear();
			int count = this.regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = this.regularBindings[i];
				if (bindingSource.IsValid)
				{
					this.visibleBindings.Add(bindingSource);
				}
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06004FB0 RID: 20400 RVA: 0x001711D9 File Offset: 0x0016F3D9
		// (set) Token: 0x06004FB1 RID: 20401 RVA: 0x00171200 File Offset: 0x0016F400
		internal InputDevice Device
		{
			get
			{
				if (this.device == null)
				{
					this.device = this.Owner.Device;
					this.UpdateVisibleBindings();
				}
				return this.device;
			}
			set
			{
				if (this.device != value)
				{
					this.device = value;
					this.UpdateVisibleBindings();
				}
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06004FB2 RID: 20402 RVA: 0x00171218 File Offset: 0x0016F418
		public InputDevice ActiveDevice
		{
			get
			{
				return this.activeDevice ?? InputDevice.Null;
			}
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06004FB3 RID: 20403 RVA: 0x00171229 File Offset: 0x0016F429
		private bool LastInputTypeIsDevice
		{
			get
			{
				return this.LastInputType == BindingSourceType.DeviceBindingSource || this.LastInputType == BindingSourceType.UnknownDeviceBindingSource;
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06004FB4 RID: 20404 RVA: 0x0017123F File Offset: 0x0016F43F
		// (set) Token: 0x06004FB5 RID: 20405 RVA: 0x00171246 File Offset: 0x0016F446
		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float LowerDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06004FB6 RID: 20406 RVA: 0x00171248 File Offset: 0x0016F448
		// (set) Token: 0x06004FB7 RID: 20407 RVA: 0x0017124F File Offset: 0x0016F44F
		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float UpperDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		// Token: 0x06004FB8 RID: 20408 RVA: 0x00171254 File Offset: 0x0016F454
		internal void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			this.ClearBindings();
			int num = reader.ReadInt32();
			int i = 0;
			while (i < num)
			{
				BindingSourceType bindingSourceType = (BindingSourceType)reader.ReadInt32();
				BindingSource bindingSource;
				switch (bindingSourceType)
				{
				case BindingSourceType.None:
					IL_81:
					i++;
					continue;
				case BindingSourceType.DeviceBindingSource:
					bindingSource = new DeviceBindingSource();
					break;
				case BindingSourceType.KeyBindingSource:
					bindingSource = new KeyBindingSource();
					break;
				case BindingSourceType.MouseBindingSource:
					bindingSource = new MouseBindingSource();
					break;
				case BindingSourceType.UnknownDeviceBindingSource:
					bindingSource = new UnknownDeviceBindingSource();
					break;
				default:
					throw new InControlException("Don't know how to load BindingSourceType: " + bindingSourceType.ToString());
				}
				bindingSource.Load(reader, dataFormatVersion);
				this.AddBinding(bindingSource);
				goto IL_81;
			}
		}

		// Token: 0x06004FB9 RID: 20409 RVA: 0x001712EC File Offset: 0x0016F4EC
		internal void Save(BinaryWriter writer)
		{
			this.RemoveOrphanedBindings();
			writer.Write(this.Name);
			int count = this.regularBindings.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = this.regularBindings[i];
				writer.Write((int)bindingSource.BindingSourceType);
				bindingSource.Save(writer);
			}
		}

		// Token: 0x04005064 RID: 20580
		public BindingListenOptions ListenOptions;

		// Token: 0x04005065 RID: 20581
		public BindingSourceType LastInputType;

		// Token: 0x04005067 RID: 20583
		public ulong LastInputTypeChangedTick;

		// Token: 0x04005068 RID: 20584
		public InputDeviceClass LastDeviceClass;

		// Token: 0x04005069 RID: 20585
		public InputDeviceStyle LastDeviceStyle;

		// Token: 0x0400506C RID: 20588
		private readonly List<BindingSource> defaultBindings = new List<BindingSource>();

		// Token: 0x0400506D RID: 20589
		private readonly List<BindingSource> regularBindings = new List<BindingSource>();

		// Token: 0x0400506E RID: 20590
		private readonly List<BindingSource> visibleBindings = new List<BindingSource>();

		// Token: 0x0400506F RID: 20591
		private readonly ReadOnlyCollection<BindingSource> bindings;

		// Token: 0x04005070 RID: 20592
		private readonly ReadOnlyCollection<BindingSource> unfilteredBindings;

		// Token: 0x04005071 RID: 20593
		private readonly BindingSourceListener[] bindingSourceListeners = new BindingSourceListener[]
		{
			new DeviceBindingSourceListener(),
			new UnknownDeviceBindingSourceListener(),
			new KeyBindingSourceListener(),
			new MouseBindingSourceListener()
		};

		// Token: 0x04005072 RID: 20594
		private bool triggerBindingEnded;

		// Token: 0x04005073 RID: 20595
		private bool triggerBindingChanged;

		// Token: 0x04005074 RID: 20596
		private InputDevice device;

		// Token: 0x04005075 RID: 20597
		private InputDevice activeDevice;
	}
}
