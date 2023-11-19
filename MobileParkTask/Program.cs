using MobileParkTask;

const int averageCount = 3;
var listener = new TcpListenerWrapper(args[0], int.Parse(args[1]), averageCount);

Console.WriteLine("Enter the command (start, stop, info, statistics)");
while (true)
{
    var command = Console.ReadLine()?.ToLower();
    switch (command)
    {
        case "start":
            _ = listener.StartAsync();
            break;
        case "stop":
            listener.Stop();
            break;
        case "info":
            listener.GetInfo();
            break;
        case "statistics":
            listener.GetStatus();
            break;
        default:
            Console.WriteLine("Command do not exist!");
            break;
    }
}