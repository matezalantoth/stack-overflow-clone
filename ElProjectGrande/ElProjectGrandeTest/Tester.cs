using ElProjectGrandeTest.TestHelpers;
using Xunit.Abstractions;

namespace ElProjectGrandeTest;

public abstract class Tester
{
    protected readonly HttpClient Client;
    protected readonly ITestOutputHelper StdOut;
    protected readonly UserControllerTestHelper UHelper;
    protected readonly QuestionControllerTestHelper QHelper;
    protected readonly AdminControllerTestHelper AHelper;

    protected Tester(ITestOutputHelper outputHelper)
    {
        StdOut = outputHelper;
        Client = new ElProjectGrandeAppFactory().CreateClient();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        UHelper = new UserControllerTestHelper(Client);
        QHelper = new QuestionControllerTestHelper(Client);
        AHelper = new AdminControllerTestHelper(Client);
    }
}