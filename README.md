# Markdown File Handler - Sample Code

This project provides a sample implementation of a file handler v2.0 for Microsoft Office 365.
The file handler allows creating, editing, and viewing Markdown text files with the `mdown` file extension.

This sample also adds a **Convert to PDF** to Microsoft Word documents and **Add to ZIP** action to all files, to illustrate using custom actions with file handlers.

## Getting Started

To get started with the sample, you need to complete the following actions:

1. Register a new application with Azure Active Directory, generate an app password, and provide a redirect URI for the application.
2. Register the file handler extensions as an add-in for your application in Azure Active Directory.
3. Run the sample project and sign-in with your Office 365 account and accept the consent prompt so the file handler is registered for your account.
4. Navigate to OneDrive for Business or a SharePoint document library and allow file hanlders to load.
5. Click New, and then Markdown Text File, to create a new file.

### Register a new application

To register an application, visit the [Microsoft Developer App Center](https://apps.dev.microsoft.com).

### Register the file handler extensions

TBD

### Run the project and sign-in

TBD

### Navigate to OneDrive and use the Markdown file handler

TBD



## Registering file handler extension by hand

To register the file handler extensions by hand, you need to make an API call to Azure Active Directory to add the additional metadata to your application's `addIn` collection.
This can currently only be done using the AAD Graph, and not the Microsoft Graph.

To make this API call, you need an access token that includes the `Directory.ReadWrite.All` scope, which requires administrator consent.
Once you have an access_token with the proper scope, you can make the following call:

```http
PUT https://graph.windows.net/myorganization/applications/{applicationObjectId}/addIns?api-version=1.6
Content-Type: application/json

{
"value": [
	// Add the completed file handler v2 schema here
	]
}
```