using RCore.Common;
using RCore.Common.Editor;
using UnityEditor;
using UnityEngine;

namespace RCore.Editor
{
	public class SearchAndReplaceAssetWindow : EditorWindow
	{
		private Vector2 m_scrollPosition;
		private SearchAndReplaceAssetToolkit m_searchAndReplaceAssetToolkit;
		private ReplaceSpriteTool m_replaceSpriteTool;
		private CutSpriteSheetTool m_cutSpriteSheetTool;
		private UpdateImagePropertyTool m_updateImagePropertyTool;
		private ReplaceObjectTool m_replaceObjectTool;
		private string m_tab;
		private ReplaceSpriteTool.Tps m_tps;
		private bool m_displayNullR;

		private void OnGUI()
		{
			m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);
			m_searchAndReplaceAssetToolkit ??= SearchAndReplaceAssetToolkit.Load();
			m_replaceSpriteTool = m_searchAndReplaceAssetToolkit.replaceSpriteTool;
			m_updateImagePropertyTool = m_searchAndReplaceAssetToolkit.updateImagePropertyTool;
			m_cutSpriteSheetTool = m_searchAndReplaceAssetToolkit.cutSpriteSheetTool;
			m_replaceObjectTool = m_searchAndReplaceAssetToolkit.replaceObjectTool;

			m_tab = EditorHelper.Tabs("m_assetsReplacer.spriteReplace", "Sprites Replacer", "Export sprites from sheet", "Sprite Utilities", "Objects Replacer");
			GUILayout.BeginVertical("box");
			switch (m_tab)
			{
				case "Sprites Replacer":
					m_replaceSpriteTool.Draw();
					break;
				case "Export sprites from sheet":
					m_cutSpriteSheetTool.Draw();
					break;
				case "Sprite Utilities":
					m_updateImagePropertyTool.Draw();
					break;
				case "Objects Replacer":
					m_replaceObjectTool.Draw();
					break;
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		[MenuItem("RCore/Tools/Assets Replacer")]
		private static void OpenEditorWindow()
		{
			var window = GetWindow<SearchAndReplaceAssetWindow>("Assets Replacer", true);
			window.Show();
		}
	}
}