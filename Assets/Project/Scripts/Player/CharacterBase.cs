using System;
using DG.Tweening;
using Fusion;
using Pathfinding.Examples;
using Pathfinding.RVO;
using Project.Scripts.Interfaces;
using Project.Scripts.Managers;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class CharacterBase : NetworkBehaviour, IClickable
    {
        private static readonly int s_MoveSpeed = Animator.StringToHash("MoveSpeed");

        [Networked] private float   AnimSpeed  { get; set; }
        public              bool    IsSelected { get; private set; }
        public              Vector3 CenterPos  => m_Col.bounds.center;

        private Vector3 m_LastMovePos;
        private Tweener m_TwSelected;

        [SerializeField] private Collider        m_Col;
        [SerializeField] private GameObject      m_SelectedVisual;
        [SerializeField] private Animator        m_Animator;
        [SerializeField] private RVOExampleAgent m_RVOAgent;
        [SerializeField] private RVOController   m_RVOController;

        private void OnEnable()
        {
            m_RVOAgent.SetTarget(transform.position);
            m_TwSelected = m_SelectedVisual.transform.DOLocalMoveY(.75f, 0.35f)
                                           .From(0)
                                           .SetRelative(true)
                                           .SetEase(Ease.OutQuad)
                                           .SetLoops(2, LoopType.Yoyo)
                                           .Pause()
                                           .SetAutoKill(false);
        }

        private void Update()
        {
            if (GameLauncher.GameState != GameLauncher.eGameState.Arena) return;
            if (HasStateAuthority) CalculateAnimSpeed();
            SetAnimSpeed();
        }

        private void CalculateAnimSpeed()
        {
            var mag    = m_RVOController.velocity.magnitude;
            var speedT = mag / m_RVOAgent.maxSpeed;

            if (speedT > 0.25f) m_RVOController.lockWhenNotMoving = true;

            AnimSpeed = Mathf.Lerp(0.25f, 1f, speedT);
        }

        private void SetAnimSpeed()
        {
            m_Animator.SetFloat(s_MoveSpeed, AnimSpeed);
        }

        public void Move(Vector3 worldPos)
        {
            if ((worldPos - m_LastMovePos).sqrMagnitude < 0.25f) return;

            m_RVOController.lockWhenNotMoving = false;
            m_RVOController.locked            = false;
            m_LastMovePos                     = worldPos;
            m_RVOAgent.SetTarget(worldPos);
        }

        public void Clicked()
        {
            if (!HasInputAuthority) return;
            SetSelected(true);
        }

        public void SetSelected(bool selected)
        {
            if (selected && !IsSelected) m_TwSelected.Restart();
            IsSelected = selected;
            m_SelectedVisual.SetActive(IsSelected);
        }
    }
}