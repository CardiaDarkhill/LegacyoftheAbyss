using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FFC RID: 4092
	[ActionCategory("Substance")]
	[Tooltip("Set a named float property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class SetProceduralFloat : FsmStateAction
	{
		// Token: 0x06007098 RID: 28824 RVA: 0x0022C0D7 File Offset: 0x0022A2D7
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.floatProperty = "";
			this.floatValue = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06007099 RID: 28825 RVA: 0x0022C107 File Offset: 0x0022A307
		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600709A RID: 28826 RVA: 0x0022C11D File Offset: 0x0022A31D
		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		// Token: 0x0600709B RID: 28827 RVA: 0x0022C125 File Offset: 0x0022A325
		private void DoSetProceduralFloat()
		{
		}

		// Token: 0x04007064 RID: 28772
		[RequiredField]
		[Tooltip("The Substance Material.")]
		public FsmMaterial substanceMaterial;

		// Token: 0x04007065 RID: 28773
		[RequiredField]
		[Tooltip("The named float property in the material.")]
		public FsmString floatProperty;

		// Token: 0x04007066 RID: 28774
		[RequiredField]
		[Tooltip("The value to set the property to.")]
		public FsmFloat floatValue;

		// Token: 0x04007067 RID: 28775
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
