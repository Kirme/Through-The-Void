# Through-The-Void
AGI22

## Network Configuration
In order for networking to work the AR player needs an empty object with the client script and the VR player needs the server script attached to an empty object.
In order to connect through LAN we need to think about three things:

1. Same internet: Make sure that the devices are on the same internet, of course.

2. Give the client the IP: Before building to AR we need to configure the IP on the client side to match the IP of the host. We can do this by running ipconfig on the server side and then look at the IPv4 address of the server computer, on the internet that both devices are connected to (Every device has a unique IPv4 address). In the code section where we create a TCP client to listen to a socket (an IP and a port) we should enter in the server's IPv4 address and that should work. We dont have to specify any ip on the server side.

3. Handle Windows Firewall: We need to disable the windows firewall on the server side (or insert an exception). On the client side we don't need to do anything.

Testing: If we want to test everything out with both client and server we don't have to anything other than setting  the TCP client to listen to "localhost". 
