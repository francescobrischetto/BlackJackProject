#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
public class RunGame : Editor
{

    [MenuItem("Game/Run Game")]
    static void StartGame()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MenuScene.unity");
            EditorApplication.isPlaying = true;
        }
    }

}
#endif