namespace ESPService.Services;

public interface ICodeGenerator
{
    ValueTask<string> GenerateUniqueCode(int length);
}
