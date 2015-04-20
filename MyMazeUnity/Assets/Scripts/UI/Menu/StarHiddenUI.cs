﻿using UnityEngine;
using System.Collections;

public class StarHiddenUI : StarUI {

    public void SetNotAvalible()
    {
        stars.recived.gameObject.SetActive(false);
        stars.lost.gameObject.SetActive(false);
    }
}
