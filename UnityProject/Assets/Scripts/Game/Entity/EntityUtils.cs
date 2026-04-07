using System;

public static class EntityUtils
{
    private static long _timeStamp = 0;
    private static int _guidInc = 0;
    public static string CreateGuidWithEntityType(string entityType)
    {
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (timeStamp > _timeStamp) {
            _timeStamp = timeStamp;
            _guidInc = 0;
        } else {
            _guidInc += 1;
        }
        return $"{entityType}_{timeStamp}_{_guidInc}";
    }
}
