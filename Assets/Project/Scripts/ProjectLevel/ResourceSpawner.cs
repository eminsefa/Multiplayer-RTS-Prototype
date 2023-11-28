using System;
using Cysharp.Threading.Tasks;
using Pathfinding;
using Project.Scripts.Managers;
using Project.Scripts.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.ProjectLevel
{
    public class ResourceSpawner : MonoBehaviour
    {
        private Collider[]              m_OverlapCols = new Collider[16];

        [SerializeField] private LayerMask  m_OverlapLayer;

        private void OnEnable()
        {
            if(!GameLauncher.IsDecider) return;
            ResourceBase.OnResourceCollected += OnResourceCollected;
        }

        private void OnDisable()
        {
            ResourceBase.OnResourceCollected -= OnResourceCollected;
        }

        private void OnResourceCollected(ResourceBase resource, CharacterBase character)
        {
            resource.RPC_DisableResource();
            SpawnResource(resource);
        }

        private async void SpawnResource(ResourceBase resource)
        {
            var spawnPos = await FindEmptySpace();
            resource.transform.position = spawnPos;
            resource.RPC_EnableResource();
        }

        private async UniTask<Vector3> FindEmptySpace()
        {
            try
            {
                var constraint = new NNConstraint
                                 {
                                     walkable = true
                                 };

                var borders    = LevelManager.CurrentLevel.Borders;

                bool    isEmpty  = false;
                Vector3 spawnPos = Vector3.zero;
                while (!isEmpty)
                {
                    var randPos     = new Vector3(Random.Range(-borders.x, borders.x), 0, Random.Range(-borders.y, borders.y));
                    spawnPos = (Vector3) AstarPath.active.GetNearest(randPos,constraint).node.position;
                    var count       = Physics.OverlapSphereNonAlloc(spawnPos, 5, m_OverlapCols, m_OverlapLayer);
                    isEmpty = count < 1;

                    await UniTask.Delay(10);
                }

                return spawnPos;
            }
            catch (Exception e)
            {
                if (!e.IsOperationCanceledException()) Debug.LogError($"error on find empty space,:{e}");
                throw;
            }
        }
    }
}