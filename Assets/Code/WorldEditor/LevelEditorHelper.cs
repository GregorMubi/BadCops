using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditorHelper : MonoBehaviour {

    [SerializeField] private WorldEditor WorldEditor = null;
    private Object PrevSelection = null;

    void Update() {
        Object selected = Selection.activeObject;
        //Debug.Log(selected == null ? "null" : selected.name);
        if (selected != PrevSelection) {
            PrevSelection = selected;
            if (selected != null) {
                GameObject selection = null;
                try {
                    selection = (GameObject)selected;
                } catch {
                }
                if (selection != null) {
                    WorldEditor.LastSelectedPosition = selection.transform.position;
                    TileController selectedTile = selection.GetComponent<TileController>();
                    if (selectedTile == null && selection.transform.parent != null) {
                        selectedTile = selection.transform.parent.GetComponent<TileController>();
                        if (selectedTile != null) {
                            Selection.activeObject = selectedTile.gameObject;
                        }
                    }
                }
            }
        }
    }
}
