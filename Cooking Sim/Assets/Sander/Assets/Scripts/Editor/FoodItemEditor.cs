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
    SerializedProperty meshRendererProp;

    private void OnEnable()
    {
        stateProp = serializedObject.FindProperty("state");
        foodTypeProp = serializedObject.FindProperty("foodType");
        rawMatProp = serializedObject.FindProperty("rawMaterial");
        cookedMatProp = serializedObject.FindProperty("cookedMaterial");
        burnedMatProp = serializedObject.FindProperty("burnedMaterial");
        meshRendererProp = serializedObject.FindProperty("meshRenderer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(foodTypeProp);
        EditorGUILayout.PropertyField(stateProp);
        EditorGUILayout.PropertyField(meshRendererProp);

        if ((CookState)stateProp.enumValueIndex != CookState.Other)
        {
            EditorGUILayout.PropertyField(rawMatProp);
            EditorGUILayout.PropertyField(cookedMatProp);
            EditorGUILayout.PropertyField(burnedMatProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}