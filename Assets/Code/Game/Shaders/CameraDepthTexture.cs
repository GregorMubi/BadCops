using UnityEngine;

[ExecuteInEditMode]
public class CameraDepthTexture : MonoBehaviour {

    [SerializeField]private Camera camera;

    void Start() {
        camera.depthTextureMode = DepthTextureMode.Depth;
    }


    private void Update() {
        //camera.depthTextureMode = DepthTextureMode.Depth;
    }
}