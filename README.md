# Sadna

app.config file holds the exteral services url, database connection, system manager details and the start point of the trading system (initialize or not). to change this propery values you should edit to your chosen value, compile the code and run sadna express. Good luck and have fun!

Currently there are 3 possible initial states for the system to start with:
<ul>
  <li> empty state </li>
  <li> state1 
    <ul> 
      <li> Registered users: U1, U2, U3 - members. </li>
      <li> U1 logs in and opens store S1. </li>
      <li> U1 appoints U2 as S1 store owner. </li>
      <li> U2 appoints U3 as S1 store owner. </li>
      <li> U1 logs out. </li>
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




Documents can be found here:
- Use Cases / Glossary - https://docs.google.com/document/d/1nwZft8kNO3OjLYLuS8tpSzU0Gz247nENy4-0tZwhq_A/edit

- Architecture Diagram - https://drive.google.com/file/d/11LNjJDK7azYbV6blG5h-ucHGxrN6UemM/view

- Class Diagram - https://drive.google.com/file/d/1y6IqBJfDbgYzhdU5ixmV_VKtPHapEDGq/view?usp=sharing
