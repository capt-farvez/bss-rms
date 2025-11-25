# Deploying the .NET API Project to the Monsterasp.net Hosting Service


## Opening the Project in Visual Studio
To open the project in Visual Studio, follow these steps:
1. Go to the directory where you cloned the repository.
2. Go to the `bss-rms-api` folder.
3. Locate the `BssRmsApi.sln` file.
4. Double-click on the `BssRmsApi.sln` file to open it in Visual Studio.
5. Visual Studio will load the solution and all its projects. You can now start working on the project.

Make sure you have the necessary .NET SDK installed to work with the project. You can check the required SDK version in the `global.json` file if it exists in the solution directory.


## Running the Application To run the application in Visual Studio, follow these steps:
1. In Visual Studio, ensure that the `BssRmsApi` project is set as the startup project. You can do this by right-clicking on the `BssRmsApi` project in the Solution Explorer and selecting "Set as StartUp Project".
2. Press `F5` or click on the "Start Debugging" button in the toolbar to run the application.
3. Visual Studio will build the project and start the application. You should see the output in the console window or the web browser, depending on the type of application.
4. If the application is a web API, it will typically open a web browser pointing to the local URL where the API is hosted (e.g., `https://localhost:7212`).

## Publishing the Application in Monsterasp.net
Download the visual studio login details from Monsterasp.net and keep it in a safe location.
1. Go to the Monsterasp.net control panel and navigate to the "Overview" section.
2. Click on the `Deploy` button to access the deployment options.
3. Select the option `Download Publish Profile` to download the Visual Studio login details.

To publish the application, follow these steps:
1. In Visual Studio, right-click on the `BssRmsApi` project in the Solution Explorer and select `Publish`.
2. Choose a target for publishing (Folder) and select the Downloaded Publish Profile and click `Next`.
3. Configure the settings for your chosen target and click `Finish`.
4. Click on the `Publish` button to start the publishing process.
5. Visual Studio will build and publish the application to the specified target location. You can then deploy the published files to your desired environment.