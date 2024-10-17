/***
 * Author RadBear - Nguyen Ba Hung - nbhung71711@gmail.com 
 **/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RCore.Common;
using RCore.Common.Editor;

public class ProfilesCollection : ScriptableObject
{
	private const string FILE_PATH = "Assets/Editor/ProfilesCollection.asset";

	public List<EnvSetting.Profile> profiles = new List<EnvSetting.Profile>();

    public static ProfilesCollection LoadOrCreateCollection()
    {
        var collection = AssetDatabase.LoadAssetAtPath(FILE_PATH, typeof(ProfilesCollection)) as ProfilesCollection;
        if (collection == null)
            collection = EditorHelper.CreateScriptableAsset<ProfilesCollection>(FILE_PATH);
        return collection;
    }
}