using BlazorAppForms.Client.Extensions;
using BlazorAppForms.Shared;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorAppForms.Client.Pages
{
    public sealed partial class Customer
    {
        private CustomerTransport _customer;
        private EditContext _editContext;
        private ValidationMessageStore _errors;
        protected override async Task OnInitializedAsync()
        {
            _customer = await Http.GetFromJsonWithOptionsAsync<CustomerTransport>("Customer/1").ConfigureAwait(false);
            _editContext = new EditContext(_customer);
            _errors = new ValidationMessageStore(_editContext);
            _editContext.OnFieldChanged += editContext_OnFieldChanged;
            _editContext.OnValidationRequested += editContext_OnValidationRequested;
        }

        private void editContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidateAll();
        }

        private void editContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            Validate(e.FieldIdentifier);
        }

        public bool ValidateAll()
        {
            return ValidateCustomer(_customer, true);
        }

        private void Validate(FieldIdentifier identifier)
        {
            _errors.Clear();
            var model = identifier.Model;

            bool isValid = model switch { 
                CustomerTransport customer => ValidateCustomer(customer, true),
                CustomerLocationTransport location => ValidateCustomerLocation(location, false) && ValidateCustomer(_customer, true)
            };

            _editContext.NotifyValidationStateChanged();
        }

        private bool ValidateCustomer(CustomerTransport customer, bool validateChild = false)
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(_customer.Name))
            {
                _errors.Add(_editContext.Field(nameof(CustomerTransport.Name)), "The customer name is required");
                isValid = false;
            }

            if (validateChild)
            {
                foreach (var location in _customer.CustomerLocation)
                {
                    isValid = ValidateCustomerLocation(location, validateChild) && isValid;
                }
            }

            return isValid;
        }

        private bool ValidateCustomerLocation(CustomerLocationTransport location, bool validateChild = false)
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(location.Street))
            {
                _errors.Add(_editContext.Field(nameof(CustomerLocationTransport.Street)), "The street is required in location");
                isValid = false;
            }

            if(validateChild)
            {

            }

            return isValid;
        }

        private Task Update()
        {
            if (IsEdit)
            {
            }
            return Task.CompletedTask;
        }

        private bool IsEdit => _customer != null && _customer.IdCustomer > 0;


    }
}