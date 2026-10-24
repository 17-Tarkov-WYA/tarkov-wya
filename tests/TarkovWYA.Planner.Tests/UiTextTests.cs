namespace TarkovWYA.Planner.Tests;

public class UiTextTests
{
    [Fact]
    public void MainWindowTitle_Default_IsPlannerName()
    {
        Assert.Equal("Tarkov-WYA Planner", UiText.MainWindowTitle);
    }
}
