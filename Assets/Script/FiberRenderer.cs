using UnityEngine;
using System.Linq;

namespace DepthCamFx
{
    [ExecuteInEditMode]
    sealed class FiberRenderer : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] int _pointCount = 1000;

        [SerializeField] float _curveLength = 10;
        [SerializeField] float _curveAnimation = 0.02f;

        [SerializeField] float _noiseAmplitude = 0.05f;
        [SerializeField] float _noiseAnimation = 1;

        [SerializeField] Texture _sourceTexture = null;
        [SerializeField] float _depthScale = 1;
        [SerializeField, ColorUsage(false, true)] Color _lineColor = Color.white;

        [SerializeField, HideInInspector] Shader _shader = null;

        void OnValidate()
        {
            _pointCount = Mathf.Max(_pointCount, 1);
        }

        #endregion

        #region Private members

        Mesh _mesh;
        Material _material;
        float _time;

        #endregion

        #region Internal methods

        internal void Reconstruct()
        {
            // Material object lazy initialization
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            // Mesh object lazy initialization
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.hideFlags = HideFlags.DontSave;
                _mesh.name = "Fiber";
            }

            // Mesh reconstruction
            _mesh.Clear();
            _mesh.vertices = new Vector3[_pointCount];
            _mesh.SetIndices(
                Enumerable.Range(0, _pointCount).ToArray(),
                MeshTopology.LineStrip, 0
            );
            _mesh.bounds = new Bounds(Vector3.zero, new Vector3(1, 1, 10));
            _mesh.UploadMeshData(true);
        }

        #endregion

        #region MonoBehaviour implementation

        void OnDestroy()
        {
            Utility.Destroy(_mesh);
            Utility.Destroy(_material);
        }

        void Update()
        {
            if (_mesh == null) Reconstruct();

            if (Application.isPlaying) _time += Time.deltaTime;

            _material.mainTexture = _sourceTexture;
            _material.SetFloat("_DepthScale", _depthScale);

            _material.SetVector("_CurveParams", new Vector2(
                _curveLength / _pointCount, _curveAnimation
            ));

            _material.SetVector("_NoiseParams", new Vector2(
                _noiseAmplitude, _noiseAnimation
            ));

            _material.SetColor("_LineColor", _lineColor);
            _material.SetFloat("_LocalTime", _time);

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );
        }

        #endregion
    }
}
