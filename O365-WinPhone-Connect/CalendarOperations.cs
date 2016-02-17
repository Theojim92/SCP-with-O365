// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O365_WinPhone_Connect
{
    class CalendarOperations
    {
        private string _calendarCapability = "Calendar";

        /// <summary>
        /// Gets the details of an event.
        /// </summary>
        /// <param name="eventId">string. The unique identifier of an event selected in the UI.</param>
        /// <returns>A calendar event.</returns>

        internal async Task<IEvent> GetEventDetailsAsync(string eventId)
        {
            // Make sure we have a reference to the calendar client
            var exchangeClient = await AuthenticationHelper.GetOutlookClientAsync(_calendarCapability);

            // This results in a call to the service.
            return await exchangeClient.Me.Calendar.Events.GetById(eventId).ExecuteAsync();
        }

        /// <summary>
        /// Gets a collection of calendar events.
        /// </summary>
        /// <returns>A collection of all calendar events.</returns>
        internal async Task<List<EventViewModel>> GetCalendarEventsAsync()
        {
            // Make sure we have a reference to the Exchange client
            var exchangeClient = await AuthenticationHelper.GetOutlookClientAsync(_calendarCapability);

            List<EventViewModel> returnResults = new List<EventViewModel>();

            var eventsResults = await exchangeClient.Me.Calendar.Events.OrderBy(e => e.Start).ExecuteAsync();
            foreach (IEvent calendarEvent in eventsResults.CurrentPage)
            {
                IEvent thisEvent = await GetEventDetailsAsync(calendarEvent.Id);
                EventViewModel calendarEventModel = new EventViewModel(thisEvent);
                returnResults.Add(calendarEventModel);
            }
            return returnResults;
        }


    }
}

//********************************************************* 
// 
//O365-WinPhone-Connect, https://github.com/OfficeDev/O365-WinPhone-Connect
//
//Copyright (c) Microsoft Corporation
//All rights reserved. 
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:

// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
//********************************************************* 