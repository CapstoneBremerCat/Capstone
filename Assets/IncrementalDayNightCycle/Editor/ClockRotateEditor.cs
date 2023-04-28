using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ClockRotate))]

public class ClockRotateEditor : Editor
{
    SerializedProperty isDigital;
    SerializedProperty txtTime;
    SerializedProperty DNC;
    SerializedProperty HourHand;
    SerializedProperty RotateHourX;
    SerializedProperty RotateHourY;
    SerializedProperty RotateHourZ;
    SerializedProperty HourOffset;
    SerializedProperty MinuteHand;
    SerializedProperty RotateMinuteX;
    SerializedProperty RotateMinuteY;
    SerializedProperty RotateMinuteZ;
    SerializedProperty MinuteOffset;

    
    bool Digital;

    void OnEnable()
    {

        isDigital = serializedObject.FindProperty("isDigital");
        txtTime = serializedObject.FindProperty("txtTime");
        DNC = serializedObject.FindProperty("DNC");
        HourHand = serializedObject.FindProperty("HourHand");
        RotateHourX = serializedObject.FindProperty("RotateHourX");
        RotateHourY = serializedObject.FindProperty("RotateHourY");
        RotateHourZ = serializedObject.FindProperty("RotateHourZ");
        HourOffset = serializedObject.FindProperty("HourOffset");
        MinuteHand = serializedObject.FindProperty("MinuteHand");
        RotateMinuteX = serializedObject.FindProperty("RotateMinuteX");
        RotateMinuteY = serializedObject.FindProperty("RotateMinuteY");
        RotateMinuteZ = serializedObject.FindProperty("RotateMinuteZ");
        MinuteOffset = serializedObject.FindProperty("MinuteOffset");
        
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        



        EditorGUILayout.PropertyField(DNC);




        EditorGUILayout.PropertyField(isDigital);
        Digital = isDigital.boolValue;
            if (!Digital)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(HourHand);
                    EditorGUILayout.PropertyField(RotateHourX);
                    EditorGUILayout.PropertyField(RotateHourY);
                    EditorGUILayout.PropertyField(RotateHourZ);
                    EditorGUILayout.PropertyField(HourOffset);
                    EditorGUILayout.PropertyField(MinuteHand);
                    EditorGUILayout.PropertyField(RotateMinuteX);
                    EditorGUILayout.PropertyField(RotateMinuteY);
                    EditorGUILayout.PropertyField(RotateMinuteZ);
                    EditorGUILayout.PropertyField(MinuteOffset);
                    EditorGUILayout.Space();
                }
            EditorGUI.indentLevel = 0;
        if (Digital)
            if (Selection.activeTransform)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(txtTime);
                EditorGUILayout.Space();
            }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        




    


        serializedObject.ApplyModifiedProperties();
    }

}
