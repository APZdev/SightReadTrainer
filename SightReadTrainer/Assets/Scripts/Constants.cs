using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static bool IntToBool(int value)
    {
        return value == 1 ? true : false;
    }

    public static int BoolToInt(bool value)
    {
        return value == true ? 1 : 0;
    }
}
