using ESPService.Data.Models;
using ESPService.Repository;
using ESPService.Services;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPSService.Tests;

public class DiscountGeneratorServiceTests
{
    Mock<IDiscountCodeRepository> MockRepository(List<DiscountCode> dataSource)
    {
        var repositoryMock = new Moq.Mock<IDiscountCodeRepository>();

        repositoryMock.Setup(e => e.GetAll()).Returns(() =>
        {
            return Task.FromResult(dataSource.Select(e => e.Code).ToArray());
        });

        repositoryMock.Setup(e => e.Insert(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync((string code, bool checkIfExists) =>
        {
            var model = new DiscountCode() { Code = code };
            dataSource.Add(model);
            return model;
        });

        repositoryMock.Setup(e => e.UseCode(It.IsAny<string>())).ReturnsAsync((string code) =>
        {
            var model = dataSource.Find(x => x.Code == code);
            if (model is not null)
            {
                model.UsedAt = DateTime.UtcNow;
                return true;
            }

            return false;
        });

        return repositoryMock;
    }

    Mock<IWebsocketNotificationService> MockNotificationService()
    {
        var mock = new Mock<IWebsocketNotificationService>();
        mock.Setup(e => e.OnCodesGenerated(It.IsAny<string[]>())).Returns(Task.CompletedTask);
        mock.Setup(e => e.OnCodeUsed(It.IsAny<string>())).Returns(Task.CompletedTask);
        return mock;
    }

    DiscountCodeGeneratorService CreateService(
        IDiscountCodeRepository codeRepository,
        ICodeGenerator codeGenerator,
        IWebsocketNotificationService notificationService
        )
    {
        var service = new DiscountCodeGeneratorService(
                Mock.Of<ILogger<DiscountCodeGeneratorService>>(),
                codeGenerator,
                codeRepository,
                notificationService
            );

        return service;
    }

    ServerCallContext GetCallContext(string method) => TestServerCallContext.Create(method,
                                                                                    host: "localhost",
                                                                                    deadline: DateTime.Now.AddMinutes(30),
                                                                                    requestHeaders: new Metadata(),
                                                                                    cancellationToken: CancellationToken.None,
                                                                                    peer: "0.0.0.0:5001",
                                                                                    authContext: null,
                                                                                    contextPropagationToken: null,
                                                                                    writeHeadersFunc: (metadata) => Task.CompletedTask,
                                                                                    writeOptionsGetter: () => new WriteOptions(),
                                                                                    writeOptionsSetter: (writeOptions) => { });
    [Theory]
    [InlineData(10, 7)]
    [InlineData(20, 8)]
    [InlineData(100, 8)]
    public async Task Shoud_Generate_Random_Codes_Successfully(uint size, int length)
    {
        var codes = new List<DiscountCode>() { };
        var repository = MockRepository(codes);
        var notification = MockNotificationService();

        var codeGen = new CodeGeneratorService();
        var service = CreateService(repository.Object, codeGen, notification.Object);

        var response = await service.GenerateCodes(new ESPService.GenerateCodeRequest()
        {
            Count = size,
            Length = length
        }, GetCallContext("GenerateCodes"));


        response.Should().NotBeNull();
        response.Result.Should().BeTrue();
        notification.Verify(e => e.OnCodesGenerated(It.IsAny<string[]>()));
    }

    [Fact]
    public async Task Should_Get_All_Codes_Successfully()
    {
        var codes = new List<DiscountCode>()
        {
            new DiscountCode() { Code = "12345678" , CreatedAt = DateTime.UtcNow},
            new DiscountCode() { Code = "22222223" , CreatedAt = DateTime.UtcNow},
            new DiscountCode() { Code = "4546555" , CreatedAt = DateTime.UtcNow},
            new DiscountCode() { Code = "66868787" , CreatedAt = DateTime.UtcNow},
            new DiscountCode() { Code = "7878787" , CreatedAt = DateTime.UtcNow}
        };

        var repository = MockRepository(codes);
        var notification = MockNotificationService();

        var codeGen = new CodeGeneratorService();
        var service = CreateService(repository.Object, codeGen, notification.Object);

        var result = await service.GetAllCodes(new(), GetCallContext("GetAllCodes"));

        result.Should().NotBeNull();
        result.Codes.ToArray().Should().BeEquivalentTo(codes.Select(e => e.Code));
    }

    [Fact]
    public async Task Should_Use_Code_Successfully()
    {
        var codes = new List<DiscountCode>()
        {
            new DiscountCode()
            {
                Code = "12345678"
            }
        };

        var repository = MockRepository(codes);
        var notification = MockNotificationService();

        var codeGen = new CodeGeneratorService();
        var service = CreateService(repository.Object, codeGen, notification.Object);

        var result = await service.UseCode(new ESPService.UseCodeRequest()
        {
            Code = "12345678"
        }, GetCallContext("UseCode"));

        result.Should().NotBeNull();
        result.Result.Should().Be(1);
        notification.Verify(e => e.OnCodeUsed("12345678"));
    }

}
