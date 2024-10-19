using System.Net.WebSockets;
using System.Text;
using WebSocketsSample.Models;

namespace WebSocketsSample.Snippets
{
    public static class WebSocketConnectionManager
    {
        // Nhận và xử lý tin nhắn từ client
        public static async Task ReceiveMessagesAsync(WebSocket webSocket, string uuid, Dictionary<string, WebSocket> connections, Dictionary<string, Users> users)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message from {users[uuid].Username}: {message}");

                    // Cập nhật trạng thái người dùng
                    users[uuid].State = message;

                    // Gửi broadcast tin nhắn mới
                    await Broadcast(connections, users);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the client", CancellationToken.None);
                }
            }
        }

        // Phát tin nhắn tới tất cả các client đang kết nối
        public static async Task Broadcast(Dictionary<string, WebSocket> connections, Dictionary<string, Users> users)
        {
            var userJson = System.Text.Json.JsonSerializer.Serialize(users);

            foreach (var socket in connections.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(userJson);
                    await socket.SendAsync(new ArraySegment<byte>(messageBytes, 0, messageBytes.Length),
                                           WebSocketMessageType.Text,
                                           true,
                                           CancellationToken.None);
                }
            }
        }


    }
}