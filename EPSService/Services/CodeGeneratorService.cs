using ESPService.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPService.Services;

public class CodeGeneratorService: ICodeGenerator
{
    public ValueTask<string> GenerateUniqueCode(int length)
    {
        return ValueTask.FromResult(Guid.NewGuid().ToString("N").Substring(0, length).ToUpperInvariant());
    }
}
