using System;
using HutongGames.PlayMaker;

// Token: 0x020003EF RID: 1007
[ActionCategory("Hollow Knight")]
public class SetSpawnJarContents : FsmStateAction
{
	// Token: 0x0600226A RID: 8810 RVA: 0x0009E7C3 File Offset: 0x0009C9C3
	public override void Reset()
	{
		this.storedObject = null;
		this.enemyPrefab = null;
		this.enemyHealth = null;
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x0009E7DC File Offset: 0x0009C9DC
	public override void OnEnter()
	{
		if (this.storedObject.Value)
		{
			SpawnJarControl component = this.storedObject.Value.GetComponent<SpawnJarControl>();
			if (component)
			{
				component.SetEnemySpawn(this.enemyPrefab.Value, this.enemyHealth.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04002139 RID: 8505
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storedObject;

	// Token: 0x0400213A RID: 8506
	public FsmGameObject enemyPrefab;

	// Token: 0x0400213B RID: 8507
	public FsmInt enemyHealth;
}
