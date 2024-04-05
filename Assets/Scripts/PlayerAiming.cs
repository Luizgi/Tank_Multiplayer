using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turrentTransform;
    [SerializeField] private InputReader inputReader;
    public Texture2D aim;
    private Vector2 cursorHotspot;

    public void Start()
    {
        cursorHotspot = new Vector2 (75.5f , 75.5f);

        Cursor.SetCursor(aim, cursorHotspot, CursorMode.Auto);
    }


    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector2 aimScreenPosition = inputReader.aimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turrentTransform.up = new Vector2(aimWorldPosition.x - turrentTransform.position.x, aimWorldPosition.y - turrentTransform.position.y)                                         ;
    }
}
