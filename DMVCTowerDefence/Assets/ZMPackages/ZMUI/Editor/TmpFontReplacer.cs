using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
public class TmpFontReplacer : OdinEditorWindow
{
    
    #region 字体样式
    [BoxGroup("字体样式"),LabelText("需要替换的字体"),InlineEditor,AssetList()]
    public TMP_FontAsset m_targetFontAsset;
    [BoxGroup("字体样式"),LabelText("待替换字体"),InlineEditor,AssetList()]
    public TMP_FontAsset m_replaceFontAsset;
    #endregion

    #region 参数选项
    [TitleGroup("字体替换选择")]
    [ShowInInspector,LabelText("选择全部字体")]
    public bool m_isReplaceAllTmpFont = false;
    [ShowInInspector,FolderPath(RequireExistingPath = true),LabelText("文件夹路径(FolderPath)"),HideIf("m_isReplaceAllTmpFont"),OnValueChanged("GetAllTmpTextInFolder")]
    public string m_fontDirectoryPath = string.Empty;
    
    [ShowInInspector,ShowIf("@this.m_fontDirectoryPath !=string.Empty && this.m_fontDirectoryPath.Length > 0 && !this.m_isReplaceAllTmpFont")]
    public List<TextMeshProUGUI> m_allTmpList = new List<TextMeshProUGUI>();
    [ShowInInspector,ShowIf("@this.m_fontDirectoryPath !=string.Empty && this.m_fontDirectoryPath.Length > 0 && !this.m_isReplaceAllTmpFont")]private List<GameObject> m_tmpOwners = new List<GameObject>();
    #endregion
    
    #region 查看并获取包含所有Tmp资源列表
    public void GetAllTmpTextInFolder()
    {
        if(m_fontDirectoryPath.Length <= 0 || m_fontDirectoryPath == null) return;
        m_allTmpList.Clear();
        m_tmpOwners.Clear();
        var prefabGuids = AssetDatabase.FindAssets("t:prefab",new[] { m_fontDirectoryPath});
        if(prefabGuids == null && prefabGuids.Length <=0 ) return;
        foreach (var prefabGUID in prefabGuids)
        { 
            var assetPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab.GetComponentInChildren<TextMeshProUGUI>(true) == null) continue;
            if (assetPath.StartsWith("Packages/"))
                continue;
            var tmps = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmp in tmps)
            {
                m_allTmpList.Add(tmp);
            }
            m_tmpOwners.Add(prefab);
        }
    }
    
    #endregion
    
    [MenuItem("Tools/字体/TMP字体替换工具", false, 1)]
    public static void OpenWindow()
    {
        var window = GetWindow<TmpFontReplacer>();
        window.titleContent = new GUIContent("TMP字体替换工具");
        window.position = new Rect(0, 0, 1080, 540);
        window.Show();
    }
    
    #region 函数
    [Button("替换字体")]
    private void ReplaceTmpFont()
    {
        if (m_targetFontAsset == null || m_replaceFontAsset == null) return;
        if (m_isReplaceAllTmpFont)
        {
            var saveDirPath = m_fontDirectoryPath;
            m_fontDirectoryPath = "Assets";
            GetAllTmpTextInFolder();
            ReplaceFontFromGo();
            SaveReplaceCompleteGoAsset();
            m_fontDirectoryPath = saveDirPath;
        }
        else
        {
            ReplaceFontFromGo();
            SaveReplaceCompleteGoAsset();
        }
    }
    
    private void ReplaceFontFromGo()
    {
        if (m_allTmpList == null || m_allTmpList.Count <= 0 ) return;
        foreach (var tmpText in m_allTmpList)
        {
            if (tmpText.font == m_targetFontAsset)
            {
                tmpText.font = m_replaceFontAsset;
            }
        }
    }
    private void SaveReplaceCompleteGoAsset()
    {
        if(m_tmpOwners == null || m_tmpOwners.Count <= 0) return;
        foreach (var owner in m_tmpOwners)
        {
            Debug.Log(owner.name);
            EditorUtility.SetDirty(owner);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("替换成功");
    }
    #endregion

}
#endif