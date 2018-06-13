using UnityEngine;
using System.Collections;

namespace MSL{
    public static class Assert
    {
        public static void IsTrue(bool condition, string message)
        {
            if(!condition)
            {
                Log.Error(message);
                throw new System.Exception();
            }
        }

        public static void IsFalse(bool condition, string message)
        {
            IsTrue(!condition, message);
        }
    }
}