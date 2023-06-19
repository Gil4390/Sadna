# Sadna Express

Hello! welcome to Sadna Express, our trading system program.
we are third year students in software engineering in Ben Gurion University, we wrote this project for "Sadna" course.

# Team members:

Gil khais - 207919374

Noga Schawartz - 207687849

Shay kresner - 209120278

Radwan Ganen - 322509951

Tal Galmor - 318416575

Dina Agapov - 207181629

# Running the project:
To run the project, we will perform the following steps -

Running the server:
- Open Visual Studio
- We will check the fields in the app.config file (detailed below) and check if they match the parameters we want to run
- Compile the code and run sadna express.
- Now our server is running and ready for action

Running the clients:
- Open Visual Studio Code
- Open a terminal and run npm install (if already installed, run npm install -f)
- We will run npm start in the terminal
- A client will open a web page for us
- To run another client, open a new web page, enter the address according to the Host field in the app.config file like this: Host:3000

# app.config
 
app.config file holds the exteral services url, database connection, system manager details and the start point of the trading system (initialize or not). 
The file is in Json format,  you can change the key values that will fit to your use, but make sure you supply valid values -> if not the system will not execute.

Here is a breakdown of the fields in the file:

appSettings:
* Host: It specifies the host name or IP address of the server.
* ApiPort: It defines the port number for the API service.
* SignalRPort: It specifies the port number for the Notifications and messages service. 
* PaymentServiceURL: It represents the URL for the payment service.
* SupplierServiceURL: It represents the URL for the supplier service.
* SystemManagerEmail: It specifies the email address of the system manager.
* SystemManagerFirstName: It represents the first name of the system manager.
* SystemManagerLastName: It represents the last name of the system manager.
* SystemManagerPass: It represents the password of the system manager.
* InitTradingSystem: It is a boolean value indicating whether the trading system should be initialized when running it.
* StartWithCleanDB: It is a boolean value indicating whether the system should start with a clean database.
* RunLoadData: It is a boolean value indicating whether to load data into the system.
* StateFileConfig: It represents the configuration for the state file.
* TestDB: It specifies the connection string for the test database

connectionStrings:
* name: It represents the name of the remote database.
* connectionString: It represents the configuration settings of the remote database.

# State Configuration File:

The state configuration file for the Sadna Trading System is a JSON file containing an array of objects. Each object represents a system function to be executed during startup. The object consists of two fields: "function" and "params".

"function" (string): Specifies the name of the function to be executed.

"params" (array): Contains the parameters required for the corresponding function.

# example:
![image](https://github.com/Gil4390/Sadna/assets/76035272/75c7dc29-a937-4f2d-b57b-6fd3905e5bf0)

in order to run the system from the init file you must set the following settings in app.config:
![image](https://github.com/Gil4390/Sadna/assets/80397780/c92c9455-2628-46d2-a870-c274e810499c)

and choose either "data" or "data2" for the field StateFileConfig

Currently there are 3 possible initial states for the system to start with:
<ul>
  <li> empty state </li>
  <li> state1 
    <ul> 
      <li> Registered users: U1, U2, U3 - members. </li>
      <li> U1 logs in and opens store S1. </li>
      <li> U1 appoints U2 as S1 store owner. </li>
      <li> U2 logs in and appoints U3 as S1 store owner. </li>
      <li> U1, U2 logs out. </li>
    </ul>
  </li>
    <li> state2
    <ul> 
      <li> Registered users: U1 - admin, U2, U3, U4, U5 - members. </li>
      <li> U2 logs in and opens store store1. </li>
      <li> U2 adds item "apple" to store S1 with cost 20$ and quantity 10. </li>
      <li> U2 logs out. </li>
    </ul>
  </li>
</ul>

supported initialization functions: 
- Register(userId, email, firstName, lastName, password)
- CreateSystemManager(email)
- Login(userId, email, password)
- Logout(userId)
- OpenNewStore(userId, StoreName)
- AddItemToStore(userId, storeID, itemName, itemCategory, itemPrice, quantity)
- AppointStoreManager(userId, storeID, anotherEmail)
- AppointStoreOwner(userId, storeID, anotherEmail)
- ReactToJobOffer(userId, storeID, userId2, react)


Documents can be found here:
- Use Cases / Glossary - https://docs.google.com/document/d/1nwZft8kNO3OjLYLuS8tpSzU0Gz247nENy4-0tZwhq_A/edit

- Architecture Diagram - https://drive.google.com/file/d/11LNjJDK7azYbV6blG5h-ucHGxrN6UemM/view

- Class Diagram - https://drive.google.com/file/d/1y6IqBJfDbgYzhdU5ixmV_VKtPHapEDGq/view?usp=sharing

- Wireframe - https://www.figma.com/file/x74k0dMJ9Yw7SZ3jvyAhLH/login-(Community)?node-id=102-2&t=mLByabG70xMQZryD-0
