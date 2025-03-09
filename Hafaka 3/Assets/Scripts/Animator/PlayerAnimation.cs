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
    }


    private void OnDisable()
    {
        _input.Disable();
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
