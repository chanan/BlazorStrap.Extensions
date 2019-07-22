using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorComponentUtilities;
using BlazorStrap.Util;
using BlazorStrap;
using Microsoft.AspNetCore.Components;
using Toolbelt.Blazor.SpeechRecognition;

namespace BlazorStrap.Extensions
{
    public class CodeBSSpeachInput : CodeBSInput , IDisposable
    {
        [Parameter]
        internal string Lable { get; set; }

        protected string Classname =>
        new CssBuilder()
           .AddClass("half-round-left")
           .AddClass("animate", isRunning)
         .Build();

        protected override string Value { get; set; }

        private bool isRunning { get; set; }
        [Parameter]
        protected override EventCallback<string> ValueChanged { get; set; }

        [Inject] SpeechRecognition SpeechRecognition { get; set; }
        public virtual async Task InitializeSpeechRecognition()
        {
            this.SpeechRecognition.Lang = "en-US";
            this.SpeechRecognition.InterimResults = true;
            this.SpeechRecognition.Continuous = false;

            await Task.CompletedTask;
        }

        public async Task ToggleSpeechRecognition()
        {
            if (isRunning)
            {
                this.SpeechRecognition.Result -= OnSpeechRecognized;
                this.SpeechRecognition.Stop();
                isRunning = false;
            }
            else
            {
                this.SpeechRecognition.Result += OnSpeechRecognized;
                this.SpeechRecognition.Start();
                isRunning = true;
            }
            await Task.CompletedTask;
        }
        public async Task StartSpeechRecognition()
        {
            this.SpeechRecognition.Result += OnSpeechRecognized;
            this.SpeechRecognition.Start();

            await Task.CompletedTask;
        }

        public async Task StopSpeechRecognition()
        {
            this.SpeechRecognition.Result -= OnSpeechRecognized;
            this.SpeechRecognition.Stop();

            await Task.CompletedTask;
        }

        protected override async Task OnInitAsync()
        {
            await this.InitializeSpeechRecognition();
        }

        protected virtual async Task OnFocus()
        {
            await this.StartSpeechRecognition();
        }

        protected virtual async Task OnClick()
        {
            await this.StartSpeechRecognition();
        }

        protected virtual async Task OnBlur()
        {
            await this.StopSpeechRecognition();
        }

        protected virtual void OnSpeechRecognized(object sender, SpeechRecognitionEventArgs args)
        {
            this.Value = string.Join(" ", args.Results.Select(e => e.Items[0].Transcript));
            isRunning = false;
            this.StateHasChanged();
        }

        public virtual void Dispose()
        {
            this.StopSpeechRecognition().Wait();
        }
    }
}
