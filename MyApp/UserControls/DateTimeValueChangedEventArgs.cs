using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.UserControls
{
    /// <summary>
    /// Provides data for the DatePicker and TimePicker's ValueChanged event.
    /// </summary>
    public class DateTimeValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeValueChangedEventArgs class.
        /// </summary>
        /// <param name="oldDateTime">Old DateTime value.</param>
        /// <param name="newDateTime">New DateTime value.</param>
        public DateTimeValueChangedEventArgs(DateTime? oldDateTime, DateTime? newDateTime)
        {
            OldDateTime = oldDateTime;
            NewDateTime = newDateTime;
        }

        /// <summary>
        /// Gets or sets the old DateTime value.
        /// </summary>
        public DateTime? OldDateTime { get; private set; }

        /// <summary>
        /// Gets or sets the new DateTime value.
        /// </summary>
        public DateTime? NewDateTime { get; private set; }
    }
}
