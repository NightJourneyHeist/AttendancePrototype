using System.IO.Ports;

// both are now in the same baudRate and port
// this will ensure that we ONLY use this
using SerialPort trafficPort = new ("/dev/ttyACM0", 115200);

/// <summary>
/// this is a mini console application that will read data from Bailey (aka Esp32-C3) and unto a text file on my laptop
/// the reason why is that currently, any information that I write on Bailey is stored in their small memory and I don't have a way to see it
/// beusides I want to be able to use that information to update the database later, so I kinda have to pull it somehow lol
/// </summary>


Console.WriteLine("Hello, World!");


trafficPort.Open();
Console.WriteLine("Bailey Connected!");

// tell Bailey to get the current logs
trafficPort.WriteLine("GET_LOGS");

// tell Bailey to clear the logs and reset
//trafficPort.WriteLine("CLEAR_LOGS");

// empty container to store stuff from Bailey
string logs = "";

// read limit so it doesn't keep going forever
trafficPort.ReadTimeout = 10000;
try
{
    // keep reading till EOF
    while(true)
    {
        // get data from bailey, one byte at a time
        string response = trafficPort.ReadLine().Trim();

        // ignore any input before the marker
        if(response == "LOG_START")
        {
            continue;
        }
        // stop reading when EOF is reached
        else if(response == "LOG_END")
        {
            break;
        }
        // add it to the list of stuff to post
        logs +=  response + Environment.NewLine;
    }
    
}
catch (TimeoutException)
{
    Console.WriteLine("Bailey didn't finish sending the logs!");
}

// write the stuff that you got so far
File.WriteAllText("BeilyLogs.txt", logs);