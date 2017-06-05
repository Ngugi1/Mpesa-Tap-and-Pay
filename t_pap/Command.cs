using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npay
{
    /// <summary>
    /// Contains base commands needed to communicate with the reader as well as methods to format the commands.
    /// Methods to read tags are also included here
    /// 
    /// </summary>
    public class Command
    {
        //To understand these commands read documentation here 
        //https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=1&cad=rja&uact=8&ved=0ahUKEwj27_apmIvUAhUJCcAKHXQ7C-cQFggjMAA&url=https%3A%2F%2Fwww.acs.com.hk%2Fdownload-manual%2F6402%2FAPI-ACR1252U-1.09.pdf&usg=AFQjCNEju2tFkgnUslK03fyoWA3KEbGzQA&sig2=Q-bP2JWIS2ltiCbwtMf93g
        // Page 33 - Reading Binary Blocks
        // Send SELECT command
        public static byte[] READ_FIRST_12_BLOCKS_FROM_BLOCK_5 { get { return new byte[] { 0XFF, 0XB0, 0X00, 0x05, 0x10};}} // Read only property 
        
        public static byte[] READNEXT_12BLOCKS_FROM_BLOCK_9 { get{ return  new byte[] { 0XFF, 0XB0, 0X00, 0x09, 0x10 };}}

        public static byte[]  formatResponses(byte [] dataBlock1 , byte [] dataBlock2)
        {
            byte[] readData = new byte[64];
            //Copy the first datablock from index 3 since here is where the real data starts, ignore the other elements that come before
            // index 3, they are just information for the tag.
            Array.Copy(dataBlock1, 3, readData, 0, 13);
            //Copy all data from the second block, we are reading the tag in two cycles since one can only read max of 12 bytes 
            //at a go in type 2 NFC tags
            Array.Copy(dataBlock2, 0, readData, 13, dataBlock2.Length);
            //Return this byte array which is basically a formatted ndef message containing the moile number, place a break 
            //point to inspect 
            return readData;

        } 


        

    }
}
