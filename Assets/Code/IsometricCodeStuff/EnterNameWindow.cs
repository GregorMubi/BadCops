using System;
using UnityEditor;
using UnityEngine;

public class EnterNameWindow : EditorWindow {

    private static String Name = "";
    private static String Title = "";
    private static Action<String> OnNamneEntered;
    private static EnterNameWindow Window;

    public static void Open(String title, Action<String> onNamneEntered) {
        Name = "";
        Title = title;
        OnNamneEntered = onNamneEntered;
        Window = (EnterNameWindow)EditorWindow.GetWindow(typeof(EnterNameWindow));
        Window.Show();
    }

    void OnGUI() {
        GUILayout.Label(Title, EditorStyles.boldLabel);
        Name = EditorGUILayout.TextField(Name);
        if (GUILayout.Button("OK")) {
            OnNamneEntered(Name);
            Window.Close();
        }
    }
}
