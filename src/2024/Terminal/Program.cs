using Store;
using System.Text;


#if DEBUG
using QrCodeGenerator; 
//
QrCode.IsDemo = false;
// QrCode.IsInvert = true;
// QrCode.IsUtf8 = false;
// Console.OutputEncoding = Encoding.UTF8;
// Console.BackgroundColor = ConsoleColor.White;
// Console.ForegroundColor = ConsoleColor.Black;

var sb = new StringBuilder();
while (sb.Length < 1140) //2956 / 2 - 2)
 sb.Append('А');
//var text = sb.ToString();
//var text = "https://habr.com/ru/articles/172525/";
var text = "Съешь ещё этих мягких французских булок, да выпей же чаю12345";
var code =new QrCode(text, EncodingMode.Binary, EccLevel.M, QR.V7, Mask.M111);
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