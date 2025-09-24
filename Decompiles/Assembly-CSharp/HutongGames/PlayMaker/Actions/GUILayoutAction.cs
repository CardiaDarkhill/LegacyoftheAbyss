using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE0 RID: 3808
	[Tooltip("GUILayout base action - don't use!")]
	public abstract class GUILayoutAction : FsmStateAction
	{
		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x06006B18 RID: 27416 RVA: 0x00215D34 File Offset: 0x00213F34
		public GUILayoutOption[] LayoutOptions
		{
			get
			{
				if (this.options == null)
				{
					this.options = new GUILayoutOption[this.layoutOptions.Length];
					for (int i = 0; i < this.layoutOptions.Length; i++)
					{
						this.options[i] = this.layoutOptions[i].GetGUILayoutOption();
					}
				}
				return this.options;
			}
		}

		// Token: 0x06006B19 RID: 27417 RVA: 0x00215D8A File Offset: 0x00213F8A
		public override void Reset()
		{
			this.layoutOptions = new LayoutOption[0];
		}

		// Token: 0x04006A5F RID: 27231
		[Tooltip("An array of layout options.See <a href=\"http://unity3d.com/support/documentation/ScriptReference/GUILayoutOption.html\" rel=\"nofollow\">GUILayoutOption</a>.")]
		public LayoutOption[] layoutOptions;

		// Token: 0x04006A60 RID: 27232
		private GUILayoutOption[] options;
	}
}
