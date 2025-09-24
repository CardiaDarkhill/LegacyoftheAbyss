using System;
using UnityEngine;

// Token: 0x020005DC RID: 1500
public class SceneImporter : MonoBehaviour
{
	// Token: 0x040038A8 RID: 14504
	public string xml_folder = "./Assets/_Porting/Scene XML/";

	// Token: 0x040038A9 RID: 14505
	public string prefabs_folder = "Prefabs/";

	// Token: 0x040038AA RID: 14506
	public string xml_doc_filename;

	// Token: 0x040038AB RID: 14507
	public string level_name;

	// Token: 0x040038AC RID: 14508
	public int tile_size;

	// Token: 0x040038AD RID: 14509
	public int scene_width;

	// Token: 0x040038AE RID: 14510
	public int scene_height;

	// Token: 0x040038AF RID: 14511
	public int layer_count;

	// Token: 0x040038B0 RID: 14512
	public GameObject placeholder_prefab;

	// Token: 0x040038B1 RID: 14513
	public int importMode = 1;

	// Token: 0x040038B2 RID: 14514
	public bool lookForPrefabsFirst = true;

	// Token: 0x040038B3 RID: 14515
	public bool debug_output;
}
