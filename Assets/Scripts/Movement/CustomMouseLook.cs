using UnityEngine;
using System.Collections;

public class CustomMouseLook
{

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public Space space;





    private Rigidbody character;


    public CustomMouseLook(Rigidbody character, float sensitivityX, float sensitivityY)
    {
        this.character = character;
        this.sensitivityX = sensitivityX;
        this.sensitivityY = sensitivityY;
    }

    public void rotateView(float mouseX, float mouseY)
    {
        mouseX *= sensitivityX;
        mouseY *= sensitivityY;
        character.transform.rotation *= Quaternion.Euler(-mouseY, mouseX, 0);
        


        
    }

    //void Update()
    //{


    //    float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
    //    float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        
    //    transform.rotation *= Quaternion.Euler(-mouseY, mouseX, 0);
    //}
}