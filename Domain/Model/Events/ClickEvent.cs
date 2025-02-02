﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppReadyGo.Core.Interfaces;

namespace AppReadyGo.Domain.Model.Events
{
    public class ClickEvent : IEvent
    {
        public virtual long Id { get; set; }

        public virtual int ClientX { get; set; }

        public virtual int ClientY { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual long VisitInfoId { get; set; }

        public virtual int Press { get; set; }

        public virtual int Orientation { get; set; }

        public virtual SessionInfoEvent SessionInfoEvent { get; set; }

        public virtual ScrollEvent ScrollEvent { get; set; }
    }
}
