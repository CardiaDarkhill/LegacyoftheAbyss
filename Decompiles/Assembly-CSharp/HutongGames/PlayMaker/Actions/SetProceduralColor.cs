using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FFB RID: 4091
	[ActionCategory("Substance")]
	[Tooltip("Set a named color property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class SetProceduralColor : FsmStateAction
	{
		// Token: 0x06007093 RID: 28819 RVA: 0x0022C07F File Offset: 0x0022A27F
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.colorProperty = "";
			this.colorValue = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x06007094 RID: 28820 RVA: 0x0022C0AF File Offset: 0x0022A2AF
		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007095 RID: 28821 RVA: 0x0022C0C5 File Offset: 0x0022A2C5
		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		// Token: 0x06007096 RID: 28822 RVA: 0x0022C0CD File Offset: 0x0022A2CD
		private void DoSetProceduralFloat()
		{
		}

		// Token: 0x04007060 RID: 28768
		[RequiredField]
		[Tooltip("The Substance Material.")]
		public FsmMaterial substanceMaterial;

		// Token: 0x04007061 RID: 28769
		[RequiredField]
		[Tooltip("The named color property in the material.")]
		public FsmString colorProperty;

		// Token: 0x04007062 RID: 28770
		[RequiredField]
		[Tooltip("The value to set the property to.")]
		public FsmColor colorValue;

		// Token: 0x04007063 RID: 28771
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
