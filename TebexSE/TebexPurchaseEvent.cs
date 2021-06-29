using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TebexSE
{
    public delegate void TebexPurchase(string details);
    public class TebexPurchaseEvent
    {
        public event TebexPurchase TebexPurchaseReceived;

        public void purchaseReceived(string details)
        {
            var handler = TebexPurchaseReceived;
            handler?.Invoke(details);
        }
    }
}
