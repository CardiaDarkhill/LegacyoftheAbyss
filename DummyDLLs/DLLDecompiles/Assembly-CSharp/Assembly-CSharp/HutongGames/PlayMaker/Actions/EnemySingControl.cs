using System;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C28 RID: 3112
	[ActionCategory(ActionCategory.Audio)]
	public class EnemySingControl : ComponentAction<AudioSource>
	{
		// Token: 0x06005EBA RID: 24250 RVA: 0x001DF7C8 File Offset: 0x001DD9C8
		public override void Reset()
		{
			this.enemyGameObject = null;
			this.audioPlayer = null;
			this.singAudioTable = null;
			this.noThreadEffects = null;
			this.noPuppetString = null;
			this.randomSingStartTime = null;
			this.dontStopAudioOnExit = null;
			this.startedJitter = false;
			this.altThreadSpawnPoint = null;
		}

		// Token: 0x06005EBB RID: 24251 RVA: 0x001DF814 File Offset: 0x001DDA14
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.enemyGameObject);
			this.needolinTextOwner = null;
			this.inForcedSing = false;
			bool flag = true;
			if (ownerDefaultTarget != null)
			{
				BlackThreadState component = ownerDefaultTarget.GetComponent<BlackThreadState>();
				if (component)
				{
					flag = false;
					if (!component.IsVisiblyThreaded && !component.IsBlackThreaded)
					{
						this.PlaySing();
					}
					if (component.IsInForcedSing)
					{
						this.inForcedSing = true;
						return;
					}
				}
				this.needolinTextOwner = ownerDefaultTarget.GetComponent<NeedolinTextOwner>();
				if (this.needolinTextOwner)
				{
					this.needolinTextOwner.AddNeedolinText();
				}
				if (!this.noThreadEffects.Value)
				{
					EnemyHitEffectsRegular component2 = ownerDefaultTarget.GetComponent<EnemyHitEffectsRegular>();
					this.followOffset = (component2 ? component2.EffectOrigin : Vector3.zero);
					Vector3 position = ownerDefaultTarget.transform.TransformPoint(this.followOffset);
					if (this.altThreadSpawnPoint.Value != null)
					{
						position = this.altThreadSpawnPoint.Value.transform.position;
					}
					bool blackThreadWorld = GameManager.instance.playerData.blackThreadWorld;
					if (!this.noPuppetString.Value && !blackThreadWorld)
					{
						this.possessionObj = Effects.SilkPossesionObjSing.Spawn(position);
					}
					else
					{
						this.possessionObj = Effects.SilkPossesionObjSingNoPuppet.Spawn(position);
					}
					EnemySingControl.MatchEffectsToObject(ownerDefaultTarget, this.possessionObj, out this.followOffset);
				}
				this.durationController = ownerDefaultTarget.GetComponent<EnemySingDuration>();
			}
			else
			{
				Debug.LogError(string.Format("{0} : {1}{2} : {3} - Missing Game Object", new object[]
				{
					base.Owner,
					base.Fsm.Name,
					(base.Fsm.Template != null) ? (" : Template : " + base.Fsm.Template.name) : string.Empty,
					base.Fsm.ActiveStateName
				}), base.Owner);
			}
			if (flag)
			{
				this.PlaySing();
			}
			if (Gameplay.MusicianCharmTool.IsEquipped)
			{
				this.singDuration = Random.Range(6.5f, 8f);
			}
			else
			{
				this.singDuration = Random.Range(4f, 6.75f);
			}
			this.sentEndEvent = false;
		}

		// Token: 0x06005EBC RID: 24252 RVA: 0x001DFA4C File Offset: 0x001DDC4C
		private void PlaySing()
		{
			this.audioSource = this.audioPlayer.GetSafe<AudioSource>();
			if (this.audioSource != null)
			{
				RandomAudioClipTable randomAudioClipTable = this.singAudioTable.Value as RandomAudioClipTable;
				if (randomAudioClipTable)
				{
					this.audioSource.clip = randomAudioClipTable.SelectClip(true);
					this.audioSource.pitch = randomAudioClipTable.SelectPitch();
					this.audioSource.volume = randomAudioClipTable.SelectVolume();
				}
				this.audioSource.Stop();
				if (this.randomSingStartTime.Value && this.audioSource.clip != null)
				{
					this.audioSource.time = Random.Range(0f, this.audioSource.clip.length);
				}
				this.audioSource.Play();
			}
		}

		// Token: 0x06005EBD RID: 24253 RVA: 0x001DFB24 File Offset: 0x001DDD24
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.enemyGameObject);
			if (!this.inForcedSing && !this.sentEndEvent)
			{
				if (this.singDuration > 0f)
				{
					this.singDuration -= Time.deltaTime;
				}
				else if (ownerDefaultTarget != null)
				{
					FSMUtility.SendEventToGameObject(ownerDefaultTarget, "SING DURATION END", false);
					this.sentEndEvent = true;
				}
			}
			if (!this.noThreadEffects.Value && ownerDefaultTarget != null && this.possessionObj != null)
			{
				if (this.altThreadSpawnPoint.Value != null)
				{
					this.possessionObj.transform.position = this.altThreadSpawnPoint.Value.transform.position;
					return;
				}
				this.possessionObj.transform.position = ownerDefaultTarget.transform.TransformPoint(this.followOffset);
			}
		}

		// Token: 0x06005EBE RID: 24254 RVA: 0x001DFC14 File Offset: 0x001DDE14
		public override void OnExit()
		{
			if (this.audioSource != null && !this.dontStopAudioOnExit.Value)
			{
				this.audioSource.Stop();
			}
			if (!this.noThreadEffects.Value && this.possessionObj)
			{
				this.possessionObj.Recycle();
				this.possessionObj = null;
			}
			if (this.needolinTextOwner)
			{
				this.needolinTextOwner.RemoveNeedolinText();
				this.needolinTextOwner = null;
			}
			if (this.durationController)
			{
				this.durationController.StartSingCooldown();
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.enemyGameObject);
			if (ownerDefaultTarget != null && !this.noThreadEffects.Value && !this.inForcedSing)
			{
				this.possessionObjEnd = Effects.SilkPossesionObjSingEnd.Spawn(ownerDefaultTarget.transform.position);
				this.possessionObjEnd.transform.parent = ownerDefaultTarget.transform;
				this.possessionObjEnd = null;
			}
		}

		// Token: 0x06005EBF RID: 24255 RVA: 0x001DFD14 File Offset: 0x001DDF14
		private static void MatchEffectsToObject(GameObject gameObject, GameObject effectObj, out Vector2 centreOffset)
		{
			centreOffset = Vector2.zero;
			Transform transform = gameObject.transform;
			Transform transform2 = effectObj.transform;
			transform2.localPosition = Vector3.zero;
			Collider2D component = gameObject.GetComponent<Collider2D>();
			if (!component)
			{
				return;
			}
			Vector2 vector;
			Vector3 vector2;
			if (component.enabled)
			{
				Bounds bounds = component.bounds;
				vector = bounds.size;
				vector2 = bounds.center;
			}
			else
			{
				BoxCollider2D boxCollider2D = component as BoxCollider2D;
				if (boxCollider2D == null)
				{
					CircleCollider2D circleCollider2D = component as CircleCollider2D;
					if (circleCollider2D == null)
					{
						Debug.LogError("EnemySingControl \"" + gameObject.name + "\" has inactive collider that can't be manually handled!", gameObject);
						return;
					}
					float num = circleCollider2D.radius * 2f;
					vector = new Vector2(num, num);
					vector2 = transform.TransformPoint(circleCollider2D.offset);
				}
				else
				{
					vector = boxCollider2D.size;
					vector2 = transform.TransformPoint(boxCollider2D.offset);
				}
			}
			transform2.SetPosition2D(vector2);
			centreOffset = transform.InverseTransformPoint(vector2);
			float value = Mathf.Max(vector.x, vector.y);
			MinMaxFloat minMaxFloat = new MinMaxFloat(3f, 6f);
			MinMaxFloat minMaxFloat2 = new MinMaxFloat(1f, 1.6f);
			float lerpedValue = minMaxFloat2.GetLerpedValue(minMaxFloat.GetTBetween(value));
			transform2.SetScale2D(new Vector2(lerpedValue, lerpedValue));
		}

		// Token: 0x04005B4C RID: 23372
		private const float SING_DURATION_MIN = 4f;

		// Token: 0x04005B4D RID: 23373
		private const float SING_DURATION_MAX = 6.75f;

		// Token: 0x04005B4E RID: 23374
		private const float SING_DURATION_STRONG_MIN = 6.5f;

		// Token: 0x04005B4F RID: 23375
		private const float SING_DURATION_STRONG_MAX = 8f;

		// Token: 0x04005B50 RID: 23376
		private const float COLLIDER_REF_SCALE_MIN = 3f;

		// Token: 0x04005B51 RID: 23377
		private const float COLLIDER_REF_SCALE_MAX = 6f;

		// Token: 0x04005B52 RID: 23378
		private const float EFFECT_SCALE_MIN = 1f;

		// Token: 0x04005B53 RID: 23379
		private const float EFFECT_SCALE_MAX = 1.6f;

		// Token: 0x04005B54 RID: 23380
		public FsmOwnerDefault enemyGameObject;

		// Token: 0x04005B55 RID: 23381
		public FsmGameObject audioPlayer;

		// Token: 0x04005B56 RID: 23382
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject singAudioTable;

		// Token: 0x04005B57 RID: 23383
		public FsmBool noThreadEffects;

		// Token: 0x04005B58 RID: 23384
		public FsmBool noPuppetString;

		// Token: 0x04005B59 RID: 23385
		public FsmBool randomSingStartTime;

		// Token: 0x04005B5A RID: 23386
		public FsmBool dontStopAudioOnExit;

		// Token: 0x04005B5B RID: 23387
		public FsmGameObject altThreadSpawnPoint;

		// Token: 0x04005B5C RID: 23388
		private Vector2 followOffset;

		// Token: 0x04005B5D RID: 23389
		private AudioSource audioSource;

		// Token: 0x04005B5E RID: 23390
		private GameObject possessionObj;

		// Token: 0x04005B5F RID: 23391
		private GameObject possessionObjEnd;

		// Token: 0x04005B60 RID: 23392
		private NeedolinTextOwner needolinTextOwner;

		// Token: 0x04005B61 RID: 23393
		private EnemySingDuration durationController;

		// Token: 0x04005B62 RID: 23394
		private float singDuration;

		// Token: 0x04005B63 RID: 23395
		private bool inForcedSing;

		// Token: 0x04005B64 RID: 23396
		private bool sentEndEvent;

		// Token: 0x04005B65 RID: 23397
		private bool startedJitter;
	}
}
