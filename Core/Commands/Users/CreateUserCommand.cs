﻿using AppReadyGo.Core.Entities;
using System.Collections.Generic;

namespace AppReadyGo.Core.Commands.Users
{

    public abstract class CreateUserCommand
    {
        public string Email { get; protected set; }

        public string Password { get; protected set; }

        public bool ThirdParty { get; set; }

        protected CreateUserCommand(string email, string password)
        {
            this.Email = email;
            this.Password = password;
            this.ThirdParty = false;
        }

        protected CreateUserCommand(string email)
        {
            this.Email = email;
            this.ThirdParty = true;
        }

        public virtual IEnumerable<ValidationResult> ValidatePermissions(ISecurityContext security)
        {
            yield break;
        }

        public virtual IEnumerable<ValidationResult> Validate(IValidationContext validation)
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                yield return new ValidationResult(ErrorCode.WrongEmail, "The command must have an Email parameter.");
            }

            if (!validation.IsCorrectEmail(this.Email))
            {
                yield return new ValidationResult(ErrorCode.WrongEmail, "The email is wrong.");
            }
        }
    }
}
