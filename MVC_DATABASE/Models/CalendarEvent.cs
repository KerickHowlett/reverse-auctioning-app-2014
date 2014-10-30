﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models
{
    public class CalendarEvent
    {
        public string title;
        public bool allDay;
        public DateTime start;
        public string color;

        public CalendarEvent(string newTitle, bool newAllDay, DateTime newStart, string newColor)
        {
            title = newTitle;
            allDay = newAllDay;
            start = newStart;
            color = newColor;
        }

        public CalendarEvent()
        { }
    }
}