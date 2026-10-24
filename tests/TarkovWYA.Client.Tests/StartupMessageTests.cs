namespace TarkovWYA.Client.Tests;

public class StartupMessageTests
{
    [Fact]
    public void Text_Default_IsTraditionalChineseMessage()
    {
        Assert.Equal("Tarkov-WYA Client 已啟動。", StartupMessage.Text);
    }
}
