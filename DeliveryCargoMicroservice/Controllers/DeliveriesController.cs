using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Railway.DeliveryCargo.Domain;

namespace Railway.DeliveryCargo.API.Controllers
{
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Dispatch>> Get()
        {
            return new[]
            {
                new Dispatch
                {
                     Id = 1,
                     Operation = 2,
                     OperationDate = new DateTime(2019, 1, 1),
                     OperationNumber = "1"
                }
            };
        }
    }
}