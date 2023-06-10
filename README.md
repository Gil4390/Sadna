# Sadna

Team members:

Gil khais - 207181629

Noga Schawartz - 207181629

Shay kresner - 207181629

Tal Galmor - 207181629

Radwan Ganen - 207181629

Dina Agapov - 207181629


State Configuration File:


The state configuration file for the Sadna Trading System is a JSON file containing an array of objects. Each object represents a system function to be executed during startup. The object consists of two fields: "function" and "params".

"function" (string): Specifies the name of the function to be executed.

"params" (array): Contains the parameters required for the corresponding function.


![image](https://github.com/Gil4390/Sadna/assets/76035272/75c7dc29-a937-4f2d-b57b-6fd3905e5bf0)

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
      <li> Registered users: U1 - admin, U2, U3, U4, U5, U6 - members. </li>
      <li> U2 logs in and opens store S1. </li>
      <li> U2 adds item "Bamba" to store S1 with cost 30$ and quantity 20. </li>
      <li> U2 appoints U3 as S1 store manager with permission to manage inventory. </li>
      <li> U2 appoints U4 and U5 as S1 store owners. </li>  
      <li> U2 logs out. </li>
    </ul>
  </li>
</ul>


app.config file holds the exteral services url, database connection, system manager details and the start point of the trading system (initialize or not). to change this propery values you should edit to your chosen value, compile the code and run sadna express. Good luck and have fun!

Documents can be found here:
- Use Cases / Glossary - https://docs.google.com/document/d/1nwZft8kNO3OjLYLuS8tpSzU0Gz247nENy4-0tZwhq_A/edit

- Architecture Diagram - https://drive.google.com/file/d/11LNjJDK7azYbV6blG5h-ucHGxrN6UemM/view

- Class Diagram - https://drive.google.com/file/d/1y6IqBJfDbgYzhdU5ixmV_VKtPHapEDGq/view?usp=sharing

- Wireframe - https://www.figma.com/file/x74k0dMJ9Yw7SZ3jvyAhLH/login-(Community)?node-id=102-2&t=mLByabG70xMQZryD-0
