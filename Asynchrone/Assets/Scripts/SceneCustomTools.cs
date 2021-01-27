using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneCustomTools : Editor
{
    static string path = "Assets/Scenes";

    [MenuItem("Scenes/Open Scene Menu")]
    static void LoadSceneMenu()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Open Scene 3C")]
    static void LoadScene3C()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/3C.unity", OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Open Scene IA")]
    static void LoadSceneIA()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/IA.unity", OpenSceneMode.Single);
    }

    [MenuItem("Scenes/Open Scene Level1")]
    static void LoadSceneLevel1()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Levels/Level_1.unity", OpenSceneMode.Single);
    }
}
