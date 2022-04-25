using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileWeapon : MonoBehaviour
{
   [SerializeField] private int bullets = 10;
   [SerializeField] private GameObject bullet;
   
   public void FireProjectileWeapon(InputAction.CallbackContext context)
   {
      if (context.performed && bullets > 0)
      {
	      Instantiate(bullet, transform.forward + Vector3.up, transform.rotation, null);
      }
   }
}
