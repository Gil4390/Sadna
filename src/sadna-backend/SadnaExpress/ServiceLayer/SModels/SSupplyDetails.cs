namespace SadnaExpress.ServiceLayer.SModels
{
    public class SSupplyDetails
    {
        private string name;
        
        public string Name{get=>name;}
        
        private string address;
        
        public string Address{get=>address;}
        
        private string city;
        
        public string City{get=>city;}
        
        private string country;
        
        public string Country{get=>country;}
        
        private string zip;
        
        public string Zip{get=>zip;}

        public SSupplyDetails(string name, string address, string city, string country, string zip)
        {
            this.name = name;
            this.address = address;
            this.city = city;
            this.country = country;
            this.zip = zip;
        }

        public bool ValidationSettings()
        {
            return name != "-" && address != "-" && city != "-" && country != "-" &&
                   zip != "-";
        }
    }
}