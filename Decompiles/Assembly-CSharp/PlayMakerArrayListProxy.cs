using System;
using System.Collections;

// Token: 0x0200001E RID: 30
public class PlayMakerArrayListProxy : PlayMakerCollectionProxy
{
	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000124 RID: 292 RVA: 0x00006A54 File Offset: 0x00004C54
	public ArrayList arrayList
	{
		get
		{
			return this._arrayList;
		}
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00006A5C File Offset: 0x00004C5C
	public void Awake()
	{
		this._arrayList = new ArrayList();
		this.PreFillArrayList();
		this.TakeSnapShot();
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00006A75 File Offset: 0x00004C75
	public bool isCollectionDefined()
	{
		return this.arrayList != null;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00006A80 File Offset: 0x00004C80
	public void TakeSnapShot()
	{
		this._snapShot = new ArrayList();
		this._snapShot.AddRange(this._arrayList);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00006A9E File Offset: 0x00004C9E
	public void RevertToSnapShot()
	{
		this._arrayList = new ArrayList();
		this._arrayList.AddRange(this._snapShot);
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00006ABC File Offset: 0x00004CBC
	public void Add(object value, string type, bool silent = false)
	{
		this.arrayList.Add(value);
		if (!silent)
		{
			base.dispatchEvent(this.addEvent, value, type);
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00006ADC File Offset: 0x00004CDC
	public int AddRange(ICollection collection, string type)
	{
		this.arrayList.AddRange(collection);
		return this.arrayList.Count;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00006AF5 File Offset: 0x00004CF5
	public void InspectorEdit(int index)
	{
		base.dispatchEvent(this.setEvent, index, "int");
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00006B0E File Offset: 0x00004D0E
	public void Set(int index, object value, string type)
	{
		this.arrayList[index] = value;
		base.dispatchEvent(this.setEvent, index, "int");
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00006B34 File Offset: 0x00004D34
	public bool Remove(object value, string type, bool silent = false)
	{
		if (this.arrayList.Contains(value))
		{
			this.arrayList.Remove(value);
			if (!silent)
			{
				base.dispatchEvent(this.removeEvent, value, type);
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00006B64 File Offset: 0x00004D64
	private void PreFillArrayList()
	{
		switch (this.preFillType)
		{
		case PlayMakerCollectionProxy.VariableEnum.GameObject:
			this.arrayList.InsertRange(0, this.preFillGameObjectList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Int:
			this.arrayList.InsertRange(0, this.preFillIntList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Float:
			this.arrayList.InsertRange(0, this.preFillFloatList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.String:
			this.arrayList.InsertRange(0, this.preFillStringList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Bool:
			this.arrayList.InsertRange(0, this.preFillBoolList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Vector3:
			this.arrayList.InsertRange(0, this.preFillVector3List);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Rect:
			this.arrayList.InsertRange(0, this.preFillRectList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Quaternion:
			this.arrayList.InsertRange(0, this.preFillQuaternionList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Color:
			this.arrayList.InsertRange(0, this.preFillColorList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Material:
			this.arrayList.InsertRange(0, this.preFillMaterialList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Texture:
			this.arrayList.InsertRange(0, this.preFillTextureList);
			return;
		case PlayMakerCollectionProxy.VariableEnum.Vector2:
			this.arrayList.InsertRange(0, this.preFillVector2List);
			return;
		case PlayMakerCollectionProxy.VariableEnum.AudioClip:
			this.arrayList.InsertRange(0, this.preFillAudioClipList);
			return;
		default:
			return;
		}
	}

	// Token: 0x040000B3 RID: 179
	public ArrayList _arrayList;

	// Token: 0x040000B4 RID: 180
	private ArrayList _snapShot;
}
