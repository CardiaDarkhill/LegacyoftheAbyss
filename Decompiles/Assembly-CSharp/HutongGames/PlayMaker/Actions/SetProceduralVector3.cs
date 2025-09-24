using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FFE RID: 4094
	[ActionCategory("Substance")]
	[Tooltip("Set a named Vector3 property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class SetProceduralVector3 : FsmStateAction
	{
		// Token: 0x060070A2 RID: 28834 RVA: 0x0022C175 File Offset: 0x0022A375
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.vector3Property = null;
			this.vector3Value = null;
			this.everyFrame = false;
		}

		// Token: 0x060070A3 RID: 28835 RVA: 0x0022C193 File Offset: 0x0022A393
		public override void OnEnter()
		{
			this.DoSetProceduralVector();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070A4 RID: 28836 RVA: 0x0022C1A9 File Offset: 0x0022A3A9
		public override void OnUpdate()
		{
			this.DoSetProceduralVector();
		}

		// Token: 0x060070A5 RID: 28837 RVA: 0x0022C1B1 File Offset: 0x0022A3B1
		private void DoSetProceduralVector()
		{
		}

		// Token: 0x0400706C RID: 28780
		[RequiredField]
		[Tooltip("The Substance Material.")]
		public FsmMaterial substanceMaterial;

		// Token: 0x0400706D RID: 28781
		[RequiredField]
		[Tooltip("The named vector property in the material.")]
		public FsmString vector3Property;

		// Token: 0x0400706E RID: 28782
		[RequiredField]
		[Tooltip("The value to set the property to.\nNOTE: Use Set Procedural Vector3 for Vector3 values.")]
		public FsmVector3 vector3Value;

		// Token: 0x0400706F RID: 28783
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
