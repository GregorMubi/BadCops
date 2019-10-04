using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WeaponData weaponData = (WeaponData)target;

        EditorGUILayout.IntField("Rate Of Fire", weaponData.rateOfFire);
        EditorGUILayout.FloatField("Damage", weaponData.damage);
        EditorGUILayout.FloatField("Range", weaponData.range);

        if (GUILayout.Button("Random values"))
        {
            weaponData.RandomValues();
        }

    }
}