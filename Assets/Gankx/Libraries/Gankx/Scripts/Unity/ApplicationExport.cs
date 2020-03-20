using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationExport {

    public static void SetBackgroundLoadingPriority(int level) {
        Application.backgroundLoadingPriority = (ThreadPriority)level;
    }
}
