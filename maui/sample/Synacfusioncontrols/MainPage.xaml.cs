namespace Synacfusioncontrols
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new gridEntries());
        }

        private void numericButton_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new numeric());
        }

        private void maskButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new mask());
        }

        private void comboButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new combobox());
        }

        private void autoButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new autocomplete());
        }
    }

}
