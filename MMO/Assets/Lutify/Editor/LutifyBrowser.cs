// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
#define UNITY_5_1_PLUS
#endif

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
public class LutifyBrowser : EditorWindow
{
	public static LutifyBrowser inst;

	protected Lutify m_Lutify;
	protected Vector2 m_ScrollView = Vector2.zero;

	protected string m_BasePath = "";

	protected Dictionary<string, List<Texture2D>> m_Collections;

	protected string[] m_CollectionMenu;
	protected string[] m_CollectionNames;
	protected FilterMode[] m_CollectionFilters;
	protected int m_SelectedCollection = 0;

	protected Material m_MaterialGamma;
	protected Material m_MaterialLinear;
	protected Material PreviewMaterial
	{
		get
		{
			if (m_MaterialGamma == null)
			{
				m_MaterialGamma = new Material(Shader.Find("Hidden/Lutify-Preview-Gamma"));
				m_MaterialGamma.hideFlags = HideFlags.HideAndDontSave;
			}

			if (m_MaterialLinear == null)
			{
				m_MaterialLinear = new Material(Shader.Find("Hidden/Lutify-Preview-Linear"));
				m_MaterialLinear.hideFlags = HideFlags.HideAndDontSave;
			}

			if (m_Lutify != null)
				return m_Lutify.IsLinear ? m_MaterialLinear : m_MaterialGamma;

			return m_MaterialGamma;
		}
	}

	public static void Init(Lutify inst)
	{
		LutifyBrowser window = EditorWindow.GetWindow<LutifyBrowser>();
		window.Prepare(inst);
		window.autoRepaintOnSceneChange = true;
		window.minSize = new Vector2(370f, 200f);
		window.Show();
	}

	public static Texture2D LoadIcon()
	{
		string[] results = AssetDatabase.FindAssets("LutifyBrowser t:Script", null);

		if (results.Length > 0)
		{
			string p = AssetDatabase.GUIDToAssetPath(results[0]);
			p = Path.GetDirectoryName(p);
			p = p.Substring(0, p.LastIndexOf('/'));
			string path = Path.Combine(p, Path.Combine("Editor", "browser-icon.png"));
			return LoadAssetAt<Texture2D>(path);
		}
         
		return null;
	}

	void Prepare(Lutify inst)
	{
		m_Lutify = inst;
		m_SelectedCollection = m_Lutify._LastSelectedCategory;
		if (m_CollectionMenu == null || m_SelectedCollection >= m_CollectionMenu.Length)
			m_SelectedCollection = 0;
	}

	void OnEnable()
	{
		// Title icon
#if UNITY_5_1_PLUS
		GUIContent windowTitle = new GUIContent(" Lutify", LoadIcon());
		titleContent = windowTitle;
#else
		title = "Lutify";
		GUIContent windowTitle = InternalGUIUtility.GetTitleContent(this);

		if (windowTitle != null)
		{
			windowTitle.text = " Lutify";
			windowTitle.image = LoadIcon();
		}
#endif

		inst = this;
		FetchLuts();
	}

	void OnDestroy()
	{
		inst = null;

		if (m_MaterialGamma)
			DestroyImmediate(m_MaterialGamma);

		if (m_MaterialLinear)
			DestroyImmediate(m_MaterialLinear);

		m_Collections.Clear();
		m_Collections = null;
	}

	public static T LoadAssetAt<T>(string path) where T : UnityEngine.Object
	{
#if UNITY_5_1_PLUS
		return AssetDatabase.LoadAssetAtPath<T>(path);
#else
		return Resources.LoadAssetAtPath<T>(path);
#endif
	}

	void FetchLuts()
	{
		// Find the lut location
		string baseRelativePath = null;
		string baseAbsolutePath = null;
		string[] results = AssetDatabase.FindAssets("LutifyBrowser t:Script", null);

		if (results.Length > 0)
		{
			string p = AssetDatabase.GUIDToAssetPath(results[0]);
			p = Path.GetDirectoryName(p);
			p = p.Substring(0, p.LastIndexOf('/'));
			m_BasePath = Path.Combine(p, "Luts");
			baseRelativePath = Path.Combine(m_BasePath, "Standard");
			baseAbsolutePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, baseRelativePath);
		}
		else
		{
			// Should never happen but just in case
			Debug.LogError("Couldn't find the LutifyBrowser script");
			Close();
		}

		// Collection listing
		if (m_Collections != null)
			m_Collections.Clear();

		m_Collections = new Dictionary<string, List<Texture2D>>();
		string[] dirs = Directory.GetDirectories(baseAbsolutePath);
        StringBuilder sb = new StringBuilder();
		foreach (string dir in dirs)
		{
			string[] files = Directory.GetFiles(dir, "*.png");
			Array.Sort(files, (x, y) => String.Compare(x, y, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols));

			// Skip empty directories
			if (files.Length == 0)
				continue;

			string collection = Path.GetFileName(dir);
			List<Texture2D> content = null;

			foreach (string file in files)
			{
				string relativePath = Path.Combine(baseRelativePath, Path.Combine(collection, Path.GetFileName(file)));
                sb.Append(relativePath);
                sb.Append("\n");
                Texture2D lut = LoadAssetAt<Texture2D>(relativePath);

				if (lut == null)
					continue;

				if (content == null)
					content = new List<Texture2D>();

				content.Add(lut);
			}

			if (content.Count == 0)
				continue;

			m_Collections.Add(collection, content);
		}
        //File.WriteAllText(Application.dataPath+"/lutify.txt",sb.ToString());
		List<string> menu = new List<string>();
		List<string> names = new List<string>();
		List<FilterMode> filters = new List<FilterMode>();

		foreach (KeyValuePair<string, List<Texture2D>> entry in m_Collections)
		{
			string name = entry.Key;
			string displayName = name + " (" + entry.Value.Count + ")";
			menu.Add(displayName);
			names.Add(name);

			// Filter
			string filename = Path.Combine(baseAbsolutePath, Path.Combine(name, "Filtering.txt"));

			if (!File.Exists(filename))
			{
				filters.Add(FilterMode.Bilinear);
			}
			else
			{
				string txt = File.ReadAllText(filename);

				try
				{
					FilterMode mode = (FilterMode)Enum.Parse(typeof(FilterMode), txt, true);
					filters.Add(mode);
				}
				catch
				{
					filters.Add(FilterMode.Bilinear);
				}
			}
		}

		m_CollectionMenu = menu.ToArray();
		m_CollectionNames = names.ToArray();
		m_CollectionFilters = filters.ToArray();
		menu.Clear();
		names.Clear();
	}

	void Update()
	{
		if (m_Lutify == null)
			Close();
	}

	void OnGUI()
	{
		if (m_Lutify == null)
			return; // Can happen when Unity is restarted and the LUT browser is still opened

		// Header
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		{
			EditorGUI.BeginChangeCheck();

			m_SelectedCollection = EditorGUILayout.Popup(m_SelectedCollection, m_CollectionMenu, EditorStyles.toolbarPopup, GUILayout.MaxWidth(170f));

			if (EditorGUI.EndChangeCheck())
			{
				m_ScrollView = Vector2.zero;
				m_Lutify._LastSelectedCategory = m_SelectedCollection;
			}

			GUILayout.FlexibleSpace();

			EditorGUI.BeginChangeCheck();

			m_Lutify._ThumbWidth = Mathf.RoundToInt(GUILayout.HorizontalSlider(m_Lutify._ThumbWidth, 100f, 300f, new GUIStyle("preSlider"), new GUIStyle("preSliderThumb"), GUILayout.Width(64f)));

			if (EditorGUI.EndChangeCheck())
				InternalGUIUtility.RepaintGameView();

			if (GUILayout.Button("?", EditorStyles.toolbarButton))
				Application.OpenURL("http://www.thomashourdel.com/lutify/");
		}
		GUILayout.EndHorizontal();

		// Component check
		if (!m_Lutify.enabled)
		{
			EditorGUILayout.HelpBox("Lutify is disabled ! Please enable the component get the LUT browser to work.", MessageType.Error);

			if (GUILayout.Button("Enable"))
				m_Lutify.enabled = true;

			return;
		}

		// Gallery
		List<Texture2D> luts = null;

		if (!m_Collections.TryGetValue(m_CollectionNames[m_SelectedCollection], out luts) || luts.Count == 0)
			return;

		m_ScrollView = GUILayout.BeginScrollView(m_ScrollView, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		{
			int w = Mathf.FloorToInt(position.width);
			int tw = m_Lutify._ThumbWidth;
			int th = m_Lutify._ThumbHeight;
			int spacing = 4;
			int margin = 16;
			int c = (w - margin) / (tw + spacing);

			GUILayout.BeginVertical();
			{
				GUILayout.Space(spacing);

				FilterMode filterMode = m_CollectionFilters[m_SelectedCollection];
				bool layoutState = false;
				int j = 0;

				for (int i = 0; i < luts.Count; i++)
				{
					j++;

					if (!layoutState)
					{
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						layoutState = true;
					}

					Rect r = GUILayoutUtility.GetRect(tw, th);

					if (m_Lutify._PreviewRT != null)
						DrawPreview(r, luts[i], filterMode);

					if (j < c && c != 1 && i != luts.Count - 1)
						GUILayout.Space(spacing);

					if (layoutState && j == c)
					{
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						GUILayout.Space(spacing);
						layoutState = false;
						j = 0;
					}
				}

				if (layoutState && j < c)
				{
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}

				GUILayout.Space(spacing);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndScrollView();
	}

	void DrawPreview(Rect r, Texture2D lut, FilterMode filterMode)
	{
		Event e = Event.current;

		if (e.type == EventType.Repaint)
		{
			bool selected = lut == m_Lutify.LookupTexture;

			// Preview texture
			lut.filterMode = filterMode;
			float tileSize = Mathf.Sqrt((float)lut.width);
			PreviewMaterial.SetTexture("_LookupTex2D", lut);
			PreviewMaterial.SetVector("_Params", new Vector4(1f / (float)lut.width, 1f / (float)lut.height, tileSize - 1f, filterMode == FilterMode.Point ? 1f : 0f));
			PreviewMaterial.SetFloat("_BottomFrame", 17f * (1f / (float)m_Lutify._ThumbHeight));
			PreviewMaterial.SetColor("_FrameColor", selected ? Color.white : Color.black);
			EditorGUI.DrawPreviewTexture(r, m_Lutify._PreviewRT, PreviewMaterial);

			// Borders
			Handles.color = selected ? new Color(1f, 1f, 1f, 2f) : new Color(0f, 0f, 0f, 2f); // Removes default alpha on handles
			Handles.DrawLine(new Vector3(r.x, r.y + 1, 0f), new Vector3(r.xMax, r.y + 1, 0f));
			Handles.DrawLine(new Vector3(r.x + 1, r.y + 1, 0f), new Vector3(r.x + 1, r.yMax, 0f));
			Handles.DrawLine(new Vector3(r.xMax, r.y + 1, 0f), new Vector3(r.xMax, r.yMax, 0f));

			// Name overlay (text)
			Color oldColor = GUI.color;
			GUI.color = selected ? new Color32(20, 20, 20, 255) : new Color32(156, 216, 246, 255);
			GUI.Label(new Rect(r.x + 4, r.yMax - 17, r.width - 8, 16), lut.name, EditorStyles.miniLabel);
			GUI.color = oldColor;

			// Selection shape
			if (selected)
			{
				Vector3[] verts = new Vector3[] {
					new Vector3(r.xMax, r.y + 1, 0),
					new Vector3(r.xMax, r.y + 18, 0),
					new Vector3(r.xMax - 17, r.y + 1, 0),
					new Vector3(r.xMax - 17, r.y + 1, 0)
				};

				Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 2f), new Color(1f, 1f, 1f, 2f));
			}
		}
		else if (e.type == EventType.MouseDown)
		{
			if (r.Contains(e.mousePosition))
			{
				if (e.button == 0)
				{
					Undo.RecordObject(m_Lutify, "Set Lutify LUT");
					m_Lutify.LutFiltering = m_CollectionFilters[m_SelectedCollection];
					m_Lutify.LookupTexture = lut;
					InternalGUIUtility.RepaintGameView();
				}
				else
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Show in project (Standard)"), false, () => EditorGUIUtility.PingObject(lut));
					menu.AddItem(new GUIContent("Show in project (Scion)"), false, () => PingExtTexture(lut, "Scion"));
					menu.AddItem(new GUIContent("Show in project (Amplify)"), false, () => PingExtTexture(lut, "AmplifyColor"));
					menu.ShowAsContext();
				}

				e.Use();
			}
		}
	}

	void PingExtTexture(Texture2D lut, string basePath)
	{
		string loc = Path.Combine(m_BasePath, Path.Combine(basePath, m_CollectionNames[m_SelectedCollection]));
		string path = Path.Combine(loc, lut.name + ".png");
		Texture2D tex = LoadAssetAt<Texture2D>(path);

		if (tex == null)
			return;

		EditorGUIUtility.PingObject(tex);
	}

	static class InternalGUIUtility
	{
		static Type m_GameViewType;

		static InternalGUIUtility()
		{
			// GameView
			Assembly assembly = typeof(EditorWindow).Assembly;
			m_GameViewType = assembly.GetType("UnityEditor.GameView");
		}

		public static GUIContent GetTitleContent(EditorWindow editor)
		{
			const BindingFlags bFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			PropertyInfo p = typeof(EditorWindow).GetProperty("cachedTitleContent", bFlags);
			if (p == null) return null;
			return p.GetValue(editor, null) as GUIContent;
		}

		public static void RepaintGameView()
		{
			EditorWindow gameview = EditorWindow.GetWindow(m_GameViewType);
			gameview.Repaint();
		}
	}
}
