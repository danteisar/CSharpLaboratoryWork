using Store;

#if DEBUG
using QrCodeGenerator;
QrCode.IsDEmo = false;//
//var text = "ХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХХ";
//var text = "https://habr.com/ru/articles/172525/";
var text = "Съешь ещё этих мягких французских булок, да выпей же чаю";
var code =new QrCode(text, EncodingMode.Binary, EccLevel.Q, QR.V6, Mask.M3);//, QR.V20, EccLevel.Q, Mask.M3); //
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