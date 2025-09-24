using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200001F RID: 31
public abstract class PlayMakerCollectionProxy : MonoBehaviour
{
	// Token: 0x06000130 RID: 304 RVA: 0x00006CB1 File Offset: 0x00004EB1
	internal string getFsmVariableType(VariableType _type)
	{
		return _type.ToString();
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00006CC0 File Offset: 0x00004EC0
	internal void dispatchEvent(string anEvent, object value, string type)
	{
		if (!this.enablePlayMakerEvents)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(type);
		if (num <= 1707186308U)
		{
			if (num <= 655918371U)
			{
				if (num != 398550328U)
				{
					if (num != 639140752U)
					{
						if (num == 655918371U)
						{
							if (type == "vector3")
							{
								Fsm.EventData.Vector3Data = (Vector3)value;
							}
						}
					}
					else if (type == "vector2")
					{
						Fsm.EventData.Vector3Data = (Vector3)value;
					}
				}
				else if (type == "string")
				{
					Fsm.EventData.StringData = (string)value;
				}
			}
			else if (num != 1013213428U)
			{
				if (num != 1031692888U)
				{
					if (num == 1707186308U)
					{
						if (type == "gameObject")
						{
							Fsm.EventData.ObjectData = (GameObject)value;
						}
					}
				}
				else if (type == "color")
				{
					Fsm.EventData.ColorData = (Color)value;
				}
			}
			else if (type == "texture")
			{
				Fsm.EventData.TextureData = (Texture)value;
			}
		}
		else if (num <= 3099987130U)
		{
			if (num != 2515107422U)
			{
				if (num != 2797886853U)
				{
					if (num == 3099987130U)
					{
						if (type == "object")
						{
							Fsm.EventData.ObjectData = (Object)value;
						}
					}
				}
				else if (type == "float")
				{
					Fsm.EventData.FloatData = (float)value;
				}
			}
			else if (type == "int")
			{
				Fsm.EventData.IntData = (int)value;
			}
		}
		else if (num <= 3538210912U)
		{
			if (num != 3365180733U)
			{
				if (num == 3538210912U)
				{
					if (type == "material")
					{
						Fsm.EventData.MaterialData = (Material)value;
					}
				}
			}
			else if (type == "bool")
			{
				Fsm.EventData.BoolData = (bool)value;
			}
		}
		else if (num != 3940830471U)
		{
			if (num == 4091954829U)
			{
				if (type == "quaternion")
				{
					Fsm.EventData.QuaternionData = (Quaternion)value;
				}
			}
		}
		else if (type == "rect")
		{
			Fsm.EventData.RectData = (Rect)value;
		}
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
		List<Fsm> list = new List<Fsm>(Fsm.FsmList);
		if (list.Count > 0)
		{
			list[0].Event(fsmEventTarget, anEvent);
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00006FD8 File Offset: 0x000051D8
	public void cleanPrefilledLists()
	{
		if (this.preFillKeyList.Count > this.preFillCount)
		{
			this.preFillKeyList.RemoveRange(this.preFillCount, this.preFillKeyList.Count - this.preFillCount);
		}
		if (this.preFillBoolList.Count > this.preFillCount)
		{
			this.preFillBoolList.RemoveRange(this.preFillCount, this.preFillBoolList.Count - this.preFillCount);
		}
		if (this.preFillColorList.Count > this.preFillCount)
		{
			this.preFillColorList.RemoveRange(this.preFillCount, this.preFillColorList.Count - this.preFillCount);
		}
		if (this.preFillFloatList.Count > this.preFillCount)
		{
			this.preFillFloatList.RemoveRange(this.preFillCount, this.preFillFloatList.Count - this.preFillCount);
		}
		if (this.preFillIntList.Count > this.preFillCount)
		{
			this.preFillIntList.RemoveRange(this.preFillCount, this.preFillIntList.Count - this.preFillCount);
		}
		if (this.preFillMaterialList.Count > this.preFillCount)
		{
			this.preFillMaterialList.RemoveRange(this.preFillCount, this.preFillMaterialList.Count - this.preFillCount);
		}
		if (this.preFillGameObjectList.Count > this.preFillCount)
		{
			this.preFillGameObjectList.RemoveRange(this.preFillCount, this.preFillGameObjectList.Count - this.preFillCount);
		}
		if (this.preFillObjectList.Count > this.preFillCount)
		{
			this.preFillObjectList.RemoveRange(this.preFillCount, this.preFillObjectList.Count - this.preFillCount);
		}
		if (this.preFillQuaternionList.Count > this.preFillCount)
		{
			this.preFillQuaternionList.RemoveRange(this.preFillCount, this.preFillQuaternionList.Count - this.preFillCount);
		}
		if (this.preFillRectList.Count > this.preFillCount)
		{
			this.preFillRectList.RemoveRange(this.preFillCount, this.preFillRectList.Count - this.preFillCount);
		}
		if (this.preFillStringList.Count > this.preFillCount)
		{
			this.preFillStringList.RemoveRange(this.preFillCount, this.preFillStringList.Count - this.preFillCount);
		}
		if (this.preFillTextureList.Count > this.preFillCount)
		{
			this.preFillTextureList.RemoveRange(this.preFillCount, this.preFillTextureList.Count - this.preFillCount);
		}
		if (this.preFillVector2List.Count > this.preFillCount)
		{
			this.preFillVector2List.RemoveRange(this.preFillCount, this.preFillVector2List.Count - this.preFillCount);
		}
		if (this.preFillVector3List.Count > this.preFillCount)
		{
			this.preFillVector3List.RemoveRange(this.preFillCount, this.preFillVector3List.Count - this.preFillCount);
		}
		if (this.preFillAudioClipList.Count > this.preFillCount)
		{
			this.preFillAudioClipList.RemoveRange(this.preFillCount, this.preFillAudioClipList.Count - this.preFillCount);
		}
	}

	// Token: 0x040000B5 RID: 181
	public bool showEvents;

	// Token: 0x040000B6 RID: 182
	public bool showContent;

	// Token: 0x040000B7 RID: 183
	public bool TextureElementSmall;

	// Token: 0x040000B8 RID: 184
	public bool condensedView;

	// Token: 0x040000B9 RID: 185
	public bool liveUpdate;

	// Token: 0x040000BA RID: 186
	public string referenceName = "";

	// Token: 0x040000BB RID: 187
	public bool enablePlayMakerEvents;

	// Token: 0x040000BC RID: 188
	public string addEvent;

	// Token: 0x040000BD RID: 189
	public string setEvent;

	// Token: 0x040000BE RID: 190
	public string removeEvent;

	// Token: 0x040000BF RID: 191
	public int contentPreviewStartIndex;

	// Token: 0x040000C0 RID: 192
	public int contentPreviewMaxRows = 10;

	// Token: 0x040000C1 RID: 193
	public PlayMakerCollectionProxy.VariableEnum preFillType;

	// Token: 0x040000C2 RID: 194
	public int preFillObjectTypeIndex;

	// Token: 0x040000C3 RID: 195
	public int preFillCount;

	// Token: 0x040000C4 RID: 196
	public List<string> preFillKeyList = new List<string>();

	// Token: 0x040000C5 RID: 197
	public List<bool> preFillBoolList = new List<bool>();

	// Token: 0x040000C6 RID: 198
	public List<Color> preFillColorList = new List<Color>();

	// Token: 0x040000C7 RID: 199
	public List<float> preFillFloatList = new List<float>();

	// Token: 0x040000C8 RID: 200
	public List<GameObject> preFillGameObjectList = new List<GameObject>();

	// Token: 0x040000C9 RID: 201
	public List<int> preFillIntList = new List<int>();

	// Token: 0x040000CA RID: 202
	public List<Material> preFillMaterialList = new List<Material>();

	// Token: 0x040000CB RID: 203
	public List<Object> preFillObjectList = new List<Object>();

	// Token: 0x040000CC RID: 204
	public List<Quaternion> preFillQuaternionList = new List<Quaternion>();

	// Token: 0x040000CD RID: 205
	public List<Rect> preFillRectList = new List<Rect>();

	// Token: 0x040000CE RID: 206
	public List<string> preFillStringList = new List<string>();

	// Token: 0x040000CF RID: 207
	public List<Texture2D> preFillTextureList = new List<Texture2D>();

	// Token: 0x040000D0 RID: 208
	public List<Vector2> preFillVector2List = new List<Vector2>();

	// Token: 0x040000D1 RID: 209
	public List<Vector3> preFillVector3List = new List<Vector3>();

	// Token: 0x040000D2 RID: 210
	public List<AudioClip> preFillAudioClipList = new List<AudioClip>();

	// Token: 0x020013C6 RID: 5062
	public enum VariableEnum
	{
		// Token: 0x040080A4 RID: 32932
		GameObject,
		// Token: 0x040080A5 RID: 32933
		Int,
		// Token: 0x040080A6 RID: 32934
		Float,
		// Token: 0x040080A7 RID: 32935
		String,
		// Token: 0x040080A8 RID: 32936
		Bool,
		// Token: 0x040080A9 RID: 32937
		Vector3,
		// Token: 0x040080AA RID: 32938
		Rect,
		// Token: 0x040080AB RID: 32939
		Quaternion,
		// Token: 0x040080AC RID: 32940
		Color,
		// Token: 0x040080AD RID: 32941
		Material,
		// Token: 0x040080AE RID: 32942
		Texture,
		// Token: 0x040080AF RID: 32943
		Vector2,
		// Token: 0x040080B0 RID: 32944
		AudioClip
	}
}
