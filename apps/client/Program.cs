using System.Text;
using TarkovWYA.Client;

// Windows 主控台的預設字碼頁不一定支援繁體中文，改用 UTF-8 避免顯示成亂碼
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(StartupMessage.Text);
