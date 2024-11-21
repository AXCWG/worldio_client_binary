using System.Net.WebSockets;
using System.Text.Json;
using Websocket.Client;

namespace worldio_client_binary;

class Send
{
    
    public string? username;
    public string? msg;
}

class Program
{
    static void SetUsername()
    {
        try
        {
            Console.WriteLine("Please enter your username:");
            start:
            username = Console.ReadLine()!;
            if (username.Length == 0)
            {
                Console.WriteLine("May not be null or empty. Try again.");
                goto start;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed for: {exception.Message}");
        }
        Console.WriteLine($"Successfully changed your username to: {username}");
        
    }

    private static string username = "Anonymous";

    static async Task<int> Main(string[] args)
    {
        try
        {
            username = File.ReadAllText("./username.txt");

        }
        catch (FileNotFoundException e)
        {
            
        }
        Console.TreatControlCAsInput = true;
        Console.Clear();
        Console.WriteLine("Welcome to World IO v1.0.0_C#!\n" +
                          "欢迎来到 世界IO 1.0.0_C#\n" +
                          "使用\\n以换行 :q以退出 :uset以设置用户名 | Use \\n to return, :q to quit, :uset to set username.  \n" +
                          "不消考虑任何规则：发就完事了 \uff5c No need to consider boundaries: just SEND IT!!!!!\n" +
                          "历史记录：|History: \n" +
                          "");
        var client = new WebsocketClient(new Uri("wss://andyxie.cn:8181"));
        client.MessageReceived.Subscribe(msg => Console.WriteLine($"{msg.Text}"));
        await client.Start();
        Thread task = new Thread((() =>
        {
            while (true)
            {
                var input = Console.ReadLine()!;
                if (input == ":q")
                {
                    File.AppendAllText("./username.txt", username);
                    Environment.Exit(0);
                }
                else if (input == ":uset")
                {
                    SetUsername();
                }
                else
                {
                    Send send = new Send()
                    {
                        username = username, msg = input.Replace("\\n", "\n")
                    };
                    var message = JsonSerializer
                        .Serialize<Send>(send, new JsonSerializerOptions() { IncludeFields = true });
                    client.Send(message);
                }
            }
        }));
        task.Start();

        return 0;
    }
}