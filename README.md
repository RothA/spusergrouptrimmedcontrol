SPUserGroup Trimmed Control
=========================

SharePoint 2010 Control that extends the Microsoft.SharePoint.WebControls.SPSecurityTrimmedControl class to allow for specifying SharePoint Groups for trimming content.

Installation
------------

To install this control you can package the solution yourself OR grab the \installation\SPUserGroupTrimmedControl.wsp file and deploy it to your SharePoint environment.

You can also copy the following files to your SharePoint server:
	
	1. \installation\Install-Solutions.ps1
	
	2. \installation\solutions.xml
	
	3. \installations\SPUserGroupTrimmedControl.wsp

Then do the following on your SharePoint server.
	
	1.  Run the SharePoint 2010 Management Shell as an administrator.
	
	2.  Navigate to the folder where the files listed above are located.
	
	3.  Run the Install-Solutions.ps1 file which will handle deploying and activiting the SharePoint solution

Usage
-----

To use this control on a page in SharePoint do the following.

	1.  Register the tag prefix on the page by including this at the top of your ASPX page.
		
		<%@ Register TagPrefix="aroth" Namespace="aroth.sharepoint.webcontrols" Assembly="aroth.sharepoint.controls.SPUserGroupTrimmedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c7f26d7ca086c312" %>

	2.  You can then wrap content that you want to trim with the control as such.  You simply specify the names of the groups that you want to display the content to.

		<aroth:SPUserGroupTrimmedControl id="trimcontrol1" GroupsString="Group Name 1, Group Name 2, Group Name 3" runat="server">
			<h1>Content to Trim</h1>
		</aroth:SPUserGroupTrimmedControl>

		
