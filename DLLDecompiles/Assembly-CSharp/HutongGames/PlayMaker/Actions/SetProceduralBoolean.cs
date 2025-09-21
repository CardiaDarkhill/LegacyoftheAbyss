using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FFA RID: 4090
	[ActionCategory("Substance")]
	[Tooltip("Set a named bool property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class SetProceduralBoolean : FsmStateAction
	{
		// Token: 0x0600708E RID: 28814 RVA: 0x0022C02B File Offset: 0x0022A22B
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.boolProperty = "";
			this.boolValue = false;
			this.everyFrame = false;
		}

		// Token: 0x0600708F RID: 28815 RVA: 0x0022C057 File Offset: 0x0022A257
		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007090 RID: 28816 RVA: 0x0022C06D File Offset: 0x0022A26D
		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		// Token: 0x06007091 RID: 28817 RVA: 0x0022C075 File Offset: 0x0022A275
		private void DoSetProceduralFloat()
		{
		}

		// Token: 0x0400705C RID: 28764
		[RequiredField]
		[Tooltip("The Substance Material.")]
		public FsmMaterial substanceMaterial;

		// Token: 0x0400705D RID: 28765
		[RequiredField]
		[Tooltip("The named bool property in the material.")]
		public FsmString boolProperty;

		// Token: 0x0400705E RID: 28766
		[RequiredField]
		[Tooltip("The value to set the property to.")]
		public FsmBool boolValue;

		// Token: 0x0400705F RID: 28767
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
