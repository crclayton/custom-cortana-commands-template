using System;
using System.Collections.Generic;
using Windows.UI.Popups;


namespace CustomCortanaCommands
{

    class CortanaFunctions
    {
        /* This is the lookup of VCD CommandNames as defined in 
        CustomVoiceCommandDefinitios.xml to their corresponding functions */
        static public Dictionary<string, Delegate> vcdLookup = new Dictionary<string, Delegate>{
            // {"CommandName", new Action(voidFunction)}
            {"ShutDown", new Action(ShutDown)},
            {"Hibernate", new Action(Hibernate) }
        };

        static async void ShutDown()
        {
            var dialog = new MessageDialog("This is where I would shut the computer down.");
            await dialog.ShowAsync();
        }

        static async void Hibernate()
        {
            var dialog = new MessageDialog("This is where I would hibernate the computer.");
            await dialog.ShowAsync();
        }

    }
}
