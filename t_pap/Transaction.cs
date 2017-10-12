using npay.MpesaService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace npay
{
    /// <summary>
    /// This class contains necessary to complete MPESA Payment , in future any other methods for other services can be included here
    /// or a new class made for their sake
    /// </summary>
    public class Transaction
    {
        private string PASSWORD { get; set; }
        private string TIMESTAMP { get { return "20160510161908"; } } // For testing purposes
        LNMO_portTypeClient client;
        double amount;
        string phone;
        //This is a test paybill number ,  be sure  not to make any payments to it, you will be charged!!!
        const string MERCHANT_ID = ""; // Enter your MERCHANT_ID
        string REFERENCE_ID = String.Empty;
        const string PASSKEY = "";//Again test passkey, this key is only provided to allow testing, redistribution of this key is a serious offense, 
        // Read the terms and conditions on this link regarding passkeys  http://online.verypdf.com/u/78076/api/20151215-050804-2472535945.pdf
        CheckOutHeader header = null;
        processCheckOutRequest request = null;
        processCheckOutResponse checkoutresponse = null;
        // Variables names following MPESA online checkout convension refer to link below for more information 
        // http://online.verypdf.com/u/78076/api/20151215-050804-2472535945.pdf
        string TRX_ID = String.Empty;
        string DESCRIPRION = String.Empty;
        string MERCHANT_TRANSACTION_ID = String.Empty;
        public Transaction(int amount , string phone, string service)
        {
            this.amount = amount;
            this.phone = phone;
            REFERENCE_ID = service;
            MERCHANT_TRANSACTION_ID = new Random().Next(0, 100000).ToString();
        }
        public string pay()
        {
            try
            {
                //Make a request to pay 
            checkoutresponse = getClient(getCheckOutHeader(), getCheckOutRequest()).processCheckOut(header, request);
            //Check if successfull
            if (checkoutresponse.RETURN_CODE == "00")
            {
                //go ahead and confirm the transaction
                return confirm(checkoutresponse);
            }//success
            //Confirm payment 
            return checkForErrors(checkoutresponse.RETURN_CODE);
            }
            catch (EndpointNotFoundException ex)
            {
                string message = ex.Message;
                return null;
            }
            
        }
        /// <summary>
        /// Generate password based on LNMO documentation 
        /// </summary>
        /// <returns></returns>
        private string generatePassword()
        {
            //Hash using SHA256  then encode to base 64
            HashAlgorithm algorithm = new SHA256CryptoServiceProvider();
            //Generate timestamp 
            DateTime dt = DateTime.Now;
            string formattedDate = dt.ToString("yyyyMMddHHmmss");
            //For testing we use the provided timestamp 
            string toHash  = MERCHANT_ID + PASSKEY + formattedDate;
            byte [] hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            PASSWORD =  BitConverter.ToString(hashedBytes); 
            // but for testing we use the provided password, this is because I do not want my passkey to be redistributed
  
            return PASSWORD;

        }
        /// <summary>
        /// Sets up the request header 
        /// </summary>
        /// <returns>the header </returns>
        private CheckOutHeader getCheckOutHeader()
        {
            header = new CheckOutHeader();
            header.PASSWORD = generatePassword();
            header.TIMESTAMP = TIMESTAMP;
            header.MERCHANT_ID = MERCHANT_ID;
            return header;
        }
        /// <summary>
        /// constructs the soap request body 
        /// </summary>
        /// <returns>A request body </returns>
        private processCheckOutRequest getCheckOutRequest()
        {
            request = new processCheckOutRequest();
            request.AMOUNT = amount;
            request.CALL_BACK_METHOD = "POST";
            request.CALL_BACK_URL = "http://127.0.0.1"; // This will not work since we do not have a registered callback Url
            request.MERCHANT_TRANSACTION_ID = this.MERCHANT_TRANSACTION_ID;
            request.MSISDN = this.phone;
            request.REFERENCE_ID = REFERENCE_ID;
            request.TIMESTAMP = TIMESTAMP;
            return request;
        }
        /// <summary>
        /// Get a client that can be used to make requests 
        /// </summary>
        /// <param name="coHeader"></param>
        /// <param name="coRequest"></param>
        /// <returns>Client</returns>
        private LNMO_portTypeClient getClient(CheckOutHeader coHeader, processCheckOutRequest coRequest)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(0, 1, 30);
            binding.MaxReceivedMessageSize = 20000000;
            EndpointAddress address = new EndpointAddress("http://safaricom.co.ke/mpesa_online/lnmo_checkout_server.php?wsdl");
            client = new LNMO_portTypeClient(binding, address);
            return client;
        }
        /// <summary>
        /// Confirms payment 
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private string confirm(processCheckOutResponse res)
        {
            string TRX_ID = res.TRX_ID;
            string DESCRIPRION = res.DESCRIPTION;
            string MERCHANT_T_ID = MERCHANT_TRANSACTION_ID;
            string status = client.confirmTransaction(header, ref TRX_ID, ref MERCHANT_TRANSACTION_ID, out DESCRIPRION);
            return checkForErrors(status);
        }

        /// <summary>
        /// Proccesses return codes 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string checkForErrors(string code)
        {
            string message = String.Empty;
            switch (code)
            {
                case "00":
                   message = "Success: Request completed";
                    break;
                case "01":
                    message = "Failure: Authentication failed";
                    break;
                case "03":
                    message = "Amount less than the minimum single transfer allowed on the system";
                    break;
                case "04":
                    message = "Amount more than the maximum single transfer amount allowed";
                    break;
                case "05":
                    message = "Transaction expired before it was processed, please try again";
                    break;
                case "06":
                    message = " Transaction could not be confirmed possibly due to confirm operation failure";
                    break;
                case "08":
                    message = "Paybill provided doesn't exist";
                    break;
                case "09":
                    message = "The customer has reached transaction limit for the day";
                    break;
                case "10":
                    message = "Your customer is not a registered customer of MPESA";
                    break;
                case "11":
                    message = "Sorry the system could't process your request, try again later";
                    break;
                case "12":
                    message = "Corrupt transaction details, details differ from ones provided in initial request";
                    break;
                case "29":
                    message = "System is down! Sorry for the inconvinience";
                    break;
                case "30":
                    message = "Reference ID is missing";
                    break;
                case "31":
                    message = "Invalid amount to be paid";
                    break;
                case "32":
                    message = "Your customer's MPESA account has not been activated";
                    break;
                case "33":
                    message = "Your customer's is not approved to transact";

                    break;
                case "34":
                    message = "Ypur request took longer than expected";
                    break;
                case "35":
                    message = "Duplicate transaction detected";
                    break;
                default:
                    message = "Unknown error, try again later";
                    break;
            }
            return message;
        }


    }
}
