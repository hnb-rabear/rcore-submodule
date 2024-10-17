using RCore.Common;
using RCore.Common.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace RCore.Editor
{
	public class SearchAndReplaceAssetToolkit : ScriptableObject
	{
		private static readonly string m_FilePath = $"Assets/Editor/{nameof(SearchAndReplaceAssetToolkit)}Cache.asset";
		[FormerlySerializedAs("spritesReplacer")]
        public ReplaceSpriteTool replaceSpriteTool;
		[FormerlySerializedAs("spritesCutter")]
        public CutSpriteSheetTool cutSpriteSheetTool;
		[FormerlySerializedAs("imageComponentPropertiesFixer")]
        public UpdateImagePropertyTool updateImagePropertyTool;
		[FormerlySerializedAs("objectsReplacer")]
        public ReplaceObjectTool replaceObjectTool;

		public static SearchAndReplaceAssetToolkit Load()
		{
			var collection = AssetDatabase.LoadAssetAtPath(m_FilePath, typeof(SearchAndReplaceAssetToolkit)) as SearchAndReplaceAssetToolkit;
			if (collection == null)
				collection = EditorHelper.CreateScriptableAsset<SearchAndReplaceAssetToolkit>(m_FilePath);
			return collection;
		}
	}
}