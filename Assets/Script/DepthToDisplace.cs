using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace DepthCamFx
{
    [ExecuteInEditMode]
    sealed class DepthToDisplace : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField, Range(8, 512)] int _columnCount = 256;
        [SerializeField, Range(8, 512)] int _rowCount = 256;
        [SerializeField, ColorUsage(false, true)] Color _baseColor = Color.white;
        [SerializeField, ColorUsage(false, true)] Color _sparkleColor = Color.white;
        [SerializeField] float _depthScale = 1;
        [SerializeField] Texture _sourceTexture = null;
        [SerializeField] Shader _shader = null;

        #endregion

        #region Private members

        Mesh _mesh;
        Material _material;

        void LazyInitialize()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.hideFlags = HideFlags.DontSave;
                _mesh.name = "Depth To Displace";
                _mesh.indexFormat = IndexFormat.UInt32;
                ReconstructMesh();
            }

            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }
        }

        #endregion

        #region Internal methods

        internal void ReconstructMesh()
        {
            var vertices  = new List<Vector3>();

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
            if (Application.isPlaying)
            {
                if (_mesh != null) Destroy(_mesh);
                if (_material != null) Destroy(_material);
            }
            else
            {
                if (_mesh != null) DestroyImmediate(_mesh);
                if (_material != null) DestroyImmediate(_material);
            }

            _mesh = null;
            _material = null;
        }

        void Update()
        {
            LazyInitialize();

            _material.SetColor("_BaseColor", _baseColor);
            _material.SetColor("_SparkleColor", _sparkleColor);
            _material.SetFloat("_DepthScale", _depthScale);
            _material.mainTexture = _sourceTexture;

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );
        }

        #endregion
    }
}
