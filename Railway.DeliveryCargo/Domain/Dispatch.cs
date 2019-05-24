using System;

namespace Railway.DeliveryCargo.Domain
{
    public class Dispatch
    {
        public int Id { get; set; }
        public int State { get; set; }
        public int Operation { get; set; }
        public DateTime OperationDate { get; set; }
        public string OperationNumber { get; set; }
    }
}
