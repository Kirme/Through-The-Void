// Inspiration: https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb
// As Daniel did in his Git Hub we establish a network connection in two steps:
// 1. We set up a background thread to listen on our socket. If the server sends a message as a byte array we decode that to a string (Incoming data).
// 2. We encode a message as a byte array and feed it into the socket connection, and the server can decode it as we did in step 1.   (Outgoing data).

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// Documentation overview: 
// We handle client-networking in three steps, where step 1 is the only one relevant for anything else other than pure networking.

// Step 1: HANDLING INCOMING DATA (Interesting for the AR role):
// In this step the data that the server sends is fed into a client_handler object which handles all the messages that can be sent by the server.
// The AR player can insert whatever functionality they want triggered on their end here:

// The Main class:
public class Client : MonoBehaviour {
    // Networking data:
	private int port = 8052;
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
	private FaultHandler faultHandler;
		
	void Start () {
		ConnectToHost();
		faultHandler = GetComponent<FaultHandler>();

		// Debug
		//StartCoroutine(Crt());
		//faultHandler.ReceiveMessage("0", "0");
	}

	IEnumerator Crt() {
		yield return new WaitForSeconds(1);
		faultHandler.ReceiveMessage("0", "1");
		
		yield return new WaitForSeconds(1);
		faultHandler.ReceiveMessage("2", "0");

		//yield return new WaitForSeconds(3);
		//faultHandler.ReceiveMessage("clear");
	}

	public void Reconnect() {
		Disconnect();
		ConnectToHost();
	}

	public void Disconnect() {
		clientReceiveThread.Abort();

		if (socketConnection != null)
			socketConnection.Close();
	}

	private void ConnectToHost () {
		// We start a backround thread to listen for incoming requests from the host:
		clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
		clientReceiveThread.IsBackground = true; 			
		clientReceiveThread.Start();  		
	}

	private void ListenForData() { 				
			// Set to IPv4 address for LAN:
			socketConnection = new TcpClient("193.10.37.246", port);  			
			Byte[] bytes = new Byte[256];             
			
			while (true) { 							
				using (NetworkStream stream = socketConnection.GetStream()) {
					// Read server's byte_stream and convert it to a string on our side: 					
					while (true) {
						int length = stream.Read(bytes, 0, bytes.Length);
						if (length == 0)
							break;

						byte[] incomingData = new byte[length]; 						
						Array.Copy(bytes, 0, incomingData, 0, length); 						 						
						string serverMessage = Encoding.ASCII.GetString(incomingData);

						// Message: "reset"
						if (string.Compare(serverMessage, "reset") == 0)
							faultHandler.ReceiveMessage(serverMessage);

						// Message: "id variation"
						string[] vals = serverMessage.Split(' ');

						if (vals.Length >= 2)
							faultHandler.ReceiveMessage(vals[0], vals[1]);
					} 				
				} 			
			}              
	}  	

	// SendMessage
	// Takes in a string and converts the string to a byte array which can be sent through the stream to the host:
	public void SendMessage(string msg) {
		Debug.Log(msg + " " + Encoding.ASCII.GetBytes(msg).Length);

		if (socketConnection == null) {             
			return;         
		}
		
		NetworkStream out_stream = socketConnection.GetStream(); 			
		if (out_stream.CanWrite) {
				byte[] byte_arr = Encoding.ASCII.GetBytes(msg);                                 
				out_stream.Write(byte_arr, 0, byte_arr.Length);                              
		}             		   
	}

	// OnApplicationQuit
	// When we are done playing we abort the thread and close down the network socket:
	void OnApplicationQuit() {
		Disconnect();
    } 
}