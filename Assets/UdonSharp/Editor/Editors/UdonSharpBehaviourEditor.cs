﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Editor;

namespace UdonSharpEditor
{
    [CustomEditor(typeof(UdonSharpBehaviour), true)]
    [CanEditMultipleObjects]
    internal class UdonSharpBehaviourEditor : Editor
    {
        [MenuItem("Assets/Create/U# Script", false, 5)]
        private static void CreateUSharpScript()
        {
            string folderPath = "Assets/";
            if (Selection.activeObject != null)
            {
                folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Selection.activeObject.GetType() != typeof(UnityEditor.DefaultAsset))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }
            }
            else if (Selection.assetGUIDs.Length > 0)
            {
                folderPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            }

            folderPath = folderPath.Replace('\\', '/');
            
            string chosenFilePath = EditorUtility.SaveFilePanelInProject("Save UdonSharp File", "", "cs", "Save UdonSharp file", folderPath);

            if (chosenFilePath.Length > 0)
            {
                string chosenFileName = Path.GetFileNameWithoutExtension(chosenFilePath).Replace(" ", "").Replace("#", "Sharp");
                string assetFilePath = Path.Combine(Path.GetDirectoryName(chosenFilePath), $"{chosenFileName}.asset");

                if (AssetDatabase.LoadAssetAtPath<UdonSharpProgramAsset>(assetFilePath) != null)
                {
                    if (!EditorUtility.DisplayDialog("File already exists", $"Corresponding asset file '{assetFilePath}' already found for new UdonSharp script. Overwrite?", "Ok", "Cancel"))
                        return;
                }

                string fileContents = UdonSharpSettings.GetProgramTemplateString(chosenFileName);

                File.WriteAllText(chosenFilePath, fileContents);

                AssetDatabase.ImportAsset(chosenFilePath, ImportAssetOptions.ForceSynchronousImport);
                MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(chosenFilePath);

                UdonSharpProgramAsset newProgramAsset = ScriptableObject.CreateInstance<UdonSharpProgramAsset>();
                newProgramAsset.sourceCsScript = newScript;

                AssetDatabase.CreateAsset(newProgramAsset, assetFilePath);

                AssetDatabase.Refresh();
            }
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawConvertToUdonBehaviourButton(targets))
                return;

            base.OnInspectorGUI();
        }
    }

    #region Drawer override boilerplate
    [InitializeOnLoad]
    internal class UdonBehaviourDrawerOverride
    {
        static UdonBehaviourDrawerOverride()
        {
            OverrideUdonBehaviourDrawer();
        }

        static FieldInfo customEditorField;
        static MethodInfo removeTypeMethod;
        static MethodInfo addTypeMethod;

        static System.Type monoEditorTypeType;
        static System.Type monoEditorTypeListType;
        static MethodInfo listAddTypeMethod;
        static MethodInfo listClearMethod;
        static FieldInfo monoEditorTypeInspectedTypeField;
        static FieldInfo monoEditorTypeInspectorTypeField;

        static readonly object[] udonBehaviourTypeArr = new object[] { typeof(UdonBehaviour) };
        static readonly object[] addTypeInvokeParams = new object[] { typeof(UdonBehaviour), null };
        static readonly object[] listCreateParams = new object[] { 1 };

        static object customEditorDictionary;
        static object editorTypeList;
        static object editorTypeObject;

        /// <summary>
        /// Handles removing the reference to the default UdonBehaviourEditor and injecting our own custom editor UdonBehaviourOverrideEditor
        /// </summary>
        static void OverrideUdonBehaviourDrawer() 
        {
            if (customEditorField == null)
            {
                Assembly editorAssembly = AppDomain.CurrentDomain.GetAssemblies().First(e => e.GetName().Name == "UnityEditor");

                System.Type editorAttributesClass = editorAssembly.GetType("UnityEditor.CustomEditorAttributes");
                customEditorField = editorAttributesClass.GetField("kSCustomEditors", BindingFlags.NonPublic | BindingFlags.Static);

                System.Type fieldType = customEditorField.FieldType;

                removeTypeMethod = fieldType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                            .FirstOrDefault(e => e.Name == "Remove" &&
                                                                 e.GetParameters().Length == 1 &&
                                                                 e.GetParameters()[0].ParameterType == typeof(System.Type));

                monoEditorTypeType = editorAttributesClass.GetNestedType("MonoEditorType", BindingFlags.NonPublic);
                monoEditorTypeInspectedTypeField = monoEditorTypeType.GetField("m_InspectedType", BindingFlags.Public | BindingFlags.Instance);
                monoEditorTypeInspectorTypeField = monoEditorTypeType.GetField("m_InspectorType", BindingFlags.Public | BindingFlags.Instance);

                monoEditorTypeListType = typeof(List<>).MakeGenericType(monoEditorTypeType);


                addTypeMethod = fieldType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                         .FirstOrDefault(e => e.Name == "Add" &&
                                                              e.GetParameters().Length == 2 &&
                                                              e.GetParameters()[0].ParameterType == typeof(System.Type) &&
                                                              e.GetParameters()[1].ParameterType == monoEditorTypeListType);

                listAddTypeMethod = monoEditorTypeListType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                          .FirstOrDefault(e => e.Name == "Add" &&
                                                                               e.GetParameters().Length == 1 &&
                                                                               e.GetParameters()[0].ParameterType == monoEditorTypeType);

                listClearMethod = monoEditorTypeListType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                        .FirstOrDefault(e => e.Name == "Clear" &&
                                                                             e.GetParameters().Length == 0);

                customEditorDictionary = customEditorField.GetValue(null);

                editorTypeObject = Activator.CreateInstance(monoEditorTypeType);
                monoEditorTypeInspectedTypeField.SetValue(editorTypeObject, typeof(UdonBehaviour));
                monoEditorTypeInspectorTypeField.SetValue(editorTypeObject, typeof(UdonBehaviourOverrideEditor));

                editorTypeList = Activator.CreateInstance(monoEditorTypeListType);

                listCreateParams[0] = editorTypeObject;
            }

            listClearMethod.Invoke(editorTypeList, null);
            listAddTypeMethod.Invoke(editorTypeList, listCreateParams);

            removeTypeMethod.Invoke(customEditorDictionary, udonBehaviourTypeArr);

            addTypeInvokeParams[1] = editorTypeList;
            addTypeMethod.Invoke(customEditorDictionary, addTypeInvokeParams);
        }
    }
    #endregion

    #region Editor Manager
    [InitializeOnLoad]
    static class UdonSharpCustomEditorManager
    {
        static Dictionary<System.Type, System.Type> _typeInspectorMap;

        static UdonSharpCustomEditorManager()
        {
            InitInspectorMap();
            Undo.postprocessModifications += OnPostprocessUndoModifications;
        }

        static void InitInspectorMap()
        {
            _typeInspectorMap = new Dictionary<Type, Type>();
            FieldInfo inspectedTypeField = typeof(CustomEditor).GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (Assembly asm in UdonSharpUtils.GetLoadedEditorAssemblies())
            {
                foreach (Type editorType in asm.GetTypes())
                {
                    CustomEditor editorAttribute = editorType.GetCustomAttribute<CustomEditor>();

                    if (editorAttribute != null)
                    {
                        Type inspectedType = (Type)inspectedTypeField.GetValue(editorAttribute);

                        if (inspectedType.IsSubclassOf(typeof(UdonSharpBehaviour)))
                        {
                            if (_typeInspectorMap.ContainsKey(inspectedType))
                            {
                                Debug.LogError($"Cannot register inspector '{editorType.Name}' for type '{inspectedType.Name}' since inspector '{_typeInspectorMap[inspectedType].Name}' is already registered");
                                continue;
                            }

                            _typeInspectorMap.Add(inspectedType, editorType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dirties the underlying UdonBehaviour when a proxy UdonSharpBehaviour is modified since Unity does not mark the scene dirty when a modified object is marked 'HideFlags.DontSaveInEditor'
        /// Has the downside that the scene will not be "undirtied" when a change that dirtied it is undone.
        /// </summary>
        /// <param name="propertyModifications"></param>
        /// <returns></returns>
        static UndoPropertyModification[] OnPostprocessUndoModifications(UndoPropertyModification[] propertyModifications)
        {
            if (!EditorApplication.isPlaying)
            {
                foreach (UndoPropertyModification propertyModification in propertyModifications)
                {
                    UnityEngine.Object target = propertyModification.currentValue.target;

                    if (target is UdonSharpBehaviour udonSharpBehaviour)
                    {
                        UdonBehaviour backingBehaviour = UdonSharpEditorUtility.GetBackingUdonBehaviour(udonSharpBehaviour);

                        if (backingBehaviour)
                        {
                            EditorSceneManager.MarkSceneDirty(backingBehaviour.gameObject.scene);
                        }
                    }
                }
            }

            return propertyModifications;
        }

        public static System.Type GetInspectorEditorType(System.Type udonSharpBehaviourType)
        {
            System.Type editorType;
            _typeInspectorMap.TryGetValue(udonSharpBehaviourType, out editorType);

            return editorType;
        }
    }
    #endregion

    /// <summary>
    /// Custom U# editor for UdonBehaviours that can have custom behavior for drawing stuff like sync position and the program asset info
    /// Will also allow people to override the inspector for their own custom inspectors
    /// </summary>
    internal class UdonBehaviourOverrideEditor : Editor
    {
        Editor baseEditor;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
            UdonEditorManager.Instance.WantRepaint += Repaint;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            UdonEditorManager.Instance.WantRepaint -= Repaint;
        }

        void OnUndoRedo()
        {
            UdonSharpBehaviour inspectorTarget = UdonSharpEditorUtility.FindProxyBehaviour(target as UdonBehaviour, false);

            if (inspectorTarget)
            {
                UdonSharpEditorUtility.CopyProxyToBacker(inspectorTarget);
            }
        }

        private void OnDestroy()
        {
            if (baseEditor)
                DestroyImmediate(baseEditor);
        }

        public override void OnInspectorGUI()
        {
            UdonBehaviour behaviour = target as UdonBehaviour;

            // Fall back to the default Udon inspector if not a U# behaviour
            if (behaviour.programSource == null || !(behaviour.programSource is UdonSharpProgramAsset udonSharpProgram))
            {
                if (!baseEditor)
                    Editor.CreateCachedEditorWithContext(targets, this, typeof(UdonBehaviourEditor), ref baseEditor);
            
                baseEditor.OnInspectorGUI();
                return;
            }

            MonoScript sourceScript = ((UdonSharpProgramAsset)behaviour.programSource).sourceCsScript;

            System.Type customEditorType;
            if (sourceScript)
                customEditorType = UdonSharpCustomEditorManager.GetInspectorEditorType(sourceScript.GetClass());
            else
                customEditorType = null;

            if (customEditorType != null)
            {
                if (baseEditor != null && baseEditor.GetType() != customEditorType)
                {
                    DestroyImmediate(baseEditor);
                    baseEditor = null;
                }

                UdonSharpBehaviour inspectorTarget = UdonSharpEditorUtility.GetProxyBehaviour(behaviour);
                inspectorTarget.enabled = false;

                Editor.CreateCachedEditorWithContext(inspectorTarget, this, customEditorType, ref baseEditor);

                baseEditor.serializedObject.Update();

                baseEditor.OnInspectorGUI();

                UdonSharpEditorUtility.CopyProxyToBacker(inspectorTarget);
            }
            else
            {
                DrawDefaultUdonSharpInspector();
            }
        }

        void DrawDefaultUdonSharpInspector()
        {
            UdonBehaviour behaviour = target as UdonBehaviour;

            if (UdonSharpGUI.DrawProgramSource(behaviour))
                return;

            UdonSharpGUI.DrawSyncSettings(behaviour);
            UdonSharpGUI.DrawInteractSettings(behaviour);

            UdonSharpProgramAsset udonSharpProgramAsset = (UdonSharpProgramAsset)behaviour.programSource;

            UdonSharpGUI.DrawUtilities(behaviour, udonSharpProgramAsset);

            UdonSharpGUI.DrawUILine();

            udonSharpProgramAsset.DrawErrorTextAreas();

            bool dirty = false;
            UdonSharpGUI.DrawPublicVariables(behaviour, udonSharpProgramAsset, ref dirty);
        }

        // Force repaint for variable update in play mode
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }
    }
}
