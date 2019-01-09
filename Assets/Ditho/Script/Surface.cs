using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Ditho
{
    [ExecuteInEditMode]
    sealed class Surface : MonoBehaviour
    {
        #region Editable attributes

        enum RenderMode { Body, Aura }

        [SerializeField, Range(8, 512)] int _columnCount = 256;
        [SerializeField, Range(8, 512)] int _rowCount = 256;

        [SerializeField] Texture _sourceTexture = null;
        [SerializeField] float _depthScale = 1;

        [SerializeField, ColorUsage(false, true)] Color _lineColor = Color.white;
        [SerializeField] float _lineRepeat = 200;

        [SerializeField, ColorUsage(false, true)] Color _sparkleColor = Color.white;
        [SerializeField, Range(0, 1)] float _sparkleDensity = 0.5f;

        [SerializeField] RenderMode _renderMode = RenderMode.Body;
        [SerializeField, Range(0, 1)] float _deformation = 0;

        [SerializeField] Shader _bodyShader = null;
        [SerializeField] Shader _auraShader = null;

        #endregion

        #region Private members

        Mesh _mesh;
        Material _material;

        Shader ShaderForCurrentMode { get {
            return _renderMode == RenderMode.Body ? _bodyShader : _auraShader;
        } }

        #endregion

        #region Internal methods

        internal void Reconstruct()
        {
            // Material reconstruction
            if (_material != null) Utility.Destroy(_material);
            _material = new Material(ShaderForCurrentMode);
            _material.hideFlags = HideFlags.DontSave;

            // Mesh object lazy initialization
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.hideFlags = HideFlags.DontSave;
                _mesh.name = "Depth To Displace";
                _mesh.indexFormat = IndexFormat.UInt32;
            }

            // Mesh reconstruction
            var vertices = new List<Vector3>();

            for (var ri = 0; ri < _rowCount; ri++)
            {
                var v = (float)ri / (_rowCount - 1);
                for (var ci = 0; ci < _columnCount; ci++)
                {
                    var u = (float)ci / (_columnCount - 1);
                    vertices.Add(new Vector3(u, v, 0));
                }
            }

            var indices = new int[(_rowCount - 1) * (_columnCount - 1) * 6];
            var i = 0;

            for (var ri = 0; ri < _rowCount - 1; ri++)
            {
                for (var ci = 0; ci < _columnCount - 1; ci++)
                {
                    var head = _columnCount * ri + ci;

                    indices[i++] = head;
                    indices[i++] = head + 1;
                    indices[i++] = head + _columnCount;

                    indices[i++] = head + 1;
                    indices[i++] = head + _columnCount + 1;
                    indices[i++] = head + _columnCount;
                }
            }

            _mesh.Clear();
            _mesh.SetVertices(vertices);
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
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

            _material.mainTexture = _sourceTexture;
            _material.SetFloat("_DepthScale", _depthScale);

            _material.SetColor("_LineColor", _lineColor);
            _material.SetFloat("_LineRepeat", _lineRepeat);

            _material.SetColor("_SparkleColor", _sparkleColor);
            _material.SetFloat("_SparkleDensity", _sparkleDensity);

            _material.SetFloat("_Deformation", _deformation);

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );
        }

        #endregion
    }
}
