using UnityEngine;

namespace PS_TiltFive.Core
{
    public static class CameraHelper
    {
        public static Camera Main =>
            Camera.main != null ? Camera.main
            : Camera.allCameras.Length > 0 ? Camera.allCameras[0]
            : null;
    }
}
