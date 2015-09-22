using UnityEngine;
using System.Collections;

public class CharacterRotator {


    Transform character;

    public CharacterRotator(Transform character)
    {
        this.character = character;
    }

    public void rotateCharacter(float direction)
    {
        character.rotation *= Quaternion.Euler(0, 0, direction);
    }



	
}
