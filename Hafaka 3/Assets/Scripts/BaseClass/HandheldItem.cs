using UnityEngine;

public abstract class HandheldItem : MonoBehaviour
{
    [SerializeField] protected AnimatorOverrideController toolAnimator; // Unique animator for each tool

    public abstract void Use(); // Define unique behavior for each tool

    public virtual void Equip(Animator playerAnimator)
    {
        if (toolAnimator != null && playerAnimator != null)
        {
            playerAnimator.runtimeAnimatorController = toolAnimator;
        }

        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }
}
