using System.Net.Sockets;

public class ConfigMessage
{

	public const int DATA_SIZE = 4;

//static
	public static int Size
	{
		get
		{
			return MessageHeader.HEADER_SIZE + DATA_SIZE;
		}
	}

//member
	MessageHeader header;

	public byte[] data;

	public int SampleRate
	{
		get
		{
			return (((int)data[1]) << 8) | (data[0]);
		}
		set
		{
			int data_16bit = value & 0xffff;
			data[0] = (byte)(data_16bit & 0xff);
			data[1] = (byte)(data_16bit >> 8);
		}
	}

	public byte ClientId
	{
		get
		{
			return data[3];
		}
		set
		{
			data[3] = value;
		}
	}

	public byte SlicesPerSecond
	{
		get
		{
			return data[3];
		}
		set
		{
			data[3] = value;
		}
	}


	public byte[] GetBytes()
	{
		var output = new byte[DATA_SIZE + MessageHeader.HEADER_SIZE];

		header.GetBytes().CopyTo(output, 0);
		data.CopyTo(output, MessageHeader.HEADER_SIZE);

		return output;
	}

	public void ReadDataFromNetworkStream(NetworkStream networkStream, MessageHeader _header)
	{
		header = new MessageHeader(_header.GetBytes());
		networkStream.Read(data, 0, DATA_SIZE);
	}

	public ConfigMessage()
	{
		header = new MessageHeader(MessageHeader.MessageType.configMessage, byte.MaxValue);
		data = new byte[DATA_SIZE];
	}

	public ConfigMessage(NetworkStream networkStream, MessageHeader _header)
	{
		data = new byte[DATA_SIZE];
		ReadDataFromNetworkStream(networkStream, _header);
	}

}

