// Inspiration: https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb
// As Daniel did in his Git Hub we establish a network connection in two steps:
// 1. We set up a background thread to listen on our socket. If the server sends a message as a byte array we decode that to a string (Incoming data).
// 2. We encode a message as a byte array and feed it into the socket connection, and the server can decode it as we did in step 1.   (Outgoing data).

using System;
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;


// Step 1: Handle incoming data
public class server_handler
{

	public FaultHandler faultHandler; 

    public void handle_msg(string msg)
    {
		faultHandler.QueueFix(msg);
		
        Debug.Log("client sent: " + msg);
    }

}

public class SocketServer : MonoBehaviour {  	
	// Step 2. METHODS FOR OUTGOING DATA: They corrospond to the id in the JSON parser. 
	public void break_engine(){
		SendData("0");
	}

	public void break_core(){
		SendData("1");
	}

	public void break_thrusters(){
		SendData("2");
	}

	private TcpListener tcpListener; 
	private Thread serverRecieveThread;  	
	private TcpClient client;
	private int port = 8053;
	private server_handler sh; 	
	

	// Part 3: Networking
	void Start () { 		
		// We start a new background thread that is listening for messages from the Client:
		sh = new server_handler(); 		
		serverRecieveThread = new Thread (new ThreadStart(ListenOnClient)); 		
		serverRecieveThread.IsBackground = true; 		
		serverRecieveThread.Start();
		sh.faultHandler = gameObject.GetComponent<FaultHandler>();
	}

	private void ListenOnClient () {
		// We dont care what ip the client has: 					
		tcpListener = new TcpListener(IPAddress.Any, port); 			
		tcpListener.Start();                           
		
		// Declare a byte array to convert into a str:
		Byte[] recieved_bytes = new Byte[256];  			
		while (true) { 				
			using (client = tcpListener.AcceptTcpClient()) { 					 					
					using (NetworkStream in_stream = client.GetStream()) { 						
						int length = 1;

						// Read server's byte_stream and convert it to a string on our side: 					
						while (true) {
							length = in_stream.Read(recieved_bytes, 0, recieved_bytes.Length);
							if (length == 0)
								break;

							// Decode the bytestream to a string:
							string recMsg = Encoding.ASCII.GetString(recieved_bytes); 											
							sh.handle_msg(recMsg);
						} 					
					} 				
				} 			
			} 				
	}  	
 	
	public void SendData(string msg) { 		
		if (client == null) {             
			//Debug.Log("No connected clients to send to");
			return;         
		}  		
		
		// Get the output stream of the client:
		NetworkStream out_stream = client.GetStream(); 			
		if (out_stream.CanWrite) {                          
				byte[] sendMessage = Encoding.ASCII.GetBytes(msg); 				               
				out_stream.Write(sendMessage, 0, sendMessage.Length);               
				Debug.Log("Server sent his message");           
		}        	
	} 

	// Part 4: Testing
	void Update(){
		if(Input.GetKeyDown("space")){
			break_engine();
			
			//break_core();
			//break_thrusters();
		}
	}

	void  OnApplicationQuit()
    {
		if (CheckConnection())
		{
			SendData("Disconnect");
			client.Close();
		}
    } 

	public bool CheckConnection()
	{
		if (client == null){
			return false;
		}
		else
		{
			return true;
		}
	}
}

