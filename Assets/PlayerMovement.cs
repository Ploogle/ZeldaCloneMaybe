using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    Rigidbody myRigidbody;
    float movementSpeed = .1f;
    float rotationSpeed = .3f;//.5f;
    
	void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
	}
    

	void Update()
    {
        Quaternion facingTarget;
        Quaternion currentRotation;
        Vector3 moveDirection;

        // Relative to world X and Z axes.
        moveDirection = Vector3.forward * Input.GetAxisRaw("Vertical") +
                        Vector3.right * Input.GetAxisRaw("Horizontal");

        currentRotation = transform.rotation;
        transform.LookAt(transform.position + moveDirection);
        facingTarget = transform.rotation;
        transform.rotation = Quaternion.Lerp(currentRotation, facingTarget, rotationSpeed);

        //if (moveDirection != Vector3.zero)
        //    myRigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

        if (moveDirection != Vector3.zero)
            myRigidbody.MovePosition(transform.position + moveDirection * movementSpeed);
    }
}
