using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebRole.Models
{
    public class MyAppointment : TableEntity
    {
        public MyAppointment()
        { }

        public MyAppointment(string userId)
        {
            this.PartitionKey = userId;
            this.RowKey = Guid.NewGuid().ToString();
        }

        //public string UserId { get; set; }
        //public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}