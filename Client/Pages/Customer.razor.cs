using BlazorAppForms.Client.Extensions;
using BlazorAppForms.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace BlazorAppForms.Client.Pages
{
    public sealed partial class Customer : IDisposable
    {
        #region fields
        private CustomerTransport? _customer;
        private EditContext? _editContext;
        private ValidationMessageStore? _errors;
        private string _lastInputResponseMessage;
        #endregion

        #region parameters
        [Parameter]
        public int IdCustomer
        {
            get;
            set;
        }
        #endregion

        #region properties
        private bool IsEdit => _customer != null && _customer.IdCustomer > 0;
        #endregion

        #region methods
        protected override async Task OnInitializedAsync()
        {
            
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            int oldIdCustomer = IdCustomer;
            parameters.SetParameterProperties(this);
            
            if (oldIdCustomer != IdCustomer)
            {
                if (_editContext != null)
                {
                    _editContext.OnFieldChanged -= editContext_OnFieldChanged;
                    _editContext.OnValidationRequested -= editContext_OnValidationRequested;
                }
                _customer = await Http.GetFromJsonWithOptionsAsync<CustomerTransport>($"Customer/{IdCustomer}")
                    .ConfigureAwait(false);
                if (_customer != null)
                {
                    _editContext = new EditContext(_customer)!;
                    _errors = new ValidationMessageStore(_editContext)!;
                    _editContext.OnFieldChanged += editContext_OnFieldChanged!;
                    _editContext.OnValidationRequested += editContext_OnValidationRequested!;
                    StateHasChanged();
                }
            }
        }

        public bool Validate() => ValidateCustomer(_customer, true);

        private void Validate(FieldIdentifier identifier)
        {
            if (_errors != null && _editContext != null && _customer != null)
            {
                _errors.Clear();
                var model = identifier.Model;

                _ = model switch
                {
                    CustomerTransport customer => ValidateCustomer(customer, true),
                    CustomerLocationTransport location => ValidateCustomerLocation(location, false)
                        && ValidateCustomer(_customer, true),
                    _ => throw new InvalidOperationException()
                };

                _editContext.NotifyValidationStateChanged();
            }
        }

        private bool ValidateCustomer(CustomerTransport customer, bool validateChild = false)
        {
            bool isValid = true;
            if (_errors != null && customer != null && _editContext != null)
            {
                if (string.IsNullOrWhiteSpace(customer.Name))
                {
                    _errors.Add(_editContext.Field(nameof(CustomerTransport.Name)), "The customer name is required");
                    isValid = false;
                }

                if (validateChild)
                {
                    foreach (CustomerLocationTransport location in customer.CustomerLocation)
                    {
                        isValid = ValidateCustomerLocation(location, validateChild) && isValid;
                    }
                }
            }

            return isValid;
        }

        private bool ValidateCustomerLocation(CustomerLocationTransport location, bool validateChild = false)
        {
            bool isValid = true;
            if (_errors != null && _customer != null && _editContext != null)
            {
                if (string.IsNullOrWhiteSpace(location.Street))
                {
                    _errors.Add(_editContext.Field(nameof(CustomerLocationTransport.Street)), "The street is required in location");
                    isValid = false;
                }

                if (validateChild)
                {
                    // Add some validations
                }
            }

            return isValid;
        }

        private async Task LoadFiles(InputFileChangeEventArgs e)
        {
            if (e.File != null)
            {
                //Current limit is aprox 500 Mb. The buffering is done in the browser that produces a OutOfMemoryException
                using MultipartFormDataContent content = new();
                using StreamContent fileContent = new(e.File.OpenReadStream(1024L * 1024 * 1024), 10 * 1024 * 1024);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(e.File.ContentType);
                content.Add(content: fileContent, name: "\"file\"", fileName: e.File.Name);
                using HttpResponseMessage response = await Http.PostAsync("Customer/upload", content)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                _lastInputResponseMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Used when the Form is submitted
        /// </summary>
        /// <returns></returns>
        private Task Update()
        {
            if (IsEdit)
            {

            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_editContext != null)
            {
                _editContext.OnFieldChanged -= editContext_OnFieldChanged;
                _editContext.OnValidationRequested -= editContext_OnValidationRequested;
            }
        }
        #endregion

        #region events
        private void editContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e) => Validate();
        private void editContext_OnFieldChanged(object? sender, FieldChangedEventArgs e) => Validate(e.FieldIdentifier);
        #endregion
    }
}