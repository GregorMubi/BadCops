using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    [SerializeField] private SpriteRenderer SpriteRenderer = null;

    [SerializeField] public Sprite SpriteNorth = null;
    [SerializeField] public Sprite SpriteEast = null;
    [SerializeField] public Sprite SpriteSouth = null;
    [SerializeField] public Sprite SpriteWest = null;

    private float Rotation = 0.0f;

    public void SetRotation(float rotation) {
        while (rotation < 0) {
            rotation += 360;
        }
        while (rotation > 360) {
            rotation -= 360;
        }

        Rotation = rotation;
        if (Rotation < 45) {
            SpriteRenderer.sprite = SpriteNorth;
        } else if (Rotation < 135) {
            SpriteRenderer.sprite = SpriteEast;
        } else if (Rotation < 225) {
            SpriteRenderer.sprite = SpriteSouth;
        } else {
            SpriteRenderer.sprite = SpriteWest;
        }
    }
}
