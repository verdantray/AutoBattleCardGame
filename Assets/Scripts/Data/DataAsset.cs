using UnityEngine;

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using GoogleSheetsToUnity;
using GoogleSheetsToUnity.ThirdPary;
using UnityEditor;

#endif

namespace AutoBattleCardGame.Data.Editor
{
    public interface IFieldUpdatable
    {
        public void UpdateFields(List<GSTU_Cell> cells);
    }
    
    public abstract class DataAsset : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private string sheetAddress;
        [SerializeField] private DataUpdater[] dataUpdaters;

        public abstract void UpdateDataFromSheet();

        protected void UpdateData<T>(string fieldName, ICollection<T> collection) where T : IFieldUpdatable, new()
        {
            foreach (var updater in dataUpdaters)
            {
                if (updater.TryUpdateData(sheetAddress, fieldName, collection))
                {
                    OnDataUpdated(fieldName);
                    break;
                }
            }
        }

        private void OnDataUpdated(string fieldName)
        {
            Undo.RecordObject(this, $"Update {name}.{fieldName}");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DataAsset), editorForChildClasses: true)]
    public class DataAssetEditor : UnityEditor.Editor
    {
        private readonly string[] _propertyNames = { "sheetAddress", "dataUpdaters" };
        private static bool _foldOut = true;
        
        private SerializedProperty[] _serializedProperties = null;
        private EditorCoroutine _updateCoroutine = null;
        
        private void OnEnable()
        {
            _serializedProperties = new SerializedProperty[_propertyNames.Length];

            for (int i = 0; i < _propertyNames.Length; i++)
            {
                _serializedProperties[i] = serializedObject.FindProperty(_propertyNames[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, _propertyNames);
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2.0f);
            DrawRemains();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRemains()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            _foldOut = EditorGUILayout.Foldout(_foldOut, "Update Data from sheets");
            if (_foldOut)
            {
                foreach (SerializedProperty property in _serializedProperties)
                {
                    EditorGUILayout.PropertyField(property);
                }
            
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
            
                if (GUILayout.Button("Update data from spread sheets"))
                {
                    if (_updateCoroutine != null)
                    {
                        EditorCoroutineRunner.KillCoroutine(ref _updateCoroutine);
                        _updateCoroutine = null;
                    }

                    _updateCoroutine = EditorCoroutineRunner.StartCoroutine(UpdateDataRoutine());
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private IEnumerator UpdateDataRoutine()
        {
            yield return GoogleAuthrisationHelper.CheckForRefreshOfToken();
            
            ((DataAsset)target).UpdateDataFromSheet();
        }
    }
    
#endif
}
