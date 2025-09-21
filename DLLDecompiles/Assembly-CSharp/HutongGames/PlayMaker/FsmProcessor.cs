using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF3 RID: 2803
	[Preserve]
	public class FsmProcessor
	{
		// Token: 0x060058F1 RID: 22769 RVA: 0x001C3780 File Offset: 0x001C1980
		public static void OnPreprocess(PlayMakerFSM fsm)
		{
			if (fsm.Fsm.HandleLegacyNetworking && !FsmProcessor.AddEventHandlerComponent(fsm, ReflectionUtils.GetGlobalType("PlayMakerLegacyNetworking")))
			{
				Debug.LogError("Could not add PlayMakerLegacyNetworking proxy!");
			}
			if (fsm.Fsm.HandleUiEvents != UiEvents.None)
			{
				FsmProcessor.HandleUiEvent<PlayMakerUiClickEvent>(fsm, UiEvents.Click);
				FsmProcessor.HandleUiEvent<PlayMakerUiDragEvents>(fsm, UiEvents.DragEvents);
				FsmProcessor.HandleUiEvent<PlayMakerUiDropEvent>(fsm, UiEvents.Drop);
				FsmProcessor.HandleUiEvent<PlayMakerUiPointerEvents>(fsm, UiEvents.PointerEvents);
				FsmProcessor.HandleUiEvent<PlayMakerUiBoolValueChangedEvent>(fsm, UiEvents.BoolValueChanged);
				FsmProcessor.HandleUiEvent<PlayMakerUiFloatValueChangedEvent>(fsm, UiEvents.FloatValueChanged);
				FsmProcessor.HandleUiEvent<PlayMakerUiIntValueChangedEvent>(fsm, UiEvents.IntValueChanged);
				FsmProcessor.HandleUiEvent<PlayMakerUiVector2ValueChangedEvent>(fsm, UiEvents.Vector2ValueChanged);
				FsmProcessor.HandleUiEvent<PlayMakerUiEndEditEvent>(fsm, UiEvents.EndEdit);
			}
		}

		// Token: 0x060058F2 RID: 22770 RVA: 0x001C381C File Offset: 0x001C1A1C
		private static void HandleUiEvent<T>(PlayMakerFSM fsm, UiEvents uiEvent) where T : PlayMakerUiEventBase
		{
			if ((fsm.Fsm.HandleUiEvents & uiEvent) != UiEvents.None)
			{
				FsmProcessor.AddUiEventHandler<T>(fsm);
			}
		}

		// Token: 0x060058F3 RID: 22771 RVA: 0x001C3834 File Offset: 0x001C1A34
		private static void AddUiEventHandler<T>(PlayMakerFSM fsm) where T : PlayMakerUiEventBase
		{
			T t = fsm.GetComponent<T>();
			if (t == null)
			{
				t = fsm.gameObject.AddComponent<T>();
				if (!PlayMakerPrefs.ShowEventHandlerComponents)
				{
					t.hideFlags = HideFlags.HideInInspector;
				}
			}
			t.AddTargetFsm(fsm);
		}

		// Token: 0x060058F4 RID: 22772 RVA: 0x001C3884 File Offset: 0x001C1A84
		private static bool AddEventHandlerComponent(PlayMakerFSM fsm, Type type)
		{
			if (type == null)
			{
				return false;
			}
			PlayMakerProxyBase eventHandlerComponent = FsmProcessor.GetEventHandlerComponent(fsm.gameObject, type);
			if (eventHandlerComponent == null)
			{
				return false;
			}
			eventHandlerComponent.AddTarget(fsm);
			if (!PlayMakerGlobals.IsEditor)
			{
				bool logPerformanceWarnings = PlayMakerPrefs.LogPerformanceWarnings;
			}
			return true;
		}

		// Token: 0x060058F5 RID: 22773 RVA: 0x001C38CC File Offset: 0x001C1ACC
		public static PlayMakerProxyBase GetEventHandlerComponent(GameObject go, Type type)
		{
			if (go == null)
			{
				return null;
			}
			Component component = go.GetComponent(type);
			if (component == null)
			{
				component = go.AddComponent(type);
				if (!PlayMakerPrefs.ShowEventHandlerComponents)
				{
					component.hideFlags = HideFlags.HideInInspector;
				}
			}
			return component as PlayMakerProxyBase;
		}
	}
}
