# Communication
A simple example of a two-way communication through RabbitMQ.

There are two C# programs demonstrating how RabbitMQ can be used for two processes to share data with each other - the two processes are named Alice and Bob in our example.

1. Install .NET Core SDK from https://dotnet.microsoft.com/download

2. Install Docker from https://www.docker.com/products/docker-desktop

3. Deploy RabbitMQ on Docker
  - start docker
  - go to DockerRabbit directory which contains the configuration files for building the RabbitMQ docker image
  - run the following command to build the docker image
  ```bash
  docker build -t rabbit .
  ```
  - start the image on docker with the following command
  ```bash
  docker run -d --name rabbit-docker -p 15672:15672 -p 5672:5672 rabbit
  ```
  Here we need to publish ports 15672 and 5672 so that RabbitMQ can be accessed from outside of the container.
  
4. Open two terminals in directories Communicate/Bob and Communicate/Alice.
   After the RabbitMQ is initiated and running in docker, use the command 
   ```bash
   dotnet run send.txt receive.txt
   ```
   to start the programs in both terminals. Here "send.txt" is the file containing the data to send and "receive.txt" is the file to save the received data.
   For example, there is already a simple data file Communicate/Alice/AliceData.txt, we can initiate the program at the terminal for Alice using 
   ```bash
   dotnet run AliceData.txt Received.txt"
   ```
   similarly, there is a data file Communicate/Bob/BobData.txt for Bob.
   
5. Presse enter to stop the processes after the data exchange was completed.


Below we discuss the advantages and disadvantages of this demonstration.

Advantages:
- Messages are encrypted before being sent with AES-256 block cipher
- User does not need to install RabbitMQ locally
- User can choose any file to share with each other
- User can specify any file to save the received data to

Disadvantages:
- There is no autentication and integrity check in place
- The system does not deal with data loss during the communication
- There is no key exchange - keys are hardcoded in the programs
- Same keys are used for every run of the program
