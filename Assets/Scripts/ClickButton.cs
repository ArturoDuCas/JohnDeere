using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image img;
    [SerializeField] private Sprite defaultSprite, pressedSprite;
    [SerializeField] private AudioClip compressClip, uncompressClip;
    [SerializeField] private AudioSource source;
    [SerializeField] private PanelTransition panelTransition;

    private void Start()
    {
        // Assuming you attach this script to the same GameObject as the PanelTransition script
        panelTransition = GetComponent<PanelTransition>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = pressedSprite;
        source.PlayOneShot(compressClip);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img.sprite = defaultSprite;
        source.PlayOneShot(uncompressClip);

        // Call the ShowIDPanel method to transition to the ID Panel
        panelTransition.ShowIDPanel();
    }
}
