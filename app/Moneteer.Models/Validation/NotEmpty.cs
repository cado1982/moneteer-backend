using System;
using System.ComponentModel.DataAnnotations;

namespace Moneteer.Models.Validation
{
    public class NotEmpty : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null) 
            {
                return true;
            }

            switch (value)
            {
                case Guid guid:
                    return guid != Guid.Empty;
                default:
                    return true;
            }
        }
    }
}