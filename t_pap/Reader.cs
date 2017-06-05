using PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npay
{
    /// <summary>
    /// Contains all methods needed to create a new reader and allow you to connect to it before beginning transactions
    /// </summary>
    public class Reader
    {
        public  static string ReaderName { get{ return "ACS ACR1252 1S CL Reader PICC 0";} }
         SCardReader ACR1252U_Reader { get; set; }
         SCardContext HContext { get; set; }
        /// <summary>
        /// Get the reader context 
        /// </summary>
        /// <param name="scope"></param>
        /// <returns>Scope of the reader </returns>
        public SCardContext getReaderContext(SCardScope scope)
        {
            HContext = new SCardContext();
            if (scope != null)
            {
                HContext.Establish(scope);
                return HContext;
            }
            return null;
        }

        public void releaseContext()
        {
            if (HContext != null)
            {
                HContext.Release();
            }
        }

        /// <summary>
        /// Create a new instance of a reader, it has to be ACR1252U otherwise reject it 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>New instance of a reader that is not connected to yet, to connect call Reader.Connect(readername)</returns>
        public SCardReader getReader(SCardContext context)
        {
            string [] readers = context.GetReaders();
            if (readers.Length > 0)
            {
                for (int i = 0; i < readers.Length; i++)
                {
                    if (readers[i] == ReaderName)
                    {
                        if(context!=null)
                            ACR1252U_Reader = new SCardReader(context);
                        return ACR1252U_Reader;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Connects to the reader with a given reader name 
        /// </summary>
        /// <param name="reader">Reader name got from ACR1252U_Reader.ReaderName</param>
        /// <returns>A reader that has a real connection</returns>
        public SCardReader connect()
        {
            if (ACR1252U_Reader != null)
            {
                ACR1252U_Reader.Connect(ReaderName, SCardShareMode.Shared, SCardProtocol.T1);
                return ACR1252U_Reader;
            }
               
              return null;
        }

    }
}
