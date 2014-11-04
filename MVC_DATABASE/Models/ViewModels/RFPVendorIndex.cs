﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFPVendorIndex
    {
        public RFP RFP { get; set; }
        public ICollection<RFP> rfplist { get; set; }

        public RFPINVITE VendorRFPInvite { get; set; }
    }
}