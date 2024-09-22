using Store;

var terminal = new Terminal();
while (terminal.CanRun)
{
    terminal.Run();
}

Console.ResetColor();
Console.Clear();

//dotnet publish -r win-x64 -p:PublishSingleFile=true  src\2024\Terminal\Terminal.csproj
