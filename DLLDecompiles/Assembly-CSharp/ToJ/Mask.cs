using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ToJ
{
	// Token: 0x02000AE1 RID: 2785
	[ExecuteInEditMode]
	[AddComponentMenu("Alpha Mask")]
	public class Mask : MonoBehaviour
	{
		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x0600582F RID: 22575 RVA: 0x001BFDF8 File Offset: 0x001BDFF8
		// (set) Token: 0x06005830 RID: 22576 RVA: 0x001BFE00 File Offset: 0x001BE000
		public Mask.MappingAxis maskMappingWorldAxis
		{
			get
			{
				return this._maskMappingWorldAxis;
			}
			set
			{
				this.ChangeMappingAxis(value, this._maskMappingWorldAxis, this._invertAxis);
				this._maskMappingWorldAxis = value;
			}
		}

		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x06005831 RID: 22577 RVA: 0x001BFE1C File Offset: 0x001BE01C
		// (set) Token: 0x06005832 RID: 22578 RVA: 0x001BFE24 File Offset: 0x001BE024
		public bool invertAxis
		{
			get
			{
				return this._invertAxis;
			}
			set
			{
				this.ChangeMappingAxis(this._maskMappingWorldAxis, this._maskMappingWorldAxis, value);
				this._invertAxis = value;
			}
		}

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x06005833 RID: 22579 RVA: 0x001BFE40 File Offset: 0x001BE040
		// (set) Token: 0x06005834 RID: 22580 RVA: 0x001BFE48 File Offset: 0x001BE048
		public bool clampAlphaHorizontally
		{
			get
			{
				return this._clampAlphaHorizontally;
			}
			set
			{
				this.SetMaskBoolValueInMaterials("_ClampHoriz", value);
				this._clampAlphaHorizontally = value;
			}
		}

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x06005835 RID: 22581 RVA: 0x001BFE5D File Offset: 0x001BE05D
		// (set) Token: 0x06005836 RID: 22582 RVA: 0x001BFE65 File Offset: 0x001BE065
		public bool clampAlphaVertically
		{
			get
			{
				return this._clampAlphaVertically;
			}
			set
			{
				this.SetMaskBoolValueInMaterials("_ClampVert", value);
				this._clampAlphaVertically = value;
			}
		}

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x06005837 RID: 22583 RVA: 0x001BFE7A File Offset: 0x001BE07A
		// (set) Token: 0x06005838 RID: 22584 RVA: 0x001BFE82 File Offset: 0x001BE082
		public float clampingBorder
		{
			get
			{
				return this._clampingBorder;
			}
			set
			{
				this.SetMaskFloatValueInMaterials("_ClampBorder", value);
				this._clampingBorder = value;
			}
		}

		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x06005839 RID: 22585 RVA: 0x001BFE97 File Offset: 0x001BE097
		// (set) Token: 0x0600583A RID: 22586 RVA: 0x001BFE9F File Offset: 0x001BE09F
		public bool useMaskAlphaChannel
		{
			get
			{
				return this._useMaskAlphaChannel;
			}
			set
			{
				this.SetMaskBoolValueInMaterials("_UseAlphaChannel", value);
				this._useMaskAlphaChannel = value;
			}
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x001BFEB4 File Offset: 0x001BE0B4
		private void Start()
		{
			this._maskedSpriteWorldCoordsShader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
			this._maskedUnlitWorldCoordsShader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
			MeshRenderer component = base.GetComponent<MeshRenderer>();
			base.GetComponent<MeshFilter>();
			if (Application.isPlaying && component != null)
			{
				component.enabled = false;
			}
		}

		// Token: 0x0600583C RID: 22588 RVA: 0x001BFF08 File Offset: 0x001BE108
		private void Update()
		{
			if (this._maskedSpriteWorldCoordsShader == null)
			{
				this._maskedSpriteWorldCoordsShader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
			}
			if (this._maskedUnlitWorldCoordsShader == null)
			{
				this._maskedUnlitWorldCoordsShader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
			}
			if (this._maskedSpriteWorldCoordsShader == null || this._maskedUnlitWorldCoordsShader == null)
			{
				if (!this.shaderErrorLogged)
				{
					Debug.LogError("Shaders necessary for masking don't seem to be present in the project.");
				}
				return;
			}
			if (base.transform.hasChanged)
			{
				base.transform.hasChanged = false;
				if (this.maskMappingWorldAxis == Mask.MappingAxis.X && (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.x, 0f)) > 0.01f || Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.y, (float)(this.invertAxis ? -90 : 90))) > 0.01f))
				{
					base.transform.eulerAngles = new Vector3(0f, (float)(this.invertAxis ? 270 : 90), base.transform.eulerAngles.z);
				}
				else if (this.maskMappingWorldAxis == Mask.MappingAxis.Y && (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.x, (float)(this.invertAxis ? -90 : 90))) > 0.01f || Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.z, 0f)) > 0.01f))
				{
					base.transform.eulerAngles = new Vector3((float)(this.invertAxis ? -90 : 90), base.transform.eulerAngles.y, 0f);
				}
				else if (this.maskMappingWorldAxis == Mask.MappingAxis.Z && (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.x, 0f)) > 0.01f || Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.y, (float)(this.invertAxis ? -180 : 0))) > 0.01f))
				{
					base.transform.eulerAngles = new Vector3(0f, (float)(this.invertAxis ? -180 : 0), base.transform.eulerAngles.z);
				}
				if (base.transform.parent != null)
				{
					Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
					Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
					List<Material> list = new List<Material>();
					Dictionary<Material, Graphic> dictionary = new Dictionary<Material, Graphic>();
					foreach (Renderer renderer in componentsInChildren)
					{
						if (renderer.gameObject != base.gameObject)
						{
							foreach (Material item in renderer.sharedMaterials)
							{
								if (!list.Contains(item))
								{
									list.Add(item);
								}
							}
						}
					}
					foreach (Graphic graphic in componentsInChildren2)
					{
						if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
						{
							list.Add(graphic.material);
							Canvas canvas = graphic.canvas;
							if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
							{
								dictionary.Add(list[list.Count - 1], graphic);
							}
						}
					}
					foreach (Material material in list)
					{
						if (material.shader.ToString() == this._maskedSpriteWorldCoordsShader.ToString() && material.shader.GetInstanceID() != this._maskedSpriteWorldCoordsShader.GetInstanceID())
						{
							this._maskedSpriteWorldCoordsShader = null;
						}
						if (material.shader.ToString() == this._maskedUnlitWorldCoordsShader.ToString() && material.shader.GetInstanceID() != this._maskedUnlitWorldCoordsShader.GetInstanceID())
						{
							this._maskedUnlitWorldCoordsShader = null;
						}
						if (material.shader == this._maskedSpriteWorldCoordsShader || material.shader == this._maskedUnlitWorldCoordsShader)
						{
							material.DisableKeyword("_SCREEN_SPACE_UI");
							Vector2 vector = new Vector2(1f / base.transform.lossyScale.x, 1f / base.transform.lossyScale.y);
							Vector2 vector2 = Vector2.zero;
							float num = 0f;
							int num2 = 1;
							if (this.maskMappingWorldAxis == Mask.MappingAxis.X)
							{
								num2 = (this.invertAxis ? 1 : -1);
								vector2 = new Vector2(-base.transform.position.z, -base.transform.position.y);
								num = (float)num2 * base.transform.eulerAngles.z;
							}
							else if (this.maskMappingWorldAxis == Mask.MappingAxis.Y)
							{
								vector2 = new Vector2(-base.transform.position.x, -base.transform.position.z);
								num = -base.transform.eulerAngles.y;
							}
							else if (this.maskMappingWorldAxis == Mask.MappingAxis.Z)
							{
								num2 = (this.invertAxis ? -1 : 1);
								vector2 = new Vector2(-base.transform.position.x, -base.transform.position.y);
								num = (float)num2 * base.transform.eulerAngles.z;
							}
							RectTransform component = base.GetComponent<RectTransform>();
							if (component != null)
							{
								Rect rect = component.rect;
								vector2 += base.transform.right * (component.pivot.x - 0.5f) * rect.width * base.transform.lossyScale.x + base.transform.up * (component.pivot.y - 0.5f) * rect.height * base.transform.lossyScale.y;
								vector.x /= rect.width;
								vector.y /= rect.height;
							}
							if (dictionary.ContainsKey(material))
							{
								vector2 = dictionary[material].transform.InverseTransformVector(vector2);
								switch (this.maskMappingWorldAxis)
								{
								case Mask.MappingAxis.X:
									vector2.x *= dictionary[material].transform.lossyScale.z;
									vector2.y *= dictionary[material].transform.lossyScale.y;
									break;
								case Mask.MappingAxis.Y:
									vector2.x *= dictionary[material].transform.lossyScale.x;
									vector2.y *= dictionary[material].transform.lossyScale.z;
									break;
								case Mask.MappingAxis.Z:
									vector2.x *= dictionary[material].transform.lossyScale.x;
									vector2.y *= dictionary[material].transform.lossyScale.y;
									break;
								}
								Canvas canvas2 = dictionary[material].canvas;
								vector2 /= canvas2.scaleFactor;
								vector2 = this.RotateVector(vector2, dictionary[material].transform.eulerAngles);
								vector2 += canvas2.GetComponent<RectTransform>().sizeDelta * 0.5f;
								vector *= canvas2.scaleFactor;
								material.EnableKeyword("_SCREEN_SPACE_UI");
							}
							Vector2 mainTextureScale = base.gameObject.GetComponent<Renderer>().sharedMaterial.mainTextureScale;
							vector.x *= mainTextureScale.x;
							vector.y *= mainTextureScale.y;
							vector.x *= (float)num2;
							Vector2 vector3 = vector2;
							float num3 = Mathf.Sin(-num * 0.017453292f);
							float num4 = Mathf.Cos(-num * 0.017453292f);
							vector2.x = (num4 * vector3.x - num3 * vector3.y) * vector.x + 0.5f * mainTextureScale.x;
							vector2.y = (num3 * vector3.x + num4 * vector3.y) * vector.y + 0.5f * mainTextureScale.y;
							vector2 += base.gameObject.GetComponent<Renderer>().sharedMaterial.mainTextureOffset;
							material.SetTextureOffset("_AlphaTex", vector2);
							material.SetTextureScale("_AlphaTex", vector);
							material.SetFloat("_MaskRotation", num * 0.017453292f);
						}
					}
				}
			}
		}

		// Token: 0x0600583D RID: 22589 RVA: 0x001C0884 File Offset: 0x001BEA84
		private Vector3 RotateVector(Vector3 point, Vector3 angles)
		{
			return Quaternion.Euler(angles) * point;
		}

		// Token: 0x0600583E RID: 22590 RVA: 0x001C0894 File Offset: 0x001BEA94
		private void SetMaskMappingAxisInMaterials(Mask.MappingAxis mappingAxis)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			List<Material> list = new List<Material>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.gameObject != base.gameObject)
				{
					foreach (Material material in renderer.sharedMaterials)
					{
						if (!list.Contains(material))
						{
							list.Add(material);
							this.SetMaskMappingAxisInMaterial(mappingAxis, material);
						}
					}
				}
			}
			foreach (Graphic graphic in componentsInChildren2)
			{
				if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
				{
					list.Add(graphic.material);
					this.SetMaskMappingAxisInMaterial(mappingAxis, graphic.material);
				}
			}
		}

		// Token: 0x0600583F RID: 22591 RVA: 0x001C09A4 File Offset: 0x001BEBA4
		public void SetMaskMappingAxisInMaterial(Mask.MappingAxis mappingAxis, Material material)
		{
			if (material.shader == this._maskedSpriteWorldCoordsShader || material.shader == this._maskedUnlitWorldCoordsShader)
			{
				switch (mappingAxis)
				{
				case Mask.MappingAxis.X:
					material.SetFloat("_Axis", 0f);
					material.EnableKeyword("_AXIS_X");
					material.DisableKeyword("_AXIS_Y");
					material.DisableKeyword("_AXIS_Z");
					return;
				case Mask.MappingAxis.Y:
					material.SetFloat("_Axis", 1f);
					material.DisableKeyword("_AXIS_X");
					material.EnableKeyword("_AXIS_Y");
					material.DisableKeyword("_AXIS_Z");
					return;
				case Mask.MappingAxis.Z:
					material.SetFloat("_Axis", 2f);
					material.DisableKeyword("_AXIS_X");
					material.DisableKeyword("_AXIS_Y");
					material.EnableKeyword("_AXIS_Z");
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005840 RID: 22592 RVA: 0x001C0A84 File Offset: 0x001BEC84
		private void SetMaskFloatValueInMaterials(string variable, float value)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			List<Material> list = new List<Material>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.gameObject != base.gameObject)
				{
					foreach (Material material in renderer.sharedMaterials)
					{
						if (!list.Contains(material))
						{
							list.Add(material);
							material.SetFloat(variable, value);
						}
					}
				}
			}
			foreach (Graphic graphic in componentsInChildren2)
			{
				if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
				{
					list.Add(graphic.material);
					graphic.material.SetFloat(variable, value);
				}
			}
		}

		// Token: 0x06005841 RID: 22593 RVA: 0x001C0B94 File Offset: 0x001BED94
		private void SetMaskBoolValueInMaterials(string variable, bool value)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			List<Material> list = new List<Material>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.gameObject != base.gameObject)
				{
					foreach (Material material in renderer.sharedMaterials)
					{
						if (!list.Contains(material))
						{
							list.Add(material);
							this.SetMaskBoolValueInMaterial(variable, value, material);
						}
					}
				}
			}
			foreach (Graphic graphic in componentsInChildren2)
			{
				if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
				{
					list.Add(graphic.material);
					this.SetMaskBoolValueInMaterial(variable, value, graphic.material);
				}
			}
		}

		// Token: 0x06005842 RID: 22594 RVA: 0x001C0CA4 File Offset: 0x001BEEA4
		public void SetMaskBoolValueInMaterial(string variable, bool value, Material material)
		{
			if (material.shader == this._maskedSpriteWorldCoordsShader || material.shader == this._maskedUnlitWorldCoordsShader)
			{
				material.SetFloat(variable, (float)(value ? 1 : 0));
				if (value)
				{
					material.EnableKeyword(variable.ToUpper() + "_ON");
					return;
				}
				material.DisableKeyword(variable.ToUpper() + "_ON");
			}
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x001C0D18 File Offset: 0x001BEF18
		private void CreateAndAssignQuad(Mesh mesh, float minX = -0.5f, float maxX = 0.5f, float minY = -0.5f, float maxY = 0.5f)
		{
			mesh.vertices = new Vector3[]
			{
				new Vector3(minX, minY, 0f),
				new Vector3(maxX, minY, 0f),
				new Vector3(minX, maxY, 0f),
				new Vector3(maxX, maxY, 0f)
			};
			mesh.triangles = new int[]
			{
				0,
				2,
				1,
				2,
				3,
				1
			};
			mesh.normals = new Vector3[]
			{
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward
			};
			mesh.uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x001C0E61 File Offset: 0x001BF061
		public void SetMaskRendererActive(bool value)
		{
			if (base.GetComponent<Renderer>() != null)
			{
				if (value)
				{
					base.GetComponent<Renderer>().enabled = true;
					return;
				}
				base.GetComponent<Renderer>().enabled = false;
			}
		}

		// Token: 0x06005845 RID: 22597 RVA: 0x001C0E90 File Offset: 0x001BF090
		private void ChangeMappingAxis(Mask.MappingAxis currMaskMappingWorldAxis, Mask.MappingAxis prevMaskMappingWorldAxis, bool currInvertAxis)
		{
			if (currMaskMappingWorldAxis == Mask.MappingAxis.X)
			{
				if (prevMaskMappingWorldAxis == Mask.MappingAxis.Y)
				{
					base.transform.eulerAngles = new Vector3(0f, (float)(currInvertAxis ? -90 : 90), base.transform.eulerAngles.y);
				}
				else
				{
					base.transform.eulerAngles = new Vector3(0f, (float)(currInvertAxis ? -90 : 90), base.transform.eulerAngles.z);
				}
			}
			else if (currMaskMappingWorldAxis == Mask.MappingAxis.Y)
			{
				if (prevMaskMappingWorldAxis == Mask.MappingAxis.Y)
				{
					base.transform.eulerAngles = new Vector3((float)(currInvertAxis ? -90 : 90), base.transform.eulerAngles.y, 0f);
				}
				else
				{
					base.transform.eulerAngles = new Vector3((float)(currInvertAxis ? -90 : 90), base.transform.eulerAngles.z, 0f);
				}
			}
			else if (currMaskMappingWorldAxis == Mask.MappingAxis.Z)
			{
				if (prevMaskMappingWorldAxis == Mask.MappingAxis.Y)
				{
					base.transform.eulerAngles = new Vector3(0f, (float)(currInvertAxis ? -180 : 0), base.transform.eulerAngles.y);
				}
				else
				{
					base.transform.eulerAngles = new Vector3(0f, (float)(currInvertAxis ? -180 : 0), base.transform.eulerAngles.z);
				}
			}
			this.SetMaskMappingAxisInMaterials(currMaskMappingWorldAxis);
		}

		// Token: 0x040053F4 RID: 21492
		private bool shaderErrorLogged;

		// Token: 0x040053F5 RID: 21493
		[SerializeField]
		private Mask.MappingAxis _maskMappingWorldAxis = Mask.MappingAxis.Z;

		// Token: 0x040053F6 RID: 21494
		[SerializeField]
		private bool _invertAxis;

		// Token: 0x040053F7 RID: 21495
		[SerializeField]
		private bool _clampAlphaHorizontally;

		// Token: 0x040053F8 RID: 21496
		[SerializeField]
		private bool _clampAlphaVertically;

		// Token: 0x040053F9 RID: 21497
		[SerializeField]
		private float _clampingBorder = 0.01f;

		// Token: 0x040053FA RID: 21498
		[SerializeField]
		private bool _useMaskAlphaChannel;

		// Token: 0x040053FB RID: 21499
		private Shader _maskedSpriteWorldCoordsShader;

		// Token: 0x040053FC RID: 21500
		private Shader _maskedUnlitWorldCoordsShader;

		// Token: 0x02001B71 RID: 7025
		public enum MappingAxis
		{
			// Token: 0x04009CFF RID: 40191
			X,
			// Token: 0x04009D00 RID: 40192
			Y,
			// Token: 0x04009D01 RID: 40193
			Z
		}
	}
}
