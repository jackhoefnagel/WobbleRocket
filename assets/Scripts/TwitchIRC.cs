using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AsyncOperation = System.ComponentModel.AsyncOperation;


namespace TechLifeForum
{
	[System.Serializable]
	public class TwitchIRC : MonoBehaviour
	{
		#region Private Variables
		
		private IrcClient jtvClient;    // probablly could have named this better
		
		// this handles the cross-thread stuff (beautifully)
		// so the actual application doesn't have to
		private AsyncOperation op;
		
		// this keeps the users in a list so
		// we don't have to rely on updating
		// it via IRC
		private List<string> userList;
		
		#endregion
		
		#region Properties
		
		public string _Username;
		public string Username { get { return _Username; } }
		
		public string _Password;
		public string Password { get { return _Password; } }
		
		public string _Server = "irc.justin.tv";
		public string Server
		{
			get { return _Server; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					_Server = value;
			}
		}
		#endregion
		
		#region Events
		
		public event ExceptionThrownEventDelegate ExceptionThrown;
		public event MessageReceivedDelegate MessageReceived;
		public event UpdateUsersDelegate UpdateUsers;
		public event ConnectedEventDelegate OnConnected;
		#endregion
		
		#region Constructors
		
		public void Start()
		{




			if (string.IsNullOrEmpty(_Username) || string.IsNullOrEmpty(_Password))
				throw new Exception("Invalid Username or Password"); // not thrown async so they can catch it w/ try/catch
			
			//_Username = Username;
			//_Password = Password;
			
			
			// instantiate variables for awsomeness
			op = AsyncOperationManager.CreateOperation(null);
			userList = new List<string>();

			Connect();
		}
		
		#endregion
		
		#region Public Methods
		
		public void Connect()
		{
			jtvClient = new IrcClient(_Server);
			jtvClient.ServerPass = _Password;
			jtvClient.Nick = _Username;
			jtvClient.AltNick = _Username;
			
			// add listeners before we connect
			AddListeners();
			
			Thread t = new Thread(DoConnect);
			t.IsBackground = true;
			t.Start();
			//DoConnect(); // debug only, comment out t.Start() if you uncomment this
			
		}

		void OnApplicationQuit() {
			Disconnect();
		}


		public void Disconnect()
		{
			jtvClient.Disconnect();
			jtvClient = null;
		}
		
		/// <summary>
		/// Sends a message to the Justin.Tv server
		/// </summary>
		/// <param name="Message">Message to send the server</param>
		public void Send(string Message)
		{
			jtvClient.SendMessage("#" + _Username, Message);
		}
		
		#endregion
		
		#region Private Methods
		
		/// <summary>
		/// Method that is called on a new thread to
		/// avoid tieing up the UI thread
		/// </summary>
		private void DoConnect()
		{
			try
			{
				jtvClient.Connect();
				jtvClient.JoinChannel(TwitchChannel.instance.channelName);
			}
			catch (Exception ex)
			{
				op.Post(x => OnExceptionThrown((Exception)x), ex);  // TODO: double check that exceptions
			}                                                       //       actually show up at this level
		}
		
		/// <summary>
		/// Method to add the required listeners
		/// </summary>
		private void AddListeners()
		{
			jtvClient.OnConnect += () =>
			{
				jtvClient.JoinChannel(TwitchChannel.instance.channelName);
				op.Post(x => OnConnectedFire(), null);
			};
			// listen for messages on the channel
			// we don't care about which channel it comes from
			// since we are only listening to our room
			jtvClient.ChannelMessage += (channel, user, message) =>
			{
				op.Post(x => OnMessageReceived((ChannelMessage)x), new ChannelMessage(user, message));
			};
			jtvClient.UpdateUsers += (channel, users) =>
			{
				userList.Clear();
				userList.AddRange(users);
				userList.Sort();
				op.Post(x => OnUsersUpdate((List<string>)x), userList);
			};
			jtvClient.UserJoined += (channel, user) =>
			{
				if (userList.Contains(user)) return;
				userList.Add(user);
				userList.Sort();
				op.Post(x => OnUsersUpdate((List<string>)x), userList);
			};
			jtvClient.UserLeft += (channel, user) =>
			{
				if (!userList.Contains(user)) return;
				userList.Remove(user);
				userList.Sort();
				op.Post(x => OnUsersUpdate((List<string>)x), userList);
			};
		}
		
		/// <summary>
		/// Triggers the JtvClient Event (UpdateUsers)
		/// </summary>
		/// <param name="users">A list of users</param>
		private void OnUsersUpdate(List<string> users)
		{
			//
			// op.Post(x => OnUsersUpdate((string[])x), users); <- code to trigger this event
			//
			
			// the actual event takes a string array.
			if (UpdateUsers != null) UpdateUsers(users.ToArray());
		}
		private void OnMessageReceived(ChannelMessage channelMessage)
		{
			//
			// op.Post(x => OnMessageReceived((ChannelMessage)x), channelMessage); <- code to trigger this event
			//
			
			if (MessageReceived != null) MessageReceived(channelMessage.From, channelMessage.Message);
		}
		private void OnConnectedFire()
		{
			if (OnConnected != null) OnConnected();
		}
		
		/// <summary>
		/// Used to raise an exception event to the user. These are done asyncronously.
		/// </summary>
		/// <param name="e"></param>
		private void OnExceptionThrown(Exception e)
		{
			//
			// op.Post(x => OnExceptionThrown((Exception)x),e); <- code to throw the exception
			//
			if (ExceptionThrown != null) ExceptionThrown(e);
		}
		
		#endregion
		
		/// <summary>
		/// Stuct to contain a channel message object. 
		/// Needed it to be a single object so this worked great
		/// </summary>
		public struct ChannelMessage
		{
			public string From;
			public string Message;
			public ChannelMessage(string From, string Message)
			{
				this.From = From;
				this.Message = Message;
	
				Modifiers.instance.CheckIfModifierTrigger(Message);

			}
		}
	}
}
