using System;
using System.Reflection;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001E5 RID: 485
[Serializable]
public class PlayerDataTest
{
	// Token: 0x060012B8 RID: 4792 RVA: 0x00056926 File Offset: 0x00054B26
	public PlayerDataTest()
	{
		this.playerDataOverride = null;
		this.TestGroups = Array.Empty<PlayerDataTest.TestGroup>();
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x00056940 File Offset: 0x00054B40
	public PlayerDataTest(PlayerDataBase playerDataOverride)
	{
		this.playerDataOverride = playerDataOverride;
		this.TestGroups = Array.Empty<PlayerDataTest.TestGroup>();
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x0005695C File Offset: 0x00054B5C
	public PlayerDataTest(string fieldName, bool value)
	{
		this.playerDataOverride = null;
		this.TestGroups = new PlayerDataTest.TestGroup[]
		{
			new PlayerDataTest.TestGroup
			{
				Tests = new PlayerDataTest.Test[]
				{
					new PlayerDataTest.Test
					{
						FieldName = fieldName,
						Type = PlayerDataTest.TestType.Bool,
						BoolValue = value
					}
				}
			}
		};
	}

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x060012BB RID: 4795 RVA: 0x000569C8 File Offset: 0x00054BC8
	public bool IsFulfilled
	{
		get
		{
			if (this.TestGroups.Length == 0)
			{
				return true;
			}
			foreach (PlayerDataTest.TestGroup testGroup in this.TestGroups)
			{
				if (testGroup.IsFulfilled(this.playerDataOverride ?? GameManager.instance.playerData))
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x060012BC RID: 4796 RVA: 0x00056A20 File Offset: 0x00054C20
	public bool IsDefined
	{
		get
		{
			PlayerDataTest.TestGroup[] testGroups = this.TestGroups;
			for (int i = 0; i < testGroups.Length; i++)
			{
				if (testGroups[i].Tests.Length != 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x04001176 RID: 4470
	private readonly PlayerDataBase playerDataOverride;

	// Token: 0x04001177 RID: 4471
	[Tooltip("If any single test group succeeds, test will be passed.")]
	public PlayerDataTest.TestGroup[] TestGroups;

	// Token: 0x02001521 RID: 5409
	public enum TestType
	{
		// Token: 0x04008607 RID: 34311
		Bool,
		// Token: 0x04008608 RID: 34312
		Int,
		// Token: 0x04008609 RID: 34313
		Float,
		// Token: 0x0400860A RID: 34314
		Enum,
		// Token: 0x0400860B RID: 34315
		String
	}

	// Token: 0x02001522 RID: 5410
	public enum NumTestType
	{
		// Token: 0x0400860D RID: 34317
		Equal,
		// Token: 0x0400860E RID: 34318
		NotEqual,
		// Token: 0x0400860F RID: 34319
		LessThan,
		// Token: 0x04008610 RID: 34320
		MoreThan
	}

	// Token: 0x02001523 RID: 5411
	public enum StringTestType
	{
		// Token: 0x04008612 RID: 34322
		Equal,
		// Token: 0x04008613 RID: 34323
		NotEqual,
		// Token: 0x04008614 RID: 34324
		Contains,
		// Token: 0x04008615 RID: 34325
		NotContains
	}

	// Token: 0x02001524 RID: 5412
	[Serializable]
	public struct Test
	{
		// Token: 0x060085DE RID: 34270 RVA: 0x002714A0 File Offset: 0x0026F6A0
		public bool IsFulfilled(PlayerDataBase playerData)
		{
			float? num = null;
			float num2 = 0f;
			switch (this.Type)
			{
			case PlayerDataTest.TestType.Bool:
				return playerData.GetVariable(this.FieldName) == this.BoolValue;
			case PlayerDataTest.TestType.Int:
				num = new float?((float)playerData.GetVariable(this.FieldName));
				num2 = (float)this.IntValue;
				break;
			case PlayerDataTest.TestType.Float:
				num = new float?(playerData.GetVariable(this.FieldName));
				num2 = this.FloatValue;
				break;
			case PlayerDataTest.TestType.Enum:
			{
				FieldInfo field = playerData.GetType().GetField(this.FieldName);
				if (field.FieldType.IsEnum)
				{
					num = new float?((float)((int)field.GetValue(playerData)));
				}
				else
				{
					Debug.LogErrorFormat("Could not test field {0} is it was not an enum type", new object[]
					{
						this.FieldName
					});
				}
				num2 = (float)this.IntValue;
				break;
			}
			case PlayerDataTest.TestType.String:
			{
				string text = playerData.GetVariable(this.FieldName) ?? string.Empty;
				switch (this.StringType)
				{
				case PlayerDataTest.StringTestType.Equal:
					return text.Equals(this.StringValue);
				case PlayerDataTest.StringTestType.NotEqual:
					return !text.Equals(this.StringValue);
				case PlayerDataTest.StringTestType.Contains:
					return text.Contains(this.StringValue);
				case PlayerDataTest.StringTestType.NotContains:
					return !text.Contains(this.StringValue);
				}
				break;
			}
			}
			if (num == null)
			{
				return false;
			}
			bool result;
			switch (this.NumType)
			{
			case PlayerDataTest.NumTestType.Equal:
			{
				float? num3 = num;
				float num4 = num2;
				result = (num3.GetValueOrDefault() == num4 & num3 != null);
				break;
			}
			case PlayerDataTest.NumTestType.NotEqual:
			{
				float? num3 = num;
				float num4 = num2;
				result = !(num3.GetValueOrDefault() == num4 & num3 != null);
				break;
			}
			case PlayerDataTest.NumTestType.LessThan:
			{
				float? num3 = num;
				float num4 = num2;
				result = (num3.GetValueOrDefault() < num4 & num3 != null);
				break;
			}
			case PlayerDataTest.NumTestType.MoreThan:
			{
				float? num3 = num;
				float num4 = num2;
				result = (num3.GetValueOrDefault() > num4 & num3 != null);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		// Token: 0x04008616 RID: 34326
		public PlayerDataTest.TestType Type;

		// Token: 0x04008617 RID: 34327
		public string FieldName;

		// Token: 0x04008618 RID: 34328
		public bool BoolValue;

		// Token: 0x04008619 RID: 34329
		public PlayerDataTest.NumTestType NumType;

		// Token: 0x0400861A RID: 34330
		public int IntValue;

		// Token: 0x0400861B RID: 34331
		public float FloatValue;

		// Token: 0x0400861C RID: 34332
		public string StringValue;

		// Token: 0x0400861D RID: 34333
		public PlayerDataTest.StringTestType StringType;
	}

	// Token: 0x02001525 RID: 5413
	[Serializable]
	public struct TestGroup
	{
		// Token: 0x060085DF RID: 34271 RVA: 0x002716B0 File Offset: 0x0026F8B0
		public bool IsFulfilled(PlayerDataBase playerData)
		{
			foreach (PlayerDataTest.Test test in this.Tests)
			{
				if (!test.IsFulfilled(playerData))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400861E RID: 34334
		[Tooltip("All tests in this group must be passed for the test group to pass (only 1 test group needs to pass for entire test to pass, however).")]
		public PlayerDataTest.Test[] Tests;
	}
}
