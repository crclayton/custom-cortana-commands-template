using System;
using System.Collections.Generic;
using Windows.UI.Popups;

using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;

using Windows.System;
using Windows.Media.SpeechRecognition;
using Windows.ApplicationModel.Activation;

// for SendSerialData example
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace CustomCortanaCommands
{

    class CortanaFunctions
    {
        /*
        This is the lookup of VCD CommandNames as defined in 
        CustomVoiceCommandDefinitios.xml to their corresponding actions
        */
        private readonly static IReadOnlyDictionary<string, Delegate> vcdLookup = new Dictionary<string, Delegate>{

            /*
            {<command name from VCD>, (Action)(async () => {
                 <code that runs when that commmand is called>
            })}
            */

            {"OpenWebsite", (Action)(async () => {
                 Uri website = new Uri(@"http://www.crclayton.com");
                 await Launcher.LaunchUriAsync(website);
             })},

            {"OpenFile", (Action)(async () => {
                StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(@"Test.txt");
                await Launcher.LaunchFileAsync(file);
            })},

            {"CreateFile", (Action)(async () => {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.CreateFileAsync(
                    @"NewFile.txt", CreationCollisionOption.ReplaceExisting);

                await storageFolder.GetFileAsync("NewFile.txt");
                await FileIO.WriteTextAsync(sampleFile, "This file was created by Cortana at " + DateTime.Now);

            })},

            {"SendSerialData", (Action)(async () => {
                string comPort = "COM3";
                string serialMessage = "String sent to the COM port";

                string selector = SerialDevice.GetDeviceSelector(comPort);
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

                if(devices.Count == 0)
                {
                    MessageDialog popup = new MessageDialog($"No {comPort} device found.");
                    await popup.ShowAsync();
                    return;
                }

                DeviceInformation deviceInfo = devices[0];
                SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
                serialDevice.BaudRate = 9600;
                serialDevice.DataBits = 8;
                serialDevice.StopBits = SerialStopBitCount.Two;
                serialDevice.Parity = SerialParity.None;

                DataWriter dataWriter = new DataWriter(serialDevice.OutputStream);
                dataWriter.WriteString(serialMessage);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
                dataWriter = null;
            })},

        };

        /*
        Register Custom Cortana Commands from VCD file
        */
        public static async void RegisterVCD()
        {
            StorageFile vcd = await Package.Current.InstalledLocation.GetFileAsync(
                @"CustomVoiceCommandDefinitions.xml");

            await VoiceCommandDefinitionManager
                .InstallCommandDefinitionsFromStorageFileAsync(vcd);
        }

        /*
        Look up the spoken command and execute its corresponding action
        */
        public static void RunCommand(VoiceCommandActivatedEventArgs cmd)
        {
            SpeechRecognitionResult result = cmd.Result;
            string commandName = result.RulePath[0];
            vcdLookup[commandName].DynamicInvoke();
        }
    }
}
