using BackendTest.TestHelpers;
using ElProjectGrandeTest.TestHelpers;
using Xunit.Abstractions;

namespace BackendTest;

public abstract class Tester
{
    protected readonly AdminControllerTestHelper AHelper;
    protected readonly AnswerControllerTestHelper AnsHelper;
    protected readonly HttpClient Client;
    protected readonly QuestionControllerTestHelper QHelper;
    protected readonly ITestOutputHelper StdOut;
    protected readonly TagControllerTestHelper TagHelper;
    protected readonly UserControllerTestHelper UHelper;

    protected Tester(ITestOutputHelper outputHelper)
    {
        StdOut = outputHelper;
        Client = new BackendServerAppFactory().CreateClient();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        UHelper = new UserControllerTestHelper(Client);
        QHelper = new QuestionControllerTestHelper(Client);
        AHelper = new AdminControllerTestHelper(Client);
        AnsHelper = new AnswerControllerTestHelper(Client);
        TagHelper = new TagControllerTestHelper(Client);
    }
}