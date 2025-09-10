using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class RespawnTrigger : MonoBehaviour
{
	// Token: 0x0600071A RID: 1818 RVA: 0x00023368 File Offset: 0x00021568
	private void Awake()
	{
		this.gm = GameManager.instance;
		this.playerData = PlayerData.instance;
		if (this.playerData == null)
		{
			Debug.LogError(base.name + "- Player Data reference is null, please check this is being set correctly.");
		}
		if (this.singleUse)
		{
			this.myFsm = base.GetComponent<PlayMakerFSM>();
			if (this.myFsm == null)
			{
				Debug.LogError(base.name + " - Respawn Trigger set to Single Use but has no PlayMakerFSM attached.");
			}
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x000233DF File Offset: 0x000215DF
	private void Start()
	{
		if (this.respawnMarker == null)
		{
			Debug.LogWarning(base.name + " does not have a Death Respawn Marker Set");
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x00023404 File Offset: 0x00021604
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.layer == 9)
		{
			if (this.singleUse)
			{
				if (!this.myFsm.FsmVariables.GetFsmBool("Activated").Value)
				{
					this.playerData.SetBenchRespawn(this.respawnMarker, this.gm.sceneName, this.respawnType);
					this.myFsm.FsmVariables.GetFsmBool("Activated").Value = true;
					GameManager.instance.SetCurrentMapZoneAsRespawn();
					return;
				}
			}
			else
			{
				this.playerData.SetBenchRespawn(this.respawnMarker, this.gm.sceneName, this.respawnType);
				GameManager.instance.SetCurrentMapZoneAsRespawn();
			}
		}
	}

	// Token: 0x040006E7 RID: 1767
	public RespawnMarker respawnMarker;

	// Token: 0x040006E8 RID: 1768
	[Tooltip("If true, trigger deactivates itself after the first instance the hero uses it.")]
	public bool singleUse;

	// Token: 0x040006E9 RID: 1769
	[Tooltip("0 = face down, 1 = on bench")]
	public int respawnType;

	// Token: 0x040006EA RID: 1770
	private GameManager gm;

	// Token: 0x040006EB RID: 1771
	private PlayerData playerData;

	// Token: 0x040006EC RID: 1772
	private PlayMakerFSM myFsm;
}
