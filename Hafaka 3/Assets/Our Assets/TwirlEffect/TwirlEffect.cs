using UnityEngine;

public class TwirlEffect : MonoBehaviour
{
    public Material twirlMaterial;  // Assign your Shader Graph material
    private float transitionProgress = 0f;
    private bool isTransitioning = false;

    public void StartTwirl()
    {
        transitionProgress = 0f;
        isTransitioning = true;
    }

    private void Update()
    {
        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / 2f; // 2 sec transition
            if (transitionProgress >= 1f)
            {
                transitionProgress = 1f;
                isTransitioning = false;
            }
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (twirlMaterial)
        {
            twirlMaterial.SetFloat("_TwirlAmount", transitionProgress);
            Graphics.Blit(src, dest, twirlMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
