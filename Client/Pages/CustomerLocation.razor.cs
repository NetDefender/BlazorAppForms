using BlazorAppForms.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorAppForms.Client.Pages
{
    public sealed partial class CustomerLocation
    {
        [CascadingParameter()]
        public EditContext? EditContext
        {
            get;
            set;
        }

        [Parameter]
        public List<CustomerLocationTransport>? Locations
        {
            get;
            set;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            EditContext? oldEditContext = EditContext;
            parameters.SetParameterProperties(this);
            if (oldEditContext != EditContext)
            {
                if (oldEditContext != null)
                {
                    oldEditContext.OnFieldChanged -= EditContext_OnFieldChanged;
                }
                if (EditContext != null)
                {
                    EditContext.OnFieldChanged += EditContext_OnFieldChanged;
                }
            }
            await base.SetParametersAsync(ParameterView.Empty);
        }

        private void EditContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            if (e.FieldIdentifier.Model is CustomerLocationTransport location)
            { 
            
            }
        }

        private void Update(CustomerLocationTransport location)
        {
            //EditContext.IsModified()
        }
    }
}