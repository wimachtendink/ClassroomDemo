using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
	private static TcpListener listener;
	private static bool listen = true;
			
	public static void Main()
	{
		Client.Init(41, 22050, 8);

		listener = new TcpListener(IPAddress.Any, 8080);
		listener.Start();

		List<Client> deadList = new List<Client>();

		while (listen)
		{
			if (listener.Pending())
			{
				Client.ClientsList.Add(new Client(listener.AcceptTcpClient().GetStream()));
			}

			foreach (Client client in Client.ClientsList)
			{
				if(client.HasMessages)
				{
					client.HandleMessages();
				}
				else if(Environment.TickCount - client.MostRecentMessageTime > 5000)//we haven't heard from them in 5 seconds...
				{
					deadList.Add(client);
					continue;
				}

				if(client.NeedsConfig)
				{
					client.SendMessage_Config();
				}

				if(client.NeedsAudio)
				{
					//calculate audio for this client
					client.SendMessage_Audio();
				}

				if(client.NeedsPoseData)
				{
					client.SendMessage_Pose();//not implemented
				}
			}

			while(deadList.Count > 0)
			{
				deadList[0].Dispose();
				deadList.RemoveAt(0);
			}


			Task.Delay(15).Wait();
		}
	}
}