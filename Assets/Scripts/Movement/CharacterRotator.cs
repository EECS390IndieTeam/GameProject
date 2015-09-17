using UnityEngine;
using System.Collections;

public class CharacterRotator {


    Rigidbody character;

    public CharacterRotator(Rigidbody character)
    {
        this.character = character;
    }

    public void rotateCharacter(float direction)
    {
        character.transform.rotation *= Quaternion.Euler(0, 0, direction);
    }



	
}
