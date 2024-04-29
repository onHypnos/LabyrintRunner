using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.Extensions
{
    public static class FirstSceneLoaderUtil
    {      
        private const string PlayFromFirstMenuStr = "Edit/Always Start From Scene 0 &p";

        static bool PlayFromFirstScene
        {
            get{return EditorPrefs.HasKey(PlayFromFirstMenuStr) && EditorPrefs.GetBool(PlayFromFirstMenuStr);}
            set{EditorPrefs.SetBool(PlayFromFirstMenuStr, value);}
        }

        [MenuItem(PlayFromFirstMenuStr, false, 150)]
        static void PlayFromFirstSceneCheckMenu() 
        {
            PlayFromFirstScene = !PlayFromFirstScene;
            Menu.SetChecked(PlayFromFirstMenuStr, PlayFromFirstScene);

            ShowNotifyOrLog(PlayFromFirstScene ? "Play from scene 0" : "Play from current scene");
        }

        // The menu won't be gray out, we use this validate method for update check state
        [MenuItem(PlayFromFirstMenuStr, true)]
        static bool PlayFromFirstSceneCheckMenuValidate()
        {
            Menu.SetChecked(PlayFromFirstMenuStr, PlayFromFirstScene);
            return true;
        }

#if UNITY_EDITOR
        // This method is called before any Awake. It's the perfect callback for this feature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
        static void LoadFirstSceneAtGameBegins()
        {
            if(!PlayFromFirstScene)
                return;

            if(EditorBuildSettings.scenes.Length  == 0)
            {
                Debug.LogWarning("The scene build list is empty. Can't play from first scene.");
                return;
            }

            foreach(GameObject go in Object.FindObjectsOfType<GameObject>())
                go.SetActive(false);
        
            SceneManager.LoadScene(0);
        }
#endif
    
        static void ShowNotifyOrLog(string msg)
        {
            if(Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
                EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
            else
                Debug.Log(msg); // When there's no scene view opened, we just print a log
        }
    }
}
