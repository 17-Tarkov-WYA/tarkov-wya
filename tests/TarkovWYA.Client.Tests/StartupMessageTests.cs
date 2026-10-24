namespace TarkovWYA.Client.Tests;

public class StartupMessageTests
{
    [Fact]
    public void Text_OnStartup_IsNotEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(StartupMessage.Text));
    }
}
