using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using FootballPlayerTest;

namespace TCP__server
{
	class Program
	{
		private static readonly List<FootballPlayer> playerLists = new List<FootballPlayer>
		{
			new FootballPlayer {ID = 1, Name = "Tobias", Price = 6963969, shirtNumber = 13},
			new FootballPlayer {ID = 2, Name = "Freja", Price = 202020, shirtNumber = 20}
		};
		static void Main(string[] args)
		{
			Console.WriteLine("TCP Server for FootballPlayers is now Active!");

			TcpListener listener = new TcpListener(IPAddress.Any, 2121);
			listener.Start();

			while (true)
			{
				TcpClient socket = listener.AcceptTcpClient();
				Console.WriteLine("New client");

				Task.Run(() =>
				{
					HandleClient(socket);
				});

			}
		}
		private static void HandleClient(TcpClient socket)
		{
			NetworkStream ns = socket.GetStream();
			StreamReader reader = new StreamReader(ns);
			StreamWriter writer = new StreamWriter(ns);

			while (true)
			{
				writer.WriteLine("Choose between GetAll, get or save !");
				writer.Flush();
				string message = reader.ReadLine();
				writer.WriteLine("Write the ID of the player you want <3");
				writer.Flush();
				string messageID = reader.ReadLine();

				if (message.ToLower().StartsWith("GetAll"))
				{
					if (playerLists != null)
					{
						foreach (var players in playerLists)
						{
							writer.WriteLine($"Name: {players.Name}, Price: {players.Price}, shirtnumber: {players.shirtNumber}");
							writer.Flush();
						}
					}
				}
				else if (message.ToLower() == "get")
				{
					int id = -1;
					if (int.TryParse(messageID, out id))
					{
						foreach (var players in playerLists)
						{
							if (players.ID == id)
							{
								writer.WriteLine($"ID: {players.ID}, Name: {players.Name}, Price: {players.Price}, shirtnumber: {players.shirtNumber}");
								writer.Flush();
							}
						}
					}
				}
				else if (message.ToLower().StartsWith("save"))
				{
					FootballPlayer player = new FootballPlayer();
					playerLists.Add(player);
					player = JsonSerializer.Deserialize<FootballPlayer>(messageID);

					writer.WriteLine("Player is now saved");
					writer.Flush();
				}
				socket.Close();
			}
		}
	}
}