using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace gomoru.su.Ndmarsh
{
    internal static class Utils
    {
        public static T CreateDelegate<T>(this DynamicMethod method) where T : Delegate
        {
            return method.CreateDelegate(typeof(T)) as T;
        }

        public static void Ldarg(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0: il.Emit(OpCodes.Ldarg_0); break;
                case 1: il.Emit(OpCodes.Ldarg_1); break;
                case 2: il.Emit(OpCodes.Ldarg_2); break;
                case 3: il.Emit(OpCodes.Ldarg_3); break;
                default:
                    if (index <= byte.MaxValue)
                        il.Emit(OpCodes.Ldarg_S, index);
                    else
                        il.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }
    }
}
