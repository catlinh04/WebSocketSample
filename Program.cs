using System.Net.WebSockets;
using WebSocketsSample.Models;
using WebSocketsSample.Snippets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();
app.Urls.Add("http://localhost:8080");
// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
    //định nghĩa rằng cứ mỗi 2 phút, một tin nhắn "ping" 
    //sẽ được gửi để duy trì kết nối, đảm bảo rằng kết nối không bị đóng do không hoạt động trong thời gian dài.
};
// Danh sách lưu trữ kết nối WebSocket và thông tin người dùng
var connections = new Dictionary<string, WebSocket>();
var users = new Dictionary<string, Users>();
app.UseWebSockets();//use middleware để nhận ws tự động 
                    // </snippet_UseWebSockets>
                    // Map đường dẫn cho WebSocket
app.Map("/ws", async (context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var username = context.Request.Query["username"];
        if (string.IsNullOrEmpty(username))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Missing username");
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var uuid = Guid.NewGuid().ToString();

        Console.WriteLine($"User {username} connected with ID {uuid}");

        // Lưu kết nối và thông tin người dùng
        connections[uuid] = webSocket;
        users[uuid] = new Users { Username = username, State = "{}" };

        // Gửi broadcast khi có kết nối mới
        await WebSocketConnectionManager.Broadcast(connections, users);

        // Xử lý tin nhắn từ client
        await WebSocketConnectionManager.ReceiveMessagesAsync(webSocket, uuid, connections, users);

        // Xử lý đóng kết nối
        Console.WriteLine($"User {users[uuid].Username} disconnected");
        connections.Remove(uuid);
        users.Remove(uuid);
        await WebSocketConnectionManager.Broadcast(connections, users);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
