using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AirEngine : MonoBehaviour
{
   protected abstract void MiddleMode();
   protected abstract void WeakMode();
   protected abstract void StrongMode();

   protected abstract void EffectOn(Transform[] places, GameObject prefabEffect, int maxEffects);
}
