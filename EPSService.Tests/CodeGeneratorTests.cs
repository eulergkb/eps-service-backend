using ESPService.Services;
using FluentAssertions;

namespace EPSService.Tests;

public class CodeGeneratorTests
{
    [Theory]
    [InlineData(7)]
    [InlineData(8)]
    public async Task Should_Generate_Random_Code_Of_Length(int codeLength)
    {
        ICodeGenerator codeGenerator = new CodeGeneratorService();

        var code = await codeGenerator.GenerateUniqueCode(codeLength);

        code.Should().NotBeNull();
        code.Should().HaveLength(codeLength);
    }
}