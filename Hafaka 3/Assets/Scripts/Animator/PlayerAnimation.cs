using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAnimation : MonoBehaviour
{

    private PlayerNewInput _input;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _input = new PlayerNewInput();
    }
    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Attack.started += _ => AttackAnim();
        _input.Player.Move.performed += _ => StartWalking();
        _input.Player.Move.canceled += _ => StopWalking();

        _input.Player.Escape.started += _ => Pause();
        _input.UI.Escape.performed += _ => Pause();
        _input.UI.Escape.Disable();
    }


    private void OnDisable()
    {
        _input.Disable();
    }

    private void Pause()
    {
        if (GameManager.Instance.IsPaused)
        {
            _input.Player.Disable();
            _input.UI.Escape.Enable();
        }
        else
        {
            _input.Player.Enable();
            _input.UI.Escape.Disable();
        }
    }

    private void StartWalking()
    {
        _animator.SetBool("isWalking", true);
    }
    
    private void StopWalking()
    {
        _animator.SetBool("isWalking", false);
    }

    private void AttackAnim()
    {
        if (!PlayerController.Instance.IsInventoryOpen)
        {
            if (PlayerController.Instance.AttackCounter == 0)
            {
                _animator.SetBool("SwingLeftToRight", true);
            }
            else if (PlayerController.Instance.AttackCounter == 1)
            {
                _animator.SetBool("SwingRightToLeft", true);
            }
            else if (PlayerController.Instance.AttackCounter == 2)
            {
                _animator.SetBool("SwingUpToDawn", true);
            }
            StopAttackAnim();
        }
    }

    private void StopAttackAnim()
    {
            StartCoroutine(StopAttackDelay());
    }

    public IEnumerator StopAttackDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _animator.SetBool("SwingLeftToRight", false);
        _animator.SetBool("SwingRightToLeft", false);
        _animator.SetBool("SwingUpToDawn", false);
    }
}
