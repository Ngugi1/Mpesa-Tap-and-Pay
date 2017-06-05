using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npay
{
    public class NdefHelper
    {
        public static string getNdefFromByteArray(byte[] readData)
        {
            //Create Ndef message 
            string phone = String.Empty;
            NdefMessage message = NdefMessage.FromByteArray(readData);
            //Loop through the records 
            foreach (NdefRecord record in message)
            {
                if (record.CheckSpecializedType(false) == typeof(NdefTextRecord))
                {
                    var textRecord = new NdefTextRecord(record);
                    phone = textRecord.Text;

                }
               
            }
            return phone;
        }
    }
}
