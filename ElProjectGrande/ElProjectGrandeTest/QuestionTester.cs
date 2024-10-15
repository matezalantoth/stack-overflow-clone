using ElProjectGrande.Models.QuestionModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest;

public abstract class QuestionTester(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    protected readonly NewQuestion NewQuestion =  new NewQuestion { Content = "This is a test question", Tags = [], Title = "This is a test title", PostedAt = DateTime.Now };
}