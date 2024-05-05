using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
   private Func<bool> _condition;

   public Condition(Func<bool> condition)
   {
      _condition = condition;
   }
   
   public bool Evaluate()
   {
      return _condition.Invoke();
   }
}
