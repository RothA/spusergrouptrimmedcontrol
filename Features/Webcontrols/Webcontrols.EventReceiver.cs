using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;

namespace aroth.sharepoint.controls.Features.Webcontrols
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("571ebe9d-31b4-4a48-91cb-d92432adfaec")]
    public class WebcontrolsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                //Make Web Config Modifications so that the custom web controls can be used
                //Get the Content Service
                SPWebService contentService = SPWebService.ContentService;

                //Add Web Config Modification
                contentService.WebConfigModifications.Add(this.createWebConfigModification());

                //Update Service
                contentService.Update();

                //Apply Modification
                contentService.ApplyWebConfigModifications();
            }
            catch { }
        }

        /// <summary>
        /// Creates the necessary web config modification for running the custom web controls
        /// </summary>
        /// <returns></returns>
        private SPWebConfigModification createWebConfigModification()
        {
            SPWebConfigModification myModification = new SPWebConfigModification();
            myModification.Path = "configuration/system.web/pages/controls";
            myModification.Name = @"add[@tagPrefix='aroth'][@namespace='aroth.sharepoint.controls'][@assembly='aroth.sharepoint.controls.SPUserGroupTrimmedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c7f26d7ca086c312']";
            myModification.Sequence = 0;
            myModification.Owner = "Admin";
            myModification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            myModification.Value = "<add tagPrefix='aroth' namespace='aroth.sharepoint.controls' assembly='aroth.sharepoint.controls.SPUserGroupTrimmedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c7f26d7ca086c312' /> ";

            return myModification;
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                // Remove Web Config modification
                //Get Content Service
                SPWebService contentService = SPWebService.ContentService;

                //Remove web config modification
                contentService.WebConfigModifications.Remove(this.createWebConfigModification());

                //Update
                contentService.Update();

                //Apply Modifications
                contentService.ApplyWebConfigModifications();
            }
            catch { }
        }

    }
}
