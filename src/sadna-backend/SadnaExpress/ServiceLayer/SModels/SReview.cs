using SadnaExpress.DomainLayer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer.SModels
{
    public class SReview
    {
        private Guid itemID;
        private Guid reviewerID;
        private Guid storeID;
        private string reviewText;

        public SReview(Review review)
        {
            this.ItemID = review.Item.ItemID;
            this.ReviewerID = review.ReviewerID;
            this.StoreID = review.Store.StoreID;
            this.ReviewText = review.ReviewText;
        }

        public Guid ItemID { get => itemID; set => itemID = value; }
        public Guid ReviewerID { get => reviewerID; set => reviewerID = value; }
        public Guid StoreID { get => storeID; set => storeID = value; }
        public string ReviewText { get => reviewText; set => reviewText = value; }
    }
}
