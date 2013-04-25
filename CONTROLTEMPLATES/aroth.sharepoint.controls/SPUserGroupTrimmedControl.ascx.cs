using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace aroth.sharepoint.controls
{
    /// <summary>
    /// This is an extension of the default SPSecurityTrimmedControl that allows for trimming content 
    /// based on membership of the currently logged in user in SharePoint Groups
    /// </summary>
    public partial class SPUserGroupTrimmedControl : SPSecurityTrimmedControl
    {
        /// <summary>
        /// The list of groups that will be used for trimming the content
        /// </summary>
        private List<string> groups;

        /// <summary>
        /// The comma delimited string of group names
        /// </summary>
        public string GroupsString
        {
            get
            {
                // Returns the comma delimited list of group names
                return string.Join(",", groups.ToArray());
            }
            set
            {
                // Takes a list of comma separated group names and sets them to the array of groups
                groups.AddRange(
                    value.Split(new char[] { ',' },
                   System.StringSplitOptions.RemoveEmptyEntries)
                    );
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SPUserGroupTrimmedControl()
        {
            //INITIALIZE
            groups = new List<string>();
        }

        /// <summary>
        /// Overrides the Render function to handle looking up group membership and determining if content should be rendered to page
        /// </summary>
        /// <param name="output">HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter output)
        {
            //Check to see if content should be rendered
            if (!string.IsNullOrEmpty(GroupsString) && IsMember())
            {
                // Group String is NOT empty AND current user is a member of the group so render output to page
                base.Render(output);
            }
        }

        /// <summary>
        /// Determines if currently logged in user is a member of any of the groups specified in the groups list
        /// </summary>
        /// <returns>True if current user is a member of one of the groups, False otherwise</returns>
        private bool IsMember()
        {
            try
            {
                // Set isMember to False so it will not allow output to be rendered in case of errors, etc
                bool isMember = false;

                // Go through each group and check memberships
                foreach (string group in groups)
                {
                    try
                    {
                        //First check membership of current web
                        isMember = SPContext.Current.Web.IsCurrentUserMemberOfGroup(SPContext.Current.Web.Groups[group.Trim()].ID);

                    }
                    catch
                    {
                        try
                        {
                            //Group did not exist on the current web object so check to see if it is a site group
                            isMember = SPContext.Current.Web.IsCurrentUserMemberOfGroup(SPContext.Current.Web.SiteGroups[group.Trim()].ID);
                        }
                        catch { /*Group is not a site group either*/ }

                    }

                    //Check isMember and break out of loop if one membership has already been found
                    if (isMember)
                    {
                        break;
                    }
                }

                // Return isMember
                return isMember;

            }
            catch { }

            //Return false if somehow got to this point
            return false;
        }

    }
}
