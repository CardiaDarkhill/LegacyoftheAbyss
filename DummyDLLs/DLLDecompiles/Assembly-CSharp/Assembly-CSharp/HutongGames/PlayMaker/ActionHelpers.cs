using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AE5 RID: 2789
	public static class ActionHelpers
	{
		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x06005891 RID: 22673 RVA: 0x001C2771 File Offset: 0x001C0971
		public static Texture2D WhiteTexture
		{
			get
			{
				return Texture2D.whiteTexture;
			}
		}

		// Token: 0x06005892 RID: 22674 RVA: 0x001C2778 File Offset: 0x001C0978
		public static Color BlendColor(ColorBlendMode blendMode, Color c1, Color c2)
		{
			switch (blendMode)
			{
			case ColorBlendMode.Normal:
				return Color.Lerp(c1, c2, c2.a);
			case ColorBlendMode.Multiply:
				return Color.Lerp(c1, c1 * c2, c2.a);
			case ColorBlendMode.Screen:
			{
				Color b = Color.white - (Color.white - c1) * (Color.white - c2);
				return Color.Lerp(c1, b, c2.a);
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x001C27F4 File Offset: 0x001C09F4
		public static bool IsVisible(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			Renderer component = go.GetComponent<Renderer>();
			return component != null && component.isVisible;
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x001C2824 File Offset: 0x001C0A24
		public static bool IsVisible(GameObject go, Camera camera, bool useBounds)
		{
			if (go == null || !camera)
			{
				return false;
			}
			Renderer component = go.GetComponent<Renderer>();
			if (component == null)
			{
				return false;
			}
			if (useBounds)
			{
				return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), component.bounds);
			}
			Vector3 vector = camera.WorldToViewportPoint(go.transform.position);
			return vector.z > 0f && vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f;
		}

		// Token: 0x06005895 RID: 22677 RVA: 0x001C28C0 File Offset: 0x001C0AC0
		public static GameObject GetOwnerDefault(FsmStateAction action, FsmOwnerDefault ownerDefault)
		{
			return action.Fsm.GetOwnerDefaultTarget(ownerDefault);
		}

		// Token: 0x06005896 RID: 22678 RVA: 0x001C28D0 File Offset: 0x001C0AD0
		public static PlayMakerFSM GetGameObjectFsm(GameObject go, string fsmName)
		{
			if (!string.IsNullOrEmpty(fsmName))
			{
				foreach (PlayMakerFSM playMakerFSM in go.GetComponents<PlayMakerFSM>())
				{
					if (playMakerFSM.FsmName == fsmName)
					{
						return playMakerFSM;
					}
				}
			}
			return go.GetComponent<PlayMakerFSM>();
		}

		// Token: 0x06005897 RID: 22679 RVA: 0x001C2914 File Offset: 0x001C0B14
		public static int GetRandomWeightedIndex(FsmFloat[] weights)
		{
			float num = 0f;
			foreach (FsmFloat fsmFloat in weights)
			{
				num += fsmFloat.Value;
			}
			float num2 = Random.Range(0f, num);
			for (int j = 0; j < weights.Length; j++)
			{
				if (num2 < weights[j].Value)
				{
					return j;
				}
				num2 -= weights[j].Value;
			}
			return -1;
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x001C2980 File Offset: 0x001C0B80
		public static void AddAnimationClip(GameObject go, AnimationClip animClip)
		{
			if (animClip == null)
			{
				return;
			}
			Animation component = go.GetComponent<Animation>();
			if (component != null)
			{
				component.AddClip(animClip, animClip.name);
			}
		}

		// Token: 0x06005899 RID: 22681 RVA: 0x001C29B4 File Offset: 0x001C0BB4
		public static bool HasAnimationFinished(AnimationState anim, float prevTime, float currentTime)
		{
			return anim.wrapMode != WrapMode.Loop && anim.wrapMode != WrapMode.PingPong && (((anim.wrapMode == WrapMode.Default || anim.wrapMode == WrapMode.Once) && prevTime > 0f && currentTime.Equals(0f)) || (prevTime < anim.length && currentTime >= anim.length));
		}

		// Token: 0x0600589A RID: 22682 RVA: 0x001C2A18 File Offset: 0x001C0C18
		public static Vector3 GetPosition(FsmGameObject fsmGameObject, FsmVector3 fsmVector3)
		{
			Vector3 result;
			if (fsmGameObject.Value != null)
			{
				result = ((!fsmVector3.IsNone) ? fsmGameObject.Value.transform.TransformPoint(fsmVector3.Value) : fsmGameObject.Value.transform.position);
			}
			else
			{
				result = fsmVector3.Value;
			}
			return result;
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x001C2A6E File Offset: 0x001C0C6E
		public static Vector3 GetDeviceAcceleration()
		{
			return Input.acceleration;
		}

		// Token: 0x0600589C RID: 22684 RVA: 0x001C2A75 File Offset: 0x001C0C75
		public static Vector3 GetMousePosition()
		{
			return Input.mousePosition;
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x001C2A7C File Offset: 0x001C0C7C
		public static bool AnyKeyDown()
		{
			return Input.anyKeyDown;
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001C2A83 File Offset: 0x001C0C83
		public static bool IsMouseOver(GameObject gameObject, float distance, int layerMask)
		{
			return !(gameObject == null) && gameObject == ActionHelpers.MouseOver(distance, layerMask);
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x001C2A9D File Offset: 0x001C0C9D
		public static RaycastHit MousePick(float distance, int layerMask)
		{
			if (!ActionHelpers.mousePickRaycastTime.Equals((float)Time.frameCount) || ActionHelpers.mousePickDistanceUsed < distance || ActionHelpers.mousePickLayerMaskUsed != layerMask)
			{
				ActionHelpers.DoMousePick(distance, layerMask);
			}
			return ActionHelpers.mousePickInfo;
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x001C2AD0 File Offset: 0x001C0CD0
		public static GameObject MouseOver(float distance, int layerMask)
		{
			if (!ActionHelpers.mousePickRaycastTime.Equals((float)Time.frameCount) || ActionHelpers.mousePickDistanceUsed < distance || ActionHelpers.mousePickLayerMaskUsed != layerMask)
			{
				ActionHelpers.DoMousePick(distance, layerMask);
			}
			if (ActionHelpers.mousePickInfo.collider != null && ActionHelpers.mousePickInfo.distance < distance)
			{
				return ActionHelpers.mousePickInfo.collider.gameObject;
			}
			return null;
		}

		// Token: 0x060058A1 RID: 22689 RVA: 0x001C2B38 File Offset: 0x001C0D38
		private static void DoMousePick(float distance, int layerMask)
		{
			if (Camera.main == null)
			{
				return;
			}
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ActionHelpers.mousePickInfo, distance, layerMask);
			ActionHelpers.mousePickLayerMaskUsed = layerMask;
			ActionHelpers.mousePickDistanceUsed = distance;
			ActionHelpers.mousePickRaycastTime = (float)Time.frameCount;
		}

		// Token: 0x060058A2 RID: 22690 RVA: 0x001C2B88 File Offset: 0x001C0D88
		public static int LayerArrayToLayerMask(FsmInt[] layers, bool invert)
		{
			int num = 0;
			foreach (FsmInt fsmInt in layers)
			{
				num |= 1 << fsmInt.Value;
			}
			if (invert)
			{
				num = ~num;
			}
			if (num != 0)
			{
				return num;
			}
			return -5;
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x001C2BC6 File Offset: 0x001C0DC6
		public static bool IsLoopingWrapMode(WrapMode wrapMode)
		{
			return wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPong;
		}

		// Token: 0x060058A4 RID: 22692 RVA: 0x001C2BD2 File Offset: 0x001C0DD2
		public static string CheckRayDistance(float rayDistance)
		{
			if (rayDistance > 0f)
			{
				return "";
			}
			return "Ray Distance should be greater than zero!\n";
		}

		// Token: 0x060058A5 RID: 22693 RVA: 0x001C2BE8 File Offset: 0x001C0DE8
		public static string CheckForValidEvent(FsmState state, string eventName)
		{
			if (state == null)
			{
				return "Invalid State!";
			}
			if (string.IsNullOrEmpty(eventName))
			{
				return "";
			}
			FsmTransition[] array = state.Fsm.GlobalTransitions;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].EventName == eventName)
				{
					return "";
				}
			}
			array = state.Transitions;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].EventName == eventName)
				{
					return "";
				}
			}
			return "Fsm will not respond to Event: " + eventName;
		}

		// Token: 0x060058A6 RID: 22694 RVA: 0x001C2C72 File Offset: 0x001C0E72
		public static string CheckPhysicsSetup(FsmOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "";
			}
			return ActionHelpers.CheckPhysicsSetup(ownerDefault.GameObject.Value);
		}

		// Token: 0x060058A7 RID: 22695 RVA: 0x001C2C8D File Offset: 0x001C0E8D
		public static string CheckOwnerPhysicsSetup(GameObject gameObject)
		{
			return ActionHelpers.CheckPhysicsSetup(gameObject);
		}

		// Token: 0x060058A8 RID: 22696 RVA: 0x001C2C98 File Offset: 0x001C0E98
		public static string CheckPhysicsSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<Rigidbody>() == null)
			{
				text += "GameObject requires RigidBody/Collider!\n";
			}
			return text;
		}

		// Token: 0x060058A9 RID: 22697 RVA: 0x001C2CDD File Offset: 0x001C0EDD
		public static string CheckPhysics2dSetup(FsmOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "";
			}
			return ActionHelpers.CheckPhysics2dSetup(ownerDefault.GameObject.Value);
		}

		// Token: 0x060058AA RID: 22698 RVA: 0x001C2CF8 File Offset: 0x001C0EF8
		public static string CheckOwnerPhysics2dSetup(GameObject gameObject)
		{
			return ActionHelpers.CheckPhysics2dSetup(gameObject);
		}

		// Token: 0x060058AB RID: 22699 RVA: 0x001C2D00 File Offset: 0x001C0F00
		public static string CheckPhysics2dSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider2D>() == null && gameObject.GetComponent<Rigidbody2D>() == null)
			{
				text += "GameObject requires a RigidBody2D or Collider2D component!\n";
			}
			return text;
		}

		// Token: 0x060058AC RID: 22700 RVA: 0x001C2D48 File Offset: 0x001C0F48
		public static void DebugLog(Fsm fsm, LogLevel logLevel, string text, bool sendToUnityLog = false)
		{
		}

		// Token: 0x060058AD RID: 22701 RVA: 0x001C2D55 File Offset: 0x001C0F55
		public static void LogError(string text)
		{
			ActionHelpers.DebugLog(FsmExecutionStack.ExecutingFsm, LogLevel.Error, text, true);
		}

		// Token: 0x060058AE RID: 22702 RVA: 0x001C2D64 File Offset: 0x001C0F64
		public static void LogWarning(string text)
		{
			ActionHelpers.DebugLog(FsmExecutionStack.ExecutingFsm, LogLevel.Warning, text, true);
		}

		// Token: 0x060058AF RID: 22703 RVA: 0x001C2D74 File Offset: 0x001C0F74
		public static string FormatUnityLogString(string text)
		{
			if (FsmExecutionStack.ExecutingFsm == null)
			{
				return text;
			}
			string str = Fsm.GetFullFsmLabel(FsmExecutionStack.ExecutingFsm);
			if (FsmExecutionStack.ExecutingState != null)
			{
				str = str + " : " + FsmExecutionStack.ExecutingStateName;
			}
			if (FsmExecutionStack.ExecutingAction != null)
			{
				str += FsmExecutionStack.ExecutingAction.Name;
			}
			return str + " : " + text;
		}

		// Token: 0x060058B0 RID: 22704 RVA: 0x001C2DD4 File Offset: 0x001C0FD4
		public static string StripTags(string textWithTags)
		{
			textWithTags = textWithTags.Replace("<i>", "");
			textWithTags = textWithTags.Replace("</i>", "");
			textWithTags = textWithTags.Replace("<b>", "");
			textWithTags = textWithTags.Replace("</b>", "");
			return textWithTags;
		}

		// Token: 0x060058B1 RID: 22705 RVA: 0x001C2E2A File Offset: 0x001C102A
		public static string GetValueLabel(INamedVariable variable)
		{
			return "";
		}

		// Token: 0x060058B2 RID: 22706 RVA: 0x001C2E31 File Offset: 0x001C1031
		public static string GetValueLabel(Fsm fsm, FsmOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "[null]";
			}
			if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return "Owner";
			}
			return ActionHelpers.GetValueLabel(ownerDefault.GameObject);
		}

		// Token: 0x060058B3 RID: 22707 RVA: 0x001C2E55 File Offset: 0x001C1055
		public static string AutoName(FsmStateAction action, params INamedVariable[] exposedFields)
		{
			if (action != null)
			{
				return ActionHelpers.AutoName(action.GetType().Name, exposedFields);
			}
			return null;
		}

		// Token: 0x060058B4 RID: 22708 RVA: 0x001C2E6D File Offset: 0x001C106D
		public static string AutoName(FsmStateAction action, Fsm fsm, FsmOwnerDefault ownerDefault)
		{
			if (action != null)
			{
				return ActionHelpers.AutoName(action.GetType().Name, new string[]
				{
					ActionHelpers.GetValueLabel(fsm, ownerDefault)
				});
			}
			return null;
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x001C2E94 File Offset: 0x001C1094
		public static string AutoName(FsmStateAction action, params string[] labels)
		{
			if (action != null)
			{
				return ActionHelpers.AutoName(action.GetType().Name, labels);
			}
			return null;
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x001C2EAC File Offset: 0x001C10AC
		public static string AutoName(FsmStateAction action, FsmEvent fsmEvent)
		{
			if (action != null)
			{
				return ActionHelpers.AutoName(action.GetType().Name, new string[]
				{
					(fsmEvent != null) ? fsmEvent.Name : "None"
				});
			}
			return null;
		}

		// Token: 0x060058B7 RID: 22711 RVA: 0x001C2EDC File Offset: 0x001C10DC
		public static string AutoName(string actionName, params INamedVariable[] exposedFields)
		{
			string text = actionName + ": ";
			foreach (INamedVariable variable in exposedFields)
			{
				text = text + ActionHelpers.GetValueLabel(variable) + " ";
			}
			return text;
		}

		// Token: 0x060058B8 RID: 22712 RVA: 0x001C2F1C File Offset: 0x001C111C
		public static string AutoName(string actionName, params string[] labels)
		{
			string text = actionName + ": ";
			foreach (string str in labels)
			{
				text = text + str + " ";
			}
			return text;
		}

		// Token: 0x060058B9 RID: 22713 RVA: 0x001C2F57 File Offset: 0x001C1157
		public static string AutoName(FsmStateAction action, Fsm fsm, FsmOwnerDefault target, params INamedVariable[] exposedFields)
		{
			if (action != null)
			{
				return ActionHelpers.AutoName(action.GetType().Name, fsm, target, exposedFields);
			}
			return null;
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x001C2F74 File Offset: 0x001C1174
		public static string AutoName(string actionName, Fsm fsm, FsmOwnerDefault target, params INamedVariable[] exposedFields)
		{
			string text = actionName + ": " + ActionHelpers.GetValueLabel(fsm, target) + " ";
			foreach (INamedVariable variable in exposedFields)
			{
				text = text + ActionHelpers.GetValueLabel(variable) + " ";
			}
			return text;
		}

		// Token: 0x060058BB RID: 22715 RVA: 0x001C2FC0 File Offset: 0x001C11C0
		public static string AutoNameRange(FsmStateAction action, NamedVariable min, NamedVariable max)
		{
			if (action != null)
			{
				return ActionHelpers.AutoNameRange(action.GetType().Name, min, max);
			}
			return null;
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x001C2FD9 File Offset: 0x001C11D9
		public static string AutoNameRange(string actionName, NamedVariable min, NamedVariable max)
		{
			return string.Concat(new string[]
			{
				actionName,
				": ",
				ActionHelpers.GetValueLabel(min),
				" - ",
				ActionHelpers.GetValueLabel(max)
			});
		}

		// Token: 0x060058BD RID: 22717 RVA: 0x001C300C File Offset: 0x001C120C
		public static string AutoNameSetVar(FsmStateAction action, NamedVariable var, NamedVariable value)
		{
			if (action != null)
			{
				return ActionHelpers.AutoNameSetVar(action.GetType().Name, var, value);
			}
			return null;
		}

		// Token: 0x060058BE RID: 22718 RVA: 0x001C3025 File Offset: 0x001C1225
		public static string AutoNameSetVar(string actionName, NamedVariable var, NamedVariable value)
		{
			return string.Concat(new string[]
			{
				actionName,
				": ",
				ActionHelpers.GetValueLabel(var),
				" = ",
				ActionHelpers.GetValueLabel(value)
			});
		}

		// Token: 0x060058BF RID: 22719 RVA: 0x001C3058 File Offset: 0x001C1258
		public static string AutoNameConvert(FsmStateAction action, NamedVariable fromVariable, NamedVariable toVariable)
		{
			if (action != null)
			{
				return ActionHelpers.AutoNameConvert(action.GetType().Name, fromVariable, toVariable);
			}
			return null;
		}

		// Token: 0x060058C0 RID: 22720 RVA: 0x001C3074 File Offset: 0x001C1274
		public static string AutoNameConvert(string actionName, NamedVariable fromVariable, NamedVariable toVariable)
		{
			return string.Concat(new string[]
			{
				actionName.Replace("Convert", ""),
				": ",
				ActionHelpers.GetValueLabel(fromVariable),
				" to ",
				ActionHelpers.GetValueLabel(toVariable)
			});
		}

		// Token: 0x060058C1 RID: 22721 RVA: 0x001C30C1 File Offset: 0x001C12C1
		public static string AutoNameGetProperty(FsmStateAction action, NamedVariable property, NamedVariable store)
		{
			if (action != null)
			{
				return ActionHelpers.AutoNameGetProperty(action.GetType().Name, property, store);
			}
			return null;
		}

		// Token: 0x060058C2 RID: 22722 RVA: 0x001C30DA File Offset: 0x001C12DA
		public static string AutoNameGetProperty(string actionName, NamedVariable property, NamedVariable store)
		{
			return string.Concat(new string[]
			{
				actionName,
				": ",
				ActionHelpers.GetValueLabel(property),
				" -> ",
				ActionHelpers.GetValueLabel(store)
			});
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x001C310D File Offset: 0x001C130D
		[Obsolete("Use LogError instead.")]
		public static void RuntimeError(FsmStateAction action, string error)
		{
			action.LogError(((action != null) ? action.ToString() : null) + ": " + error);
		}

		// Token: 0x040053FF RID: 21503
		public static RaycastHit mousePickInfo;

		// Token: 0x04005400 RID: 21504
		private static float mousePickRaycastTime;

		// Token: 0x04005401 RID: 21505
		private static float mousePickDistanceUsed;

		// Token: 0x04005402 RID: 21506
		private static int mousePickLayerMaskUsed;

		// Token: 0x04005403 RID: 21507
		public const string colon = ": ";
	}
}
