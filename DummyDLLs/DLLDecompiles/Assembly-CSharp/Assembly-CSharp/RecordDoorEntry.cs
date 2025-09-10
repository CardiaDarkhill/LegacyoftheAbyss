using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005B4 RID: 1460
public class RecordDoorEntry : MonoBehaviour
{
	// Token: 0x06003475 RID: 13429 RVA: 0x000E940F File Offset: 0x000E760F
	private void Reset()
	{
		this.door = base.GetComponent<TransitionPoint>();
	}

	// Token: 0x06003476 RID: 13430 RVA: 0x000E941D File Offset: 0x000E761D
	private void Awake()
	{
		this.door.OnBeforeTransition += delegate()
		{
			PlayerData instance = PlayerData.instance;
			instance.SetVariable(this.pdFromSceneName, base.gameObject.scene.name);
			if (this.isMazeEntrance)
			{
				instance.PreviousMazeScene = base.gameObject.scene.name;
				instance.PreviousMazeDoor = this.door.gameObject.name;
				instance.PreviousMazeTargetDoor = this.door.entryPoint;
			}
		};
	}

	// Token: 0x040037EE RID: 14318
	[SerializeField]
	[InspectorValidation]
	private TransitionPoint door;

	// Token: 0x040037EF RID: 14319
	[SerializeField]
	[PlayerDataField(typeof(string), true)]
	private string pdFromSceneName;

	// Token: 0x040037F0 RID: 14320
	[SerializeField]
	private bool isMazeEntrance;
}
