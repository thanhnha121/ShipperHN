namespace ShipperHN.Business
{
    class NotificationBusiness
    {
        private readonly ShipperHNDBcontext _shipperHndBcontext;
        public NotificationBusiness(ShipperHNDBcontext shipperHndBcontext)
        {
            _shipperHndBcontext = shipperHndBcontext;
        }
    }
}
