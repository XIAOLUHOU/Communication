using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.IO;
using System.Security.Cryptography;

class Bob
{
    //AES key for sending data from Alice to Bob
    public static readonly String KEY_A_TO_B = "bfaldieqlgslteldi830alcy2ldiacpq";
    public static readonly String IV_A_TO_B = "yue4950vae92pcid";


    //AES key for sending data from Bob to Alice
    public static readonly String KEY_B_TO_A = "iowalcjd93ifkwlapcu301dlsovp0d63";
    public static readonly String IV_B_TO_A = "02idla84jcye20f3";

    public static void Main(string[] args)
    {
        //default data file to send
        String sendFile = @"BobData.txt";
        //default file for saving data
        String receiveFile = @"Received.txt";

        //take first user input as "sendFile"
        if (args.Length > 0)
        {
            sendFile = args[0];
        }
        //take first user input as "receiveFile"
        if (args.Length > 1)
        {
            receiveFile = args[1];
        }

        Console.WriteLine("Sending data from file {0}", sendFile);
        Console.WriteLine("Saving data to {0}", receiveFile);

        if (!File.Exists(receiveFile))
        {
            System.IO.File.WriteAllText(receiveFile, "");
        }

        //AES key for sending data from Alice to Bob
        byte[] keyAtoB = Encoding.ASCII.GetBytes(KEY_A_TO_B);
        byte[] IVAtoB = Encoding.ASCII.GetBytes(IV_A_TO_B);

        //AES key for sending data from Bob to Alice
        byte[] keyBtoA = Encoding.ASCII.GetBytes(KEY_B_TO_A);
        byte[] IVBtoA = Encoding.ASCII.GetBytes(IV_B_TO_A);

        //send message to Alice
        Thread t1 = new Thread(() =>
        {
            Thread.Sleep(1000);
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                string message;
                System.IO.StreamReader file = new System.IO.StreamReader(@"BobData.txt");
                while ((message = file.ReadLine()) != null)
                {
                    //Encrypt message
                    byte[] body = EncryptStringToBytes(message, keyBtoA, IVBtoA);
                    channel.BasicPublish(exchange: "",
                                         routingKey: "BtoA",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine("Sent {0}", message);
                }

                connection.Close();
            }

        });

        //receiving message from Alice
        Thread t2 = new Thread(() =>
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    //Decrypt the received message
                    String message = DecryptStringFromBytes(body, keyAtoB, IVAtoB);
                    Console.WriteLine("Received {0}", message);
                    //write the received message to "receiveFile"
                    using (var tw = new StreamWriter(receiveFile, true))
                    {
                        tw.WriteLine(message);
                    }
                };
                channel.BasicConsume(queue: "AtoB",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        });




        t1.Start();
        t2.Start();


        //wait for t1 to finish
        t1.Join();

        //wait for t2 to finish
        t2.Join();



    }

    //AES Encryption
    static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments. 
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        // Create an RijndaelManaged object 
        // with the specified key and IV. 
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;

            // Create a encryptor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption. 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream. 
        return encrypted;

    }

    //AES decryption
    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}
