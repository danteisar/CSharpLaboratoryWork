using Store;
// using QrCodeGenerator;
// using System.Text;

// var text = "ФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФ";
// Console.WriteLine($"Текст: {text} Размер: {Encoding.UTF8.GetBytes(text).Length * 8}");
// Console.WriteLine(new QrCode(text, CodeType.Binary));//, QR.V9, CorrectionLevel.M, 3)); // 
// Console.WriteLine("Press any key to continue...");
// Console.ReadKey(true);

var terminal = new Terminal();
terminal.Run();

Console.ReadKey(true);
Console.ResetColor();
Console.Clear();

//dotnet publish -r win-x64 -p:PublishSingleFile=true  src\2024\Terminal\Terminal.csproj
