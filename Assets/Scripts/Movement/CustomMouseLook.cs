using UnityEngine;
using System.Collections;

public class CustomMouseLook : MonoBehaviour
{

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public Space space;





    public Transform character;


    public CustomMouseLook(Transform character, float sensitivityX, float sensitivityY)
    {
        this.character = character;
        this.sensitivityX = sensitivityX;
        this.sensitivityY = sensitivityY;
    }

    public void rotateView(float mouseX, float mouseY)
    {
        mouseX *= sensitivityX;
        mouseY *= sensitivityY;
        character.rotation *= Quaternion.Euler(-mouseY, mouseX, 0);
        


        
    }

    //void Update()
    //{


    //    float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
    //    float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        
    //    transform.rotation *= Quaternion.Euler(-mouseY, mouseX, 0);
    //}
}