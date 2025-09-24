using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002E3 RID: 739
[RequireComponent(typeof(tk2dSprite))]
public class EnemySpawner : MonoBehaviour
{
	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06001A29 RID: 6697 RVA: 0x00078748 File Offset: 0x00076948
	// (remove) Token: 0x06001A2A RID: 6698 RVA: 0x00078780 File Offset: 0x00076980
	public event Action<GameObject> OnEnemySpawned;

	// Token: 0x06001A2B RID: 6699 RVA: 0x000787B8 File Offset: 0x000769B8
	private void Awake()
	{
		this.sprite = base.GetComponent<tk2dSprite>();
		this.sprite.color = this.startColor;
		if (this.enemyPrefab)
		{
			this.spawnedEnemy = Object.Instantiate<GameObject>(this.enemyPrefab);
			this.spawnedEnemy.SetActive(false);
		}
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x0007880C File Offset: 0x00076A0C
	private void Start()
	{
		if (Random.Range(0f, 1f) <= this.spawnChance)
		{
			if (this.killEvent)
			{
				this.killEvent.ReceivedEvent += delegate()
				{
					base.gameObject.SetActive(false);
				};
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", this.moveBy);
			hashtable.Add("time", this.easeTime);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("space", Space.World);
			iTween.MoveBy(base.gameObject, hashtable);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x000788CC File Offset: 0x00076ACC
	private void Update()
	{
		if (!this.isComplete)
		{
			this.elapsed += Time.deltaTime;
			this.sprite.color = Color.Lerp(this.startColor, this.endColor, Mathf.Clamp(this.elapsed / this.easeTime, 0f, 1f));
			if (this.elapsed > this.easeTime)
			{
				this.isComplete = true;
				this.spawnedEnemy.transform.position = base.transform.position;
				this.spawnedEnemy.transform.localScale = base.transform.localScale;
				this.spawnedEnemy.SetActive(true);
				if (this.OnEnemySpawned != null)
				{
					this.OnEnemySpawned(this.spawnedEnemy);
				}
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(this.spawnedEnemy, "chaser");
				if (playMakerFSM)
				{
					playMakerFSM.FsmVariables.FindFsmBool("Start Alert").Value = true;
				}
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04001917 RID: 6423
	public GameObject enemyPrefab;

	// Token: 0x04001918 RID: 6424
	private GameObject spawnedEnemy;

	// Token: 0x04001919 RID: 6425
	[Range(0f, 1f)]
	public float spawnChance = 0.75f;

	// Token: 0x0400191A RID: 6426
	public iTween.EaseType easeType = iTween.EaseType.easeOutSine;

	// Token: 0x0400191B RID: 6427
	public Vector3 moveBy = new Vector3(0f, -8f, -16.98f);

	// Token: 0x0400191C RID: 6428
	public float easeTime = 1f;

	// Token: 0x0400191D RID: 6429
	private float elapsed;

	// Token: 0x0400191E RID: 6430
	private bool isComplete;

	// Token: 0x0400191F RID: 6431
	public Color startColor = Color.black;

	// Token: 0x04001920 RID: 6432
	public Color endColor = Color.white;

	// Token: 0x04001921 RID: 6433
	public EventRegister killEvent;

	// Token: 0x04001922 RID: 6434
	private tk2dSprite sprite;
}
