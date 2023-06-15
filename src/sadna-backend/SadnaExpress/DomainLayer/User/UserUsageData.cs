using SadnaExpress.API.SignalR;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer.SModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.User
{
    public class UserUsageData
    {
        private static UserUsageData instance;
        private ConcurrentBag<Visit> usersVisits;

        public ConcurrentBag<Visit> UsersVisits
        {
            get => usersVisits;
            set => usersVisits = value;
        }

        private UserUsageData()
        {
            usersVisits = new ConcurrentBag<Visit>();
        }

        public static UserUsageData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserUsageData();
                }
                return instance;
            }
        }

        public void LoadVisitsData()
        {
            usersVisits = new ConcurrentBag<Visit>(DBHandler.Instance.GetAllVisits());
        }

        public List<int> GetUserUsageData(DateTime fromDate, DateTime toDate)
        {
            List<int> visits = new List<int>();
    
            int guestCounter = 0;
            int memberCounter = 0;
            int managerCounter = 0;
            int founderAndOwnerCounter = 0;
            int adminCounter = 0;

          
            foreach (Visit visit in usersVisits)
            {
                if(DateTime.ParseExact(visit.VisitDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= fromDate &&
                           DateTime.ParseExact(visit.VisitDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) <= toDate)
                {
                    switch (visit.Role)
                    {
                        case "guest":
                            guestCounter++;
                            break;
                        case "member":
                            memberCounter++;
                            break;
                        case "store manager":
                            managerCounter++;
                            break;
                        case "founder or owner":
                            founderAndOwnerCounter++;
                            break;
                        case "system manager":
                            adminCounter++;
                            break;
                    }
                }

            }

            visits.Add(guestCounter);
            visits.Add(memberCounter);
            visits.Add(managerCounter);
            visits.Add(founderAndOwnerCounter);
            visits.Add(adminCounter);

            return visits;
        }

        public void AddGuestVisit(User user)
        {
            DBHandler.Instance.CanConnectToDatabase();

            Visit newVisit = new Visit { UniqueID = Guid.NewGuid(), UserID = user.UserId, Role = user.GetRole(), VisitDate = DateTime.Now.ToString("dd/MM/yyyy") };

            DBHandler.Instance.AddGuestVisit(newVisit);

            UsersVisits.Add(newVisit);

            Logger.Instance.Info($"{nameof(UserUsageData)} {nameof(AddGuestVisit)} {newVisit.Role} {newVisit.UserID} was added to the visit table on {newVisit.VisitDate}");

            SystemActivityNotifier.GetInstance().SystemActivityUpdated();
        }

        public void AddMemberVisit(Guid userId,Member member)
        {
            DBHandler.Instance.CanConnectToDatabase();

            Visit newVisit = new Visit { UniqueID = Guid.NewGuid(), UserID = member.UserId, Role = member.GetRole(), VisitDate = DateTime.Now.ToString("dd/MM/yyyy") };

            DBHandler.Instance.AddMemberVisit(userId,newVisit);

            string todayStr = DateTime.Now.ToString("dd/MM/yyyy");
            Visit toRemove= UsersVisits.FirstOrDefault(v => v.UserID.Equals(userId) && v.Role.Equals("guest") && v.VisitDate.Equals(todayStr));

            var itemsToKeep = usersVisits.Where(item => item.Equals(toRemove) == false);

            // Create a new ConcurrentBag and add the filtered items to it
            ConcurrentBag<Visit> updatedBag = new ConcurrentBag<Visit>(itemsToKeep);

            // Assign the updated bag to the original bag
            usersVisits = updatedBag;

            
            Logger.Instance.Info($"{nameof(UserUsageData)} {nameof(AddMemberVisit)} {toRemove.Role} {newVisit.UserID} was removed from the visit table because guest was logged in as member {newVisit.UserID}");

            if (UsersVisits.Count(item => item.Equals(newVisit)) == 0)
            {  //there isn't a visit of that member today
                UsersVisits.Add(newVisit);

                Logger.Instance.Info($"{nameof(UserUsageData)} {nameof(AddMemberVisit)} {newVisit.Role} {newVisit.UserID} was added to the visit table on {newVisit.VisitDate}");
            }

            SystemActivityNotifier.GetInstance().SystemActivityUpdated();
        }
        

        
    }
}
