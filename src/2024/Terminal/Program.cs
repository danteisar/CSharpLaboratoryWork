using Store;

#if DEBUG
using QrCodeGenerator;
using System.Text;

QrCode.IsDEmo = false;
var text = "ХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХ";
text = "ХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХ";
text = "https://habr.com/ru/articles/172525/";
Console.WriteLine($"Текст: {text} Размер: {Encoding.UTF8.GetBytes(text).Length * 8}");
var code =new QrCode(text, EncodingMode.Binary, QR.V1, EccLevel.Q, Mask.M5);

//var code =new QrCode(text, EncodingMode.Binary);//, QR.V20, EccLevel.Q, Mask.M3); //
Console.WriteLine(code);
Console.WriteLine($"{code.Version} {code.EncodingMode} {code.CorrectionLevel} {code.Mask}"); 
Console.WriteLine("Press any key to continue...");
Console.ReadKey(true);
#endif
var terminal = new Terminal();
terminal.Run();

Console.ReadKey(true);
Console.ResetColor();
Console.Clear();

//dotnet publish -r win-x64 -p:PublishSingleFile=true  src\2024\Terminal\Terminal.csproj
