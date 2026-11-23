#include <stdint.h>
#include <string.h>
#include <time.h>
#include <sys/sysctl.h>

extern "C" uint64_t GroveGamesPlatform_GetTimeSinceBootNanoseconds(void)
{
    return clock_gettime_nsec_np(CLOCK_MONOTONIC_RAW);
}

extern "C" int32_t GroveGamesPlatform_GetBootId(char *buffer, int32_t capacity)
{
    if (buffer == NULL || capacity <= 0)
    {
        return 0;
    }

    size_t length = (size_t)capacity;

    if (sysctlbyname("kern.bootsessionuuid", buffer, &length, NULL, 0) != 0)
    {
        return 0;
    }

    return (int32_t)strnlen(buffer, length);
}
