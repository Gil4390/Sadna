using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingCart
    {
        private List<ShoppingBasket> baskets;
        // list for saved items

        public ShoppingCart()
        {
            baskets = new List<ShoppingBasket>();
        }
        public ShoppingCart(List<ShoppingBasket> newbaskets)
        {
            baskets = newbaskets;
        }
        
        public bool addInventoryToCart(Inventory inv, int stock)
        {
            // add product to existing basket from the same store
            foreach (ShoppingBasket sb in baskets)
            {
                if (sb.getStore() == inv.getStore())
                {
                    sb.addItem(inv, stock);
                    return true;
                }
            }

            // no basket store match this item then add new basket and the item to it 
            ShoppingBasket b = new ShoppingBasket(inv.getStore(), new Dictionary<Inventory, int>());
            b.addItem(inv, stock);
            this.baskets.Add(b);
            return true;

        }

        // functions to implement

        // getters


        // add / remove basket

        // add / remove items

        //  purchase basket

        // purchase items

        // get num of items

        // edit stock of item

        // save items for purchase




    }
}