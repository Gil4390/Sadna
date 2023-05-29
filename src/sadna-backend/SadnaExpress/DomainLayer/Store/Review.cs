using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SadnaExpress.DomainLayer.Store
{
    public class Review
    {
        [Key]
        public Guid ReviewID { get; set; }


        private Store store;
        [NotMapped]
        public Store Store { get => store; set => store = value; }

        private Guid storeID;
        public Guid StoreID { get => storeID; set => storeID = value; }

        private Item item;
        [NotMapped]
        public Item Item { get => item; set => item = value; }


        private Guid itemID;
        public Guid ItemID { get => itemID; set => itemID = value; }

        private string reviewText;
        public string ReviewText {get=>reviewText; set => reviewText = value;}

        private Guid reviewerID;
        public Guid ReviewerID {get=>reviewerID; set => reviewerID = value; }
        
        public Review(Guid reviewerID, Store store, Item item, string reviewText)
        {
            this.ReviewID = Guid.NewGuid();
            this.store = store;
            this.item = item;
            this.reviewText = reviewText;
            this.reviewerID = reviewerID;
            this.ItemID = item.ItemID;
            this.storeID = store.StoreID;
        }

        public Review()
        {

        }

    }
}