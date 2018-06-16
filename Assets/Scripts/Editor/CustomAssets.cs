using UnityEngine;
using UnityEditor;

public class CustomAssets
{
	[MenuItem("Assets/Create/Resource/Character Control")]
	public static void CreateCharacterControlAsset()
	{
		MSL.CustomAssetUtility.CreateAsset<CharacterControlAsset>();
	}
}