using System;
using System.Collections;

// Token: 0x02000020 RID: 32
public class PlayMakerHashTableProxy : PlayMakerCollectionProxy
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000134 RID: 308 RVA: 0x000073DB File Offset: 0x000055DB
	public Hashtable hashTable
	{
		get
		{
			return this._hashTable;
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x000073E3 File Offset: 0x000055E3
	public void Awake()
	{
		this._hashTable = new Hashtable();
		this.PreFillHashTable();
		this.TakeSnapShot();
	}

	// Token: 0x06000136 RID: 310 RVA: 0x000073FC File Offset: 0x000055FC
	public bool isCollectionDefined()
	{
		return this.hashTable != null;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007408 File Offset: 0x00005608
	public void TakeSnapShot()
	{
		this._snapShot = new Hashtable();
		foreach (object key in this._hashTable.Keys)
		{
			this._snapShot[key] = this._hashTable[key];
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00007480 File Offset: 0x00005680
	public void RevertToSnapShot()
	{
		this._hashTable = new Hashtable();
		foreach (object key in this._snapShot.Keys)
		{
			this._hashTable[key] = this._snapShot[key];
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x000074F8 File Offset: 0x000056F8
	public void InspectorEdit(int index)
	{
		base.dispatchEvent(this.setEvent, index, "int");
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00007514 File Offset: 0x00005714
	private void PreFillHashTable()
	{
		for (int i = 0; i < this.preFillKeyList.Count; i++)
		{
			switch (this.preFillType)
			{
			case PlayMakerCollectionProxy.VariableEnum.GameObject:
				this.hashTable[this.preFillKeyList[i]] = this.preFillGameObjectList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Int:
				this.hashTable[this.preFillKeyList[i]] = this.preFillIntList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Float:
				this.hashTable[this.preFillKeyList[i]] = this.preFillFloatList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.String:
				this.hashTable[this.preFillKeyList[i]] = this.preFillStringList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Bool:
				this.hashTable[this.preFillKeyList[i]] = this.preFillBoolList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Vector3:
				this.hashTable[this.preFillKeyList[i]] = this.preFillVector3List[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Rect:
				this.hashTable[this.preFillKeyList[i]] = this.preFillRectList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Quaternion:
				this.hashTable[this.preFillKeyList[i]] = this.preFillQuaternionList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Color:
				this.hashTable[this.preFillKeyList[i]] = this.preFillColorList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Material:
				this.hashTable[this.preFillKeyList[i]] = this.preFillMaterialList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Texture:
				this.hashTable[this.preFillKeyList[i]] = this.preFillTextureList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Vector2:
				this.hashTable[this.preFillKeyList[i]] = this.preFillVector2List[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.AudioClip:
				this.hashTable[this.preFillKeyList[i]] = this.preFillAudioClipList[i];
				break;
			}
		}
	}

	// Token: 0x040000D3 RID: 211
	public Hashtable _hashTable;

	// Token: 0x040000D4 RID: 212
	private Hashtable _snapShot;
}
