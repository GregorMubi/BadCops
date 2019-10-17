using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        /*WeaponData weaponData = (WeaponData)target;

        GUIStyle style = new GUIStyle(GUI.skin.textField);
        //style.fontSize = 30;

        weaponData.weaponName = EditorGUILayout.TextField(weaponData.weaponName, style, GUILayout.MinHeight(150));

        weaponData.rateOfFire = EditorGUILayout.IntField("Rate Of Fire", weaponData.rateOfFire);
        weaponData.damage = EditorGUILayout.FloatField("Damage", weaponData.damage);
        weaponData.range = EditorGUILayout.FloatField("Range", weaponData.range);

        weaponData.projectile = (ProjectileContoller)EditorGUILayout.ObjectField(weaponData.projectile, typeof(ProjectileContoller), false);
        weaponData.explosion = (ExplosionController)EditorGUILayout.ObjectField(weaponData.explosion, typeof(ExplosionController), false);

        if (GUILayout.Button("Random values"))
        {
            weaponData.RandomValues();
        }
        */

    }
}