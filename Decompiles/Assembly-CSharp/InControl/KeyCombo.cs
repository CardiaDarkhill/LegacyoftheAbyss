using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InControl
{
	// Token: 0x020008E3 RID: 2275
	public struct KeyCombo
	{
		// Token: 0x06004F52 RID: 20306 RVA: 0x0016FCE0 File Offset: 0x0016DEE0
		public KeyCombo(params Key[] keys)
		{
			this.includeData = 0UL;
			this.includeSize = 0;
			this.excludeData = 0UL;
			this.excludeSize = 0;
			for (int i = 0; i < keys.Length; i++)
			{
				this.AddInclude(keys[i]);
			}
		}

		// Token: 0x06004F53 RID: 20307 RVA: 0x0016FD22 File Offset: 0x0016DF22
		private void AddIncludeInt(int key)
		{
			if (this.includeSize == 8)
			{
				return;
			}
			this.includeData |= (ulong)((ulong)((long)key & 255L) << this.includeSize * 8);
			this.includeSize++;
		}

		// Token: 0x06004F54 RID: 20308 RVA: 0x0016FD5E File Offset: 0x0016DF5E
		private int GetIncludeInt(int index)
		{
			return (int)(this.includeData >> index * 8 & 255UL);
		}

		// Token: 0x06004F55 RID: 20309 RVA: 0x0016FD75 File Offset: 0x0016DF75
		[Obsolete("Use KeyCombo.AddInclude instead.")]
		public void Add(Key key)
		{
			this.AddInclude(key);
		}

		// Token: 0x06004F56 RID: 20310 RVA: 0x0016FD7E File Offset: 0x0016DF7E
		[Obsolete("Use KeyCombo.GetInclude instead.")]
		public Key Get(int index)
		{
			return this.GetInclude(index);
		}

		// Token: 0x06004F57 RID: 20311 RVA: 0x0016FD87 File Offset: 0x0016DF87
		public void AddInclude(Key key)
		{
			this.AddIncludeInt((int)key);
		}

		// Token: 0x06004F58 RID: 20312 RVA: 0x0016FD90 File Offset: 0x0016DF90
		public Key GetInclude(int index)
		{
			if (index < 0 || index >= this.includeSize)
			{
				throw new IndexOutOfRangeException("Index " + index.ToString() + " is out of the range 0.." + this.includeSize.ToString());
			}
			return (Key)this.GetIncludeInt(index);
		}

		// Token: 0x06004F59 RID: 20313 RVA: 0x0016FDCD File Offset: 0x0016DFCD
		private void AddExcludeInt(int key)
		{
			if (this.excludeSize == 8)
			{
				return;
			}
			this.excludeData |= (ulong)((ulong)((long)key & 255L) << this.excludeSize * 8);
			this.excludeSize++;
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x0016FE09 File Offset: 0x0016E009
		private int GetExcludeInt(int index)
		{
			return (int)(this.excludeData >> index * 8 & 255UL);
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x0016FE20 File Offset: 0x0016E020
		public void AddExclude(Key key)
		{
			this.AddExcludeInt((int)key);
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x0016FE29 File Offset: 0x0016E029
		public Key GetExclude(int index)
		{
			if (index < 0 || index >= this.excludeSize)
			{
				throw new IndexOutOfRangeException("Index " + index.ToString() + " is out of the range 0.." + this.excludeSize.ToString());
			}
			return (Key)this.GetExcludeInt(index);
		}

		// Token: 0x06004F5D RID: 20317 RVA: 0x0016FE66 File Offset: 0x0016E066
		public static KeyCombo With(params Key[] keys)
		{
			return new KeyCombo(keys);
		}

		// Token: 0x06004F5E RID: 20318 RVA: 0x0016FE70 File Offset: 0x0016E070
		public KeyCombo AndNot(params Key[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				this.AddExclude(keys[i]);
			}
			return this;
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x0016FE9A File Offset: 0x0016E09A
		public void Clear()
		{
			this.includeData = 0UL;
			this.includeSize = 0;
			this.excludeData = 0UL;
			this.excludeSize = 0;
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06004F60 RID: 20320 RVA: 0x0016FEBA File Offset: 0x0016E0BA
		[Obsolete("Use KeyCombo.IncludeCount instead.")]
		public int Count
		{
			get
			{
				return this.includeSize;
			}
		}

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06004F61 RID: 20321 RVA: 0x0016FEC2 File Offset: 0x0016E0C2
		public int IncludeCount
		{
			get
			{
				return this.includeSize;
			}
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06004F62 RID: 20322 RVA: 0x0016FECA File Offset: 0x0016E0CA
		public int ExcludeCount
		{
			get
			{
				return this.excludeSize;
			}
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06004F63 RID: 20323 RVA: 0x0016FED4 File Offset: 0x0016E0D4
		public bool IsPressed
		{
			get
			{
				if (this.includeSize == 0)
				{
					return false;
				}
				IKeyboardProvider keyboardProvider = InputManager.KeyboardProvider;
				bool flag = true;
				for (int i = 0; i < this.includeSize; i++)
				{
					Key include = this.GetInclude(i);
					flag = (flag && keyboardProvider.GetKeyIsPressed(include));
				}
				for (int j = 0; j < this.excludeSize; j++)
				{
					Key exclude = this.GetExclude(j);
					if (keyboardProvider.GetKeyIsPressed(exclude))
					{
						return false;
					}
				}
				return flag;
			}
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x0016FF48 File Offset: 0x0016E148
		public static KeyCombo Detect(bool modifiersAsKeys)
		{
			KeyCombo empty = KeyCombo.Empty;
			IKeyboardProvider keyboardProvider = InputManager.KeyboardProvider;
			if (keyboardProvider == null)
			{
				return empty;
			}
			if (modifiersAsKeys)
			{
				for (Key key = Key.LeftShift; key <= Key.RightControl; key++)
				{
					if (keyboardProvider.GetKeyIsPressed(key))
					{
						empty.AddInclude(key);
						if (key == Key.LeftControl && keyboardProvider.GetKeyIsPressed(Key.RightAlt))
						{
							empty.AddInclude(Key.RightAlt);
						}
						return empty;
					}
				}
			}
			else
			{
				for (Key key2 = Key.Shift; key2 <= Key.Control; key2++)
				{
					if (keyboardProvider.GetKeyIsPressed(key2))
					{
						empty.AddInclude(key2);
					}
				}
			}
			for (Key key3 = Key.Escape; key3 <= Key.QuestionMark; key3++)
			{
				if (keyboardProvider.GetKeyIsPressed(key3))
				{
					empty.AddInclude(key3);
					return empty;
				}
			}
			empty.Clear();
			return empty;
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x0016FFF4 File Offset: 0x0016E1F4
		public override string ToString()
		{
			string text;
			if (!KeyCombo.cachedStrings.TryGetValue(this.includeData, out text))
			{
				KeyCombo.cachedStringBuilder.Clear();
				for (int i = 0; i < this.includeSize; i++)
				{
					if (i != 0)
					{
						KeyCombo.cachedStringBuilder.Append(" ");
					}
					Key include = this.GetInclude(i);
					KeyCombo.cachedStringBuilder.Append(InputManager.KeyboardProvider.GetNameForKey(include));
				}
				text = KeyCombo.cachedStringBuilder.ToString();
				KeyCombo.cachedStrings[this.includeData] = text;
			}
			return text;
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x0017007F File Offset: 0x0016E27F
		public static bool operator ==(KeyCombo a, KeyCombo b)
		{
			return a.includeData == b.includeData && a.excludeData == b.excludeData;
		}

		// Token: 0x06004F67 RID: 20327 RVA: 0x0017009F File Offset: 0x0016E29F
		public static bool operator !=(KeyCombo a, KeyCombo b)
		{
			return a.includeData != b.includeData || a.excludeData != b.excludeData;
		}

		// Token: 0x06004F68 RID: 20328 RVA: 0x001700C4 File Offset: 0x0016E2C4
		public override bool Equals(object other)
		{
			if (other is KeyCombo)
			{
				KeyCombo keyCombo = (KeyCombo)other;
				return this.includeData == keyCombo.includeData && this.excludeData == keyCombo.excludeData;
			}
			return false;
		}

		// Token: 0x06004F69 RID: 20329 RVA: 0x00170100 File Offset: 0x0016E300
		public override int GetHashCode()
		{
			return (17 * 31 + this.includeData.GetHashCode()) * 31 + this.excludeData.GetHashCode();
		}

		// Token: 0x06004F6A RID: 20330 RVA: 0x00170124 File Offset: 0x0016E324
		internal void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			if (dataFormatVersion == 1)
			{
				this.includeSize = reader.ReadInt32();
				this.includeData = reader.ReadUInt64();
				return;
			}
			if (dataFormatVersion != 2)
			{
				throw new InControlException("Unknown data format version: " + dataFormatVersion.ToString());
			}
			this.includeSize = reader.ReadInt32();
			this.includeData = reader.ReadUInt64();
			this.excludeSize = reader.ReadInt32();
			this.excludeData = reader.ReadUInt64();
		}

		// Token: 0x06004F6B RID: 20331 RVA: 0x0017019B File Offset: 0x0016E39B
		internal void Save(BinaryWriter writer)
		{
			writer.Write(this.includeSize);
			writer.Write(this.includeData);
			writer.Write(this.excludeSize);
			writer.Write(this.excludeData);
		}

		// Token: 0x04005042 RID: 20546
		public static readonly KeyCombo Empty = default(KeyCombo);

		// Token: 0x04005043 RID: 20547
		private int includeSize;

		// Token: 0x04005044 RID: 20548
		private ulong includeData;

		// Token: 0x04005045 RID: 20549
		private int excludeSize;

		// Token: 0x04005046 RID: 20550
		private ulong excludeData;

		// Token: 0x04005047 RID: 20551
		private static readonly Dictionary<ulong, string> cachedStrings = new Dictionary<ulong, string>();

		// Token: 0x04005048 RID: 20552
		private static readonly StringBuilder cachedStringBuilder = new StringBuilder(256);
	}
}
