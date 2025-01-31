# Sadna Express

This project is a **C# and React-based trading system** designed to provide a comprehensive marketplace platform for users. It implements a modular and scalable architecture that integrates with external services for payments and supplies.

## Table of Contents
- [Features](#features)
- [Technical Architecture](#technical-architecture)
- [System Components](#system-components)
- [Performance and Testing](#performance-and-testing)

---

## Features

### User Management
- Supports Guests, Members, Store Owners, and Managers.
- Registration, login, and profile management.
- Secure password handling and email validation protocols.

### Marketplace Functionality
- Browse stores and search for products using filters.
- Create and manage shopping carts.
- Store Owners can manage inventory and define store policies.

### Store Management
- Store creation and ownership delegation.
- Advanced role-based permissions system.

### Transaction Handling
- Comprehensive purchase workflows with discount and purchase policy validation.
- Integration with external payment and supply services.

### Notifications
- Real-time notifications via **SignalR** for events such as purchases and reviews.
- Offline notifications stored for later retrieval.

### Bidding System
- Allows negotiation through item bidding.
- Supports counter-offers and bid approval workflows.

### Analytics and Logging
- Tracks user and system activity for audits.
- Provides revenue reports and activity insights.

### Robust Policy Management
- Implements complex discount and purchase conditions (e.g., value- and quantity-based rules).

---

## Technical Architecture

### Backend (C#)
- Built around a singleton **TradingSystem** core component.
- Includes microservices for Payments, Supplies, and Notifications.
- Persistent layers for data storage (users, orders, items, policies).

### Frontend (React)
- Organized into modular components: actions, hooks, models, and pages.
- Provides a responsive and intuitive interface for users.

---

## System Components

### Core Modules
1. **User Management**
   - Handles user registration, login, and profile updates.
2. **Store Management**
   - Manages store creation, inventory, and policies.
3. **Transaction Processing**
   - Integrates external services for payments and supplies.
4. **Notification System**
   - Powered by SignalR for real-time updates.
5. **Bidding Workflow**
   - Facilitates item negotiation between users and store owners.

### Data Persistence
- Stores information on users, stores, items, orders, and policies in a structured database.

### Microservices
- **PaymentService**: Handles payment transactions.
- **SupplierService**: Manages supply chain operations.
- **SignalR Notification**: Ensures real-time notifications and updates.

---

## Performance and Testing

### Load Testing
- Evaluated search functionality, shopping cart operations, and checkout processes.
- Ensures high throughput with minimal error rates.

### Stress Testing
- Validates system resilience under peak load conditions with up to 300 concurrent users.

### Key Metrics
- **High Throughput**: Consistently processes user actions with negligible delays.
- **Error Handling**: Minimal errors under high load scenarios.


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
- To run another client, open a new web page, enter the address according to the Host field in the app.config file like this: localhost:3000

# app.config
 
app.config file holds the exteral services url, database connection, system manager details and the start point of the trading system (initialize or not). 
The file is in Json format,  you can change the key values that will fit to your use, but make sure you supply valid values -> if not the system will not execute.

Here is a breakdown of the fields in the file:

appSettings:
* <code>Host</code>: It specifies the host name or IP address of the server.
* <code>ApiPort</code>: It defines the port number for the API service.
* <code>SignalRPort</code>: It specifies the port number for the Notifications and messages service. 
* <code>PaymentServiceURL</code>: It represents the URL for the payment service.
* <code>SupplierServiceURL</code>: It represents the URL for the supplier service.
* <code>SystemManagerEmail</code>: It specifies the email address of the system manager - if there is no data file loading, that system manager will be set in the system if RunLoadData will be set to true.
* <code>SystemManagerFirstName</code>: It represents the first name of the system manager.
* <code>SystemManagerLastName</code>: It represents the last name of the system manager.
* <code>SystemManagerPass</code>: It represents the password of the system manager.
* <code>InitTradingSystem</code>: It is a boolean value indicating whether the trading system should be initialized when running it.
* <code>StartWithCleanDB</code>: It is a boolean value indicating whether the system should start with a clean database.
* <code>RunLoadData</code>: It is a boolean value indicating whether to load data into the system.
* <code>StateFileConfig</code>: It represents the configuration for the state file.
* <code>TestDB</code>: It specifies the connection string for the test database

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
  <li> empty state  - ""</li>
  <li> state1  - "data"
    <ul> 
      <li> Registered users: U1, U2, U3 - members. </li>
      <li> U1 logs in and opens store S1. </li>
      <li> U1 appoints U2 as S1 store owner. </li>
      <li> U2 logs in and appoints U3 as S1 store owner. </li>
      <li> U1, U2 logs out. </li>
    </ul>
  </li>
    <li> state2 - "data2"
    <ul> 
      <li> Registered users: U1 - admin, U2, U3, U4, U5 - members. </li>
      <li> U2 logs in and opens store store1. </li>
      <li> U2 adds item "apple" to store S1 with cost 20$ and quantity 10. </li>
      <li> U2 logs out. </li>
    </ul>
  </li>
</ul>

supported initialization functions: 
- <code>Register(userId, email, firstName, lastName, password)</code>
- <code>CreateSystemManager(email)</code>
- <code>Login(userId, email, password)</code>
- <code>Logout(userId)</code>
- <code>OpenNewStore(userId, StoreName)</code>
- <code>AddItemToStore(userId, storeID, itemName, itemCategory, itemPrice, quantity)</code>
- <code>AppointStoreManager(userId, storeID, anotherEmail)</code>
- <code>AppointStoreOwner(userId, storeID, anotherEmail)</code>
- <code>ReactToJobOffer(userId, storeID, userId2, react)</code>


Documents can be found here:
- Use Cases / Glossary - https://docs.google.com/document/d/1nwZft8kNO3OjLYLuS8tpSzU0Gz247nENy4-0tZwhq_A/edit

- Architecture Diagram - https://drive.google.com/file/d/11LNjJDK7azYbV6blG5h-ucHGxrN6UemM/view

- Class Diagram - https://drive.google.com/file/d/1y6IqBJfDbgYzhdU5ixmV_VKtPHapEDGq/view?usp=sharing

- Wireframe - https://www.figma.com/file/x74k0dMJ9Yw7SZ3jvyAhLH/login-(Community)?node-id=102-2&t=mLByabG70xMQZryD-0
