
using UnityEditor;

public static class Menu
{
    [MenuItem("Assets/Save")]
    public static void SaveAssets()
    {
        AssetDatabase.SaveAssets();
    }
}
