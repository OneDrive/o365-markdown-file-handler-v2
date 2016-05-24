# Markdown File Handler - Sample Code

This project provides a sample implementation of a file handler 2.0 for Microsoft Office 365.
The file handler allows creating, editing, and viewing Markdown text files with the `md` file extension.

This sample also illustrates a custom action, with the **Add to ZIP** action added to all files.

## Getting Started

To get started with the sample, you need to complete the following actions:

1. Register a new application with Azure Active Directory, generate an app password, and provide a redirect URI for the application.
2. Register the file handler extensions as an add-in for your application in Azure Active Directory.
3. Run the sample project and sign-in with your Office 365 account and accept the consent prompt so the file handler is registered for your account.
4. Navigate to OneDrive for Business or a SharePoint document library and allow file hanlders to load.
5. Click New, and then Markdown Document, to create a new file.

### Register a new application

To register a new application with Azure Active Directory, log into the [Azure Portal](https://portal.azure.com).
File handler apps **cannot** be registered through the [Application Registration Portal](https://apps.dev.microsoft.com) since that portal created converged applications which are not currently compatible with file handlers.

After logging into the Azure Portal, the following steps will allow you to register your file handler application:

1. Navigate to the **Azure Active Directory** module.
2. Select **App registrations** and click **New application registration**.
	1. Type the name of your file handler application.
	2. Ensure **Application Type** is set to **Web app / API**
	3. Enter a sign-on URL for your application, for this sample use `https://localhost:44362`.
	4. Click **Create** to create the app.
3. After the app has been created successfully, select the app from the list of applications. It should be at the bottom of the list.
4. Copy the **Application ID** for the app you registered and paste it into the Web.config file on the line: `<add key="ida:ClientId" value="application id here" />`
5. Make a note of the **Object ID** for this application, since you will need this later to register the file handler manifest.
6. Configure the application settings for this sample:
	1. Select **Reply URLs** and ensure that `https://localhost:44362` is listed.
	2. Select **Required Permissions** and then **Add**.
	3. Select **Select an API** and then choose **Microsoft Graph** and click **Select**.
	4. Find the permission **Have full access to all files user can access** and check the box next to it, then click **Select**, and then **Done**.
	5. Select **Keys** and generate a new application key by entering a description for the key, selecting a duration, and then click **Save**. Copy the value of the displayed key since it will only be displayed once.
		* Paste the value of the key you generated into the Web.config file in this project, inside the value for the line: `<add key="ida:ClientSecret" value="put application key here" />`

### Register the file handler manifest

After registering your app with Azure Active Directory, you can upload the file hanlder manifest information into the application.

For detailed instructions on how to upload the file handler manifest, see [Registering file handlers](https://dev.onedrive.com/file-handler-v2/file-handlers/register-file-handler-manually.htm).

The file handler manifest for the sample file is available in the `addin-schema-debug.json` file in this project.

### Run the project and sign-in

Once your project is registered and configured, you're ready to run it. Press F5 to launch the project in the debugger.
The file handler project will load in your default browser and be ready for you to sign in.
Sign in to the file handler project, and authorize the application to have access to the data in your OneDrive.

### Navigate to OneDrive and use the Markdown file handler

Once you have authorized the file handler to have access, the file handler will be available in OneDrive and SharePoint.
After signing in to the app, click the "Try it in OneDrive" button to launch your OneDrive.
Due to service caches, it may take a few minutes before your file handler shows up in OneDrive.
You may need to close your browser and open it again before the file handler will be activated.

## Next Steps

Now that the file handler is wired up, try creating a new Markdown document.

### Create a new Markdown file

Click **New** and then **Markdown Document**.
OneDrive will create a new file with the `.md` extension and launch the file handler app's editor.
You can add content to the file, click **Save** and then close the file handler's tab.

### Preview the Markdown file 

Now that you've created a markdown file in your OneDrive, click back to OneDrive and refresh the page.
Find the new file you created (look for `MD File.md`) and click on it.
This will launch the file handler in preview mode.
You will see a preview of the content written into the Markdown file displayed inline in the OneDrive preview experience.

### Open the Markdown file

Next try to open the file using the file handler experience.
Click the **Open** button, and select **Open in {app name}** to launch your file handler project and edit the file again.
You can use the **Rename** button to rename the file in OneDrive or create a sharing link for the file by clicking **Get Link**.

### Add to ZIP

The last part of this sample is to use the custom action provided by the file handler.
Close the file handler tab and switch back to OneDrive.
Select a file (click the check) and then in the toolbar, click `...` and then **Add to ZIP - Debug**.
This will launch the file handler's custom action handler and provide all of the items selected.
The file handler code will download the items, compress them into a ZIP file, and upload the file back to OneDrive as `archive.zip` in the same folder.

## Related references

For more information about file handlers, check out the [OneDrive developer portal](https://dev.onedrive.com).

For more inforamtion about Microsoft Graph API, see [Microsoft Graph](https://graph.microsoft.com).


## License

See [License](LICENSE.txt) for the license agreement convering this sample code.
