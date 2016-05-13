/* ***********************************
* Created by KoBE at TechLifeForum
* http://tech.reboot.pro
*************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using UnityEngine;
using Random = System.Random ;

using Debug = UnityEngine.Debug;

namespace TechLifeForum
{
	/// <summary>
	/// IRC Client class written at http://tech.reboot.pro
	/// </summary>
	public class IrcClient
	{
		#region Variables
		// default server
		private string _server = "irc.twitch.tv";
		
		// default port
		private int _port = 6667;
		
		private string _ServerPass = "oauth:hmfm3lgkrndfytq44koqcghrztz1dg";
		
		// default nick
		private string _nick = "waarboks";
		
		// default alternate nick
		private string _altNick = "waarboks";
		
		// private TcpClient used to talk to the server
		private TcpClient irc;
		
		// private network stream used to read/write from/to
		private NetworkStream stream;
		
		// global variable used to read input from the client
		private string inputLine;
		
		// stream reader to read from the network stream
		private StreamReader reader;
		
		// stream writer to write to the stream
		private StreamWriter writer;
		
		#endregion
		
		#region Constructors
		/// <summary>
		/// IrcClient used to connect to an IRC Server
		/// </summary>
		/// <param name="Server">IRC Server</param>
		/// <param name="Port">IRC Port (6667 if you are unsure)</param>
		public IrcClient(string Server, int Port)
		{
			_server = Server;
			_port = Port;
		}
		public IrcClient(string Server)
		{
			_server = Server;
		}
		#endregion
		
		#region Properties
		/// <summary>
		/// Returns the Server address used
		/// </summary>
		public string Server
		{
			get { return _server; }
		}
		/// <summary>
		/// Returns the Port used
		/// </summary>
		public int Port
		{
			get { return _port; }
		}
		/// <summary>
		/// Returns the password used to auth to the server
		/// </summary>
		public string ServerPass
		{
			get { return _ServerPass; }
			set { _ServerPass = value; }
		}
		/// <summary>
		/// Returns the current nick being used.
		/// </summary>
		public string Nick
		{
			get { return _nick; }
			set { _nick = value; }
		}
		/// <summary>
		/// Returns the alternate nick being used
		/// </summary>
		public string AltNick
		{
			get { return _altNick; }
			set { _altNick = value; }
		}
		/// <summary>
		/// Returns true if the client is connected.
		/// </summary>
		public bool Connected
		{
			get
			{
				if (irc != null)
					if (irc.Connected)
						return true;
				return false;
			}
		}
		#endregion
		
		#region Events
		
		public event UpdateUserListEventDelegate UpdateUsers;
		public event UserJoinedEventDelegate UserJoined;
		public event UserLeftEventDelegate UserLeft;
		public event UserNickChangeEventDelegate UserNickChange;
		
		public event ChannelMesssageEventDelegate ChannelMessage;
		public event NoticeMessageEventDelegate NoticeMessage;
		public event PrivateMessageEventDelegate PrivateMessage;
		public event ServerMessageEventDelegate ServerMessage;
		
		public event NickTakenEventDelegate NickTaken;
		
		public event ConnectedEventDelegate OnConnect;
		public event DisconnectedEventDelegate Disconnected;
		#endregion
		
		#region PublicMethods
		/// <summary>
		/// Connect to the IRC server
		/// </summary>
		public void Connect()
		{
			
			irc = new TcpClient(_server, _port);
			stream = irc.GetStream();
			reader = new StreamReader(stream);
			writer = new StreamWriter(stream);
			
			//if (!string.IsNullOrEmpty(_ServerPass))
			Send("PASS " + _ServerPass);
			
			Send("NICK " + _nick);
			Send("USER " + _nick + " 0 * :" + _nick);
			
			Listen();
		}
		/// <summary>
		/// Disconnect from the IRC server
		/// </summary>
		public void Disconnect()
		{
			if (irc != null)
			{
				if (irc.Connected)
				{
					Send("QUIT Client Disconnected: http://tech.reboot.pro");
				}
				irc = null;
				if (Disconnected != null)  // TODO: shouldn't need this event,
					Disconnected();        //       it's only fired her
			}
		}
		/// <summary>
		/// Sends the JOIN command to the server
		/// </summary>
		/// <param name="Channel">Channel to join</param>
		public void JoinChannel(string Channel)
		{
			if (irc != null && irc.Connected)
			{
				Debug.Log("join: " + Channel);
				Send("JOIN " + Channel);
			}
		}
		/// <summary>
		/// Sends the PART command for a given channel
		/// </summary>
		/// <param name="Channel">Channel to leave</param>
		public void PartChannel(string Channel)
		{
			Send("PART " + Channel);
		}
		/// <summary>
		/// Send a notice to a user
		/// </summary>
		/// <param name="Nick">User to send the notice to</param>
		/// <param name="message">The message to send</param>
		public void SendNotice(string Nick, string message)
		{
			Send("NOTICE " + Nick + " :" + message);
		}
		/// <summary>
		/// Send a message to the channel
		/// </summary>
		/// <param name="message">Message to send</param>
		public void SendMessage(string Channel, string Message)
		{
			Send("PRIVMSG " + Channel + " :" + Message);
		}
		/// <summary>
		/// Send RAW IRC commands
		/// </summary>
		/// <param name="message"></param>
		public void SendRAW(string message)
		{
			Send(message);
		}
		#endregion
		
		#region PrivateMethods
		/// <summary>
		/// Listens for messages from the server
		/// </summary>
		private void Listen()
		{

			


			
			while ((inputLine = reader.ReadLine()) != null)
			{
				ParseData(inputLine);
				Console.Write(inputLine);
				//Debug.Log(inputLine);
				//break;
			}//end while

			//JoinChannel("#nl_kripp");

		}
		/// <summary>
		/// Parses data sent from the server
		/// </summary>
		/// <param name="data">message from the server</param>
		private void ParseData(string data)
		{
			// split the data into parts
			string[] ircData = data.Split(' ');
			
			// if the message starts with PING we must PONG back
			if (data.Length > 4)
			{
				if (data.Substring(0, 4) == "PING")
				{
					Send("PONG " + ircData[1]);
					return;
				}
				
			}
			
			// re-act according to the IRC Commands
			switch (ircData[1])
			{
			case "001": // server welcome message, after this we can join
				Send("MODE " + _nick + " +B");
				if(OnConnect != null) OnConnect();
				break;
			case "353": // member list
				if (UpdateUsers != null)
					UpdateUsers(ircData[4], JoinArray(ircData, 5).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
				break;
			case "433":
				if (NickTaken != null)
				{
					NickTaken(ircData[3]);
				}
				
				if (ircData[3] == _altNick)
				{
					Random rand = new Random();
					string randomNick = "Guest" + rand.Next(0, 9) + rand.Next(0, 9) + rand.Next(0, 9);
					Send("NICK " + randomNick);
					Send("USER " + randomNick + " 0 * :" + randomNick);
					_nick = randomNick;
				}
				else
				{
					Send("NICK " + _altNick);
					Send("USER " + _altNick + " 0 * :" + _altNick);
					_nick = _altNick;
				}
				break;
			case "JOIN": // someone joined
				if (UserJoined != null)
					UserJoined(ircData[2], ircData[0].Substring(1, ircData[0].IndexOf("!") - 1));
				Send("NAMES " + ircData[2]);
				break;
			case "NICK": // someone changed their nick
				if (UserNickChange != null)
					UserNickChange(ircData[0].Substring(1, ircData[0].IndexOf("!") - 1), JoinArray(ircData, 3));
				break;
			case "NOTICE": // someone sent a notice
				if (NoticeMessage != null)
					if (ircData[0].Contains("!"))
				{
					NoticeMessage(ircData[0].Substring(1, ircData[0].IndexOf('!') - 1),
					              JoinArray(ircData, 3));
				}
				else
					NoticeMessage(_server, JoinArray(ircData, 3));
				break;
			case "PRIVMSG": // message was sent to the channel or as private
				// if it's a private message
				if (ircData[2].ToLower() == _nick.ToLower())
				{
					if (PrivateMessage != null)
					{
						PrivateMessage(ircData[0].Substring(1, ircData[0].IndexOf('!') - 1),
						               JoinArray(ircData, 3));
					}
				}
				else
				{
					if (ChannelMessage != null)
					{
						ChannelMessage(ircData[2], ircData[0].Substring(1, ircData[0].IndexOf('!') - 1),
						               JoinArray(ircData, 3));
					}
				}
				break;
			case "PART":
			case "QUIT":// someone left
				if (UserLeft != null)
					UserLeft(ircData[2], ircData[0].Substring(1, data.IndexOf("!") - 1));
				Send("NAMES " + ircData[2]);
				break;
			default:
				// still using this while debugging
				if (ServerMessage != null)
				{
					if (ircData.Length > 3)
						ServerMessage(JoinArray(ircData, 3));
				}
				break;
			}
			
		}
		/// <summary>
		/// Strips the message of unnecessary characters
		/// </summary>
		/// <param name="message">Message to strip</param>
		/// <returns>Stripped message</returns>
		private string StripMessage(string message)
		{
			// remove IRC Color Codes
			foreach (Match m in new Regex((char)3 + @"(?:\d{1,2}(?:,\d{1,2})?)?").Matches(message))
				message = message.Replace(m.Value, "");
			
			// if there is nothing to strip
			if (message == "")
				return "";
			else if (message.Substring(0, 1) == ":" && message.Length > 2)
				return message.Substring(1, message.Length - 1);
			else
				return message;
		}
		/// <summary>
		/// Joins the array into a string after a specific index
		/// </summary>
		/// <param name="strArray">Array of strings</param>
		/// <param name="startIndex">Starting index</param>
		/// <returns>String</returns>
		private string JoinArray(string[] strArray, int startIndex)
		{
			return StripMessage(String.Join(" ", strArray, startIndex, strArray.Length - startIndex));
		}
		/// <summary>
		/// Send message to server
		/// </summary>
		/// <param name="message">Message to send</param>
		private void Send(string message)
		{
			writer.WriteLine(message);
			writer.Flush();
		}
		#endregion
		
	}
	
}