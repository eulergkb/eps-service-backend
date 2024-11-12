using ESPService.Repository;
using Grpc.Core;

namespace ESPService.Services;

public partial class DiscountCodeGeneratorService(
        ILogger<DiscountCodeGeneratorService> logger,
        ICodeGenerator codeGenerator,
        IDiscountCodeRepository codeRepository,
        IWebsocketNotificationService websocketNotification
    ) : DiscountCodeGenerator.DiscountCodeGeneratorBase

{
    public override async Task<GetAllCodesReply> GetAllCodes(GetAllCodesRequest request, ServerCallContext context)
    {
        var codes = await codeRepository.GetAll();
        return new GetAllCodesReply() { Codes = { codes } };
    }

    public override async Task<GenerateReply> GenerateCodes(GenerateCodeRequest request, ServerCallContext context)
    {
        logger.LogInformation("Generating Codes - [ Count: {0}, Length: {1} ]", request.Count, request.Length);

        var size = (int)request.Count;
        var length = request.Length;

        var codes = new List<string>(size);
        while (codes.Count < size)
        {
            var code = await codeGenerator.GenerateUniqueCode(length);
            var newCode = await codeRepository.Insert(code, true);
            if (newCode != null)
            {
                codes.Add(code);
            }
            context.CancellationToken.ThrowIfCancellationRequested();
        }

        //  Broadcast message
        await websocketNotification.OnCodesGenerated(codes.ToArray());

        return new GenerateReply() { Result = true };

    }

    public override async Task<UseCodeReply> UseCode(UseCodeRequest request, ServerCallContext context)
    {
        logger.LogInformation("Using Code - [ Code: {0} ]", request.Code);

        var code = request.Code.ToUpperInvariant();

        var result = await codeRepository.UseCode(code);
        if (result)
            await websocketNotification.OnCodeUsed(code);

        return new UseCodeReply() { Result = result ? 1 : 0 };
    }
}
