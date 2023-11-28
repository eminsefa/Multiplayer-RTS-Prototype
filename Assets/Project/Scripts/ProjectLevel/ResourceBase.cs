using System;
using Fusion;
using Project.Scripts.Interfaces;
using Project.Scripts.Managers;
using Project.Scripts.Player;
using UnityEngine;

namespace Project.Scripts.ProjectLevel
{
    public class ResourceBase : NetworkBehaviour, IClickable
    {
        public static event Action<ResourceBase, CharacterBase> OnResourceCollected;

        private bool m_IsCollected;

        [SerializeField] private Collider   m_Col;
        [SerializeField] private GameObject m_Model;

        public override void Spawned()
        {
            base.Spawned();
            if (GameLauncher.IsDecider)
            {
                Object.RequestStateAuthority();
                RPC_EnableResource();
            }
        }
        
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_EnableResource()
        {
            m_Col.enabled = true;
            m_IsCollected = false;
            m_Model.SetActive(true);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_DisableResource()
        {
            m_IsCollected = true;
            m_Model.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_IsCollected) return;
            m_Col.enabled = false;
            m_Model.SetActive(false);

            if (!HasStateAuthority) return;
            other.TryGetComponent(out CharacterBase character);
            RPC_HasCollected(character);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_HasCollected(CharacterBase character)
        {
            OnResourceCollected?.Invoke(this, character);
        }

        public void Clicked()
        {
        }
    }
}