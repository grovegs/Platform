using System.Threading;

namespace GroveGames.Platform.Tests;

public sealed class SystemClockDeviceTests
{
    [Fact]
    public void TimeSinceBoot_AfterAWait_AdvancesByAboutTheWait()
    {
        var device = new SystemClockDevice();
        var before = device.TimeSinceBoot;

        Thread.Sleep(50);
        var elapsed = device.TimeSinceBoot - before;

        Assert.InRange(elapsed, TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void BootId_TwoDevices_Differ()
    {
        var first = new SystemClockDevice();
        var second = new SystemClockDevice();

        Assert.NotEqual(first.BootId, second.BootId);
    }
}
