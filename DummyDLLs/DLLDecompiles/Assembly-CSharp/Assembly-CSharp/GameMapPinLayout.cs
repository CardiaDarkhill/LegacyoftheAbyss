using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000666 RID: 1638
public class GameMapPinLayout : MonoBehaviour
{
	// Token: 0x06003AA9 RID: 15017 RVA: 0x001024C4 File Offset: 0x001006C4
	private void OnValidate()
	{
		this.DoLayout();
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x001024CC File Offset: 0x001006CC
	private void OnEnable()
	{
		if (this.isDirty)
		{
			this.DoLayout();
		}
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x001024DC File Offset: 0x001006DC
	public void DoLayout()
	{
		this.isDirty = false;
		int num = 0;
		using (IEnumerator enumerator = base.transform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (((Transform)enumerator.Current).gameObject.activeSelf)
				{
					num++;
				}
			}
		}
		Vector2 b = this.itemOffset * (float)(num - 1) / 2f;
		int num2 = 0;
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.activeSelf)
			{
				Vector2 position = this.itemOffset * (float)num2 - b;
				transform.SetLocalPosition2D(position);
				GameMapPinLayout.ILayoutHook component = transform.GetComponent<GameMapPinLayout.ILayoutHook>();
				if (component != null)
				{
					component.LayoutFinished();
				}
				num2++;
			}
		}
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x001025EC File Offset: 0x001007EC
	public void Evaluate()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(true);
			GameMapPinLayout.IEvaluateHook component = transform.GetComponent<GameMapPinLayout.IEvaluateHook>();
			if (component != null)
			{
				component.ForceEvaluate();
			}
		}
		this.DoLayout();
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x00102660 File Offset: 0x00100860
	public void SetLayoutDirty()
	{
		this.isDirty = true;
		if (base.gameObject.activeInHierarchy)
		{
			this.DoLayout();
		}
	}

	// Token: 0x04003D10 RID: 15632
	[SerializeField]
	private Vector2 itemOffset;

	// Token: 0x04003D11 RID: 15633
	private bool isDirty;

	// Token: 0x02001976 RID: 6518
	public interface IEvaluateHook
	{
		// Token: 0x0600945B RID: 37979
		void ForceEvaluate();
	}

	// Token: 0x02001977 RID: 6519
	public interface ILayoutHook
	{
		// Token: 0x0600945C RID: 37980
		void LayoutFinished();
	}
}
