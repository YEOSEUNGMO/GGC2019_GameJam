using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager _uiManager;

    private void Awake()
    {
        _uiManager = this;
    }

    public GameObject _deadUI;

    public SystemMessage _systemMessage;

    public Image _bossHP;

    public GameObject _victory;
}
