# Communication
A simple example of communication through RabbitMQ

This a C# program demonstrating how RabbitMQ can be used for two processes to share data with each other - the two processes are named Alice and Bob in our example.

1. Install RabbitMQ from https://www.rabbitmq.com/#getstarted

2. Install .NET Core runtime from https://dotnet.microsoft.com/download

3. Download "definitions.json" from this repository. Create/Open rabbitmq.conf file, put the following line into rabbitmq.conf
"management.load_definitions = "Path to your saved definitions.json file"
  - the purpose of definitions.json file is to create two queues for communincation between Alice and Bob before we start the programs. So that we will not run into the problem when the process tries to access a queue which does not exist.

4. Open two terminals in directories Communicate/Bob and Communicate/Alice respectively.
   Use the command "dotnet run A.txt B.txt" to start the programs in both terminals, where A.txt is the file containing the data to send and B.txt is the file to save received data.
   For example, there is a simple data file Communicate/Alice/AliceData.txt, we can initiate the program at the terminal for Alice using "dotnet run AliceData.txt Received.txt".
   
5. Presse enter to stop the processes.


Here we discuss the advantages and disadvantages of the demonstration.

Advantages:
- User can choose any file to share with each other
- User can specify any file to save the received data

Disadvantages:
- There is no autentication and integrity check in place
- The system does not deal with data loss during the communication
