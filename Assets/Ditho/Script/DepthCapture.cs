using UnityEngine;

namespace Ditho
{
    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    sealed class DepthCapture : MonoBehaviour
    {
        [SerializeField, HideInInspector] Shader _shader = null;

        Material _material;

        void OnDestroy()
        {
            Utility.Destroy(_material);
        }

        void Update()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            Graphics.Blit(source, destination, _material, 0);
        }
    }
}
