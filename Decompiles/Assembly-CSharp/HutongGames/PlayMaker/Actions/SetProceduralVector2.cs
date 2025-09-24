using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FFD RID: 4093
	[ActionCategory("Substance")]
	[Tooltip("Set a named Vector2 property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class SetProceduralVector2 : FsmStateAction
	{
		// Token: 0x0600709D RID: 28829 RVA: 0x0022C12F File Offset: 0x0022A32F
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.vector2Property = null;
			this.vector2Value = null;
			this.everyFrame = false;
		}

		// Token: 0x0600709E RID: 28830 RVA: 0x0022C14D File Offset: 0x0022A34D
		public override void OnEnter()
		{
			this.DoSetProceduralVector();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600709F RID: 28831 RVA: 0x0022C163 File Offset: 0x0022A363
		public override void OnUpdate()
		{
			this.DoSetProceduralVector();
		}

		// Token: 0x060070A0 RID: 28832 RVA: 0x0022C16B File Offset: 0x0022A36B
		private void DoSetProceduralVector()
		{
		}

		// Token: 0x04007068 RID: 28776
		[RequiredField]
		[Tooltip("The Substance Material.")]
		public FsmMaterial substanceMaterial;

		// Token: 0x04007069 RID: 28777
		[RequiredField]
		[Tooltip("The named vector property in the material.")]
		public FsmString vector2Property;

		// Token: 0x0400706A RID: 28778
		[RequiredField]
		[Tooltip("The Vector3 value to set the property to.\nNOTE: Use Set Procedural Vector2 for Vector3 values.")]
		public FsmVector2 vector2Value;

		// Token: 0x0400706B RID: 28779
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
