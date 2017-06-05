using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;
using PCSC.Iso7816;
using NdefLibrary.Ndef;
using npay.MpesaService;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using npay;
    class Program
    {
        private static readonly IContextFactory _contextFactory = ContextFactory.Instance;
        static Reader reader = null;
      
        /// <summary>
        /// Program entry point 
        /// </summary>
        /// <param name="args"></param>
    
         static void Main(string[] args)
        {
            reader = new Reader();
            try
            {
                using (var monitor = new SCardMonitor(_contextFactory, SCardScope.System))
                {
                    AttachEventsListeners(monitor);
                    monitor.Start(Reader.ReaderName);
                    initiateTransaction();
                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (ExitRequested(key))
                        {
                            break;
                        }
                        if (monitor.Monitoring)
                        {
                            monitor.Cancel();
                            Console.WriteLine("Monitoring paused. (Press CTRL-Q to quit)");
                        }
                        else
                        {
                            monitor.Start(Reader.ReaderName);
                            Console.WriteLine("Monitoring started. (Press CTRL-Q to quit)");
                        }
                    }
                }
                //Create a reader 
                

               
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("Ouch: "
                    + ex.Message
                    + " (" + ex.SCardError.ToString() + ")");
            }
            Console.ReadKey();

            //release the context
            reader.releaseContext();
        }

        private static void AttachEventsListeners(SCardMonitor monitor)
        {
            monitor.CardInserted += monitor_CardInserted;

        }

         static void monitor_CardInserted(object sender, CardStatusEventArgs e)
        {
            initiateTransaction();
        }
        //Called every time the card is inserted 
         public static void initiateTransaction()
         {
             //Read the tag 
             SCardReader scardReader = reader.getReader(reader.getReaderContext(SCardScope.System));
             if (scardReader != null)
             {
                 //Success :: Connect to the reader 
                 scardReader = reader.connect();
                 string phoneNo = ReadTag(scardReader);
                 //Make sure phone no is not empty or null;
                 if (!String.IsNullOrEmpty(phoneNo))
                 {
                     //Call the checkout process
                     Transaction t = new Transaction(300, phoneNo, "DATA Shoes");
                     string respone = t.pay();
                     Console.WriteLine("MPESA Response ::::::::" + respone);

                 }
                 else
                 {
                     Console.WriteLine("No Tag detected, tap your phone again please");
                 }
             }
             else
             {
                 Console.WriteLine("ACR1252U Reader is not connected, connec and try again later");
             }
         }
        /// <summary>
        /// Exit request 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool ExitRequested(ConsoleKeyInfo key)
        {
            return key.Modifiers == ConsoleModifiers.Control
                   && key.Key == ConsoleKey.Q;
        }

        /// <summary>
        /// Checks for errors in the card requests 
        /// </summary>
        /// <param name="err"></param>
        static void CheckErr(SCardError err)
        {
            if (err != SCardError.Success)
                throw new PCSCException(err,
                    SCardHelper.StringifyError(err));
        }
        /// <summary>
        /// Read all the data on the tag and format it as a phone number -  will be used to make the payment 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static string ReadTag(SCardReader reader)
        {
            try
            {
                if (reader != null)
                {
                    if (reader.IsConnected)
                    {
                        // Byte arrays to hold the responses

                        byte[] dataBlock1 = new byte[256];
                        byte[] dataBlock2 = new byte[256];

                        // Transmit the commands 

                        SCardError err1 = reader.Transmit(Command.READ_FIRST_12_BLOCKS_FROM_BLOCK_5, ref dataBlock1);
                        SCardError err2 = reader.Transmit(Command.READNEXT_12BLOCKS_FROM_BLOCK_9, ref dataBlock2);
                        //Check for any errors and thro any exceptions 
                        CheckErr(err1);
                        CheckErr(err2);

                        // Idf no errors then read out the data
                        byte[] readData = Command.formatResponses(dataBlock1, dataBlock2);
                        return NdefHelper.getNdefFromByteArray(readData);

                    }
                    
                }
                else
                {
                    Console.WriteLine("Tag couldn't be read, no reader is available");
                }
            }
            catch (PCSCException ex)
            {
                Console.WriteLine(SCardHelper.StringifyError(ex.SCardError));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

       
    }


