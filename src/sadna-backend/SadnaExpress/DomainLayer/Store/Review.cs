using System;
using System.ComponentModel.DataAnnotations;

namespace SadnaExpress.DomainLayer.Store
{
    public class Review
    {
        [Key]
        public Guid ReviewID { get; set; }

        private Store store;
        public Store Store { get => store; set => store = value; }

        private Item item;
        public Item Item { get => item; set => item = value; }

        private string reviewText;
        public string ReviewText {get=>reviewText; set => reviewText = value;}

        private Guid reviewerID;
        public Guid ReviewerID {get=>reviewerID; set => reviewerID = value; }
        
        public Review(Guid reviewerID, Store store, Item item, string reviewText)
        {
            this.reviewerID = Guid.NewGuid();
            this.store = store;
            this.item = item;
            this.reviewText = reviewText;
            this.reviewerID = reviewerID;
        }

        public Review()
        {

        }

    }
}