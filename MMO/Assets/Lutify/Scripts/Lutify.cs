// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera)), ExecuteInEditMode]
[AddComponentMenu("Image Effects/Lutify")]
public class Lutify : MonoBehaviour
{
	public enum SplitMode
	{
		None,
		Horizontal,
		Vertical
	}

	[Tooltip("The texture to use as a lookup table. Size should be: height = sqrt(width)")]
	public Texture2D LookupTexture;

	[Tooltip("Shows a before/after comparison by splitting the screen in half.")]
	public SplitMode Split = SplitMode.None;

	[Tooltip("Lutify will automatically detect the correct shader to use for the device but you can force it to only use the compatibility shader.")]
	public bool ForceCompatibility = false;

	[Tooltip("Sets the filter mode for the LUT texture. You'll want to set this to Point when using palette reduction LUTs.")]
	public FilterMode LutFiltering = FilterMode.Bilinear;

	[Range(0f, 1f), Tooltip("Blending factor.")]
	public float Blend = 1f;

	#region Editor-only stuff
	public int _LastSelectedCategory = 0;
	public int _ThumbWidth = 110;
	public int _ThumbHeight;
	int cache_ThumbWidth;
	int cache_ThumbHeight;
	bool cache_IsLinear;
	public RenderTexture _PreviewRT;
    #endregion

    public Texture3D m_Lut3D;
	protected int m_BaseTextureIntanceID;
	protected bool m_Use2DLut = false;

	public bool IsLinear { get { return QualitySettings.activeColorSpace == ColorSpace.Linear; } }

	public Shader Shader2D;
	public Shader Shader3D;
    public Material m_Material2D;
    public Material m_Material3D;
	public Material Material
	{
		get
		{
			if (m_Use2DLut || ForceCompatibility)
			{
				if (m_Material2D == null)
				{
					m_Material2D = new Material(Shader2D);
					m_Material2D.hideFlags = HideFlags.HideAndDontSave;
				}

				return m_Material2D;
			}
			else
			{
				if (m_Material3D == null)
				{
					m_Material3D = new Material(Shader3D);
					m_Material3D.hideFlags = HideFlags.HideAndDontSave;
				}

				return m_Material3D;
			}
		}
	}

	protected virtual void Start()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("Image effects aren't supported on this device");
			enabled = false;
			return;
		}

		// Switch to the 2D lut if the platform doesn't support 3D textures
		if (!SystemInfo.supports3DTextures)
			m_Use2DLut = true;

		// Disable the image effect if the shader can't run on the users graphics card
		if ((!m_Use2DLut && (!Shader3D || !Shader3D.isSupported)) ||
			( m_Use2DLut && (!Shader2D || !Shader2D.isSupported)))
		{
			Debug.LogWarning("The shader is null or unsupported on this device");
			enabled = false;
		}
	}

	protected virtual void OnEnable()
	{
		if (LookupTexture != null && LookupTexture.GetInstanceID() != m_BaseTextureIntanceID)
			ConvertBaseTexture3D();
	}

	protected virtual void OnDisable()
	{
		if (m_Material2D)
			DestroyImmediate(m_Material2D);

		if (m_Material3D)
			DestroyImmediate(m_Material3D);

		if (m_Lut3D)
			DestroyImmediate(m_Lut3D);
		
#if UNITY_EDITOR
		if (_PreviewRT)
			DestroyImmediate(_PreviewRT);
#endif

		m_BaseTextureIntanceID = -1;
	}

	protected void SetIdentityLut3D()
	{
		int dim = 16;
		Color[] newC = new Color[dim * dim * dim];
		float oneOverDim = 1f / (1f * dim - 1f);

		for (int i = 0; i < dim; i++)
		{
			for (int j = 0; j < dim; j++)
			{
				for (int k = 0; k < dim; k++)
				{
					newC[i + (j * dim) + (k * dim * dim)] = new Color((float)i * oneOverDim, (float)j * oneOverDim, (float)k * oneOverDim, 1f);
				}
			}
		}

		if (m_Lut3D)
			DestroyImmediate(m_Lut3D);

		m_Lut3D = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
		m_Lut3D.hideFlags = HideFlags.HideAndDontSave;
		m_Lut3D.SetPixels(newC);
		m_Lut3D.Apply();
		m_BaseTextureIntanceID = -1;
	}

	public bool ValidDimensions(Texture2D tex2D)
	{
		if (tex2D == null || tex2D.height != Mathf.FloorToInt(Mathf.Sqrt(tex2D.width)))
			return false;

		return true;
	}

    protected void ConvertBaseTexture3D()
	{
		if (!ValidDimensions(LookupTexture))
		{
			Debug.LogWarning("The given 2D texture " + LookupTexture.name + " cannot be used as a LUT. Pick another texture or adjust dimension to e.g. 256x16.");
			return;
		}

		m_BaseTextureIntanceID = LookupTexture.GetInstanceID();

		int dim = LookupTexture.height;

		Color[] c = LookupTexture.GetPixels();//x=256*y=16, first x then y
		Color[] newC = new Color[c.Length];//16*16*16 所以对应起来 3Dpixel first xy then z
        Debug.Log(c.Length);
		for (int i = 0; i < dim; i++)//x
		{
			for (int j = 0; j < dim; j++)//y
			{
				for (int k = 0; k < dim; k++)//z
				{
					int j_ = dim - j - 1;
					newC[i + (j * dim) + (k * dim * dim)] = c[k * dim + i + j_ * dim * dim];
                   //对应关系//for3D先读z 再读yx
                   //for2D先读了最后一整行每隔16x像素的第一个点，也就是塞进3D的深度值。
                }//i+
			}
		}

		if (m_Lut3D)
			DestroyImmediate(m_Lut3D);
		m_Lut3D = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
		m_Lut3D.hideFlags = HideFlags.HideAndDontSave;
		m_Lut3D.wrapMode = TextureWrapMode.Clamp;
		m_Lut3D.SetPixels(newC);
		m_Lut3D.Apply();
        Debug.Log(m_Lut3D.GetPixels().Length);
    }
	
#if UNITY_EDITOR
	bool RebuildPreview(int w, int h)
	{
		_ThumbHeight = Mathf.FloorToInt((float)_ThumbWidth / ((float)w / (float)h));

		if (_ThumbWidth != cache_ThumbWidth || cache_ThumbHeight != _ThumbHeight || cache_IsLinear != IsLinear)
		{
			cache_ThumbWidth = _ThumbWidth;
			cache_ThumbHeight = _ThumbHeight;
			cache_IsLinear = IsLinear;
			return true;
		}

		if (_PreviewRT == null)
			return true;

		return false;
	}
#endif

	protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
#if UNITY_EDITOR
		if (RebuildPreview(source.width, source.height))
		{
			if (_PreviewRT != null)
			{
				DestroyImmediate(_PreviewRT);
				_PreviewRT = null;
			}

			_PreviewRT = new RenderTexture(_ThumbWidth, _ThumbHeight, 0, RenderTextureFormat.ARGB32, IsLinear ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
			_PreviewRT.hideFlags = HideFlags.HideAndDontSave;
			_PreviewRT.Create();
		}

		Graphics.Blit(source, _PreviewRT);
#endif

		if (LookupTexture == null || Blend <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}

		int pass = 0;
		if (Split == SplitMode.Horizontal) pass = 2;
		else if (Split == SplitMode.Vertical) pass = 4;
		if (IsLinear) pass++;

		if (m_Use2DLut || ForceCompatibility)
			RenderLut2D(source, destination, pass);
		else
			RenderLut3D(source, destination, pass);
	}

	void RenderLut3D(RenderTexture source, RenderTexture destination, int pass)
	{
		if (LookupTexture.GetInstanceID() != m_BaseTextureIntanceID)
			ConvertBaseTexture3D();

		if (m_Lut3D == null)
			SetIdentityLut3D();

		m_Lut3D.filterMode = LutFiltering;

		// Uniforms
		float lutSize = (float)m_Lut3D.width;
		Material.SetTexture("_LookupTex3D", m_Lut3D);
		Material.SetVector("_Params", new Vector3((lutSize - 1f) / lutSize, 1f / (2f * lutSize), Blend));

		Graphics.Blit(source, destination, Material, pass);
	}

	void RenderLut2D(RenderTexture source, RenderTexture destination, int pass)
	{
		LookupTexture.filterMode = LutFiltering;

		// Uniforms
		float tileSize = Mathf.Sqrt((float)LookupTexture.width);
		Material.SetTexture("_LookupTex2D", LookupTexture);
		Material.SetVector("_Params", new Vector4(1f / (float)LookupTexture.width, 1f / (float)LookupTexture.height, tileSize - 1f, Blend));

		Graphics.Blit(source, destination, Material, pass + (LutFiltering == FilterMode.Point ? 6 : 0));
	}
}
