using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Ditho
{
    [ExecuteInEditMode]
    sealed class Warp : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] int _lineCount = 1000;

        [SerializeField] float _speed = 1;
        [SerializeField, Range(0, 1)] float _speedRandomness = 0.5f;

        [SerializeField, Range(0, 0.3f)] float _length = 0.1f;
        [SerializeField, Range(0, 1)] float _lengthRandomness = 0.5f;

        [SerializeField, ColorUsage(false, true)] Color _lineColor = Color.white;
        [SerializeField, ColorUsage(false, true)] Color _sparkleColor = Color.white;
        [SerializeField, Range(0, 1)] float _sparkleDensity = 0.5f;

        [SerializeField, HideInInspector] Shader _shader = null;

        void OnValidate()
        {
            _lineCount = Mathf.Max(_lineCount, 1);
        }

        #endregion

        #region Private members

        Mesh _mesh;
        Material _material;
        float _time = 10;

        void LazyInitialize()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.hideFlags = HideFlags.DontSave;
                _mesh.name = "Warp Effect";
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
            _mesh.Clear();
            _mesh.vertices = new Vector3[_lineCount * 2];
            _mesh.SetIndices(
                Enumerable.Range(0, _lineCount * 2).ToArray(),
                MeshTopology.Lines, 0
            );
            _mesh.bounds = new Bounds(Vector3.zero, new Vector3(1, 1, 1));
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
            if (Application.isPlaying) _time += Time.deltaTime;

            LazyInitialize();

            _material.SetVector("_Speed", new Vector2(_speed, _speedRandomness));
            _material.SetVector("_Length", new Vector2(_length, _lengthRandomness));
            _material.SetColor("_LineColor", _lineColor);
            _material.SetColor("_SparkleColor", _sparkleColor);
            _material.SetFloat("_SparkleDensity", _sparkleDensity);
            _material.SetFloat("_LocalTime", _time);

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );
        }

        #endregion
    }
}
