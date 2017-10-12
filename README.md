# NPay
NFC Payments via MPESA. Tap, enter pin and you are done.
This project was conceptualized before MPESA 1 tap came to the market. This project and MPESA 1 Tap are similar but with minor differences especially on the hardware used.

## SYSTEM IMPLEMENTATION
## `Drivers and Operating System Requirements`
To use the solution provided here one needs to do some set up first. Here are the of the items required before the solution can be executed.

* Personal computer running Windows operating system (Windows 10 Recommended , see full details in table below)

* Windows 10 Pro (At least this is tested)
* Version  1607
* OS Build 14393.1198   
* Microsoft .NET Framework  Version 4.6.01586

Install the following drivers on your PC(Choose 64/32 bit drivers). You can find them [here] (http://www.acs.com.hk/en/products/342/acr1252u-usb-nfc-reader-iii-nfc-forum-certified-reader/#tab_downloads)

* MSI Installer for PC/SC Driver (64-bit) or MSI Installer for PC/SC Driver (32-bit)
* PC/SC Drivers 


### `Software Tools Used`

Microsoft Visual Studio Ultimate 2013 - Update 4
GoToTags Reader Software 
NFC Tools (Android)

### `Hardware Tools Used`
1. ACR1252U USB NFC Reader III (NFC Forum Certified Reader) <br/>
Interface : USB 2.0 Full Speed <br/>
Operating distance - up to 50 mm (depends on the tag type) <br/>
Smart Card Interface Support <br/>	
ISO 14443 Type A and B , MIFARE <br/>
FeliCa 4 types of NFC (ISO/IEC 18092) tags <br/>

2. NFC Enabled Android/Windows Phone 
3. NTAG213 NFC tags (Mifare Classic , SMARTRAC BullsEye)

## `Environment Setup`
### `Preparing NTAG213 NFC tags`
Data needs to be written on the Tag provided so that it can be used to initiate transactions. The data to be written on the Tag is the mobile number of the customer.
Follow the steps below: 
* Turn on NFC on your android device
* Open NFC Tools application you downloaded on your device
* Go to Write Tab
* Click on add record button 
* select Text  record type
* Insert a valid phone number in this format including country code e.g 254713195124. Be sure to enter a phone number that is M-Pesa Registered.
* Click finish when you are done.
* Notice a Write Tag button, click on it and then approach a NFC Tag provided. **Warning: Follow these steps strictly to avoid corrupting the Tag!**
* You can now stick the tag at the back of your phone.

### `Preparing ACR1252U-M1 Reader`
* Ensure you are connected to the internet 
* Plug in the reader to your USB 2.0 port, if it makes a buzzing sound you are set.
####Running the solution
* Fire up Microsoft Visual Studio Ultimate 2013
* Open the project folder and open the project in visual studio, all the libraries used will be installed automatically by nuget package manager, it might take a while, just be patient.
* Tap the sticker at the back of your phone on your reader.
* You will receive a USSD push notification requesting for your Bonga Point PIN. To complete the transaction provide the pin. **`Warning: If you reply with your pin the amount you provided will be deducted from your account. I will not be held accountable for any amounts transferred, funds are transferred at your own risk`**

