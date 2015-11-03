# nvmsharp
<img alt="NVM# logo" src="https://cloud.githubusercontent.com/assets/7021835/10910990/618ff19e-81f7-11e5-9f9a-8b00ff108cb2.png" />

NVM# is a Windows 10 desktop app (.Net 4.6) which provides an effective way of managing the environment variables. 

Managing environment variables has been a tedious task since the early days of Windows. Windows 10 offers very little improvement on the way environment variables were managed (till Windows 8.1). However, I feel that the users should be provided with a better user experience for managing the environment variables. I strongly recommend that environment variables management be added to the Settings app in Windows 10 (coz that's where all the Control Panel features are going to be ultimately)

So I made this app as a proof of concept on how the user experience should be. This app is an update to my previous project NVM in CodeProject (which I wrote 8 years ago!).

<img alt="screenshot" src="https://cloud.githubusercontent.com/assets/7021835/10911175/6caccdf8-81f8-11e5-9c6f-e9b71ad4c353.png" />

NVM# matches the look and feel of the Settings app in Windows 10. I wanted to make NVM# as a Windows Store app, but you cannot access (and modify) Environment Variables in Store app as they are sandboxed. So I created a WPF desktop app which must be run with Administrator privileges to manage your Environment Variables.

<img alt="import" src="https://cloud.githubusercontent.com/assets/7021835/10911173/6c95fc04-81f8-11e5-9131-48cd6bba29b4.png" />

In addition to providing the basic CRUD operations on Environment Variables, NVM# also allows you to Import and Export environment variables from and to an XML file, respectively. It proves really helpful in a scenario where the team needs to have the same environment variables to work efficiently (believe me, I have faced this scenario a lot several years ago and had wished such an app existed back then!)

<img alt="export" src="https://cloud.githubusercontent.com/assets/7021835/10911174/6c97a586-81f8-11e5-81ec-a33a0ce49795.png" />

I do hope Microsoft provides such a feature in their Settings app in the near future!

Till then, feel free to use my app!

Cheers!
