using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    [SerializeField] private SpriteRenderer SpriteRenderer = null;

    private float Rotation = 0.0f;
    private TileData Data = null;
    public void SetData(TileData data) {
        Data = data;
    }

    public void SetRotation(float rotation) {
        while (rotation < 0) {
            rotation += 360;
        }
        while (rotation > 360) {
            rotation -= 360;
        }

        Rotation = rotation;
        if (Rotation < 45) {
            SpriteRenderer.sprite = Data.SpriteNorth;
        } else if (Rotation < 135) {
            SpriteRenderer.sprite = Data.SpriteEast;
        } else if (Rotation < 225) {
            SpriteRenderer.sprite = Data.SpriteSouth;
        } else {
            SpriteRenderer.sprite = Data.SpriteWest;
        }
    }
}
