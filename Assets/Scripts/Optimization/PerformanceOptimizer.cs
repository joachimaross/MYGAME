using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MarketHustle.Optimization
{
    /// <summary>
    /// Performance optimization utilities for mobile deployment.
    /// </summary>
    public class PerformanceOptimizer : MonoBehaviour
    {
        [Header("Object Pooling")]
        public GameObject buildingPrefab;
        public int initialPoolSize = 50;

        [Header("LOD Settings")]
        public float lodDistance1 = 50f;
        public float lodDistance2 = 100f;

        [Header("Culling")]
        public float cullingDistance = 200f;

        private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<GameObject, LODGroup> lodGroups = new Dictionary<GameObject, LODGroup>();

        void Awake()
        {
            InitializeObjectPooling();
            OptimizePhysics();
            OptimizeRendering();
        }

        void InitializeObjectPooling()
        {
            // Create object pools for frequently spawned objects
            CreatePool("Building", buildingPrefab, initialPoolSize);
            CreatePool("Furniture", null, 20); // Will be populated dynamically
        }

        void CreatePool(string poolName, GameObject prefab, int size)
        {
            if (prefab == null) return;

            var pool = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.name = $"{poolName}_{i}";
                pool.Enqueue(obj);
            }
            objectPools[poolName] = pool;
        }

        public GameObject GetPooledObject(string poolName)
        {
            if (objectPools.ContainsKey(poolName) && objectPools[poolName].Count > 0)
            {
                var obj = objectPools[poolName].Dequeue();
                obj.SetActive(true);
                return obj;
            }
            return null;
        }

        public void ReturnToPool(string poolName, GameObject obj)
        {
            if (objectPools.ContainsKey(poolName))
            {
                obj.SetActive(false);
                objectPools[poolName].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        void OptimizePhysics()
        {
            // Reduce physics calculations for mobile
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 2;
            Physics.sleepThreshold = 0.005f;
            Physics.defaultContactOffset = 0.01f;

            // Disable unnecessary physics layers
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (i != j) // Only keep self-collision
                        Physics.IgnoreLayerCollision(i, j, true);
                }
            }
            // Re-enable necessary collisions
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Default"), false);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Interactable"), false);
        }

        void OptimizeRendering()
        {
            // Set quality settings for mobile
            QualitySettings.SetQualityLevel(1); // Medium quality
            QualitySettings.pixelLightCount = 2;
            QualitySettings.shadowDistance = 20f;
            QualitySettings.shadowCascades = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

            // Enable GPU instancing
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
        }

        public void SetupLOD(GameObject target)
        {
            if (target == null) return;

            var lodGroup = target.GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                lodGroup = target.AddComponent<LODGroup>();
            }

            // Create LOD levels
            var lods = new LOD[3];

            // LOD 0: Full detail (0-50m)
            lods[0] = new LOD(lodDistance1 / cullingDistance, new Renderer[] { target.GetComponent<Renderer>() });

            // LOD 1: Medium detail (50-100m) - reduce mesh complexity
            var mediumLOD = CreateLODMesh(target, 0.7f);
            lods[1] = new LOD(lodDistance2 / cullingDistance, mediumLOD != null ? new Renderer[] { mediumLOD } : new Renderer[0]);

            // LOD 2: Low detail/Cull (100m+)
            lods[2] = new LOD(0.01f, new Renderer[0]); // Cull

            lodGroup.SetLODs(lods);
            lodGroups[target] = lodGroup;
        }

        Renderer CreateLODMesh(GameObject original, float scale)
        {
            var lodObject = Instantiate(original);
            lodObject.transform.localScale = original.transform.localScale * scale;
            lodObject.name = original.name + "_LOD";

            // Simplify mesh (basic implementation - in practice use mesh decimation)
            var meshFilter = lodObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                // Reduce vertex count by scaling (simplified)
                var mesh = meshFilter.mesh;
                var vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] *= scale;
                }
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            }

            return lodObject.GetComponent<Renderer>();
        }

        // Memory management
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        // Scene loading optimization
        public void LoadSceneAdditive(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}