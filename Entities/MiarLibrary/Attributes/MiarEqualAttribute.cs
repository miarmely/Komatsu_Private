﻿using Entities.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Entities.MiarLibrary.Attributes
{
    public class MiarEqualAttribute : ValidationAttribute
    {
        private readonly object[] _values;
        private readonly string _displayNameInTR;
        private readonly string _displayNameInEN;

        public MiarEqualAttribute(
            object[] values,
            string displayNameInTR,
            string displayNameInEN)
        {
            _values = values;
            _displayNameInTR = displayNameInTR;
            _displayNameInEN = displayNameInEN;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            #region when value is null (return)
            if (value == null)
                return null;
            #endregion

            #region when value not exists valid values (throw)			
            if (!_values.Contains(value))
            {
                #region set error message
                var validValuesInString = string.Join(',', _values);
                var errorMessage = new
                {
                    TR = $"\"{_displayNameInTR}\" geçerli değil." +
                        $" Geçerli değerler: [ {validValuesInString} ]",

                    EN = $"\"{_displayNameInEN}\" not valid." +
                        $" Valid values = [ {validValuesInString} ]"
                };
                #endregion

                #region throw error
                var first2CharOfDisplayNameInEN = _displayNameInEN.Substring(0, 2);

                throw new ExceptionWithMessage(
                    400,
                    $"FE-NV-{first2CharOfDisplayNameInEN}",
                    $"Format Error - Not Valid - {_displayNameInTR}",
                    JsonSerializer.Serialize(errorMessage));
                #endregion
            }
            #endregion

            return ValidationResult.Success;
        }
    }
}
