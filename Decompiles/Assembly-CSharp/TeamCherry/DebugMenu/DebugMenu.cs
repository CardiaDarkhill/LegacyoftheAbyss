using System;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace TeamCherry.DebugMenu
{
	// Token: 0x020008B9 RID: 2233
	public class DebugMenu : MonoBehaviour
	{
		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06004D64 RID: 19812 RVA: 0x0016BA8D File Offset: 0x00169C8D
		// (set) Token: 0x06004D65 RID: 19813 RVA: 0x0016BA94 File Offset: 0x00169C94
		public static bool IsActive { get; private set; }

		// Token: 0x06004D66 RID: 19814 RVA: 0x0016BA9C File Offset: 0x00169C9C
		private void Awake()
		{
			DebugMenu.instance = this;
			DebugMenu.buttonPool = new ObjectPool<DebugMenuButton>(new Func<DebugMenuButton>(this.CreateFunc), new Action<DebugMenuButton>(this.ActionOnGet), new Action<DebugMenuButton>(this.ActionOnRelease), null, true, 10, 10000);
			Object.DontDestroyOnLoad(base.gameObject);
			if (this.buttonParent == null)
			{
				if (this.debugMenuButton)
				{
					this.buttonParent = this.debugMenuButton.transform.parent;
				}
				else
				{
					this.buttonParent = base.transform;
				}
			}
			if (this.debugMenuButton)
			{
				this.debugMenuButton.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004D67 RID: 19815 RVA: 0x0016BB4E File Offset: 0x00169D4E
		private void Start()
		{
			if (!DebugMenu.IsActive)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x0016BB64 File Offset: 0x00169D64
		private void Update()
		{
			if (DebugMenu.activeMenu == null)
			{
				if (DebugMenu.menuStack.Count > 0)
				{
					DebugMenu.activeMenu = DebugMenu.menuStack.Pop();
				}
				else
				{
					DebugMenu.MainMenu mainMenu = new DebugMenu.MainMenu();
					foreach (GameObject root in this.targetRoots)
					{
						mainMenu.AddRootMenu(root);
					}
					DebugMenu.PushMenu(mainMenu);
				}
			}
			if (DebugMenu.activeMenu != null)
			{
				DebugMenu.activeMenu.DoMenu();
			}
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x0016BBFC File Offset: 0x00169DFC
		public static void Show()
		{
			if (!DebugMenu.IsActive && DebugMenu.instance)
			{
				DebugMenu.IsActive = true;
				DebugMenu.instance.gameObject.SetActive(true);
				if (DebugMenu.activeMenu != null)
				{
					DebugMenu.activeMenu.SetDirty();
				}
			}
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x0016BC38 File Offset: 0x00169E38
		public static void Hide()
		{
			if (DebugMenu.IsActive && DebugMenu.instance)
			{
				DebugMenu.IsActive = false;
				DebugMenu.instance.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004D6B RID: 19819 RVA: 0x0016BC63 File Offset: 0x00169E63
		private DebugMenuButton CreateFunc()
		{
			return Object.Instantiate<DebugMenuButton>(this.debugMenuButton, this.buttonParent);
		}

		// Token: 0x06004D6C RID: 19820 RVA: 0x0016BC78 File Offset: 0x00169E78
		private void ActionOnGet(DebugMenuButton obj)
		{
			DebugMenu.activeButtons.Add(obj);
			obj.gameObject.SetActive(true);
			if (EventSystem.current && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.gameObject.activeInHierarchy))
			{
				EventSystem.current.SetSelectedGameObject(obj.gameObject);
			}
			obj.Button.onClick.RemoveAllListeners();
		}

		// Token: 0x06004D6D RID: 19821 RVA: 0x0016BCF0 File Offset: 0x00169EF0
		private void ActionOnRelease(DebugMenuButton obj)
		{
			DebugMenu.activeButtons.Remove(obj);
			obj.gameObject.SetActive(false);
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x0016BD0C File Offset: 0x00169F0C
		public static void ClearMenus()
		{
			for (int i = DebugMenu.activeButtons.Count - 1; i >= 0; i--)
			{
				DebugMenuButton element = DebugMenu.activeButtons[i];
				DebugMenu.buttonPool.Release(element);
			}
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x0016BD47 File Offset: 0x00169F47
		public static void PopMenu()
		{
			if (DebugMenu.activeMenu != null)
			{
				DebugMenu.activeMenu.Exit();
			}
			DebugMenu.activeMenu = DebugMenu.menuStack.Pop();
			DebugMenu.MenuStack menuStack = DebugMenu.activeMenu;
			if (menuStack == null)
			{
				return;
			}
			menuStack.Enter();
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x0016BD78 File Offset: 0x00169F78
		public static void PushMenu(DebugMenu.MenuStack stack)
		{
			if (stack == null)
			{
				return;
			}
			if (DebugMenu.activeMenu != null)
			{
				DebugMenu.activeMenu.Exit();
				DebugMenu.menuStack.Push(DebugMenu.activeMenu);
			}
			DebugMenu.activeMenu = stack;
			DebugMenu.activeMenu.Enter();
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x0016BDAE File Offset: 0x00169FAE
		public static DebugMenuButton GetButton()
		{
			return DebugMenu.buttonPool.Get();
		}

		// Token: 0x06004D72 RID: 19826 RVA: 0x0016BDBC File Offset: 0x00169FBC
		private static void DrawNamedObjects()
		{
			if (DebugMenu.instance)
			{
				DebugMenu.NamedObject[] array = DebugMenu.instance.namedObjects;
				for (int i = 0; i < array.Length; i++)
				{
					DebugMenu.NamedObject namedObject = array[i];
					DebugMenu.GetButton().DoButton(namedObject.name, delegate
					{
						Object.Instantiate<GameObject>(namedObject.gameObject);
					});
				}
			}
		}

		// Token: 0x06004D73 RID: 19827 RVA: 0x0016BE20 File Offset: 0x0016A020
		private static void DrawToggleObjects(Action callback)
		{
			if (DebugMenu.instance)
			{
				using (List<GameObject>.Enumerator enumerator = DebugMenu.instance.toggleableObjects.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameObject namedObject = enumerator.Current;
						if (namedObject)
						{
							DebugMenu.GetButton().DoButton(namedObject.name + " " + (namedObject.activeSelf ? "<color=\"green\">Enabled</color>" : "<color=\"red\">Disabled</color>"), delegate
							{
								Action callback2 = callback;
								if (callback2 != null)
								{
									callback2();
								}
								namedObject.SetActive(!namedObject.activeSelf);
								DebugMenu.MenuStack menuStack = DebugMenu.activeMenu;
								if (menuStack == null)
								{
									return;
								}
								menuStack.SetDirty();
							});
						}
					}
				}
			}
		}

		// Token: 0x06004D74 RID: 19828 RVA: 0x0016BEF0 File Offset: 0x0016A0F0
		private static void DisplayDetailMessage(string message)
		{
			if (DebugMenu.instance)
			{
				DebugMenu.instance.detailsText.text = message;
			}
		}

		// Token: 0x04004E2E RID: 20014
		[FormerlySerializedAs("simpleTextButton")]
		[SerializeField]
		private DebugMenuButton debugMenuButton;

		// Token: 0x04004E2F RID: 20015
		[SerializeField]
		private TextMeshProUGUI detailsText;

		// Token: 0x04004E30 RID: 20016
		[SerializeField]
		private DebugMenu.NamedObject[] namedObjects;

		// Token: 0x04004E31 RID: 20017
		[SerializeField]
		private List<GameObject> toggleableObjects = new List<GameObject>();

		// Token: 0x04004E32 RID: 20018
		[SerializeField]
		private List<GameObject> targetRoots = new List<GameObject>();

		// Token: 0x04004E33 RID: 20019
		[SerializeField]
		private Transform buttonParent;

		// Token: 0x04004E34 RID: 20020
		private static ObjectPool<DebugMenuButton> buttonPool;

		// Token: 0x04004E35 RID: 20021
		private static List<DebugMenuButton> activeButtons = new List<DebugMenuButton>();

		// Token: 0x04004E36 RID: 20022
		private static DebugMenu instance;

		// Token: 0x04004E37 RID: 20023
		private static DebugMenu.MenuStack activeMenu;

		// Token: 0x04004E38 RID: 20024
		private static Stack<DebugMenu.MenuStack> menuStack = new Stack<DebugMenu.MenuStack>();

		// Token: 0x04004E39 RID: 20025
		private GameObject lastSelected;

		// Token: 0x02001B3F RID: 6975
		[Serializable]
		private class NamedObject
		{
			// Token: 0x04009C15 RID: 39957
			public string name;

			// Token: 0x04009C16 RID: 39958
			public GameObject gameObject;
		}

		// Token: 0x02001B40 RID: 6976
		public abstract class MenuStack
		{
			// Token: 0x06009986 RID: 39302 RVA: 0x002B1EF4 File Offset: 0x002B00F4
			public virtual void Enter()
			{
				this.isDirty = true;
			}

			// Token: 0x06009987 RID: 39303 RVA: 0x002B1EFD File Offset: 0x002B00FD
			public virtual void Exit()
			{
				this.isDirty = true;
			}

			// Token: 0x06009988 RID: 39304 RVA: 0x002B1F06 File Offset: 0x002B0106
			public void SetDirty()
			{
				this.isDirty = true;
				this.lastSelected = EventSystem.current.currentSelectedGameObject;
			}

			// Token: 0x06009989 RID: 39305 RVA: 0x002B1F20 File Offset: 0x002B0120
			public void DoMenu()
			{
				this.UpdateInput();
				if (EventSystem.current.currentSelectedGameObject)
				{
					if (EventSystem.current.currentSelectedGameObject.activeInHierarchy)
					{
						this.lastSelected = EventSystem.current.currentSelectedGameObject;
					}
				}
				else if (this.lastSelected && this.lastSelected.activeInHierarchy)
				{
					EventSystem.current.SetSelectedGameObject(this.lastSelected);
				}
				if (!this.isDirty)
				{
					return;
				}
				this.isDirty = false;
				DebugMenu.ClearMenus();
				this.DrawMenu();
				if (this.lastSelected && this.lastSelected.activeInHierarchy)
				{
					EventSystem.current.SetSelectedGameObject(this.lastSelected);
				}
			}

			// Token: 0x0600998A RID: 39306
			protected abstract void UpdateInput();

			// Token: 0x0600998B RID: 39307
			protected abstract void DrawMenu();

			// Token: 0x04009C17 RID: 39959
			protected bool isDirty = true;

			// Token: 0x04009C18 RID: 39960
			protected GameObject lastSelected;

			// Token: 0x04009C19 RID: 39961
			public string name;
		}

		// Token: 0x02001B41 RID: 6977
		public class MainMenu : DebugMenu.MenuStack
		{
			// Token: 0x0600998D RID: 39309 RVA: 0x002B1FE5 File Offset: 0x002B01E5
			public MainMenu()
			{
				this.trophyMenu = new DebugMenu.TrophyMenu();
				this.activityMenu = new DebugMenu.ActivityMenu();
			}

			// Token: 0x0600998E RID: 39310 RVA: 0x002B200E File Offset: 0x002B020E
			public void AddRootMenu(GameObject root)
			{
				if (root)
				{
					this.isDirty = true;
				}
			}

			// Token: 0x0600998F RID: 39311 RVA: 0x002B201F File Offset: 0x002B021F
			protected override void UpdateInput()
			{
			}

			// Token: 0x06009990 RID: 39312 RVA: 0x002B2024 File Offset: 0x002B0224
			protected override void DrawMenu()
			{
				DebugMenu.GetButton().DoButton("Trophy Menu", delegate
				{
					this.isDirty = true;
					DebugMenu.PushMenu(this.trophyMenu);
				});
				DebugMenu.GetButton().DoButton("Activity Menu", delegate
				{
					this.isDirty = true;
					DebugMenu.PushMenu(this.activityMenu);
				});
				using (List<DebugMenu.MenuStack>.Enumerator enumerator = this.menus.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DebugMenu.MenuStack menu = enumerator.Current;
						DebugMenu.GetButton().DoButton(menu.name, delegate
						{
							this.isDirty = true;
							DebugMenu.PushMenu(menu);
						});
					}
				}
			}

			// Token: 0x04009C1A RID: 39962
			private DebugMenu.TrophyMenu trophyMenu;

			// Token: 0x04009C1B RID: 39963
			private DebugMenu.ActivityMenu activityMenu;

			// Token: 0x04009C1C RID: 39964
			private List<DebugMenu.MenuStack> menus = new List<DebugMenu.MenuStack>();
		}

		// Token: 0x02001B42 RID: 6978
		public abstract class SubMenu : DebugMenu.MenuStack
		{
			// Token: 0x06009993 RID: 39315 RVA: 0x002B2104 File Offset: 0x002B0304
			protected void ReturnMenu()
			{
				HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
				if (Platform.Current.GetMenuAction(inputActions, false, false) == Platform.MenuActions.Cancel)
				{
					DebugMenu.PopMenu();
				}
			}
		}

		// Token: 0x02001B43 RID: 6979
		public abstract class NamedSubMenu : DebugMenu.SubMenu
		{
			// Token: 0x06009995 RID: 39317 RVA: 0x002B2139 File Offset: 0x002B0339
			protected NamedSubMenu(string name)
			{
				this.name = name;
			}
		}

		// Token: 0x02001B44 RID: 6980
		public class SaveMenu : DebugMenu.SubMenu
		{
			// Token: 0x06009996 RID: 39318 RVA: 0x002B2148 File Offset: 0x002B0348
			protected override void UpdateInput()
			{
				base.ReturnMenu();
			}

			// Token: 0x06009997 RID: 39319 RVA: 0x002B2150 File Offset: 0x002B0350
			protected override void DrawMenu()
			{
				DebugMenu.DrawNamedObjects();
			}
		}

		// Token: 0x02001B45 RID: 6981
		public class ToggleMenu : DebugMenu.SubMenu
		{
			// Token: 0x06009999 RID: 39321 RVA: 0x002B215F File Offset: 0x002B035F
			protected override void UpdateInput()
			{
				base.ReturnMenu();
			}

			// Token: 0x0600999A RID: 39322 RVA: 0x002B2167 File Offset: 0x002B0367
			protected override void DrawMenu()
			{
				DebugMenu.DrawToggleObjects(delegate
				{
					this.isDirty = true;
				});
			}
		}

		// Token: 0x02001B46 RID: 6982
		public class TargetRootMenu : DebugMenu.SubMenu
		{
			// Token: 0x17001198 RID: 4504
			// (get) Token: 0x0600999D RID: 39325 RVA: 0x002B218B File Offset: 0x002B038B
			public string Name
			{
				get
				{
					return this.rootName;
				}
			}

			// Token: 0x0600999E RID: 39326 RVA: 0x002B2194 File Offset: 0x002B0394
			public TargetRootMenu(GameObject gameObject)
			{
				if (gameObject)
				{
					this.rootName = gameObject.name;
					this.toggleObjects = new List<DebugMenu.TargetRootMenu.Target>();
					for (int i = 0; i < gameObject.transform.childCount; i++)
					{
						GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
						this.toggleObjects.Add(new DebugMenu.TargetRootMenu.Target(gameObject2));
					}
				}
			}

			// Token: 0x0600999F RID: 39327 RVA: 0x002B21FF File Offset: 0x002B03FF
			protected override void UpdateInput()
			{
				base.ReturnMenu();
			}

			// Token: 0x060099A0 RID: 39328 RVA: 0x002B2208 File Offset: 0x002B0408
			protected override void DrawMenu()
			{
				foreach (DebugMenu.TargetRootMenu.Target target in this.toggleObjects)
				{
					GameObject targetGO = target.gameObject;
					DebugMenu.GetButton().DoButton(targetGO.name + " " + (targetGO.activeSelf ? "<color=\"green\">Enabled</color>" : "<color=\"red\">Disabled</color>"), delegate
					{
						targetGO.SetActive(!targetGO.activeSelf);
						DebugMenu.MenuStack activeMenu = DebugMenu.activeMenu;
						if (activeMenu == null)
						{
							return;
						}
						activeMenu.SetDirty();
					});
					if (target.hasAudioSource)
					{
						AudioSource audioSource = target.audioSource;
						DebugMenu.GetButton().DoButton("Play Clip : " + (audioSource.clip ? audioSource.clip.name : "none"), delegate
						{
							audioSource.Play();
						});
					}
				}
			}

			// Token: 0x04009C1D RID: 39965
			private List<DebugMenu.TargetRootMenu.Target> toggleObjects;

			// Token: 0x04009C1E RID: 39966
			private string rootName;

			// Token: 0x02001C31 RID: 7217
			private class Target
			{
				// Token: 0x06009B04 RID: 39684 RVA: 0x002B4E79 File Offset: 0x002B3079
				public Target(GameObject gameObject)
				{
					this.gameObject = gameObject;
					this.audioSource = this.gameObject.GetComponent<AudioSource>();
					this.hasAudioSource = this.audioSource;
				}

				// Token: 0x0400A043 RID: 41027
				public GameObject gameObject;

				// Token: 0x0400A044 RID: 41028
				public AudioSource audioSource;

				// Token: 0x0400A045 RID: 41029
				public bool hasAudioSource;
			}
		}

		// Token: 0x02001B47 RID: 6983
		public class TrophyMenu : DebugMenu.SubMenu
		{
			// Token: 0x060099A1 RID: 39329 RVA: 0x002B2314 File Offset: 0x002B0514
			protected override void UpdateInput()
			{
				base.ReturnMenu();
			}

			// Token: 0x060099A2 RID: 39330 RVA: 0x002B231C File Offset: 0x002B051C
			protected override void DrawMenu()
			{
			}
		}

		// Token: 0x02001B48 RID: 6984
		public class ActivityMenu : DebugMenu.NamedSubMenu
		{
			// Token: 0x060099A4 RID: 39332 RVA: 0x002B2326 File Offset: 0x002B0526
			public ActivityMenu() : base("Activity Menu")
			{
			}

			// Token: 0x060099A5 RID: 39333 RVA: 0x002B2333 File Offset: 0x002B0533
			public override void Enter()
			{
				base.Enter();
			}

			// Token: 0x060099A6 RID: 39334 RVA: 0x002B233B File Offset: 0x002B053B
			protected override void UpdateInput()
			{
				base.ReturnMenu();
			}

			// Token: 0x060099A7 RID: 39335 RVA: 0x002B2343 File Offset: 0x002B0543
			protected override void DrawMenu()
			{
			}
		}
	}
}
