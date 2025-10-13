using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FoodItem))]
public class FoodItemEditor : Editor
{
    SerializedProperty stateProp;
    SerializedProperty foodTypeProp;
    SerializedProperty rawMatProp;
    SerializedProperty cookedMatProp;
    SerializedProperty burnedMatProp;
    SerializedProperty emptyMatProp;
    SerializedProperty filledMatProp;
    SerializedProperty meshRendererProp;

    private void OnEnable()
    {
        stateProp = serializedObject.FindProperty("state");
        foodTypeProp = serializedObject.FindProperty("foodType");
        rawMatProp = serializedObject.FindProperty("rawMaterial");
        cookedMatProp = serializedObject.FindProperty("cookedMaterial");
        burnedMatProp = serializedObject.FindProperty("burnedMaterial");
        emptyMatProp = serializedObject.FindProperty("emptyMaterial");
        filledMatProp = serializedObject.FindProperty("filledMaterial");
        meshRendererProp = serializedObject.FindProperty("meshRenderer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(foodTypeProp);
        EditorGUILayout.PropertyField(stateProp);
        EditorGUILayout.PropertyField(meshRendererProp);

        if ((CookState)stateProp.enumValueIndex != CookState.Other && (CookState)stateProp.enumValueIndex != CookState.Filled && (CookState)stateProp.enumValueIndex != CookState.Empty)
        {
            EditorGUILayout.PropertyField(rawMatProp);
            EditorGUILayout.PropertyField(cookedMatProp);
            EditorGUILayout.PropertyField(burnedMatProp);
        }

        if ((CookState)stateProp.enumValueIndex == CookState.Filled || (CookState)stateProp.enumValueIndex == CookState.Empty)
        {
            EditorGUILayout.PropertyField(emptyMatProp);
            EditorGUILayout.PropertyField(filledMatProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}