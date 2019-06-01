namespace Railway.DeliveryCargo.Data.EF.Dto
{
    using System;
    using Interfaces;

    [Serializable]
    public class DispatchEntity : IDomainEntity
    {
        public int Id { get; set; }
    }
}
