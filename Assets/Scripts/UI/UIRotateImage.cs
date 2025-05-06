using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateImage : MonoBehaviour
{
   public float rotationSpeed = 30f;

   void Update()
   {
      transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
   }
}
