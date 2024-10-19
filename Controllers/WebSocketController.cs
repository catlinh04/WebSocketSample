// using System.Net.WebSockets;
// using System.Text;
// using Microsoft.AspNetCore.Mvc;
// using WebSocketsSample.Snippets;

// namespace WebSocketsSample.Controllers;

// public class WebSocketController : ControllerBase
// {

//     [Route("/ws")]

//     public async Task Get()
//     {
//         if (HttpContext.WebSockets.IsWebSocketRequest)
//         {
//             using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

//             // Thêm WebSocket vào danh sách quản lý
//             WebSocketConnectionManager.AddSocket(webSocket);

//             await ReceiveAndBroadcast(webSocket);
//         }
//         else
//         {
//             HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
//         }
//     }


//     private async Task ReceiveAndBroadcast(WebSocket webSocket)
//     {
//         var buffer = new byte[1024 * 4];
//         var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

//         while (!receiveResult.CloseStatus.HasValue)
//         {
//             // Chuyển tin nhắn thành chuỗi và phát tới tất cả client
//             var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
//             await WebSocketConnectionManager.BroadcastMessage(message);

//             // Đọc tin nhắn tiếp theo
//             receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//         }

//         // Khi client ngắt kết nối
//         await WebSocketConnectionManager.RemoveSocket(webSocket);
//     }
// }
