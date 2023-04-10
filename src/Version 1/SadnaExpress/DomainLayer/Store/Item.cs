namespace SadnaExpress.DomainLayer.Store
{
    public class Item
    {
        private static int _id = 0;
        private int id;
        private string name;
        private string category;
        private double price;
        private int rating;

        public Item(string name, string category, double price)
        {
            this.name = name;
            this.category = category;
            this.price = price;
            this.rating = 0;
            this.id = _id;
            _id++;
        }

        //setters
        public void setName(string name)
        {
            this.name = name;
        }

        public void setCategory(string category)
        {
            this.category = category;
        }

        public void setPrice(double price)
        {
            this.price = price;
        }

        //getters 
        public string getName()
        {
            return name;
        }

        public string getCategory()
        {
            return category;
        }

        public double getPrice()
        {
            return price;
        }

        public void setRating(int rating)
        {
            this.rating = rating;
        }

        public int getItemRating()
        {
            return this.rating;
        }

        public int getId()
        {
            return this.id;
        }

    }
}