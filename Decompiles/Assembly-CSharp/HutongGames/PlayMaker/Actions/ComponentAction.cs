using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E65 RID: 3685
	public abstract class ComponentAction<T> : FsmStateAction where T : Component
	{
		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x06006924 RID: 26916 RVA: 0x0020FF75 File Offset: 0x0020E175
		// (set) Token: 0x06006925 RID: 26917 RVA: 0x0020FF7D File Offset: 0x0020E17D
		public Transform cachedTransform { get; private set; }

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x06006926 RID: 26918 RVA: 0x0020FF86 File Offset: 0x0020E186
		protected Rigidbody rigidbody
		{
			get
			{
				return this.cachedComponent as Rigidbody;
			}
		}

		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x06006927 RID: 26919 RVA: 0x0020FF98 File Offset: 0x0020E198
		protected Rigidbody2D rigidbody2d
		{
			get
			{
				return this.cachedComponent as Rigidbody2D;
			}
		}

		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x06006928 RID: 26920 RVA: 0x0020FFAA File Offset: 0x0020E1AA
		protected Renderer renderer
		{
			get
			{
				return this.cachedComponent as Renderer;
			}
		}

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x06006929 RID: 26921 RVA: 0x0020FFBC File Offset: 0x0020E1BC
		protected Animation animation
		{
			get
			{
				return this.cachedComponent as Animation;
			}
		}

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x0600692A RID: 26922 RVA: 0x0020FFCE File Offset: 0x0020E1CE
		protected AudioSource audio
		{
			get
			{
				return this.cachedComponent as AudioSource;
			}
		}

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x0600692B RID: 26923 RVA: 0x0020FFE0 File Offset: 0x0020E1E0
		protected Camera camera
		{
			get
			{
				return this.cachedComponent as Camera;
			}
		}

		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x0600692C RID: 26924 RVA: 0x0020FFF2 File Offset: 0x0020E1F2
		protected Light light
		{
			get
			{
				return this.cachedComponent as Light;
			}
		}

		// Token: 0x0600692D RID: 26925 RVA: 0x00210004 File Offset: 0x0020E204
		protected bool UpdateCache(GameObject go)
		{
			if (this.cachedGameObject == go)
			{
				return this.cachedComponent;
			}
			if (go == null)
			{
				return false;
			}
			this.cacheVersion++;
			this.cachedComponent = go.GetComponent<T>();
			this.cachedGameObject = go;
			this.cachedComponent == null;
			return this.cachedComponent != null;
		}

		// Token: 0x0600692E RID: 26926 RVA: 0x00210080 File Offset: 0x0020E280
		protected bool UpdateCachedTransform(GameObject go)
		{
			if (this.cachedGameObject == go)
			{
				return this.cachedTransform;
			}
			if (go == null)
			{
				return false;
			}
			this.cachedTransform = go.transform;
			this.cachedComponent = (this.cachedTransform as T);
			this.cachedGameObject = go;
			return this.cachedTransform != null;
		}

		// Token: 0x0600692F RID: 26927 RVA: 0x002100E7 File Offset: 0x0020E2E7
		protected bool UpdateCacheAndTransform(GameObject go)
		{
			if (!this.UpdateCache(go))
			{
				return false;
			}
			this.cachedTransform = go.transform;
			return true;
		}

		// Token: 0x06006930 RID: 26928 RVA: 0x00210104 File Offset: 0x0020E304
		protected bool UpdateCacheAddComponent(GameObject go)
		{
			if (this.cachedGameObject == go)
			{
				return this.cachedComponent;
			}
			if (go == null)
			{
				return false;
			}
			this.cachedComponent = go.GetComponent<T>();
			this.cachedGameObject = go;
			if (this.cachedComponent == null)
			{
				this.cachedComponent = go.AddComponent<T>();
				this.cachedComponent.hideFlags = HideFlags.DontSaveInEditor;
			}
			return this.cachedComponent != null;
		}

		// Token: 0x06006931 RID: 26929 RVA: 0x0021018F File Offset: 0x0020E38F
		protected void SendEvent(FsmEventTarget eventTarget, FsmEvent fsmEvent)
		{
			base.Fsm.Event(this.cachedGameObject, eventTarget, fsmEvent);
		}

		// Token: 0x04006874 RID: 26740
		protected GameObject cachedGameObject;

		// Token: 0x04006876 RID: 26742
		protected T cachedComponent;

		// Token: 0x04006877 RID: 26743
		protected int cacheVersion;
	}
}
