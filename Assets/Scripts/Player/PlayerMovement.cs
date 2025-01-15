using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float Speed = 3;

    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite back;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool isMove = false;

    private void Start()
    {
        playerObj = gameObject;
        
        Action upAction = () =>
        {
            if (!isMove || GameManager.Instance.mode == Now_Mode.Battle) return;
            playerObj.transform.position += new Vector3(0, 1, 0) * (Speed * Time.deltaTime);
            spriteRenderer.sprite = back;
            GameManager.Instance.countStep += 1;
        };
        InputManager.Instance.SetKeyEvent(UseButtonType.Player, InputMaster.Entity.FieldKey.Up, upAction);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Player,KeyCode.UpArrow,upAction,false);

        Action downAction = () =>
        {
            if (!isMove || GameManager.Instance.mode == Now_Mode.Battle) return;
            playerObj.transform.position -= new Vector3(0, 1, 0) * (Speed * Time.deltaTime);
            spriteRenderer.sprite = idle;
            GameManager.Instance.countStep += 1;
        };
        InputManager.Instance.SetKeyEvent(UseButtonType.Player, InputMaster.Entity.FieldKey.Down,downAction);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Player,KeyCode.DownArrow,downAction,false);

        Action rightAction = () =>
        {
            if (!isMove || GameManager.Instance.mode == Now_Mode.Battle) return;
            playerObj.transform.position += new Vector3(1, 0, 0) * (Speed * Time.deltaTime);
            spriteRenderer.sprite = right;
            GameManager.Instance.countStep += 1;
        };
        InputManager.Instance.SetKeyEvent(UseButtonType.Player, InputMaster.Entity.FieldKey.Right,rightAction);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Player,KeyCode.RightArrow,rightAction,false);

        Action leftAction = () =>
        {
            if (!isMove || GameManager.Instance.mode == Now_Mode.Battle) return;
            playerObj.transform.position -= new Vector3(1, 0, 0) * (Speed * Time.deltaTime);
            spriteRenderer.sprite = left;
            GameManager.Instance.countStep += 1;
        };

        InputManager.Instance.SetKeyEvent(UseButtonType.Player, InputMaster.Entity.FieldKey.Left,leftAction);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Player,KeyCode.LeftArrow,leftAction,false);
        

    }
}
