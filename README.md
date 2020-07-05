# Communication
A simple example of communication through RabbitMQ

This a C# program demonstrating how RabbitMQ can be used for two processes to share data with each other - the two processes are named Alice and Bob in our example.

1. Install .NET Core runtime from https://dotnet.microsoft.com/download

2. Deploy RabbitMQ on Docker
  - download docker image rabbit.tar
  - in the same directory as rabbit.tar
  - run the following command to load the image to docker
  ```bash
  docker load --input rabbit.tar
  ```
  - start rabbit on docker with the following command
  ```bash
  docker run -d --name rabbit-docker -p 15672:15672 -p 5672:5672 rabbit
  ```
  Here we need to publish ports 15672 and 5672 so that RabbitMQ can be accessed from outside of the container.
  
3. Open two terminals in directories Communicate/Bob and Communicate/Alice respectively.
   Use the command "dotnet run A.txt B.txt" to start the programs in both terminals, where A.txt is the file containing the data to send and B.txt is the file to save received data.
   For example, there is a simple data file Communicate/Alice/AliceData.txt, we can initiate the program at the terminal for Alice using "dotnet run AliceData.txt Received.txt".
   
4. Presse enter to stop the processes.


Here we discuss the advantages and disadvantages of the demonstration.

Advantages:
- User can choose any file to share with each other
- User can specify any file to save the received data

Disadvantages:
- There is no autentication and integrity check in place
- The system does not deal with data loss during the communication
