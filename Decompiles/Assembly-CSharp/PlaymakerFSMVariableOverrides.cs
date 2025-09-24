using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200077C RID: 1916
public class PlaymakerFSMVariableOverrides : MonoBehaviour
{
	// Token: 0x06004410 RID: 17424 RVA: 0x0012A8C0 File Offset: 0x00128AC0
	private void Awake()
	{
		if (!this.fsm)
		{
			return;
		}
		this.OverrideVariables<FsmFloat, float>(this.fsm.FsmVariables.FloatVariables, this.floatOverrides);
		this.OverrideVariables<FsmInt, int>(this.fsm.FsmVariables.IntVariables, this.intOverrides);
		this.OverrideVariables<FsmBool, bool>(this.fsm.FsmVariables.BoolVariables, this.boolOverrides);
		this.OverrideVariables<FsmGameObject, GameObject>(this.fsm.FsmVariables.GameObjectVariables, this.gameObjectOverrides);
		this.OverrideVariables<FsmVector2, Vector2>(this.fsm.FsmVariables.Vector2Variables, this.vector2Overrides);
		this.OverrideVariables<FsmVector3, Vector3>(this.fsm.FsmVariables.Vector3Variables, this.vector3Overrides);
		this.OverrideVariables<FsmString, string>(this.fsm.FsmVariables.StringVariables, this.stringOverrides);
		this.OverrideVariables<FsmColor, Color>(this.fsm.FsmVariables.ColorVariables, this.colorOverrides);
		this.OverrideVariables<FsmQuaternion, Material>(this.fsm.FsmVariables.QuaternionVariables, this.materialOverrides);
		this.OverrideVariables<FsmObject, Object>(this.fsm.FsmVariables.ObjectVariables, this.objectOverrides);
		foreach (FsmEnum fsmEnum in this.fsm.FsmVariables.EnumVariables)
		{
			if ((fsmEnum.ShowInInspector || this.overrideInternalVariables) && this.intOverrides.Dictionary.ContainsKey(fsmEnum.Name))
			{
				fsmEnum.RawValue = Enum.ToObject(fsmEnum.EnumType, this.intOverrides.Dictionary[fsmEnum.Name]);
			}
		}
	}

	// Token: 0x06004411 RID: 17425 RVA: 0x0012AA68 File Offset: 0x00128C68
	public void PurgeBlankNames()
	{
		this.PurgeBlankName<float>(this.floatOverrides);
		this.PurgeBlankName<int>(this.intOverrides);
		this.PurgeBlankName<bool>(this.boolOverrides);
		this.PurgeBlankName<GameObject>(this.gameObjectOverrides);
		this.PurgeBlankName<Vector2>(this.vector2Overrides);
		this.PurgeBlankName<Vector3>(this.vector3Overrides);
		this.PurgeBlankName<string>(this.stringOverrides);
		this.PurgeBlankName<Color>(this.colorOverrides);
		this.PurgeBlankName<Material>(this.materialOverrides);
		this.PurgeBlankName<Object>(this.objectOverrides);
	}

	// Token: 0x06004412 RID: 17426 RVA: 0x0012AAF0 File Offset: 0x00128CF0
	private void OverrideVariables<T, U>(T[] variables, PlaymakerFSMVariableOverrides.IDictionaryAccessable<U> dictionary) where T : NamedVariable
	{
		foreach (T t in variables)
		{
			if ((t.ShowInInspector || this.overrideInternalVariables) && dictionary.Dictionary.ContainsKey(t.Name))
			{
				t.RawValue = dictionary.Dictionary[t.Name];
			}
		}
	}

	// Token: 0x06004413 RID: 17427 RVA: 0x0012AB68 File Offset: 0x00128D68
	public void AddOverride(VariableType type, string variableName)
	{
		if (string.IsNullOrEmpty(variableName))
		{
			return;
		}
		switch (type)
		{
		case VariableType.Float:
			this.floatOverrides.Dictionary[variableName] = 0f;
			return;
		case VariableType.Int:
		case VariableType.Enum:
			this.intOverrides.Dictionary[variableName] = 0;
			return;
		case VariableType.Bool:
			this.boolOverrides.Dictionary[variableName] = false;
			return;
		case VariableType.GameObject:
			this.gameObjectOverrides.Dictionary[variableName] = null;
			return;
		case VariableType.String:
			this.stringOverrides.Dictionary[variableName] = null;
			return;
		case VariableType.Vector2:
			this.vector2Overrides.Dictionary[variableName] = default(Vector2);
			return;
		case VariableType.Vector3:
			this.vector3Overrides.Dictionary[variableName] = default(Vector3);
			return;
		case VariableType.Color:
			this.colorOverrides.Dictionary[variableName] = default(Color);
			return;
		case VariableType.Rect:
		case VariableType.Texture:
		case VariableType.Quaternion:
		case VariableType.Array:
			break;
		case VariableType.Material:
			this.materialOverrides.Dictionary[variableName] = null;
			return;
		case VariableType.Object:
			this.objectOverrides.Dictionary[variableName] = null;
			break;
		default:
			return;
		}
	}

	// Token: 0x06004414 RID: 17428 RVA: 0x0012AC9C File Offset: 0x00128E9C
	public bool HasOverride(VariableType type, string variableName)
	{
		if (string.IsNullOrEmpty(variableName))
		{
			return false;
		}
		switch (type)
		{
		case VariableType.Float:
			return this.floatOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Int:
		case VariableType.Enum:
			return this.intOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Bool:
			return this.boolOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.GameObject:
			return this.gameObjectOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.String:
			return this.stringOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Vector2:
			return this.vector2Overrides.Dictionary.ContainsKey(variableName);
		case VariableType.Vector3:
			return this.vector3Overrides.Dictionary.ContainsKey(variableName);
		case VariableType.Color:
			return this.colorOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Material:
			return this.materialOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Object:
			return this.objectOverrides.Dictionary.ContainsKey(variableName);
		}
		return false;
	}

	// Token: 0x06004415 RID: 17429 RVA: 0x0012ADAF File Offset: 0x00128FAF
	private void PurgeBlankName<U>(PlaymakerFSMVariableOverrides.IDictionaryAccessable<U> dictionary)
	{
		dictionary.OnAfterDeserialize();
		if (dictionary.Dictionary.ContainsKey(string.Empty))
		{
			dictionary.Dictionary.Remove(string.Empty);
		}
		dictionary.OnBeforeSerialize();
	}

	// Token: 0x06004416 RID: 17430 RVA: 0x0012ADE0 File Offset: 0x00128FE0
	public NamedVariable[] GetFsmVariables()
	{
		if (!this.fsm)
		{
			return new NamedVariable[0];
		}
		if (this.overrideInternalVariables)
		{
			return this.fsm.FsmVariables.GetAllNamedVariablesSorted();
		}
		return (from var in this.fsm.FsmVariables.GetAllNamedVariablesSorted()
		where var.ShowInInspector
		select var).ToArray<NamedVariable>();
	}

	// Token: 0x06004417 RID: 17431 RVA: 0x0012AE54 File Offset: 0x00129054
	public Type GetOverriddenObjectType(string variableName)
	{
		if (!this.fsm)
		{
			return null;
		}
		FsmObject fsmObject = this.fsm.FsmVariables.ObjectVariables.FirstOrDefault((FsmObject obj) => obj.Name == variableName);
		if (fsmObject == null)
		{
			return null;
		}
		return fsmObject.ObjectType;
	}

	// Token: 0x06004418 RID: 17432 RVA: 0x0012AEAC File Offset: 0x001290AC
	public Type GetEnumType(string variableName)
	{
		if (!this.fsm)
		{
			return null;
		}
		FsmEnum fsmEnum = this.fsm.FsmVariables.EnumVariables.FirstOrDefault((FsmEnum obj) => obj.Name == variableName);
		if (fsmEnum == null)
		{
			return null;
		}
		return fsmEnum.EnumType;
	}

	// Token: 0x0400454F RID: 17743
	[SerializeField]
	private PlayMakerFSM fsm;

	// Token: 0x04004550 RID: 17744
	[SerializeField]
	private bool overrideInternalVariables;

	// Token: 0x04004551 RID: 17745
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideFloats floatOverrides;

	// Token: 0x04004552 RID: 17746
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideInts intOverrides;

	// Token: 0x04004553 RID: 17747
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideBools boolOverrides;

	// Token: 0x04004554 RID: 17748
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideGameObjects gameObjectOverrides;

	// Token: 0x04004555 RID: 17749
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideVector2s vector2Overrides;

	// Token: 0x04004556 RID: 17750
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideVector3s vector3Overrides;

	// Token: 0x04004557 RID: 17751
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideStrings stringOverrides;

	// Token: 0x04004558 RID: 17752
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideColors colorOverrides;

	// Token: 0x04004559 RID: 17753
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideMaterials materialOverrides;

	// Token: 0x0400455A RID: 17754
	[SerializeField]
	private PlaymakerFSMVariableOverrides.NamedOverrideObjects objectOverrides;

	// Token: 0x02001A3E RID: 6718
	public interface IDictionaryAccessable<T>
	{
		// Token: 0x1700110A RID: 4362
		// (get) Token: 0x0600965E RID: 38494
		// (set) Token: 0x0600965F RID: 38495
		Dictionary<string, T> Dictionary { get; set; }

		// Token: 0x06009660 RID: 38496
		void OnAfterDeserialize();

		// Token: 0x06009661 RID: 38497
		void OnBeforeSerialize();
	}

	// Token: 0x02001A3F RID: 6719
	[Serializable]
	private class NamedOverrideFloat : SerializableNamedData<float>
	{
	}

	// Token: 0x02001A40 RID: 6720
	[Serializable]
	private class NamedOverrideFloats : SerializableNamedList<float, PlaymakerFSMVariableOverrides.NamedOverrideFloat>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<float>
	{
		// Token: 0x1700110B RID: 4363
		// (get) Token: 0x06009663 RID: 38499 RVA: 0x002A7FD5 File Offset: 0x002A61D5
		// (set) Token: 0x06009664 RID: 38500 RVA: 0x002A7FDD File Offset: 0x002A61DD
		public Dictionary<string, float> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A41 RID: 6721
	[Serializable]
	private class NamedOverrideInt : SerializableNamedData<int>
	{
	}

	// Token: 0x02001A42 RID: 6722
	[Serializable]
	private class NamedOverrideInts : SerializableNamedList<int, PlaymakerFSMVariableOverrides.NamedOverrideInt>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<int>
	{
		// Token: 0x1700110C RID: 4364
		// (get) Token: 0x06009667 RID: 38503 RVA: 0x002A7FF6 File Offset: 0x002A61F6
		// (set) Token: 0x06009668 RID: 38504 RVA: 0x002A7FFE File Offset: 0x002A61FE
		public Dictionary<string, int> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A43 RID: 6723
	[Serializable]
	private class NamedOverrideBool : SerializableNamedData<bool>
	{
	}

	// Token: 0x02001A44 RID: 6724
	[Serializable]
	private class NamedOverrideBools : SerializableNamedList<bool, PlaymakerFSMVariableOverrides.NamedOverrideBool>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<bool>
	{
		// Token: 0x1700110D RID: 4365
		// (get) Token: 0x0600966B RID: 38507 RVA: 0x002A8017 File Offset: 0x002A6217
		// (set) Token: 0x0600966C RID: 38508 RVA: 0x002A801F File Offset: 0x002A621F
		public Dictionary<string, bool> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A45 RID: 6725
	[Serializable]
	private class NamedOverrideGameObject : SerializableNamedData<GameObject>
	{
	}

	// Token: 0x02001A46 RID: 6726
	[Serializable]
	private class NamedOverrideGameObjects : SerializableNamedList<GameObject, PlaymakerFSMVariableOverrides.NamedOverrideGameObject>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<GameObject>
	{
		// Token: 0x1700110E RID: 4366
		// (get) Token: 0x0600966F RID: 38511 RVA: 0x002A8038 File Offset: 0x002A6238
		// (set) Token: 0x06009670 RID: 38512 RVA: 0x002A8040 File Offset: 0x002A6240
		public Dictionary<string, GameObject> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A47 RID: 6727
	[Serializable]
	private class NamedOverrideVector2 : SerializableNamedData<Vector2>
	{
	}

	// Token: 0x02001A48 RID: 6728
	[Serializable]
	private class NamedOverrideVector2s : SerializableNamedList<Vector2, PlaymakerFSMVariableOverrides.NamedOverrideVector2>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<Vector2>
	{
		// Token: 0x1700110F RID: 4367
		// (get) Token: 0x06009673 RID: 38515 RVA: 0x002A8059 File Offset: 0x002A6259
		// (set) Token: 0x06009674 RID: 38516 RVA: 0x002A8061 File Offset: 0x002A6261
		public Dictionary<string, Vector2> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A49 RID: 6729
	[Serializable]
	private class NamedOverrideVector3 : SerializableNamedData<Vector3>
	{
	}

	// Token: 0x02001A4A RID: 6730
	[Serializable]
	private class NamedOverrideVector3s : SerializableNamedList<Vector3, PlaymakerFSMVariableOverrides.NamedOverrideVector3>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<Vector3>
	{
		// Token: 0x17001110 RID: 4368
		// (get) Token: 0x06009677 RID: 38519 RVA: 0x002A807A File Offset: 0x002A627A
		// (set) Token: 0x06009678 RID: 38520 RVA: 0x002A8082 File Offset: 0x002A6282
		public Dictionary<string, Vector3> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A4B RID: 6731
	[Serializable]
	private class NamedOverrideString : SerializableNamedData<string>
	{
	}

	// Token: 0x02001A4C RID: 6732
	[Serializable]
	private class NamedOverrideStrings : SerializableNamedList<string, PlaymakerFSMVariableOverrides.NamedOverrideString>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<string>
	{
		// Token: 0x17001111 RID: 4369
		// (get) Token: 0x0600967B RID: 38523 RVA: 0x002A809B File Offset: 0x002A629B
		// (set) Token: 0x0600967C RID: 38524 RVA: 0x002A80A3 File Offset: 0x002A62A3
		public Dictionary<string, string> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A4D RID: 6733
	[Serializable]
	private class NamedOverrideColor : SerializableNamedData<Color>
	{
	}

	// Token: 0x02001A4E RID: 6734
	[Serializable]
	private class NamedOverrideColors : SerializableNamedList<Color, PlaymakerFSMVariableOverrides.NamedOverrideColor>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<Color>
	{
		// Token: 0x17001112 RID: 4370
		// (get) Token: 0x0600967F RID: 38527 RVA: 0x002A80BC File Offset: 0x002A62BC
		// (set) Token: 0x06009680 RID: 38528 RVA: 0x002A80C4 File Offset: 0x002A62C4
		public Dictionary<string, Color> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A4F RID: 6735
	[Serializable]
	private class NamedOverrideMaterial : SerializableNamedData<Material>
	{
	}

	// Token: 0x02001A50 RID: 6736
	[Serializable]
	private class NamedOverrideMaterials : SerializableNamedList<Material, PlaymakerFSMVariableOverrides.NamedOverrideMaterial>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<Material>
	{
		// Token: 0x17001113 RID: 4371
		// (get) Token: 0x06009683 RID: 38531 RVA: 0x002A80DD File Offset: 0x002A62DD
		// (set) Token: 0x06009684 RID: 38532 RVA: 0x002A80E5 File Offset: 0x002A62E5
		public Dictionary<string, Material> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}

	// Token: 0x02001A51 RID: 6737
	[Serializable]
	private class NamedOverrideObject : SerializableNamedData<Object>
	{
	}

	// Token: 0x02001A52 RID: 6738
	[Serializable]
	private class NamedOverrideObjects : SerializableNamedList<Object, PlaymakerFSMVariableOverrides.NamedOverrideObject>, PlaymakerFSMVariableOverrides.IDictionaryAccessable<Object>
	{
		// Token: 0x17001114 RID: 4372
		// (get) Token: 0x06009687 RID: 38535 RVA: 0x002A80FE File Offset: 0x002A62FE
		// (set) Token: 0x06009688 RID: 38536 RVA: 0x002A8106 File Offset: 0x002A6306
		public Dictionary<string, Object> Dictionary
		{
			get
			{
				return this.RuntimeData;
			}
			set
			{
				this.RuntimeData = value;
			}
		}
	}
}
