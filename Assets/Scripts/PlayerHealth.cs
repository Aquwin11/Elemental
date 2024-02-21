using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : CharacterScript
{
    public MainMenuManager mainMenu;
    public override void OnDeath()
    {
        if (mainMenu != null)
            mainMenu.EndScreen();
    }
    public void Start()
    {
        
    }
}
