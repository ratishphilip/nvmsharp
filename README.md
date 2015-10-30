# nvmsharp
NVM# is a Windows 10 desktop app which provides an effective way of managing the environment variables. 

Managing environment variables has been a tedious task since the early days of Windows. Windows 10 offers very little improvement on the way environment variables were managed (till Windows 8.1). However, I feel that the users should be provided with a better user experience for managing the environment variables. I strongly recommend that environment variables management be added to the Settings app in Windows 10 (coz that's where all the Control Panel features are going to be ultimately)

So I made this app as a proof of concept on how the user experience should be. NVM# matches the look and feel of the Settings app in Windows 10.

I wanted to make NVM# as a Windows Store app, but you cannot access (and modify) Environment Variables in Store app as they are sandboxed. So I created a WPF desktop app which must be run with Administrator privileges to manage your Environment Variables.

In addition to providing the basic CRUD operations on Environment Variables, NVM# also allows you to import and export environment variables from and to an XML file, respectively. It proves really helpful in a scenario where the team needs to have the same environment variables to work efficiently (believe me, I have faced this scenario a lot(years ago) and had wished such an app existed back then!)

I do hope Microsoft provides such a feature in their Settings app in the near future!

Till then, feel free to use my app!

Cheers!
