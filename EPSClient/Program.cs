
using ESPService;
using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine(" ==> WELCOME TO ESP CLIENT. PRESS ENTER TO CONNECT <==");
Console.ReadLine();

const int DEFAULT_CODE_LENGTH = 8;

const string GRPC_SERVER_URL = "https://localhost:7090";
const string SERVER_URL = "http://localhost:5007";

var channel = Grpc.Net.Client.GrpcChannel.ForAddress(
    GRPC_SERVER_URL,
    new Grpc.Net.Client.GrpcChannelOptions()
);

var client = new DiscountCodeGenerator.DiscountCodeGeneratorClient(channel);


var hubConnection = new HubConnectionBuilder().WithUrl($"{SERVER_URL}/CodesHub", c =>
{
    c.SkipNegotiation = true;
    c.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
}).Build();

hubConnection.On("OnCodeGenerated", (string[] codes) =>
{
    Console.WriteLine($"\n[WEBSOCKET]: New Codes Generated - {codes.Length}\n");
    Console.WriteLine(string.Join(", ", codes));
});

hubConnection.On("OnCodeUsed", (string code) =>
{
    Console.WriteLine($"\n[WEBSOCKET]: Code Used - {code}\n");
});

await hubConnection.StartAsync();


while (true)
{
    Console.WriteLine("-> Choose an option");
    Console.WriteLine("   1. Generate discount codes");
    Console.WriteLine("   2. Get all discount codes");
    Console.WriteLine("   3. Use discount code\n");
    var input = Console.ReadKey();
    Console.WriteLine();

    switch (input.KeyChar)
    {
        case '1':
            Console.Write("How many codes do you want to generate?: ");
            if (uint.TryParse(Console.ReadLine(), out uint size) && size >= 1 && size <= 2000)
            {
                var codes = await client.GenerateCodesAsync(
                    new GenerateCodeRequest() { Count = size, Length = DEFAULT_CODE_LENGTH }
                    );

                Console.WriteLine($"Code Generation Reply: {codes.Result}");
            }
            else
            {
                Console.WriteLine("You entered an invalid range. Value should be within 1-2000");
            }

            break;
        case '2':

            var discountCodes = await client.GetAllCodesAsync(new());
            Console.WriteLine($"-> Got {discountCodes.Codes.Count} discount codes");
            Console.WriteLine(string.Join(", ", discountCodes.Codes.ToArray()));

            break;
        case '3':
            Console.Write("Enter discount code: ");
            var discountCode = Console.ReadLine();
            var result = await client.UseCodeAsync(new UseCodeRequest() { Code = discountCode });
            Console.WriteLine("Use Code Reply: {0}", result.Result);
            break;
        default:
            Console.WriteLine("You entered a wrong option. Please try again (:-");
            break;
    }
}
