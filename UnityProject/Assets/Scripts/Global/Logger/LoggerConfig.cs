using UnityEngine;

namespace XNClient.Logger
{
    public static class LoggerConfig
    {
        public readonly static string PATH_LOG = Application.dataPath + "/../log";
        public static bool ENABLE_LOG_WIRTER = true;
    }
}