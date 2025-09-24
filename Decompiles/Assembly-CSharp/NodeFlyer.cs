using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class NodeFlyer : MonoBehaviour
{
	// Token: 0x06001BA7 RID: 7079 RVA: 0x00080F5C File Offset: 0x0007F15C
	private void Start()
	{
		this.nodeParent.SetParent(null, true);
		int childCount = this.nodeParent.childCount;
		this.nodes = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			this.nodes[i] = this.nodeParent.GetChild(i);
		}
		this.currentNodeIndex = Mathf.Max(0, this.nodes.Length - 2);
		if (this.activeOnStart)
		{
			this.StartFlying();
		}
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x00080FD2 File Offset: 0x0007F1D2
	public void StartFlying()
	{
		if (this.flyRoutine != null)
		{
			return;
		}
		this.flyRoutine = base.StartCoroutine(this.Fly());
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x00080FEF File Offset: 0x0007F1EF
	public void StopFlying()
	{
		if (this.flyRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.flyRoutine);
		this.flyRoutine = null;
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x0008100D File Offset: 0x0007F20D
	private IEnumerator Fly()
	{
		if (this.animator)
		{
			this.animator.Play("Fly");
		}
		for (;;)
		{
			Vector2 startPos = base.transform.position;
			Vector2 nodePos = this.nodes[this.currentNodeIndex].position;
			float num = Vector2.Distance(startPos, nodePos);
			if (num > 0.1f)
			{
				bool flag = nodePos.x < startPos.x;
				bool flag2 = base.transform.localScale.x > 0f;
				if (!this.spriteFacesLeft)
				{
					flag2 = !flag2;
				}
				if (flag != flag2)
				{
					base.transform.FlipLocalScale(true, false, false);
					if (this.animator)
					{
						this.animator.PlayFromFrame("TurnToFly", 0);
					}
				}
				float duration = num / this.speed;
				for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
				{
					float t = this.curve.Evaluate(elapsed / duration);
					Vector2 v = Vector2.Lerp(startPos, nodePos, t);
					base.transform.position = v;
					yield return null;
				}
			}
			this.currentNodeIndex++;
			if (this.currentNodeIndex >= this.nodes.Length)
			{
				this.currentNodeIndex = 0;
			}
			yield return null;
			startPos = default(Vector2);
			nodePos = default(Vector2);
		}
		yield break;
	}

	// Token: 0x04001AAF RID: 6831
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform nodeParent;

	// Token: 0x04001AB0 RID: 6832
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04001AB1 RID: 6833
	[SerializeField]
	private bool spriteFacesLeft = true;

	// Token: 0x04001AB2 RID: 6834
	[Space]
	[SerializeField]
	private float speed;

	// Token: 0x04001AB3 RID: 6835
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001AB4 RID: 6836
	[SerializeField]
	private bool activeOnStart;

	// Token: 0x04001AB5 RID: 6837
	private Transform[] nodes;

	// Token: 0x04001AB6 RID: 6838
	private int currentNodeIndex;

	// Token: 0x04001AB7 RID: 6839
	private Coroutine flyRoutine;
}
