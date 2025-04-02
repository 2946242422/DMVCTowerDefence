using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;

public class SceneQuickSwitch : EditorWindow
{
    [MenuItem("Tools/Scene Quick Switch")]
    public static void ShowWindow()
    {
        GetWindow<SceneQuickSwitch>("场景快速切换");
    }

    private Vector2 scrollPosition;
    
    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("快速场景切换", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 获取Build Settings中的场景列表
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .ToArray();

        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("没有找到可用场景，请先在Build Settings中添加场景。", MessageType.Warning);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // 创建按钮网格布局
        int columns = 3; // 每行显示3个按钮
        int buttonIndex = 0;
        
        while (buttonIndex < scenes.Length)
        {
            EditorGUILayout.BeginHorizontal();
            
            for (int i = 0; i < columns && buttonIndex < scenes.Length; i++)
            {
                var scene = scenes[buttonIndex];
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                
                // 添加场景按钮
                if (GUILayout.Button(sceneName, GUILayout.Height(30), GUILayout.Width(150)))
                {
                    TrySwitchScene(scene.path);
                }
                
                buttonIndex++;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void TrySwitchScene(string scenePath)
    {
        // 检查是否需要保存当前场景
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // 加载新场景
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}