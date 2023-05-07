using SadnaExpress.DomainLayer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer.SModels
{
    internal class SReview
    {
        private Guid itemID;
        private Guid reviewerID;
        private Guid storeID;
        private string reviewText;

        public SReview(Review review)
        {
            this.itemID = review.Item.ItemID;
            this.reviewerID = review.ReviewerID;
            this.storeID = review.Store.StoreID;
            this.reviewText = review.ReviewText;
        }
    }
}
