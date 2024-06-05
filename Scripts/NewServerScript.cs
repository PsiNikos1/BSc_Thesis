using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class NewServerScript : MonoBehaviour{

    Thread thread;
    public int connectionPort = 12345;
    TcpListener server;
    TcpClient client;
    bool running;
    Vector3 position ;
    public ForceGenerator forceGenerator;
    private bool startup = true;
    private Vector3 initialPokingDevicePosition;
    private GameObject camera;

    private void Start(){      
        this.forceGenerator = GameObject.Find("whatEverObject").GetComponent<ForceGenerator>();
        position = transform.position;

        //ConnectToServer("192.168.2.8", 12345); // Replace with your server's IP and port
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
        
        this.initialPokingDevicePosition = GameObject.Find("pokingDevice").transform.position;
        this.camera = GameObject.Find("Main Camera");

    }


    private void fixObjectPosition(){
        
        GameObject whatEverObject = GameObject.Find("whatEverObject");
        GameObject pokingDevice = GameObject.Find("pokingDevice");
        GameObject mainCamera = GameObject.Find("Main Camera");

        if (pokingDevice == null)   
        {
            Debug.LogError("GameObject with name 'pokingDecive' not found.");
            return;
        }

        if (whatEverObject == null)
        {
            Debug.LogError("GameObject with name 'whatEverObject' not found.");
            return;
        }
        Vector3 pokingDevicePosition = pokingDevice.transform.position;
        Vector3 startingPosition = new Vector3(
                pokingDevicePosition.x + 0.0f,
                pokingDevicePosition.y + 2.3f,
                pokingDevicePosition.z  );        
        whatEverObject.transform.position = startingPosition;

        this.camera.transform.position = GameObject.Find("pokingDevice").transform.position;

        print("Load completed");
        this.forceGenerator.loadIsDone = true;

    }

    void GetData()    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();


        // Start listening
        running = true;
        while (running)
        {
            Connection();
        }
        server.Stop();
    }

    void Connection()    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        // Decode the bytes into a string
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        string[] stringArray = dataReceived.Split(',');

        
        // Make sure we're not getting an empty string
        //dataReceived.Trim();

        if (dataReceived != null && dataReceived != "")
        {

            List<double> list = this.forceGenerator.getForceAndAngles();
            string responseMessage = list[0] +","+ list[1] +","+ list[2];
            //string responseMessage ="hi";
            //print("send:\n"+responseMessage);


            byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
            //nwStream.Write(responseBytes, 0, responseBytes.Length);

            Socket clientSocket = client.Client;
            clientSocket.Send(responseBytes);

            // Convert the received string of data to the format we are using
            float new_x ;
            float new_y ;
            try{
                
                float.TryParse(stringArray[0], out new_x) ;
                float.TryParse(stringArray[1], out new_y);
                    // Parsing successful, 'result' contains the parsed float value
             
                position = new Vector3(
                                -new_x,
                                -new_y,
                                this.initialPokingDevicePosition.z
                );
            //    position = new Vector3(
            //                     new_y,
            //                     new_x,
            //                     this.initialPokingDevicePosition.z
            //     );


                
            }
            catch (FormatException ex)
            {
                // Handle the exception if the input is not in the correct format
                Console.WriteLine($"Error: {ex.Message}");
                
            }
        
        
        }
    }



   

    void Update()
    {
        // Set this object's position in the scene according to the position received
        //transform.position = transform.position+ new Vector3(0.001f,0f,0f);
        // transform.position = position;
    
        // float yOffset = yOffset + 0.02f ;

        // // Move the object along the Y-axis
        // transform.Translate(0f, yOffset, 0f);
        // float constantVelocityY = 0.02f;
        // Vector3 newPosition = transform.position + Vector3.up * constantVelocityY * Time.deltaTime;
        // transform.position = newPosition;



        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5.0f); //Lerp is for a smoother movement
        if(transform.position != this.initialPokingDevicePosition && this.startup){
            Invoke("fixObjectPosition", 2f);//wait .2 secs

            //fixObjectPosition();
            this.startup = false;
        }

    }

}
