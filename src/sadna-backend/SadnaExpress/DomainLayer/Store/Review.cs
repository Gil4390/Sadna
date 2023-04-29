using System;

namespace SadnaExpress.DomainLayer.Store
{
    public class Review
    {
        private Store store;
        public Store Store {get=>store;}
        private Item item;
        public Item Item {get=>item;}
        private string reviewText;
        public string ReviewText {get=>reviewText; set => reviewText = value;}
        private Guid reviewerID;
        public Guid ReviewerID {get=>reviewerID;}
        
        public Review(Guid reviewerID, Store store, Item item, string reviewText)
        {
            this.store = store;
            this.item = item;
            this.reviewText = reviewText;
            this.reviewerID = reviewerID;
        }
    }
}