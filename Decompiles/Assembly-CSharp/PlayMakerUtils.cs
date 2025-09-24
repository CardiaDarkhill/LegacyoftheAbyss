using System;
using System.Text.RegularExpressions;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class PlayMakerUtils
{
	// Token: 0x060001CF RID: 463 RVA: 0x00009755 File Offset: 0x00007955
	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, bool includeChildren)
	{
		PlayMakerUtils.SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren, null);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00009761 File Offset: 0x00007961
	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent)
	{
		PlayMakerUtils.SendEventToGameObject(fromFsm, target, fsmEvent, false, null);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000976D File Offset: 0x0000796D
	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, FsmEventData eventData)
	{
		PlayMakerUtils.SendEventToGameObject(fromFsm, target, fsmEvent, false, eventData);
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000977C File Offset: 0x0000797C
	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, bool includeChildren, FsmEventData eventData)
	{
		if (eventData != null)
		{
			Fsm.EventData = eventData;
		}
		if (fromFsm == null)
		{
			return;
		}
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.excludeSelf = false;
		fsmEventTarget.gameObject = new FsmOwnerDefault
		{
			OwnerOption = OwnerDefaultOption.SpecifyGameObject,
			GameObject = new FsmGameObject(),
			GameObject = 
			{
				Value = target
			}
		};
		fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
		fsmEventTarget.sendToChildren = includeChildren;
		fromFsm.Fsm.Event(fsmEventTarget, fsmEvent);
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x000097FC File Offset: 0x000079FC
	public static bool DoesTargetImplementsEvent(FsmEventTarget target, string eventName)
	{
		if (target.target == FsmEventTarget.EventTarget.BroadcastAll)
		{
			return FsmEvent.IsEventGlobal(eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.FSMComponent)
		{
			return PlayMakerUtils.DoesFsmImplementsEvent(target.fsmComponent, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.GameObject)
		{
			return PlayMakerUtils.DoesGameObjectImplementsEvent(target.gameObject.GameObject.Value, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.GameObjectFSM)
		{
			return PlayMakerUtils.DoesGameObjectImplementsEvent(target.gameObject.GameObject.Value, target.fsmName.Value, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.Self)
		{
			Debug.LogError("Self target not supported yet");
		}
		if (target.target == FsmEventTarget.EventTarget.SubFSMs)
		{
			Debug.LogError("subFsms target not supported yet");
		}
		if (target.target == FsmEventTarget.EventTarget.HostFSM)
		{
			Debug.LogError("HostFSM target not supported yet");
		}
		return false;
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x000098B4 File Offset: 0x00007AB4
	public static bool DoesGameObjectImplementsEvent(GameObject go, string fsmEvent)
	{
		if (go == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
		for (int i = 0; i < components.Length; i++)
		{
			if (PlayMakerUtils.DoesFsmImplementsEvent(components[i], fsmEvent))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x000098F8 File Offset: 0x00007AF8
	public static bool DoesGameObjectImplementsEvent(GameObject go, string fsmName, string fsmEvent)
	{
		if (go == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		bool flag = !string.IsNullOrEmpty(fsmName);
		foreach (PlayMakerFSM playMakerFSM in go.GetComponents<PlayMakerFSM>())
		{
			if (flag && object.Equals(playMakerFSM, fsmName) && PlayMakerUtils.DoesFsmImplementsEvent(playMakerFSM, fsmEvent))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00009954 File Offset: 0x00007B54
	public static bool DoesFsmImplementsEvent(PlayMakerFSM fsm, string fsmEvent)
	{
		if (fsm == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		FsmTransition[] array = fsm.FsmGlobalTransitions;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].EventName.Equals(fsmEvent))
			{
				return true;
			}
		}
		FsmState[] fsmStates = fsm.FsmStates;
		for (int i = 0; i < fsmStates.Length; i++)
		{
			array = fsmStates[i].Transitions;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].EventName.Equals(fsmEvent))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x000099DC File Offset: 0x00007BDC
	public static PlayMakerFSM FindFsmOnGameObject(GameObject go, string fsmName)
	{
		if (go == null || string.IsNullOrEmpty(fsmName))
		{
			return null;
		}
		foreach (PlayMakerFSM playMakerFSM in go.GetComponents<PlayMakerFSM>())
		{
			if (string.Equals(playMakerFSM.FsmName, fsmName))
			{
				return playMakerFSM;
			}
		}
		return null;
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00009A28 File Offset: 0x00007C28
	public static void RefreshValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm == null)
		{
			return;
		}
		if (fsmVar == null)
		{
			return;
		}
		if (!fsmVar.useVariable)
		{
			return;
		}
		switch (fsmVar.Type)
		{
		case VariableType.Float:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmFloat(fsmVar.variableName));
			return;
		case VariableType.Int:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmInt(fsmVar.variableName));
			return;
		case VariableType.Bool:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmBool(fsmVar.variableName));
			return;
		case VariableType.GameObject:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmGameObject(fsmVar.variableName));
			return;
		case VariableType.String:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmString(fsmVar.variableName));
			return;
		case VariableType.Vector2:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector2(fsmVar.variableName));
			return;
		case VariableType.Vector3:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
			return;
		case VariableType.Color:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmColor(fsmVar.variableName));
			return;
		case VariableType.Rect:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmRect(fsmVar.variableName));
			return;
		case VariableType.Material:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmMaterial(fsmVar.variableName));
			return;
		case VariableType.Texture:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
			return;
		case VariableType.Quaternion:
			fsmVar.GetValueFrom(fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName));
			return;
		default:
			return;
		}
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00009BA4 File Offset: 0x00007DA4
	public static object GetValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm == null)
		{
			return null;
		}
		if (fsmVar == null)
		{
			return null;
		}
		if (fsmVar.useVariable)
		{
			string variableName = fsmVar.variableName;
			switch (fsmVar.Type)
			{
			case VariableType.Float:
				return fromFsm.Variables.GetFsmFloat(variableName).Value;
			case VariableType.Int:
				return fromFsm.Variables.GetFsmInt(variableName).Value;
			case VariableType.Bool:
				return fromFsm.Variables.GetFsmBool(variableName).Value;
			case VariableType.GameObject:
				return fromFsm.Variables.GetFsmGameObject(variableName).Value;
			case VariableType.String:
				return fromFsm.Variables.GetFsmString(variableName).Value;
			case VariableType.Vector2:
				return fromFsm.Variables.GetFsmVector2(variableName).Value;
			case VariableType.Vector3:
				return fromFsm.Variables.GetFsmVector3(variableName).Value;
			case VariableType.Color:
				return fromFsm.Variables.GetFsmColor(variableName).Value;
			case VariableType.Rect:
				return fromFsm.Variables.GetFsmRect(variableName).Value;
			case VariableType.Material:
				return fromFsm.Variables.GetFsmMaterial(variableName).Value;
			case VariableType.Texture:
				return fromFsm.Variables.GetFsmTexture(variableName).Value;
			case VariableType.Quaternion:
				return fromFsm.Variables.GetFsmQuaternion(variableName).Value;
			case VariableType.Object:
				return fromFsm.Variables.GetFsmObject(variableName).Value;
			}
		}
		else
		{
			switch (fsmVar.Type)
			{
			case VariableType.Float:
				return fsmVar.floatValue;
			case VariableType.Int:
				return fsmVar.intValue;
			case VariableType.Bool:
				return fsmVar.boolValue;
			case VariableType.GameObject:
				return fsmVar.gameObjectValue;
			case VariableType.String:
				return fsmVar.stringValue;
			case VariableType.Vector2:
				return fsmVar.vector2Value;
			case VariableType.Vector3:
				return fsmVar.vector3Value;
			case VariableType.Color:
				return fsmVar.colorValue;
			case VariableType.Rect:
				return fsmVar.rectValue;
			case VariableType.Material:
				return fsmVar.materialValue;
			case VariableType.Texture:
				return fsmVar.textureValue;
			case VariableType.Quaternion:
				return fsmVar.quaternionValue;
			case VariableType.Object:
				return fsmVar.objectReference;
			}
		}
		return null;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00009DF0 File Offset: 0x00007FF0
	public static bool ApplyValueToFsmVar(Fsm fromFsm, FsmVar fsmVar, object value)
	{
		if (fromFsm == null)
		{
			return false;
		}
		if (fsmVar == null)
		{
			return false;
		}
		if (value == null)
		{
			if (fsmVar.Type == VariableType.Bool)
			{
				fromFsm.Variables.GetFsmBool(fsmVar.variableName).Value = false;
			}
			else if (fsmVar.Type == VariableType.Color)
			{
				fromFsm.Variables.GetFsmColor(fsmVar.variableName).Value = Color.black;
			}
			else if (fsmVar.Type == VariableType.Int)
			{
				fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = 0;
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = 0f;
			}
			else if (fsmVar.Type == VariableType.GameObject)
			{
				fromFsm.Variables.GetFsmGameObject(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Material)
			{
				fromFsm.Variables.GetFsmMaterial(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Object)
			{
				fromFsm.Variables.GetFsmObject(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Quaternion)
			{
				fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName).Value = Quaternion.identity;
			}
			else if (fsmVar.Type == VariableType.Rect)
			{
				fromFsm.Variables.GetFsmRect(fsmVar.variableName).Value = new Rect(0f, 0f, 0f, 0f);
			}
			else if (fsmVar.Type == VariableType.String)
			{
				fromFsm.Variables.GetFsmString(fsmVar.variableName).Value = "";
			}
			else if (fsmVar.Type == VariableType.String)
			{
				fromFsm.Variables.GetFsmTexture(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Vector2)
			{
				fromFsm.Variables.GetFsmVector2(fsmVar.variableName).Value = Vector2.zero;
			}
			else if (fsmVar.Type == VariableType.Vector3)
			{
				fromFsm.Variables.GetFsmVector3(fsmVar.variableName).Value = Vector3.zero;
			}
			return true;
		}
		Type type = value.GetType();
		Type type2 = null;
		switch (fsmVar.Type)
		{
		case VariableType.Float:
			type2 = typeof(float);
			break;
		case VariableType.Int:
			type2 = typeof(int);
			break;
		case VariableType.Bool:
			type2 = typeof(bool);
			break;
		case VariableType.GameObject:
			type2 = typeof(GameObject);
			break;
		case VariableType.String:
			type2 = typeof(string);
			break;
		case VariableType.Vector2:
			type2 = typeof(Vector2);
			break;
		case VariableType.Vector3:
			type2 = typeof(Vector3);
			break;
		case VariableType.Color:
			type2 = typeof(Color);
			break;
		case VariableType.Rect:
			type2 = typeof(Rect);
			break;
		case VariableType.Material:
			type2 = typeof(Material);
			break;
		case VariableType.Texture:
			type2 = typeof(Texture2D);
			break;
		case VariableType.Quaternion:
			type2 = typeof(Quaternion);
			break;
		case VariableType.Object:
			type2 = typeof(Object);
			break;
		}
		bool flag = true;
		if (!type2.Equals(type))
		{
			flag = false;
			if (type2.Equals(typeof(Object)))
			{
				flag = true;
			}
			if (!flag)
			{
				if (type.Equals(typeof(double)))
				{
					flag = true;
				}
				if (type.Equals(typeof(long)))
				{
					flag = true;
				}
				if (type.Equals(typeof(byte)))
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			string[] array = new string[5];
			array[0] = "The fsmVar value <";
			int num = 1;
			Type type3 = type2;
			array[num] = ((type3 != null) ? type3.ToString() : null);
			array[2] = "> doesn't match the value <";
			int num2 = 3;
			Type type4 = type;
			array[num2] = ((type4 != null) ? type4.ToString() : null);
			array[4] = ">";
			Debug.LogError(string.Concat(array));
			return false;
		}
		if (type == typeof(bool))
		{
			fromFsm.Variables.GetFsmBool(fsmVar.variableName).Value = (bool)value;
		}
		else if (type == typeof(Color))
		{
			fromFsm.Variables.GetFsmColor(fsmVar.variableName).Value = (Color)value;
		}
		else if (type == typeof(int))
		{
			fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = Convert.ToInt32(value);
		}
		else if (type == typeof(byte))
		{
			fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = Convert.ToInt32(value);
		}
		else if (type == typeof(long))
		{
			if (fsmVar.Type == VariableType.Int)
			{
				fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = Convert.ToInt32(value);
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = Convert.ToSingle(value);
			}
		}
		else if (type == typeof(float))
		{
			fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = (float)value;
		}
		else if (type == typeof(double))
		{
			fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = Convert.ToSingle(value);
		}
		else if (type == typeof(GameObject))
		{
			fromFsm.Variables.GetFsmGameObject(fsmVar.variableName).Value = (GameObject)value;
		}
		else if (type == typeof(Material))
		{
			fromFsm.Variables.GetFsmMaterial(fsmVar.variableName).Value = (Material)value;
		}
		else if (type == typeof(Object) || type2 == typeof(Object))
		{
			fromFsm.Variables.GetFsmObject(fsmVar.variableName).Value = (Object)value;
		}
		else if (type == typeof(Quaternion))
		{
			fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName).Value = (Quaternion)value;
		}
		else if (type == typeof(Rect))
		{
			fromFsm.Variables.GetFsmRect(fsmVar.variableName).Value = (Rect)value;
		}
		else if (type == typeof(string))
		{
			fromFsm.Variables.GetFsmString(fsmVar.variableName).Value = (string)value;
		}
		else if (type == typeof(Texture2D))
		{
			fromFsm.Variables.GetFsmTexture(fsmVar.variableName).Value = (Texture2D)value;
		}
		else if (type == typeof(Vector2))
		{
			fromFsm.Variables.GetFsmVector2(fsmVar.variableName).Value = (Vector2)value;
		}
		else if (type == typeof(Vector3))
		{
			fromFsm.Variables.GetFsmVector3(fsmVar.variableName).Value = (Vector3)value;
		}
		else
		{
			string str = "?!?!";
			Type type5 = type;
			Debug.LogWarning(str + ((type5 != null) ? type5.ToString() : null));
		}
		return true;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000A54C File Offset: 0x0000874C
	public static float GetFloatFromObject(object _obj, VariableType targetType, bool fastProcessingIfPossible)
	{
		if (targetType == VariableType.Int || targetType == VariableType.Float)
		{
			return Convert.ToSingle(_obj);
		}
		if (targetType == VariableType.Vector2)
		{
			Vector2 lhs = (Vector2)_obj;
			if (lhs != Vector2.zero)
			{
				if (!fastProcessingIfPossible)
				{
					return lhs.magnitude;
				}
				return lhs.sqrMagnitude;
			}
		}
		if (targetType == VariableType.Vector3)
		{
			Vector3 lhs2 = (Vector3)_obj;
			if (lhs2 != Vector3.zero)
			{
				if (!fastProcessingIfPossible)
				{
					return lhs2.magnitude;
				}
				return lhs2.sqrMagnitude;
			}
		}
		if (targetType == VariableType.GameObject)
		{
			GameObject gameObject = (GameObject)_obj;
			if (gameObject != null)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					return component.bounds.size.x * component.bounds.size.y * component.bounds.size.z;
				}
			}
		}
		if (targetType == VariableType.Rect)
		{
			Rect rect = (Rect)_obj;
			return rect.width * rect.height;
		}
		if (targetType == VariableType.String)
		{
			string text = (string)_obj;
			if (text != null)
			{
				return float.Parse(text);
			}
		}
		return 0f;
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000A65C File Offset: 0x0000885C
	public static string ParseFsmVarToString(Fsm fsm, FsmVar fsmVar)
	{
		if (fsmVar == null)
		{
			return "";
		}
		object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(fsm, fsmVar);
		if (valueFromFsmVar == null)
		{
			return "";
		}
		if (fsmVar.Type == VariableType.String)
		{
			return (string)valueFromFsmVar;
		}
		if (fsmVar.Type == VariableType.Bool)
		{
			if (!(bool)valueFromFsmVar)
			{
				return "0";
			}
			return "1";
		}
		else
		{
			if (fsmVar.Type == VariableType.Float)
			{
				return float.Parse(valueFromFsmVar.ToString()).ToString();
			}
			if (fsmVar.Type == VariableType.Int)
			{
				return int.Parse(valueFromFsmVar.ToString()).ToString();
			}
			if (fsmVar.Type == VariableType.Vector2)
			{
				Vector2 vector = (Vector2)valueFromFsmVar;
				return vector.x.ToString() + "," + vector.y.ToString();
			}
			if (fsmVar.Type == VariableType.Vector3)
			{
				Vector3 vector2 = (Vector3)valueFromFsmVar;
				return string.Concat(new string[]
				{
					vector2.x.ToString(),
					",",
					vector2.y.ToString(),
					",",
					vector2.z.ToString()
				});
			}
			if (fsmVar.Type == VariableType.Quaternion)
			{
				Quaternion quaternion = (Quaternion)valueFromFsmVar;
				return string.Concat(new string[]
				{
					quaternion.x.ToString(),
					",",
					quaternion.y.ToString(),
					",",
					quaternion.z.ToString(),
					",",
					quaternion.w.ToString()
				});
			}
			if (fsmVar.Type == VariableType.Rect)
			{
				Rect rect = (Rect)valueFromFsmVar;
				return string.Concat(new string[]
				{
					rect.x.ToString(),
					",",
					rect.y.ToString(),
					",",
					rect.width.ToString(),
					",",
					rect.height.ToString()
				});
			}
			if (fsmVar.Type == VariableType.Color)
			{
				Color color = (Color)valueFromFsmVar;
				return string.Concat(new string[]
				{
					color.r.ToString(),
					",",
					color.g.ToString(),
					",",
					color.b.ToString(),
					",",
					color.a.ToString()
				});
			}
			if (fsmVar.Type == VariableType.GameObject)
			{
				return ((GameObject)valueFromFsmVar).name;
			}
			if (fsmVar.Type == VariableType.Material)
			{
				return ((Material)valueFromFsmVar).name;
			}
			if (fsmVar.Type == VariableType.Texture)
			{
				return ((Texture2D)valueFromFsmVar).name;
			}
			string str = "ParseValueToString type not supported ";
			Type type = valueFromFsmVar.GetType();
			Debug.LogWarning(str + ((type != null) ? type.ToString() : null));
			return "<" + fsmVar.Type.ToString() + "> not supported";
		}
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000A960 File Offset: 0x00008B60
	public static string ParseValueToString(object item, bool useBytes)
	{
		return "";
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000A968 File Offset: 0x00008B68
	public static string ParseValueToString(object item)
	{
		if (item.GetType() == typeof(string))
		{
			return "string(" + item.ToString() + ")";
		}
		if (item.GetType() == typeof(bool))
		{
			return "bool(" + (((bool)item) ? 1 : 0).ToString() + ")";
		}
		if (item.GetType() == typeof(float))
		{
			return "float(" + float.Parse(item.ToString()).ToString() + ")";
		}
		if (item.GetType() == typeof(int))
		{
			return "int(" + int.Parse(item.ToString()).ToString() + ")";
		}
		if (item.GetType() == typeof(Vector2))
		{
			Vector2 vector = (Vector2)item;
			return string.Concat(new string[]
			{
				"vector2(",
				vector.x.ToString(),
				",",
				vector.y.ToString(),
				")"
			});
		}
		if (item.GetType() == typeof(Vector3))
		{
			Vector3 vector2 = (Vector3)item;
			return string.Concat(new string[]
			{
				"vector3(",
				vector2.x.ToString(),
				",",
				vector2.y.ToString(),
				",",
				vector2.z.ToString(),
				")"
			});
		}
		if (item.GetType() == typeof(Vector4))
		{
			Vector4 vector3 = (Vector4)item;
			return string.Concat(new string[]
			{
				"vector4(",
				vector3.x.ToString(),
				",",
				vector3.y.ToString(),
				",",
				vector3.z.ToString(),
				",",
				vector3.w.ToString(),
				")"
			});
		}
		if (item.GetType() == typeof(Quaternion))
		{
			Quaternion quaternion = (Quaternion)item;
			return string.Concat(new string[]
			{
				"quaternion(",
				quaternion.x.ToString(),
				",",
				quaternion.y.ToString(),
				",",
				quaternion.z.ToString(),
				",",
				quaternion.w.ToString(),
				")"
			});
		}
		if (item.GetType() == typeof(Rect))
		{
			Rect rect = (Rect)item;
			return string.Concat(new string[]
			{
				"rect(",
				rect.x.ToString(),
				",",
				rect.y.ToString(),
				",",
				rect.width.ToString(),
				",",
				rect.height.ToString(),
				")"
			});
		}
		if (item.GetType() == typeof(Color))
		{
			Color color = (Color)item;
			return string.Concat(new string[]
			{
				"color(",
				color.r.ToString(),
				",",
				color.g.ToString(),
				",",
				color.b.ToString(),
				",",
				color.a.ToString(),
				")"
			});
		}
		if (item.GetType() == typeof(Texture2D))
		{
			byte[] inArray = ((Texture2D)item).EncodeToPNG();
			return "texture(" + Convert.ToBase64String(inArray) + ")";
		}
		if (item.GetType() == typeof(GameObject))
		{
			GameObject gameObject = (GameObject)item;
			return "gameObject(" + gameObject.name + ")";
		}
		string str = "ParseValueToString type not supported ";
		Type type = item.GetType();
		Debug.LogWarning(str + ((type != null) ? type.ToString() : null));
		string str2 = "<";
		Type type2 = item.GetType();
		return str2 + ((type2 != null) ? type2.ToString() : null) + "> not supported";
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000AE2F File Offset: 0x0000902F
	public static object ParseValueFromString(string source, bool useBytes)
	{
		return null;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000AE34 File Offset: 0x00009034
	public static object ParseValueFromString(string source, VariableType type)
	{
		Type typeFromHandle = typeof(string);
		switch (type)
		{
		case VariableType.Unknown:
			return PlayMakerUtils.ParseValueFromString(source);
		case VariableType.Float:
			typeFromHandle = typeof(float);
			break;
		case VariableType.Int:
			typeFromHandle = typeof(int);
			break;
		case VariableType.Bool:
			typeFromHandle = typeof(bool);
			break;
		case VariableType.GameObject:
			typeFromHandle = typeof(GameObject);
			break;
		case VariableType.Vector2:
			typeFromHandle = typeof(Vector2);
			break;
		case VariableType.Vector3:
			typeFromHandle = typeof(Vector3);
			break;
		case VariableType.Color:
			typeFromHandle = typeof(Color);
			break;
		case VariableType.Rect:
			typeFromHandle = typeof(Rect);
			break;
		case VariableType.Quaternion:
			typeFromHandle = typeof(Quaternion);
			break;
		}
		return PlayMakerUtils.ParseValueFromString(source, typeFromHandle);
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000AF10 File Offset: 0x00009110
	public static object ParseValueFromString(string source, Type type)
	{
		if (source == null)
		{
			return null;
		}
		if (type == typeof(string))
		{
			return source;
		}
		if (type == typeof(bool))
		{
			if (string.Equals(source, "true", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals(source, "false", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return int.Parse(source) != 0;
		}
		else
		{
			if (type == typeof(int))
			{
				return int.Parse(source);
			}
			if (type == typeof(float))
			{
				return float.Parse(source);
			}
			if (type == typeof(Vector2))
			{
				string text = "vector2\\([x],[y]\\)";
				string str = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
				text = text.Replace("[x]", "(?<x>" + str + ")");
				text = text.Replace("[y]", "(?<y>" + str + ")");
				text = "^\\s*" + text;
				Match match = new Regex(text).Match(source);
				if (match.Groups["x"].Value != "" && match.Groups["y"].Value != "")
				{
					return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
				}
				return Vector2.zero;
			}
			else if (type == typeof(Vector3))
			{
				string text2 = "vector3\\([x],[y],[z]\\)";
				string str2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
				text2 = text2.Replace("[x]", "(?<x>" + str2 + ")");
				text2 = text2.Replace("[y]", "(?<y>" + str2 + ")");
				text2 = text2.Replace("[z]", "(?<z>" + str2 + ")");
				text2 = "^\\s*" + text2;
				Match match2 = new Regex(text2).Match(source);
				if (match2.Groups["x"].Value != "" && match2.Groups["y"].Value != "" && match2.Groups["z"].Value != "")
				{
					return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
				}
				return Vector3.zero;
			}
			else if (type == typeof(Vector4))
			{
				string text3 = "vector4\\([x],[y],[z],[w]\\)";
				string str3 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
				text3 = text3.Replace("[x]", "(?<x>" + str3 + ")");
				text3 = text3.Replace("[y]", "(?<y>" + str3 + ")");
				text3 = text3.Replace("[z]", "(?<z>" + str3 + ")");
				text3 = text3.Replace("[w]", "(?<w>" + str3 + ")");
				text3 = "^\\s*" + text3;
				Match match3 = new Regex(text3).Match(source);
				if (match3.Groups["x"].Value != "" && match3.Groups["y"].Value != "" && match3.Groups["z"].Value != "" && match3.Groups["z"].Value != "")
				{
					return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
				}
				return Vector4.zero;
			}
			else if (type == typeof(Rect))
			{
				string text4 = "rect\\([x],[y],[w],[h]\\)";
				string str4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
				text4 = text4.Replace("[x]", "(?<x>" + str4 + ")");
				text4 = text4.Replace("[y]", "(?<y>" + str4 + ")");
				text4 = text4.Replace("[w]", "(?<w>" + str4 + ")");
				text4 = text4.Replace("[h]", "(?<h>" + str4 + ")");
				text4 = "^\\s*" + text4;
				Match match4 = new Regex(text4).Match(source);
				if (match4.Groups["x"].Value != "" && match4.Groups["y"].Value != "" && match4.Groups["w"].Value != "" && match4.Groups["h"].Value != "")
				{
					return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
				}
				return new Rect(0f, 0f, 0f, 0f);
			}
			else if (type == typeof(Quaternion))
			{
				string text5 = "quaternion\\([x],[y],[z],[w]\\)";
				string str5 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
				text5 = text5.Replace("[x]", "(?<x>" + str5 + ")");
				text5 = text5.Replace("[y]", "(?<y>" + str5 + ")");
				text5 = text5.Replace("[z]", "(?<z>" + str5 + ")");
				text5 = text5.Replace("[w]", "(?<w>" + str5 + ")");
				text5 = "^\\s*" + text5;
				Match match5 = new Regex(text5).Match(source);
				if (match5.Groups["x"].Value != "" && match5.Groups["y"].Value != "" && match5.Groups["z"].Value != "" && match5.Groups["z"].Value != "")
				{
					return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
				}
				return Quaternion.identity;
			}
			else if (type == typeof(Color))
			{
				string text6 = "color\\([r],[g],[b],[a]\\)";
				string str6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
				text6 = text6.Replace("[r]", "(?<r>" + str6 + ")");
				text6 = text6.Replace("[g]", "(?<g>" + str6 + ")");
				text6 = text6.Replace("[b]", "(?<b>" + str6 + ")");
				text6 = text6.Replace("[a]", "(?<a>" + str6 + ")");
				text6 = "^\\s*" + text6;
				Match match6 = new Regex(text6).Match(source);
				if (match6.Groups["r"].Value != "" && match6.Groups["g"].Value != "" && match6.Groups["b"].Value != "" && match6.Groups["a"].Value != "")
				{
					return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
				}
				return Color.black;
			}
			else
			{
				if (type == typeof(GameObject))
				{
					source = source.Substring(11, source.Length - 12);
					return GameObject.Find(source);
				}
				Debug.LogWarning("ParseValueFromString failed for " + source);
				return null;
			}
		}
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000B9B0 File Offset: 0x00009BB0
	public static object ParseValueFromString(string source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.StartsWith("string("))
		{
			source = source.Substring(7, source.Length - 8);
			return source;
		}
		if (source.StartsWith("bool("))
		{
			source = source.Substring(5, source.Length - 6);
			return int.Parse(source) == 1;
		}
		if (source.StartsWith("int("))
		{
			source = source.Substring(4, source.Length - 5);
			return int.Parse(source);
		}
		if (source.StartsWith("float("))
		{
			source = source.Substring(6, source.Length - 7);
			return float.Parse(source);
		}
		if (source.StartsWith("vector2("))
		{
			string text = "vector2\\([x],[y]\\)";
			string str = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text = text.Replace("[x]", "(?<x>" + str + ")");
			text = text.Replace("[y]", "(?<y>" + str + ")");
			text = "^\\s*" + text;
			Match match = new Regex(text).Match(source);
			if (match.Groups["x"].Value != "" && match.Groups["y"].Value != "")
			{
				return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
			}
			return Vector2.zero;
		}
		else if (source.StartsWith("vector3("))
		{
			string text2 = "vector3\\([x],[y],[z]\\)";
			string str2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text2 = text2.Replace("[x]", "(?<x>" + str2 + ")");
			text2 = text2.Replace("[y]", "(?<y>" + str2 + ")");
			text2 = text2.Replace("[z]", "(?<z>" + str2 + ")");
			text2 = "^\\s*" + text2;
			Match match2 = new Regex(text2).Match(source);
			if (match2.Groups["x"].Value != "" && match2.Groups["y"].Value != "" && match2.Groups["z"].Value != "")
			{
				return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
			}
			return Vector3.zero;
		}
		else if (source.StartsWith("vector4("))
		{
			string text3 = "vector4\\([x],[y],[z],[w]\\)";
			string str3 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text3 = text3.Replace("[x]", "(?<x>" + str3 + ")");
			text3 = text3.Replace("[y]", "(?<y>" + str3 + ")");
			text3 = text3.Replace("[z]", "(?<z>" + str3 + ")");
			text3 = text3.Replace("[w]", "(?<w>" + str3 + ")");
			text3 = "^\\s*" + text3;
			Match match3 = new Regex(text3).Match(source);
			if (match3.Groups["x"].Value != "" && match3.Groups["y"].Value != "" && match3.Groups["z"].Value != "" && match3.Groups["z"].Value != "")
			{
				return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
			}
			return Vector4.zero;
		}
		else if (source.StartsWith("rect("))
		{
			string text4 = "rect\\([x],[y],[w],[h]\\)";
			string str4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text4 = text4.Replace("[x]", "(?<x>" + str4 + ")");
			text4 = text4.Replace("[y]", "(?<y>" + str4 + ")");
			text4 = text4.Replace("[w]", "(?<w>" + str4 + ")");
			text4 = text4.Replace("[h]", "(?<h>" + str4 + ")");
			text4 = "^\\s*" + text4;
			Match match4 = new Regex(text4).Match(source);
			if (match4.Groups["x"].Value != "" && match4.Groups["y"].Value != "" && match4.Groups["w"].Value != "" && match4.Groups["h"].Value != "")
			{
				return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		else if (source.StartsWith("quaternion("))
		{
			string text5 = "quaternion\\([x],[y],[z],[w]\\)";
			string str5 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text5 = text5.Replace("[x]", "(?<x>" + str5 + ")");
			text5 = text5.Replace("[y]", "(?<y>" + str5 + ")");
			text5 = text5.Replace("[z]", "(?<z>" + str5 + ")");
			text5 = text5.Replace("[w]", "(?<w>" + str5 + ")");
			text5 = "^\\s*" + text5;
			Match match5 = new Regex(text5).Match(source);
			if (match5.Groups["x"].Value != "" && match5.Groups["y"].Value != "" && match5.Groups["z"].Value != "" && match5.Groups["z"].Value != "")
			{
				return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
			}
			return Quaternion.identity;
		}
		else if (source.StartsWith("color("))
		{
			string text6 = "color\\([r],[g],[b],[a]\\)";
			string str6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text6 = text6.Replace("[r]", "(?<r>" + str6 + ")");
			text6 = text6.Replace("[g]", "(?<g>" + str6 + ")");
			text6 = text6.Replace("[b]", "(?<b>" + str6 + ")");
			text6 = text6.Replace("[a]", "(?<a>" + str6 + ")");
			text6 = "^\\s*" + text6;
			Match match6 = new Regex(text6).Match(source);
			if (match6.Groups["r"].Value != "" && match6.Groups["g"].Value != "" && match6.Groups["b"].Value != "" && match6.Groups["a"].Value != "")
			{
				return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
			}
			return Color.black;
		}
		else
		{
			if (source.StartsWith("texture("))
			{
				source = source.Substring(8, source.Length - 9);
				byte[] data = Convert.FromBase64String(source);
				Texture2D texture2D = new Texture2D(16, 16);
				texture2D.LoadImage(data);
				return texture2D;
			}
			if (source.StartsWith("gameObject("))
			{
				source = source.Substring(11, source.Length - 12);
				return GameObject.Find(source);
			}
			Debug.LogWarning("ParseValueFromString failed for " + source);
			return null;
		}
	}
}
