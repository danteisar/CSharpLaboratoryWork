using Store;
using QrCodeGenerator;

Console.WriteLine(new QrCode(" 190924"));

Console.WriteLine("Press any key to continue...");
Console.ReadKey(true);

var terminal = new Terminal();
terminal.Run();

Console.ReadKey(true);
Console.ResetColor();
Console.Clear();

//dotnet publish -r win-x64 -p:PublishSingleFile=true  src\2024\Terminal\Terminal.csproj
