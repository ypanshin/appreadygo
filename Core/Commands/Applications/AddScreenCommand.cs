﻿using System.Collections.Generic;
using AppReadyGo.Core.Entities;

namespace AppReadyGo.Core.Commands.Applications
{
    public class AddScreenCommand : ICommand<long>
    {
        public int ApplicationId { get; protected set; }
        public string Path { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public string FileExtention { get; protected set; }

        public AddScreenCommand(int applicationId, string path, int width, int height, string fileExtention)
        {
            this.ApplicationId = applicationId;
            this.Path = path;
            this.Width = width;
            this.Height = height;
            this.FileExtention = fileExtention;
        }

        public IEnumerable<ValidationResult> Validate(IValidationContext validation)
        {
            if (string.IsNullOrEmpty(this.Path))
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "Command must have Path parameter.");
            }

            if (this.Width <= 0)
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "Command must have Width parameter.");
            }

            if (this.Height <= 0)
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "Command must have Height parameter.");
            }

            if (string.IsNullOrEmpty(this.FileExtention))
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "Command must have FileExtention parameter.");
            }
        }

        public IEnumerable<ValidationResult> ValidatePermissions(ISecurityContext security)
        {
            yield break;
        }
    }
}
