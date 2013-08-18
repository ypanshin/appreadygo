﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppReadyGo.Core.Entities;

namespace AppReadyGo.Core.Commands.API
{
    public class AddPackageCommand : ICommand<long>
    {
        public int ApplicationId { get; protected set; }
        public Location Location { get; protected set; }
        public string OS { get; protected set; }
        public string Browser { get; protected set; }
        public int ScreenWidth { get; protected set; }
        public int ScreenHeight { get; protected set; }
        public SystemInfo SystemInfo { get; protected set; }
        public IEnumerable<Session> Sessions { get; protected set; }

        public AddPackageCommand(int appId, 
            Location location,
            string os, 
            string browser, 
            int screenWidth, 
            int screenHeight,
            SystemInfo systemInfo,
            IEnumerable<Session> sessions)
        {
            this.ApplicationId = appId;
            this.Location = location;
            this.OS = os;
            this.Browser = browser;
            this.ScreenHeight = screenHeight;
            this.ScreenWidth = screenWidth;
            this.SystemInfo = systemInfo;
            this.Sessions = sessions;
        }

        public IEnumerable<ValidationResult> Validate(IValidationContext validation)
        {
            if (!this.Sessions.Any())
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "Sessions must to be at least one.");
            }

            if (this.ScreenWidth <= 0)
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "ScreenWidth must to be positive and greate than zero");
            }

            if (this.ScreenHeight <= 0)
            {
                yield return new ValidationResult(ErrorCode.WrongParameter, "ScreenHeight must to be positive and greate than zero");
            }
        }

        public IEnumerable<ValidationResult> ValidatePermissions(ISecurityContext security)
        {
            yield break;
        }

        public class Session
        {
            public string Path { get; set; }

            public int ClientWidth { get; set; }

            public int ClientHeight { get; set; }
            /// <summary>
            /// Start this session date
            /// </summary>
            public DateTime StartDate { get; set; }
            /// <summary>
            /// Finish this session date
            /// </summary>
            public DateTime CloseDate { get; set; }
            /// <summary>
            /// Click/Touches 
            /// </summary>
            public IEnumerable<Click> Clicks { get; set; }
            /// <summary>
            /// Scrolls for this session
            /// </summary>
            public IEnumerable<Scroll> Scrolls { get; set; }
            /// <summary>
            /// Parts on the main view data 
            /// </summary>
            public IEnumerable<ViewPart> ScreenViewParts { get; set; }
        }

        public class Click
        {
            public int ClientX { get; set; }

            public int ClientY { get; set; }

            public DateTime Date { get; set; }

            public int Press { get; set; }

            public int Orientation { get; set; }
        }

        public class Scroll
        {
            public Click FirstTouch { get; set; }
            /// <summary>
            /// Finish scrolling property
            /// </summary>
            public Click LastTouch { get; set; }
        }

        public class ViewPart
        {
            public int ScrollTop { get; set; }

            public int ScrollLeft { get; set; }

            public long TimeSpan { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime FinishDate { get; set; }

            public int Orientation { get; set; }
        }
    }
}